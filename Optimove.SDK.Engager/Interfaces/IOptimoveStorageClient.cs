using Optimove.SDK.Engager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optimove.SDK.Engager.Interfaces
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
		Task<List<T>> GetCustomers<T>();
	}
}
