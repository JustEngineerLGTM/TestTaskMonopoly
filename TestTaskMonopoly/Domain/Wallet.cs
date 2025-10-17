using TestTaskMonopoly.Models;
namespace TestTaskMonopoly.Domain;

public class Wallet : IEntity
{
    private readonly List<Transaction> _transactions = [];

    private Wallet(Guid id, string name, string currency, decimal initialBalance)
    {
        Id = id;
        Name = name;
        Currency = currency;
        InitialBalance = initialBalance;
    }

    public string Name { get; private set; }
    public string Currency { get; }
    private decimal InitialBalance { get; }


    public IReadOnlyList<Transaction> Transactions => _transactions;

    public decimal CurrentBalance =>
        InitialBalance +
        Transactions.Where(t => t.TransactionType == TransactionType.Income).Sum(t => t.Amount) -
        Transactions.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount);

    public Guid Id { get; }

    public static Wallet Create(string name, string currency, decimal initialBalance)
    {
        if (initialBalance < 0)
            throw new ArgumentOutOfRangeException(nameof(initialBalance), initialBalance,
                "Начальный баланс должен быть не отрицательным.");

        return new Wallet(Guid.NewGuid(), name, currency, initialBalance);
    }

    /// <summary>
    ///     Добавление транзакции в кошелек
    /// </summary>
    /// <param name="transaction"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void AddTransaction(Transaction transaction)
    {
        if (transaction.TransactionType == TransactionType.Expense && transaction.Amount > CurrentBalance)
            throw new InvalidOperationException("Недостаточно средств");

        _transactions.Add(transaction);
    }
}