FROM public.ecr.aws/lambda/dotnet:8

WORKDIR /var/task

# Copy published app contents
COPY "bin/Release/net8.0/publish" .

# Tell Lambda which function to invoke
CMD ["FunctionAppLogicDocker::FunctionAppLogicDocker.Functions::UploadDocument"]