using AucklandRide.UWP.Controls.UserControls;
using AucklandRide.UWP.Helpers;
using AucklandRide.UWP.Models;
using AucklandRide.UWP.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.Storage;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AucklandRide.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RoutesDetailPage : Page, INotifyPropertyChanged
    {
        private Calendar _calendar;
        private bool? _showStops;
        private bool _isLoading;
        private Models.AT.TripUpdate _tripUpdate;
        private bool _isLive;
        private Models.AT.ActiveVehicle _vehicle;

        public Route Route { get; set; }
        public List<Trip> Trips { get; set; }
        public Trip SelectedTrip { get; set; }
        public List<StopTime> StopTimes { get; set; }
        public List<Shape> Shapes { get; set; }
        public List<BasicGeoposition> Positions { get; set; }

        public Calendar Calendar
        {
            get { return _calendar; }
            set { _calendar = value;
                OnPropertyChanged();
            }
        }

        public bool? ShowStops
        {
            get { return _showStops; }
            set { _showStops = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value;
                OnPropertyChanged();
            }
        }

        public Models.AT.TripUpdate TripUpdate
        {
            get { return _tripUpdate; }
            set { _tripUpdate = value;
                OnPropertyChanged();
            }
        }

        public bool IsLive
        {
            get { return _isLive; }
            set { _isLive = value;
                OnPropertyChanged();
            }
        }

        public RoutesDetailPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var route = (Route)e.Parameter;
            Route = route;

            Trips = Route.Trips;
            foreach (var t in Trips)
            {
                if (Route.ActiveTrips.Select(x => x.TripUpdate.Trip.TripId).Contains(t.Id))
                {
                    t.IsLive = true;
                    t.Live = Route.ActiveTrips.FirstOrDefault(x => x.TripUpdate.Trip.TripId == t.Id);
                }
            }

            Trips = Trips.OrderByDescending(x => x.IsLive).ToList();
            SelectedTrip = Trips.FirstOrDefault();            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void TripsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsLoading = true;
            SelectedTrip = (Trip)TripsComboBox.SelectedValue;
            ShowStops = true;

            var stopTimes = await RestService.GetStopTimesByTripId(SelectedTrip.Id);
            var shapes = await RestService.GetShapesById(SelectedTrip.ShapeId);
            var calendar = await RestService.GetCalendarByServiceId(SelectedTrip.ServiceId);

            StopTimes = stopTimes;
            Shapes = shapes;
            RoutesMapControl.Children.Clear();
            Positions = new List<BasicGeoposition>();
            DrawStopsOnMap();

            //MapHelper.SetCenterOfPoints(RoutesMapControl, Positions);
            RoutesMapControl.Center = new Geopoint(Positions.First());
            RoutesMapControl.ZoomLevel = 16;
            MapHelper.DrawShapes(RoutesMapControl, Shapes.OrderBy(x => x.Sequence));

            StopTimesList.ItemsSource = stopTimes;
            Calendar = calendar;
            CalendarDatesList.ItemsSource = Calendar.CalendarDates;

            AppBarFavorite.IsChecked = IsRouteFavourite();

            IsLive = false;
            if (SelectedTrip.Live != null)
            {
                IsLive = true;
                TripUpdate = SelectedTrip.Live.TripUpdate;
                TripUpdate.StopTimeUpdate.StopName = StopTimes.FirstOrDefault(x => x.StopId == TripUpdate.StopTimeUpdate.StopId).StopName;
                TripUpdate.StopTimeUpdate.StopRegionName = StopTimes.FirstOrDefault(x => x.StopId == TripUpdate.StopTimeUpdate.StopId).StopRegionName;
                var vehicle = await RestService.GetActiveVehicleByVehicleId(TripUpdate.Vehicle.Id);
                if (vehicle.Error == null)
                {
                    MapHelper.DrawBusOnMap(RoutesMapControl, vehicle.Response.Entity.FirstOrDefault().Vehicle.Position.Latitude, vehicle.Response.Entity.FirstOrDefault().Vehicle.Position.Longitude);
                }

                _vehicle = vehicle;
            }

            IsLoading = false;
        }

        private void ShowStopCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var grids = RoutesMapControl.Children.Where(x => x.GetType() == typeof(Grid)).ToList();
            foreach (var c in grids)
            {
                var point = (Grid)c;
                if (point.Name == "Point")
                    RoutesMapControl.Children.Remove(point);
            }

            if (ShowStops.Value)
                DrawStopsOnMap();
            
            MapHelper.DrawShapes(RoutesMapControl, Shapes.OrderBy(x => x.Sequence));
        }

        private void DrawStopsOnMap()
        {
            var count = 0;
            foreach (var st in StopTimes)
            {
                count++;
                var position = new BasicGeoposition() { Latitude = (double)st.StopLatitude, Longitude = (double)st.StopLongitude };
                var quickStopControl = new QuickStopTime(st);
                MapHelper.DrawPointOnMap(RoutesMapControl, position.Latitude, position.Longitude, st.StopId, (count == 1 ? Colors.Green : (count == StopTimes.Count() ? Colors.Red : Colors.Black)), quickStopControl);
                Positions.Add(position);                
            }
        }

        private void ResetMapButton_Click(object sender, RoutedEventArgs e)
        {
            MapHelper.SetCenterOfPoints(RoutesMapControl, Positions);
        }

        private void AppBarFavorite_Click(object sender, RoutedEventArgs e)
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["r" + Route.ShortId] = AppBarFavorite.IsChecked;
        }

        private bool IsRouteFavourite()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            var route = localSettings.Values["r" + Route.ShortId];
            if (route == null)
                return false;

            return (bool)route;
        }

        private async void AppBarRefresh_Click(object sender, RoutedEventArgs e)
        {
            IsLoading = true;
            IsLive = false;
            if (SelectedTrip.Live != null)
            {
                IsLive = true;
                TripUpdate = SelectedTrip.Live.TripUpdate;
                TripUpdate.StopTimeUpdate.StopName = StopTimes.FirstOrDefault(x => x.StopId == TripUpdate.StopTimeUpdate.StopId).StopName;
                TripUpdate.StopTimeUpdate.StopRegionName = StopTimes.FirstOrDefault(x => x.StopId == TripUpdate.StopTimeUpdate.StopId).StopRegionName;
                var vehicle = await RestService.GetActiveVehicleByVehicleId(TripUpdate.Vehicle.Id);
                if (vehicle.Error == null)
                {
                    MapHelper.DrawBusOnMap(RoutesMapControl, vehicle.Response.Entity.FirstOrDefault().Vehicle.Position.Latitude, vehicle.Response.Entity.FirstOrDefault().Vehicle.Position.Longitude);
                }

                _vehicle = vehicle;
            }

            IsLoading = false;
        }

        private void CenterLiveButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTrip.Live != null)
            {
                RoutesMapControl.Center = new Geopoint(new BasicGeoposition()
                {
                    Latitude = _vehicle.Response.Entity.FirstOrDefault().Vehicle.Position.Latitude,
                    Longitude = _vehicle.Response.Entity.FirstOrDefault().Vehicle.Position.Longitude
                });
            }
        }
    }
}
