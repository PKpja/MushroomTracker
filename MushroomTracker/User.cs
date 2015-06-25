using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace MushroomTracker
{
    // wszystkie informacje które chcemy przetrzymywać w pamięci stałej,
    // czyli po zamknięciu aplikacji znajdują się w obiekcie tej klasy.
    // aplikacja posiada w każdym przypadku tylko jeden obiekt tej klasy.
    public class User
    {
        // ostatnia współprzędna na której znajdował się telefon
        public Coordinate lastLocation { get; set; }
        // lista grzybów
        public List<Mushroom> mushrooms { get; set; }

        public User()
        {
            mushrooms = new List<Mushroom>();
        }

        internal void updateMushroom(Mushroom mushroom)
        {
            if (mushrooms == null)
            {
                mushrooms = new List<Mushroom>();
            }

            if (mushroom.objectId == null)
            {
                mushrooms.Add(mushroom);
                return;
            }

            var i = mushrooms.FindIndex(x => x.objectId == mushroom.objectId);
            if (i < 0)
            {
                mushrooms.Add(mushroom);
            }
            else
            {
                mushrooms[i] = mushroom;

            }


        }

        internal void removeMushroomByObjectId(string p)
        {
            if (p == null) {
                return;
            }
            Mushroom m = mushrooms.Find(x => x.objectId == p);
            if (m != null)
            {
                mushrooms.Remove(m);
            }
        }
    }
}
