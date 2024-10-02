﻿using Amazon.CDK;
using BlazorTetris.Infrastructure;

// ReSharper disable UnusedVariable

App app = new();
IBlazorTetrisConfig config = new BlazorTetrisProductionConfig();



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

BlazorTetrisDnsStack blazorTetrisDnsStack = new(app, "BlazorTetrisDns", blazorTetrisDnsStackProps, config);

app.Synth();
