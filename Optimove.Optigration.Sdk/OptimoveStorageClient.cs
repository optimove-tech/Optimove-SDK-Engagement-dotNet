using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Newtonsoft.Json;
using Optimove.Optigration.Sdk.Exceptions;
using Optimove.Optigration.Sdk.Interfaces;
using Optimove.Optigration.Sdk.Models;
using SolTechnology.Avro;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Optimove.Optigration.Sdk
{
	/// <summary>
	/// Incapsulates Optimove Cloud Storage (OCS) integration logic
	/// </summary>
	public class OptimoveStorageClient: IOptimoveStorageClient
	{
		private const string MetadataFileNamePrefix = "METADATA";
		private StorageClient _googleStorageClient;
		private string _bucketName = String.Empty;
		private string _folderPath = String.Empty;
		private string _decryptionKey = String.Empty;

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
			_folderPath = settings.FolderPath.Substring(_bucketName.Length + 1);
		}

		/// <summary>
		/// Retrieves customers Metadata.
		/// </summary>
		/// <returns>Customers metadata object.</returns>
		public async Task<Metadata> GetMetadata()
		{
			try
			{
				var fileInfo = GetFiles(_bucketName, $"{_folderPath}/{MetadataFileNamePrefix}").Single();
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
		/// Retrieves customers collection.
		/// </summary>
		/// <returns>Customers collection.</returns>
		public async Task<List<ExpandoObject>> GetCustomers()
		{
			try
			{
				var customers = new List<ExpandoObject>();
				var files = GetFiles(_bucketName, _folderPath).Where(f => !f.Name.StartsWith($"{_folderPath}/{MetadataFileNamePrefix}"));
				foreach (var file in files)
				{
					var avro = await DownloadFile(_bucketName, file.Name, _decryptionKey);
					var json = AvroConvert.Avro2Json(avro);
					var batch = JsonConvert.DeserializeObject<List<ExpandoObject>>(json);
					customers.AddRange(batch);
				}
				return customers;
			}
			catch(Exception ex)
			{
				throw new OptimoveException(ex.Message, ex);
			}
		}

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
			var files = _googleStorageClient.ListObjects(bucketName, filePath, new ListObjectsOptions()).ToList();
			return files;
		}
	}
}
