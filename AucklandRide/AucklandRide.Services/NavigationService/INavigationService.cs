namespace Services.NavigationService
{
    public interface INavigationService
    {
        bool Navigate(string path, object param = null);
        void GoBack();
        bool CanGoBack { get; }
        void ClearHistory();
        void RemoveLastPage();
    }
}
