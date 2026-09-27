<Query Kind="Program">
  <NuGetReference>AWSSDK.SimpleNotificationService</NuGetReference>
  <NuGetReference>AWSSDK.SQS</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Amazon</Namespace>
  <Namespace>Amazon.Auth.AccessControlPolicy</Namespace>
  <Namespace>Amazon.Runtime</Namespace>
  <Namespace>Amazon.Runtime.CredentialManagement</Namespace>
  <Namespace>Amazon.Runtime.CredentialManagement.Internal</Namespace>
  <Namespace>Amazon.Runtime.Credentials</Namespace>
  <Namespace>Amazon.Runtime.Credentials.Internal</Namespace>
  <Namespace>Amazon.Runtime.Documents</Namespace>
  <Namespace>Amazon.Runtime.Documents.Internal.Transform</Namespace>
  <Namespace>Amazon.Runtime.Endpoints</Namespace>
  <Namespace>Amazon.Runtime.EventStreams</Namespace>
  <Namespace>Amazon.Runtime.EventStreams.Internal</Namespace>
  <Namespace>Amazon.Runtime.EventStreams.Utils</Namespace>
  <Namespace>Amazon.Runtime.Identity</Namespace>
  <Namespace>Amazon.Runtime.Internal</Namespace>
  <Namespace>Amazon.Runtime.Internal.Auth</Namespace>
  <Namespace>Amazon.Runtime.Internal.Compression</Namespace>
  <Namespace>Amazon.Runtime.Internal.Endpoints.StandardLibrary</Namespace>
  <Namespace>Amazon.Runtime.Internal.Settings</Namespace>
  <Namespace>Amazon.Runtime.Internal.Transform</Namespace>
  <Namespace>Amazon.Runtime.Internal.UserAgent</Namespace>
  <Namespace>Amazon.Runtime.Internal.Util</Namespace>
  <Namespace>Amazon.Runtime.Logging</Namespace>
  <Namespace>Amazon.Runtime.Pipeline.HttpHandler</Namespace>
  <Namespace>Amazon.Runtime.SharedInterfaces</Namespace>
  <Namespace>Amazon.Runtime.SharedInterfaces.Internal</Namespace>
  <Namespace>Amazon.Runtime.Telemetry</Namespace>
  <Namespace>Amazon.Runtime.Telemetry.Metrics</Namespace>
  <Namespace>Amazon.Runtime.Telemetry.Tracing</Namespace>
  <Namespace>Amazon.RuntimeDependencies</Namespace>
  <Namespace>Amazon.SimpleNotificationService</Namespace>
  <Namespace>Amazon.SimpleNotificationService.Model</Namespace>
  <Namespace>Amazon.SimpleNotificationService.Model.Internal.MarshallTransformations</Namespace>
  <Namespace>Amazon.SimpleNotificationService.Util</Namespace>
  <Namespace>Amazon.SQS</Namespace>
  <Namespace>Amazon.SQS.Endpoints</Namespace>
  <Namespace>Amazon.SQS.Internal</Namespace>
  <Namespace>Amazon.SQS.Model</Namespace>
  <Namespace>Amazon.SQS.Model.Internal.MarshallTransformations</Namespace>
  <Namespace>Amazon.SQS.Util</Namespace>
  <Namespace>Amazon.Util</Namespace>
  <Namespace>Amazon.Util.Internal</Namespace>
  <Namespace>Amazon.Util.Internal.PlatformServices</Namespace>
  <Namespace>AWSSDK.Runtime.Internal.Util</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>ThirdParty.Ionic.Zlib</Namespace>
  <Namespace>ThirdParty.MD5</Namespace>
  <Namespace>ThirdParty.RuntimeBackports</Namespace>
</Query>

#load "Services\AWS\AwsClientService"
#load "Projects\Topica\TopicaMessagesModels"
#load "Services\AWS\AwsSnsService"

private IAmazonSimpleNotificationService _snsClient = null!;

async Task Main()
{
	_snsClient = GetAwsClient<AmazonSimpleNotificationServiceClient>();

	await SendPageLoadedMessage(1);
	await SendLinkClickedMessage(1);
}

private async Task SendPageLoadedMessage(int count)
{
	const string topicNamePrefix = "topicname.fifo";
	var topicArns = GetAllTopics(_snsClient, topicNamePrefix).ToBlockingEnumerable().SelectMany(x => x).Select(x => x.TopicArn);

	if (!topicArns.Any())
	{
		throw new Exception($"No topic found for prefix: {topicNamePrefix}");
	}

	if (topicArns.Count() > 1)
	{
		throw new Exception($"More than 1 topic found for prefix: {topicNamePrefix}");
	}

	Parallel.ForEach(Enumerable.Range(1, count), index =>
	{
		var topic = topicArns.First();

		var request = new PublishRequest
		{
			TopicArn = topic,
			Message = JsonConvert.SerializeObject(new PageLoadedMessageV1
			{
				EventId = index,
				EventName = "page.loaded.event",
				ConversationId = Guid.NewGuid(),
				Type = nameof(PageLoadedMessageV1),
				RaisingComponent = "LinqPad",
				Version = "V1",
				MessageAdditionalProperties = new Dictionary<string, string> { { "prop1", "value1" } },
			}),
			MessageAttributes = new Dictionary<string, Amazon.SimpleNotificationService.Model.MessageAttributeValue>
			{
				{ "SignatureVersion", new Amazon.SimpleNotificationService.Model.MessageAttributeValue() { StringValue = "2", DataType = "String"} }
			}
		};

		if (topic.EndsWith(".fifo"))
		{
			request.MessageGroupId = Guid.NewGuid().ToString();
			request.MessageDeduplicationId = Guid.NewGuid().ToString();
		}

		_snsClient.PublishAsync(request).Wait();
		Console.WriteLine($"Sending to topic: {topic}: {index} : {nameof(PageLoadedMessageV1)}");
	});

	await Task.CompletedTask;
}

private async Task SendLinkClickedMessage(int count)
{
	const string topicNamePrefix = "topicname.fifo";
	var topicArns = GetAllTopics(_snsClient, topicNamePrefix).ToBlockingEnumerable().SelectMany(x => x).Select(x => x.TopicArn);

	if (!topicArns.Any())
	{
		throw new Exception($"No topic found for prefix: {topicNamePrefix}");
	}

	if (topicArns.Count() > 1)
	{
		throw new Exception($"More than 1 topic found for prefix: {topicNamePrefix}");
	}

	Parallel.ForEach(Enumerable.Range(1, count), index =>
	{
		var topic = topicArns.First();

		var request = new PublishRequest
		{
			TopicArn = topic,
			Message = JsonConvert.SerializeObject(new LinkClickedMessageV1
			{
				EventId = index,
				EventName = "link.clicked.web",
				ConversationId = Guid.NewGuid(),
				Type = nameof(LinkClickedMessageV1),
				RaisingComponent = "LinqPad",
				Version = "V1",
				MessageAdditionalProperties = new Dictionary<string, string> { { "prop1", "value1" } }
			}),
			MessageAttributes = new Dictionary<string, Amazon.SimpleNotificationService.Model.MessageAttributeValue>
			{
				{ "SignatureVersion", new Amazon.SimpleNotificationService.Model.MessageAttributeValue() { StringValue = "2", DataType = "String"} }
			}
		};

		if (topic.EndsWith(".fifo"))
		{
			request.MessageGroupId = Guid.NewGuid().ToString();
			request.MessageDeduplicationId = Guid.NewGuid().ToString();
		}

		_snsClient.PublishAsync(request).Wait();
		Console.WriteLine($"Sending to topic: {topic}: {index} : {nameof(LinkClickedMessageV1)}");
	});

	await Task.CompletedTask;
}