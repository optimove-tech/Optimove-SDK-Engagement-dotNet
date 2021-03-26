using System;

namespace Optimove.SDK.Engager.Models
{
	/// <summary>
	/// Metadata object.
	/// </summary>
	public class Metadata
	{
		/// <summary>
		/// Plan ID
		/// </summary>
		public int PlanID { get; set; }

		/// <summary>
		/// Action Serial
		/// </summary>
		public int ActionSerial { get; set; }

		/// <summary>
		/// Plan Detail ID
		/// </summary>
		public int PlanDetailID { get; set; }

		/// <summary>
		/// Engagement ID
		/// </summary>
		public long EngagementID { get; set; }

		/// <summary>
		/// Scheduled Time
		/// </summary>
		public DateTime ScheduledTime { get; set; }

		/// <summary>
		/// Channel Name
		/// </summary>
		public string ChannelName { get; set; }

		/// <summary>
		/// Channel ID
		/// </summary>
		public int ChannelID { get; set; }

		/// <summary>
		/// Target Group Name
		/// </summary>
		public string TargetGroupName { get; set; }

		/// <summary>
		/// Action Name
		/// </summary>
		public string ActionName { get; set; }

		/// <summary>
		/// Action ID
		/// </summary>
		public int ActionID { get; set; }

		/// <summary>
		/// Template Name
		/// </summary>
		public string TemplateName { get; set; }

		/// <summary>
		/// Template ID
		/// </summary>
		public long TemplateID { get; set; }

		/// <summary>
		/// Number Of Files
		/// </summary>
		public int NumberOfFiles { get; set; }

		/// <summary>
		/// Number Of Customers
		/// </summary>
		public int NumberOfCustomers { get; set; }

		/// <summary>
		/// Promotions
		/// </summary>
		public string Promotions { get; set; }
	}
}
