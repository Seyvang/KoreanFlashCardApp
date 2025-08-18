using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanFlashCardApp.Constants
{
    public class Enums
    {
        public enum Language
        {
            Undefined,
            English,
            Korean
        }

        public enum Word_Type
        {
            Undefined,
            Noun,
            Pronoun,
            Numeral,
            Determiner,
            Adverb,
            Interjection,
            Particle,
            Verb,
            Adjective,
        }
    }
}
