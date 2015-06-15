using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
 
namespace MushroomTracker
{
 
    public sealed partial class MainPage : Page
    {
        private CoreDispatcher dispatcher;
        private Geolocator geolocator;
        private Geoposition geoposition;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            geolocator = new Geolocator();
            dispatcher = Window.Current.CoreWindow.Dispatcher;  
        }
         
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
            refreshLocation();

            geolocator.MovementThreshold = 3;  
            geolocator.PositionChanged +=
            new TypedEventHandler<Geolocator,
                PositionChangedEventArgs>(geolocator_PositionChanged);  

        }

        async private void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {
           
           await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
           {
               refreshLocation();
           });    

        } 

        private async void refreshLocation() {
            try
            {
                geoposition = await geolocator.GetGeopositionAsync();
                mapControl.Center = geoposition.Coordinate.Point;
            }
            catch (Exception ex)
            {
            }
        }

        
    }
}
