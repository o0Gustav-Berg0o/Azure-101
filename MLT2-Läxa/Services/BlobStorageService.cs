using System;
using System.Collections.Generic;
using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace MLT2_Läxa.Services
{
    internal class BlobStorageService
    {
        private readonly BlobContainerClient _containerClient;

        public BlobStorageService(string connectionString, string containerName)
        {
            var blobServiceClient = new BlobServiceClient(connectionString);
            _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        }

        /// <summary>
        /// Laddar upp en fil till Blob Storage
        /// </summary>
        /// <param name="localFilePath">Sökväg till lokal fil</param>
        /// <param name="blobName">Namn på bloben (kan vara samma som filnamn)</param>
        /// <returns>URL till den uppladdade bloben</returns>
        public async Task<string> UploadFileAsync(string localFilePath, string blobName)
        {
            var blobClient = _containerClient.GetBlobClient(blobName);

            // Bestäm content type baserat på filändelse
            string contentType = GetContentType(blobName);

            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                }
            };

            // Ladda upp filen
            await using var stream = File.OpenRead(localFilePath);
            await blobClient.UploadAsync(stream, options);

            // Returnera URL till bloben
            return blobClient.Uri.ToString();
        }

        /// <summary>
        /// Laddar ner en fil från Blob Storage
        /// </summary>
        /// <param name="blobName">Namn på bloben</param>
        /// <param name="downloadPath">Sökväg där filen ska sparas</param>
        public async Task DownloadFileAsync(string blobName, string downloadPath)
        {
            var blobClient = _containerClient.GetBlobClient(blobName);

            // Ladda ner till lokal fil
            await blobClient.DownloadToAsync(downloadPath);
        }

        /// <summary>
        /// Tar bort en fil från Blob Storage
        /// </summary>
        /// <param name="blobName">Namn på bloben</param>
        public async Task DeleteFileAsync(string blobName)
        {
            var blobClient = _containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();
        }

        /// <summary>
        /// Kontrollerar om en blob existerar
        /// </summary>
        public async Task<bool> ExistsAsync(string blobName)
        {
            var blobClient = _containerClient.GetBlobClient(blobName);
            return await blobClient.ExistsAsync();
        }

        /// <summary>
        /// Hämtar content type baserat på filändelse
        /// </summary>
        private string GetContentType(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();

            return extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".txt" => "text/plain",
                ".html" => "text/html",
                ".css" => "text/css",
                ".js" => "application/javascript",
                ".json" => "application/json",
                ".xml" => "application/xml",
                ".zip" => "application/zip",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream"
            };
        }
    }