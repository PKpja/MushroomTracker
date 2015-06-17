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
        public List<Mushroom> mushrooms {  get;  set; }
    }
}
