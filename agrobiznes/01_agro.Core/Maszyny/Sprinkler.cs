using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core
{
    public class Sprinkler: Device
    {
        public Sprinkler()
        {
            Name = "Zraszacz ogrodowy";
        }
        public override void Tick(FarmState state)
        {
            // Jeśli maszyna jest włączona...
            if (IsOn)
            {
                // 1. Zwiększ wilgotność gleby (Globalnie)
                state.SoilMoisture += 5.0;

                // 2. Zabezpiecz, żeby nie utopić farmy (max 100%)
                if (state.SoilMoisture > 100) state.SoilMoisture = 100;

                // 3. Pobierz opłatę za wodę/prąd
                
            }
            
        }
    }
}

