using PawNest.Repository.Data.Entities;
using PawNest.Repository.Data.Requests.Pet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.Repository.Data.Requests.Booking
{
    public class UpdateBookingRequest
    {
        public bool IsPaid { get; set; }
    }
}
