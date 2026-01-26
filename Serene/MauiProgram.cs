using Serene.Services;
using Serene.Data;
using Microsoft.Extensions.Logging;
using Blazor.Sonner.Extensions;

namespace Serene
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("WorkSans-Regular.ttf", "WorkSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            // Registering AppDbContext for DI
            builder.Services.AddDbContext<AppDbContext>();
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Services.AddSonner();
            builder.Services.AddSingleton<ThemeService>();
            builder.Services.AddSingleton<ILoggerService, LoggerService>();
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddScoped<IMoodsService, MoodsService>();
            builder.Services.AddScoped<ITagsService, TagsService>();
            builder.Services.AddScoped<IStreakService, StreakService>();
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
