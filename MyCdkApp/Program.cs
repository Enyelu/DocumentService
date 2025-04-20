using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyCdkApp
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();

            var environment = (string)app.Node.TryGetContext("env") ?? "dev";
            var account = (string)app.Node.TryGetContext($"{environment.ToUpper()}_AWS_ACCOUNT_ID") ?? "";
            var region = (string)app.Node.TryGetContext($"{environment.ToUpper()}_AWS_REGION") ?? "";

            new MyCdkAppStack(app, $"MyCdkAppStack-{environment}", new StackProps
            {
                // If you don't specify 'env', this stack will be environment-agnostic.
                // Account/Region-dependent features and context lookups will not work,
                // but a single synthesized template can be deployed anywhere.

                // Uncomment the next block to specialize this stack for the AWS Account
                // and Region that are implied by the current CLI configuration.

                Env = new Amazon.CDK.Environment
                {
                    Account = System.Environment.GetEnvironmentVariable(account),
                    Region = System.Environment.GetEnvironmentVariable(region),
                }


                // Uncomment the next block if you know exactly what Account and Region you
                // want to deploy the stack to.
                /*
                Env = new Amazon.CDK.Environment
                {
                    Account = "123456789012",
                    Region = "us-east-1",
                }
                */

                // For more information, see https://docs.aws.amazon.com/cdk/latest/guide/environments.html
            });
            app.Synth();
        }
    }
}
