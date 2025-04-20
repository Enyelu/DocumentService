using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using FunctionAppLogic.DTO;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FunctionAppLogic;

public class Functions
{
    /// <summary>
    /// Default constructor that Lambda will invoke.
    /// </summary>

    private readonly IAmazonS3 _s3Client;
    public Functions()
    {
        _s3Client = new AmazonS3Client();
    }


    /// <summary>
    /// A Lambda function to respond to HTTP Get methods from API Gateway
    /// </summary>
    /// <remarks>
    /// This uses the <see href="https://github.com/aws/aws-lambda-dotnet/blob/master/Libraries/src/Amazon.Lambda.Annotations/README.md">Lambda Annotations</see> 
    /// programming model to bridge the gap between the Lambda programming model and a more idiomatic .NET model.
    /// 
    /// This automatically handles reading parameters from an APIGatewayProxyRequest
    /// as well as syncing the function definitions to serverless.template each time you build.
    /// 
    /// If you do not wish to use this model and need to manipulate the API Gateway 
    /// objects directly, see the accompanying Readme.md for instructions.
    /// </remarks>
    /// <param name="context">Information about the invocation, function, and execution environment</param>
    /// <returns>The response as an implicit <see cref="APIGatewayProxyResponse"/></returns>
    [LambdaFunction]
    [RestApi(LambdaHttpMethod.Post, "/")]
    public async Task<APIGatewayProxyResponse> UploadDocument(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation("Handling the 'Get' Request");

        var uploadRequest = JsonConvert.DeserializeObject<UploadDocumentRequest>(request.Body);

        if (uploadRequest == null || string.IsNullOrEmpty(uploadRequest.FileName) || string.IsNullOrEmpty(uploadRequest.Base64Content))
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 400,
                Body = "Invalid input: file name or content missing"
            };
        }

        var fileBytes = Convert.FromBase64String(uploadRequest.Base64Content);

        var putRequest = new PutObjectRequest
        {
            BucketName = "mycdkappstack-documentstorage21aa9d86-gxlwfvvjykkp",
            Key = uploadRequest.FileName,
            InputStream = new MemoryStream(fileBytes),
            ContentType = uploadRequest.ContentType ?? "application/octet-stream"
        };

        await _s3Client.PutObjectAsync(putRequest);

        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = "Document Uploaded"
        };
    }
}
