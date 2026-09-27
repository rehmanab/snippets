<Query Kind="Program">
  <NuGetReference>AWSSDK.SQS</NuGetReference>
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
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>ThirdParty.Ionic.Zlib</Namespace>
  <Namespace>ThirdParty.MD5</Namespace>
</Query>

#load "Services\AWS\AwsClientService"
#load "Services\AWS\AwsSqsService"

private IAmazonSQS _client = null!;

async Task Main()
{
	_client = GetAwsClient<AmazonSQSClient>("dev");

	(GetQueueUrlsAsync(_client, "").ToBlockingEnumerable().SelectMany(x => x)).Dump();
	
	//var queueUrl = await GetQueueUrlAsync(_client, "topica_web_analytics_queue_sales_v1.fifo").Dump();
	
	//await GetQueueAttributes(_client, await GetQueueUrlAsync(_client, "dte-sandbox-manual-1.fifo")).Dump();
	
	await Task.CompletedTask;
}