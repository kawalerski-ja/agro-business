using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core
{
    public class Rose : Rosliny
    {
        public Rose():base("Róża",TypRosliny.Kwiat) {
            Cena = 100;
            CenaSprzedazy = 123;
        }
        protected override void DoSpecificGrowth()
        {
            PoziomWzrostu += 10;
        }
    }
}
