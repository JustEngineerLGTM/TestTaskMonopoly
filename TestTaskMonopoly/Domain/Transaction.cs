using TestTaskMonopoly.Models;
namespace TestTaskMonopoly.Domain;

public class Transaction
{

    private Transaction(DateTime date, decimal amount, TransactionType type, string? description, Guid id)
    {
        Date = date;
        Amount = amount;
        TransactionType = type;
        Description = description;
        Id = id;
    }

    public Guid Id { get; }
    public DateTime Date { get; private set; }

    public decimal Amount { get; private set; }

    public TransactionType TransactionType { get; private set; }
    public string? Description { get; private set; }

    public static Transaction Create(DateTime date, decimal amount, TransactionType type, string? description)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Сумма транзакции должна быть больше 0");

        return new Transaction(date, amount, type, description, Guid.NewGuid());
    }
}