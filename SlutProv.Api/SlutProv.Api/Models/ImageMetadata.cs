namespace SlutProv.Api.Models
{

    public class ImageMetadata
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string BlobUrl { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }
}