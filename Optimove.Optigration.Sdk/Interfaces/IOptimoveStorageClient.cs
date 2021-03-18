using Optimove.Optigration.Sdk.Models;
using System;
using System.Collections.Generic;
using System.Text;

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
		Metadata GetMetadata();

		/// <summary>
		/// Retrieves customers collection.
		/// </summary>
		/// <returns>Customers collection.</returns>
		IEnumerable<Customer> GetCustomers();
	}
}
