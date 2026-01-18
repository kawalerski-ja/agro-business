using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core
{   //klasa FarmState czyli "stan świata": będzie przechowywała roślinki i maszyny, tick, pieniądze
    public class FarmState
    {
        // 1. Czas symulacji
        public long CurrentTick { get; set; } = 0;

        public double SoilMoisture { get; set; } = 20.0;//0-100 poziom nawodnienia gleby

        public double LightLevel { get; set; } = 20.0;

        // Delegat: To jest "miejsce na funkcję", którą podstawi Silnik.
        // Mówi: "Mogę przyjąć funkcję, która bierze string i nic nie zwraca".
        // [NotMapped] oznacza, że nie chcemy tego zapisywać w tabeli FarmState w bazie.
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public Action<string> Logger { get; set; }

        // 2. Pieniądze gracza (na start np. 1000)


        // 3. Główna lista obiektów symulacji
        // Silnik będzie robił: foreach(var obj in TickableObjects) obj.Tick(this);
        public List<ITickable> TickableObjects { get; set; }

        // Konstruktor inicjalizujący listę (żeby uniknąć błędów null)
        public FarmState()
        {
            TickableObjects = new List<ITickable>();
        }

        // --- Plany na przyszłość ---
        // Można np dodać: public List<Crop> Crops { get; set; } = new List<Crop>();
        // Czyli po prostu przechowywanie roślinek
    }
}
