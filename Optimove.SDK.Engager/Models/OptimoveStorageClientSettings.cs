﻿namespace Optimove.SDK.Engager.Models
{
	public class OptimoveStorageClientSettings
	{
		/// <summary>
		/// Optimove Cloud Storage access credentials.
		/// </summary>
		public string ServiceAccount { get; set; }

		/// <summary>
		/// Key for data decryption.
		/// </summary>
		public string DecryptionKey { get; set; }

		/// <summary>
		/// Cloud folder's path.
		/// </summary>
		public string FolderPath { get; set; }
	}
}