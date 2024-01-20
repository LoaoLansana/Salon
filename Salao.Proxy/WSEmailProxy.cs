using Salao.Proxy.Interface;
using Salao.Proxy.Utils;
using System.Net;
using System.Net.Mail;

namespace Salao.Proxy
{
    public class WSEmailProxy : IWSEmailProxy
    {
        #region Propriedades
        private const string SmtpServer = "smtp.office365.com";
        private const int SmtpPort = 587;
        private const string SmtpUsername = "joao.lansana@hotmail.com";
        private const string KeyVaultName = "akv-renavam-br-south-dev";
        private const string SecretName = "mail-renavamservice";
        private const string SmtpPassword = "lansana123";
        #endregion

        #region Metodos Públicos
        public async Task SendEmailResetPasswordAsync(string toEmail, string subject, string body, string callbackUrl)
        {
            await Send(toEmail, subject, body.Replace("[CallbackUrl]", callbackUrl));
        }

        public async Task SendPasswordEmail(string email, string body, string subject)
        {
            await Send(email, subject, body);
        }
        #endregion

        #region Metodos Privados
        private async Task Send(string toEmail, string subject, string body)
        {
            var fromEmail = SmtpUsername;
            //string valorSegredo = await ResgatarSegredoDoKeyVault();

            using (var smtpClient = new SmtpClient(SmtpServer, SmtpPort))
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(SmtpUsername, SmtpPassword);
                smtpClient.EnableSsl = true;

                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(fromEmail);
                    mailMessage.To.Add(toEmail);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
        }

        private async Task<string> ResgatarSegredoDoKeyVault()
        {
            try
            {
                //var credential = new ClientSecretCredential(
                //    tenantId: "004a48ba-3882-4361-9219-a4b2e56c0a91",
                //    clientId: "269766d6-9091-45c5-ae11-f534a0bd4da9",
                //    clientSecret: "76o8Q~I6WqQG46zMvW6dYDlL7oDj9_7E9tPjpaLY"
                //);
                string keyVaultUrl = $"https://{KeyVaultName}.vault.azure.net";

                KeyVault keyVault = new KeyVault(keyVaultUrl, "004a48ba-3882-4361-9219-a4b2e56c0a91", "269766d6-9091-45c5-ae11-f534a0bd4da9", "76o8Q~I6WqQG46zMvW6dYDlL7oDj9_7E9tPjpaLY");

                string valorSegredo = keyVault.getConnectionString(SecretName);

                return valorSegredo;
            }
            catch (Exception e)
            {
                // Trate a exceção de acordo com a necessidade do seu aplicativo
                throw;
            }
        }
        #endregion
    }
}
