using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace _04_agro.GUI
{
    // Logika okna głównego (code-behind)
    public partial class MainWindow : Window
    {
        private const int Rows = 10;
        private const int Cols = 10;

        // Stany pól (minimalny "model" w GUI, bez ruszania logiki projektu)
        private readonly bool[,] _planted = new bool[Rows, Cols];
        private readonly int[,] _water = new int[Rows, Cols]; // 0..100 (umownie)

        // Stan konta (prosty model)
        private int _balance = 1000;

        // Zaznaczone pole (do sprzedaży)
        private (int row, int col)? _selectedField = null;

        // Style ramki zaznaczenia
        private static readonly Brush NormalBorderBrush = BrushFromHex("#5A5A5A");
        private static readonly Thickness NormalBorderThickness = new Thickness(1);

        private static readonly Brush SelectedBorderBrush = BrushFromHex("#9CDCFE"); // niebieski
        private static readonly Thickness SelectedBorderThickness = new Thickness(2);

        private DispatcherTimer _guiTimer;

        public MainWindow()
        {
            InitializeComponent();

            AddLog("[INFO] MainWindow działa");

            InitializeFarmGrid();
            UpdateBalanceText();
            UpdateStatsFromGrid();

            _guiTimer = new DispatcherTimer();
            _guiTimer.Interval = TimeSpan.FromSeconds(1);
            _guiTimer.Tick += GuiTick;
            _guiTimer.Start();

            AddLog("[INFO] GUI Tick uruchomiony (1s)");
        }

        // Tworzy siatkę 10x10 przycisków (pól uprawnych)
        private void InitializeFarmGrid()
        {
            if (FarmGrid == null) return;

            FarmGrid.Children.Clear();

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    var fieldButton = new Button
                    {
                        Content = "",
                        Tag = (row, col), // zapis współrzędnych pola
                        Margin = new Thickness(2),
                        Background = BrushFromHex("#252526"),
                        BorderBrush = NormalBorderBrush,
                        BorderThickness = NormalBorderThickness
                    };

                    fieldButton.Click += FarmField_Click;

                    FarmGrid.Children.Add(fieldButton);
                }
            }

            AddLog("[INFO] Zainicjalizowano siatkę pól 10x10");
        }

        // Klik pola:
        // - puste pole -> zasiew
        // - zasiane pole -> zaznaczenie do sprzedaży
        private void FarmField_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button) return;

            var (row, col) = ((int, int))button.Tag;

            // Puste pole -> zasiew
            if (!_planted[row, col])
            {
                _planted[row, col] = true;
                _water[row, col] = 50;

                button.Background = BrushFromHex("#2E7D32"); // zielony
                AddLog($"[INFO] Zasiano pole ({row}, {col})");

                FlashButton(button);
                UpdateStatsFromGrid();
                return;
            }

            // Zasiane pole -> tylko zaznacz do sprzedaży (bez czyszczenia!)
            _selectedField = (row, col);
            AddLog($"[INFO] Zaznaczono pole ({row}, {col}) do sprzedaży");

            RefreshSelectionVisual();
            FlashButton(button);
        }

        // Efekt "mrugnięcia" po kliknięciu
        private void FlashButton(Button button)
        {
            var original = button.Background;
            button.Background = BrushFromHex("#90CAF9"); // jasny niebieski

            var t = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(120) };
            t.Tick += (_, __) =>
            {
                t.Stop();
                button.Background = original;
            };
            t.Start();
        }

        // Statystyki liczone z siatki
        private void UpdateStatsFromGrid()
        {
            int total = Rows * Cols;
            int plantedCount = 0;
            int plantedWaterSum = 0;

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (_planted[r, c])
                    {
                        plantedCount++;
                        plantedWaterSum += _water[r, c];
                    }
                }
            }

            // Średni "wzrost" = procent zasianych pól (0..100)
            double growthPercent = (double)plantedCount / total * 100.0;

            // Średnie nawodnienie = średnia dla zasianych pól
            double avgWater = plantedCount > 0
                ? (double)plantedWaterSum / plantedCount
                : 0.0;

            if (GrowthBar != null) GrowthBar.Value = Clamp0_100(growthPercent);
            if (WaterBar != null) WaterBar.Value = Clamp0_100(avgWater);
        }

        // Klik: Kup -> podlewa wszystkie zasiane pola (+10)
        private void Buy_Click(object sender, RoutedEventArgs e)
        {
            const int price = 50;

            if (_balance < price)
            {
                AddLog("[WARN] Brak środków na zakup");
                return;
            }

            _balance -= price;

            WaterAllPlanted(10);

            UpdateBalanceText();
            UpdateStatsFromGrid();
            AddLog($"[INFO] Kliknięto Kup (-{price} zł) | Podlano zasiane pola (+10)");
        }

        // Klik: Sprzedaj -> sprzedaje TYLKO zaznaczone pole
        private void Sell_Click(object sender, RoutedEventArgs e)
        {
            const int incomePerField = 30;

            if (_selectedField == null)
            {
                AddLog("[WARN] Najpierw zaznacz zasiane pole (kliknij zielone pole)");
                return;
            }

            var (row, col) = _selectedField.Value;

            if (!_planted[row, col])
            {
                AddLog("[WARN] Zaznaczone pole jest puste – nie ma czego sprzedać");
                _selectedField = null;
                RefreshSelectionVisual();
                return;
            }

            // Sprzedaż = wyczyszczenie pola
            _planted[row, col] = false;
            _water[row, col] = 0;

            _balance += incomePerField;

            // Czyścimy zaznaczenie po sprzedaży
            _selectedField = null;

            RefreshGridColors();
            RefreshSelectionVisual();
            UpdateBalanceText();
            UpdateStatsFromGrid();

            AddLog($"[INFO] Sprzedano pole ({row}, {col}) (+{incomePerField} zł)");
        }

        // Podlewa wszystkie zasiane pola
        private void WaterAllPlanted(int amount)
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (_planted[r, c])
                    {
                        _water[r, c] = (int)Clamp0_100(_water[r, c] + amount);
                    }
                }
            }
        }

        // Odświeża tło wszystkich pól na podstawie stanu
        private void RefreshGridColors()
        {
            if (FarmGrid == null) return;

            foreach (var child in FarmGrid.Children)
            {
                if (child is not Button btn) continue;
                var (row, col) = ((int, int))btn.Tag;

                btn.Background = _planted[row, col]
                    ? BrushFromHex("#2E7D32")
                    : BrushFromHex("#252526");
            }
        }

        // Podświetla zaznaczone pole (ramka) i czyści ramki na innych
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

        // Aktualizacja napisu stanu konta
        private void UpdateBalanceText()
        {
            if (BalanceText == null) return;
            BalanceText.Text = $"Stan konta: {_balance} zł";
        }

        // Dodaje wpis do panelu logów
        private void AddLog(string message)
        {
            if (LogList == null) return;

            LogList.Items.Add(message);
            LogList.ScrollIntoView(LogList.Items[LogList.Items.Count - 1]);
        }

        // Helpers
        private static SolidColorBrush BrushFromHex(string hex)
        {
            return (SolidColorBrush)new BrushConverter().ConvertFromString(hex);
        }

        private static double Clamp0_100(double v)
        {
            if (v < 0) return 0;
            if (v > 100) return 100;
            return v;
        }
        private void GuiTick(object? sender, EventArgs e)
        {
            bool changed = false;

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (_planted[r, c] && _water[r, c] > 0)
                    {
                        _water[r, c]--; // gleba wysycha w czasie
                        changed = true;
                    }
                }
            }

            if (changed)
                UpdateStatsFromGrid();
        }
    }
}
