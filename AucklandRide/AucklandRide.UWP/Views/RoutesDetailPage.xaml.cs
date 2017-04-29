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

        public RoutesDetailPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var route = (Route)e.Parameter;
            Route = route;

            Trips = Route.Trips;
            SelectedTrip = Trips.FirstOrDefault();            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void TripsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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

            MapHelper.SetCenterOfPoints(RoutesMapControl, Positions);
            MapHelper.DrawShapes(RoutesMapControl, Shapes.OrderBy(x => x.Sequence));

            StopTimesList.ItemsSource = stopTimes;
            Calendar = calendar;
            CalendarDatesList.ItemsSource = Calendar.CalendarDates;
        }

        private void ShowStopCheckBox_Click(object sender, RoutedEventArgs e)
        {
            RoutesMapControl.Children.Clear();
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
    }
}
