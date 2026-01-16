using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core.Economy
{
    /// <summary>
    /// Konto finansowe systemu.
    /// Przechowuje aktualne saldo farmy.
    /// </summary>

    public class Account
    {
        public Money Balance { get; private set; }
        public event Action<Money>? BalanceChanged;

        public Account(Money initialBalance)
        {
            Balance = initialBalance;
        }

        public void Credit(Money amount)
        {
            Balance = new Money(Balance.Amount + amount.Amount, Balance.Currency);
            BalanceChanged?.Invoke(Balance);
        }
        public void Debit(Money amount)
        {
            if (Balance.Amount > amount.Amount)
            {
                if (amount.Amount > Balance.Amount)
                    throw new InvalidOperationException("Brak środków");

                Balance = new Money(Balance.Amount - amount.Amount, Balance.Currency);
                BalanceChanged?.Invoke(Balance);
            }

        }
    }
}
