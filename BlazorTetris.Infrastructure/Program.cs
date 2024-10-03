using Amazon.CDK;
using BlazorTetris.Infrastructure;

// ReSharper disable UnusedVariable

var currentAccountId = System.Environment.GetEnvironmentVariable("AWS_DEFAULT_ACCOUNT");
var currentRegionId = System.Environment.GetEnvironmentVariable("AWS_DEFAULT_REGION");

BlazorTetrisConfigResolver resolver = new(currentAccountId, currentRegionId);
IBlazorTetrisConfig? config = resolver.GetConfig();

if (config == null)
    throw new System.ApplicationException("Could not load configuration.");

App app = new();

// Environment appEnvironment = new()
// {
//     Account = config.AwsAppAccountId,
//     Region = config.AwsAppRegionId
// };
//
// StackProps blazorTetrisAppStackProps = new()
// {
//     Description = "Contains infrastructure for blazor tetris app.",
//     Env = appEnvironment
// };
//
// BlazorTetrisAppStack blazorTetrisAppStack = new(app, "BlazorTetrisApp", blazorTetrisAppStackProps, config);

Environment dnsEnvironment = new()
{
    Account = config.AwsDnsAccountId,
    Region = config.AwsDnsRegionId
};

DefaultStackSynthesizerProps dnsStackSynthesizerProps = new()
{
    Qualifier = config.DnsQualifierId
};
DefaultStackSynthesizer synthesizer = new(dnsStackSynthesizerProps);

StackProps blazorTetrisDnsStackProps = new()
{
    Description = "Contains dns settings for blazor tetris app.",
    Env = dnsEnvironment,
    Synthesizer = synthesizer
};

BlazorTetrisDnsStack blazorTetrisDnsStack = new(app, config.DnsStackName, blazorTetrisDnsStackProps, config);

app.Synth();
