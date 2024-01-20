using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salao.Proxy.Interface
{
    public interface IWSEmailProxy
    {
        Task SendEmailResetPasswordAsync(string toEmail, string subject, string body, string callbackUrl);
        Task SendPasswordEmail(string email, string body, string subject);
    }
}
