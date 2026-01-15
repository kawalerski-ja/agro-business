using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core.Economy
{
    public class FinanceEngine
    {
        public Account Account { get; }
        public ITax Tax { get; set; }
        public IReadOnlyList<Transaction> Transactions { get; }

    }
}
