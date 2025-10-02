using TestTaskMonopoly.Domain;
using TestTaskMonopoly.Models;
namespace TestTaskMonopoly.Services;

public class WalletGenerator : IWalletGenerator
{
    // Массив с именами для генерации
    private readonly static string[] Names =
    {
        "Дебетовый", "Кредитка", "Сбережения", "Ипотека", "Инвестиции"
    };

    // Массив с валютами
    private readonly static string[] Currencies =
    {
        "RUB", "USD", "EUR"
    };

    private readonly Random _rnd = new();

    /// <summary>
    /// Генерация кошелька со случайными транзакциями и параметрами
    /// </summary>
    /// <param name="transactionsCount"></param>
    /// <returns>Кошелек со случайными параметрами</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Wallet GenerateWallet(int transactionsCount)
    {
        if (transactionsCount < 0)
            throw new ArgumentOutOfRangeException(nameof(transactionsCount),"Количество транзакций не может быть меньше 0");
        
        var wallet = Wallet.Create(
            Names[_rnd.Next(Names.Length)],
            Currencies[_rnd.Next(Currencies.Length)],
            _rnd.Next(1000, 20000)
        );

        for (int i = 1; i <= transactionsCount; i++)
        {
            var type = _rnd.Next(2) == 0 ? TransactionType.Income : TransactionType.Expense;
            decimal amount = _rnd.Next(100, 5000);

            if (type == TransactionType.Expense && amount > wallet.CurrentBalance)
            {
                type = TransactionType.Income;
            }

            var transaction = Transaction.Create(
                date: DateTime.Now.AddMonths(_rnd.Next(12)).AddDays(_rnd.Next(28)),
                amount: amount,
                type: type,
                description: type == TransactionType.Income ? "Доход" : "Трата"
            );

            wallet.AddTransaction(transaction);
        }

        return wallet;
    }
}