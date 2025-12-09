using PawNest.Repository.Data.Entities;
using PawNest.Repository.Data.Responses.Pet;

namespace PawNest.Repository.Data.Responses.Booking;

public class GetBookingDetailsResponse
{
    public Guid BookingId { get; set; }
    public PickUpTime PickUpTime { get; set; }
    public PickUpStatus PickUpStatus { get; set; }
    public DateOnly BookingDate { get; set; }
    public BookingStatus Status { get; set; }
    public decimal TotalPrice { get; set; }
    public bool IsPaid { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid FreelancerId { get; set; }
    public BookingUserResponse Freelancer { get; set; }
    public Guid CustomerId { get; set; }
    public BookingUserResponse Customer { get; set; }
    public List<BookingServiceResponse> Services { get; set; }
    public List<GetPetResponse> Pets { get; set; } = new List<GetPetResponse>();
}