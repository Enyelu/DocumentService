FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app

FROM public.ecr.aws/lambda/dotnet:8
WORKDIR /var/task
COPY --from=build /app .

# Set the Lambda handler name

# Tell Lambda which function to invoke
CMD ["FunctionAppLogicDocker::FunctionAppLogicDocker.Functions::UploadDocument"]