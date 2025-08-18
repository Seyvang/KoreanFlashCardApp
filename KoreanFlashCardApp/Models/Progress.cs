using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KoreanFlashCardApp.Constants.Enums;

namespace KoreanFlashCardApp.Models
{
    public class Progress
    {
        public int Progress_ID { get; set; }
        public Language Language_ID { get; set; }
        public int User_ID { get; set; }
    }
}
