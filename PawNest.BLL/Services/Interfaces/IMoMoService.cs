using PawNest.DAL.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.BLL.Services.Interfaces
{
    public interface IMoMoService
    {
        Task<string> CreatePaymentUrlAsync(Payment payment, string? returnUrl = null);
        bool ValidateCallback(IDictionary<string, string> payload, out Guid paymentId, out bool success);
    }
}
