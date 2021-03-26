using System;

namespace Optimove.SDK.Engager.Exceptions
{
	/// <summary>
	/// Optimove Exception.
	/// </summary>
	public class OptimoveException: Exception
	{
		/// <summary>
		/// Construct Optimove Exception.
		/// </summary>
		/// <param name="message">Exception Message.</param>
		/// <param name="innerException">Inner exception object.</param>
		public OptimoveException(string message, Exception innerException): base(message, innerException)
		{
		}
	}
}
