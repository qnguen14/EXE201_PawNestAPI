using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.Services.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendPasswordResetCodeAsync(string toEmail, string resetCode, string userName);
        Task SendBookingConfirmationAsync(string toEmail, string userName, DateTime bookingDate, string serviceName);
        Task SendDisableAccountCodeAsync(string toEmail, string disableCode, string userName);
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
