using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;

namespace MushroomTracker
{
    public class Coordinate
    {

        public Coordinate()
        {
        }

        public Coordinate(Geopoint point)
        {
            Latitude = point.Position.Latitude;
            Longitude = point.Position.Longitude;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }
}
