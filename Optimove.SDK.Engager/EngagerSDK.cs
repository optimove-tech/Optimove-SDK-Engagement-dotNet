﻿using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Newtonsoft.Json;
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
	public class EngagerSDK: IEngagerSDK
	{
		#region Fields

		private StorageClient _googleStorageClient;
		private EngagerSDKSettings _settings;
		private Dictionary<int, CustomersBatch> _customersBatchs;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs OptimoveStorageClient object.
		/// </summary>
		/// <param name="settings">Optimove Cloud Storage Client configuration settings.</param>
		public EngagerSDK(EngagerSDKSettings settings)
		{
			Initialize(settings);
		}

		/// <summary>
		/// Constructs OptimoveStorageClient object.
		/// </summary>
		/// <param name="jsonSettings">Optimove Cloud Storage Client configuration settings.</param>
		public EngagerSDK(string jsonSettings)
		{
			var settings = JsonConvert.DeserializeObject<EngagerSDKSettings>(jsonSettings);
			Initialize(settings);
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
				var fileInfo = GetFiles(_settings.BucketName, _settings.MetadataFilePath).Single();
				if (fileInfo == null)
				{
					return null;
				}
				var data = await DownloadFile(_settings.BucketName, _settings.MetadataFilePath, _settings.DecryptionKey);
				var dataString = System.Text.Encoding.UTF8.GetString(data);
				var metadata = JsonConvert.DeserializeObject<Metadata>(dataString);
				return metadata;
			}
			catch(Exception ex)
			{
				throw new OptimoveException(ex.Message, ex);
			}
		}

		/// <summary>
		/// Get Customer batches number.
		/// </summary>
		/// <returns>Batches number.</returns>
		public int GetCustomerBatchesNumber()
		{
			try
			{
				_customersBatchs = new Dictionary<int, CustomersBatch>();
				var files = GetFiles(_settings.BucketName, _settings.CustomersFolderPath);
				for (int i=0; i< files.Count; i++)
				{
					var batch = new CustomersBatch()
					{
						Name = files[i].Name,
						Id = files[i].Id.Substring(files[i].Id.LastIndexOf("/") + 1),
					};
					_customersBatchs.Add(i, batch);
				}
				return _customersBatchs.Count;
			}
			catch (Exception ex)
			{
				throw new OptimoveException(ex.Message, ex);
			}
		}

		/// <summary>
		/// Retrieves customers by batch.
		/// </summary>
		/// <returns>Customers collection.</returns>
		public async Task<List<T>> GetCustomersByBatchId<T>(int id)
		{
			try
			{
				var customers = await GetCustomers<T>(_customersBatchs[id].Name);
				return customers;
			}
			catch (Exception ex)
			{
				throw new OptimoveException(ex.Message, ex);
			}
		}

		/// <summary>
		/// Retrieves customers by batch.
		/// </summary>
		/// <returns>Customers collection as Json</returns>
		public async Task<string> GetCustomersByBatchIdAsJson(int id)
		{
			try
			{
				var customers = await GetCustomersAsJson(_customersBatchs[id].Name);
				return customers;
			}
			catch (Exception ex)
			{
				throw new OptimoveException(ex.Message, ex);
			}
		}

		#endregion

		#region Private Methods

		private void Initialize(EngagerSDKSettings settings)
		{
            var base64EncodedBytes = System.Convert.FromBase64String(settings.ServiceAccount);
            var jsonCredentials = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            var credential = GoogleCredential.FromJson(jsonCredentials);
            _googleStorageClient = StorageClient.Create(credential);
			_settings = settings;
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
			var files = _googleStorageClient.ListObjects(bucketName, filePath, null);
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
			var files = GetFiles(_settings.BucketName, prefix);
			foreach (var file in files)
			{
				var avro = await DownloadFile(_settings.BucketName, file.Name, _settings.DecryptionKey);
				var batch = AvroConvert.Deserialize<List<T>>(avro);
				customers.AddRange(batch);
			}
			return customers;
		}

		private async Task<string> GetCustomersAsJson(string prefix)
		{
			var customersJson = "";
			var file = GetFiles(_settings.BucketName, prefix).FirstOrDefault();

			if (file != null)
            {
				var avro = await DownloadFile(_settings.BucketName, file.Name, _settings.DecryptionKey);
				customersJson = AvroConvert.Avro2Json(avro);
			}	
			
			return customersJson;
		}

		#endregion
	}
}
