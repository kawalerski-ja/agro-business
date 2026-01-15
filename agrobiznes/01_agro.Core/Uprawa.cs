using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core
{
    internal class Uprawa : Rosliny, ITickable
    {
        float poziom_wzrostu; // 1-100?
        float poziom_nawodnienia; //1-100%
        float poziom_naslonecznienia; // 1-100%?
        float wzrost_na_tick;
        float odwodnienie_na_tick;
    }
}
