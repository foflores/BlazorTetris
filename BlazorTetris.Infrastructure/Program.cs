using Amazon.CDK;
using BlazorTetris.Infrastructure;

// ReSharper disable UnusedVariable

var app = new App();

var stagingEnvironment = new Environment
{
    Account = "412433735452",
    Region = "us-east-1"
};

var blazorTetrisStack = new BlazorTetrisStack(app, "BlazorTetris", new StackProps
{
    Env = stagingEnvironment
});

app.Synth();
