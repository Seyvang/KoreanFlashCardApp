using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Graphics;

namespace KoreanFlashCardApp.Models
{
    public partial class FlashCardOption : ObservableObject
    {
        public FlashCardOption(Word optionWord, int correctWordId)
        {
            OptionWord = optionWord;
            CorrectWordId = correctWordId;
        }

        public Word OptionWord { get; }

        public int CorrectWordId { get; }

        public string Text => OptionWord.PrimaryDefinition;

        public bool IsCorrect => OptionWord.Word_ID == CorrectWordId;

        [ObservableProperty]
        private bool isSelected;

        [ObservableProperty]
        private bool isRevealed;

        [ObservableProperty]
        private bool isEnabled = true;

        public Color BackgroundColor
        {
            get
            {
                if (!IsRevealed)
                {
                    return Color.FromArgb("#F5F1EA");
                }

                if (IsCorrect)
                {
                    return Color.FromArgb("#D9F2E4");
                }

                if (IsSelected)
                {
                    return Color.FromArgb("#F5D8D2");
                }

                return Color.FromArgb("#ECE7DE");
            }
        }

        public Color BorderColor
        {
            get
            {
                if (!IsRevealed)
                {
                    return IsSelected ? Color.FromArgb("#1D4D4F") : Color.FromArgb("#CDBEA7");
                }

                if (IsCorrect)
                {
                    return Color.FromArgb("#2C7A58");
                }

                if (IsSelected)
                {
                    return Color.FromArgb("#9E3B2E");
                }

                return Color.FromArgb("#D4C8B6");
            }
        }

        public Color TextColor => Color.FromArgb("#1E1B18");

        partial void OnIsSelectedChanged(bool value) => NotifyVisualState();

        partial void OnIsRevealedChanged(bool value) => NotifyVisualState();

        partial void OnIsEnabledChanged(bool value) => NotifyVisualState();

        private void NotifyVisualState()
        {
            OnPropertyChanged(nameof(BackgroundColor));
            OnPropertyChanged(nameof(BorderColor));
            OnPropertyChanged(nameof(TextColor));
        }
    }
}
