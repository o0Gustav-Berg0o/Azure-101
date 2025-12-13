using Microsoft.Extensions.Configuration;
using MLT2_Läxa.Services;

namespace MLT2_Läxa
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await EstablisConnection();
        }

        private static async Task EstablisConnection()
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory()) // This requires System.IO
               .AddJsonFile("appsettings.json", optional: false)
               .Build();

            string blobConnectionString = configuration.GetConnectionString("BlobStorage")
               ?? throw new InvalidOperationException("BlobStorage connection string saknas");
            string sqlConnectionString = configuration.GetConnectionString("SqlDatabase")
                ?? throw new InvalidOperationException("SqlDatabase connection string saknas");
            string containerName = configuration["BlobContainerName"] ?? "uploads";


            var blobService = new BlobStorageService(blobConnectionString, containerName);
            var dbService = new DatabaseService(sqlConnectionString);


            Console.WriteLine("Testar anslutningar...");

            if (!await dbService.TestConnectionAsync())
            {
                Console.WriteLine(" Kunde inte ansluta till databasen. Kontrollera connection string.");
                return;
            }
            Console.WriteLine(" Ansluten till Azure SQL Database");
            Console.WriteLine(" Ansluten till Azure Blob Storage");
            Console.WriteLine();

            // Huvudloop
            bool running = true;
            while (running)
            {
                ShowMenu();
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await UploadFile();
                        break;
                    case "2":
                        await ListFiles();
                        break;
                    case "3":
                        await DownloadFile();
                        break;
                    case "4":
                        await DeleteFile();
                        break;
                    case "5":
                        running = false;
                        Console.WriteLine("Hej då!");
                        break;
                    default:
                        Console.WriteLine("Ogiltigt val, försök igen.");
                        break;
                }

                Console.WriteLine();
            }
            void ShowMenu()
            {
                Console.WriteLine("╔═══════════════════════════════╗");
                Console.WriteLine("║        FILARKIV               ║");
                Console.WriteLine("╠═══════════════════════════════╣");
                Console.WriteLine("║  1. Ladda upp fil             ║");
                Console.WriteLine("║  2. Lista alla filer          ║");
                Console.WriteLine("║  3. Ladda ner fil             ║");
                Console.WriteLine("║  4. Ta bort fil               ║");
                Console.WriteLine("║  5. Avsluta                   ║");
                Console.WriteLine("╚═══════════════════════════════╝");
                Console.Write("Välj alternativ: ");
            }
            async Task UploadFile()
            {
                Console.Write("Ange sökväg till filen: ");
                string? filePath = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(filePath))
                {
                    Console.WriteLine("Ogiltig sökväg.");
                    return;
                }

                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Filen '{filePath}' hittades inte.");
                    return;
                }

                try
                {
                    // Hämta filinfo
                    var fileInfo = new FileInfo(filePath);
                    string fileName = fileInfo.Name;
                    long fileSize = fileInfo.Length;

                    // Skapa unikt blob-namn (undvik överskrivning)
                    string blobName = $"{DateTime.UtcNow:yyyyMMdd-HHmmss}_{fileName}";

                    Console.WriteLine($"Laddar upp '{fileName}' ({FormatFileSize(fileSize)})...");

                    // Ladda upp till Blob Storage
                    string blobUrl = await blobService.UploadFileAsync(filePath, blobName);

                    // Bestäm content type
                    string contentType = GetContentType(fileName);

                    // Spara metadata i SQL
                    int newId = await dbService.SaveFileRecordAsync(fileName, contentType, blobUrl, fileSize);

                    Console.WriteLine($"   Fil uppladdad!");
                    Console.WriteLine($"   Id: {newId}");
                    Console.WriteLine($"   Blob URL: {blobUrl}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" Fel vid uppladdning: {ex.Message}");
                }
            }

            async Task ListFiles()
            {
                try
                {
                    var files = await dbService.GetAllFilesAsync();

                    if (files.Count == 0)
                    {
                        Console.WriteLine("Inga filer finns i arkivet.");
                        return;
                    }

                    Console.WriteLine();
                    Console.WriteLine("┌──────┬────────────────────────────┬────────────┬─────────────────────┐");
                    Console.WriteLine("│ Id   │ Filnamn                    │ Storlek    │ Uppladdad           │");
                    Console.WriteLine("├──────┼────────────────────────────┼────────────┼─────────────────────┤");

                    foreach (var file in files)
                    {
                        string name = file.FileName.Length > 26
                            ? file.FileName.Substring(0, 23) + "..."
                            : file.FileName;

                        Console.WriteLine($"│ {file.Id,-4} │ {name,-26} │ {file.GetFormattedSize(),-10} │ {file.UploadedAt:yyyy-MM-dd HH:mm} │");
                    }

                    Console.WriteLine("└──────┴────────────────────────────┴────────────┴─────────────────────┘");
                    Console.WriteLine($"Totalt: {files.Count} fil(er)");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" Fel vid hämtning: {ex.Message}");
                }
            }

            async Task DownloadFile()
            {
                Console.Write("Ange fil-Id: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Ogiltigt Id.");
                    return;
                }

                try
                {
                    var file = await dbService.GetFileByIdAsync(id);

                    if (file == null)
                    {
                        Console.WriteLine($"Ingen fil med Id {id} hittades.");
                        return;
                    }

                    Console.Write($"Var ska filen sparas? (default: aktuell mapp) ");
                    string? downloadDir = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(downloadDir))
                    {
                        downloadDir = Directory.GetCurrentDirectory();
                    }

                    string downloadPath = Path.Combine(downloadDir, file.FileName);

                    // Extrahera blob-namn från URL
                    string blobName = ExtractBlobNameFromUrl(file.BlobUrl);

                    Console.WriteLine($"Laddar ner '{file.FileName}'...");
                    await blobService.DownloadFileAsync(blobName, downloadPath);

                    Console.WriteLine($" Fil nedladdad till: {downloadPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" Fel vid nedladdning: {ex.Message}");
                }
            }

            async Task DeleteFile()
            {
                Console.Write("Ange fil-Id: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Ogiltigt Id.");
                    return;
                }

                try
                {
                    var file = await dbService.GetFileByIdAsync(id);

                    if (file == null)
                    {
                        Console.WriteLine($"Ingen fil med Id {id} hittades.");
                        return;
                    }

                    Console.Write($"Är du säker på att du vill ta bort '{file.FileName}'? (j/n): ");
                    string? confirm = Console.ReadLine();

                    if (confirm?.ToLower() != "j")
                    {
                        Console.WriteLine("Borttagning avbruten.");
                        return;
                    }

                    // Extrahera blob-namn från URL
                    string blobName = ExtractBlobNameFromUrl(file.BlobUrl);

                    Console.WriteLine("Tar bort fil...");

                    // Ta bort från Blob Storage
                    await blobService.DeleteFileAsync(blobName);

                    // Ta bort från databasen
                    await dbService.DeleteFileRecordAsync(id);

                    Console.WriteLine($" '{file.FileName}' har tagits bort.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" Fel vid borttagning: {ex.Message}");
                }
            }

            // =====================
            // HJÄLPFUNKTIONER
            // =====================

            string GetContentType(string fileName)
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
                    ".json" => "application/json",
                    ".xml" => "application/xml",
                    ".zip" => "application/zip",
                    ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    _ => "application/octet-stream"
                };
            }

            string FormatFileSize(long bytes)
            {
                if (bytes < 1024)
                    return $"{bytes} B";
                else if (bytes < 1024 * 1024)
                    return $"{bytes / 1024.0:F1} KB";
                else
                    return $"{bytes / (1024.0 * 1024.0):F1} MB";
            }

            string ExtractBlobNameFromUrl(string blobUrl)
            {
                // URL format: https://storageaccount.blob.core.windows.net/container/blobname
                var uri = new Uri(blobUrl);
                // Segments: /, container/, blobname
                return uri.Segments[^1]; // Sista segmentet är blob-namnet
            }
        }
    }
}