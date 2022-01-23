using Google.Cloud.Storage.V1;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Optimove.SDK.Engager.Models;
using Optimove.SDK.Engager.Tests.Constants;
using Optimove.SDK.Engager.Tests.Models;
using SolTechnology.Avro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
			var bucketName = Configuration[ConfigurationKeys.BucketName];
			var rootFolderPath = Configuration[ConfigurationKeys.RootFolderPath];
			var channelName = "Channel 1";
			var fileInfo = UploadMetadataFile(bucketName, rootFolderPath, channelName);
			var settings = new EngagerSDKSettings
			{
				BucketName = Configuration[ConfigurationKeys.BucketName],
				CustomersFolderPath = $"{rootFolderPath}/customers",
				ServiceAccount = Configuration[ConfigurationKeys.ServiceAccount],
				DecryptionKey = Configuration[ConfigurationKeys.DecryptionKey],
				MetadataFilePath = fileInfo.Name,
			};
			var storageClient = new EngagerSDK(settings);
			var metadata = await storageClient.GetMetadata();
			Assert.IsNotNull(metadata);
			Assert.AreEqual(metadata.ChannelName, channelName);
			GoogleStorageClient.DeleteObject(bucketName, fileInfo.Name);
		}

		[TestMethod]
		public void GetCustomerBatchesTest()
		{
			var bucketName = Configuration[ConfigurationKeys.BucketName];
			var rootFolderPath = Configuration[ConfigurationKeys.RootFolderPath];
			var fileInfo = UploadCustomersFile(bucketName, $"{rootFolderPath}/customers");
			var settings = new EngagerSDKSettings
			{
				BucketName = Configuration[ConfigurationKeys.BucketName],
				CustomersFolderPath = $"{rootFolderPath}/customers",
				ServiceAccount = Configuration[ConfigurationKeys.ServiceAccount],
				DecryptionKey = Configuration[ConfigurationKeys.DecryptionKey],
				MetadataFilePath = fileInfo.Name,
			};
			var storageClient = new EngagerSDK(settings);
			var batchesCount = storageClient.GetCustomerBatchesNumber();
			Assert.IsTrue(batchesCount > 0);
			GoogleStorageClient.DeleteObject(bucketName, fileInfo.Name);
		}

		[TestMethod]
		public async Task GetCustomersByBatchTest()
		{
			var bucketName = Configuration[ConfigurationKeys.BucketName];
			var rootFolderPath = Configuration[ConfigurationKeys.RootFolderPath];
			var fileInfo1 = UploadCustomersFile(bucketName, $"{rootFolderPath}/customers");
			var fileInfo2 = UploadCustomersFile(bucketName, $"{rootFolderPath}/customers");
			var settings = new EngagerSDKSettings
			{
				BucketName = Configuration[ConfigurationKeys.BucketName],
				CustomersFolderPath = $"{rootFolderPath}/customers",
				ServiceAccount = Configuration[ConfigurationKeys.ServiceAccount],
				DecryptionKey = Configuration[ConfigurationKeys.DecryptionKey],
			};
			var storageClient = new EngagerSDK(settings);
			int batchesNumber = storageClient.GetCustomerBatchesNumber();
			for (int i = 0; i < batchesNumber; i++)
			{
				var customers = await storageClient.GetCustomersByBatchId<TestObject>(i);
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
			var stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(metadata)));
			return GoogleStorageClient.UploadObject(bucketName, $"{folderPath}/METADATA_{DateTime.Now.Ticks}.json", null, stream, GetUploadUptions());
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
