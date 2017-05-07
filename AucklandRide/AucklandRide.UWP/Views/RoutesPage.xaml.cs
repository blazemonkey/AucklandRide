using AucklandRide.UWP.Models;
using AucklandRide.UWP.Models.AT;
using AucklandRide.UWP.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AucklandRide.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RoutesPage : Page, INotifyPropertyChanged
    {
        private ActiveTrip ActiveTrips { get; set; }

        private Route _lastRoute;
        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public List<Route> Routes { get; set; }

        public RoutesPage()
        {
            this.InitializeComponent();

            Observable.FromEventPattern<TextChangedEventArgs>(FilterTextBox, nameof(FilterTextBox.TextChanged))
                                                            .Select(x => ((TextBox)x.Sender).Text.ToLower())
                                                            .DistinctUntilChanged()
                                                            .Throttle(TimeSpan.FromSeconds(.5))
                                                            .ObserveOn(SynchronizationContext.Current)
                                                            .Subscribe((x) =>
                                                            {
                                                                CollectionViewSource.Source = Routes.Where(z => z.ShortName.ToLower().Contains(x) ||
                                                                                                          z.LongName.ToLower().Contains(x));
                                                            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            IsLoading = true;
            if (CollectionViewSource.Source == null)
            {                
                Routes = await RestService.GetRoutes();
                CollectionViewSource.Source = Routes;
                
            }

            await UpdateForVisualState(AdaptiveStates.CurrentState);

            // Don't play a content transition for first item load.
            // Sometimes, this content will be animated as part of the page transition.
            DisableContentTransitions();

            ActiveTrips = await RestService.GetActiveTrips();

            var favRoutes = new List<Route>();
            var localSettings = ApplicationData.Current.LocalSettings;
            foreach (var ls in localSettings.Values)
            {
                if (ls.Key.StartsWith("r") && (bool)ls.Value == true)
                {
                    var id = ls.Key.Substring(1);
                    var route = Routes.FirstOrDefault(x => x.ShortId == id);
                    route.IsLive = ActiveTrips.Response.Entity.Any(x => x.TripUpdate.Trip.RouteId == route.Id);
                    if (route != null)
                    {
                        favRoutes.Add(route);
                    }
                }
            }

            RoutesFavList.ItemsSource = favRoutes.OrderBy(x => x.ShortName);
            IsLoading = false;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void AdaptiveStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            await UpdateForVisualState(e.NewState, e.OldState);
        }

        private async Task UpdateForVisualState(VisualState newState, VisualState oldState = null)
        {
            var isNarrow = newState == NarrowState;

            if (isNarrow && oldState == MediumState && _lastRoute != null)
            {
                _lastRoute = await RestService.GetRouteById(_lastRoute.Id);
                // Resize down to the detail item. Don't play a transition.
                Frame.Navigate(typeof(RoutesDetailPage), _lastRoute, new SuppressNavigationTransitionInfo());
            }

            if ((oldState == NarrowState || oldState == null) && newState == MediumState && RoutesList.SelectedItem == null)
                RoutesList.SelectedItem = _lastRoute;

            EntranceNavigationTransitionInfo.SetIsTargetElement(RoutesList, isNarrow);
            if (DetailContentPresenter != null)
            {
                EntranceNavigationTransitionInfo.SetIsTargetElement(DetailContentPresenter, !isNarrow);
            }
        }

        private async void RoutesList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = (Route)e.ClickedItem;
            _lastRoute = clickedItem;

            if (AdaptiveStates.CurrentState == NarrowState)
            {
                _lastRoute = await RestService.GetRouteById(_lastRoute.Id);
                _lastRoute.ActiveTrips = ActiveTrips.Response.Entity.Where(x => x.TripUpdate.Trip.RouteId == _lastRoute.Id).ToList();
                Frame.Navigate(typeof(RoutesDetailPage), _lastRoute, new DrillInNavigationTransitionInfo());
            }
            else
            {
                // Play a refresh animation when the user switches detail items.
                EnableContentTransitions();
            }
        }

        private void EnableContentTransitions()
        {
            DetailContentPresenter.ContentTransitions.Clear();
            DetailContentPresenter.ContentTransitions.Add(new EntranceThemeTransition());
        }

        private void DisableContentTransitions()
        {
            if (DetailContentPresenter != null)
            {
                DetailContentPresenter.ContentTransitions.Clear();
            }
        }
    }
}
