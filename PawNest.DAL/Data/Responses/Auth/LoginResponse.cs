using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.DAL.Data.Requests.Auth
{
    public class LoginResponse
    {
        public bool IsUnauthorized { get; set; } = false;
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string Email { get; set; }
    }
}
