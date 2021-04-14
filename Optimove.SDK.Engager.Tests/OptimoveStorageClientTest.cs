using Google.Cloud.Storage.V1;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Optimove.SDK.Engager.Models;
using Optimove.SDK.Engager.Tests.Constants;
using Optimove.SDK.Engager.Tests.Models;
using SolTechnology.Avro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Optimove.SDK.Engager.Tests
{
	[TestClass]
	public class OptimoveStorageClientTest: BaseTest
	{
		#region Tests

		[TestMethod]
		public async Task GetMetadataTest()
		{
			var bucketName = Configuration[ConfigurationKeys.FolderPath].Substring(0, Configuration[ConfigurationKeys.FolderPath].IndexOf('/'));
			var folderPath = Configuration[ConfigurationKeys.FolderPath].Substring(bucketName.Length + 1);
			var channelName = "Channel 1";
			var fileInfo = UploadMetadataFile(bucketName, folderPath, channelName);
			var settings = new OptimoveStorageClientSettings
			{
				ServiceAccount = Configuration[ConfigurationKeys.ServiceAccount],
				DecryptionKey = Configuration[ConfigurationKeys.DecryptionKey],
				FolderPath = Configuration[ConfigurationKeys.FolderPath],
			};
			var storageClient = new OptimoveStorageClient(settings);
			var metadata = await storageClient.GetMetadata();
			Assert.IsNotNull(metadata);
			Assert.AreEqual(metadata.ChannelName, channelName);
			GoogleStorageClient.DeleteObject(bucketName, fileInfo.Name);
		}

		[TestMethod]
		public void GetCustomerBatchesTest()
		{
			var bucketName = Configuration[ConfigurationKeys.FolderPath].Substring(0, Configuration[ConfigurationKeys.FolderPath].IndexOf('/'));
			var folderPath = Configuration[ConfigurationKeys.FolderPath].Substring(bucketName.Length + 1);
			var fileInfo = UploadCustomersFile(bucketName, folderPath);
			var settings = new OptimoveStorageClientSettings
			{
				ServiceAccount = Configuration[ConfigurationKeys.ServiceAccount],
				DecryptionKey = Configuration[ConfigurationKeys.DecryptionKey],
				FolderPath = Configuration[ConfigurationKeys.FolderPath],
			};
			var storageClient = new OptimoveStorageClient(settings);
			var batches = storageClient.GetCustomerBatches();
			Assert.IsTrue(batches.Count > 0);
			GoogleStorageClient.DeleteObject(bucketName, fileInfo.Name);
		}

		[TestMethod]
		public async Task GetAllCustomersTest()
		{
			var bucketName = Configuration[ConfigurationKeys.FolderPath].Substring(0, Configuration[ConfigurationKeys.FolderPath].IndexOf('/'));
			var folderPath = Configuration[ConfigurationKeys.FolderPath].Substring(bucketName.Length + 1);
			var fileInfo1 = UploadCustomersFile(bucketName, $"{folderPath}/customers");
			var fileInfo2 = UploadCustomersFile(bucketName, $"{folderPath}/customers");
			var settings = new OptimoveStorageClientSettings
			{
				ServiceAccount = Configuration[ConfigurationKeys.ServiceAccount],
				DecryptionKey = Configuration[ConfigurationKeys.DecryptionKey],
				FolderPath = Configuration[ConfigurationKeys.FolderPath],
			};
			var storageClient = new OptimoveStorageClient(settings);
			var customers = await storageClient.GetAllCustomers<TestObject>();
			Assert.AreEqual(customers.Count, 20);
			GoogleStorageClient.DeleteObject(bucketName, fileInfo1.Name);
			GoogleStorageClient.DeleteObject(bucketName, fileInfo2.Name);
		}

		[TestMethod]
		public async Task GetCustomersByBatchTest()
		{
			var bucketName = Configuration[ConfigurationKeys.FolderPath].Substring(0, Configuration[ConfigurationKeys.FolderPath].IndexOf('/'));
			var folderPath = Configuration[ConfigurationKeys.FolderPath].Substring(bucketName.Length + 1);
			var fileInfo1 = UploadCustomersFile(bucketName, $"{folderPath}/customers");
			var fileInfo2 = UploadCustomersFile(bucketName, $"{folderPath}/customers");
			var settings = new OptimoveStorageClientSettings
			{
				ServiceAccount = Configuration[ConfigurationKeys.ServiceAccount],
				DecryptionKey = Configuration[ConfigurationKeys.DecryptionKey],
				FolderPath = Configuration[ConfigurationKeys.FolderPath],
			};
			var storageClient = new OptimoveStorageClient(settings);
			foreach(var batch in storageClient.GetCustomerBatches())
			{
				var customers = await storageClient.GetCustomersByBatch<TestObject>(batch);
				Assert.AreEqual(customers.Count, 10);
			}
			GoogleStorageClient.DeleteObject(bucketName, fileInfo1.Name);
			GoogleStorageClient.DeleteObject(bucketName, fileInfo2.Name);
		}

		#endregion

		#region Private Methods

		private Google.Apis.Storage.v1.Data.Object UploadCustomersFile(string bucketName, string folderPath)
		{
			var customers = new List<TestObject>();
			for(int i = 1; i < 11; i++)
			{
				var customer = new TestObject()
				{
					Int = i,
					String = $"test_{i}",
					Bool = true,
					Float = 0.1f,
					Double = 0.2123456789,
					Object = new TestObject2() { Property1 = i, Property2 = $"test_{i}" },
					DateTime = DateTime.UtcNow,
					IntArray = new int[] { 1, 11, 23 },
					IntList = new List<int> { 11, 22 },
					ObjectList = new List<TestObject2> { new TestObject2() { Property1 = 1, Property2 = "one" },
										   new TestObject2() { Property1 = 2, Property2 = "two" } },
					ObjectArray = new TestObject2[] { new TestObject2() { Property1 = 3, Property2 = "three" },
										   new TestObject2() { Property1 = 4, Property2 = "four" } },
				};
				customers.Add(customer);
			}
			var stream = new MemoryStream(AvroConvert.Serialize(customers));
			return GoogleStorageClient.UploadObject(bucketName, $"{folderPath}/{DateTime.Now.Ticks}.avro", null, stream, GetUploadUptions());
		}

		private Google.Apis.Storage.v1.Data.Object UploadMetadataFile(string bucketName, string folderPath, string channelName)
		{
			var metadata = new Metadata
			{
				ActionID = 1,
				ActionName = "Action 1",
				CampaignID = 11,
				ChannelID = 12,
				ChannelName = channelName,
				EngagementID = 123,
				NumberOfCustomers = 100,
				NumberOfFiles = 10,
				PlanDetailChannelID = 21,
				CampaignPlanID = 24,
				Promotions = "Promotion 1",
				ScheduledTime = DateTime.Now,
				TargetGroupName = "Target Group 1",
				TemplateID = 321,
				TemplateName = "Template 1",
			};
			var stream = new MemoryStream(AvroConvert.Serialize(metadata));
			return GoogleStorageClient.UploadObject(bucketName, $"{folderPath}/METADATA_{DateTime.Now.Ticks}.avro", null, stream, GetUploadUptions());
		}

		private UploadObjectOptions GetUploadUptions()
		{
			var uploadOptions = new UploadObjectOptions();
			if (!String.IsNullOrEmpty(Configuration[ConfigurationKeys.DecryptionKey]))
			{
				uploadOptions.EncryptionKey = EncryptionKey.Create(Convert.FromBase64String(Configuration[ConfigurationKeys.DecryptionKey]));
			}
			return uploadOptions;
		}

		#endregion
	}
}
