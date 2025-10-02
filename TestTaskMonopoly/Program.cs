using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestTaskMonopoly.Domain;
using TestTaskMonopoly.Models;
using TestTaskMonopoly.Services;
var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => { services.AddSingleton<IWalletGenerator, WalletGenerator>(); });

using var host = builder.Build();
var serviceProvider = host.Services;

var walletGenerator = serviceProvider.GetRequiredService<IWalletGenerator>();

const int walletsCount = 10;
const int transactionsCount = 50;
var wallets = new List<Wallet>();

for (int i = 0; i < walletsCount; i++)
{
    wallets.Add(walletGenerator.GenerateWallet(transactionsCount));
}

var exit = false;

while (!exit)
{
    Console.WriteLine("\nВыберите действие:");
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
            exit = true;
            break;
        case "1":
            month = ReadMonth();

            foreach (var wallet in wallets)
            {
                Console.WriteLine("\nТранзакции сгруппированные по тимам:");
                PrintTransactionsByType(wallet, month);
                Console.WriteLine("\nТранзакции сгруппированные по тимам и сумме:");
                PrintByTotalAmount(wallet, month);
                Console.WriteLine("\nТранзакции сгруппированные и отсортированные по датам :");
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
                AllTransactions(wallet);
            }

            break;
        default:
            Console.WriteLine("Неизвестная команда");
            break;
    }
}

return;

int ReadMonth()
{
    Console.Write("Введите месяц (1-12): ");

    if (!int.TryParse(Console.ReadLine(), out int month) || month < 1 || month > 12)
    {
        Console.WriteLine("Неверный месяц");
        ReadMonth();
    }

    return month;
}

void AllTransactions(Wallet wallet)
{
    Console.WriteLine($"\nВсе транзакции кошелька: {wallet.Name}::{wallet.Id.ToString()[..4]} ({wallet.Currency})");

    foreach (var t in wallet.Transactions)
    {
        Console.WriteLine($"    {t.Date:yyyy-MM-dd} | {t.TransactionType} | {t.Amount} | {t.Description}");
    }
}

void PrintTransactionsByType(Wallet wallet, int month)
{
    var transactionsByMonth = wallet.Transactions
        .Where(t => t.Date.Month == month)
        .GroupBy(t => t.TransactionType);

    Console.WriteLine($"\nКошелек: {wallet.Name}::{wallet.Id.ToString()[..4]} ({wallet.Currency})");

    foreach (var group in transactionsByMonth)
    {
        Console.WriteLine($"{group.Key}:");

        foreach (var t in group)
        {
            Console.WriteLine($"    {t.Date:yyyy-MM-dd} | {t.Amount} | {t.Description}");
        }
    }
}

void PrintByTotalAmount(Wallet wallet, int month)
{
    var grouped = wallet.Transactions
        .Where(t => t.Date.Month == month)
        .GroupBy(t => t.TransactionType)
        .OrderByDescending(g => g.Sum(t => t.Amount));

    Console.WriteLine($"\nКошелек: {wallet.Name}::{wallet.Id.ToString()[..4]} ({wallet.Currency})");

    foreach (var group in grouped)
    {
        decimal total = group.Sum(t => t.Amount);
        Console.WriteLine($"    {group.Key} — общая сумма: {total}");
    }
}

void PrintTransactionsByDate(Wallet wallet, int month)
{
    var grouped = wallet.Transactions
        .Where(t => t.Date.Month == month)
        .GroupBy(t => t.TransactionType)
        .OrderByDescending(g => g.Sum(t => t.Amount));

    Console.WriteLine($"\nКошелек: {wallet.Name}::{wallet.Id.ToString()[..4]} ({wallet.Currency})");

    foreach (var group in grouped)
    {
        Console.WriteLine($"{group.Key} - общая сумма: {group.Sum(t => t.Amount)}");

        foreach (var t in group.OrderBy(t => t.Date))
        {
            Console.WriteLine($"    {t.Date:yyyy-MM-dd} | {t.Amount} | {t.Description}");
        }
    }
}

void PrintTopExpenses(Wallet wallet, int month)
{
    Console.WriteLine($"\nТоп-3 расходов для кошелька {wallet.Name}::{wallet.Id.ToString()[..4]}:");

    var topExpenses = wallet.Transactions
        .Where(t => t.TransactionType == TransactionType.Expense && t.Date.Month == month)
        .OrderByDescending(t => t.Amount)
        .Take(3);

    foreach (var t in topExpenses)
    {
        Console.WriteLine($"    {t.Date:d} | {t.Amount} | {t.Description}");
    }
}