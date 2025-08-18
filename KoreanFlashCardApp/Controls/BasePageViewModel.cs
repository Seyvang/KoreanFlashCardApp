using CommunityToolkit.Mvvm.ComponentModel;
using KoreanFlashCardApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanFlashCardApp.Controls
{
    public abstract partial class BasePageViewModel : ObservableObject, IPageLifeCycleAware
    {
        public BasePageViewModel() { }

        public virtual void OnAppearing()
        {

        }

        public virtual void OnDisappearing()
        {

        }

        public virtual void OnNavigatedTo()
        {

        }

        public virtual void OnNavigatedFrom()
        {

        }

        public virtual void OnFirstAppearing()
        {

        }
    }
}
