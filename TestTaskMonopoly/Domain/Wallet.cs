using TestTaskMonopoly.Models;

namespace TestTaskMonopoly.Domain;

public class Wallet
{
    private readonly List<Transaction> _transactions = [];

    private Wallet(Guid id, string name, string currency, decimal initialBalance)
    {
        Id = id;
        Name = name;
        Currency = currency;
        InitialBalance = initialBalance;
    }

    public Guid Id { get; }
    public string Name { get; private set; }
    public string Currency { get; }
    private decimal InitialBalance { get; }


    public IReadOnlyList<Transaction> Transactions => _transactions;

    public decimal CurrentBalance =>
        InitialBalance +
        Transactions.Where(t => t.TransactionType == TransactionType.Income).Sum(t => t.Amount) -
        Transactions.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount);

    public static Wallet Create(string name, string currency, decimal initialBalance)
    {
        if (initialBalance < 0)
            throw new ArgumentOutOfRangeException(nameof(initialBalance), initialBalance,
                "Начальный баланс должен быть положительным.");

        return new Wallet(Guid.NewGuid(), name, currency, initialBalance);
    }

    public void AddTransaction(Transaction transaction)
    {
        if (transaction.TransactionType == TransactionType.Expense && transaction.Amount > CurrentBalance)
            throw new InvalidOperationException("Недостаточно средств");

        _transactions.Add(transaction);
    }
    
}