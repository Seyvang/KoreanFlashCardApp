using KoreanFlashCardApp.Models;
using System.Text.Json;

namespace KoreanFlashCardApp.Helpers
{
    public class ProgressProvider
    {
        private readonly string progressFileName = "progress.json";
        private readonly string progressBackupFileName = "progress.backup.json";
        private List<WordProgress> wordProgress;

        public ProgressProvider()
        {
            wordProgress = new List<WordProgress>();
        }

        public IReadOnlyList<WordProgress> WordProgress => wordProgress;

        public async Task SaveProgressAsync(int word_ID, bool answeredCorrectly)
        {
            var matchedWord = wordProgress.FirstOrDefault(x => x.Word_ID == word_ID);
            if (matchedWord != null)
            {
                matchedWord.Number_Correct = answeredCorrectly ? matchedWord.Number_Correct + 1 : 0;
                matchedWord.Next_Test_Date = answeredCorrectly
                    ? HandleDateCalculation(matchedWord.Number_Correct)
                    : DateTime.Today;
            }
            else
            {
                var numberCorrect = answeredCorrectly ? 1 : 0;
                var nextTestDate = answeredCorrectly ? HandleDateCalculation(1) : DateTime.Today;
                wordProgress.Add(new WordProgress(0, 0, word_ID, nextTestDate, numberCorrect));
            }

            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            var json = JsonSerializer.Serialize(wordProgress);
            var filePath = Path.Combine(FileSystem.AppDataDirectory, progressFileName);
            var backupFilePath = Path.Combine(FileSystem.AppDataDirectory, progressBackupFileName);

            await File.WriteAllTextAsync(filePath, json);
            await File.WriteAllTextAsync(backupFilePath, json);
        }

        public async Task LoadProgressAsync()
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, progressFileName);
            var backupFilePath = Path.Combine(FileSystem.AppDataDirectory, progressBackupFileName);

            var loadedProgress = await TryLoadAsync(filePath) ?? await TryLoadAsync(backupFilePath);
            if (loadedProgress is not null)
            {
                wordProgress = loadedProgress;
                return;
            }

            wordProgress = new List<WordProgress>();
        }

        public void LoadProgress()
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, progressFileName);
            var backupFilePath = Path.Combine(FileSystem.AppDataDirectory, progressBackupFileName);

            var loadedProgress = TryLoad(filePath) ?? TryLoad(backupFilePath);
            wordProgress = loadedProgress ?? new List<WordProgress>();
        }

        private static DateTime HandleDateCalculation(int numberCorrect)
        {
            var dayMultiplier = Math.Pow(2, Math.Max(0, numberCorrect - 1));
            return DateTime.Today.AddDays(dayMultiplier);
        }

        private static async Task<List<WordProgress>?> TryLoadAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            try
            {
                var json = await File.ReadAllTextAsync(filePath);
                return JsonSerializer.Deserialize<List<WordProgress>>(json);
            }
            catch
            {
                return null;
            }
        }

        private static List<WordProgress>? TryLoad(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            try
            {
                var json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<List<WordProgress>>(json);
            }
            catch
            {
                return null;
            }
        }
    }
}
