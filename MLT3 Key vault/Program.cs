using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using System;

namespace ConfigurationDemo
{
    public class Program
    {
        // Static property för att lagra Key Vault status
        public static string KeyVaultStatus { get; private set; } = "Not Configured";
        public static string KeyVaultError { get; private set; } = null;

        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // ===== CONFIGURATION MED TRY-CATCH =====
            if (!builder.Environment.IsDevelopment())
            {
                string keyVaultName = builder.Configuration["KeyVaultName"];

                if (string.IsNullOrEmpty(keyVaultName))
                {
                    KeyVaultStatus = "Not Configured - KeyVaultName is empty";
                    Console.WriteLine("WARNING: KeyVaultName not found in Application Settings");
                    Console.WriteLine("App will start without Key Vault integration");
                }
                else
                {
                    try
                    {
                        Console.WriteLine($"Attempting to connect to Key Vault: {keyVaultName}");

                        Uri keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net");
                        Console.WriteLine($"Key Vault URI: {keyVaultUri}");

                        DefaultAzureCredential credential = new DefaultAzureCredential();
                        Console.WriteLine("Created DefaultAzureCredential");

                        builder.Configuration.AddAzureKeyVault(keyVaultUri, credential);

                        KeyVaultStatus = "Connected Successfully";
                        Console.WriteLine($"SUCCESS: Key Vault '{keyVaultName}' connected successfully!");
                    }
                    catch (UriFormatException ex)
                    {
                        KeyVaultStatus = "Failed - Invalid Key Vault Name";
                        KeyVaultError = $"Invalid Key Vault name format: {keyVaultName}";

                        Console.WriteLine("ERROR: Invalid Key Vault name format!");
                        Console.WriteLine($"KeyVaultName: {keyVaultName}");
                        Console.WriteLine("Expected format: 'my-keyvault-name' (not a URL)");
                        Console.WriteLine($"Exception: {ex.Message}");
                        Console.WriteLine("App will continue without Key Vault");
                    }
                    catch (Azure.Identity.AuthenticationFailedException ex)
                    {
                        KeyVaultStatus = "Failed - Authentication Error";
                        KeyVaultError = "Managed Identity not properly configured or lacks permissions";

                        Console.WriteLine("ERROR: Authentication failed to Key Vault!");
                        Console.WriteLine("This usually means:");
                        Console.WriteLine("  1. Managed Identity is not enabled");
                        Console.WriteLine("  2. Access Policy is missing in Key Vault");
                        Console.WriteLine($"Exception: {ex.Message}");
                        Console.WriteLine("App will continue without Key Vault");
                    }
                    catch (Azure.RequestFailedException ex)
                    {
                        KeyVaultStatus = "Failed - Access Denied";
                        KeyVaultError = $"Key Vault access denied (Status: {ex.Status})";

                        Console.WriteLine("ERROR: Access denied to Key Vault!");
                        Console.WriteLine($"Status Code: {ex.Status}");
                        Console.WriteLine($"Error: {ex.Message}");

                        if (ex.Status == 403)
                        {
                            Console.WriteLine("This is a 403 Forbidden error. Check:");
                            Console.WriteLine("  1. App Service has Managed Identity enabled");
                            Console.WriteLine("  2. Key Vault has Access Policy for this app");
                            Console.WriteLine("  3. Access Policy has 'Get' and 'List' permissions");
                        }

                        Console.WriteLine("App will continue without Key Vault");
                    }
                    catch (Exception ex)
                    {
                        KeyVaultStatus = "Failed - Unknown Error";
                        KeyVaultError = ex.Message;

                        Console.WriteLine("ERROR: Unexpected error loading Key Vault!");
                        Console.WriteLine($"Exception Type: {ex.GetType().Name}");
                        Console.WriteLine($"Message: {ex.Message}");
                        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                        Console.WriteLine("App will continue without Key Vault");
                    }
                }
            }
            else
            {
                KeyVaultStatus = "Skipped - Development Environment";
                Console.WriteLine("Development environment - Key Vault not loaded");
            }

            // ===== SERVICES =====
            Console.WriteLine("Adding services...");
            builder.Services.AddControllers();

            // ===== BUILD APP =====
            Console.WriteLine("Building application...");
            WebApplication app = builder.Build();

            // ===== MIDDLEWARE =====
            Console.WriteLine("Configuring middleware...");
            app.UseRouting();
            app.UseAuthorization();

            // ===== ENDPOINTS =====
            app.MapGet("/", () => "App with Key Vault integration (with error handling)!");

            app.MapGet("/health", (IWebHostEnvironment env) =>
            {
                return new
                {
                    status = "healthy",
                    timestamp = DateTime.UtcNow,
                    environment = env.EnvironmentName
                };
            });

            app.MapGet("/keyvault-status", (IConfiguration config) =>
            {
                return new
                {
                    status = KeyVaultStatus,
                    error = KeyVaultError,
                    keyVaultName = config["KeyVaultName"] ?? "Not Set",
                    hasManagedIdentity = !string.IsNullOrEmpty(
                        Environment.GetEnvironmentVariable("MSI_ENDPOINT")),
                    timestamp = DateTime.UtcNow
                };
            });

            app.MapControllers();

            Console.WriteLine("Application started successfully!");
            Console.WriteLine($"Key Vault Status: {KeyVaultStatus}");

            app.Run();
        }
    }
}