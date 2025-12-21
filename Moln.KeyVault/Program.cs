using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace Moln.KeyVault
{
    public class Program
    {
        public static SecretClient _client;
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();



            //Key vault credential code
            var keyVaultURL = builder.Configuration.GetSection("KeyVault:KeyVaultURL");
            var keyVaultClientId = builder.Configuration.GetSection("KeyVault:ClientId");
            var keyVaultClientSecret = builder.Configuration.GetSection("KeyVault:ClientSecret");
            var keyVaultDirectoryID = builder.Configuration.GetSection("KeyVault:DirectoryID");
            var secretName = builder.Configuration.GetSection("KeyVault:SecretName");

            var credential = new ClientSecretCredential(keyVaultDirectoryID.Value!.ToString(), keyVaultClientId.Value!.ToString(), keyVaultClientSecret.Value!.ToString());

            builder.Configuration.AddAzureKeyVault(keyVaultURL.Value!.ToString(), keyVaultClientId.Value!.ToString(), keyVaultClientSecret.Value!.ToString(), new DefaultKeyVaultSecretManager());

            var client = new SecretClient(new Uri(keyVaultURL.Value!.ToString()), credential);

            //Key vault test code
          

            _client = client;

            try
            {
                KeyVaultSecret secret = client.GetSecret(secretName.Value!.ToString());

                Console.WriteLine($"Retrieved secret '{secret.Name}': {secret.Value}");
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine($"Error retrieving secret: {ex.Message}");
            }

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}