using KoreanFlashCardApp.Models;

namespace KoreanFlashCardApp.Data
{
    class KoreanWordSentences
    {
        public static readonly WordSentenceImport[] ListOfWordSentences = [];
    }

    class WordSentenceImport
    {
        public WordSentence WordSentence { get; set; }

        public WordSentenceImport(
            int wordSentenceId,
            int wordId,
            int? translationId,
            string sentenceText,
            int languageId,
            string? translationText = null,
            bool isPrimary = false,
            int sortOrder = 0)
        {
            WordSentence = new WordSentence(
                wordSentenceId,
                wordId,
                translationId,
                sentenceText,
                languageId,
                translationText,
                isPrimary,
                sortOrder);
        }
    }
}
