using Serene.Common;
using Serene.Data;
using Serene.Entities;
using Microsoft.EntityFrameworkCore;

namespace Serene.Services;

public class AuthService : IAuthService
{
    public UserViewModel? CurrentUser { get; set; }

    private readonly AppDbContext _context;
    private readonly ILoggerService _logger;

    public event Action? OnChange;

    public AuthService(AppDbContext context, ILoggerService logger)
    {
        _context = context;
        _logger = logger;
    }

    private UserViewModel MapUserViewModel(User user)
    {
        return new UserViewModel
        {
            Id = user.Id,
            Username = user.Username,
            CurrentStreak = user.CurrentStreak,
            LongestStreak = user.LongestStreak
        };
    }

    public UserViewModel? GetCurrentUser() => CurrentUser;

    public Guid GetCurrentUserId() =>
        CurrentUser?.Id ?? Guid.Empty;

    //login
    public async Task<ServiceResult<LoginResponse>> Login(string username, string pin)
    {
        try
        {
            if (!IsValidPin(pin))
            {
                return ServiceResult<LoginResponse>.FailureResult(
                    "PIN must be numeric and 6 digits long"
                );
            }

            var userData = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (userData == null)
            {
                return ServiceResult<LoginResponse>.FailureResult("Invalid username or PIN");
            }

            bool pinMatch = BCrypt.Net.BCrypt.Verify(pin, userData.PIN);

            if (!pinMatch)
            {
                return ServiceResult<LoginResponse>.FailureResult("Invalid username or PIN");
            }

            CurrentUser = MapUserViewModel(userData);
            NotifyStateChanged();

            return ServiceResult<LoginResponse>.SuccessResult(new LoginResponse
            {
                User = CurrentUser
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Login failed: {ex}");
            return ServiceResult<LoginResponse>.FailureResult("Login failed");
        }
    }

    //signup
    public async Task<ServiceResult<SignUpResponse>> SignUp(
        string username,
        string pin,
        string journalLockPin)
    {
        try
        {
            if (!IsValidPin(pin) || !IsValidPin(journalLockPin))
            {
                return ServiceResult<SignUpResponse>.FailureResult(
                    "PINs must be numeric and 6 digits long."
                );
            }

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (existingUser != null)
            {
                return ServiceResult<SignUpResponse>.FailureResult("Sorry this username is no longer available.");
            }

            var user = new User
            {
                Username = username,
                PIN = BCrypt.Net.BCrypt.HashPassword(pin),
                JournalPin = BCrypt.Net.BCrypt.HashPassword(journalLockPin)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return ServiceResult<SignUpResponse>.SuccessResult(new SignUpResponse
            {
                UserId = user.Id
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Signup failed: {ex}");
            return ServiceResult<SignUpResponse>.FailureResult("Sorry! The signup failed.");
        }
    }

    //logout
    public async Task<ServiceResult<object?>> Logout()
    {
        await Task.Delay(800);
        CurrentUser = null;
        NotifyStateChanged();
        return ServiceResult<object?>.SuccessResult(null);
    }

    private bool IsValidPin(string pin)
    {
        return pin.All(char.IsDigit) && pin.Length is 6;
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
