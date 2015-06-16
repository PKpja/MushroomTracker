using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
 
namespace MushroomTracker
{
 
    public sealed partial class MainPage : Page
    {
        private CoreDispatcher dispatcher;
        private Geolocator geolocator;
        private Geoposition geoposition;
        private MapIcon userLocationIcon;
        private User user;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            geolocator = new Geolocator();
            dispatcher = Window.Current.CoreWindow.Dispatcher;  
        }

        private Geopoint getGeopoint(Coordinate coordinate) {
            BasicGeoposition bgp = new BasicGeoposition();
            bgp.Latitude = coordinate.Latitude;
            bgp.Longitude = coordinate.Longitude;
            Geopoint geopoint = new Geopoint(bgp);
            return geopoint;
        }
         
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            User u = getUser();
            /*Coordinate location = u.lastLocation;
             if (location != null)
             {
                 refreshLocation(getGeopoint(location));
             }
             else {*/
            refreshLocation();
            /*}*/
            if (u.mushrooms != null) {
                refreshMushrooms();
            }
            
            geolocator.MovementThreshold = 3;  
            geolocator.PositionChanged +=
            new TypedEventHandler<Geolocator,
                PositionChangedEventArgs>(geolocator_PositionChanged);  

        }

        private void refreshMushrooms()
        {
            User u = getUser();
            foreach (Mushroom mushroom in u.mushrooms)
            {
                addMushroomToMap(mushroom);
            }
        }

        private void addMushroomToMap(Mushroom mushroom)
        {
            MapIcon mushroomIcon = new MapIcon();
            mushroomIcon.Location = getGeopoint(mushroom.coordinate);
            mushroomIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
            mushroomIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/mushroom.png"));
            mapControl.MapElements.Add(mushroomIcon);
        }

        async private void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {
           
           await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
           {
               refreshLocation();
           });    

        }

        private async void refreshLocation()
        {
            try
            {
                geoposition = await geolocator.GetGeopositionAsync();
                Geopoint point = geoposition.Coordinate.Point;

                mapControl.Center = point;
                if (userLocationIcon == null)
                {
                    userLocationIcon = new MapIcon();
                    userLocationIcon.Title = "Jestes tutaj";
                    userLocationIcon.Location = geoposition.Coordinate.Point;
                    userLocationIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
                    userLocationIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/location.png"));
                    mapControl.MapElements.Add(userLocationIcon);
                }
                else {
                    userLocationIcon.Location = geoposition.Coordinate.Point;
                }
                

                User u = getUser();
                u.lastLocation = new Coordinate(point);
                saveUserToStorage(u);
            }
            catch (Exception ignore)
            {
            }
        }

        private User getUser()
        {
            if (user == null) {
                user = getUserFromStorage();
            }
            return user;
        }

       
        public void saveUserToStorage(User user)
        {
            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer jsonSer =
            new DataContractJsonSerializer(typeof(User));
            jsonSer.WriteObject(stream, user);
            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);
            string json = sr.ReadToEnd();

            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["user"] = json;  
        }

        public User getUserFromStorage()
        {
            User u;
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            string userString = localSettings.Values["user"] as string;

            if (userString != null)
            {
                byte[] data = Encoding.UTF8.GetBytes(userString);
                MemoryStream memStream = new MemoryStream(data);
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(User));
                u = (User)serializer.ReadObject(memStream);
            }
            else
            {
                u = new User();
            }

            return u;
        }

        private void mapControl_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            Geopoint point = args.Location;
            User user = getUser();
            Coordinate coordinate = new Coordinate(point);

            Mushroom mushroom = new Mushroom(coordinate);

            addMushroomToMap(mushroom);
            if (user.mushrooms == null)
            {
                user.mushrooms = new List<Mushroom>();
            }
            user.mushrooms.Add(mushroom);
            saveUserToStorage(user);
        }
        
    }
}
