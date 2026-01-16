using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core.Economy
{
    /// <summary>
    /// Podatek liniowy.
    /// </summary>

    public class FlatTax: ITax
    {
        public decimal Rate { get; set; }

        public Money CalculateTax(FinancialPeriod period)
        {
            throw new NotImplementedException();
        }
    }
}
