using Serene.Services;
using Serene.Data;
using Microsoft.Extensions.Logging;
using Blazor.Sonner.Extensions;
using QuestPDF.Infrastructure;
using CommunityToolkit.Maui;

namespace Serene
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            //setting questpdf license
            QuestPDF.Settings.License = LicenseType.Community;

            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit() //important for file saving
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("WorkSans-Regular.ttf", "WorkSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            //registering appdbcontext for dependency injection
            builder.Services.AddDbContext<AppDbContext>();
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Services.AddSonner();

            builder.Services.AddSingleton<ThemeService>();
            builder.Services.AddSingleton<ILoggerService, LoggerService>();
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddScoped<IJournalService, JournalService>();
            builder.Services.AddScoped<IStreakService, StreakService>();
            builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
            builder.Services.AddScoped<IExportService, ExportService>();

            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            }

            return app;
        }
    }
}