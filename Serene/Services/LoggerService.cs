using Microsoft.Extensions.Logging;


/// <summary>
/// Provides logging functionality for the Serene application.
/// </summary>
/// <remarks>
/// This service wraps the built-in Microsoft.Extensions.Logging functionality
/// and exposes methods to log informational messages, warnings, and errors.
/// The <see cref="LogError"/> method optionally accepts an Exception object
/// for detailed error logging. It can be used throughout the application
/// to standardize and centralize logging.
/// </remarks>
namespace Serene.Services
{
    public interface ILoggerService 
    {
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message, Exception? ex = null);
    }

    public class LoggerService : ILoggerService
    {
        private readonly ILogger<LoggerService> _logger;

        public LoggerService(ILogger<LoggerService> logger)
        {
            _logger = logger;
        }

        public void LogInfo(string message)
        {
            _logger.LogInformation(message);
        }

        public void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }

        public void LogError(string message, Exception? ex = null)
        {
            if (ex != null)
                _logger.LogError(ex, message);
            else
                _logger.LogError(message);
        }
    }
}
