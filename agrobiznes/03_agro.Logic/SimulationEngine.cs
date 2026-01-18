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
            // WCZYTYWANIE STANU GRY
            _state = GameSaver.LoadGame();
            _logger = new LogRepo();
            if (_state == null)
            {
                _state = new FarmState();
                _logger.AddLog("Rozpoczęto nową grę.");
            }
            else
            {
                _logger.AddLog("Wczytano zapis gry.");
            }

            
            _state.Logger = (message) => _logger.AddLog(message);

        }

        // Metoda do dodawania obiektów (np. GUI ją wywoła jak gracz kupi maszynę)
        // jeszcze nie wiem jak zrobić
        

        // --- GŁÓWNA PĘTLA SYMULACJI (HEARTBEAT) ---
        // Ta metoda będzie wywoływana przez Timer z GUI (np. co 1 sekundę)
        public void Tick()
        {
            _state.CurrentTick++;

            // KROK 1: Zbierz wszystko w jedną tymczasową listę
            
            var allObjects = new List<ITickable>();

            // DODAĆ OBIEKTY !!!
            allObjects.AddRange(_state.Sprinklers);
            allObjects.AddRange(_state.Solars);
            allObjects.AddRange(_state.Sensors);

            // KROK 2: Wykonaj logikę
            foreach (var obj in allObjects)
            {
                try
                {
                    obj.Tick(_state);
                }
                catch (Exception ex)
                {
                    _state.Logger?.Invoke($"Błąd obiektu: {ex.Message}");
                }
            }

            // obsługa AutoSave

            if (_state.CurrentTick % 10 == 0)
            {
                try
                {
                    GameSaver.SaveGame(_state);
                    _logger.AddLog("Wykonano automatyczny zapis gry [agro.Logic].");
                }
                catch (Exception ex)
                {
                    _logger.AddLog($"Błąd zapisu gry [agro.Logic]: {ex.Message}");
                }
                
            }
        }

        
        
    }
}
