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

public class ProductionEnvironmentArgs
{
    public required ProviderResource ManagementProvider { get; init; }
    public required ProviderResource ProductionProvider { get; init; }
    public required string ManagementZoneFavianFloresComId { get; init; }
}

public class ProductionEnvironment
{
    public ProductionEnvironment(ProductionEnvironmentArgs args)
    {
          const string appDomain = "tetris.favianflores.com";

        var developmentCertificate1 = new Certificate("blazorTetris-production-certificate-1", new CertificateArgs
        {
            DomainName = appDomain,
            ValidationMethod = "DNS"
        }, new CustomResourceOptions { Provider = args.ProductionProvider });

        var validationRecordFqdns = developmentCertificate1.DomainValidationOptions.Apply(x =>
        {
            if (x.Length < 1)
                return null;

            List<ValidationRecord> records =
            [
                new("blazorTetris-management-validationRecord-2", new ValidationRecordArgs
                {
                    ValidationOption = x[0],
                    ZoneId = args.ManagementZoneFavianFloresComId
                }, new CustomResourceOptions { Provider = args.ManagementProvider })
            ];

            return Output.All(records.Select(y => y.Record.Fqdn));
        });

        var certificateValidation = new CertificateValidation(
            "blazorTetris-production-certificateValidation-1",
            new CertificateValidationArgs
            {
                CertificateArn = developmentCertificate1.Arn,
                ValidationRecordFqdns = validationRecordFqdns
            }, new CustomResourceOptions { Provider = args.ProductionProvider });

        var bucket = new Bucket("blazorTetris-production-bucket-1", new BucketArgs
        {
            BucketPrefix = "blazor-tetris-bucket",
            ForceDestroy = true
        }, new CustomResourceOptions { Provider = args.ProductionProvider });

        var originAccessControl = new OriginAccessControl("blazorTetris-production-originAccessControl-1", new OriginAccessControlArgs
        {
            Name = "blazor-tetris-origin-access-control",
            OriginAccessControlOriginType = "s3",
            SigningBehavior = "always",
            SigningProtocol = "sigv4"
        }, new CustomResourceOptions { Provider = args.ProductionProvider });

        var distribution = new Distribution("blazorTetris-production-distribution-1", new DistributionArgs
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
                    DomainName = bucket.BucketRegionalDomainName,
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
        }, new CustomResourceOptions { Provider = args.ProductionProvider });

        var bucketPolicy = new BucketPolicy("blazorTetris-production-bucketPolicy-1", new BucketPolicyArgs
        {
            Bucket = bucket.BucketName,
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
                        Resources = [ bucket.Arn.Apply(x => $"{x}/*") ],
                        Conditions =
                        [
                            new GetPolicyDocumentStatementConditionInputArgs
                            {
                                Test = "StringEquals",
                                Values = distribution.Arn,
                                Variable = "AWS:SourceArn"
                            }
                        ],
                    }
                ]
            }, new InvokeOptions { Provider = args.ProductionProvider }).Apply(x => x.Json)
        }, new CustomResourceOptions { Provider = args.ProductionProvider });

        var dnsTetrisRecord = new Record("blazorTetris-management-record-2", new RecordArgs
        {
            Name = "tetris",
            Ttl = 300,
            Type = "CNAME",
            Records = [ distribution.DomainName ],
            ZoneId = args.ManagementZoneFavianFloresComId
        }, new CustomResourceOptions { Provider = args.ManagementProvider });
    }
}
