using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace _01_agro.Core.Economy
{
    /// <summary>
    /// Obiekt wartości reprezentujący kwotę pieniężną.
    /// Używany w całym module ekonomii (saldo, transakcje, raporty).
    /// </summary>
    [ComplexType]
    public class Money
    {
        [JsonInclude]
        public decimal Amount { get; set; }
        [JsonInclude]
        public string Currency { get; set; }

  
        public Money(decimal amount, string currency)
        {
            if (amount < 0m)
                throw new System.ArgumentOutOfRangeException(nameof(amount));
            Amount = amount;
            Currency = currency;
        }
        public Money(decimal amount) : this(amount, "PLN") { }

        public Money() { }

        public override string ToString()
        {
            return $"{Amount:0.00} {Currency}";
        }
    }
}
