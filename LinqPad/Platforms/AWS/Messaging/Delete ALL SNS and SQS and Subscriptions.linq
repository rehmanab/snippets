<Query Kind="Program">
  <NuGetReference>AWSSDK.SimpleNotificationService</NuGetReference>
  <NuGetReference>AWSSDK.SQS</NuGetReference>
  <Namespace>Amazon</Namespace>
  <Namespace>Amazon.Auth.AccessControlPolicy</Namespace>
  <Namespace>Amazon.Runtime</Namespace>
  <Namespace>Amazon.Runtime.CredentialManagement</Namespace>
  <Namespace>Amazon.Runtime.CredentialManagement.Internal</Namespace>
  <Namespace>Amazon.Runtime.Credentials.Internal</Namespace>
  <Namespace>Amazon.Runtime.Documents</Namespace>
  <Namespace>Amazon.Runtime.Documents.Internal.Transform</Namespace>
  <Namespace>Amazon.Runtime.EventStreams</Namespace>
  <Namespace>Amazon.Runtime.EventStreams.Internal</Namespace>
  <Namespace>Amazon.Runtime.Internal</Namespace>
  <Namespace>Amazon.Runtime.Internal.Auth</Namespace>
  <Namespace>Amazon.Runtime.Internal.Settings</Namespace>
  <Namespace>Amazon.Runtime.Internal.Transform</Namespace>
  <Namespace>Amazon.Runtime.Internal.Util</Namespace>
  <Namespace>Amazon.Runtime.SharedInterfaces</Namespace>
  <Namespace>Amazon.Runtime.SharedInterfaces.Internal</Namespace>
  <Namespace>Amazon.SimpleNotificationService</Namespace>
  <Namespace>Amazon.SimpleNotificationService.Internal</Namespace>
  <Namespace>Amazon.SimpleNotificationService.Model</Namespace>
  <Namespace>Amazon.SimpleNotificationService.Model.Internal.MarshallTransformations</Namespace>
  <Namespace>Amazon.SimpleNotificationService.Util</Namespace>
  <Namespace>Amazon.SQS</Namespace>
  <Namespace>Amazon.SQS.Internal</Namespace>
  <Namespace>Amazon.SQS.Model</Namespace>
  <Namespace>Amazon.SQS.Model.Internal.MarshallTransformations</Namespace>
  <Namespace>Amazon.SQS.Util</Namespace>
  <Namespace>Amazon.Util</Namespace>
  <Namespace>Amazon.Util.Internal</Namespace>
  <Namespace>Amazon.Util.Internal.PlatformServices</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>ThirdParty.Ionic.Zlib</Namespace>
  <Namespace>ThirdParty.MD5</Namespace>
</Query>

#load "Services\AWS\AwsClientService"
#load "Services\AWS\AwsSqsService"
#load "Services\AWS\AwsSnsService"

private IAmazonSQS _sqsClient = null!;
private IAmazonSimpleNotificationService _snsClient = null!;

async Task Main()
{
	string profileName = "";
	_sqsClient = GetAwsClient<AmazonSQSClient>(profileName);
	_snsClient = GetAwsClient<AmazonSimpleNotificationServiceClient>(profileName);

	const string prefix = "";

	// TOPICS
	var topicArns = GetAllTopics(_snsClient, prefix).ToBlockingEnumerable().SelectMany(x => x).Select(x => x.TopicArn);

	topicArns.ToList().Dump("Topics").ForEach(async x =>
	{
		try
		{
			await foreach (var subscriptions in GetSubscriptions(_snsClient, x)) 
			{
				foreach (var subscription in subscriptions)
				{
					var res = await _snsClient.UnsubscribeAsync(subscription.SubscriptionArn);
					Console.WriteLine($"Unsubscribed {subscription.Protocol} from topic: {subscription.TopicArn} with ARN: {subscription.SubscriptionArn} - ( {res.HttpStatusCode} )");
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Exception unsubscribing from topic: {ex.Message}");
		}

		try
		{
			var response = await _snsClient.DeleteTopicAsync(x);
			Console.WriteLine($"Topic deleted: {x} - ( {response.HttpStatusCode} )");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Exception deleting topic: {ex.Message}");
		}
	});

	// QUEUES
	var queueUrls = GetQueueUrlsAsync(_sqsClient, prefix).ToBlockingEnumerable().SelectMany(x => x);
	queueUrls.ToList().Dump("Queues").ForEach(async x =>
	{
		try
		{
			var response = await _sqsClient.DeleteQueueAsync(x);
			Console.WriteLine($"Queue deleted: {x} - ( {response.HttpStatusCode} )");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Exception: {ex.Message}");
		}
	});
	
	// Any orphaned subscriptions
	var subscriptions = GetAllSubscriptions(_snsClient, prefix).ToBlockingEnumerable().SelectMany(x => x);
	subscriptions.Select(x => x.SubscriptionArn).Dump("Orphaned Subscriptions");
	foreach (var subscription in subscriptions)
	{
		await _snsClient.UnsubscribeAsync(subscription.SubscriptionArn);
	}
	
	await Task.CompletedTask;
}
