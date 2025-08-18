using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KoreanFlashCardApp.Constants.Enums;

namespace KoreanFlashCardApp.Models
{
    public class WordProgress
    {
        public int Word_Progress_ID { get; set; }
        public int Progress_ID { get; set; }
        public int Word_ID { get; set; }

        public DateTime Next_Test_Date { get; set; }
        public int Number_Correct { get; set; }

        public WordProgress(int word_Progress_ID, int progress_ID, int word_ID, DateTime next_Test_Date, int number_Correct)
        {
            Word_Progress_ID = word_Progress_ID;
            Progress_ID = progress_ID;
            Word_ID = word_ID;
            Next_Test_Date = next_Test_Date;
            Number_Correct = number_Correct;
        }
    }
}
