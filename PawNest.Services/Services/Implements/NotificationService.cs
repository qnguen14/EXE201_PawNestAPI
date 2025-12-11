using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PawNest.Services.Hubs;
using PawNest.Services.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace PawNest.Services.Services.Implements
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IHubContext<NotificationHub> hubContext, ILogger<NotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task SendBookingCreatedNotificationAsync(Guid freelancerId, Guid bookingId, string customerName)
        {
            try
            {
                var message = new
                {
                    Type = "BookingCreated",
                    BookingId = bookingId,
                    Message = $"New booking request from {customerName}",
                    Timestamp = DateTime.UtcNow
                };

                await _hubContext.Clients.User(freelancerId.ToString())
                    .SendAsync("ReceiveNotification", message);

                _logger.LogInformation($"Booking created notification sent to freelancer {freelancerId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending booking created notification: {ex.Message}");
            }
        }

        public async Task SendBookingStatusUpdatedNotificationAsync(Guid customerId, Guid bookingId, string status)
        {
            try
            {
                var message = new
                {
                    Type = "BookingStatusUpdated",
                    BookingId = bookingId,
                    Status = status,
                    Message = $"Your booking status has been updated to {status}",
                    Timestamp = DateTime.UtcNow
                };

                await _hubContext.Clients.User(customerId.ToString())
                    .SendAsync("ReceiveNotification", message);

                _logger.LogInformation($"Booking status updated notification sent to customer {customerId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending booking status updated notification: {ex.Message}");
            }
        }

        public async Task SendBookingPickUpStatusUpdatedNotificationAsync(Guid customerId, Guid bookingId, string pickUpStatus)
        {
            try
            {
                var message = new
                {
                    Type = "BookingPickUpStatusUpdated",
                    BookingId = bookingId,
                    PickUpStatus = pickUpStatus,
                    Message = $"Your pet pickup status has been updated to {pickUpStatus}",
                    Timestamp = DateTime.UtcNow
                };

                await _hubContext.Clients.User(customerId.ToString())
                    .SendAsync("ReceiveNotification", message);

                _logger.LogInformation($"Pickup status updated notification sent to customer {customerId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending pickup status updated notification: {ex.Message}");
            }
        }

        public async Task SendBookingCancelledNotificationAsync(Guid freelancerId, Guid customerId, Guid bookingId)
        {
            try
            {
                var message = new
                {
                    Type = "BookingCancelled",
                    BookingId = bookingId,
                    Message = "A booking has been cancelled",
                    Timestamp = DateTime.UtcNow
                };

                await _hubContext.Clients.User(freelancerId.ToString())
                    .SendAsync("ReceiveNotification", message);

                _logger.LogInformation($"Booking cancelled notification sent to freelancer {freelancerId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending booking cancelled notification: {ex.Message}");
            }
        }
    }
}
