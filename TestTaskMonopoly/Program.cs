using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestTaskMonopoly.Domain;
using TestTaskMonopoly.Models;
using TestTaskMonopoly.Services;

// Переменные для настройки генератора
const int walletsCount = 10;
const int transactionsCount = 50;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => { services.AddSingleton<IWalletGenerator, WalletGenerator>(); });

using var host = builder.Build();
var serviceProvider = host.Services;

var walletGenerator = serviceProvider.GetRequiredService<IWalletGenerator>();

/* Генерируем кошельки, если бы хранили в БД, то по хорошему надо создать отдельный репозиторий,
но по заданию можно сгенерировать в приложении, так что не стал использовать такой подход.*/

var wallets = Enumerable.Range(0, walletsCount)
    .Select(_ => walletGenerator.GenerateWallet(transactionsCount))
    .ToList();

while (true)
{
    Console.WriteLine("Выберите действие:");
    Console.WriteLine("1 - Показать сгруппированные транзакции, отсортированные по общей сумме, отсортированные по дате");
    Console.WriteLine("2 - Показать топ-3 расходов за месяц");
    Console.WriteLine("3 - Показать все транзакции");
    Console.WriteLine("0 - Выход");
    Console.Write("Ваш выбор: ");
    var choice = Console.ReadLine();
    int month;

    switch (choice)
    {
        case "0":
            Environment.Exit(0);
            break;
        case "1":
            month = ReadMonth();

            foreach (var wallet in wallets)
            {
                Console.WriteLine("Транзакции сгруппированные по тимам:");
                PrintTransactionsByType(wallet, month);
                Console.WriteLine("Транзакции сгруппированные по тимам и сумме:");
                PrintByTotalAmount(wallet, month);
                Console.WriteLine("Транзакции сгруппированные и отсортированные по датам :");
                PrintTransactionsByDate(wallet, month);
            }

            break;
        case "2":
            month = ReadMonth();

            foreach (var wallet in wallets)
            {
                PrintTopExpenses(wallet, month);
            }

            break;
        case "3":
            foreach (var wallet in wallets)
            {
                PrintAllTransactions(wallet);
            }

            break;
        default:
            Console.WriteLine("Неизвестная команда");
            break;
    }
}
// Проверка ввода месяца пользователем
int ReadMonth()
{
    while (true)
    {
        Console.Write("Введите месяц (1-12): ");

        if (int.TryParse(Console.ReadLine(), out var month) && month is >= 1 and <= 12)
            return month;

        Console.WriteLine("Неверный месяц");
    }
}

// Вывод в консоль всех транзакций
void PrintAllTransactions(Wallet wallet)
{
    Console.WriteLine($"Все транзакции кошелька: {wallet.Name}::{wallet.Id.ToString()[..4]} ({wallet.Currency})");

    foreach (var t in wallet.Transactions)
    {
        Console.WriteLine($"    {t.Date:yyyy-MM-dd} | {t.TransactionType} | {t.Amount} | {t.Description}");
    }
    Console.WriteLine();
}

// Вывод в консоль транзакций сгруппированные по типу
void PrintTransactionsByType(Wallet wallet, int month)
{
    var transactionsByMonth = wallet.Transactions
        .Where(t => t.Date.Month == month)
        .GroupBy(t => t.TransactionType);

    Console.WriteLine($"Кошелек: {wallet.Name}::{wallet.Id.ToString()[..4]} ({wallet.Currency})");

    foreach (var group in transactionsByMonth)
    {
        Console.WriteLine($"{group.Key}:");

        foreach (var t in group)
        {
            Console.WriteLine($"    {t.Date:yyyy-MM-dd} | {t.Amount} | {t.Description}");
        }
    }
    Console.WriteLine();
}

// Вывод в консоль транзакций сгруппированные по типу и отсортированные по общей сумме
void PrintByTotalAmount(Wallet wallet, int month)
{
    var grouped = wallet.Transactions
        .Where(t => t.Date.Month == month)
        .GroupBy(t => t.TransactionType)
        .OrderByDescending(g => g.Sum(t => t.Amount));

    Console.WriteLine($"Кошелек: {wallet.Name}::{wallet.Id.ToString()[..4]} ({wallet.Currency})");

    foreach (var group in grouped)
    {
        decimal total = group.Sum(t => t.Amount);
        Console.WriteLine($"    {group.Key} — общая сумма: {total}");
    }
    Console.WriteLine();
}

// Вывод в консоль транзакций сгруппированные по типу и отсортированные по дате от старых до новых
void PrintTransactionsByDate(Wallet wallet, int month)
{
    var grouped = wallet.Transactions
        .Where(t => t.Date.Month == month)
        .GroupBy(t => t.TransactionType)
        .OrderByDescending(g => g.Sum(t => t.Amount));

    Console.WriteLine($"Кошелек: {wallet.Name}::{wallet.Id.ToString()[..4]} ({wallet.Currency})");

    foreach (var group in grouped)
    {
        Console.WriteLine($"{group.Key} - общая сумма: {group.Sum(t => t.Amount)}");

        foreach (var t in group.OrderBy(t => t.Date))
        {
            Console.WriteLine($"    {t.Date:yyyy-MM-dd} | {t.Amount} | {t.Description}");
        }
    }
    Console.WriteLine();
}

// Вывод в консоль топ-3 трат за указанный месяц
void PrintTopExpenses(Wallet wallet, int month)
{
    Console.WriteLine($"Топ-3 расходов для кошелька {wallet.Name}::{wallet.Id.ToString()[..4]}:");

    var topExpenses = wallet.Transactions
        .Where(t => t.TransactionType == TransactionType.Expense && t.Date.Month == month)
        .OrderByDescending(t => t.Amount)
        .Take(3);

    foreach (var t in topExpenses)
    {
        Console.WriteLine($"    {t.Date:d} | {t.Amount} | {t.Description}");
    }
    Console.WriteLine();
}