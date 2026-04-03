using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KoreanFlashCardApp.Controls;
using KoreanFlashCardApp.Helpers;
using KoreanFlashCardApp.Models;
using System.Collections.ObjectModel;

namespace KoreanFlashCardApp.ViewModels
{
    public partial class StudySessionPageViewModel : BasePageViewModel
    {
        private readonly IWordProvider _wordProvider;
        private readonly ProgressProvider _progressProvider;
        private readonly FlashCardProvider _flashCardProvider;
        private readonly List<FlashCard> _sessionCards = new();

        public StudySessionPageViewModel(
            IWordProvider wordProvider,
            ProgressProvider progressProvider,
            FlashCardProvider flashCardProvider)
        {
            _wordProvider = wordProvider;
            _progressProvider = progressProvider;
            _flashCardProvider = flashCardProvider;
        }

        public ObservableCollection<FlashCardOption> Options { get; } = new();

        [ObservableProperty]
        private int moduleNumber;

        [ObservableProperty]
        private bool isStudyAllMode;

        [ObservableProperty]
        private string sessionTitle = "Study session";

        [ObservableProperty]
        private string sessionSubtitle = string.Empty;

        [ObservableProperty]
        private FlashCard? currentCard;

        [ObservableProperty]
        private string feedbackTitle = string.Empty;

        [ObservableProperty]
        private string feedbackBody = string.Empty;

        [ObservableProperty]
        private bool isAnswerRevealed;

        [ObservableProperty]
        private bool isSessionComplete;

        [ObservableProperty]
        private int currentCardIndex = -1;

        [ObservableProperty]
        private int correctAnswers;

        [ObservableProperty]
        private int totalCards;

        public string ModuleBadge => IsStudyAllMode
            ? "Study All"
            : ModuleNumber > 0 ? $"Module {ModuleNumber:00}" : string.Empty;

        public string CounterLabel => TotalCards == 0
            ? "0 / 0"
            : $"{Math.Min(CurrentCardIndex + 1, TotalCards)} / {TotalCards}";

        public string WordDefinitionText => CurrentCard is null
            ? string.Empty
            : IsAnswerRevealed
                ? CurrentCard.TargetWord.PrimaryDefinition
                : "Recall the meaning before you reveal the answer.";

        public string WordTypeLabel => CurrentCard?.TargetWord.WordTypeDisplay ?? string.Empty;

        public string ProgressSummary => $"{CorrectAnswers} correct out of {TotalCards}";

        public string CompletionTitle => TotalCards == 0
            ? IsStudyAllMode ? "Nothing due today" : "No cards in this module"
            : IsStudyAllMode ? "Study All complete" : "Module complete";

        public string CompletionSummary => TotalCards == 0
            ? IsStudyAllMode
                ? "Come back after more review items become due."
                : "This module does not have any cards yet."
            : ProgressSummary;

        public string NextButtonText => CurrentCardIndex >= TotalCards - 1 ? "Finish module" : "Next word";

        public bool HasCard => CurrentCard is not null && !IsSessionComplete;

        public bool ShowFeedback => IsAnswerRevealed && !IsSessionComplete;

        public void LoadModule(int moduleNumber)
        {
            ModuleNumber = moduleNumber;
            IsStudyAllMode = false;
            CorrectAnswers = 0;
            IsSessionComplete = false;
            IsAnswerRevealed = false;
            FeedbackTitle = string.Empty;
            FeedbackBody = string.Empty;

            var moduleWords = _flashCardProvider.GetModuleWords(_wordProvider.Words, moduleNumber);
            _sessionCards.Clear();
            _sessionCards.AddRange(_flashCardProvider.BuildSession(moduleWords, _wordProvider.Words, _progressProvider.WordProgress));

            TotalCards = _sessionCards.Count;
            SessionTitle = $"Module {moduleNumber:00}";
            SessionSubtitle = moduleWords.Count == 0
                ? "No words available."
                : $"{moduleWords.Count}-word block from {moduleWords.First().Word_Name} to {moduleWords.Last().Word_Name}.";

            if (_sessionCards.Count == 0)
            {
                CurrentCardIndex = -1;
                CurrentCard = null;
                IsSessionComplete = true;
                return;
            }

            CurrentCardIndex = 0;
            LoadCurrentCard();
        }

        public void LoadDueToday()
        {
            ModuleNumber = 0;
            IsStudyAllMode = true;
            CorrectAnswers = 0;
            IsSessionComplete = false;
            IsAnswerRevealed = false;
            FeedbackTitle = string.Empty;
            FeedbackBody = string.Empty;

            var dueTodayWords = _flashCardProvider.GetDueTodayWords(_wordProvider.Words, _progressProvider.WordProgress);
            _sessionCards.Clear();
            _sessionCards.AddRange(_flashCardProvider.BuildSession(dueTodayWords, _wordProvider.Words, _progressProvider.WordProgress));

            TotalCards = _sessionCards.Count;
            SessionTitle = "Study All";
            SessionSubtitle = dueTodayWords.Count == 0
                ? "Nothing is due today."
                : $"{dueTodayWords.Count} due words pulled from all modules.";

            if (_sessionCards.Count == 0)
            {
                CurrentCardIndex = -1;
                CurrentCard = null;
                IsSessionComplete = true;
                NotifyComputedState();
                return;
            }

            CurrentCardIndex = 0;
            LoadCurrentCard();
        }

        [RelayCommand]
        private async Task SelectOptionAsync(FlashCardOption? option)
        {
            if (option is null || IsAnswerRevealed || CurrentCard is null)
            {
                return;
            }

            option.IsSelected = true;

            foreach (var candidate in Options)
            {
                candidate.IsRevealed = true;
                candidate.IsEnabled = false;
            }

            IsAnswerRevealed = true;

            if (option.IsCorrect)
            {
                CorrectAnswers++;
                FeedbackTitle = "Correct";
                FeedbackBody = $"{CurrentCard.TargetWord.Word_Name} means {CurrentCard.TargetWord.PrimaryDefinition}.";
            }
            else
            {
                var correctOption = Options.First(x => x.IsCorrect);
                FeedbackTitle = "Not quite";
                FeedbackBody = $"The correct answer was {correctOption.Text}.";
            }

            await _progressProvider.SaveProgressAsync(CurrentCard.TargetWord.Word_ID, option.IsCorrect);
            NotifyComputedState();
        }

        [RelayCommand]
        private Task NextAsync()
        {
            if (!IsAnswerRevealed)
            {
                return Task.CompletedTask;
            }

            if (CurrentCardIndex >= _sessionCards.Count - 1)
            {
                CurrentCard = null;
                IsSessionComplete = true;
                IsAnswerRevealed = false;
                FeedbackTitle = string.Empty;
                FeedbackBody = string.Empty;
                NotifyComputedState();
                return Task.CompletedTask;
            }

            CurrentCardIndex++;
            LoadCurrentCard();
            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task ReturnToModulesAsync()
        {
            return Shell.Current.GoToAsync("//MainPage");
        }

        private void LoadCurrentCard()
        {
            IsAnswerRevealed = false;
            FeedbackTitle = string.Empty;
            FeedbackBody = string.Empty;

            CurrentCard = _sessionCards[CurrentCardIndex];

            Options.Clear();
            foreach (var optionWord in CurrentCard.WordSelectionOptions)
            {
                Options.Add(new FlashCardOption(optionWord, CurrentCard.TargetWord.Word_ID));
            }

            NotifyComputedState();
        }

        partial void OnModuleNumberChanged(int value) => NotifyComputedState();

        partial void OnIsStudyAllModeChanged(bool value) => NotifyComputedState();

        partial void OnCurrentCardChanged(FlashCard? value) => NotifyComputedState();

        partial void OnCurrentCardIndexChanged(int value) => NotifyComputedState();

        partial void OnCorrectAnswersChanged(int value) => NotifyComputedState();

        partial void OnTotalCardsChanged(int value) => NotifyComputedState();

        partial void OnIsAnswerRevealedChanged(bool value) => NotifyComputedState();

        partial void OnIsSessionCompleteChanged(bool value) => NotifyComputedState();

        private void NotifyComputedState()
        {
            OnPropertyChanged(nameof(ModuleBadge));
            OnPropertyChanged(nameof(CounterLabel));
            OnPropertyChanged(nameof(WordDefinitionText));
            OnPropertyChanged(nameof(WordTypeLabel));
            OnPropertyChanged(nameof(ProgressSummary));
            OnPropertyChanged(nameof(CompletionTitle));
            OnPropertyChanged(nameof(CompletionSummary));
            OnPropertyChanged(nameof(NextButtonText));
            OnPropertyChanged(nameof(HasCard));
            OnPropertyChanged(nameof(ShowFeedback));
        }
    }
}
