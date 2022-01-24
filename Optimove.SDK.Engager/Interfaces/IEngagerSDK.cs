using Optimove.SDK.Engager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optimove.SDK.Engager.Interfaces
{
	/// <summary>
	/// Implementation of this interface incapsulates Optimove Cloud Storage (OCS) integration logic.
	/// </summary>
	public interface IEngagerSDK
	{
		/// <summary>
		/// Retrieves customers Metadata.
		/// </summary>
		/// <returns>Customers metadata object.</returns>
		Task<Metadata> GetMetadata();

		/// <summary>
		/// Get Customer batches number.
		/// </summary>
		/// <returns>Batches count.</returns>
		int GetCustomerBatchesNumber();

		/// <summary>
		/// Retrieves customers by batch if.
		/// </summary>
		/// <returns>Customers collection.</returns>
		Task<List<T>> GetCustomersByBatchId<T>(int id);


		/// <summary>
		/// Retrieves customers by batch if.
		/// </summary>
		/// <returns>Customers collection as json</returns>
		Task<string> GetCustomersByBatchIdAsJson(int id);
	}
}
