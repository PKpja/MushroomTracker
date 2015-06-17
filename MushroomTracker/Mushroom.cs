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
        // Pmocniczy enum któy świadczy o tym jakiego typu to jest grzyb
        // czy grzyb "żaden" któy nadaje się do usunięcia, czy grzyb "jeden"
        // któy wyświetlany jest w postaći obrazka z jednym grzybem, 
        // czy dwa czy trzy.
        public enum Kind { NONE, ONE,TWO,THREE }

        // współrzędne geograficzne długość i szerokość w postaci obiektu Coordinates
        // 
        public Coordinate coordinate { get; set; }

        // typ Kind czyli jedna z wartości z enumu na górze deklaracji klasy NONE, ONE etc.
        public Kind kind { get; set; }

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
            kind = Kind.ONE;
        }

        // zmień obecny rodzaj grzyba na kolejny
        // jeżeli grzyb ma teraz rodzaj ONE, to zmień na TWO,
        // później na THREE a później na NONE
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

        // na podstawie tego jaki jest to typ grzyba zwróć obrazek któy ma być wyświetlany na mapie
        // czy jest to jeden grzyb, czy dwa czy trzy.
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

        // jeżeli typ grzyba jest NONE, wtedy nie chcemy go wyświetlić tylko usunąć. 
        // ta metoda informuje o tym czy chcemy grzyba wyświetlić.
        internal bool shouldDisplay()
        {
            return kind != Kind.NONE;
        }
    }
}
