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
	string environmentName = "";
	_sqsClient = GetAwsClient<AmazonSQSClient>(environmentName);
	_snsClient = GetAwsClient<AmazonSimpleNotificationServiceClient>(environmentName);

	const string prefix = "";

	// TOPICS
	var topicArns = GetAllTopics(_snsClient, prefix).ToBlockingEnumerable().SelectMany(x => x).Select(x => x.TopicArn);
	topicArns.ToList().Dump("Topics");

	// QUEUES
	var queueUrls = GetQueueUrlsAsync(_sqsClient, prefix).ToBlockingEnumerable().SelectMany(x => x);
	queueUrls.ToList().Dump("Queues");
	
	// SUBSCRIPTIONS
	var subscriptions = GetAllSubscriptions(_snsClient, prefix).ToBlockingEnumerable().SelectMany(x => x);
	subscriptions.Select(x => x.SubscriptionArn).Dump("Subscriptions");
	
	await Task.CompletedTask;
}
