using System.Collections.Generic;
using System.Linq;
using BlazorTetris.Infrastructure.Route53;
using Pulumi;
using Pulumi.Aws.Acm;
using Pulumi.Aws.CloudFront;
using Pulumi.Aws.CloudFront.Inputs;
using Pulumi.Aws.Iam;
using Pulumi.Aws.Iam.Inputs;
using Pulumi.Aws.Route53;
using Pulumi.Aws.S3;

// ReSharper disable UnusedVariable

namespace BlazorTetris.Infrastructure.Environments;

public class DevelopmentEnvironmentArgs
{
    public required ProviderResource ManagementProvider { get; init; }
    public required ProviderResource DevelopmentProvider { get; init; }
    public required string ManagementZoneFavianFloresNetId { get; init; }
}

public class DevelopmentEnvironment
{
    public Bucket AppBucket { get; }
    public Distribution Distribution { get; }

    public DevelopmentEnvironment(DevelopmentEnvironmentArgs args)
    {
        const string appDomain = "tetris.favianflores.net";

        var developmentCertificate1 = new Certificate("blazorTetris-development-certificate-1", new CertificateArgs
        {
            DomainName = appDomain,
            ValidationMethod = "DNS"
        }, new CustomResourceOptions { Provider = args.DevelopmentProvider });

        var validationRecordFqdns = developmentCertificate1.DomainValidationOptions.Apply(x =>
        {
            if (x.Length < 1)
                return null;

            List<ValidationRecord> records =
            [
                new("blazorTetris-management-validationRecord-1", new ValidationRecordArgs
                {
                    ValidationOption = x[0],
                    ZoneId = args.ManagementZoneFavianFloresNetId
                }, new CustomResourceOptions { Provider = args.ManagementProvider })
            ];

            return Output.All(records.Select(y => y.Record.Fqdn));
        });

        var certificateValidation = new CertificateValidation(
            "blazorTetris-development-certificateValidation-1",
            new CertificateValidationArgs
            {
                CertificateArn = developmentCertificate1.Arn,
                ValidationRecordFqdns = validationRecordFqdns
            }, new CustomResourceOptions { Provider = args.DevelopmentProvider });

        AppBucket = new Bucket("blazorTetris-development-bucket-1", new BucketArgs
        {
            BucketPrefix = "blazor-tetris-bucket",
            ForceDestroy = true
        }, new CustomResourceOptions { Provider = args.DevelopmentProvider });

        var originAccessControl = new OriginAccessControl("blazorTetris-development-originAccessControl-1", new OriginAccessControlArgs
        {
            Name = "blazor-tetris-origin-access-control",
            OriginAccessControlOriginType = "s3",
            SigningBehavior = "always",
            SigningProtocol = "sigv4"
        }, new CustomResourceOptions { Provider = args.DevelopmentProvider });

        Distribution = new Distribution("blazorTetris-development-distribution-1", new DistributionArgs
        {
            Aliases = [ appDomain ],
            CustomErrorResponses =
            [
                new DistributionCustomErrorResponseArgs
                {
                    ErrorCode = 403,
                    ResponseCode = 404,
                    ResponsePagePath = "/index.html"
                }
            ],
            DefaultRootObject = "index.html",
            DefaultCacheBehavior = new DistributionDefaultCacheBehaviorArgs
            {
                AllowedMethods = ["GET", "HEAD"],
                CachePolicyId = "658327ea-f89d-4fab-a63d-7e88639e58f6",
                CachedMethods = ["GET", "HEAD"],
                Compress = true,
                TargetOriginId = "blazor-tetris-bucket-origin",
                ViewerProtocolPolicy = "redirect-to-https"
            },
            Enabled = true,
            HttpVersion = "http2and3",
            Origins = new[]
            {
                new DistributionOriginArgs
                {
                    DomainName = AppBucket.BucketRegionalDomainName,
                    OriginAccessControlId = originAccessControl.Id,
                    OriginId = "blazor-tetris-bucket-origin",
                }
            },
            PriceClass = "PriceClass_100",
            Restrictions = new DistributionRestrictionsArgs
            {
                GeoRestriction = new DistributionRestrictionsGeoRestrictionArgs
                {
                    Locations = [],
                    RestrictionType = "none"
                }
            },
            RetainOnDelete = false,
            ViewerCertificate = new DistributionViewerCertificateArgs
            {
                AcmCertificateArn = developmentCertificate1.Arn,
                SslSupportMethod = "sni-only",
                MinimumProtocolVersion = "TLSv1.2_2021"
            },
            WaitForDeployment = false,
        }, new CustomResourceOptions { Provider = args.DevelopmentProvider });

        var bucketPolicy = new BucketPolicy("blazorTetris-development-bucketPolicy-1", new BucketPolicyArgs
        {
            Bucket = AppBucket.BucketName,
            Policy = GetPolicyDocument.Invoke(new GetPolicyDocumentInvokeArgs
            {
                Version = "2012-10-17",
                Statements =
                [
                    new GetPolicyDocumentStatementInputArgs
                    {
                        Effect = "Allow",
                        Principals =
                        [
                            new GetPolicyDocumentStatementPrincipalInputArgs
                            {
                                Identifiers = ["cloudfront.amazonaws.com"],
                                Type = "Service"
                            }
                        ],
                        Actions = ["s3:GetObject"],
                        Resources = [ AppBucket.Arn.Apply(x => $"{x}/*") ],
                        Conditions =
                        [
                            new GetPolicyDocumentStatementConditionInputArgs
                            {
                                Test = "StringEquals",
                                Values = Distribution.Arn,
                                Variable = "AWS:SourceArn"
                            }
                        ],
                    }
                ]
            }, new InvokeOptions { Provider = args.DevelopmentProvider }).Apply(x => x.Json)
        }, new CustomResourceOptions { Provider = args.DevelopmentProvider });

        var dnsTetrisRecord = new Record($"blazorTetris-management-record-1", new RecordArgs
        {
            Name = "tetris",
            Ttl = 300,
            Type = "CNAME",
            Records = [ Distribution.DomainName ],
            ZoneId = args.ManagementZoneFavianFloresNetId
        }, new CustomResourceOptions { Provider = args.ManagementProvider });
    }
}
