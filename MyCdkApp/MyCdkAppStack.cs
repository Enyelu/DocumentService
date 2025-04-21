using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.ECS.Patterns;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.S3;
using Constructs;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.Events.Targets;
using Amazon.CDK.AWS.APIGateway;

namespace MyCdkApp
{
    public class MyCdkAppStack : Stack
    {
        internal MyCdkAppStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            var environment = Node.TryGetContext("env") ?? "dev";
            var documentUploadFunction = new Function(this, $"DocumentUploadFunction-{environment}", new FunctionProps
            {
                Runtime = Runtime.DOTNET_8,
                Handler = "FunctionAppLogic::FunctionAppLogic.Functions::UploadDocument",
                Code = Code.FromAsset("../FunctionAppLogic/bin/Release/net8.0/publish")
            });

            new LambdaRestApi(this, $"documentUploadFunctionApiEndpoint-{environment}", new LambdaRestApiProps
            {
                Handler = documentUploadFunction
            });

            var documentUploadFunctionDocker = new Function(this, $"DocumentUploadFunctionDocker-{environment}", new FunctionProps
            {
                Runtime = Runtime.FROM_IMAGE,
                Handler = "FROM_IMAGE",
                Code = Code.FromAssetImage("src/FunctionAppLogicDocker", new AssetImageCodeProps
                {
                    Cmd = new string[] { "FunctionAppLogicDocker::FunctionAppLogicDocker.Functions::UploadDocument" }
                })
            });

            new LambdaRestApi(this, $"documentUploadFunctionDockerApiEndpoint-{environment}", new LambdaRestApiProps
            {
                Handler = documentUploadFunctionDocker
            });

            var documentStorage = new Bucket(this, $"DocumentStorage-{environment}", new BucketProps
            {
                Versioned = true,
                PublicReadAccess = true,
                BlockPublicAccess = new BlockPublicAccess(new BlockPublicAccessOptions
                {
                    BlockPublicAcls = false,
                    IgnorePublicAcls = false,
                    BlockPublicPolicy = false,
                    RestrictPublicBuckets = false,
                })
            });

            documentStorage.GrantWrite(documentUploadFunction);
            documentStorage.GrantWrite(documentUploadFunctionDocker);

            var documentTable = new Table(this, $"DocumentTable-{environment}", new TableProps 
            { 
                PartitionKey = new Attribute { Name = "Id", Type = AttributeType.STRING },
                SortKey = new Attribute { Name = "DocumentName", Type = AttributeType.STRING },
                BillingMode = BillingMode.PAY_PER_REQUEST
            });

            documentTable.GrantWriteData(documentUploadFunction);
        }
    }
}