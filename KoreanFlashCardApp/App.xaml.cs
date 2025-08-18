using KoreanFlashCardApp.Helpers;

namespace KoreanFlashCardApp
{
    public partial class App : Application
    {
        public AppShell _firstPage;
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _firstPage = serviceProvider.GetService<AppShell>();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var serviceProvider = Handler?.GetServiceProvider();
            var wordProvider = serviceProvider.GetRequiredService<IWordProvider>();
            wordProvider.LoadData();
            return new Window(new AppShell());
        }
    }
}