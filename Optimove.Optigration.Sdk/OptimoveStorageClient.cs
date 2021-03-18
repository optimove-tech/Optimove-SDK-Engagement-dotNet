using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Optimove.Optigration.Sdk.Interfaces;
using Optimove.Optigration.Sdk.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

			_bucketName = settings.FolderPath.Substring(0, settings.FolderPath.IndexOf('/'));
			_folderPath = settings.FolderPath.Remove(0, _bucketName.Length + 1);
		}

		/// <summary>
		/// Retrieves customers Metadata.
		/// </summary>
		/// <returns>Customers metadata object.</returns>
		public Metadata GetMetadata()
		{
			//var filePath = $"{_settings.FolderPath}/{_settings.MetedataFileName}";
			//var data = DownloadFile(_settings.BucketName, filePath, _settings.DecryptionKey);
			return null;
		}

		/// <summary>
		/// Retrieves customers collection.
		/// </summary>
		/// <returns>Customers collection.</returns>
		public IEnumerable<Customer> GetCustomers()
		{
			return null;
		}

		/// <summary>
		/// Download file.
		/// </summary>
		/// <param name="bucketName">Bucket name.</param>
		/// <param name="filePath">File path.</param>
		/// <param name="decryptionKey">Decryption key.</param>
		/// <returns>File content.</returns>
		private byte[] DownloadFile(string bucketName, string filePath, string decryptionKey = null)
		{
			try
			{
				if (GetFileInfo(bucketName, filePath) == null)
				{
					return null;
				}
				using (var stream = new MemoryStream())
				{
					var downloadOptions = new DownloadObjectOptions();
					if (!String.IsNullOrEmpty(decryptionKey))
					{
						downloadOptions.EncryptionKey = EncryptionKey.Create(Convert.FromBase64String(decryptionKey));
					}
					_googleStorageClient.DownloadObject(bucketName, filePath, stream, downloadOptions);
					stream.Position = 0;
					return stream.ToArray();
				}
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Gets information about file.
		/// </summary>
		/// <param name="bucketName">Bucket name.</param>
		/// <param name="filePath">File path.</param>
		/// <returns>File metadata.</returns>
		private Google.Apis.Storage.v1.Data.Object GetFileInfo(string bucketName, string filePath)
		{
			try
			{
				var file = _googleStorageClient.ListObjects(bucketName, filePath).FirstOrDefault();
				return file;
			}
			catch
			{
				return null;
			}
		}
	}
}
