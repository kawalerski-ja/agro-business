using System;
using _03_agro.Logic; 

class Program
{
    static void Main(string[] args)
    {
        // Tworzymy i uruchamiamy silnik
        SimulationEngine engine = new SimulationEngine();
        engine.StartSimulation();

        // Blokujemy zamknięcie okna
        Console.ReadLine();

        // Sprzątamy przy wyjściu
        engine.StopSimulation();
    }
}