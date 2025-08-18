using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static KoreanFlashCardApp.Constants.Enums;

namespace KoreanFlashCardApp.Models
{
    public class Translation
    {
        public int Translation_ID { get; set; }
        public int Source_Word_ID{ get; set; }

        // either or the Word ID or the definition needs to be supplied. not both
        public int? Translated_Word_ID { get; set; }
        public int? Definition_ID { get; set; }

        public Translation(int translation_Id, int source_Word_Id, int? definition_Id)
        {
            Translation_ID = translation_Id;
            Source_Word_ID = source_Word_Id;
            Definition_ID = definition_Id;
        }

        [JsonIgnore]
        public Definition? Definition { get; set; }

        [JsonIgnore]
        public Translation? Translation_Word { get; set; }
    }
}
