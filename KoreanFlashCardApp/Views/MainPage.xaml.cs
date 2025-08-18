using KoreanFlashCardApp.Controls;
using KoreanFlashCardApp.ViewModels;
using Microsoft.Maui.Controls;

namespace KoreanFlashCardApp
{
    public partial class MainPage : BasePage
    {
        public MainPage(MainPageViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
