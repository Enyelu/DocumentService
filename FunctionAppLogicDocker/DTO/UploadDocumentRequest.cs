namespace FunctionAppLogicDocker.DTO
{
    public class UploadDocumentRequest
    {
        public string FileName { get; set; } = default!;
        public string Base64Content { get; set; } = default!;
        public string? ContentType { get; set; }
    }
}
