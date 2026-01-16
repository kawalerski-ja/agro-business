using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core.Economy
{
    /// <summary>
    /// Transakcja typu kara lub koszt nadzwyczajny.
    /// Zmniejsza saldo konta.
    /// </summary>

    public class PenaltyTransaction: Transaction
    {
        public override TransactionType Type => TransactionType.Penalty;

        public PenaltyTransaction(Money amount, TransactionCategory category, string description)
            : base(amount, category, description)
        {
        }
    }
}
