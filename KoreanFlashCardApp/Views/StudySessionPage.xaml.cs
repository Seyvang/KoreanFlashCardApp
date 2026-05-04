using KoreanFlashCardApp.Controls;
using KoreanFlashCardApp.ViewModels;

namespace KoreanFlashCardApp
{
    public partial class StudySessionPage : BasePage, IQueryAttributable
    {
        private readonly StudySessionPageViewModel _viewModel;

        public StudySessionPage(StudySessionPageViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _viewModel.ConfirmSkipWordAsync = ConfirmSkipWordAsync;
        }

        private Task<bool> ConfirmSkipWordAsync(Models.Word word)
        {
            return DisplayAlert(
                "Skip word?",
                $"Hide {word.Word_Name} from future study sessions?",
                "Skip",
                "Cancel");
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("studyMode", out var studyMode) &&
                string.Equals(studyMode?.ToString(), "due", StringComparison.OrdinalIgnoreCase))
            {
                _viewModel.LoadDueToday();
                return;
            }

            if (!query.TryGetValue("moduleNumber", out var rawValue))
            {
                return;
            }

            if (int.TryParse(rawValue?.ToString(), out var moduleNumber))
            {
                _viewModel.LoadModule(moduleNumber);
            }
        }
    }
}
