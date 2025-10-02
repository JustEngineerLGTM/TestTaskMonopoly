using TestTaskMonopoly.Models;
namespace TestTaskMonopoly.Domain;
    public class Transaction
    {
        public Guid  Id { get; }
        public DateTime Date { get; private set; }
        
        public decimal Amount {get; private set;}

        public TransactionType TransactionType { get;private set; }
        public string? Description { get; private set; }

        private Transaction(DateTime date, decimal amount, TransactionType type, string? description, Guid id)
        {
            Date = date;
            Amount = amount;
            TransactionType = type;
            Description = description;
            Id = id;
        }
        private Transaction(Transaction transaction)
        {
            this.Id = transaction.Id;
            this.Date = transaction.Date;
            this.Amount = transaction.Amount;
            this.TransactionType = transaction.TransactionType;
            this.Description = transaction.Description;
        }

        public static Transaction Create(DateTime date, decimal amount, TransactionType type, string? description)
        {
            return new Transaction(date, amount, type, description, Guid.NewGuid());
        }
        public static Transaction Create(Transaction transaction)
        {
            return new Transaction(transaction);
        }


    }
