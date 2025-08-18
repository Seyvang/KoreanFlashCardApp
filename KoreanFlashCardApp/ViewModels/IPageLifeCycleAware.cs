namespace KoreanFlashCardApp.ViewModels
{
    public interface IPageLifeCycleAware
    {
        public void OnAppearing();
        public void OnFirstAppearing();
        public void OnDisappearing();
        public void OnNavigatedTo();
        public void OnNavigatedFrom();
    }
}