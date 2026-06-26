namespace KoreanFlashCardApp.Models
{
    public class StudyWord
    {
        public StudyWord(Word word, WordProgress? progress)
        {
            Word = word;
            Progress = progress;
        }

        public Word Word { get; }

        public WordProgress? Progress { get; }

        public bool IsSkipped => Progress?.SkipWord == true;

        public bool IsStudied => Progress is not null && !IsSkipped;

        public bool IsDue =>
            Progress is not null &&
            !IsSkipped &&
            Progress.Next_Test_Date.Date <= DateTime.Today;
    }
}
