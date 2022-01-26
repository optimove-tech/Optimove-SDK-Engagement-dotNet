The **"Optimove Engager Integration SDK**" is a NuGet package for integration between Optimove client systems (applications) with the Optimove SaaS Platform.

The integration flow is:
- Add reference to the **OptimoveSdk.Engager.Integration** NuGet package
- Create Web Hook entry point / Web Api entry point. Details should be discussed with Optimove support team.
- Receive Web Hook notification from **Optimove Notifications Subsystem** about customers data readiness.
  The message format is the following:
```json
{
  "eventTypeID": 0,
  "timeStamp": null,
  "campaignID": 0,
  "engagementID": 0,
  "tenantID": 0,
  "bucketName": null,
  "customersFolderPath": null,
  "metadataFilePath": null,
  "serviceAccount": null,
  "decryptionKey": null
}
```
- Instantiate **OptimoveStorageClient** object. Input parameter is Web Hook message serialized into the JSON
```csharp
var client = new OptimoveStorageClient(jsonSettings);
```
- Get Metadata object
```csharp
var metadata = await client.GetMetadata();
```
Metadata object's format is:
```csharp
public class Metadata
{
   /// <summary>
   /// Campaign Plan ID
   /// </summary>
   public int CampaignPlanID { get; set; }

   /// <summary>
   /// Campaign ID
   /// </summary>
   public int CampaignID { get; set; }

   /// <summary>
   /// Plan Detail Channe lID
   /// </summary>
   public int PlanDetailChannelID { get; set; }

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
   
	/// <summary>
	/// Duration
	/// </summary>
	public int Duration { get; set; }
}
```
- Get Customers by batch
T is the type that the customer batch should be parsed into.
```csharp
int batchesNumber = storageClient.GetCustomerBatchesNumber();
for (int i = 0; i < batchesNumber; i++)
{
	var customers = await storageClient.GetCustomersByBatchId<T>(i);
}
```
