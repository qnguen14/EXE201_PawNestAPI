using PawNest.Repository.Data.Requests.Booking;
using PawNest.Repository.Data.Responses.Booking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PawNest.Repository.Data.Entities;

namespace PawNest.Services.Services.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<GetBookingResponse>> GetAllBookingsAsync();
        Task<GetBookingResponse> GetBookingByIdAsync(Guid bookingId);
        Task<GetBookingResponse> GetBookingDetailsAsync(Guid bookingId);
        Task<GetBookingResponse> CreateBookingAsync(CreateBookingRequest request);
        Task<GetBookingUpdateResponse> UpdateBookingAsync(Guid bookingId, UpdateBookingRequest request);
        Task<bool> CancelBookingAsync(Guid bookingId);
        Task<bool> UpdateBookingStatusAsync(Guid bookingId, BookingStatus status);
        Task<bool> UpdateBookingPickUpStatusAsync(Guid bookingId, PickUpStatus status);
        Task<IEnumerable<GetBookingResponse>> GetBookingHistoryByCustomerAsync(Guid customerId);
        Task<IEnumerable<GetBookingResponse>> GetMyBookingHistoryAsync();
        Task<IEnumerable<GetBookingResponse>> GetBookingHistoryByFreelancerAsync(Guid freelancerId);
    }
}
