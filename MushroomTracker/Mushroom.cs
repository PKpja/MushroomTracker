using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace MushroomTracker
{
    public class Mushroom
    {
        public Coordinate coordinate { get; set; }

        public Mushroom()
        {
        }

        public Mushroom(Coordinate point)
        {
            this.coordinate = point;
        }
    }
}
