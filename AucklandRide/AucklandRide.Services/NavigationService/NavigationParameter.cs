
namespace Services.NavigationService
{
    public sealed class NavigationParameters
    {
        static readonly NavigationParameters _instance = new NavigationParameters();

        public static NavigationParameters Instance
        {
            get { return _instance; }
        }

        private object _parameter;

        public void SetParameters(object parameter)
        {
            _parameter = parameter;
        }

        public object GetParameters()
        {
            return _parameter;
        }
    }
}
