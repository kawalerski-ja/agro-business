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
    public class Rosliny: ITickable,
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
        public TypRosliny TypRosliny1 { get => TypRosliny; set => TypRosliny = value; }
        public float Poziom_wzrostu { get => poziom_wzrostu; set => poziom_wzrostu = value; }
        public float Poziom_nawodnienia { get => poziom_nawodnienia; set => poziom_nawodnienia = value; }
        public float Poziom_naslonecznienia { get => poziom_naslonecznienia; set => poziom_naslonecznienia = value; }
        public float Cena
        {
            get => cena;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Cena), "Cena nie może być ujemna.");
                cena = value;
            }
        }

        public float Wzrost_na_tick
        {
            get => wzrost_na_tick;
            set
            {
                if (value < 0 || value > 25)
                    throw new ArgumentOutOfRangeException(
                        nameof(Wzrost_na_tick),
                        "Wzrost na tick musi być w zakresie 0–25."
                    );

                wzrost_na_tick = value;
            }
        }

        public float Odwodnienie_na_tick
        {
            get => odwodnienie_na_tick;
            set
            {
                if (value < 0 || value > 25)
                    throw new ArgumentOutOfRangeException(
                        nameof(Odwodnienie_na_tick),
                        "Odwodnienie na tick musi być w zakresie 0–25."
                    );

                odwodnienie_na_tick = value;
            }
        }

        public float Zuzycie_energii_slonecznej_na_tick
        {
            get => zuzycie_energii_slonecznej_na_tick;
            set
            {
                if (value < 0 || value > 25)
                    throw new ArgumentOutOfRangeException(
                        nameof(Zuzycie_energii_slonecznej_na_tick),
                        "Wzrost na tick musi być w zakresie 0–25."
                    );

                zuzycie_energii_slonecznej_na_tick = value;
            }
        }

        public void Tick(FarmState state)
        {
            throw new NotImplementedException();
            poziom_naslonecznienia-=zuzycie_energii_slonecznej_na_tick;
            poziom_nawodnienia-= odwodnienie_na_tick;
            poziom_wzrostu += wzrost_na_tick;

        }
    }
}
