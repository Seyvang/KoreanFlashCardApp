using KoreanFlashCardApp.ViewModels;

namespace KoreanFlashCardApp.Controls;

[XamlCompilation(XamlCompilationOptions.Compile)]
public abstract partial class BasePage : ContentPage
{
    protected BasePage(IPageLifeCycleAware viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        if (BindingContext is IPageLifeCycleAware vm)
        {
            vm.OnAppearing();
        }

        base.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        if (BindingContext is IPageLifeCycleAware vm)
        {
            vm.OnDisappearing();
        }

        base.OnDisappearing();
    }
}
