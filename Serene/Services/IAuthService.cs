using Serene.Common;


//interface for AuthService
public interface IAuthService
{
    Task<ServiceResult<LoginResponse>> Login(string email, string pin);
    Task<ServiceResult<SignUpResponse>> SignUp(string username, string pin);
    Task<ServiceResult<object?>> Logout();  //returns null that's why it's type is object?
    UserViewModel? GetCurrentUser();

    public event Action? OnChange;
}