using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers; // Do Timera
using _01_agro.Core;
using _02_agro.Data;


namespace _03_agro.Logic
{
    public class SimulationEngine
    {
        private FarmState _state;
        private LogRepo _logger;
        private System.Timers.Timer _gameTimer;

        // --- KONSTRUKTOR ---
        public SimulationEngine()
        {
            _logger = new LogRepo();
            _state = GameSaver.LoadGame();

            if (_state == null)
            {
                _state = new FarmState();
                _logger.AddLog("Rozpoczęto nową grę.");
                
            }
            else
            {
                _logger.AddLog("Wczytano zapis gry.");
            }
            // Inicjalizacja farmy - kod wykona się tylko gdy nie będzie ani jednego pomidora
            InitializeStarterFarm();


            // Podpięcie loggera
            _state.Logger = (message) => _logger.AddLog(message);

            // Konfiguracja Timera
            _gameTimer = new System.Timers.Timer(1000); // 1 sekunda
            _gameTimer.Elapsed += OnTimedEvent;
            _gameTimer.AutoReset = true;
        }

        // --- METODY STERUJĄCE (START / STOP) ---
        public void StartSimulation()
        {
            _gameTimer.Start();
            _logger.AddLog("SYMULACJA: Rozpoczęto.");
        }

        public void StopSimulation()
        {
            _gameTimer.Stop();
            _logger.AddLog("SYMULACJA: Zatrzymano.");
            GameSaver.SaveGame(_state);
        }

        // --- OBSŁUGA TIMERA ---
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Tick(); // Logika 

            // --- PODGLĄD NA ŻYWO ---
            Console.Clear(); // Czyści ekran

            Console.WriteLine($"=== AGRO SYMULACJA | Tura: {_state.CurrentTick} ===");
            Console.WriteLine($"UV: {_state.LightLevel:F1} | GLEBA (Woda): {_state.SoilMoisture:F1}%");
            Console.WriteLine("---------------------------------------------");

            // Sprawdźmy pierwszego pomidora (jeśli istnieje)
            var testPomidor = _state.Tomatoes.FirstOrDefault();
            if (testPomidor != null)
            {
                string status = testPomidor.IsDead ? "MARTWY" : "ŻYJE";
                Console.WriteLine($"[MONITORING] Pomidor #1: {status}");
                Console.WriteLine($" - Nawodnienie: {testPomidor.PoziomNawodnienia:F1}%");
                Console.WriteLine($" - Wzrost:      {testPomidor.PoziomWzrostu:F1}%");
                Console.WriteLine($" - Nasłonecznienie:      {testPomidor.PoziomNaslonecznienia:F1}%");
            }
            else
            {
                Console.WriteLine("[INFO] Brak pomidorów na farmie.");
            }

            Console.WriteLine("\n[Wciśnij Enter w drugim oknie, aby zamknąć]");
        }

        // --- REJESTRACJA OBIEKTÓW (To woła GUI jak coś się kupuje) ---
        public void RegisterObject(ITickable obj)
        {
            // SORTOWNIA: Wrzucamy obiekt do odpowiedniej listy
            if (obj is Tomato tomato) _state.Tomatoes.Add(tomato);
            else if (obj is Apple apple) _state.Apples.Add(apple);
            else if (obj is Rose rose) _state.Roses.Add(rose);
            else if (obj is Cactus cactus) _state.Cactile.Add(cactus);

            else if (obj is Sprinkler sprinkler) _state.Sprinklers.Add(sprinkler);
            else if (obj is Solar solar) _state.Solars.Add(solar);
            else if (obj is Sensor sensor) _state.Sensors.Add(sensor);

            else
            {
                _logger.AddLog($"BŁĄD: Nieznany typ obiektu: {obj.GetType().Name}");
                return;
            }

            _logger.AddLog($"[agro.Logic]: Dodano nowy obiekt: {obj.GetType().Name}");
        }

        // --- PAKIET STARTOWY ---
        public void InitializeStarterFarm()
        {
            if (_state.Tomatoes.Count == 0)
            {
                _state.SoilMoisture = 30;
                _state.LightLevel = 3;
                _logger.AddLog("[agro.Logic]: Wykryto pustą farmę. Sadzenie pakietu startowego...");
                for (int i = 0; i < 5; i++)
                {
                    var p = new Tomato();
                    p.PoziomNawodnienia = 60;
                    p.PoziomNaslonecznienia = 60;
                    _state.Tomatoes.Add(p);
                }
                GameSaver.SaveGame(_state);
            }
        }

        // --- GŁÓWNA PĘTLA (TICK) ---
        public void Tick()
        {
            _state.CurrentTick++; // Zwiększenie ticka
            // Gleba sama wysycha i zmniejsza się poziom UV
            _state.SoilMoisture -= 1.0;
            if (_state.SoilMoisture < 0) _state.SoilMoisture = 0;
            _state.LightLevel -= 1.0;
            if (_state.LightLevel < 0) _state.LightLevel = 0;
            // 1. Zbieramy wszystko do jednej listy
            var allObjects = new List<ITickable>();

            allObjects.AddRange(_state.Sprinklers);
            allObjects.AddRange(_state.Solars);
            allObjects.AddRange(_state.Sensors);
            allObjects.AddRange(_state.Tomatoes);
            allObjects.AddRange(_state.Apples);
            allObjects.AddRange(_state.Roses);
            allObjects.AddRange(_state.Cactile);

            // 2. Wykonujemy logikę
            foreach (var obj in allObjects)
            {
                try
                {
                    obj.Tick(_state);
                }
                catch (Exception ex)
                {
                    _state.Logger?.Invoke($"Błąd obiektu {obj.GetType().Name}: {ex.Message}");
                }
            }

            // 3. AutoSave (co 10 tur)
            if (_state.CurrentTick % 10 == 0)
            {
                try
                {
                    GameSaver.SaveGame(_state);

                }
                catch (Exception ex)
                {
                    _logger.AddLog($"[agro.Logic]Błąd zapisu: {ex.Message}");
                }
            }

            // 4. Sprzątanie martwych roślin
            _state.Tomatoes.RemoveAll(p => p.IsDead);
            _state.Cactile.RemoveAll(p => p.IsDead);
            _state.Roses.RemoveAll(p => p.IsDead);
            _state.Apples.RemoveAll(p => p.IsDead);
        }
    } 
} 