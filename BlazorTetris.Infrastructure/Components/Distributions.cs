using Pulumi;
using Pulumi.Aws;
using Pulumi.Aws.Acm;
using Pulumi.Aws.CloudFront;
using Pulumi.Aws.CloudFront.Inputs;
using Pulumi.Aws.S3;

namespace BlazorTetris.Infrastructure.Components;

public class DistributionsArgs
{
    public required Provider EnvProvider { get; init; }
    public required Bucket SourceBucket { get; init; }
    public required Certificate MainCertificate { get; init; }
    public required CertificateValidation MainCertificateValidation { get; init; }
    public required Input<string> MainDistributionDomain { get; init; }
}

public class Distributions
{
    public Distribution MainDistribution { get; }
    public OriginAccessControl MainOriginAccessControl { get; }

    public Distributions(string prefix, DistributionsArgs args)
    {
        MainOriginAccessControl = new OriginAccessControl($"{prefix}-originaccesscontrol-main", new OriginAccessControlArgs
        {
            OriginAccessControlOriginType = "s3",
            SigningBehavior = "always",
            SigningProtocol = "sigv4"
        }, new CustomResourceOptions { Provider = args.EnvProvider });

        var originId = $"{prefix}-origin-main";

        MainDistribution = new Distribution($"{prefix}-distribution-main", new DistributionArgs
        {
            Aliases = [ args.MainDistributionDomain ],
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
                TargetOriginId = originId,
                ViewerProtocolPolicy = "redirect-to-https"
            },
            Enabled = true,
            HttpVersion = "http2and3",
            Origins = new[]
            {
                new DistributionOriginArgs
                {
                    DomainName = args.SourceBucket.BucketRegionalDomainName,
                    OriginAccessControlId = MainOriginAccessControl.Id,
                    OriginId = originId,
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
                AcmCertificateArn = args.MainCertificate.Arn,
                SslSupportMethod = "sni-only",
                MinimumProtocolVersion = "TLSv1.2_2021"
            },
            WaitForDeployment = false,
        }, new CustomResourceOptions { Provider = args.EnvProvider, DependsOn = args.MainCertificateValidation });
    }
}
