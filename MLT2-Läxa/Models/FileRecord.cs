using System;
using System.Collections.Generic;
using System.Text;

namespace MLT2_Läxa.Models
{
    public class FileRecord
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string? ContentType { get; set; }
        public string BlobUrl { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }

        public string GetFormattedSize()
        {
            if (FileSize < 1024)
                return $"{FileSize} B";
            else if (FileSize < 1024 * 1024)
                return $"{FileSize / 1024.0:F1} KB";
            else
                return $"{FileSize / (1024.0 * 1024.0):F1} MB";
        }
    }
}
