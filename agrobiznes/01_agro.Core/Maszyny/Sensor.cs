using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core
{
    public class Sensor : ITickable
    {
        [Key] // Klucz główny w bazie
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Używamy własnego GUID, nie auto-number
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "Sensor Wilgotności i UV";

        // Ustawienia sensora
        public double CriticalThreshold { get; set; } = 20.0; // Poniżej 20% włącza alarm

        // Ostatni odczyt (żeby np. GUI mogło go wyświetlić)
        public double WaterReading { get; private set; }
        public double UVReading { get; private set; }

        public void Tick(FarmState state)
        {
            // 1. POBIERZ DANE (READ)
            // Sensor "patrzy" na glebę w FarmState
            WaterReading = state.SoilMoisture;
            UVReading = state.LightLevel;

            // 2a. ANALIZA I ALARM: Woda
            if (WaterReading < CriticalThreshold)
            {
                // Tutaj sensor reaguje - uruchomienie zraszacza na stanie
                state.Logger?.Invoke($"[ALARM] Wilgotność gleby krytyczna: {WaterReading}%");
                /*
                 * var pump = state.TickableObjects.OfType<Sprinkler>().FirstOrDefault();
                   if (pump != null) pump.TurnOn();
                 */
            }
            // 2b. ANALIZA I ALARM: UV
            if (UVReading < CriticalThreshold) {
                //dodaj uruchomienie lamp UV
                state.Logger?.Invoke($"[ALARM] Krytyczny poziom UV: {UVReading}%");
            }
        }
    }
}
