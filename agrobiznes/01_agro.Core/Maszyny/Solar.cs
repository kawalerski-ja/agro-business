using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core
{
    public class Solar : Device
    {
        public Solar()
        {
            Name = "Lampa UV";
        }
        public override void Tick(FarmState state)
        {
            // Jeśli maszyna jest włączona...
            if (IsOn)
            {
                // 1. Zwiększ poziom UV
                state.LightLevel += 5.0;

                // 2. Zabezpieczenie
                if (state.LightLevel > 100) state.LightLevel = 100;

                // 3. Pobierz opłatę za prąd

            }
            else
            {
                // Naświetlenie UV samo się zmniejsza

                state.LightLevel -= 1.0;
                if (state.LightLevel < 0) state.LightLevel = 0;
            }
        }
    }
}
