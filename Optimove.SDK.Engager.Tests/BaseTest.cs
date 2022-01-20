using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Optimove.SDK.Engager.Tests.Constants;

namespace Optimove.SDK.Engager.Tests
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
			GoogleStorageClient = StorageClient.Create();
		}
	}
}
