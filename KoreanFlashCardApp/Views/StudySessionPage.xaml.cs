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
