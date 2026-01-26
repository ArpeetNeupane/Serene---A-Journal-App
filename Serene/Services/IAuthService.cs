using Serene.Common;

public interface IAuthService
{
    Task<ServiceResult<LoginResponse>> Login(string email, string pin);
    Task<ServiceResult<SignUpResponse>> SignUp(string username, string pin, string journalPin);
    Task<ServiceResult<object?>> Logout();  //returns null that's why it's type is object
    UserViewModel? GetCurrentUser();

    //Task RefreshUser();

    public event Action? OnChange;
}