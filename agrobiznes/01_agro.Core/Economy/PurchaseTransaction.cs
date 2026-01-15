using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core.Economy
{
    public class PurchaseTransaction : Transaction
    {
        public override TransactionType Type => TransactionType.Purchase;

        public PurchaseTransaction(Money amount, TransactionCategory category, string description)
            : base(amount, category, description)
        {
        }
    }
}
