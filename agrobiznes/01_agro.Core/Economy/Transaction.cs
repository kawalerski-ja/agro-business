using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core.Economy
{
    /// <summary>
    /// Kategoria biznesowa transakcji (do raportów).
    /// </summary>

    public enum TransactionCategory
    {
        Energy,
        Water,
        Seeds,
        Sales,
        Fine,
        Other
    }

    /// <summary>
    /// Typ transakcji określający wpływ na saldo.
    /// </summary>

    public enum TransactionType
    {
        Purchase,
        Sale,
        Penalty
    }

    /// <summary>
    /// Abstrakcyjna klasa bazowa dla wszystkich transakcji finansowych.
    /// </summary>

    public abstract class Transaction
    {
        public Guid Id { get; }
        public DateTimeOffset OccurredAt { get; }
        public Money Amount { get; }
        public string Description { get; }
        public TransactionCategory Category { get; }
        public abstract TransactionType Type{ get; }

        public Transaction(Money amount, TransactionCategory category, string description, DateTimeOffset? occurredAt = null)
        {
            Id = Guid.NewGuid();
            OccurredAt = occurredAt ?? DateTimeOffset.UtcNow;
            Amount = amount;
            Category = category;
            Description = description;
        }


    }
}
