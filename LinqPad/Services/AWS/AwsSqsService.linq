<Query Kind="Program">
  <NuGetReference>AWSSDK.SimpleNotificationService</NuGetReference>
  <NuGetReference>AWSSDK.SQS</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Amazon</Namespace>
  <Namespace>Amazon.Auth.AccessControlPolicy</Namespace>
  <Namespace>Amazon.DynamoDBv2</Namespace>
  <Namespace>Amazon.DynamoDBv2.DataModel</Namespace>
  <Namespace>Amazon.DynamoDBv2.DocumentModel</Namespace>
  <Namespace>Amazon.DynamoDBv2.Internal</Namespace>
  <Namespace>Amazon.DynamoDBv2.Model</Namespace>
  <Namespace>Amazon.DynamoDBv2.Model.Internal.MarshallTransformations</Namespace>
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
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Bson</Namespace>
  <Namespace>Newtonsoft.Json.Converters</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>Newtonsoft.Json.Schema</Namespace>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
  <Namespace>static UserQuery.AwsQueueAttributes</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>ThirdParty.Ionic.Zlib</Namespace>
  <Namespace>ThirdParty.MD5</Namespace>
</Query>

// SQS QUEUES

private const string ErrorQueueSuffix = "_error";
private const string FifoQueueSuffix = ".fifo";
private const int DefaultMaxReceiveCount = 3;

public async Task<bool> QueueExitsByQueueNameAsync(IAmazonSQS sqsClient, string queueName)
{
	try
	{
		var request = new GetQueueUrlRequest { QueueName = queueName };
		var response = await sqsClient.GetQueueUrlAsync(request);

		return response != null && !string.IsNullOrWhiteSpace(response.QueueUrl);
	}
	catch (QueueDoesNotExistException)
	{
		return false;
	}
}

public async Task<bool> QueueExitsByQueueUrlAsync(IAmazonSQS sqsClient, string queueUrl)
{
	try
	{
		var getQueueAttributesRequest = new GetQueueAttributesRequest
		{
			AttributeNames = new List<string> { "All" },
			QueueUrl = queueUrl
		};

		var queueAttributes = (await sqsClient.GetQueueAttributesAsync(getQueueAttributesRequest)).Attributes.Dump();
		queueAttributes.Add("QueueUrl", queueUrl);

		return queueAttributes.ContainsKey("QueueArn") && !string.IsNullOrWhiteSpace(queueAttributes["QueueArn"]);
	}
	catch (AmazonSQSException ex)
	{
		if (ex.ErrorCode == "AWS.SimpleQueueService.NonExistentQueue") return false;

		Console.WriteLine($"SQS: Error - {ex.Message}");
		throw;
	}
	catch (Exception ex)
	{
		Console.WriteLine($"Error - {ex.Message}");
		throw;
	}
}

public async IAsyncEnumerable<IEnumerable<string>> GetQueueUrlsAsync(IAmazonSQS sqsClient, string queueNamePrefix, bool showNameOfQueueOnly = false)
{
	Func<string, string> parseName = x =>
	{
		if (string.IsNullOrWhiteSpace(x) || !x.Contains("/")) return x;

		var lastEntryQueueName = x.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).LastOrDefault();

		return !string.IsNullOrEmpty(lastEntryQueueName) ? lastEntryQueueName : x;
	};

	var response = await sqsClient.ListQueuesAsync(new ListQueuesRequest 
	{
		QueueNamePrefix = queueNamePrefix,
		MaxResults = 1000
	});

	if (response == null || response.QueueUrls == null || !response.QueueUrls.Any()) yield break;

	yield return showNameOfQueueOnly ? response.QueueUrls.Select(parseName) : response.QueueUrls;

	while (!string.IsNullOrWhiteSpace(response.NextToken))
	{
		response = await sqsClient.ListQueuesAsync(new ListQueuesRequest
		{
			QueueNamePrefix = queueNamePrefix,
			NextToken = response.NextToken,
			MaxResults = 1000
		});
		if (response == null || response.QueueUrls == null || !response.QueueUrls.Any()) yield break;
		yield return showNameOfQueueOnly ? response.QueueUrls.Select(parseName) : response.QueueUrls;
	}
}

public async Task<string> GetQueueUrlAsync(IAmazonSQS sqsClient, string queueName)
{
	try
	{
		var response = await sqsClient.GetQueueUrlAsync(queueName);

		if (response != null && !string.IsNullOrWhiteSpace(response.QueueUrl))
		{
			return response.QueueUrl;
		}

		throw new ApplicationException($"Can not find the queue named: {queueName} on your account");
	}
	catch (QueueDoesNotExistException)
	{
		Console.WriteLine($"Queue: {queueName} does not exist");
		return null;
	}
}

public async Task<Dictionary<string, string>> GetQueueAttributesAsync(IAmazonSQS sqsClient, string queueUrl)
{
	try
	{
		var getQueueAttributesRequest = new GetQueueAttributesRequest
		{
			AttributeNames = new List<string> { "All" },
			QueueUrl = queueUrl
		};

		var queueAttributes = (await sqsClient.GetQueueAttributesAsync(getQueueAttributesRequest)).Attributes;
		queueAttributes.Add("QueueUrl", queueUrl);

		return queueAttributes;
	}
	catch (QueueDoesNotExistException)
	{
		Console.WriteLine($"Queue: {queueUrl} does not exist");
		return null;
	}
	catch (AmazonSQSException ex)
	{
		Console.WriteLine($"SQS: Error - {ex.Message}");
		throw;
	}
	catch (Exception ex)
	{
		Console.WriteLine($"Error - {ex.Message}");
		throw;
	}
}

public async Task<(string, string)> CreateQueueWithErrorQueue(IAmazonSQS sqsClient, string queueName, SqsConfiguration configuration)
{
	//Create Error Queue
	//Create normal queue passing in redrive policy
	var isFifo = configuration.QueueAttributes.IsFifoQueue;
	if (isFifo) queueName = queueName.Replace(".fifo", string.Empty);
	var errorQueueUrl = await InternalCreateErrorQueue(sqsClient, $"{queueName}{ErrorQueueSuffix}{(isFifo ? FifoQueueSuffix : "")}", configuration);
	var errorQueueArn = await GetQueueArn(sqsClient, errorQueueUrl);

	var redrivePolicy = new RedrivePolicy(configuration.MaxReceiveCount ?? DefaultMaxReceiveCount, errorQueueArn).ToJson();

	var mainQueueUrl = await InternalMainCreateQueue(sqsClient, $"{queueName}{(isFifo ? FifoQueueSuffix : "")}", configuration, redrivePolicy);

	return (mainQueueUrl, errorQueueUrl);
}

public async Task<string> CreateQueueSole(IAmazonSQS sqsClient, string queueName, SqsConfiguration configuration)
{
	var isFifo = configuration.QueueAttributes.IsFifoQueue;
	if (isFifo) queueName = queueName.Replace(".fifo", string.Empty);

	var request = new CreateQueueRequest
	{
		QueueName = $"{queueName}{(isFifo ? FifoQueueSuffix : "")}",
		Attributes = configuration.QueueAttributes.GetAttributeDictionary()
	};

	var response = await sqsClient.CreateQueueAsync(request);

	if (response.HttpStatusCode.Dump("HttpStatusCode") == HttpStatusCode.OK)
	{
		return response.QueueUrl;
	}

	throw new ApplicationException($"Error creating queue, response from AWS: { JsonConvert.SerializeObject(response) }");
}

private async Task<string> InternalCreateErrorQueue(IAmazonSQS sqsClient, string queueName, SqsConfiguration configuration)
{
	return await InternalCreateQueue(sqsClient, queueName , configuration.QueueAttributes.GetAttributeDictionary());
}

private async Task<string> InternalMainCreateQueue(IAmazonSQS sqsClient, string queueName, SqsConfiguration configuration, string redrivePolicy)
{
	var attributeDictionary = configuration.QueueAttributes.GetAttributeDictionary();
	attributeDictionary.Add(AwsQueueAttributes.RedrivePolicyName, redrivePolicy);

	return await InternalCreateQueue(sqsClient, queueName, attributeDictionary);
}

private async Task<string> InternalCreateQueue(IAmazonSQS sqsClient, string queueName, Dictionary<string, string> attributes)
{
	var request = new CreateQueueRequest
	{
		QueueName = queueName,
		Attributes = attributes
	};

	var response = await sqsClient.CreateQueueAsync(request);

	if (response.HttpStatusCode == HttpStatusCode.OK)
	{
		return response.QueueUrl;
	}

	throw new ApplicationException($"Error creating queue, response from AWS: { JsonConvert.SerializeObject(response) }");
}

private async Task<string> GetQueueArn(IAmazonSQS sqsClient, string queueUrl)
{
	var getQueueAttributesRequest = new GetQueueAttributesRequest
	{
		AttributeNames = new List<string> { AwsQueueAttributes.QueueArnName },
		QueueUrl = queueUrl
	};

	var queueAttributes = (await sqsClient.GetQueueAttributesAsync(getQueueAttributesRequest)).Attributes;

	return queueAttributes[AwsQueueAttributes.QueueArnName];
}

public class SqsConfiguration
{
	public SqsConfiguration()
	{
		QueueAttributes = new AwsQueueAttributes();
	}

	public AwsQueueAttributes QueueAttributes { get; set; }
	public bool? CreateErrorQueue { get; set; }
	public int? MaxReceiveCount { get; set; }
}

public class AwsQueueAttributes
{
	public const int DelaySecondsMin = 0;
	public const int DelaySecondsMax = 900;
	public const int MaximumMessageSizeMin = 1024;
	public const int MaximumMessageSizeMax = 262144;
	public const int MessageRetentionPeriodMin = 60;
	public const int MessageRetentionPeriodMax = 1209600;
	public const int ReceiveMessageWaitTimeSecondsMin = 0;
	public const int ReceiveMessageWaitTimeSecondsMax = 20;
	public const int VisibilityTimeoutMin = 0;
	public const int VisibilityTimeoutMax = 43200;

	public const string AllName = "All";
	public const string DelaySecondsName = "DelaySeconds";
	public const string MaximumMessageSizeName = "MaximumMessageSize";
	public const string MessageRetentionPeriodName = "MessageRetentionPeriod";
	public const string PolicyName = "Policy";
	public const string ReceiveMessageWaitTimeSecondsName = "ReceiveMessageWaitTimeSeconds";
	public const string VisibilityTimeoutName = "VisibilityTimeout";
	public const string RedrivePolicyName = "RedrivePolicy";
	public const string QueueArnName = "QueueArn";
	public const string ApproximateNumberOfMessagesName = "ApproximateNumberOfMessages";
	public const string ApproximateNumberOfMessagesNotVisibleName = "ApproximateNumberOfMessagesNotVisible";
	public const string ApproximateNumberOfMessagesDelayedName = "ApproximateNumberOfMessagesDelayed";
	public const string CreatedTimestampName = "CreatedTimestamp";
	public const string LastModifiedTimestampName = "LastModifiedTimestamp";
	public const string IsFifoQueueName = "FifoQueue";
	public const string FifoContentBasedDeduplicationName = "ContentBasedDeduplication";

	public int? DelaySeconds { get; set; }
	public int? MaximumMessageSize { get; set; }
	public int? MessageRetentionPeriod { get; set; } = 1209600;
	public string Policy { get; set; }
	public int? ReceiveMessageWaitTimeSeconds { get; set; }
	public int? VisibilityTimeout { get; set; }
	public bool IsFifoQueue { get; set; }
	public bool IsFifoContentBasedDeduplication { get; set; }

	public Dictionary<string, string> GetAttributeDictionary()
	{
		Validate();

		var properties = new Dictionary<string, string>();

		if (DelaySeconds.HasValue) properties.Add(DelaySecondsName, DelaySeconds.Value.ToString());
		if (MaximumMessageSize.HasValue) properties.Add(MaximumMessageSizeName, MaximumMessageSize.Value.ToString());
		if (MessageRetentionPeriod.HasValue) properties.Add(MessageRetentionPeriodName, MessageRetentionPeriod.Value.ToString());
		if (!string.IsNullOrWhiteSpace(Policy)) properties.Add(PolicyName, Policy);
		if (ReceiveMessageWaitTimeSeconds.HasValue) properties.Add(ReceiveMessageWaitTimeSecondsName, ReceiveMessageWaitTimeSeconds.Value.ToString());
		if (VisibilityTimeout.HasValue) properties.Add(VisibilityTimeoutName, VisibilityTimeout.Value.ToString());
		if (IsFifoQueue)
		{
			properties.Add(IsFifoQueueName, "true");
			properties.Add(FifoContentBasedDeduplicationName, IsFifoContentBasedDeduplication.ToString().ToLower());
		}

		return properties;
	}

	private void Validate()
	{
		if (DelaySeconds.HasValue)
		{
			if (DelaySeconds.Value < DelaySecondsMin || DelaySeconds.Value > DelaySecondsMax)
				throw new AwsQueueAttributesException($"AWS: DelaySeconds must be between 0 and 300, currently: {DelaySeconds.Value}");
		}

		if (MaximumMessageSize.HasValue)
		{
			if (MaximumMessageSize.Value < MaximumMessageSizeMin || MaximumMessageSize.Value > MaximumMessageSizeMax)
				throw new AwsQueueAttributesException($"AWS: MaximumMessageSize must be between 1024 and 262144, currently: {MaximumMessageSize.Value}");
		}

		if (MessageRetentionPeriod.HasValue)
		{
			if (MessageRetentionPeriod.Value < MessageRetentionPeriodMin || MessageRetentionPeriod.Value > MessageRetentionPeriodMax)
				throw new AwsQueueAttributesException($"AWS: MessageRetentionPeriod must be between 60 and 1209600, currently: {MessageRetentionPeriod.Value}");
		}

		if (ReceiveMessageWaitTimeSeconds.HasValue)
		{
			if (ReceiveMessageWaitTimeSeconds.Value < ReceiveMessageWaitTimeSecondsMin || ReceiveMessageWaitTimeSeconds.Value > ReceiveMessageWaitTimeSecondsMax)
				throw new AwsQueueAttributesException($"AWS: ReceiveMessageWaitTimeSeconds must be between 0 and 20, currently: {ReceiveMessageWaitTimeSeconds.Value}");
		}

		if (VisibilityTimeout.HasValue)
		{
			if (VisibilityTimeout.Value < VisibilityTimeoutMin || VisibilityTimeout.Value > VisibilityTimeoutMax)
				throw new AwsQueueAttributesException($"AWS: VisibilityTimeout must be between 0 and 43200, currently: {VisibilityTimeout.Value}");
		}
	}

	public class AwsQueueAttributesException : Exception
	{
		public AwsQueueAttributesException(string message) : base(message) { }
	}

	public class RedrivePolicy
	{
		[Newtonsoft.Json.JsonProperty(PropertyName = "maxReceiveCount")]
		public int MaxReceiveCount { get; set; }

		[Newtonsoft.Json.JsonProperty(PropertyName = "deadLetterTargetArn")]
		public string DeadLetterTargetArn { get; set; }

		public RedrivePolicy(int maxReceiveCount, string deadLetterTargetArn)
		{
			MaxReceiveCount = maxReceiveCount;
			DeadLetterTargetArn = deadLetterTargetArn;
		}

		public string ToJson() => ToString();

		public override string ToString() => $"{{\"maxReceiveCount\":\"{MaxReceiveCount}\", \"deadLetterTargetArn\":\"{DeadLetterTargetArn}\"}}";

		public static RedrivePolicy ConvertFromString(string policy) => JsonConvert.DeserializeObject<RedrivePolicy>(policy) ?? throw new ArgumentException("Invalid RedrivePolicy JSON string.", nameof(policy));
	}
}
