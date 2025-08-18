using CommunityToolkit.Mvvm.ComponentModel;
using KoreanFlashCardApp.Controls;
using KoreanFlashCardApp.Helpers;
using KoreanFlashCardApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanFlashCardApp.ViewModels
{
    public partial class MainPageViewModel : BasePageViewModel
    {
        //public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Word> Words { get; set; } = new();

        [ObservableProperty] private string _title;

        public MainPageViewModel(IWordProvider wordProvider)
        {
            Words = new ObservableCollection<Word>(wordProvider.Words.Take(20));

            Title = "Test why isn't this working";
            OnPropertyChanged(nameof(Words));
        }

        //public override OnNavigatedTo()
    }
}
