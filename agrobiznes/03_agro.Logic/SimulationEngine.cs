using _01_agro.Core;
using _01_agro.Core.Economy;
using _02_agro.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers; // Do Timera


namespace _03_agro.Logic
{
    public class SimulationEngine
    {
        private FarmState _state;
        private LogRepo _logger;
        private const int BillingIntervalTicks = 30;

        public FarmState State => _state;
        public LogRepo Logger => _logger;

        private System.Timers.Timer _gameTimer;
        
        public Market Market { get; private set; }


        // Meotda do sprawdzania uruchomeina finance engine
        private void EnsureFinanceInitialized()
        {
            if (_state.Finance == null)
            {
                _state.Finance = new FinanceEngine(
                    new Account(new Money(_state.BalanceAmount, _state.BalanceCurrency)),
                    new NoTax()
                );
            }
        }

        // Metoda do naliczania kosztów operacyjnych
        private void ApplyOperatingCostsIfDue()
        {
            // rozliczamy koszty co 30 ticków
            if (_state.CurrentTick % BillingIntervalTicks != 0)
                return;

            int onSprinklers = _state.Sprinklers.Count(s => s.IsOn);
            int onLamps = _state.Solars.Count(l => l.IsOn);

            // stawki (dobierz jak chcesz)
            decimal waterCostPerDevice = 0.30m;
            decimal energyCostPerDevice = 0.50m;

            decimal waterTotal = onSprinklers * waterCostPerDevice;
            decimal energyTotal = onLamps * energyCostPerDevice;

            // jedna transakcja na wodę i jedna na energię (max 2 wpisy/30 ticków)
            if (waterTotal > 0)
            {
                _state.Finance.Apply(new PurchaseTransaction(
                    new Money(waterTotal, "PLN"),
                    TransactionCategory.Water,
                    $"Koszty wody ({BillingIntervalTicks} ticków): {onSprinklers} aktywnych zraszaczy"
                ));
            }

            if (energyTotal > 0)
            {
                _state.Finance.Apply(new PurchaseTransaction(
                    new Money(energyTotal, "PLN"),
                    TransactionCategory.Energy,
                    $"Koszty energii ({BillingIntervalTicks} ticków): {onLamps} aktywnych lamp UV"
                ));
            }

            _state.Logger?.Invoke($"[FINANSE] Rozliczono koszty operacyjne za {BillingIntervalTicks} ticków (woda: {waterTotal:0.00} PLN, energia: {energyTotal:0.00} PLN).");
        }



        public event Action<FarmState>? TickHappened;


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

            // Inicjalizacja silnika finansowego
            EnsureFinanceInitialized();

            // Inicjalizacja marketu
            Market = new Market(_state, _logger);

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
            Tick(); // Wykonaj logikę

            // --- DASHBOARD KONSOLOWY ---
            Console.Clear();
            Console.WriteLine("==============================================");
            Console.WriteLine($"   AGRO-BIZNES SYMULACJA | Tura: {_state.CurrentTick}");
            Console.WriteLine("==============================================");

            // Finanse
            Console.WriteLine($"[FINANSE] Saldo: {_state.Finance.Account.Balance}");


            // 1. Środowisko
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[GLEBA]  Wilgotność: {_state.SoilMoisture:F1}%");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[UV] Poziom:     {_state.LightLevel}%");
            Console.ResetColor();
            Console.WriteLine("----------------------------------------------");

            // 2. Maszyny
            int onSprinklers = _state.Sprinklers.Count(x => x.IsOn);
            int onLamps = _state.Solars.Count(x => x.IsOn); // UV

            Console.Write("MASZYNY: ");
            if (onSprinklers > 0) Console.Write($"ZRASZACZE ON ({onSprinklers}/{_state.Sprinklers.Count})  ");
            else Console.Write($"   Zraszacze OFF ({_state.Sprinklers.Count})  ");

            if (onLamps > 0) Console.Write($"LAMPY UV ON ({onLamps}/{_state.Solars.Count})");
            else Console.Write($"   Lampy UV OFF ({_state.Solars.Count})");
            Console.WriteLine("\n----------------------------------------------");

            // 3. Rośliny (Statystyka zbiorcza)
            if (_state.Tomatoes.Count > 0)
            {
                double avgWoda = _state.Tomatoes.Average(t => t.PoziomNawodnienia);
                double avgWzrost = _state.Tomatoes.Average(t => t.PoziomWzrostu);
                int dojrzale = _state.Tomatoes.Count(t => t.IsMature);
                int martwe = _state.Tomatoes.Count(t => t.IsDead); // Powinny zniknąć, ale na ułamek sekundy może złapie

                Console.WriteLine($"[POMIDORY] Ilość: {_state.Tomatoes.Count} szt.");
                Console.WriteLine($" - Średnie Nawodnienie: {avgWoda:F1}%");
                Console.WriteLine($" - Średni Wzrost:       {avgWzrost:F1}%");
                Console.WriteLine($" - Gotowe do zbioru:    {dojrzale} szt.");

                // Ostrzeżenie
                if (avgWoda < 20)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("!!! ALARM: ROŚLINY USYCHAJĄ !!!");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("BRAK ŻYWYCH ROŚLIN - KONIEC GRY?");
                Console.ResetColor();
            }

            Console.WriteLine("==============================================");
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
            if (_state.Tomatoes.Count == 0 && _state.Roses.Count==0 && _state.Cactile.Count==0 && _state.Apples.Count==0)
            {
                _state.SoilMoisture = 100;
                Random rnd = new Random();
                _state.LightLevel = rnd.Next(30, 90);
                _logger.AddLog("[agro.Logic]: Wykryto pustą farmę. Tworzenie pakietu startowego...");
                for (int i = 0; i < 40; i++)
                {
                    var p = new Tomato();
                    p.PoziomNawodnienia = 60;
                    p.PoziomNaslonecznienia = 60;
                    p.PoziomWzrostu = 0;
                    _state.Tomatoes.Add(p);
                }
                // 2. MASZYNY
                // 5 Zraszaczy (rozstawione teoretycznie po polu)
                for (int i = 0; i < 5; i++) _state.Sprinklers.Add(new Sprinkler() { IsOn = false });

                // 3 Lampy UV (dawne Solary)
                for (int i = 0; i < 3; i++) _state.Solars.Add(new Solar() { IsOn = false });

                // 3. AUTOMATYKA
                // 1 Sensor (Smart Hub), który tym wszystkim steruje
                _state.Sensors.Add(new Sensor());

                // JAREK DODAJ TWORZENIE POCZĄTKOWYCH PIENIĘDZY NA KONCIE

                GameSaver.SaveGame(_state);
            }
        }

        // --- GŁÓWNA PĘTLA (TICK) ---
        public void Tick()
        {
            _state.CurrentTick++; // Zwiększenie ticka
            // Gleba sama wysycha (w zależności od ilości roślin) i zmniejsza się poziom UV
            _state.SoilMoisture -= 0.1*(_state.Tomatoes.Count+_state.Roses.Count+_state.Apples.Count);
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

            // 2. Naliczanie kosztów operacyjnych (przed wykonaniem ticków obiektów)
            //     Dzięki temu naliczenie uwzględnia stan urządzeń na początku tury
            //     (np. test oczekuje obciążenia gdy urządzenia były włączone przed tickiem).
            ApplyOperatingCostsIfDue();

            // 3. Wykonujemy logikę obiektów
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

            //Aktualizacja stanu konta w FarmState
            _state.BalanceAmount = _state.Finance.Account.Balance.Amount;
            _state.BalanceCurrency = _state.Finance.Account.Balance.Currency;

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

            TickHappened?.Invoke(_state); //reagowanie GUI
        }

    } 
} 