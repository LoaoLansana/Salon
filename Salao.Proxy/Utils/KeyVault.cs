using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Salao.Proxy.Utils
{
    public class KeyVault
    {
        private string _key_vault_uri;
        private string _tenant_id;
        private string _client_id;
        private string _client_secret;
        private SecretClientOptions options;

        public KeyVault(string key_vault_uri, string tenant_id, string client_id, string client_secret)
        {
            this._key_vault_uri = key_vault_uri;
            this._tenant_id = tenant_id;
            this._client_id = client_id;
            this._client_secret = client_secret;
            this.options = new SecretClientOptions()
            {
                Retry =
                {
                    Delay = TimeSpan.FromSeconds(2),
                    MaxDelay = TimeSpan.FromSeconds(16),
                    MaxRetries = 5,
                    Mode = RetryMode.Exponential
                }
            };
        }

        public string getConnectionString(string secret_name)
        {
            var client = new SecretClient(new Uri(this._key_vault_uri), new ClientSecretCredential(this._tenant_id.ToString(), this._client_id.ToString(), this._client_secret.ToString()), this.options);
            KeyVaultSecret secret = client.GetSecret(secret_name.ToString());
            return secret.Value;
        }
    }
}
