using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _01_agro.Core;
using _02_agro.Data;

namespace _03_agro.Logic
{
    public class SimulationEngine
    {
        // 1. Stan Świata (To tutaj żyją wszystkie obiekty: rośliny, maszyny, kasa)
        // Dzięki temu silnik wie, czym zarządzać.
        private FarmState _state;

        // 2. Narzędzie do logowania
        // Odpowiada za raportowanie błędów do bazy SQL.
        private LogRepo _logger;

        public SimulationEngine()
        {
            _state = new FarmState();
            _logger = new LogRepo();

           
        }

        // Metoda do dodawania obiektów (np. GUI ją wywoła jak gracz kupi maszynę)
        // Oczywiście cała farma, maszyny, portfel będą wczytywane inną metodą
        public void RegisterObject(ITickable obj)
        {
            _state.TickableObjects.Add(obj);

            // Logujemy zdarzenie techniczne do bazy
            _logger.AddLog($"SYSTEM: Dodano nowy obiekt do symulacji: {obj.GetType().Name}");
        }

        // --- GŁÓWNA PĘTLA SYMULACJI (HEARTBEAT) ---
        // Ta metoda będzie wywoływana przez Timer z GUI (np. co 1 sekundę)
        public void Tick()
        {
            _state.CurrentTick++;

            

            // KROK 1: Iteracja po wszystkim co żyje
            // Używamy .ToList(), żeby zrobić kopię listy. 
            // Zapobiega to błędom, gdyby np. roślina umarła (została usunięta) w trakcie pętli.
            foreach (var obj in _state.TickableObjects.ToList())
            {
                try
                {
                    // === Tu dzieje sie cala symulacja ===
                    // Jeśli obj to Roślina -> tutaj sprawdzi wodę i urośnie.
                    // Jeśli obj to Maszyna -> tutaj zużyje prąd i zadziała.
                    // Jeśli obj to Rynek -> tutaj zmieni ceny.
                    obj.Tick(_state);
                }
                catch (Exception ex)
                {
                    

                    string errorMsg = $"CRITICAL ERROR w obiekcie {obj.GetType().Name}: {ex.Message}";

                    Console.WriteLine(errorMsg); // Na ekran debugowania
                    _logger.AddLog(errorMsg);    // Do tabeli SystemLogs w bazie
                }
            }

            // KROK 2: Obsługa AutoSave (działka Jana)
            // Co 10 cykli (czyli np. co 10 sekund) zapisujemy grę
            if (_state.CurrentTick % 10 == 0)
            {
                PerformAutoSave();
            }
        }

        // Metoda pomocnicza do zapisu stanu
        private void PerformAutoSave()
        {
            try
            {
                // [TODO] Tu w przyszłości wstawiam kod zapisu całego obiektu FarmState do bazy
                // np. _repo.SaveGame(_state);

                
                _logger.AddLog("Wykonano automatyczny zapis gry."); // Opcjonalnie, żeby nie spamować bazy
            }
            catch (Exception ex)
            {
                _logger.AddLog($"Błąd zapisu gry (AutoSave): {ex.Message}");
            }
        }
    }
}
