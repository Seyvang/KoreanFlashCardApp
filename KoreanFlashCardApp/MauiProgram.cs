using KoreanFlashCardApp.Helpers;
using KoreanFlashCardApp.ViewModels;
using Microsoft.Extensions.Logging;

namespace KoreanFlashCardApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            AddServices(builder.Services);
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        public static void AddServices(IServiceCollection services)
        {
            services.AddSingleton<IWordProvider, WordProvider>();
            services.AddSingleton<ProgressProvider>();
            services.AddSingleton<FlashCardProvider>();
            services.AddSingleton<AppShell>();
            services.AddSingleton<MainPageViewModel>();
            services.AddSingleton<MainPage>();
            services.AddTransient<StudySessionPageViewModel>();
            services.AddTransient<StudySessionPage>();
        }
    }
}
