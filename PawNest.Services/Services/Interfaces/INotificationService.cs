using System;
using System.Threading.Tasks;

namespace PawNest.Services.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendBookingCreatedNotificationAsync(Guid freelancerId, Guid bookingId, string customerName);
        Task SendBookingStatusUpdatedNotificationAsync(Guid customerId, Guid bookingId, string status);
        Task SendBookingPickUpStatusUpdatedNotificationAsync(Guid customerId, Guid bookingId, string pickUpStatus);
        Task SendBookingCancelledNotificationAsync(Guid freelancerId, Guid customerId, Guid bookingId);
    }
}