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
        public const string RegisterEndpoint = ApiEndpoint + "/register"; // to do
        public const string ChangePasswordEndpoint = ApiEndpoint + "/changepassword"; // to do
        public const string RefreshTokenEndpoint = ApiEndpoint + "/refreshtoken"; // to do
        public const string LogoutEndpoint = ApiEndpoint + "/logout"; // to do
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
    }
}