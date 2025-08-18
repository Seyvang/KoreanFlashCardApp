using KoreanFlashCardApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanFlashCardApp.Helpers
{
    public static class ExtensionHelpers
    {
        public static IList<Word> GenerateOptions(this IList<Word> wordList, Word targetWord)
        {
            var stopwatch = Stopwatch.StartNew();
            var options = wordList.Where(x => x.Word_Type_ID == targetWord.Word_Type_ID).Take(4);
            var elapsedTime = stopwatch.ElapsedMilliseconds;
            return options.ToList();
        }
    }
}
