using System.Collections.Generic;
using BlazorTetris.Infrastructure.Environments;
using Pulumi;
using Pulumi.Aws;
using Pulumi.Aws.Inputs;

// ReSharper disable UnusedVariable

return await Deployment.RunAsync(() =>
{
    // Config
    var config = new Pulumi.Config();
    var managementId = config.Require("management-id");
    var productionId = config.Require("production-id");
    var developmentId = config.Require("development-id");
    var managementRoleIacArn = config.Require("management-roleIac-arn");
    var productionRoleIacArn = config.Require("production-roleIac-arn");
    var developmentRoleIacArn = config.Require("development-roleIac-arn");
    var managementZoneFavianFloresComId = config.Require("management-zoneFavianFloresCom-id");
    var managementZoneFavianFloresNetId = config.Require("management-zoneFavianFloresNet-id");

    // Providers
    var managementProvider = new Provider("blazorTetris-management-provider", new ProviderArgs
    {
        AllowedAccountIds = [ managementId ],
        AssumeRole = new ProviderAssumeRoleArgs
        {
            RoleArn = managementRoleIacArn,
            SessionName = "pulumi-blazorTetris-deploy"
        }
    });

    var productionProvider = new Provider("blazorTetris-production-provider", new ProviderArgs
    {
        AllowedAccountIds = [ productionId ],
        AssumeRole = new ProviderAssumeRoleArgs
        {
            RoleArn = productionRoleIacArn,
            SessionName = "pulumi-blazorTetris-deploy"
        },
    });

    var developmentProvider = new Provider("blazorTetris-development-provider", new ProviderArgs
    {
        AllowedAccountIds = [ developmentId ],
        AssumeRole = new ProviderAssumeRoleArgs
        {
            RoleArn = developmentRoleIacArn,
            SessionName = "pulumi-blazorTetris-deploy"
        },
    });

    DevelopmentEnvironment developmentEnvironment = new(new DevelopmentEnvironmentArgs
    {
        ManagementProvider = managementProvider,
        DevelopmentProvider = developmentProvider,
        ManagementZoneFavianFloresNetId = managementZoneFavianFloresNetId
    });

    ProductionEnvironment productionEnvironment = new(new ProductionEnvironmentArgs
    {
        ManagementProvider = managementProvider,
        ProductionProvider = productionProvider,
        ManagementZoneFavianFloresComId = managementZoneFavianFloresComId
    });

    return new Dictionary<string, object?>
    {
        ["production-bucketApp-name"] = productionEnvironment.AppBucket.BucketName,
        ["development-bucketApp-name"] = developmentEnvironment.AppBucket.BucketName,
        ["production-distribution-id"] = productionEnvironment.Distribution.Id,
        ["development-distribution-id"] = developmentEnvironment.Distribution.Id
    };
});
