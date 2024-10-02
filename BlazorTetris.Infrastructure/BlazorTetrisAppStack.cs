using Amazon.CDK;
using Amazon.CDK.AWS.CloudFront;
using Amazon.CDK.AWS.CloudFront.Origins;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.S3;
using Constructs;

// ReSharper disable UnusedVariable

namespace BlazorTetris.Infrastructure;

public class BlazorTetrisAppStack : Stack
{
    public BlazorTetrisAppStack(Construct scope, string id, IStackProps props, IBlazorTetrisConfig config)
        : base(scope, id, props)
    {
        BucketProps bucketProps = new()
        {
            RemovalPolicy = RemovalPolicy.DESTROY
        };
        Bucket bucket = new(this, "Bucket", bucketProps);

        var origin = S3BucketOrigin.WithOriginAccessControl(bucket);

        BehaviorOptions behaviorOptions = new()
        {
            Origin = origin,
        };

        DistributionProps distributionProps = new()
        {
            DefaultBehavior = behaviorOptions,
            PriceClass = PriceClass.PRICE_CLASS_100,
        };

        Distribution distribution = new (this, "Distribution", distributionProps);
    }
}
