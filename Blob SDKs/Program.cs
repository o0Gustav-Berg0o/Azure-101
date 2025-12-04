using Azure.Storage.Blobs;

internal class Program
{
    static async Task Main(string[] args)
    {
        var accountUri = "https://gustavsblobbar.blob.core.windows.net";
        var sas = "sv=2024-11-04&ss=bfqt&srt=sco&sp=rwdlacupiytfx&se=2025-12-04T22:48:43Z&st=2025-12-04T14:33:43Z&spr=https&sig=Pa5t8n7O8OtYFix5rcq%2Fvjsdbd6tUdw1fe%2Fi8qezVFU%3D";

        var containerName = "mincontainer";
        var blobName = "testfil.txt";
        var contentText = "Detta är en testfil.";

        var containerClient = new BlobContainerClient(
            new Uri($"{accountUri}/{containerName}?{sas}")
        );

        await containerClient.CreateIfNotExistsAsync();

        var blobClient = containerClient.GetBlobClient(blobName);

        using var uploadStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(contentText));
        await blobClient.UploadAsync(uploadStream, overwrite: true);

        var downloaded = await blobClient.DownloadContentAsync();
        Console.WriteLine(downloaded.Value.Content.ToString());

        Console.WriteLine("Filer i containern:");
        await foreach (var blobItem in containerClient.GetBlobsAsync())
        {
            Console.WriteLine(blobItem.Name);
        }

        // Ta bort filen
        Console.WriteLine("Tar bort testfilen...");
        await blobClient.DeleteIfExistsAsync();

        // Ta bort containern
        Console.WriteLine("Tar bort containern...");
        await containerClient.DeleteIfExistsAsync();
    }
}
