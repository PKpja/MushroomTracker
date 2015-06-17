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
        public enum Kind { NONE, ONE,TWO,THREE }

        public Coordinate coordinate { get; set; }
        public Kind kind { get; set; }

        public Mushroom()
        {
        }

        public Mushroom(Coordinate point)
        {
            this.coordinate = point;
            kind = Kind.ONE;
        }

        internal void toggleKind()
        {
            if (kind == Kind.ONE) {
                kind = Kind.TWO;
            } else  if (kind == Kind.TWO) {
                kind = Kind.THREE;
            } else  if (kind == Kind.THREE) {
                kind = Kind.NONE;
            }  
        }

        internal Uri getUri()
        {
            if (kind == Kind.ONE) {
                return new Uri("ms-appx:///Assets/mushroom.png");
            } else  if (kind == Kind.TWO) {
                return new Uri("ms-appx:///Assets/mushroom_two.png");
            }
            else if (kind == Kind.THREE)
            {
                return new Uri("ms-appx:///Assets/mushroom_three.png");
            }
            else {
                return null;
            }
            
        }

        internal bool shouldDisplay()
        {
            return kind != Kind.NONE;
        }
    }
}
