using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Optimove.Optigration.Sdk.Tests
{
	[TestClass]
	public class BaseTest
	{
		protected IConfiguration Configuration { get; private set; }

		[TestInitialize]
		public void TestsSetup()
		{
			var builder = new ConfigurationBuilder();
			builder.AddUserSecrets<OptimoveStorageClientTest>();
			Configuration = builder.Build();
		}
	}
}
