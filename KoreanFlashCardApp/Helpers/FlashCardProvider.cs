using KoreanFlashCardApp.Models;

namespace KoreanFlashCardApp.Helpers
{
    public class FlashCardProvider
    {
        public const int ModuleSize = 15;

        public IReadOnlyList<StudyModule> BuildModules(IList<Word> allWords, IReadOnlyList<WordProgress> progressEntries)
        {
            var modules = new List<StudyModule>();
            var progressLookup = progressEntries
                .GroupBy(x => x.Word_ID)
                .ToDictionary(group => group.Key, group => group.OrderByDescending(x => x.Next_Test_Date).First());

            for (var startIndex = 0; startIndex < allWords.Count; startIndex += ModuleSize)
            {
                var moduleWords = allWords.Skip(startIndex).Take(ModuleSize).ToList();
                if (moduleWords.Count == 0)
                {
                    continue;
                }

                var studiedCount = moduleWords.Count(word => progressLookup.ContainsKey(word.Word_ID));
                var dueCount = moduleWords.Count(word =>
                    progressLookup.TryGetValue(word.Word_ID, out var progress) &&
                    progress.Next_Test_Date.Date <= DateTime.Today);

                modules.Add(new StudyModule(
                    moduleNumber: modules.Count + 1,
                    startIndex: startIndex,
                    words: moduleWords,
                    studiedCount: studiedCount,
                    dueCount: dueCount));
            }

            return modules;
        }

        public IReadOnlyList<FlashCard> BuildSession(IList<Word> moduleWords, IList<Word> allWords, IReadOnlyList<WordProgress> progressEntries)
        {
            var progressLookup = progressEntries
                .GroupBy(x => x.Word_ID)
                .ToDictionary(group => group.Key, group => group.OrderByDescending(x => x.Next_Test_Date).First());

            return moduleWords
                .OrderBy(word => GetPriority(word, progressLookup))
                .ThenBy(word => progressLookup.TryGetValue(word.Word_ID, out var progress)
                    ? progress.Next_Test_Date
                    : DateTime.MinValue)
                .ThenBy(word => word.Word_ID)
                .Select(word => new FlashCard(word, allWords))
                .ToList();
        }

        public IList<Word> GetDueTodayWords(IList<Word> allWords, IReadOnlyList<WordProgress> progressEntries)
        {
            var progressLookup = progressEntries
                .GroupBy(x => x.Word_ID)
                .ToDictionary(group => group.Key, group => group.OrderByDescending(x => x.Next_Test_Date).First());

            return allWords
                .Where(word =>
                    progressLookup.TryGetValue(word.Word_ID, out var progress) &&
                    progress.Next_Test_Date.Date <= DateTime.Today)
                .OrderBy(word => progressLookup[word.Word_ID].Next_Test_Date)
                .ThenBy(word => word.Word_ID)
                .ToList();
        }

        public int GetDueTodayCount(IList<Word> allWords, IReadOnlyList<WordProgress> progressEntries)
        {
            return GetDueTodayWords(allWords, progressEntries).Count;
        }

        public IList<Word> GetModuleWords(IList<Word> allWords, int moduleNumber)
        {
            var startIndex = Math.Max(0, (moduleNumber - 1) * ModuleSize);
            return allWords.Skip(startIndex).Take(ModuleSize).ToList();
        }

        private static int GetPriority(Word word, IReadOnlyDictionary<int, WordProgress> progressLookup)
        {
            if (!progressLookup.TryGetValue(word.Word_ID, out var progress))
            {
                return 1;
            }

            return progress.Next_Test_Date.Date <= DateTime.Today ? 0 : 2;
        }
    }
}
