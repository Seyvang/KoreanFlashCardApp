using KoreanFlashCardApp.Helpers;

namespace KoreanFlashCardApp
{
    public partial class App : Application
    {
        private readonly AppShell _appShell;

        public App(AppShell appShell, IWordProvider wordProvider)
        {
            InitializeComponent();

            _appShell = appShell;
            wordProvider.LoadData();
        }

        protected override Window CreateWindow(IActivationState? activationState) => new(_appShell);
    }
}
