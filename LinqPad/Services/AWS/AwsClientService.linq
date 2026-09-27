<Query Kind="Program">
  <NuGetReference>AWSSDK.CloudWatchLogs</NuGetReference>
  <NuGetReference>AWSSDK.CognitoIdentityProvider</NuGetReference>
  <NuGetReference>AWSSDK.DynamoDBv2</NuGetReference>
  <NuGetReference>AWSSDK.EC2</NuGetReference>
  <NuGetReference>AWSSDK.ECS</NuGetReference>
  <NuGetReference>AWSSDK.Extensions.NETCore.Setup</NuGetReference>
  <NuGetReference>AWSSDK.Lambda</NuGetReference>
  <NuGetReference>AWSSDK.S3</NuGetReference>
  <NuGetReference>AWSSDK.SecretsManager</NuGetReference>
  <NuGetReference>AWSSDK.SecurityToken</NuGetReference>
  <NuGetReference>AWSSDK.SimpleNotificationService</NuGetReference>
  <NuGetReference>AWSSDK.SQS</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Configuration</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Configuration.UserSecrets</NuGetReference>
  <NuGetReference>Microsoft.Extensions.DependencyInjection</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Amazon</Namespace>
  <Namespace>Amazon.Auth.AccessControlPolicy</Namespace>
  <Namespace>Amazon.CloudWatchLogs</Namespace>
  <Namespace>Amazon.CloudWatchLogs.Internal</Namespace>
  <Namespace>Amazon.CloudWatchLogs.Model</Namespace>
  <Namespace>Amazon.CloudWatchLogs.Model.Internal.MarshallTransformations</Namespace>
  <Namespace>Amazon.CognitoIdentityProvider</Namespace>
  <Namespace>Amazon.CognitoIdentityProvider.Internal</Namespace>
  <Namespace>Amazon.CognitoIdentityProvider.Model</Namespace>
  <Namespace>Amazon.CognitoIdentityProvider.Model.Internal.MarshallTransformations</Namespace>
  <Namespace>Amazon.DynamoDBv2</Namespace>
  <Namespace>Amazon.EC2</Namespace>
  <Namespace>Amazon.EC2.Endpoints</Namespace>
  <Namespace>Amazon.EC2.Internal</Namespace>
  <Namespace>Amazon.EC2.Model</Namespace>
  <Namespace>Amazon.EC2.Model.Internal.MarshallTransformations</Namespace>
  <Namespace>Amazon.ECS</Namespace>
  <Namespace>Amazon.ECS.Endpoints</Namespace>
  <Namespace>Amazon.ECS.Internal</Namespace>
  <Namespace>Amazon.ECS.Model</Namespace>
  <Namespace>Amazon.ECS.Model.Internal.MarshallTransformations</Namespace>
  <Namespace>Amazon.Lambda</Namespace>
  <Namespace>Amazon.Lambda.Internal</Namespace>
  <Namespace>Amazon.Lambda.Model</Namespace>
  <Namespace>Amazon.Lambda.Model.Internal.MarshallTransformations</Namespace>
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
  <Namespace>Amazon.S3</Namespace>
  <Namespace>Amazon.S3.Internal</Namespace>
  <Namespace>Amazon.S3.Model</Namespace>
  <Namespace>Amazon.S3.Model.Internal.MarshallTransformations</Namespace>
  <Namespace>Amazon.S3.Transfer</Namespace>
  <Namespace>Amazon.S3.Util</Namespace>
  <Namespace>Amazon.SecretsManager</Namespace>
  <Namespace>Amazon.SecretsManager.Internal</Namespace>
  <Namespace>Amazon.SecretsManager.Model</Namespace>
  <Namespace>Amazon.SecretsManager.Model.Internal.MarshallTransformations</Namespace>
  <Namespace>Amazon.SecurityToken</Namespace>
  <Namespace>Amazon.SecurityToken.Endpoints</Namespace>
  <Namespace>Amazon.SecurityToken.Internal</Namespace>
  <Namespace>Amazon.SecurityToken.Model</Namespace>
  <Namespace>Amazon.SecurityToken.Model.Internal.MarshallTransformations</Namespace>
  <Namespace>Amazon.SecurityToken.SAML</Namespace>
  <Namespace>Amazon.SimpleNotificationService</Namespace>
  <Namespace>Amazon.SQS</Namespace>
  <Namespace>Amazon.Util</Namespace>
  <Namespace>Amazon.Util.Internal</Namespace>
  <Namespace>Amazon.Util.Internal.PlatformServices</Namespace>
  <Namespace>AWSSDK.Runtime.Internal.Util</Namespace>
  <Namespace>Microsoft.Extensions.Configuration</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>ThirdParty.Ionic.Zlib</Namespace>
  <Namespace>ThirdParty.MD5</Namespace>
  <Namespace>ThirdParty.RuntimeBackports</Namespace>
</Query>


public enum Environment
{
	LocalStack, Floci, Sandbox, Dev, Qa, Uat, Oat, Prod
}

// docker run --rm -p 4566:4566 -d --name floci floci/floci:latest
// pointing to default parallels host Mac ipaddress (10.211.55.2) in hosts file (10.211.55.2 host), Parallels VM needs to be "Shared Network"
public const string defaultEmulatorServiceUrl = "http://localhost:4566";
public const string userSecretsName = "AWS.LinqPad.Secrets";

/// <summary>
/// Gets the AWS Credentials from UserSecrets
/// </summary>
public TClient GetAwsClient<TClient>(string? environmentName = null) where TClient : AmazonServiceClient
{
	var serviceName = typeof(TClient).Name;

	var serviceCollection = new ServiceCollection();
	var configuration = new ConfigurationBuilder().AddUserSecrets(userSecretsName).Build();
	var awsOptions = configuration.GetAWSOptions();

	if (string.IsNullOrWhiteSpace(environmentName))
	{
		awsOptions.DefaultClientConfig.ServiceURL = defaultEmulatorServiceUrl;

		Console.WriteLine($"Using Emulator: {serviceName}; ServiceUrl: {awsOptions.DefaultClientConfig.ServiceURL} ");
		Console.WriteLine();

		awsOptions.Credentials = new BasicAWSCredentials("test", "test");
	}
	else
	{
		var section = configuration.GetSection(environmentName);
		awsOptions.Region = RegionEndpoint.GetBySystemName(section["Region"]);
		var accessKey = section["AccessKey"];
		var secretKey = section["SecretKey"];
		var serviceURL = section["ServiceURL"];

		if (!string.IsNullOrWhiteSpace(accessKey) && !string.IsNullOrWhiteSpace(secretKey))
		{
			awsOptions.Credentials = new BasicAWSCredentials(accessKey, secretKey);
		}

		if (!string.IsNullOrWhiteSpace(serviceURL))
		{
			awsOptions.DefaultClientConfig.ServiceURL = serviceURL;
		}

		Console.WriteLine($"Using AWS UserSecrets section: {environmentName} : {awsOptions.Region} for {serviceName}");
		Console.WriteLine();
	}

	serviceCollection.AddDefaultAWSOptions(awsOptions);

	serviceCollection.AddAWSService<AmazonSQSClient>();
	serviceCollection.AddAWSService<AmazonSimpleNotificationServiceClient>();
	serviceCollection.AddAWSService<AmazonDynamoDBClient>();
	serviceCollection.AddAWSService<AmazonSecretsManagerClient>();
	serviceCollection.AddAWSService<AmazonCognitoIdentityProviderClient>();
	serviceCollection.AddAWSService<AmazonS3Client>();
	serviceCollection.AddAWSService<AmazonLambdaClient>();
	serviceCollection.AddAWSService<AmazonEC2Client>();
	serviceCollection.AddAWSService<AmazonECSClient>();
	serviceCollection.AddAWSService<AmazonCloudWatchLogsClient>();
	serviceCollection.AddAWSService<AmazonSecurityTokenServiceClient>();

	return serviceCollection.BuildServiceProvider().GetService<TClient>() ?? throw new Exception($"Failed to get {serviceName}");
}

/// <summary>
/// Gets the AWS Credentials from AWS profile (.aws) user folder
/// </summary>
public TClient GetAwsClient<TClient, TConfig>(string? profileName = null, string regionEndpoint = "eu-west-2", string? serviceUrl = null)
	where TClient : AmazonServiceClient
	where TConfig : ClientConfig, new()
{
	var sharedFile = new SharedCredentialsFile();
	var config = new TConfig { RegionEndpoint = RegionEndpoint.GetBySystemName(regionEndpoint) };

	var serviceName = typeof(TClient).Name;

	if (string.IsNullOrWhiteSpace(profileName) || !sharedFile.TryGetProfile(profileName, out var profile))
	{
		config.ServiceURL = serviceUrl ?? defaultEmulatorServiceUrl;
		
		Console.WriteLine($"Using Emulator: {serviceName}; ServiceUrl: {config.ServiceURL} ");
		Console.WriteLine();

		var localCredentials = new BasicAWSCredentials("test", "test");

		return (TClient)Activator.CreateInstance(typeof(TClient), localCredentials, config)!;
	}

	AWSCredentialsFactory.TryGetAWSCredentials(profile, sharedFile, out var credentials);

	Console.WriteLine($"Using AWS profile: {profileName} : {regionEndpoint} for {serviceName}");
	Console.WriteLine();

	return (TClient)Activator.CreateInstance(typeof(TClient), credentials, config)!;
}

public AWSCredentials GetCredentialsFromProfile(string profileName)
{
	var sharedFile = new SharedCredentialsFile();
	sharedFile.TryGetProfile(profileName, out var profile);
	AWSCredentialsFactory.TryGetAWSCredentials(profile, sharedFile, out var credentials);

	return credentials;
}

public void AddCredentialsFromProfile(string profileName, string accessKey, string secret)
{
	var sharedFile = new SharedCredentialsFile();
	var exixts = sharedFile.TryGetProfile(profileName, out var profile);

	if (!exixts) Console.WriteLine("Aws credentials file does not exist");

	sharedFile.RegisterProfile(new CredentialProfile(profileName, new CredentialProfileOptions
	{
		AccessKey = accessKey,
		SecretKey = secret
	}));
}

[Obsolete($"Use {nameof(GetAwsClient)} ex. GetAwsClient<AmazonSQSClient, AmazonSQSConfig>(profileName)")]
public IAmazonSQS GetSqsClient(string? profileName = null, string regionEndpoint = "eu-west-2", string? serviceUrl = null) => GetAwsClient<AmazonSQSClient, AmazonSQSConfig>(profileName);

[Obsolete($"Use {nameof(GetAwsClient)} ex. GetAwsClient<AmazonSimpleNotificationServiceClient, AmazonSimpleNotificationServiceConfig>(profileName)")]
public IAmazonSimpleNotificationService GetSnsClient(string? profileName = null, string regionEndpoint = "eu-west-2", string? serviceUrl = null) => GetAwsClient<AmazonSimpleNotificationServiceClient, AmazonSimpleNotificationServiceConfig>(profileName);

[Obsolete($"Use {nameof(GetAwsClient)} ex. GetAwsClient<AmazonDynamoDBClient, AmazonDynamoDBConfig>(profileName)")]
public IAmazonDynamoDB GetDynamoDbClient(string? profileName = null, string regionEndpoint = "eu-west-2", string? serviceUrl = null) => GetAwsClient<AmazonDynamoDBClient, AmazonDynamoDBConfig>(profileName);

[Obsolete($"Use {nameof(GetAwsClient)} ex. GetAwsClient<AmazonSecretsManagerClient, AmazonSecretsManagerConfig>(profileName)")]
public IAmazonSecretsManager GetSecretsManagerClient(string? profileName = null, string regionEndpoint = "eu-west-2", string? serviceUrl = null) => GetAwsClient<AmazonSecretsManagerClient, AmazonSecretsManagerConfig>(profileName);

[Obsolete($"Use {nameof(GetAwsClient)} ex. GetAwsClient<AmazonCognitoIdentityProviderClient, AmazonCognitoIdentityProviderConfig>(profileName)")]
public IAmazonCognitoIdentityProvider GetCognitoClient(string? profileName = null, string regionEndpoint = "eu-west-2", string? serviceUrl = null) => GetAwsClient<AmazonCognitoIdentityProviderClient, AmazonCognitoIdentityProviderConfig>(profileName);

[Obsolete($"Use {nameof(GetAwsClient)} ex. GetAwsClient<AmazonS3Client, AmazonS3Config>(profileName)")]
public IAmazonS3 GetS3Client(string? profileName = null, string regionEndpoint = "eu-west-2", string? serviceUrl = null) => GetAwsClient<AmazonS3Client, AmazonS3Config>(profileName);

[Obsolete($"Use {nameof(GetAwsClient)} ex. GetAwsClient<AmazonLambdaClient, AmazonLambdaConfig>(profileName)")]
public IAmazonLambda GetLambdaClient(string? profileName = null, string regionEndpoint = "eu-west-2", string? serviceUrl = null) => GetAwsClient<AmazonLambdaClient, AmazonLambdaConfig>(profileName);

[Obsolete($"Use {nameof(GetAwsClient)} ex. GetAwsClient<AmazonEC2Client, AmazonEC2Config>(profileName)")]
public IAmazonEC2 GetEC2Client(string? profileName = null, string regionEndpoint = "eu-west-2", string? serviceUrl = null) => GetAwsClient<AmazonEC2Client, AmazonEC2Config>(profileName);

[Obsolete($"Use {nameof(GetAwsClient)} ex. GetAwsClient<AmazonECSClient, AmazonECSConfig>(profileName)")]
public IAmazonECS GetECSClient(string? profileName = null, string regionEndpoint = "eu-west-2", string? serviceUrl = null) => GetAwsClient<AmazonECSClient, AmazonECSConfig>(profileName);

[Obsolete($"Use {nameof(GetAwsClient)} ex. GetAwsClient<AmazonCloudWatchLogsClient, AmazonCloudWatchLogsConfig>(profileName)")]
public IAmazonCloudWatchLogs GetCloudWatchLogsClient(string? profileName = null, string regionEndpoint = "eu-west-2", string? serviceUrl = null) => GetAwsClient<AmazonCloudWatchLogsClient, AmazonCloudWatchLogsConfig>(profileName);

[Obsolete($"Use {nameof(GetAwsClient)} ex. GetAwsClient<AmazonSecurityTokenServiceClient, AmazonSecurityTokenServiceConfig>(profileName)")]
public IAmazonSecurityTokenService GetSecurityTokenServiceClient(string? profileName = null, string regionEndpoint = "eu-west-2", string? serviceUrl = null) => GetAwsClient<AmazonSecurityTokenServiceClient, AmazonSecurityTokenServiceConfig>(profileName);

// old eg.

/*
		var sharedFile = new SharedCredentialsFile();
		var config = new AmazonSecurityTokenServiceConfig { RegionEndpoint = RegionEndpoint.GetBySystemName(regionEndpoint) };
	
		if (string.IsNullOrWhiteSpace(profileName) || !sharedFile.TryGetProfile(profileName, out var profile))
		{
			Console.WriteLine("Using LocalStack: CloudWatchLogs");
			Console.WriteLine();
			config.ServiceURL = defaultLocalStackServiceUrl;
	
			return new AmazonSecurityTokenServiceClient(new BasicAWSCredentials("test", "test"), config);
		}
	
		AWSCredentialsFactory.TryGetAWSCredentials(profile, sharedFile, out var credentials);
	
		Console.WriteLine($"Using AWS profile: {profileName} : {regionEndpoint} for CloudWatchLogs client");
		Console.WriteLine();
		return new AmazonSecurityTokenServiceClient(credentials, config);
*/

//public TClient GetAwsClient<TClient, TConfig>(string? profileName = null, string regionEndpoint = "eu-west-2", string? serviceUrl = null)
//	where TClient : AmazonServiceClient
//	where TConfig : ClientConfig, new()
//{
//	var sharedFile = new SharedCredentialsFile();
//	var config = new TConfig { RegionEndpoint = RegionEndpoint.GetBySystemName(regionEndpoint) };

//	var serviceName = typeof(TClient).Name;

//	if (string.IsNullOrWhiteSpace(profileName) || !sharedFile.TryGetProfile(profileName, out var profile))
//	{
//		config.ServiceURL = serviceUrl ?? defaultEmulatorServiceUrl;

//		Console.WriteLine($"Using Emulator: {serviceName}; ServiceUrl: {config.ServiceURL} ");
//		Console.WriteLine();

//		var localCredentials = new BasicAWSCredentials("test", "test");

//		return (TClient)Activator.CreateInstance(typeof(TClient), localCredentials, config)!;
//	}

//	AWSCredentialsFactory.TryGetAWSCredentials(profile, sharedFile, out var credentials);

//	Console.WriteLine($"Using AWS profile: {profileName} : {regionEndpoint} for {serviceName}");
//	Console.WriteLine();

//	return (TClient)Activator.CreateInstance(typeof(TClient), credentials, config)!;
//}
