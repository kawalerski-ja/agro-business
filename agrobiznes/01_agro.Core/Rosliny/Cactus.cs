using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core
{
    public class Cactus : Rosliny
    {
        public Cactus():base("Kaktus",TypRosliny.Sukulent) {
            Cena = 20;
            CenaSprzedazy = 40;
        }
        protected override void DoSpecificGrowth()
        {
            PoziomWzrostu += 5;
        }
    }
}
