using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

var client = new SecretClient(
    new Uri("https://hemligtvalv.vault.azure.net/"),
    new DefaultAzureCredential()
);

// Hämta senaste version
var secret = await client.GetSecretAsync("ConnectionStrings--SQLAZ");
Console.WriteLine(secret.Value.Value);

// Eller specifik version
var secretVersion = await client.GetSecretAsync("ConnectionStrings--SQLAZ", "e0a87340d9924e66b69a4c3d24c25bcd");
Console.WriteLine(secretVersion.Value.Value);