using PawNest.DAL.Data.Requests.Booking;
using PawNest.DAL.Data.Responses.Booking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.BLL.Services.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<GetBookingResponse>> GetAllBookingsAsync();
        Task<GetBookingResponse> GetBookingByIdAsync(Guid bookingId);
        Task<GetBookingResponse> CreateBookingAsync(CreateBookingRequest request);
        Task<GetBookingUpdateResponse> UpdateBookingAsync(Guid bookingId, UpdateBookingRequest request);
        Task<bool> CancelBookingAsync(Guid bookingId);
    }
}
