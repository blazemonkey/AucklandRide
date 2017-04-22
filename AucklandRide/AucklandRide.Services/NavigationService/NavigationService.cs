namespace Services.NavigationService
{
    public class NavigationService : INavigationService
    {
        public Prism.Windows.Navigation.INavigationService _navigationService { get; private set; }

        public NavigationService(Prism.Windows.Navigation.INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public bool Navigate(string path, object param = null)
        {
            return _navigationService.Navigate(path, param);
        }

        public void ClearHistory()
        {
            _navigationService.ClearHistory();
        }

        public bool CanGoBack { get { return _navigationService.CanGoBack(); } }

        public void GoBack()
        {
            if (_navigationService.CanGoBack())
                _navigationService.GoBack();
        }

        public void RemoveLastPage()
        {
            if (_navigationService.CanGoBack())
                _navigationService.RemoveLastPage();
        }
    }
}
