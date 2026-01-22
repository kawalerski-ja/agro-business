using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using _03_agro.Logic;
using _01_agro.Core;

namespace _04_agro.GUI
{
    public partial class MainWindow : Window
    {
        private const int Rows = 10;
        private const int Cols = 10;

        private SimulationEngine _engine;

        private (int row, int col)? _selectedField;

        // быстрый доступ: (row,col) -> Button
        private readonly Dictionary<(int row, int col), Button> _cellButtons = new();

        // log throttling
        private DateTime _lastLogRefresh = DateTime.MinValue;
        private static readonly TimeSpan LogRefreshInterval = TimeSpan.FromSeconds(2.5);
        private int _lastLogCount = -1;

        // Style ramki zaznaczenia
        private static readonly Brush NormalBorderBrush = BrushFromHex("#5A5A5A");
        private static readonly Thickness NormalBorderThickness = new Thickness(1);

        private static readonly Brush SelectedBorderBrush = BrushFromHex("#9CDCFE");
        private static readonly Thickness SelectedBorderThickness = new Thickness(2);

        private enum PlantType
        {
            Tomato,
            Rose,
            Cactus
        }

        public MainWindow()
        {
            InitializeComponent();

            InitializeFarmGrid();

            _engine = new SimulationEngine();
            _engine.TickHappened += OnEngineTick;
            _engine.StartSimulation();

            _engine.LoggerRepo.AddLog("[GUI] Uruchomiono okno MainWindow.");

            RenderFromEngine();
            LoadLogsFromEngine(force: true);
        }

        // =========================
        //  GRID (10x10)
        // =========================
        private void InitializeFarmGrid()
        {
            if (FarmGrid == null) return;

            FarmGrid.Children.Clear();
            _cellButtons.Clear();

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    var btn = new Button
                    {
                        Content = "",
                        Tag = (row, col),
                        Margin = new Thickness(2),
                        Background = BrushFromHex("#252526"),
                        BorderBrush = NormalBorderBrush,
                        BorderThickness = NormalBorderThickness
                    };

                    btn.Click += FarmField_Click;

                    FarmGrid.Children.Add(btn);
                    _cellButtons[(row, col)] = btn;
                }
            }
        }

        private void FarmField_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button) return;

            _selectedField = ((int, int))button.Tag;
            RefreshSelectionVisual();

            var (r, c) = _selectedField.Value;
            _engine.LoggerRepo.AddLog($"[GUI] Zaznaczono pole ({r},{c}).");
            // throttling в tick

        }

        private void RefreshSelectionVisual()
        {
            if (FarmGrid == null) return;

            foreach (var child in FarmGrid.Children)
            {
                if (child is not Button btn) continue;

                btn.BorderBrush = NormalBorderBrush;
                btn.BorderThickness = NormalBorderThickness;

                if (_selectedField.HasValue)
                {
                    var (r, c) = ((int, int))btn.Tag;
                    if (r == _selectedField.Value.row && c == _selectedField.Value.col)
                    {
                        btn.BorderBrush = SelectedBorderBrush;
                        btn.BorderThickness = SelectedBorderThickness;
                    }
                }
            }
        }

        // =========================
        //  NEW GAME
        // =========================
        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show(
                "Czy na pewno rozpocząć nową grę?\nObecny zapis zostanie usunięty.",
                "Nowa gra",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (res != MessageBoxResult.Yes)
                return;

            try
            {
                // 1) ostatnić stary silnik
                _engine?.StopSimulation();

                // 2) zresetowac savegame.json
                const string savePath = "savegame.json";
                if (System.IO.File.Exists(savePath))
                    System.IO.File.Delete(savePath);

                // 3) wyczyścić zaznaczenie
                _selectedField = null;
                RefreshSelectionVisual();

                // 4) zrozpocząć nowy silnik
                _engine = new SimulationEngine();
                _engine.TickHappened += OnEngineTick;
                _engine.StartSimulation();

                _engine.LoggerRepo.AddLog("[GUI] Rozpoczęto NOWĄ GRĘ (zresetowano zapis).");

                // 5) odnowic UI
                RenderFromEngine();
                LoadLogsFromEngine(force: true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas tworzenia nowej gry: " + ex.Message);
            }
        }

        // =========================
        //  BUY
        // =========================
        private void BuyTomato_Click(object sender, RoutedEventArgs e) => BuySelectedField(PlantType.Tomato);
        private void BuyRose_Click(object sender, RoutedEventArgs e) => BuySelectedField(PlantType.Rose);
        private void BuyCactus_Click(object sender, RoutedEventArgs e) => BuySelectedField(PlantType.Cactus);

        private void BuySelectedField(PlantType type)
        {
            if (_selectedField == null)
            {
                _engine.LoggerRepo.AddLog("[GUI] Najpierw zaznacz pole, potem kup roślinę.");
                return;
                RenderGridFromEngine();

            }

            var (row, col) = _selectedField.Value;

            if (_engine.IsOccupied(row, col))
            {
                _engine.LoggerRepo.AddLog($"[GUI] Pole ({row},{col}) jest zajęte.");
                return;
            }

            float cost = type switch
            {
                PlantType.Tomato => 50f,
                PlantType.Rose => 80f,
                PlantType.Cactus => 40f,
                _ => 50f
            };

            string plantName = type.ToString();

            if (!_engine.Market.TryBuyPlant(cost, plantName, out string msg))
            {
                _engine.LoggerRepo.AddLog($"[GUI] Zakup nieudany: {msg}");
                return;
            }

            if (!_engine.PlantAt(row, col, plantName))
            {
                return; // PlantAt samo loguje
            }

            RenderFromEngine();
        }

        // =========================
        //  SELL (from selected field)
        // =========================
        private void Sell_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedField == null)
            {
                _engine.LoggerRepo.AddLog("[GUI] Sprzedaj: najpierw zaznacz pole.");
                return;
                RenderGridFromEngine();

            }

            var (row, col) = _selectedField.Value;

            if (_engine.Market.TrySellAt(row, col, out string msg))
            {
                _engine.LoggerRepo.AddLog(msg);
                RenderFromEngine();
                return;
            }

            _engine.LoggerRepo.AddLog(msg);
        }

        // =========================
        //  ENGINE TICK -> UI
        // =========================
        private void OnEngineTick(FarmState state)
        {
            Dispatcher.Invoke(() =>
            {
                
                UpdateStatsOnly(state);

                
                if (DateTime.Now - _lastLogRefresh >= LogRefreshInterval)
                {
                    _lastLogRefresh = DateTime.Now;
                    LoadLogsFromEngine(force: false);
                }
            });
        }


        private void RenderFromEngine()
        {
            var state = _engine.State;

            if (TickText != null)
                TickText.Text = $"Tura: {state.CurrentTick}";

            if (BalanceText != null)
                BalanceText.Text = $"Stan konta: {state.Finance.Account.Balance}";

            // średnie dla wszystkich roślin
            var allPlants = state.Tomatoes.Cast<Rosliny>()
                .Concat(state.Roses)
                .Concat(state.Cactile)
                .Concat(state.Apples)
                .ToList();

            double avgWater = allPlants.Count > 0 ? allPlants.Average(p => p.PoziomNawodnienia) : 0;
            double avgGrowth = allPlants.Count > 0 ? allPlants.Average(p => p.PoziomWzrostu) : 0;

            if (WaterBar != null) WaterBar.Value = Clamp0_100(avgWater);
            if (GrowthBar != null) GrowthBar.Value = Clamp0_100(avgGrowth);

            RenderGridFromEngine();
            
        }

        private void RenderGridFromEngine()
        {
            if (FarmGrid == null) return;

            // 1) reset wszystkich pól
            foreach (var btn in _cellButtons.Values)
                btn.Background = BrushFromHex("#252526");

            // 2) TOMATO
            foreach (var t in _engine.State.Tomatoes)
            {
                PaintCell(
                    t.Row,
                    t.Col,
                    t.IsMature ? "#FFD600" : "#2E7D32" // 🟨 зрелый / 🟩 растёт
                );
            }

            // 3) ROSE
            foreach (var r in _engine.State.Roses)
            {
                PaintCell(
                    r.Row,
                    r.Col,
                    r.IsMature ? "#FFD600" : "#AD1457"
                );
            }

            // 4) CACTUS
            foreach (var c in _engine.State.Cactile)
            {
                PaintCell(
                    c.Row,
                    c.Col,
                    c.IsMature ? "#FFD600" : "#558B2F"
                );
            }

            // 5) APPLE
            foreach (var a in _engine.State.Apples)
            {
                PaintCell(
                    a.Row,
                    a.Col,
                    a.IsMature ? "#FFD600" : "#1565C0"
                );
            }
        }


        private void PaintCell(int row, int col, string hex)
        {
            if (_cellButtons.TryGetValue((row, col), out var btn))
                btn.Background = BrushFromHex(hex);
        }


        private void LoadLogsFromEngine(bool force)
        {
            if (LogList == null) return;

            var logs = _engine.LoggerRepo.GetLogs(200);

            if (!force && logs.Count <= _lastLogCount)
                return;

            _lastLogCount = logs.Count;

            LogList.Items.Clear();
            foreach (var line in logs)
                LogList.Items.Add(line);

            LogList.ScrollIntoView(LogList.Items[^1]);
        }


        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _engine?.StopSimulation();
        }

        // helpers
        private static SolidColorBrush BrushFromHex(string hex)
            => (SolidColorBrush)new BrushConverter().ConvertFromString(hex);

        private static double Clamp0_100(double v)
        {
            if (v < 0) return 0;
            if (v > 100) return 100;
            return v;
        }
        private void UpdateStatsOnly(FarmState state)
        {
            if (TickText != null)
                TickText.Text = $"Tura: {state.CurrentTick}";

            if (BalanceText != null)
                BalanceText.Text = $"Stan konta: {state.Finance.Account.Balance}";

            var allPlants = state.Tomatoes.Cast<Rosliny>()
                .Concat(state.Roses)
                .Concat(state.Cactile)
                .Concat(state.Apples)
                .ToList();

            double avgWater = allPlants.Count > 0 ? allPlants.Average(p => p.PoziomNawodnienia) : 0;
            double avgGrowth = allPlants.Count > 0 ? allPlants.Average(p => p.PoziomWzrostu) : 0;

            if (WaterBar != null) WaterBar.Value = Clamp0_100(avgWater);
            if (GrowthBar != null) GrowthBar.Value = Clamp0_100(avgGrowth);
        }

    }
}
