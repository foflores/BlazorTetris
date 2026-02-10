using Pulumi;
using Pulumi.Aws;
using Pulumi.Aws.CloudFront;
using Pulumi.Aws.Iam;
using Pulumi.Aws.Iam.Inputs;
using Pulumi.Aws.S3;

namespace BlazorTetris.Infrastructure.Components;

public class BucketsArgs
{
    public required Provider EnvProvider { get; init; }
}

public class Buckets
{
    private readonly BucketsArgs _args;
    private readonly string _prefix;

    public Bucket SourceBucket { get; }
    public BucketPolicy? SourceBucketPolicy { get; private set; }

    public Buckets(string prefix, BucketsArgs args)
    {
        _args = args;
        _prefix = prefix;

        SourceBucket = new Bucket($"{prefix}-bucket-source", new BucketArgs
        {
            ForceDestroy = true
        }, new CustomResourceOptions { Provider = args.EnvProvider });
    }

    public void CreateSourceBucketPolicy(Distribution distribution)
    {
        SourceBucketPolicy = new BucketPolicy($"{_prefix}-bucketPolicy-source", new BucketPolicyArgs
        {
            Bucket = SourceBucket.BucketName,
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
                        Resources = [ SourceBucket.Arn.Apply(x => $"{x}/*") ],
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
            }, new InvokeOptions { Provider = _args.EnvProvider }).Apply(x => x.Json)
        }, new CustomResourceOptions { Provider = _args.EnvProvider });
    }
}
