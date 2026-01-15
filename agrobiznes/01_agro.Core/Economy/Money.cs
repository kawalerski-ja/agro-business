using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core.Economy
{
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
