using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core.Economy
{
    public enum TransactionCategory
    {
        Energy,
        Water,
        Seeds,
        Sales,
        Fine,
        Other
    }
    public enum TransactionType
    {
        Purchase,
        Sale,
        Penalty
    }
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
