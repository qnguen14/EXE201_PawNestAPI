using System.CodeDom;

namespace PawNest.API.Constants;

public class ApiEndpointConstants
{
    static ApiEndpointConstants() {  }

    public const string RootEndpoint = "/api";
    public const string ApiVersion = "/v1";
    public const string ApiEndpoint = RootEndpoint + ApiVersion;
    public const string UserEndpoint = ApiEndpoint + "/user";

    public static class Auth
    {
        public const string AuthEndpoint = ApiEndpoint + "/auth";
        public const string LoginEndpoint = ApiEndpoint + "/login";
        public const string RegisterEndpoint = ApiEndpoint + "/register"; 
        public const string SendResetCodeEndpoint = ApiEndpoint + "/reset"; 
        public const string VerifyResetCodeEndpoint = ApiEndpoint + "/verify/reset"; 
        public const string DisableAccountEndpoint = ApiEndpoint + "/disable"; 
        public const string VerifyDisableCodeEndpoint = ApiEndpoint + "/verify/disable";
        public const string LogoutEndpoint = ApiEndpoint + "/logout"; 
    }

    public static class User
    {
        public const string GetUserEndpoint = UserEndpoint + "/{id}";
        public const string GetAllUsersEndpoint = UserEndpoint + "/getall";
        public const string GetUsersByRoleEndpoint = GetAllUsersEndpoint + "/{role}";
        public const string CreateUserEndpoint = UserEndpoint + "/create";
        public const string UpdateUserEndpoint = UserEndpoint + "/update/{id}";
        public const string DeleteUserEndpoint = UserEndpoint + "/delete/{id}";
        public const string ToggleUserStatusEndpoint = UserEndpoint + "/toggle-status/{id}";

        // Freelancer endpoints 
        public const string FreelancerEndpoint = ApiEndpoint + "/freelancer";
        public const string GetFreelancersByServiceEndpoint = FreelancerEndpoint + "/service/{serviceId}";
        public const string GetFreelancerByIdEndpoint = FreelancerEndpoint + "/{id}";
        public const string GetAllFreelancersEndpoint = FreelancerEndpoint + "/getall";
        public const string SearchFreelancersEndpoint = FreelancerEndpoint + "/search";
        public const string SortFreelancersEndpoint = FreelancerEndpoint + "/sort";
    }

    public static class Pet
    {
        // General use
        public const string PetEndpoint = ApiEndpoint + "/pet";
        public const string GetPetByIdEndpoint = PetEndpoint + "/{id}";
        public const string GetAllPetsEndpoint = PetEndpoint + "/getall";

        // Admin
        public const string CreatePetEndpoint = PetEndpoint + "/create";
        public const string UpdatePetEndpoint = PetEndpoint + "/update/{id}";
        public const string DeletePetEndpoint = PetEndpoint + "/delete/{id}";

        // Customer
        public const string OwnersPetsEndpoint = PetEndpoint + "/user/{userId}/pets";
        public const string AddPetEndpoint = PetEndpoint + "/add/{id}";
        public const string EditPetEndpoint = PetEndpoint + "/edit/{petId}";
        public const string RemovePetEndpoint = PetEndpoint + "/remove/{id}";
        public const string MyPetsEndpoint = PetEndpoint + "/{userId}";
    }

    public static class Booking
    {
        public const string BookingEndpoint = ApiEndpoint + "/booking";
        public const string GetBookingByIdEndpoint = BookingEndpoint + "/{id}";
        public const string GetAllBookingsEndpoint = BookingEndpoint + "/getall";
        public const string CreateBookingEndpoint = BookingEndpoint + "/create";
        public const string UpdateBookingEndpoint = BookingEndpoint + "/update/{id}";
        public const string CancelBookingEndpoint = BookingEndpoint + "/cancel/{id}";
    }

    public static class Service
    {
        public const string ServiceEndpoint = ApiEndpoint + "/service";
        public const string GetServiceByIdEndpoint = ServiceEndpoint + "/{id}";
        public const string GetAllServicesEndpoint = ServiceEndpoint + "/getall";
        public const string CreateServiceEndpoint = ServiceEndpoint + "/create";
        public const string UpdateServiceEndpoint = ServiceEndpoint + "/update/{id}";
        public const string DeleteServiceEndpoint = ServiceEndpoint + "/delete/{id}";
    }
   public static class Payment
    {
        public const string PaymentEndpoint = ApiEndpoint + "/payment";
        public const string CreatePaymentEndpoint = PaymentEndpoint + "/create";
        public const string VNPayCallbackEndpoint = PaymentEndpoint + "/vnpay-callback";
        public const string MoMoCallbackEndpoint = PaymentEndpoint + "/momo-callback";
        public const string MoMoReturnEndpoint = PaymentEndpoint + "/momo-return";
        public const string GetPaymentByBookingIdEndpoint = PaymentEndpoint + "/booking/{bookingId}";
        public const string GetPaymentByIdEndpoint = PaymentEndpoint + "/{paymentId}";
        public const string CancelPaymentEndpoint = PaymentEndpoint + "/{paymentId}/cancel";
    }

    public static class Profile
    {
        // Profile endpoints
        public const string GetMyProfileEndpoint = UserEndpoint + "/profile/me";
        public const string SetRoleEndpoint = UserEndpoint + "/set-role/{id}";
        public const string UpdateProfileEndpoint = UserEndpoint + "/profile/{id}";
        public const string UpdateMyProfileEndpoint = UserEndpoint + "/profile/me";
        public const string UpdateAvatarEndpoint = UserEndpoint + "/avatar/{id}";
        public const string UpdateMyAvatarEndpoint = UserEndpoint + "/avatar/me";
        public const string GetFreelancerProfileEndpoint = UserEndpoint + "/profile/freelancer/me";
    }

    public static class Review
    {
        public const string ReviewEndpoint = ApiEndpoint + "/review";
        public const string GetReviewByIdEndpoint = ReviewEndpoint + "/{id}";
        public const string GetAllReviewsEndpoint = ReviewEndpoint + "/getall";
        public const string CreateReviewEndpoint = ReviewEndpoint + "/create";
        public const string RespondReviewEndpoint = ReviewEndpoint + "/respond/{id}";
        public const string DeleteReviewEndpoint = ReviewEndpoint + "/delete/{id}";
    }
}