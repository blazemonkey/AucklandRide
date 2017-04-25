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
using Windows.UI.Xaml.Media;

namespace AucklandRide.UWP.Helpers
{
    public class MapHelper
    {
        public static Geopoint DrawPointOnMap(MapControl mapControl, double latitude, double longitude, string displayText)
        {
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
                Height = 17,
                Width = 35,
                Margin = new Thickness(10),
                Background = new SolidColorBrush(Colors.Black)
            };

            grid.Children.Add(text);
            MapControl.SetLocation(grid, centerPoint);
            MapControl.SetNormalizedAnchorPoint(grid, new Point(0.5, 0.5));
            mapControl.Children.Add(grid);

            return centerPoint;
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

        public static void DrawShapes(MapControl mapControl, IEnumerable<Shape> shapes)
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
