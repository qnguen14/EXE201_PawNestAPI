namespace PawNest.API.Constants;

public class ApiEndpointConstants
{
    static ApiEndpointConstants() {  }

    public const string RootEndpoint = "/api";
    public const string ApiVersion = "/v1";
    public const string ApiEndpoint = RootEndpoint + ApiVersion;

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
        public const string UserEndpoint = ApiEndpoint + "/user";
        public const string GetUserEndpoint = UserEndpoint + "/{id}";
        public const string GetAllUsersEndpoint = UserEndpoint + "/getall";
        public const string GetUsersByRoleEndpoint = GetAllUsersEndpoint + "/{role}";
        public const string CreateUserEndpoint = UserEndpoint + "/create";
        public const string UpdateUserEndpoint = UserEndpoint + "/update/{id}";
        public const string DeleteUserEndpoint = UserEndpoint + "/delete/{id}";
        public const string ToggleUserStatusEndpoint = UserEndpoint + "/toggle-status/{id}";
    
        // Profile endpoints
        public const string GetMyProfileEndpoint = UserEndpoint + "/profile/me";
        public const string SetRoleEndpoint = UserEndpoint + "/set-role/{id}";
        public const string UpdateProfileEndpoint = UserEndpoint + "/profile/{id}";
        public const string UpdateMyProfileEndpoint = UserEndpoint + "/profile/me";
        public const string UpdateAvatarEndpoint = UserEndpoint + "/avatar/{id}";
        public const string UpdateMyAvatarEndpoint = UserEndpoint + "/avatar/me";

        // Freelancer endpoints 
        public const string FreelancerEndpoint = ApiEndpoint + "/freelancer";
        public const string GetFreelancersByServiceEndpoint = FreelancerEndpoint + "/service/{serviceId}";
        public const string GetFreelancerByIdEndpoint = FreelancerEndpoint + "/{id}";
        public const string GetAllFreelancersEndpoint = FreelancerEndpoint + "/getall";
        public const string SearchFreelancersEndpoint = FreelancerEndpoint + "/search";
        public const string SortFreelancersEndpoint = FreelancerEndpoint + "/sort";
    }
}