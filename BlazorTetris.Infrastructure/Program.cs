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
    var managementRoleAdministratorArn = config.Require("management-roleAdministrator-arn");
    var productionRoleAdministratorArn = config.Require("production-roleAdministrator-arn");
    var developmentRoleAdministratorArn = config.Require("development-roleAdministrator-arn");
    var managementZoneFavianFloresComId = config.Require("management-zoneFavianFloresCom-id");
    var managementZoneFavianFloresNetId = config.Require("management-zoneFavianFloresNet-id");

    // Providers
    var managementProvider = new Provider("blazorTetris-management-provider", new ProviderArgs
    {
        AllowedAccountIds = [ managementId ],
        AssumeRole = new ProviderAssumeRoleArgs
        {
            RoleArn = managementRoleAdministratorArn,
            SessionName = "pulumi-blazorTetris-deploy"
        }
    });

    var productionProvider = new Provider("blazorTetris-production-provider", new ProviderArgs
    {
        AllowedAccountIds = [ productionId ],
        AssumeRole = new ProviderAssumeRoleArgs
        {
            RoleArn = productionRoleAdministratorArn,
            SessionName = "pulumi-blazorTetris-deploy"
        },
    });

    var developmentProvider = new Provider("blazorTetris-development-provider", new ProviderArgs
    {
        AllowedAccountIds = [ developmentId ],
        AssumeRole = new ProviderAssumeRoleArgs
        {
            RoleArn = developmentRoleAdministratorArn,
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
});
