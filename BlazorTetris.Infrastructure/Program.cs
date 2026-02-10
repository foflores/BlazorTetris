using System.Collections.Generic;
using BlazorTetris.Infrastructure.Components;
using Pulumi;

// ReSharper disable UnusedVariable

return await Deployment.RunAsync(() =>
{
    // Config
    var prefix = $"{Deployment.Instance.ProjectName}-{Deployment.Instance.StackName}";
    var config = new Config();
    var zoneId = config.Require("zone-id");
    var domain = config.Require("domain");

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

    var certificates = new Certificates(prefix, new CertificatesArgs
    {
        DnsProvider = providers.DnsProvider,
        EnvProvider = providers.EnvProvider,
        Domain = domain,
        SubjectAlternativeNames = new InputList<string>(),
        ZoneId = zoneId
    });

    var distributions = new Distributions(prefix, new DistributionsArgs
    {
        EnvProvider = providers.EnvProvider,
        SourceBucket = buckets.SourceBucket,
        Certificate = certificates.Certificate,
        CertificateValidation = certificates.CertificateValidation,
        Domain = domain
    });

    buckets.CreateSourceBucketPolicy(distributions.Distribution);

    var records = new Records(prefix, new RecordsArgs
    {
        DnsProvider = providers.DnsProvider,
        Distribution = distributions.Distribution,
        ZoneId = zoneId
    });

    return new Dictionary<string, object?>
    {
        [$"{prefix}-bucket-source-arn"] = buckets.SourceBucket.Arn,
        [$"{prefix}-distribution-main-arn"] = distributions.Distribution.Arn
    };
});
