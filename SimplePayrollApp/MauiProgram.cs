using Microsoft.Extensions.Logging;
using SimplePayrollApp.Services;
using SimplePayrollApp.ViewModels;
using SimplePayrollApp.Views;
using QuestPDF.Infrastructure;
using CommunityToolkit.Maui; // Add this import

namespace SimplePayrollApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            // Set QuestPDF license to Community (free for most uses)
            QuestPDF.Settings.License = LicenseType.Community;

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit() // Add this line to initialize Community Toolkit
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register services
            builder.Services.AddSingleton<IDialogService, DialogService>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddSingleton<IPdfService, PdfService>();
            builder.Services.AddSingleton<IShareService, ShareService>();

            // Register view models
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<ResultsViewModel>();

            // Register views
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<ResultsPage>();

            // Register app
            builder.Services.AddSingleton<App>(serviceProvider => new App(serviceProvider));

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
