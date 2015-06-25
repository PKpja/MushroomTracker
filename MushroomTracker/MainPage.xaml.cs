using Parse;
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

        /*
         * Konstruktor. Metoda wykonuje się przed uruchomieniem widoku mapy
         */
        public MainPage()
        {
            // poniższe dwie metody są wymagane przez framework windows phone.
            // przy utworzeniu nowego kliku są one automatycznie wygenerowane przez
            // visual studio
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;

            // utworzenie nowego obiektu Geolocator.
            // Ten obiekt będziemy wykorzystywać podczas wykonywania 
            // operacji na mapie. Pozwala nam na uzyskanie współrzędnych geograficznych 
            // urzytkownika.
            geolocator = new Geolocator();

            // stworzenie nowego obiektu Dispatcher. służy on do uruchamiania
            // metod asynchronicznych, czyli takich które wykonują się w tle
            // tj. nie blokują interfejsu użytkownika.
            dispatcher = Window.Current.CoreWindow.Dispatcher;  
        }

        /*
         * Metoda służy do konwertowania obiektu Coordinate (pomocniczego obiektu stworzonego
         * na potrzeby aplikacji) na obiekt Geopoint. Używamy obiektu Coordinate dlatego,
         * że obiekt Geopoint nie może być zapisywany do pamięci trwałej, gdyż nie implementuje
         * interfejsu Serializable
         */
        private Geopoint getGeopoint(Coordinate coordinate) {
            BasicGeoposition bgp = new BasicGeoposition();
            bgp.Latitude = coordinate.Latitude;
            bgp.Longitude = coordinate.Longitude;
            Geopoint geopoint = new Geopoint(bgp);
            return geopoint;
        }
         
        /*
         * Ta metoda odpalana jest kiedy wyświetla się strona z mapą. 
         * Jak sama nazwa wspazuje OnNavigatedTo czyli jak użytkownik
         * został przekierowany na tą stronę. W naszym przypadku w momencie
         * uruchomienia aplikacji.
         */
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                geolocator = new Geolocator();
                if (App.navigationObject != null)
                {
                    User u = getUser();
                    if (App.navigationObject.GetType() == typeof(Mushroom))
                    {
                        Mushroom newMushroom = (Mushroom)App.navigationObject;
                        u.updateMushroom(newMushroom);
                    }
                    else if (App.navigationObject.GetType() == typeof(string))
                    {
                        u.removeMushroomByObjectId((string)App.navigationObject);
                    }
                    App.navigationObject = null;
                    redrawMushrooms();                    
                }

            }
            else {
                refreshMushrooms();
            }
 
            // Odświerzenie ikonki obecnego położenia użytkownika czyli w naszym przypadku
            // niebieskiego celownika. Więcej info w deklaracji metody poniżej.
            refreshLocation();
            
            // konfiguracja obiektu Geolocator. Ustawiamy MovementThreshhold na 3, czyli 
            // z jaką obiekt będzie informował o zmianie położenia geograficznego
            // w jakim znajduje się obecnie telefon.
            geolocator.MovementThreshold = 3;  
            // dodajemy metodę która będzie się odpalać, kiedy Geolocator będzie chciał 
            // nas poinformować o tym, że zmieniło się położenie geograficzne telefonu.
            // Uruchomi się wtedy metoda geolocator_PositionChanged. więcej informaji 
            // w deklaracji metody poniżej.
            geolocator.PositionChanged +=
            new TypedEventHandler<Geolocator,
                PositionChangedEventArgs>(geolocator_PositionChanged);  

        }

        private void redrawMushrooms()
        {
            mapControl.Children.Clear();
            User u = getUser();
            foreach (Mushroom item in u.mushrooms)
            {
                addMushroomToMap(item);
            }
        }

        private async void refreshMushrooms()
        {
            User u = getUser();

            ParseQuery<ParseObject> q = ParseObject.GetQuery("Mushroom");
            IEnumerable<ParseObject> results = await q.FindAsync();
            foreach (ParseObject item in results)
            {
                Mushroom mushroom = new Mushroom(new Coordinate(item.Get<double>("Latitude"), item.Get<double>("Longitude")));
                mushroom.kind = item.Get<int>("kind");
                mushroom.density = item.Get<int>("density");
                mushroom.objectId = item.ObjectId;
                if (u.mushrooms == null) {
                    u.mushrooms = new List<Mushroom>();
                }
                u.mushrooms.Add(mushroom);
            }

            redrawMushrooms();

        }

        /*
         * Dodaj pojedynczego grzyba do mapy. Metoda ta jest wykorzystywana przy 
         * iterowaniu przez wszystkie grzyby użytkownika.
          */
        private void addMushroomToMap(Mushroom mushroom)
        {
            // Stworzenie nowego obiektu obrazka.
            Image image = new Image();
            // każdy grzyb posiada obrazek, w tym momencie pobieramy odpowiedni obrazek 
            // z obiektu grzyb
            Uri uri = mushroom.getUri();
            // dodanie grafiki do obrazka. Na podstawie adresu obrazka tworzymi bitmapę, którą następnie przypisujemy 
            // do obiektu image (obrazek) żeby wyswietliła się odpowiednia ikonka na mapie
            image.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(uri);
            // przypisujemy blok kodu który odpali się, kiedy uzytkownik kliknie na obrazek grzyba na mapie.
            image.Tapped += delegate(object sender, TappedRoutedEventArgs e)
            {
                string json = JsonHandler.convertToString(mushroom);
                Frame.Navigate(typeof(DetailsPage), json); 
            }; 
             
            // tworzymy obiekt Geopoin na podstawie długości i szerokości geograficznej
            // w jakiej grzyb się znajduje
            Geopoint location = new Geopoint(new BasicGeoposition()
            {
                // przypisujemy długość / szerokość geograficzną 
                // i mamy stworzony obiekt Geopoint
                Latitude = mushroom.coordinate.Latitude,
                Longitude = mushroom.coordinate.Longitude
            });
            // informujemy mapę o tym gdzie ma wyświetlić na mapie dany obrazek
            // wysyłamy do mapy informację w ten sposób na jakich współrzędnych geograficznych
            // powinien zostać wyświetlony jaki obrazek grzyba. Do tego służą dwie poniżesz metody.
            MapControl.SetLocation(image, location);
            MapControl.SetNormalizedAnchorPoint(image, new Point(0.5, 0.5));
            // dodajemy obrazek do mapy. Mapa już sama będzie wiedziała gdzie ten obrazek wyświetlić.
            mapControl.Children.Add(image);
            
        }
       

        /*
         * Metoda odpala się wtedy, kiedy telefon zmieni położenie i stworzony na samym poczatku działania 
         * mapy obiekt Geolocator będzie nas o tym chciał poinformować. Ta metoda ma deklarację "async"
         * oznacza to, że wykonuje się asynchronicznie i interfejs użytkownika nie zamarza w bezruchu czekając 
         * na wykonanie metody do końca.
         */
        async private void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {
            // odświerz obrazek niebieskiego celownika.
            refreshLocation();
        }

        /*
         * Odświerz obrazek niebieskiego celownika. Metoda słuzy do tego, żeby pokazać na mapie 
         * obrazek informujący o tym gdzie obecnie znaduje się telefon. Metoda ma teklarację "async"
         * czyli działa w tle
         */
        private async void refreshLocation()
        {
            // poniższy fragment kodu może generować niespodziewane błędy 
            // ze względu na pewnego rodzaju zdarzenia związane telefonem, które mogą 
            // zaburzać działanie poprawne lokalizatora gps
            // jeżeli taki problem nastąpi, to nie chcemy, żeby aplikacja się zawiesiła,
            // a raczej żeby cała operacja została zignorowana i wtedy poczekamy aż
            // metoda odpali się po raz kolejny.
            try
            {
                // pobieramy obecną lokalizację w któej znajduje się telefon. 
                // deklaracja await świadzczy o tym, że program będzie czekał
                // aż obiekt Geolocator zwróci obecne położenie.
                // Może to potrwać trochę czasu, ale nie szkodzi, bo metoda wykonuje sie
                // asynchronicznie i interfejs użytkownika nie zastygnie
                geoposition = await geolocator.GetGeopositionAsync();
                // z obiektu Geoposition pobieramy obiekt poin, który jest jego częścią składową
                // będzie nam on potrzbny w dalszej części metody.
                Geopoint point = geoposition.Coordinate.Point;

                // w momencie kiedy przypiszemy Geopoint do pola Center
                // mapa wyśwrodkuje się w tych współrzędnych geograficznych.
                mapControl.Center = point;
                // jeżeli ikonka niebieskiego celownika nie była wcześniej stworzona podczas dziłania programu, 
                // wtedy msimy ją stworzyć. kiedy następnym razem odpalimy tą metodę nie trzeba będzie tworzyć 
                // tego obrazka ponownie, a jedynie zaktualizować jego współrzędne na mapie.
                if (userLocationIcon == null)
                {
                    // Tworzenie nowej instancji obiektu MapIcon. MapIcon to taki rodzaj obrazka, któy jest
                    // wyswietlany na mapie.
                    userLocationIcon = new MapIcon();
                    // obrazek celownika jest podpisany "jesteś tutaj"
                    userLocationIcon.Title = "Jesteś tutaj";
                    // to musi być, służy do kotwiczenia ikonki
                    userLocationIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
                    // dodajemy grafikę do obrazka z pliku, który znajduje się na dysku.
                    userLocationIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/location.png"));
                    // dodajemy ikonkę do mapy i mapa już automatycznie będzie ją sobie wyświetlać
                    mapControl.MapElements.Add(userLocationIcon);
                }

                    // Celownik znajduje się na współrzędnych punktu dokłanie takiego samego 
                    // jak w przypadku wycentrowania mapy powyżej.
                    userLocationIcon.Location = point;
                
                // pobranie obiektu użytkownik
                User u = getUser();
                // zaktualizowanie lokalizacji w której uzytkownik ostatnio się znajdował
                u.lastLocation = new Coordinate(point);
                // zapisanie zaktualizowanego użytkownika do pamięci trwałej.
                //saveUserToStorage(u);
            }
            catch (Exception ignore)
            {
            }
        }

        /*
         * metoda służy do tego, żeby otrzymać obiekt użytkownik.
         */
        private User getUser()
        {
            // jeżeli w obiekcie tego widoku uzytkownik wcześniej nie był pobrany z bazy, 
            // to będzie miał wartość null
            if (user == null) {
                // pobranie użytkownika z pamięci trwałej telefonu
                user = new User();
            }
            // zwrócenie użytkownika
            return user;
        }

        /*
         * Metoda odpala się wtedy, kiedy uzytkownik kliknie w dowolne miejsce na mapie
         */
        private void mapControl_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            // pobieramu z obiektu MapInputEventArgs który dostalismy od mapy informacje o tym
            // na jaka współrzędną geograficzną użytkownik kliknął
            Geopoint point = args.Location;
            // pobieramy użytkownika.
            User user = getUser();
            // tworzymy obiekt Coordinate, czyi obiekt którego uzywamy żeby przechowywać współrzędne
            // geograficzne wenątrz naszego modelu danych (żeby można było go zapisać)
            Coordinate coordinate = new Coordinate(point);
            // tworzymy obiekt grzyba i w konstruktorze dodajemy współrzędne.
            // W ten sposób stworzyliśmy obiekt grzyba z jednym grzybem na określonych współrzędnych geograficznych.
            // W konstruktorze grzyba więcej informacji na ten temat.
            Mushroom mushroom = new Mushroom(coordinate);

            
            string json = JsonHandler.convertToString(mushroom);

            Frame.Navigate(typeof(DetailsPage), json); 
        
         
        }


        
    }
}
