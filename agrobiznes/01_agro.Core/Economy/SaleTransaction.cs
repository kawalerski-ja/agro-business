using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core.Economy
{
    /// <summary>
    /// Transakcja typu sprzedaż (przychód).
    /// Zwiększa saldo konta.
    /// </summary>

    public class SaleTransaction: Transaction
    {
        public override TransactionType Type => TransactionType.Sale;

        public SaleTransaction(Money amount, TransactionCategory category, string description)
            : base(amount, category, description)
        {
        }
    }
}
