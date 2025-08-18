using KoreanFlashCardApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanFlashCardApp.Controls;

[XamlCompilation(XamlCompilationOptions.Compile)]
public abstract partial class BasePage : ContentPage
{
    public object _vm;
    //public BasePage() => InitializeComponent();
    public BasePage(IPageLifeCycleAware viewModel)
    {
        InitializeComponent();
        _vm = viewModel;
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

        base.OnAppearing();
    }
}
