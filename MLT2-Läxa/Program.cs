using Microsoft.Extensions.Configuration;
using MLT2_Läxa.Services;

namespace MLT2_Läxa
{
    internal class Program
    {
        static void Main(string[] args)
        {
            EstablisConnection();
        }

        private static async void EstablisConnection()
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
            Console.WriteLine(containerName);
        }
    }
}
