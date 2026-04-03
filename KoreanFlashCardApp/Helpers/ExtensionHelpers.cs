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

            var options = wordList
                .Where(x => x.Word_Type_ID == targetWord.Word_Type_ID && x.Word_ID != targetWord.Word_ID)
                .OrderBy(_ => Random.Shared.Next())
                .Take(3)
                .ToList();

            options.Add(targetWord);

            var elapsedTime = stopwatch.ElapsedMilliseconds;
            return options
                .OrderBy(_ => Random.Shared.Next())
                .ToList();
        }
    }
}
