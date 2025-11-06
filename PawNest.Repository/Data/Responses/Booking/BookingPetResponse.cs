using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.Repository.Data.Responses.Booking
{
    public class BookingPetResponse
    {
        public Guid PetId { get; set; }
        public string PetName { get; set; }
        public string Species { get; set; }
        public string Breed { get; set; }
    }
}
