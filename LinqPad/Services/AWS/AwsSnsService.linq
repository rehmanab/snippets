<Query Kind="Program">
  <NuGetReference>AWSSDK.SimpleNotificationService</NuGetReference>
  <NuGetReference>AWSSDK.SQS</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Amazon.SimpleNotificationService.Model</Namespace>
  <Namespace>Amazon.SimpleNotificationService</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Net</Namespace>
  <RemoveNamespace>System.Collections</RemoveNamespace>
  <RemoveNamespace>System.Data</RemoveNamespace>
  <RemoveNamespace>System.Diagnostics</RemoveNamespace>
  <RemoveNamespace>System.IO</RemoveNamespace>
  <RemoveNamespace>System.Linq.Expressions</RemoveNamespace>
  <RemoveNamespace>System.Reflection</RemoveNamespace>
  <RemoveNamespace>System.Text</RemoveNamespace>
  <RemoveNamespace>System.Text.RegularExpressions</RemoveNamespace>
  <RemoveNamespace>System.Threading</RemoveNamespace>
  <RemoveNamespace>System.Transactions</RemoveNamespace>
  <RemoveNamespace>System.Xml</RemoveNamespace>
  <RemoveNamespace>System.Xml.Linq</RemoveNamespace>
  <RemoveNamespace>System.Xml.XPath</RemoveNamespace>
</Query>

// SNS TOPICS

private const string FifoTopicSuffix = ".fifo";

// Topics

public async IAsyncEnumerable<IEnumerable<Topic>> GetAllTopics(IAmazonSimpleNotificationService snsClient, string topicNamePrefix = null)
{
	Func<Topic, bool> filterTopics = x =>
	{
		if(string.IsNullOrWhiteSpace(topicNamePrefix)) return true;
		
		if (string.IsNullOrWhiteSpace(x.TopicArn) || !x.TopicArn.Contains(":")) return false;

		var lastEntryTopicName = x.TopicArn.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).LastOrDefault();

		return !string.IsNullOrEmpty(lastEntryTopicName) && lastEntryTopicName.ToLower().StartsWith(topicNamePrefix.ToLower());
	};

	var response = await snsClient.ListTopicsAsync();

	if (response == null || response.Topics == null || !response.Topics.Any()) yield break;

	yield return response.Topics.Where(filterTopics);

	while (!string.IsNullOrWhiteSpace(response.NextToken))
	{
		response = await snsClient.ListTopicsAsync(response.NextToken);
		if (response == null || response.Topics == null || !response.Topics.Any()) yield break;
		yield return response.Topics.Where(filterTopics);
	}
}

public async Task<string> GetTopicArnAsync(IAmazonSimpleNotificationService snsClient, string topicName, bool isFifo)
{
	string topicArnFound = null;
	var found = false;
	string nextToken = string.Empty;

	do
	{
		var response = await snsClient.ListTopicsAsync(nextToken);
		if (response == null || response.Topics == null) break;

		topicArnFound = response.Topics?.Select(x => x.TopicArn).Where(y => y.ToLower().EndsWith($"{topicName.ToLower()}{(isFifo && !topicName.ToLower().EndsWith(FifoTopicSuffix) ? FifoTopicSuffix : "")}")).SingleOrDefault();
		if (!string.IsNullOrWhiteSpace(topicArnFound)) found = true;
		nextToken = response.NextToken;
	}
	while (!found && !string.IsNullOrEmpty(nextToken));

	return !string.IsNullOrWhiteSpace(topicArnFound) ? topicArnFound : null;
}

// Subscriptions

public async IAsyncEnumerable<IEnumerable<Subscription>> GetAllSubscriptions(IAmazonSimpleNotificationService snsClient, string prefix)
{
	Func<string, string> parseName = x => x.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
	Func<Subscription, bool> filterTopics = x =>
	{
		var topicName = parseName(x.TopicArn);

		return !string.IsNullOrWhiteSpace(topicName) && topicName.ToLower().StartsWith(prefix.ToLower());
	};

	var response = await snsClient.ListSubscriptionsAsync();

	if (response.Subscriptions == null || !response.Subscriptions.Any()) yield break;

	yield return response.Subscriptions.Where(filterTopics);

	while (!string.IsNullOrWhiteSpace(response.NextToken))
	{
		response = await snsClient.ListSubscriptionsAsync(response.NextToken);
		if (response.Subscriptions == null || !response.Subscriptions.Any()) yield break;
		yield return response.Subscriptions.Where(filterTopics);
	}
}

public async IAsyncEnumerable<IEnumerable<Subscription>> GetSubscriptions(IAmazonSimpleNotificationService snsClient, string topicArn)
{
	var response = await snsClient.ListSubscriptionsByTopicAsync(topicArn);

	if (response == null || response.Subscriptions == null || !response.Subscriptions.Any()) yield break;

	yield return response.Subscriptions;

	while (!string.IsNullOrWhiteSpace(response.NextToken))
	{
		response = await snsClient.ListSubscriptionsByTopicAsync(topicArn, response.NextToken);
		if (response == null || response.Subscriptions == null || !response.Subscriptions.Any()) yield break;
		yield return response.Subscriptions;
	}
}

public async Task<bool> SubscriptionExistsAsync(IAmazonSimpleNotificationService snsClient, string topicArn, string endpointArn)
{
	var subscriptions = GetSubscriptions(snsClient, topicArn).ToBlockingEnumerable().SelectMany(x => x);

	if (subscriptions == null || !subscriptions.Any()) return false;
	
	await Task.CompletedTask;

	return subscriptions.Any(x => string.Equals(endpointArn, x.Endpoint, StringComparison.CurrentCultureIgnoreCase));
}
