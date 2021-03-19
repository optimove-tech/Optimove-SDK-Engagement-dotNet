using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Optimove.Optigration.Sdk.Models;
using Optimove.Optigration.Sdk.Tests.Constants;
using SolTechnology.Avro;
using System;
using System.IO;

namespace Optimove.Optigration.Sdk.Tests
{
	[TestClass]
	public class OptimoveStorageClientTest: BaseTest
	{
		[TestMethod]
		public void GetMetadataTest()
		{
			var bucketName = Configuration[ConfigurationKeys.FolderPath].Substring(0, Configuration[ConfigurationKeys.FolderPath].IndexOf('/'));
			var folderPath = Configuration[ConfigurationKeys.FolderPath].Substring(bucketName.Length + 1);
			var channelName = "Channel 1";
			var fileInfo = UploadMetadataTest(bucketName, folderPath, channelName);
			var settings = new OptimoveStorageClientSettings
			{
				ServiceAccount = Configuration[ConfigurationKeys.ServiceAccount],
				DecryptionKey = Configuration[ConfigurationKeys.DecryptionKey],
				FolderPath = Configuration[ConfigurationKeys.FolderPath],
			};
			OptimoveStorageClient storageClient = new OptimoveStorageClient(settings);
			var metadata = storageClient.GetMetadata();
			Assert.IsNotNull(metadata);
			Assert.AreEqual(metadata.ChannelName, channelName);
			GoogleStorageClient.DeleteObject(bucketName, fileInfo.Name);
		}

		[TestMethod]
		public void GetCustomersTest()
		{
			
		}

		private Google.Apis.Storage.v1.Data.Object UploadMetadataTest(string bucketName, string folderPath, string channelName)
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

			var stream = new MemoryStream(AvroConvert.Serialize(metadata));
			var uploadOptions = new UploadObjectOptions
			{
				EncryptionKey = EncryptionKey.Create(Convert.FromBase64String(Configuration[ConfigurationKeys.DecryptionKey]))
			};
			
			return GoogleStorageClient.UploadObject(bucketName, $"{folderPath}/METADATA_{DateTime.Now.Ticks}.avro", null, stream, uploadOptions);
		}
	}
}
