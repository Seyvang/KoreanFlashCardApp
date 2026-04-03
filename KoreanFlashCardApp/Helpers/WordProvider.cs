using KoreanFlashCardApp.Data;
using KoreanFlashCardApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KoreanFlashCardApp.Constants.Enums;

namespace KoreanFlashCardApp.Helpers
{
    public class WordProvider : IWordProvider
    {
        public IList<Word> Words { get; set; }
        public Language SourceLanguage { get; set; }
        public Language LearnLanguage { get; set; }

        public WordProvider()
        {
            Words = new List<Word>();
            LearnLanguage = Language.Korean;
            SourceLanguage = Language.English;
        }

        public void LoadData()
        {
            if (Words.Count > 0)
            {
                return;
            }

            foreach (var wordImport in KoreanWords.ListOfWords)
            {
                foreach (var translationImport in wordImport.Translations)
                {
                    translationImport.Translation.Definition = translationImport.Definition;
                }

                wordImport.Word.Translations = wordImport.Translations.Select(x => x.Translation).ToArray();
                Words.Add(wordImport.Word);
            }
        }
    }
}
