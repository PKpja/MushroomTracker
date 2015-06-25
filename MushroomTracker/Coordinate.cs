using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;

namespace MushroomTracker
{
    /*
     * To jest obiekt pomocniczy, który słuzy do tego, żeby można było zapisać w pamięci stałej
     * współrzędne geograficzne długość i szerokość.
     */
    public class Coordinate
    {
      

        //pusty konstruktor musi być żeby odtworzyc obiekt z bazy danych.
        // W przciwnym wypadku kiedy będziemy chcieli tekst Json przekonwertować na User, to aplikacja się wywali.
        public Coordinate()
        {
        }

        // konwertowanie obiektu Geopoint na Coordinate w konstruktorze
        public Coordinate(Geopoint point)
        {
            Latitude = point.Position.Latitude;
            Longitude = point.Position.Longitude;
        }

        public Coordinate(double Latitude, double Longitude)
        {
            // TODO: Complete member initialization
            this.Latitude = Latitude;
            this.Longitude = Longitude;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }
}
