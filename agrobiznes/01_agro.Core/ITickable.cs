using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core
{
    public interface ITickable
    {
        // Metoda wywoływana przez silnik w każdej klatce symulacji
        void Tick(FarmState state);
    }
}
