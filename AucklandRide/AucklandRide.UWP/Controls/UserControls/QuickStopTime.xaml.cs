using AucklandRide.UWP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AucklandRide.UWP.Controls.UserControls
{
    public sealed partial class QuickStopTime : QuickMapControl
    {
        public StopTime StopTime { get; set; }

        public QuickStopTime(StopTime stopTime)
        {
            this.InitializeComponent();

            StopTime = stopTime;
            PopupGrid.Width = Window.Current.Bounds.Width;
            PopupGrid.Height = Window.Current.Bounds.Height;

            Window.Current.SizeChanged += Current_SizeChanged;
        }

        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            PopupGrid.Width = Window.Current.Bounds.Width;
            PopupGrid.Height = Window.Current.Bounds.Height;
        }

        private void BackToMapButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            BackToMapButtonTapped.Invoke(sender, EventArgs.Empty);
        }
    }
}
