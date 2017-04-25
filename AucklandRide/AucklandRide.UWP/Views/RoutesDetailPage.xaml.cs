using AucklandRide.UWP.Helpers;
using AucklandRide.UWP.Models;
using AucklandRide.UWP.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class RoutesDetailPage : Page
    {
        public Route Route { get; set; }
        public List<Trip> Trips { get; set; }
        public Trip SelectedTrip { get; set; }

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

        private async void TripsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedTrip = (Trip)TripsComboBox.SelectedValue;
            var stopTimes = await RestService.GetStopTimesByTripId(SelectedTrip.Id);
            var shapes = await RestService.GetShapesById(SelectedTrip.ShapeId);

            RoutesMapControl.Children.Clear();
            var positionsList = new List<BasicGeoposition>();
            foreach (var st in stopTimes)
            {
                var position = new BasicGeoposition() { Latitude = (double)st.StopLatitude, Longitude = (double)st.StopLongitude };
                MapHelper.DrawPointOnMap(RoutesMapControl, position.Latitude, position.Longitude, st.StopId);
                positionsList.Add(position);
            }

            MapHelper.SetCenterOfPoints(RoutesMapControl, positionsList);
            MapHelper.DrawShapes(RoutesMapControl, shapes.OrderBy(x => x.Sequence));
        }
    }
}
