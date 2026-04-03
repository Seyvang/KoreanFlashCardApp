using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KoreanFlashCardApp.Constants.Enums;

namespace KoreanFlashCardApp.Models
{
    class WordImport
    {
        public Word Word { get; set; }
        public List<TranslationImport> Translations { get; set; }
        public WordImport(int Word_ID, string termWord, string translation, string? translation2, int wordType)
        {
            Word = new Word(Word_ID, termWord, 2, wordType);
            Translations = new List<TranslationImport> { new TranslationImport { Translation = new Translation(0, Word.Word_ID, 0), Definition = new Definition(0, 1, translation) } };
            if (!String.IsNullOrWhiteSpace(translation2))
            {
                Translations.Add(new TranslationImport { Translation = new Translation(0, Word.Word_ID, 0), Definition = new Definition(0, 1, translation2!) });
            }
        }
    }

    class TranslationImport
    {
        public Translation Translation { get; set; }
        public Definition Definition { get; set; }
    }
}
