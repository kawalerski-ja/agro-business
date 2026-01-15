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
    abstract public class Rosliny
    {
        string nazwa;
        float poziom_wzrostu; // 1-100?
        float poziom_nawodnienia; // 
        float cena;
        TypRosliny TypRosliny;
    }
}
