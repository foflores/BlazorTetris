using Pulumi;
using Pulumi.Aws;
using Pulumi.Aws.CloudFront;
using Pulumi.Aws.Route53;

namespace BlazorTetris.Infrastructure.Components;

public class RecordsArgs
{
    public required Provider DnsProvider { get; init; }
    public required Distribution MainDistribution { get; init; }
    public required string MainZoneId { get; init; }
}

public class Records
{
    public Record MainRecord { get; }
    public Records(string prefix, RecordsArgs args)
    {
        MainRecord = new Record($"{prefix}-record-main", new RecordArgs
        {
            Name = "tetris",
            Ttl = 300,
            Type = "CNAME",
            Records = [ args.MainDistribution.DomainName ],
            ZoneId = args.MainZoneId
        }, new CustomResourceOptions { Provider = args.DnsProvider });
    }
}
