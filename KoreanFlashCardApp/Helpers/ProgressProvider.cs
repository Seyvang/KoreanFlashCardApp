using KoreanFlashCardApp.Models;
using System.Text.Json;

namespace KoreanFlashCardApp.Helpers
{
    public class ProgressProvider
    {
        private readonly string progressFileName = "progress.json";
        private readonly string progressBackupFileName = "progress.backup.json";
        private const int ExportRetentionDays = 7;
        private const string ProgressExportSearchPattern = "progress*.json";
        private List<WordProgress> wordProgress;
        private Dictionary<int, WordProgress> mappedWordProgress;

        public ProgressProvider()
        {
            wordProgress = new List<WordProgress>();
            mappedWordProgress = new Dictionary<int, WordProgress>();
        }

        public IReadOnlyList<WordProgress> WordProgress => wordProgress;

        public IReadOnlyDictionary<int, WordProgress> MappedWordProgress => mappedWordProgress;

        public string ProgressFileName => progressFileName;

        public async Task SaveProgressAsync(int word_ID, bool answeredCorrectly)
        {
            var matchedWord = wordProgress.FirstOrDefault(x => x.Word_ID == word_ID);
            if (matchedWord != null)
            {
                // shouldn't be able to save a word for something that has been purposely skipped
                if (matchedWord.SkipWord)
                {
                    throw new InvalidOperationException();
                }

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

            RebuildMappedWordProgress();
            await SaveAsync();
        }

        public async Task SkipWordProgress(int word_ID)
        {
            var matchedWord = wordProgress.FirstOrDefault(x => x.Word_ID == word_ID);


            if (matchedWord != null)
            {
                matchedWord.SkipWord = true;
                matchedWord.Number_Correct = -1;
            }
            else
            {
                wordProgress.Add(new WordProgress(0, 0, word_ID, DateTime.MaxValue, -1, true));
            }

            RebuildMappedWordProgress();
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

        public async Task<string> ExportProgressToDownloadsAsync()
        {
            await SaveAsync();

            var json = JsonSerializer.Serialize(wordProgress, new JsonSerializerOptions { WriteIndented = true });
            await DeleteOldProgressExportsAsync(DateTimeOffset.Now.AddDays(-ExportRetentionDays));
            return await WriteExportAsync(json);
        }

        public async Task ImportProgressAsync(FileResult fileResult)
        {
            await using var stream = await fileResult.OpenReadAsync();
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();
            var importedProgress = JsonSerializer.Deserialize<List<WordProgress>>(json)
                ?? throw new InvalidOperationException("Selected file does not contain progress data.");

            wordProgress = importedProgress;
            RebuildMappedWordProgress();
            await SaveAsync();
        }

        public async Task LoadProgressAsync()
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, progressFileName);
            var backupFilePath = Path.Combine(FileSystem.AppDataDirectory, progressBackupFileName);

            var loadedProgress = await TryLoadAsync(filePath) ?? await TryLoadAsync(backupFilePath);
            if (loadedProgress is not null)
            {
                wordProgress = loadedProgress;
                RebuildMappedWordProgress();
                return;
            }

            wordProgress = new List<WordProgress>();
            RebuildMappedWordProgress();
        }

        public void LoadProgress()
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, progressFileName);
            var backupFilePath = Path.Combine(FileSystem.AppDataDirectory, progressBackupFileName);

            var loadedProgress = TryLoad(filePath) ?? TryLoad(backupFilePath);
            wordProgress = loadedProgress ?? new List<WordProgress>();
            RebuildMappedWordProgress();
        }

        private void RebuildMappedWordProgress()
        {
            mappedWordProgress = wordProgress
                .GroupBy(x => x.Word_ID)
                .ToDictionary(group => group.Key, group => group.OrderByDescending(x => x.Next_Test_Date).First());
        }

        private static DateTime HandleDateCalculation(int numberCorrect)
        {
            var dayMultiplier = Math.Pow(2, Math.Max(0, numberCorrect - 1));
            return DateTime.Today.AddDays(dayMultiplier);
        }

        private static async Task<string> WriteExportAsync(string json)
        {
#if WINDOWS
            return await WriteExportToDirectoryAsync(GetDownloadsPath(), json);
#elif MACCATALYST
            return await WriteExportToDirectoryAsync(GetDownloadsPath(), json);
#elif ANDROID
            if (!OperatingSystem.IsAndroidVersionAtLeast(29))
            {
                return await WriteExportToDirectoryAsync(GetDownloadsPath(), json);
            }

            var resolver = Android.App.Application.Context.ContentResolver
                ?? throw new InvalidOperationException("Android content resolver is unavailable.");

            var values = new Android.Content.ContentValues();
            values.Put(Android.Provider.MediaStore.IMediaColumns.DisplayName, "progress.json");
            values.Put(Android.Provider.MediaStore.IMediaColumns.MimeType, "application/json");
            values.Put(
                Android.Provider.MediaStore.IMediaColumns.RelativePath,
                Android.OS.Environment.DirectoryDownloads);

            var collectionUri = Android.Provider.MediaStore.Downloads.ExternalContentUri
                ?? throw new InvalidOperationException("Android Downloads storage is unavailable.");
            var itemUri = resolver.Insert(collectionUri, values)
                ?? throw new InvalidOperationException("Could not create progress.json in Downloads.");

            await using var stream = resolver.OpenOutputStream(itemUri)
                ?? throw new InvalidOperationException("Could not open progress.json for writing.");
            await using var writer = new StreamWriter(stream);
            await writer.WriteAsync(json);

            return itemUri.ToString() ?? "progress.json";
#else
            return await WriteExportToDirectoryAsync(GetDownloadsPath(), json);
#endif
        }

        private async Task DeleteOldProgressExportsAsync(DateTimeOffset oldestAllowed)
        {
#if ANDROID
            if (!OperatingSystem.IsAndroidVersionAtLeast(29))
            {
                await DeleteOldProgressExportsFromDirectoryAsync(oldestAllowed);
                return;
            }

            var resolver = Android.App.Application.Context.ContentResolver;
            if (resolver is null)
            {
                return;
            }

            var collectionUri = Android.Provider.MediaStore.Downloads.ExternalContentUri;
            if (collectionUri is null)
            {
                return;
            }

            var projection = new string[]
            {
                "_id",
                Android.Provider.MediaStore.IMediaColumns.DisplayName,
                Android.Provider.MediaStore.IMediaColumns.DateModified,
            };

            using var cursor = resolver.Query(collectionUri, projection, null, null, null);
            if (cursor is null)
            {
                return;
            }

            var idColumn = cursor.GetColumnIndexOrThrow("_id");
            var nameColumn = cursor.GetColumnIndexOrThrow(Android.Provider.MediaStore.IMediaColumns.DisplayName);
            var modifiedColumn = cursor.GetColumnIndexOrThrow(Android.Provider.MediaStore.IMediaColumns.DateModified);

            while (cursor.MoveToNext())
            {
                var fileName = cursor.GetString(nameColumn);
                if (!IsProgressExportFileName(fileName))
                {
                    continue;
                }

                var modifiedSeconds = cursor.GetLong(modifiedColumn);
                var modifiedAt = DateTimeOffset.FromUnixTimeSeconds(modifiedSeconds);
                if (modifiedAt >= oldestAllowed)
                {
                    continue;
                }

                var id = cursor.GetLong(idColumn);
                var itemUri = Android.Content.ContentUris.WithAppendedId(collectionUri, id);
                try
                {
                    resolver.Delete(itemUri, null, null);
                }
                catch
                {
                    // Cleanup is best-effort; a protected file should not block export.
                }
            }

            await Task.CompletedTask;
#else
            await DeleteOldProgressExportsFromDirectoryAsync(oldestAllowed);
#endif
        }

        private async Task DeleteOldProgressExportsFromDirectoryAsync(DateTimeOffset oldestAllowed)
        {
            var downloadsPath = GetDownloadsPath();
            if (!Directory.Exists(downloadsPath))
            {
                return;
            }

            foreach (var filePath in Directory.EnumerateFiles(downloadsPath, ProgressExportSearchPattern))
            {
                var fileName = Path.GetFileName(filePath);
                if (!IsProgressExportFileName(fileName))
                {
                    continue;
                }

                var modifiedAt = File.GetLastWriteTime(filePath);
                if (modifiedAt >= oldestAllowed.LocalDateTime)
                {
                    continue;
                }

                try
                {
                    File.Delete(filePath);
                }
                catch
                {
                    // Cleanup is best-effort; a locked file should not block export.
                }
            }

            await Task.CompletedTask;
        }

        private static string GetDownloadsPath()
        {
#if WINDOWS
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads");
#elif MACCATALYST
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads");
#elif ANDROID
            return Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads)?.AbsolutePath
                ?? FileSystem.AppDataDirectory;
#else
            return FileSystem.AppDataDirectory;
#endif
        }

        private static async Task<string> WriteExportToDirectoryAsync(string downloadsPath, string json)
        {
            Directory.CreateDirectory(downloadsPath);

            var exportPath = Path.Combine(downloadsPath, "progress.json");
            await File.WriteAllTextAsync(exportPath, json);
            return exportPath;
        }

        private static bool IsProgressExportFileName(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            if (fileName.Equals("progress.json", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return fileName.StartsWith("progress (", StringComparison.OrdinalIgnoreCase)
                && fileName.EndsWith(").json", StringComparison.OrdinalIgnoreCase);
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
