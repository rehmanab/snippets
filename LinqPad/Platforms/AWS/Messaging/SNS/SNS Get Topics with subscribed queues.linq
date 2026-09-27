<Query Kind="Program">
  <NuGetReference>AWSSDK.SimpleNotificationService</NuGetReference>
  <NuGetReference>AWSSDK.SQS</NuGetReference>
  <Namespace>Amazon.SimpleNotificationService</Namespace>
  <Namespace>Amazon.SimpleNotificationService.Model</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <RuntimeVersion>9.0</RuntimeVersion>
</Query>

#load "Services\AWS\AwsClientService"
#load "Services\AWS\AwsSnsService"

private IAmazonSimpleNotificationService _snsClient = null!;

async Task Main()
{
	_snsClient = GetAwsClient<AmazonSimpleNotificationServiceClient>();
	
	await FindTopic("topicName", true);
	//await GetTopicList();
}

private async Task<string> FindTopic(string topicName, bool isFifo)
{
	string topicArn = await GetTopicArnAsync(_snsClient, topicName, isFifo);
	
	if(string.IsNullOrWhiteSpace(topicArn))
	{
		Console.WriteLine($"Topic: {topicName} - Not found!");
		return null;
	}

	var subs = GetSubscriptions(_snsClient, topicArn).ToBlockingEnumerable().SelectMany(x => x);

	topicArn.Dump("Topic");
	subs.Dump("Subscribed Queues");

	return topicArn;
}

private async Task GetTopicList()
{
	var topics = GetAllTopics(_snsClient).ToBlockingEnumerable().SelectMany(x => x).ToList();
	
	if(topics == null || !topics.Any())
	{
		Console.WriteLine("No topics");
		return;
	}

	Console.WriteLine($"Found {topics.Count} Topics");
	Console.WriteLine();
	foreach (var topic in topics)
	{
		var subscriptions = GetSubscriptions(_snsClient, topic.TopicArn).ToBlockingEnumerable().SelectMany(x => x).ToList();

		Console.WriteLine($"{topic.TopicArn} - {subscriptions.Count} Subscriptions");
		subscriptions.Dump("Subscriptions");
	}
	
	await Task.CompletedTask;
}