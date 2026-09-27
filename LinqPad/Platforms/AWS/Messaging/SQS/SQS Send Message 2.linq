<Query Kind="Program">
  <NuGetReference>AWSSDK.SQS</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Amazon</Namespace>
  <Namespace>Amazon.Auth.AccessControlPolicy</Namespace>
  <Namespace>Amazon.Runtime</Namespace>
  <Namespace>Amazon.Runtime.CredentialManagement</Namespace>
  <Namespace>Amazon.Runtime.Internal</Namespace>
  <Namespace>Amazon.Runtime.Internal.Auth</Namespace>
  <Namespace>Amazon.Runtime.Internal.Settings</Namespace>
  <Namespace>Amazon.Runtime.Internal.Transform</Namespace>
  <Namespace>Amazon.Runtime.Internal.Util</Namespace>
  <Namespace>Amazon.Runtime.SharedInterfaces</Namespace>
  <Namespace>Amazon.SQS</Namespace>
  <Namespace>Amazon.SQS.Internal</Namespace>
  <Namespace>Amazon.SQS.Model</Namespace>
  <Namespace>Amazon.SQS.Model.Internal.MarshallTransformations</Namespace>
  <Namespace>Amazon.SQS.Util</Namespace>
  <Namespace>Amazon.Util</Namespace>
  <Namespace>Amazon.Util.Internal</Namespace>
  <Namespace>Amazon.Util.Internal.PlatformServices</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>ThirdParty.Ionic.Zlib</Namespace>
  <Namespace>ThirdParty.MD5</Namespace>
</Query>

#load "Services\AWS\AwsClientService"
#load "Services\AWS\AwsSqsService"

private IAmazonSQS _client = null!;

async Task Main()
{
	_client = GetAwsClient<AmazonSQSClient>("");

	var message = JsonConvert.SerializeObject(new
	{
		EventId = 1,
		EventName = "button.clicked.web.v1",
		ConversationId = Guid.NewGuid(),
		Type = "type",
		RaisingComponent = "LinqPad",
		Version = "V1",
		MessageAdditionalProperties = new Dictionary<string, string> { { "prop1", "value1" } }
	});

	
	var queueUrl = await GetQueueUrlAsync(_client, "queuename.fifo");

	var sendMessageRequest = new SendMessageRequest
	{
		QueueUrl = queueUrl,
		MessageBody = message,
		MessageGroupId = Guid.NewGuid().ToString(),
		MessageDeduplicationId = Guid.NewGuid().ToString()
	};
	
	var sendMessageResponse = await _client.SendMessageAsync(sendMessageRequest);
	sendMessageResponse.HttpStatusCode.Dump("HttpStatusCode");
}

public class Message
{
	public string Data { get; set; }
}