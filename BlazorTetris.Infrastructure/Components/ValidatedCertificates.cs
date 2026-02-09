using System.Collections.Generic;
using System.Linq;
using Pulumi;
using Pulumi.Aws;
using Pulumi.Aws.Acm;
using Pulumi.Aws.Route53;

namespace BlazorTetris.Infrastructure.Components;

public class ValidatedCertificatesArgs
{
    public required Provider DnsProvider { get; init; }
    public required Provider EnvProvider { get; init; }
    public required Input<string> PrimaryDomain { get; init; }
    public required InputList<string> SubjectAlternativeNames { get; init; }
    public required Input<string> HostedZoneId { get; init; }
}

public class ValidatedCertificates
{
    public Certificate MainCertificate { get; }
    public CertificateValidation MainCertificateValidation { get; }

    public ValidatedCertificates(string prefix, ValidatedCertificatesArgs args)
    {
        MainCertificate = new Certificate($"{prefix}-certicate-main", new CertificateArgs
        {
            DomainName = args.PrimaryDomain,
            SubjectAlternativeNames = args.SubjectAlternativeNames,
            ValidationMethod = "DNS"
        }, new CustomResourceOptions { Provider = args.EnvProvider });

        var records = MainCertificate.DomainValidationOptions.Apply(domainValidationOptions =>
        {
            List<Record> records = [];
            foreach (var option in domainValidationOptions)
            {
                if (option.DomainName is null
                    || option.ResourceRecordName is null
                    || option.ResourceRecordType is null
                    || option.ResourceRecordValue is null)
                {
                    continue;
                }

                records.Add(new Record($"{prefix}-record-dnsvalidation-{option.DomainName}", new RecordArgs
                {
                    AllowOverwrite = true,
                    Name = option.ResourceRecordName,
                    Records = [ option.ResourceRecordValue ],
                    Ttl = 60,
                    Type = option.ResourceRecordType,
                    ZoneId = args.HostedZoneId
                }, new CustomResourceOptions { Provider = args.DnsProvider }));
            }

            return Output.All(records.Select(y => y.Fqdn));
        });

        MainCertificateValidation = new CertificateValidation($"{prefix}-certificatevalidation-main", new CertificateValidationArgs
        {
            CertificateArn = MainCertificate.Arn,
            ValidationRecordFqdns = records
        }, new CustomResourceOptions { Provider = args.EnvProvider });
    }
}
