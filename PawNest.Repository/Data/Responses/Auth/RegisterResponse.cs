using PawNest.Repository.Data.Responses.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.Repository.Data.Responses.Auth
{
    public class RegisterResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public CreateUserResponse? User { get; set; }
    }
}
