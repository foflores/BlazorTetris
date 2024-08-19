using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.S3;
using Constructs;

// ReSharper disable UnusedVariable

namespace BlazorTetris.Infrastructure;

public class BlazorTetrisStack : Stack
{
    public BlazorTetrisStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
    {
        // var policyDocument = new PolicyDocument(new PolicyDocumentProps
        // {
        //     Statements = [
        //         new PolicyStatement(new PolicyStatementProps
        //         {
        //             Sid = "AllowCloudFrontServicePrincipal",
        //             Effect = Effect.ALLOW,
        //             Principals = [
        //                 new ServicePrincipal("AllowCloudFrontServicePrincipal")
        //             ],
        //             Actions = ["s3:GetObject"]
        //         })
        //     ]
        // });

        var bucket = new Bucket(this, "SourceFiles", new BucketProps
        {
            RemovalPolicy = RemovalPolicy.DESTROY,
            AutoDeleteObjects = false,
        });
    }
}

