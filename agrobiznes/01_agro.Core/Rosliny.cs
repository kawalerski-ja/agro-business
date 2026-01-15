using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core
{
    public enum TypRosliny
    {
        Warzywo,
        Owoc,
        Kwiat,
        Sukulent
    }
    public class Rosliny: ITickable
    {
        string nazwa;
        float cena;
        TypRosliny TypRosliny;
        float poziom_wzrostu; // 1-100?
        float poziom_nawodnienia; //1-100%
        float poziom_naslonecznienia; // 1-100%?
        float wzrost_na_tick;
        float odwodnienie_na_tick;
        float zuzycie_energii_slonecznej_na_tick;

        public string Nazwa { get => nazwa; set => nazwa = value; }
        public float Cena { get => cena; set => cena = value; }
        public TypRosliny TypRosliny1 { get => TypRosliny; set => TypRosliny = value; }
        public float Poziom_wzrostu { get => poziom_wzrostu; set => poziom_wzrostu = value; }
        public float Poziom_nawodnienia { get => poziom_nawodnienia; set => poziom_nawodnienia = value; }
        public float Poziom_naslonecznienia { get => poziom_naslonecznienia; set => poziom_naslonecznienia = value; }
        public float Wzrost_na_tick { get => wzrost_na_tick; set => wzrost_na_tick = value; }
        public float Odwodnienie_na_tick { get => odwodnienie_na_tick; set => odwodnienie_na_tick = value; }
        public float Zuzycie_energii_slonecznej_na_tick { get => zuzycie_energii_slonecznej_na_tick; set => zuzycie_energii_slonecznej_na_tick = value; }

        public void Tick(FarmState state)
        {
            throw new NotImplementedException();
            

        }
    }
}
