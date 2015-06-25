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
        // ID nadawane przez parse.com
        public string objectId { get; set; }
        // współrzędne geograficzne długość i szerokość w postaci obiektu Coordinates
        // 
        public Coordinate coordinate { get; set; }
        // typ Kind czyli jedna z wartości z enumu na górze deklaracji klasy NONE, ONE etc.
        public int kind { get; set; }
        // zageszczenie density
        public int density { get; set; }

        // pusty konstruktor. Musi być żeby przekonwertować obiekt z jsona (tekstu) spowrotem na obiekt C#
        public Mushroom()
        {
        }

        // ten konstruktor słuzy do prostego stworzenia nowego 
        // grzyba. 
        public Mushroom(Coordinate point)
        {
            //Grzyb ten znajduje sie na pewnych współrzednych geograficznych
            this.coordinate = point;
            // i jest typu Jeden grzyb
            kind = 0;
            density = 0;
        }

        // na podstawie tego jaki jest to typ grzyba zwróć obrazek któy ma być wyświetlany na mapie
        // czy jest to jeden grzyb, czy dwa czy trzy.
        internal Uri getUri()
        {
            string output = "ms-appx:///Assets/";

            if (kind == 0)
            {
                output += "bay_bolete_";
            }
            else if (kind == 1)
            {
                output += "cep_mushroom_";
            }
            else if (kind == 2)
            {
                output += "chanterelle_";
            }
            else if (kind == 3)
            {
                output += "cossack_";
            }
            else if (kind == 4)
            {
                output += "suillus_";
            }

            if (density == 2)
            {
                output += "3";
            }
            else if (density == 1)
            {
                output += "2";
            }
            else
            {
                output += "1";
            }

            output += ".png";

            return new Uri(output);

        }

        internal void setKindFromInt(int i)
        {
            kind = i;
        }

        internal void setDensityFromInt(int p)
        {
            density = p;
        }


    }
}
