using System;
using System.Collections.Generic;
using System.Linq;
using _01_agro.Core;
using _02_agro.Data;

namespace _03_agro.Logic
{
    public class Market
    {
        private readonly FarmState _state;
        private readonly LogRepo _logger;

        public Market(FarmState state, LogRepo logger)
        {
            _state = state;
            _logger = logger;
        }

        // ==========================================
        // 1. KUPOWANIE (PROSTE)
        // ==========================================

        public string KupPomidory(int ilosc)
        {
            // Tworzymy "prototyp", żeby sprawdzić aktualną cenę w tym sezonie
            var wzorzec = new Tomato();
            float koszt = wzorzec.Cena * ilosc; // "Cena" to Twoja cena zakupu z klasy Roslina

            // [MIEJSCE NA IF FINANSOWY] 
            // if (state.Account.Balance < koszt) return "Brak środków";

            // --- LOGIKA ZAKUPU ---
            // _state.Account.Withdraw(koszt); // TODO: Odkomentować jak będą finanse

            for (int i = 0; i < ilosc; i++)
            {
                // Tworzymy nową sztukę
                var t = new Tomato();

                _state.Tomatoes.Add(t);
            }

            string msg = $"SKLEP: Kupiono {ilosc} pomidorów. Koszt: {koszt:C}";
            _logger.AddLog(msg);
            return msg;
        }

        public string KupJablka(int ilosc)
        {
            var wzorzec = new Apple();
            float koszt = wzorzec.Cena * ilosc;
            for (int i = 0; i < ilosc; i++)
            {
                // Tworzymy nową sztukę
                var t = new Apple();

                _state.Apples.Add(t);
            }
            string msg = $"SKLEP: Kupiono {ilosc} jabłek. Koszt: {koszt:C}";
            _logger.AddLog(msg);
            return msg;
        }

        public string KupKaktusy(int ilosc)
        {
            var wzorzec = new Cactus();
            float koszt = wzorzec.Cena * ilosc;
            for (int i = 0; i < ilosc; i++)
            {
                // Tworzymy nową sztukę
                var t = new Cactus();

                _state.Cactile.Add(t);
            }
            string msg = $"SKLEP: Kupiono {ilosc} kaktusów. Koszt: {koszt:C}";
            _logger.AddLog(msg);
            return msg;
        }

        public string KupRóże(int ilosc)
        {
            var wzorzec = new Rose();
            float koszt = wzorzec.Cena * ilosc;
            for (int i = 0; i < ilosc; i++)
            {
                // Tworzymy nową sztukę
                var t = new Rose();

                _state.Roses.Add(t);
            }
            string msg = $"SKLEP: Kupiono {ilosc} róż. Koszt: {koszt:C}";
            _logger.AddLog(msg);
            return msg;
        }

        // ==========================================
        // 2. SPRZEDAŻ (BARDZO PROSTA)
        // ==========================================

        public string SprzedajWszystko()
        {
            float zarobekCalkowity = 0;
            int iloscCalkowita = 0;

            

            var wynikPomidory = SprzedajZListy(_state.Tomatoes);
            zarobekCalkowity += wynikPomidory.zarobek;
            iloscCalkowita += wynikPomidory.ilosc;

            var wynikJablka = SprzedajZListy(_state.Apples);
            zarobekCalkowity += wynikJablka.zarobek; 
            iloscCalkowita += wynikJablka.ilosc;

            var wynikKaktusy = SprzedajZListy(_state.Cactile);
            zarobekCalkowity += wynikKaktusy.zarobek;
            iloscCalkowita += wynikKaktusy.ilosc;

            var wynikRoze = SprzedajZListy(_state.Roses);
            zarobekCalkowity += wynikRoze.zarobek;
            iloscCalkowita += wynikRoze.ilosc;

            if (iloscCalkowita > 0)
            {
                // --- WPŁATA NA KONTO ---
                // _state.Account.Deposit(zarobekCalkowity); // TODO

                string msg = $"SKUP: Sprzedano {iloscCalkowita} roślin za {zarobekCalkowity:C}.";
                _logger.AddLog(msg);
                return msg;
            }

            return "SKUP: Magazyn pusty (brak dojrzałych roślin).";
        }

        // ==========================================
        // 3. SILNIK SPRZEDAŻY (GENERYCZNY)
        // ==========================================

        // Ta metoda przyjmuje dowolną listę roślin (T : Roslina)
        private (int ilosc, float zarobek) SprzedajZListy<T>(List<T> listaRoslin) where T : Rosliny
        {
            // 1. Wybierz te do sprzedania
            var doSprzedania = listaRoslin.Where(r => r.IsMature && !r.IsDead).ToList();

            if (doSprzedania.Count == 0) return (0, 0);

            // 2. Policz zysk (Suma cen sprzedaży konkretnych obiektów)
            // Dzięki temu, że cena jest w roślinie, to działa automatycznie!
            float zysk = doSprzedania.Sum(r => r.CenaSprzedazy);

            // 3. Usuń fizycznie z farmy
            listaRoslin.RemoveAll(r => r.IsMature && !r.IsDead);

            return (doSprzedania.Count, zysk);
        }

        // ==========================================
        // 4. KUPOWANIE MASZYN
        // ==========================================

        public string KupZraszacz()
        {
            var nowaMaszyna = new Sprinkler();
            return KupMaszyneKonkretna(nowaMaszyna, _state.Sprinklers);
        }

        public string KupPanelSloneczny()
        {
            var nowaMaszyna = new Solar();
            return KupMaszyneKonkretna(nowaMaszyna, _state.Solars);
        }

        // --- POMOCNICZA METODA DLA MASZYN ---
        
        private string KupMaszyneKonkretna<T>(T maszyna, List<T> listaDocelowa) where T : Device
        {
            float koszt = maszyna.Cena;

            // 1. Walidacja finansowa (Zostawiam miejsce)
            // if (_state.GameAccount.Balance.Amount < koszt) 
            //      return $"BŁĄD: Maszyna kosztuje {koszt:C}, a masz {_state.GameAccount.Balance.Amount:C}";

            // 2. Pobranie pieniędzy (TODO)
            // _state.GameAccount.Withdraw(koszt); 

            // 3. Dodanie do farmy
            listaDocelowa.Add(maszyna);

            string msg = $"SKLEP: Zakupiono maszynę: {maszyna.Name}. Koszt: {koszt:C}";
            _logger.AddLog(msg);
            return msg;
        }
    }
}