using Azure.Storage.Blobs;

internal class Program
{
    private static async Task Main(string[] args)
    {

        var accountUri = "https://gustavsblobbar.blob.core.windows.net";
        var container = "containerfynd";
        var blobName = "bob (2).docx";

        var sas = "sp=r&st=2025-12-04T14:07:34Z&se=2025-12-04T22:22:34Z&spr=https&sv=2024-11-04&sr=b&sig=kinNnRh1wU5dlRqiZrUtbFaicg0KHff%2BVBMZhs2b%2Fa0%3D";

        var url = $"{accountUri}/{container}/{Uri.EscapeDataString(blobName)}?{sas}";

        var blob = new BlobClient(new Uri(url));

        var content = await blob.DownloadContentAsync();
        Console.WriteLine(content.Value.Content.ToString());
    }
}