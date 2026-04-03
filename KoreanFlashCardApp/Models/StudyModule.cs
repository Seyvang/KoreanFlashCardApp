using System.Collections.Generic;
using System.Linq;

namespace KoreanFlashCardApp.Models
{
    public class StudyModule
    {
        public StudyModule(int moduleNumber, int startIndex, IReadOnlyList<Word> words, int studiedCount, int dueCount)
        {
            ModuleNumber = moduleNumber;
            StartIndex = startIndex;
            Words = words;
            StudiedCount = studiedCount;
            DueCount = dueCount;
        }

        public int ModuleNumber { get; }

        public int StartIndex { get; }

        public IReadOnlyList<Word> Words { get; }

        public int StudiedCount { get; }

        public int DueCount { get; }

        public int Count => Words.Count;

        public string ModuleLabel => $"Module {ModuleNumber:00}";

        public string RangeLabel => $"Words {StartIndex + 1}-{StartIndex + Count}";

        public string SpanLabel => $"{Words.First().Word_Name} to {Words.Last().Word_Name}";

        public string ProgressLabel => $"{StudiedCount}/{Count} studied";

        public string DueLabel => DueCount == 0 ? "Nothing due" : $"{DueCount} due today";
    }
}
