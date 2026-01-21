using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02_agro.Data
{
        public class LogRepo
        {
            private readonly List<string> _logs = new();
            public void AddLog(string message)
            {
                    // 1. Otwieramy połączenie (używając using, żeby samo się zamknęło)
                    using (var db = new FarmContext())
                    {
                        // 2. Tworzymy nowy wpis
                        var log = new SystemLog
                        {
                            Message = message,
                            Date = DateTime.Now
                        };

                        db.SystemLogs.Add(log);

                        db.SaveChanges();

                        Console.WriteLine("Zapisano log do bazy!");
                    }
            }
            public List<string> GetLogs()
            {
                 return new List<string>(_logs);
            }
        }
}
