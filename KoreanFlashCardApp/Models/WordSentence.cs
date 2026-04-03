using System.Text.Json.Serialization;
using static KoreanFlashCardApp.Constants.Enums;

namespace KoreanFlashCardApp.Models
{
    public class WordSentence
    {
        public int Word_Sentence_ID { get; set; }
        public int Word_ID { get; set; }
        public int? Translation_ID { get; set; }
        public string Sentence_Text { get; set; }
        public Language Language_ID { get; set; }
        public string? Translation_Text { get; set; }
        public bool IsPrimary { get; set; }
        public int Sort_Order { get; set; }

        public WordSentence(
            int wordSentenceId,
            int wordId,
            int? translationId,
            string sentenceText,
            int languageId,
            string? translationText = null,
            bool isPrimary = false,
            int sortOrder = 0)
        {
            Word_Sentence_ID = wordSentenceId;
            Word_ID = wordId;
            Translation_ID = translationId;
            Sentence_Text = sentenceText;
            Language_ID = (Language)languageId;
            Translation_Text = translationText;
            IsPrimary = isPrimary;
            Sort_Order = sortOrder;
        }

        [JsonIgnore]
        public Translation? Translation { get; set; }
    }
}
