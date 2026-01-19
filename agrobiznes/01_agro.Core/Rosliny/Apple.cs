using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core
{
    public class Apple:Rosliny
    {
        public Apple():base("Jabłoń",TypRosliny.Owoc) { }

        protected override void DoSpecificGrowth()
        {
            PoziomWzrostu += 1;
        }
    }
}
