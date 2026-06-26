using KoreanFlashCardApp.Models;

namespace KoreanFlashCardApp.Helpers
{
    public class FlashCardProvider
    {
        public const int ModuleSize = 15;
        private readonly ProgressProvider _progressProvider;

        public FlashCardProvider(ProgressProvider progressProvider)
        {
            _progressProvider = progressProvider;
        }

        public IReadOnlyList<StudyModule> BuildModules(IList<Word> allWords)
        {
            var modules = new List<StudyModule>();
            var progressLookup = _progressProvider.MappedWordProgress;

            for (var startIndex = 0; startIndex < allWords.Count; startIndex += ModuleSize)
            {
                var moduleStudyWords = allWords
                    .Skip(startIndex)
                    .Take(ModuleSize)
                    .Select(word =>
                    {
                        progressLookup.TryGetValue(word.Word_ID, out var progress);
                        return new StudyWord(word, progress);
                    })
                    .Where(studyWord => !studyWord.IsSkipped)
                    .ToList();

                if (moduleStudyWords.Count == 0)
                {
                    continue;
                }

                var studiedCount = moduleStudyWords.Count(studyWord => studyWord.IsStudied);
                var dueCount = moduleStudyWords.Count(studyWord => studyWord.IsDue);

                modules.Add(new StudyModule(
                    moduleNumber: (startIndex / ModuleSize) + 1,
                    startIndex: startIndex,
                    endIndex: Math.Min(startIndex + ModuleSize, allWords.Count),
                    words: moduleStudyWords.Select(studyWord => studyWord.Word).ToList(),
                    studiedCount: studiedCount,
                    dueCount: dueCount));
            }

            return modules;
        }

        public IReadOnlyList<FlashCard> BuildSession(IList<Word> moduleWords, IList<Word> allWords)
        {
            var progressLookup = _progressProvider.MappedWordProgress;

            return moduleWords
                .Where(word => !progressLookup.TryGetValue(word.Word_ID, out var progress) || !progress.SkipWord)
                .OrderBy(word => GetPriority(word, progressLookup))
                .ThenBy(word => progressLookup.TryGetValue(word.Word_ID, out var progress)
                    ? progress.Next_Test_Date
                    : DateTime.MinValue)
                .ThenBy(word => word.Word_ID)
                .Select(word => new FlashCard(word, allWords))
                .ToList();
        }

        public IList<Word> GetDueTodayWords(IList<Word> allWords, int maxNumberOfWords = 0)
        {
            var progressLookup = _progressProvider.MappedWordProgress;

            var returnWords = allWords
                .Where(word =>
                    progressLookup.TryGetValue(word.Word_ID, out var progress) &&
                    !progress.SkipWord &&
                    progress.Next_Test_Date.Date <= DateTime.Today)
                .OrderBy(word => progressLookup[word.Word_ID].Next_Test_Date)
                .ThenBy(word => word.Word_ID);

            if (maxNumberOfWords > 0)
            {
                return returnWords.Take(maxNumberOfWords).ToList();
            }
            else
            {
                return returnWords.ToList();
            }
        }

        public int GetDueTodayCount(IList<Word> allWords)
        {
            return GetDueTodayWords(allWords).Count;
        }

        public IList<Word> GetModuleWords(IList<Word> allWords, int moduleNumber)
        {
            var startIndex = Math.Max(0, (moduleNumber - 1) * ModuleSize);
            var progressLookup = _progressProvider.MappedWordProgress;

            return allWords
                .Skip(startIndex)
                .Take(ModuleSize)
                .Where(word => !progressLookup.TryGetValue(word.Word_ID, out var progress) || !progress.SkipWord)
                .ToList();
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
