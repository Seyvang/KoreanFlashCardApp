using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
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
                    return GetThemeColor("AppSurfaceRaised");
                }

                if (IsCorrect)
                {
                    return GetThemeColor("AppSuccessSoft");
                }

                if (IsSelected)
                {
                    return GetThemeColor("AppDangerSoft");
                }

                return GetThemeColor("AppSurfaceDisabled");
            }
        }

        public Color BorderColor
        {
            get
            {
                if (!IsRevealed)
                {
                    return IsSelected ? GetThemeColor("AppPrimary") : GetThemeColor("AppBorderMuted");
                }

                if (IsCorrect)
                {
                    return GetThemeColor("AppSuccess");
                }

                if (IsSelected)
                {
                    return GetThemeColor("AppDanger");
                }

                return GetThemeColor("AppBorderDisabled");
            }
        }

        public Color TextColor => GetThemeColor("AppInk");

        partial void OnIsSelectedChanged(bool value) => NotifyVisualState();

        partial void OnIsRevealedChanged(bool value) => NotifyVisualState();

        partial void OnIsEnabledChanged(bool value) => NotifyVisualState();

        private void NotifyVisualState()
        {
            OnPropertyChanged(nameof(BackgroundColor));
            OnPropertyChanged(nameof(BorderColor));
            OnPropertyChanged(nameof(TextColor));
        }

        private static Color GetThemeColor(string key)
        {
            if (Application.Current?.Resources.TryGetValue(key, out var value) == true &&
                value is Color color)
            {
                return color;
            }

            return Colors.Transparent;
        }
    }
}
