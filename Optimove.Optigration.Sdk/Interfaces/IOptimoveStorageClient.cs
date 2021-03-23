using Optimove.Optigration.Sdk.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace Optimove.Optigration.Sdk.Interfaces
{
	/// <summary>
	/// Implementation of this interface incapsulates Optimove Cloud Storage (OCS) integration logic.
	/// </summary>
	public interface IOptimoveStorageClient
	{
		/// <summary>
		/// Retrieves customers Metadata.
		/// </summary>
		/// <returns>Customers metadata object.</returns>
		Task<Metadata> GetMetadata();

		/// <summary>
		/// Retrieves customers collection.
		/// </summary>
		/// <returns>Customers collection.</returns>
		Task<List<ExpandoObject>> GetCustomers();
	}
}
