using Azure.Storage.Blobs;

internal class Program
{
    private static async Task Main(string[] args)
    {

        var accountUri = "https://gustavsblobbar.blob.core.windows.net";
        var container = "bob";
        var blobName = "aaa.docx";

        var sas = "sp=r&st=2026-01-08T08:18:04Z&se=2026-01-08T16:33:04Z&spr=https&sv=2024-11-04&sr=b&sig=qnXSJD9V3t1tEz%2F1oItbfx5AlY1kurxUy0pXzxOP%2Bj0%3D";

        var url = $"{accountUri}/{container}/{Uri.EscapeDataString(blobName)}?{sas}";

        var blob = new BlobClient(new Uri(url));

        var content = await blob.DownloadContentAsync();
        Console.WriteLine(content.Value.Content.ToString());
    }
}