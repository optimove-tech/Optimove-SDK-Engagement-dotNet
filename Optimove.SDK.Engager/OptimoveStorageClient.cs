using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Optimove.SDK.Engager.Exceptions;
using Optimove.SDK.Engager.Interfaces;
using Optimove.SDK.Engager.Models;
using SolTechnology.Avro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Optimove.SDK.Engager
{
	/// <summary>
	/// Incapsulates Optimove Cloud Storage (OCS) integration logic
	/// </summary>
	public class OptimoveStorageClient: IOptimoveStorageClient
	{
		#region Constants

		private const string MetadataFileNamePrefix = "METADATA";
		private const string AvroFileExtenssion = ".avro";
		private const string CustomersSubFolder = "customers";

		#endregion

		#region Fields

		private StorageClient _googleStorageClient;
		private string _bucketName = String.Empty;
		private string _rootFolderPath = String.Empty;
		private string _decryptionKey = String.Empty;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs OptimoveStorageClient object.
		/// </summary>
		/// <param name="settings">Optimove Cloud Storage Client configuration settings.</param>
		public OptimoveStorageClient(OptimoveStorageClientSettings settings)
		{
			var base64EncodedBytes = System.Convert.FromBase64String(settings.ServiceAccount);
			var jsonCredentials = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
			var credential = GoogleCredential.FromJson(jsonCredentials);
			_googleStorageClient = StorageClient.Create(credential);
			_decryptionKey = settings.DecryptionKey;
			_bucketName = settings.FolderPath.Substring(0, settings.FolderPath.IndexOf('/'));
			_rootFolderPath = settings.FolderPath.Substring(_bucketName.Length + 1);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Retrieves customers Metadata.
		/// </summary>
		/// <returns>Customers metadata object.</returns>
		public async Task<Metadata> GetMetadata()
		{
			try
			{
				var fileInfo = GetFiles(_bucketName, $"{_rootFolderPath}/{MetadataFileNamePrefix}").Single();
				if (fileInfo == null)
				{
					return null;
				}
				var data = await DownloadFile(_bucketName, fileInfo.Name, _decryptionKey);
				var metadata = AvroConvert.Deserialize<Metadata>(data);
				return metadata;
			}
			catch(Exception ex)
			{
				throw new OptimoveException(ex.Message, ex);
			}
		}

		/// <summary>
		/// Get Customer batches.
		/// </summary>
		/// <returns>Batches collection.</returns>
		public List<CustomersBatch> GetCustomerBatches()
		{
			try
			{
				var batches = new List<CustomersBatch>();
				var files = GetFiles(_bucketName, $"{_rootFolderPath}/{CustomersSubFolder}/");
				foreach (var file in files)
				{
					var batch = new CustomersBatch()
					{
						Name = file.Name,
						Id = file.Id.Substring(file.Id.LastIndexOf("/") + 1),
					};
					batches.Add(batch);
				}
				return batches;
			}
			catch (Exception ex)
			{
				throw new OptimoveException(ex.Message, ex);
			}
		}

		/// <summary>
		/// Retrieves customers collection.
		/// </summary>
		/// <returns>Customers collection.</returns>
		public async Task<List<T>> GetAllCustomers<T>()
		{
			try
			{
				var path = $"{_rootFolderPath}/{CustomersSubFolder}/";
				var customers = await GetCustomers<T>(path);
				return customers;
			}
			catch(Exception ex)
			{
				throw new OptimoveException(ex.Message, ex);
			}
		}

		/// <summary>
		/// Retrieves customers by batch.
		/// </summary>
		/// <returns>Customers collection.</returns>
		public async Task<List<T>> GetCustomersByBatch<T>(CustomersBatch batch)
		{
			try
			{
				var customers = await GetCustomers<T>(batch.Name);
				return customers;
			}
			catch (Exception ex)
			{
				throw new OptimoveException(ex.Message, ex);
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Download file.
		/// </summary>
		/// <param name="bucketName">Bucket name.</param>
		/// <param name="filePath">File path.</param>
		/// <param name="decryptionKey">Decryption key.</param>
		/// <returns>File content.</returns>
		private async Task<byte[]> DownloadFile(string bucketName, string filePath, string decryptionKey = null)
		{
			using (var stream = new MemoryStream())
			{
				var downloadOptions = new DownloadObjectOptions();
				if (!String.IsNullOrEmpty(decryptionKey))
				{
					downloadOptions.EncryptionKey = EncryptionKey.Create(Convert.FromBase64String(decryptionKey));
				}
				await _googleStorageClient.DownloadObjectAsync(bucketName, filePath, stream, downloadOptions);
				stream.Position = 0;
				return stream.ToArray();
			}
		}

		/// <summary>
		/// Gets information about file.
		/// </summary>
		/// <param name="bucketName">Bucket name.</param>
		/// <param name="filePath">File path.</param>
		/// <returns>File metadata.</returns>
		private List<Google.Apis.Storage.v1.Data.Object> GetFiles(string bucketName, string filePath)
		{
			var files = _googleStorageClient.ListObjects(bucketName, filePath, null)
											.Where(f => f.Name.EndsWith(AvroFileExtenssion));
			return files.ToList();
		}

		/// <summary>
		/// Get customers by avro file prefix
		/// </summary>
		/// <typeparam name="T">Reference to the customer type.</typeparam>
		/// <param name="prefix">Avro file prefix</param>
		/// <returns>Customers collection.</returns>
		private async Task<List<T>> GetCustomers<T>(string prefix)
		{
			var customers = new List<T>();
			var files = GetFiles(_bucketName, prefix);
			foreach (var file in files)
			{
				var avro = await DownloadFile(_bucketName, file.Name, _decryptionKey);
				var batch = AvroConvert.Deserialize<List<T>>(avro);
				customers.AddRange(batch);
			}
			return customers;
		}

		#endregion
	}
}
