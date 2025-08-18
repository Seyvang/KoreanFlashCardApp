using KoreanFlashCardApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KoreanFlashCardApp.Helpers
{
    public class ProgressProvider
    {
        private string progressFileName = "progress.json";
        public List<WordProgress> wordProgress;
        public async Task SaveProgressAsync(int word_ID)
        {
            var matchedWord = wordProgress.FirstOrDefault(x => x.Word_ID == word_ID);
            if (matchedWord != null)
            {
                matchedWord.Number_Correct += 1;
                matchedWord.Next_Test_Date = HandleDateCalculation(matchedWord.Number_Correct);
            }
            else
            {
                // assume this is a new word and start progress
                wordProgress.Add(new WordProgress(0, 0, word_ID, HandleDateCalculation(1), 1));
            }

        }

        public async Task SaveOnDisappearing()
        {
            var json = JsonSerializer.Serialize(wordProgress);

            // Get the file path for storing the data
            var filePath = Path.Combine(FileSystem.AppDataDirectory, progressFileName);

            // Write the JSON data to the file
            await File.WriteAllTextAsync(filePath, json);
        }

        public ProgressProvider()
        {
            
        }

        public async Task LoadProgressAsync()
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, progressFileName);

            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath);

                // Deserialize the JSON into a list of WordProgress objects
                wordProgress = JsonSerializer.Deserialize<List<WordProgress>>(json) ?? new List<WordProgress>();
            }

            wordProgress = new List<WordProgress>();
        }

        private DateTime HandleDateCalculation(int numberCorrect)
        {
            var dayMultiplier = Math.Pow(2, numberCorrect - 1);

            return DateTime.Today.AddDays(dayMultiplier);
        }
    }
}
