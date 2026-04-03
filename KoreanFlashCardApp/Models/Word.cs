using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static KoreanFlashCardApp.Constants.Enums;

namespace KoreanFlashCardApp.Models
{
    public class Word
    {
        public int Word_ID { get; set; }
        public string Word_Name { get; set; }
        public Language Language_ID { get; set; }
        public Word_Type Word_Type_ID { get; set; }

        public Word(int word_Id, string word_Name, int language_Id, int word_Type_Id)
        {
            Word_ID = word_Id;
            Word_Name = word_Name;
            Language_ID = (Language)language_Id;
            Word_Type_ID = (Word_Type)word_Type_Id;
        }

        [JsonIgnore]
        public Translation[] Translations { get; set; }

        [JsonIgnore]
        public WordSentence[] Sentences { get; set; } = Array.Empty<WordSentence>();

        [JsonIgnore]
        public string PrimaryDefinition =>
            Translations?
                .Select(x => x.Definition?.Definition_Description)
                .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? string.Empty;

        [JsonIgnore]
        public WordSentence? PrimarySentence =>
            Sentences
                .OrderByDescending(x => x.IsPrimary)
                .ThenBy(x => x.Sort_Order)
                .FirstOrDefault();

        [JsonIgnore]
        public string WordTypeDisplay => Word_Type_ID.ToString();
    }
}
