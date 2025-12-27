using System.ComponentModel.DataAnnotations;

namespace SlutProv.Api.Models
{
    /// <summary>
    /// Request för att skapa bildmetadata
    /// </summary>
    public class CreateImageRequest
    {
        /// <summary>
        /// Namnet på bildfilen (t.ex. "myimage.jpg")
        /// </summary>
        [Required(ErrorMessage = "Filnamn är obligatoriskt")]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// URL till bilden i Blob Storage
        /// </summary>
        [Required(ErrorMessage = "Blob URL är obligatoriskt")]
        [Url(ErrorMessage = "Måste vara en giltig URL")]
        public string BlobUrl { get; set; } = string.Empty;
    }
}