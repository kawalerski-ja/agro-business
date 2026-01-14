using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;


namespace _02_agro.Data
{
    public class FarmContext : DbContext
    {

        // Tabela z logami
        public DbSet<SystemLog> SystemLogs { get; set; }
    }
}
