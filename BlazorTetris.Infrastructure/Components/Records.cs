using Pulumi;
using Pulumi.Aws;
using Pulumi.Aws.CloudFront;
using Pulumi.Aws.Route53;

namespace BlazorTetris.Infrastructure.Components;

public class RecordsArgs
{
    public required Provider DnsProvider { get; init; }
    public required Distribution Distribution { get; init; }
    public required string ZoneId { get; init; }
}

public class Records
{
    public Record TetrisRecord { get; }
    public Records(string prefix, RecordsArgs args)
    {
        TetrisRecord = new Record($"{prefix}-record-main", new RecordArgs
        {
            Name = "tetris",
            Ttl = 300,
            Type = "CNAME",
            Records = [ args.Distribution.DomainName ],
            ZoneId = args.ZoneId
        }, new CustomResourceOptions { Provider = args.DnsProvider });
    }
}
