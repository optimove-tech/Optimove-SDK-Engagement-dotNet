using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Optimove.Optigration.Sdk.Tests.Constants;

namespace Optimove.Optigration.Sdk.Tests
{
	[TestClass]
	public class BaseTest
	{
		protected IConfiguration Configuration { get; private set; }

		protected StorageClient GoogleStorageClient { get; private set; }


		[TestInitialize]
		public void TestsSetup()
		{
			var builder = new ConfigurationBuilder();
			builder.AddUserSecrets<OptimoveStorageClientTest>();
			Configuration = builder.Build();

			var base64EncodedBytes = System.Convert.FromBase64String(Configuration[ConfigurationKeys.ServiceAccount]);
			var jsonCredentials = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
			var credential = GoogleCredential.FromJson(jsonCredentials);
			GoogleStorageClient = StorageClient.Create(credential);
		}
	}
}
