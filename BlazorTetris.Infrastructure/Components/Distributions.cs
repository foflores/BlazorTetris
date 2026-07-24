using Pulumi;
using Pulumi.Aws;
using Pulumi.Aws.CloudFront;
using Pulumi.Aws.CloudFront.Inputs;
using Pulumi.Aws.S3;

namespace BlazorTetris.Infrastructure.Components;

public class DistributionsArgs
{
    public required Provider EnvProvider { get; init; }
    public required Bucket SourceBucket { get; init; }
    public required Input<string> Domain { get; init; }
}

public class Distributions
{
    public Distribution Distribution { get; }
    public OriginAccessControl OriginAccessControl { get; }

    public Distributions(string prefix, DistributionsArgs args)
    {
        OriginAccessControl = new OriginAccessControl($"{prefix}-originaccesscontrol-main", new OriginAccessControlArgs
        {
            OriginAccessControlOriginType = "s3",
            SigningBehavior = "always",
            SigningProtocol = "sigv4"
        }, new CustomResourceOptions { Provider = args.EnvProvider });

        var originId = $"{prefix}-origin-main";

        Distribution = new Distribution($"{prefix}-distribution-main", new DistributionArgs
        {
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
                    OriginAccessControlId = OriginAccessControl.Id,
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
                CloudfrontDefaultCertificate = true
            },
            WaitForDeployment = false,
        }, new CustomResourceOptions { Provider = args.EnvProvider });
    }
}
