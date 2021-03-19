using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Optimove.Optigration.Sdk.Models;
using SolTechnology.Avro;
using System;
using System.IO;
using static Optimove.Optigration.Sdk.Tests.Constants.ConfigurationKeys;

namespace Optimove.Optigration.Sdk.Tests
{
	[TestClass]
	public class OptimoveStorageClientTest: BaseTest
	{
		[TestMethod]
		public void GetMetadataTest()
		{
			var channelName = "Channel 1";
			UploadMetadataTest(channelName);
			var settings = new OptimoveStorageClientSettings
			{
				ServiceAccount = Configuration[ServiceAccount],
				DecryptionKey = Configuration[DecryptionKey],
				FolderPath = Configuration[FolderPath],
			};
			OptimoveStorageClient storageClient = new OptimoveStorageClient(settings);
			var metadata = storageClient.GetMetadata();
			Assert.IsNotNull(metadata);
			Assert.AreEqual(metadata.ChannelName, channelName);
		}

		private Google.Apis.Storage.v1.Data.Object UploadMetadataTest(string channelName)
		{
			var metadata = new Metadata
			{
				ActionID = 1,
				ActionName = "Action 1",
				ActionSerial = 11,
				ChannelID = 12,
				ChannelName = channelName,
				EngagementID = 123,
				NumberOfCustomers = 100,
				NumberOfFiles = 10,
				PlanDetailID = 21,
				PlanID = 24,
				Promotions = "Promotion 1",
				ScheduledTime = DateTime.Now,
				TargetGroupName = "Target Group 1",
				TemplateID = 321,
				TemplateName = "Template 1",
			};

			var base64EncodedBytes = System.Convert.FromBase64String(Configuration[ServiceAccount]);
			var jsonCredentials = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
			var credential = GoogleCredential.FromJson(jsonCredentials);
			var googleStorageClient = StorageClient.Create(credential);

			var data = AvroConvert.Serialize(metadata);
			var stream = new MemoryStream(data);
			var bucketName = Configuration[FolderPath].Substring(0, Configuration[FolderPath].IndexOf('/'));
			var folderPath = Configuration[FolderPath].Substring(bucketName.Length + 1);

			var uploadOptions = new UploadObjectOptions
			{
				EncryptionKey = EncryptionKey.Create(Convert.FromBase64String(Configuration[DecryptionKey]))
			};
			
			return googleStorageClient.UploadObject(bucketName, $"{folderPath}/METADATA_{DateTime.Now.Ticks}.avro", null, stream, uploadOptions);
		}
	}
}
