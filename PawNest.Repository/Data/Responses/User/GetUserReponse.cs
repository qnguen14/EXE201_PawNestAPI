using PawNest.Repository.Data.Responses.Booking;
using PawNest.Repository.Data.Responses.Pet;

namespace PawNest.Repository.Data.Responses.User;

public class GetUserReponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string Role { get; set; }
    public string AvatarUrl { get; set; }
    public List<GetPetResponse> Pets { get; set; } 
    public List<GetBookingResponse> Bookings { get; set; }
}