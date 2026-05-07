using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KoreanFlashCardApp.Controls;
using KoreanFlashCardApp.Helpers;
using KoreanFlashCardApp.Models;
using System.Collections.ObjectModel;

namespace KoreanFlashCardApp.ViewModels
{
    public partial class MainPageViewModel : BasePageViewModel
    {
        private readonly IWordProvider _wordProvider;
        private readonly ProgressProvider _progressProvider;
        private readonly FlashCardProvider _flashCardProvider;
        private Task? _initializationTask;
        private bool _isInitialized;

        public MainPageViewModel(
            IWordProvider wordProvider,
            ProgressProvider progressProvider,
            FlashCardProvider flashCardProvider)
        {
            _wordProvider = wordProvider;
            _progressProvider = progressProvider;
            _flashCardProvider = flashCardProvider;
        }

        public ObservableCollection<StudyModule> Modules { get; } = new();

        [ObservableProperty]
        private string title = "Korean study modules";

        [ObservableProperty]
        private string subtitle = "Fifteen-word blocks with due cards surfaced first.";

        [ObservableProperty]
        private string moduleSummary = string.Empty;

        [ObservableProperty]
        private string studyAllSummary = string.Empty;

        [ObservableProperty]
        private bool hasDueToday;

        [ObservableProperty]
        private bool isLoading = true;

        [ObservableProperty]
        private string loadingMessage = "Loading study progress...";

        [ObservableProperty]
        private string loadError = string.Empty;

        [ObservableProperty]
        private string progressTransferStatus = string.Empty;

        [ObservableProperty]
        private bool isTransferringProgress;

        public bool HasModules => Modules.Count > 0;

        public bool HasLoadError => !string.IsNullOrWhiteSpace(LoadError);

        public bool HasProgressTransferStatus => !string.IsNullOrWhiteSpace(ProgressTransferStatus);

        public bool CanStartStudying => !IsLoading && !HasLoadError;

        public bool CanTransferProgress => !IsLoading && !IsTransferringProgress;

        public override void OnAppearing()
        {
            if (_isInitialized)
            {
                RefreshModules();
                return;
            }

            _initializationTask ??= InitializeAsync();
        }

        [RelayCommand(CanExecute = nameof(CanStartStudying))]
        private Task StartModuleAsync(int moduleNumber)
        {
            return Shell.Current.GoToAsync($"{nameof(StudySessionPage)}?moduleNumber={moduleNumber}");
        }

        [RelayCommand(CanExecute = nameof(CanStartStudying))]
        private Task StudyAllAsync()
        {
            return Shell.Current.GoToAsync($"{nameof(StudySessionPage)}?studyMode=due");
        }

        [RelayCommand(CanExecute = nameof(CanTransferProgress))]
        private async Task ExportProgressAsync()
        {
            try
            {
                IsTransferringProgress = true;
                ProgressTransferStatus = "Exporting progress...";

                var exportPath = await _progressProvider.ExportProgressToDownloadsAsync();
                ProgressTransferStatus = $"Exported progress.json to {exportPath}";
            }
            catch (Exception ex)
            {
                ProgressTransferStatus = $"Export failed: {ex.Message}";
            }
            finally
            {
                IsTransferringProgress = false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanTransferProgress))]
        private async Task ImportProgressAsync()
        {
            try
            {
                IsTransferringProgress = true;
                ProgressTransferStatus = "Choose a progress.json file to import.";

                var fileResult = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Import progress.json",
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.WinUI, new[] { ".json" } },
                        { DevicePlatform.macOS, new[] { "json" } },
                        { DevicePlatform.iOS, new[] { "public.json" } },
                        { DevicePlatform.Android, new[] { "application/json", "text/json" } },
                    }),
                });

                if (fileResult is null)
                {
                    ProgressTransferStatus = "Import canceled.";
                    return;
                }

                await _progressProvider.ImportProgressAsync(fileResult);
                RefreshModules();
                ProgressTransferStatus = $"Imported {fileResult.FileName}.";
            }
            catch (Exception ex)
            {
                ProgressTransferStatus = $"Import failed: {ex.Message}";
            }
            finally
            {
                IsTransferringProgress = false;
            }
        }

        [RelayCommand]
        private Task RetryAsync()
        {
            _isInitialized = false;
            _initializationTask = null;
            return InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                IsLoading = true;
                LoadError = string.Empty;
                LoadingMessage = "Loading study progress...";

                await _progressProvider.LoadProgressAsync();

                _isInitialized = true;
                RefreshModules();
            }
            catch (Exception ex)
            {
                LoadError = $"Progress failed to load: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
                _initializationTask = null;
                NotifyStateChanged();
            }
        }

        private void RefreshModules()
        {
            Modules.Clear();

            foreach (var module in _flashCardProvider.BuildModules(_wordProvider.Words))
            {
                Modules.Add(module);
            }

            ModuleSummary = $"{Modules.Count} modules across {_wordProvider.Words.Count} words";
            var dueTodayCount = _flashCardProvider.GetDueTodayCount(_wordProvider.Words);
            HasDueToday = dueTodayCount > 0;
            StudyAllSummary = dueTodayCount == 0
                ? "No words are due today."
                : $"{dueTodayCount} words are due today across all modules.";
            NotifyStateChanged();
        }

        partial void OnLoadErrorChanged(string value) => NotifyStateChanged();

        partial void OnIsLoadingChanged(bool value) => NotifyStateChanged();

        partial void OnProgressTransferStatusChanged(string value) => NotifyStateChanged();

        partial void OnIsTransferringProgressChanged(bool value) => NotifyStateChanged();

        private void NotifyStateChanged()
        {
            OnPropertyChanged(nameof(HasModules));
            OnPropertyChanged(nameof(HasLoadError));
            OnPropertyChanged(nameof(HasProgressTransferStatus));
            OnPropertyChanged(nameof(CanStartStudying));
            OnPropertyChanged(nameof(CanTransferProgress));
            StartModuleCommand.NotifyCanExecuteChanged();
            StudyAllCommand.NotifyCanExecuteChanged();
            ExportProgressCommand.NotifyCanExecuteChanged();
            ImportProgressCommand.NotifyCanExecuteChanged();
        }
    }
}
