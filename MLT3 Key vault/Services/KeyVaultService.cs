using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace ConfigurationDemo.Services
{
    public interface IKeyVaultService
    {
        Task<string> GetSecretAsync(string secretName);
        Task<bool> SecretExistsAsync(string secretName);
    }

    public class KeyVaultService : IKeyVaultService
    {
        private readonly SecretClient _secretClient;
        private readonly bool _isEnabled;

        public KeyVaultService(IConfiguration configuration)
        {
            string keyVaultName = configuration["KeyVaultName"];

            if (!string.IsNullOrEmpty(keyVaultName))
            {
                Uri keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net");
                _secretClient = new SecretClient(keyVaultUri, new DefaultAzureCredential());
                _isEnabled = true;
            }
            else
            {
                _isEnabled = false;
            }
        }

        public async Task<string> GetSecretAsync(string secretName)
        {
            if (!_isEnabled)
            {
                throw new InvalidOperationException("Key Vault is not configured");
            }

            try
            {
                Azure.Response<KeyVaultSecret> secret = await _secretClient.GetSecretAsync(secretName);
                return secret.Value.Value;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get secret '{secretName}' from Key Vault", ex);
            }
        }

        public async Task<bool> SecretExistsAsync(string secretName)
        {
            if (!_isEnabled)
            {
                return false;
            }

            try
            {
                await _secretClient.GetSecretAsync(secretName);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}