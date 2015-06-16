using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace MushroomTracker
{

    public class User
    {
        public Coordinate lastLocation { get; set; }
        public List<Mushroom> mushrooms {  get;  set; }
    }
}
