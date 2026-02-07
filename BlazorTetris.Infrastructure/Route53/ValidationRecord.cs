using System;
using Pulumi;
using Pulumi.Aws.Acm.Outputs;
using Pulumi.Aws.Route53;

namespace BlazorTetris.Infrastructure.Route53;

public class ValidationRecordArgs
{
    public required CertificateDomainValidationOption ValidationOption { get; init; }
    public required string ZoneId { get; init; }
}

public class ValidationRecord
{
    public Record Record { get; }

    public ValidationRecord(string name, ValidationRecordArgs args, CustomResourceOptions options)
    {
        if (args.ValidationOption.DomainName is null)
            throw new ArgumentException("args.ValidationOption.DomainName cannot be null");
        if (args.ValidationOption.ResourceRecordName is null)
            throw new ArgumentException("args.ValidationOption.ResourceRecordName cannot be null");
        if (args.ValidationOption.ResourceRecordType is null)
            throw new ArgumentException("args.ValidationOption.ResourceRecordType cannot be null");
        if (args.ValidationOption.ResourceRecordValue is null)
            throw new ArgumentException("args.ValidationOption.ResourceRecordValue cannot be null");

        var validationOption = args.ValidationOption;

        Record = new Record(name, new RecordArgs
        {
            AllowOverwrite = true,
            Name = validationOption.ResourceRecordName,
            Records = [ validationOption.ResourceRecordValue ],
            Ttl = 60,
            Type = validationOption.ResourceRecordType,
            ZoneId = args.ZoneId
        }, options);
    }
}
