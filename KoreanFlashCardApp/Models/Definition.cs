using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KoreanFlashCardApp.Constants.Enums;

namespace KoreanFlashCardApp.Models
{
    public class Definition
    {
        public int Definition_ID { get; set; }
        public Language Language_ID{ get; set; }
        public string Definition_Description { get; set; }

        public Definition(int definition_ID, int language_ID, string definition_Description)
        {
            Definition_ID = definition_ID;
            Language_ID = (Language)language_ID;
            Definition_Description = definition_Description;
        }
    }
}
