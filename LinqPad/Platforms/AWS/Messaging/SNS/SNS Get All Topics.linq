<Query Kind="Program">
  <NuGetReference>AWSSDK.SimpleNotificationService</NuGetReference>
  <NuGetReference>AWSSDK.SQS</NuGetReference>
  <Namespace>Amazon.SimpleNotificationService</Namespace>
  <Namespace>Amazon.SimpleNotificationService.Model</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Amazon</Namespace>
  <DisableMyExtensions>true</DisableMyExtensions>
  <RuntimeVersion>9.0</RuntimeVersion>
</Query>

#load "Services\AWS\AwsClientService"
#load "Services\AWS\AwsSnsService"

private IAmazonSimpleNotificationService _client = null!;

async Task Main()
{
	_client = GetAwsClient<AmazonSimpleNotificationServiceClient>("");
	
	var topics = GetAllTopics(_client, "").ToBlockingEnumerable().SelectMany(x => x).Select(x => x.TopicArn);
	
	topics.Dump();
	
	await Task.CompletedTask;
}