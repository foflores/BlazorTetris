using System;

namespace BlazorTetris.Infrastructure;

public interface IBlazorTetrisConfig
{
    string HostedZoneId { get; }
    string Domain { get; }
    string AwsAppAccountId { get; }
    string AwsAppRegionId { get; }
    string AwsDnsAccountId { get; }
    string AwsDnsRegionId { get; }
    string DistributionId { get; }
    string DnsQualifierId { get; }
    string DnsStackName { get; }
}

public class BlazorTetrisConfigResolver
{
    private readonly string? _accountId;
    private readonly string? _regionId;

    public BlazorTetrisConfigResolver(string? accountId, string? regionId)
    {
        _accountId = accountId;
        _regionId = regionId;
    }

    public IBlazorTetrisConfig? GetConfig()
    {
        if (string.Equals(_accountId, "412433735452")
            && string.Equals(_regionId, "us-east-1", StringComparison.OrdinalIgnoreCase))
        {
            return new BlazorTetrisDevelopmentConfig();
        }
        if (string.Equals(_accountId, "633067888675")
            && string.Equals(_regionId, "us-east-1", StringComparison.OrdinalIgnoreCase))
        {
            return new BlazorTetrisProductionConfig();
        }

        return null;
    }

    private class BlazorTetrisDevelopmentConfig : IBlazorTetrisConfig
    {
        public string HostedZoneId => "Z064174917OLIOD8DL3OY";
        public string Domain => "favianflores.net";
        public string AwsAppAccountId => "412433735452";
        public string AwsAppRegionId => "us-east-1";
        public string AwsDnsAccountId => "888359517863";
        public string AwsDnsRegionId => "us-east-1";
        public string DistributionId => "";
        public string DnsQualifierId => "dev";
        public string DnsStackName => "BlazorTetrisDnsDevelopment";
    }

    private class BlazorTetrisProductionConfig : IBlazorTetrisConfig
    {
        public string HostedZoneId => "Z0926047XIN35SIX6DXS";
        public string Domain => "favianflores.com";
        public string AwsAppAccountId => "633067888675";
        public string AwsAppRegionId => "us-east-1";
        public string AwsDnsAccountId => "888359517863";
        public string AwsDnsRegionId => "us-east-1";
        public string DistributionId => "EX7KZ251EZG0X";
        public string DnsQualifierId => "prod";
        public string DnsStackName => "BlazorTetrisDnsProduction";
    }
}
