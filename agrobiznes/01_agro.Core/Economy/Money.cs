using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core.Economy
{
    /// <summary>
    /// Obiekt wartości reprezentujący kwotę pieniężną.
    /// Używany w całym module ekonomii (saldo, transakcje, raporty).
    /// </summary>

    public readonly struct Money
    {
        public decimal Amount { get; }
        public string Currency { get; }

  
        public Money(decimal amount, string currency)
        {
            if (amount < 0m)
                throw new ArgumentOutOfRangeException(nameof(amount));

            Amount = amount;
            Currency = "PLN";
        }
        public override string ToString()
        {
            return $"{Amount:0.00} {Currency}";
        }
    }
}
