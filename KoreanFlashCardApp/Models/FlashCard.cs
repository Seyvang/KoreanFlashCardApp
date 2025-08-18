using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using KoreanFlashCardApp.Helpers;
using static KoreanFlashCardApp.Constants.Enums;

namespace KoreanFlashCardApp.Models
{
    public partial class FlashCard : ObservableObject
    {
        [ObservableProperty]
        private Word _targetWord;

        [ObservableProperty]
        private IList<Word> _wordSelectionOptions = new List<Word>();

        [ObservableProperty]
        private bool _isWordTermQuizzed;

        public FlashCard(Word word, IList<Word> wordList)
        {
            TargetWord = word;
            WordSelectionOptions = wordList.GenerateOptions(word);
        }
    }
}
