using AucklandRide.UWP.Controls.UserControls;
using AucklandRide.UWP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace AucklandRide.UWP.Helpers
{
    public class MapHelper
    {
        private static Popup _popup;

        public static Geopoint DrawPointOnMap(MapControl mapControl, double latitude, double longitude, string displayText, Color background, QuickMapControl showTapControl = null)
        {
            if (mapControl == null)
                return null;

            var center = new BasicGeoposition()
            {
                Latitude = latitude,
                Longitude = longitude
            };
            var centerPoint = new Geopoint(center);

            var text = new TextBlock
            {
                FontWeight = FontWeights.Light,
                FontSize = 12,
                Text = displayText,
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var grid = new Grid
            {
                Name = "Point",
                Height = 17,
                Width = 35,
                Margin = new Thickness(10),
                Background = new SolidColorBrush(background)
            };

            grid.Children.Add(text);
            if (showTapControl != null)
            {
                grid.Tag = showTapControl;
                grid.Tapped -= Grid_Tapped;
                grid.Tapped += Grid_Tapped;
            }

            Canvas.SetZIndex(grid, 0);
            MapControl.SetLocation(grid, centerPoint);
            MapControl.SetNormalizedAnchorPoint(grid, new Point(0.5, 0.5));
            mapControl.Children.Add(grid);

            return centerPoint;
        }

        public static Geopoint DrawBusOnMap(MapControl mapControl, double latitude, double longitude)
        {
            var center = new BasicGeoposition()
            {
                Latitude = latitude,
                Longitude = longitude
            };
            var centerPoint = new Geopoint(center);

            var text = new TextBlock
            {
                FontFamily = new FontFamily("Segoe MDL2 Assets"),
                FontSize = 20,
                Text = "\uE806",
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var grid = new Grid()
            {
                Name = "Live"
            };
            var ellipse = new Ellipse
            {
                Height = 50,
                Width = 50,
                Fill = new SolidColorBrush(Colors.DeepSkyBlue)
            };

            var exist = mapControl.Children.Where(x => x.GetType() == typeof(Grid));
            if (exist != null)
            {
                foreach (var c in exist)
                {
                    var gridExist = (Grid)c;
                    if (gridExist.Name == "Live")
                        mapControl.Children.Remove(gridExist);
                }
            }

            grid.Children.Add(ellipse);
            grid.Children.Add(text);

            Canvas.SetZIndex(grid, 100);
            MapControl.SetLocation(grid, centerPoint);
            MapControl.SetNormalizedAnchorPoint(grid, new Point(0.5, 0.5));
            mapControl.Children.Add(grid);

            return centerPoint;
        }

        private static void Grid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var grid = (Grid)sender;
            var userControl = (QuickMapControl)grid.Tag;
            userControl.BackToMapButtonTapped -= new EventHandler(BackToMapButton_Tapped);
            userControl.BackToMapButtonTapped += new EventHandler(BackToMapButton_Tapped);
            _popup = new Popup()
            {
                Child = userControl,
                IsOpen = true
            };
        }

        private static void BackToMapButton_Tapped(object sender, EventArgs e)
        {
            _popup.IsOpen = false;
            _popup.Child = null;
        }

        public static void SetCenterOfPoints(MapControl mapControl, IEnumerable<BasicGeoposition> positions)
        {
            var mapWidth = mapControl.ActualWidth;
            var mapHeight = mapControl.ActualHeight;
            if (mapWidth == 0 || mapHeight == 0)
                return;

            if (positions.Count() == 0)
                return;

            if (positions.Count() == 1)
            {
                var singleGeoposition = new BasicGeoposition()
                {
                    Latitude = positions.First().Latitude,
                    Longitude = positions.First().Longitude
                };
                mapControl.Center = new Geopoint(singleGeoposition);
                mapControl.ZoomLevel = 16;
                return;
            }

            var maxLatitude = positions.Max(x => x.Latitude);
            var minLatitude = positions.Min(x => x.Latitude);

            var maxLongitude = positions.Max(x => x.Longitude);
            var minLongitude = positions.Min(x => x.Longitude);

            var centerLatitude = ((maxLatitude - minLatitude) / 2) + minLatitude;
            var centerLongitude = ((maxLongitude - minLongitude) / 2) + minLongitude;

            var nw = new BasicGeoposition()
            {
                Latitude = maxLatitude,
                Longitude = minLongitude
            };

            var se = new BasicGeoposition()
            {
                Latitude = minLatitude,
                Longitude = maxLongitude
            };

            var buffer = 1;
            //best zoom level based on map width
            var zoomWidth = Math.Log(360.0 / 256.0 * (mapWidth - 2 * buffer) / (maxLongitude - minLongitude)) / Math.Log(2);
            //best zoom level based on map height
            var zoomHeight = Math.Log(180.0 / 256.0 * (mapHeight - 2 * buffer) / (maxLatitude - minLatitude)) / Math.Log(2);
            var zoom = (zoomWidth + zoomHeight) / 2;
            mapControl.ZoomLevel = zoom - 0.8;

            //var box = new GeoboundingBox(nw, se);
            var geoposition = new BasicGeoposition()
            {
                Latitude = ((maxLatitude - minLatitude) / 2) + minLatitude,
                Longitude = ((maxLongitude - minLongitude) / 2) + minLongitude
            };
            mapControl.Center = new Geopoint(geoposition);
        }

        public static void DrawShapes(MapControl mapControl, IEnumerable<Models.Shape> shapes)
        {
            mapControl.MapElements.Clear();

            var polyline = new MapPolyline();
            var posList = new List<BasicGeoposition>();
            foreach (var shape in shapes)
            {
                posList.Add(new BasicGeoposition()
                {
                    Latitude = Convert.ToDouble(shape.Latitude),
                    Longitude = Convert.ToDouble(shape.Longitude)
                });
            }

            polyline.StrokeColor = Color.FromArgb(0xFF, 0x00, 0x97, 0xFF);
            polyline.StrokeThickness = 4;
            polyline.Path = new Geopath(posList);
            mapControl.MapElements.Add(polyline);
        }
    }
}
