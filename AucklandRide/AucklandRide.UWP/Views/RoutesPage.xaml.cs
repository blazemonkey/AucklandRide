using AucklandRide.UWP.Controls.UserControls;
using AucklandRide.UWP.Helpers;
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
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
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
        private MapControl _mapControl;
        private ActiveTrip ActiveTrips { get; set; }

        private Route _selectedRoute;
        private CombinedData _combinedData;
        private bool _isLoading;
        private bool _isLoadingRoute;
        private bool? _showStops;
        private Models.AT.TripUpdate _tripUpdate;
        private Models.AT.ActiveVehicle _vehicle;

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoadingRoute
        {
            get { return _isLoadingRoute; }
            set
            {
                _isLoadingRoute = value;
                OnPropertyChanged();
            }
        }

        public CombinedData CombinedData
        {
            get { return _combinedData; }
            set { _combinedData = value;
                OnPropertyChanged();
            }
        }
        public Route SelectedRoute
        {
            get { return _selectedRoute; }
            set { _selectedRoute = value;
                OnPropertyChanged();
                CombinedData.Route = value;
            }
        }

        public bool? ShowStops
        {
            get { return _showStops; }
            set
            {
                _showStops = value;
                OnPropertyChanged();
            }
        }

        public Models.AT.TripUpdate TripUpdate
        {
            get { return _tripUpdate; }
            set
            {
                _tripUpdate = value;
                OnPropertyChanged();
            }
        }

        public List<Route> Routes { get; set; }
        public List<Shape> Shapes { get; set; }
        public List<BasicGeoposition> Positions { get; set; }

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

            CombinedData = new CombinedData();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            IsLoading = true;
            ActiveTrips = await RestService.GetActiveTrips();

            if (CollectionViewSource.Source == null)
            {                
                Routes = await RestService.GetRoutes();

                if ((ActiveTrips != null) && (ActiveTrips.Response != null))
                {
                    foreach (var r in Routes)
                        r.IsLive = ActiveTrips.Response.Entity.Any(x => x.TripUpdate.Trip.RouteId == r.Id);
                }

                CollectionViewSource.Source = Routes;
                if (Routes.Any())
                {
                    SelectedRoute = Routes.First();
                    await GetRouteDetails();
                }
            }

            await UpdateForVisualState(AdaptiveStates.CurrentState);

            // Don't play a content transition for first item load.
            // Sometimes, this content will be animated as part of the page transition.
            DisableContentTransitions();            

            var favRoutes = new List<Route>();
            var localSettings = ApplicationData.Current.LocalSettings;
            foreach (var ls in localSettings.Values)
            {
                if (ls.Key.StartsWith("r") && (bool)ls.Value == true)
                {
                    var id = ls.Key.Substring(1);
                    var route = Routes.FirstOrDefault(x => x.ShortId == id);
                    if (route != null)
                        favRoutes.Add(route);
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
            await GetRouteDetails();

            if (isNarrow && oldState == MediumState && SelectedRoute != null)
            {
                // Resize down to the detail item. Don't play a transition.
                Frame.Navigate(typeof(RoutesDetailPage), SelectedRoute, new SuppressNavigationTransitionInfo());
            }

            if ((oldState == NarrowState || oldState == null) && newState == MediumState && RoutesList.SelectedItem == null)
                RoutesList.SelectedItem = SelectedRoute;

            EntranceNavigationTransitionInfo.SetIsTargetElement(RoutesList, isNarrow);
            if (DetailContentPresenter != null)
            {
                EntranceNavigationTransitionInfo.SetIsTargetElement(DetailContentPresenter, !isNarrow);
            }
        }

        private async void RoutesList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = (Route)e.ClickedItem;
            SelectedRoute = clickedItem;
            await GetRouteDetails();

            if (AdaptiveStates.CurrentState == NarrowState)
            {
                Frame.Navigate(typeof(RoutesDetailPage), SelectedRoute, new DrillInNavigationTransitionInfo());
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

        private async Task GetRouteDetails()
        {
            SelectedRoute = await RestService.GetRouteById(SelectedRoute.Id);
            SelectedRoute.ActiveTrips = ActiveTrips.Response.Entity.Where(x => x.TripUpdate.Trip.RouteId == SelectedRoute.Id).ToList();

            AppBarFavorite.IsChecked = IsRouteFavourite();

            foreach (var t in SelectedRoute.Trips)
            {
                if (SelectedRoute.ActiveTrips == null)
                    continue;

                if (SelectedRoute.ActiveTrips.Select(x => x.TripUpdate.Trip.TripId).Contains(t.Id))
                {
                    t.IsLive = true;
                    t.Live = SelectedRoute.ActiveTrips.FirstOrDefault(x => x.TripUpdate.Trip.TripId == t.Id);
                }
            }
        
            if (SelectedRoute.Trips.Any())
            {
                if (SelectedRoute.Trips.Any(x => x.IsLive))
                    CombinedData.SelectedTrip = SelectedRoute.Trips.First(x => x.IsLive);
                else
                    CombinedData.SelectedTrip = SelectedRoute.Trips.FirstOrDefault();
            }
        }

        private async void TripsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsLoadingRoute = true;
            var trip = (Trip)((ComboBox)sender).SelectedItem;
            if (trip == null)
            {
                _mapControl.Children.Clear();
                _mapControl.MapElements.Clear();
                IsLoadingRoute = false;
                return;
            }

            ShowStops = true;

            var stopTimes = await RestService.GetStopTimesByTripId(trip.Id);
            var shapes = await RestService.GetShapesById(trip.ShapeId);
            var calendar = await RestService.GetCalendarByServiceId(trip.ServiceId);

            CombinedData.StopTimes = stopTimes;
            CombinedData.Calendar = calendar;
            Shapes = shapes;
            Positions = new List<BasicGeoposition>();
            DrawStopsOnMap();

            _mapControl.Center = new Geopoint(Positions.First());
            _mapControl.ZoomLevel = 16;
            MapHelper.DrawShapes(_mapControl, Shapes.OrderBy(x => x.Sequence));

            await GetVehiclePosition();

            IsLoadingRoute = false;
        }

        private void DrawStopsOnMap()
        {
            var count = 0;
            foreach (var st in CombinedData.StopTimes)
            {
                count++;
                var position = new BasicGeoposition() { Latitude = (double)st.StopLatitude, Longitude = (double)st.StopLongitude };
                var quickStopControl = new QuickStopTime(st);
                MapHelper.DrawPointOnMap(_mapControl, position.Latitude, position.Longitude, st.StopId, (count == 1 ? Colors.Green : (count == CombinedData.StopTimes.Count() ? Colors.Red : Colors.Black)), quickStopControl);
                Positions.Add(position);
            }
        }

        private void RoutesMapControl_Loaded(object sender, RoutedEventArgs e)
        {
            _mapControl = (MapControl)sender;
        }

        private bool IsRouteFavourite()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            var route = localSettings.Values["r" + CombinedData.Route.ShortId];
            if (route == null)
                return false;

            return (bool)route;
        }

        private async Task GetVehiclePosition()
        {
            if (CombinedData.SelectedTrip.Live != null)
            {
                TripUpdate = CombinedData.SelectedTrip.Live.TripUpdate;
                TripUpdate.StopTimeUpdate.StopName = CombinedData.StopTimes.FirstOrDefault(x => x.StopId == TripUpdate.StopTimeUpdate.StopId).StopName;
                TripUpdate.StopTimeUpdate.StopRegionName = CombinedData.StopTimes.FirstOrDefault(x => x.StopId == TripUpdate.StopTimeUpdate.StopId).StopRegionName;
                var vehicle = await RestService.GetActiveVehicleByVehicleId(TripUpdate.Vehicle.Id);
                if (vehicle.Error == null)
                {
                    var latitude = vehicle.Response.Entity.FirstOrDefault().Vehicle.Position.Latitude;
                    var longitude = vehicle.Response.Entity.FirstOrDefault().Vehicle.Position.Longitude;
                    MapHelper.DrawBusOnMap(_mapControl, latitude, longitude);
                    var diffsList = new List<Tuple<int, double>>();
                    foreach (var s in Shapes)
                    {
                        var diffLat = Math.Abs((double)s.Latitude - latitude);
                        var diffLong = Math.Abs((double)s.Longitude - longitude);
                        var diffTotal = diffLat + diffLong;
                        diffsList.Add(new Tuple<int, double>(s.Sequence, diffTotal));
                    }

                    var shapeSeq = diffsList.OrderByDescending(x => x.Item2).ToList().First();
                    var stops = from s in Shapes
                               join st in CombinedData.StopTimes
                               on new { s.Latitude, s.Longitude } equals new { Latitude = st.StopLatitude, Longitude = st.StopLongitude }
                               select new { st.StopId, st.StopSequence, st.StopLatitude, st.StopLongitude };
                    var stopsList = stops.GroupBy(x => x.StopId).Distinct().Select(x => x.First()).ToList();
                }                

                _vehicle = vehicle;
                AppBarCenterLiveLocation.IsEnabled = true;
            }
            else
            {
                AppBarCenterLiveLocation.IsEnabled = false;                
            }
        }

        private void AppBarFavorite_Click(object sender, RoutedEventArgs e)
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["r" + CombinedData.Route.ShortId] = AppBarFavorite.IsChecked;
        }

        private void AppBarShowStops_Click(object sender, RoutedEventArgs e)
        {
            var grids = _mapControl.Children.Where(x => x.GetType() == typeof(Grid)).ToList();
            foreach (var c in grids)
            {
                var point = (Grid)c;
                if (point.Name == "Point")
                    _mapControl.Children.Remove(point);
            }

            if (ShowStops.Value)
                DrawStopsOnMap();

            MapHelper.DrawShapes(_mapControl, Shapes.OrderBy(x => x.Sequence));
        }

        private void AppBarFitMap_Click(object sender, RoutedEventArgs e)
        {
            MapHelper.SetCenterOfPoints(_mapControl, Positions);
        }

        private void AppBarCenterLiveLocation_Click(object sender, RoutedEventArgs e)
        {
            if (CombinedData.SelectedTrip.Live != null)
            {
                _mapControl.Center = new Geopoint(new BasicGeoposition()
                {
                    Latitude = _vehicle.Response.Entity.FirstOrDefault().Vehicle.Position.Latitude,
                    Longitude = _vehicle.Response.Entity.FirstOrDefault().Vehicle.Position.Longitude
                });
            }
        }

        private async void AppBarRefresh_Click(object sender, RoutedEventArgs e)
        {
            IsLoadingRoute = true;
            await GetVehiclePosition();
            IsLoadingRoute = false;
        }

        private void CalendarView_CalendarViewDayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
            if (CombinedData == null || CombinedData.Calendar == null)
                return;

            if (args.Item.Date.DayOfWeek == DayOfWeek.Monday)
                args.Item.IsBlackout = !CombinedData.Calendar.Monday.Value;
            else if (args.Item.Date.DayOfWeek == DayOfWeek.Tuesday)
                args.Item.IsBlackout = !CombinedData.Calendar.Tuesday.Value;
            else if (args.Item.Date.DayOfWeek == DayOfWeek.Wednesday)
                args.Item.IsBlackout = !CombinedData.Calendar.Wednesday.Value;
            else if (args.Item.Date.DayOfWeek == DayOfWeek.Thursday)
                args.Item.IsBlackout = !CombinedData.Calendar.Thursday.Value;
            else if (args.Item.Date.DayOfWeek == DayOfWeek.Friday)
                args.Item.IsBlackout = !CombinedData.Calendar.Friday.Value;
            else if (args.Item.Date.DayOfWeek == DayOfWeek.Saturday)
                args.Item.IsBlackout = !CombinedData.Calendar.Saturday.Value;
            else
                args.Item.IsBlackout = !CombinedData.Calendar.Sunday.Value;

            if (CombinedData.Calendar.CalendarDates.Exists(x => x.Date == args.Item.Date))
                args.Item.IsBlackout = true;
        }
    }
}
