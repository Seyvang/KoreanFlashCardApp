using KoreanFlashCardApp.Constants;
using KoreanFlashCardApp.Models;
using static KoreanFlashCardApp.Constants.Enums;

namespace KoreanFlashCardApp.Helpers
{
    public interface IWordProvider
    {
        public Language SourceLanguage { get; set; }
        public Language LearnLanguage { get; set; }
        IList<Word> Words { get; set; }

        void LoadData();
    }
}