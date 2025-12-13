using Microsoft.Extensions.Configuration;
using System.IO; 

namespace MLT2_Läxa
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // This requires System.IO
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
        }
    }
}
