using BlazorTetris.Infrastructure.Components;
using Pulumi;

// ReSharper disable UnusedVariable

return await Deployment.RunAsync(() =>
{
    // Config
    var prefix = $"{Deployment.Instance.ProjectName}-{Deployment.Instance.StackName}";
    var config = new Config();
    var mainZoneId = config.Require("main-zone-id");
    var mainDomain = config.Require("main-domain");

    var providers = new Providers(prefix, new ProvidersArgs
    {
        DnsAccountId = config.Require("dns-account-id"),
        DnsIacRoleArn = config.Require("dns-iac-role-arn"),
        EnvAccountId = config.Require("env-account-id"),
        EnvIacRoleArn = config.Require("env-iac-role-arn")
    });

    var buckets = new Buckets(prefix, new BucketsArgs
    {
        EnvProvider = providers.EnvProvider
    });

    var validatedCertificates = new ValidatedCertificates(prefix, new ValidatedCertificatesArgs
    {
        DnsProvider = providers.DnsProvider,
        EnvProvider = providers.EnvProvider,
        PrimaryDomain = mainDomain,
        SubjectAlternativeNames = new InputList<string>(),
        HostedZoneId = mainZoneId
    });

    var distributions = new Distributions(prefix, new DistributionsArgs
    {
        EnvProvider = providers.EnvProvider,
        SourceBucket = buckets.SourceBucket,
        MainCertificate = validatedCertificates.MainCertificate,
        MainCertificateValidation = validatedCertificates.MainCertificateValidation,
        MainDistributionDomain = mainDomain
    });

    buckets.CreateSourceBucketPolicy(prefix, distributions.MainDistribution);

    var records = new Records(prefix, new RecordsArgs
    {
        DnsProvider = providers.DnsProvider,
        MainDistribution = distributions.MainDistribution,
        MainZoneId = mainZoneId
    });
});
