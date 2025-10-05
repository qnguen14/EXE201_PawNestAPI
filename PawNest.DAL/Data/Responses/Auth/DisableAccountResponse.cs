using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.DAL.Data.Responses.Auth
{
    public class DisableAccountResponse
    {
        public bool IsDisabled { get; set; }
        public required string Message { get; set; }
    }
}
