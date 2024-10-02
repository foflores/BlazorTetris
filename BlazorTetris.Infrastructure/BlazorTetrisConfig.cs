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
}

public class BlazorTetrisDevelopmentConfig : IBlazorTetrisConfig
{
    public string HostedZoneId => "Z064174917OLIOD8DL3OY";
    public string Domain => "favianflores.net";
    public string AwsAppAccountId => "412433735452";
    public string AwsAppRegionId => "us-east-1";
    public string AwsDnsAccountId => "888359517863";
    public string AwsDnsRegionId => "us-east-1";
    public string DistributionId => "";
    public string DnsQualifierId => "dev";
}

public class BlazorTetrisProductionConfig : IBlazorTetrisConfig
{
    public string HostedZoneId => "Z0926047XIN35SIX6DXS";
    public string Domain => "favianflores.com";
    public string AwsAppAccountId => "633067888675";
    public string AwsAppRegionId => "us-east-1";
    public string AwsDnsAccountId => "888359517863";
    public string AwsDnsRegionId => "us-east-1";
    public string DistributionId => "EX7KZ251EZG0X";
    public string DnsQualifierId => "prod";
}
