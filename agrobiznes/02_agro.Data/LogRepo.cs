using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02_agro.Data
{
    /// <summary>
    /// Klasa LogRepo odpowiada za sam zapis logów do tabeli
    /// </summary>
        public class LogRepo
        {
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
        }
    }
