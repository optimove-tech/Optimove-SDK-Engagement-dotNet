namespace Optimove.SDK.Engager.Models
{
	/// <summary>
	/// Optimove Storage Client Settings
	/// </summary>
	public class EngagerSDKSettings
	{
		/// <summary>
		/// Cloud Bucket name.
		/// </summary>
		public string BucketName { get; set; }

		/// <summary>
		/// Customers folder path.
		/// </summary>
		public string CustomersFolderPath { get; set; }

		/// <summary>
		/// Metadata file path.
		/// </summary>
		public string MetadataFilePath { get; set; }

		/// <summary>
		/// Optimove Cloud Storage access credentials.
		/// </summary>
		public string ServiceAccount { get; set; }

		/// <summary>
		/// Key for data decryption.
		/// </summary>
		public string DecryptionKey { get; set; }
	}
}
