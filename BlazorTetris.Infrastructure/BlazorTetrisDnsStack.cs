using Amazon.CDK;
using Amazon.CDK.AWS.Route53;
using Constructs;

// ReSharper disable UnusedVariable

namespace BlazorTetris.Infrastructure;

public class BlazorTetrisDnsStack : Stack
{
    public BlazorTetrisDnsStack(Construct scope, string id, IStackProps props, IBlazorTetrisConfig config)
        : base(scope, id, props)
    {
        HostedZoneAttributes hostedZoneAttributes = new()
        {
            HostedZoneId = config.HostedZoneId,
            ZoneName = "favianflores.com"
        };
        var hostedZone = HostedZone.FromHostedZoneAttributes(this, "HostedZone", hostedZoneAttributes);

        CnameRecordProps tetrisRecordProps = new()
        {
            DomainName = "d1zip8nosxerk3.cloudfront.net",
            Zone = hostedZone,
            RecordName = "tetris",
            Ttl = Duration.Minutes(5)
        };
        CnameRecord tetrisRecord = new(this, "Tetris", tetrisRecordProps);
    }
}
