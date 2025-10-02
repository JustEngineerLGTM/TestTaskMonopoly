using TestTaskMonopoly.Domain;
using TestTaskMonopoly.Models;

namespace Tests;

public class WalletTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(1.1)]
    public void CreateWallet_NegativeBalance_ShouldNotThrow(decimal initialBalance)
    {
        Wallet.Create("test", "test", initialBalance);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-1.1)]
    public void CreateWallet_NegativeBalance_ShouldThrow(decimal initialBalance)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Wallet.Create("test", "test", initialBalance));
    }

    [Theory]
    [InlineData(1,TransactionType.Expense)]
    [InlineData(1.1,TransactionType.Expense)]
    [InlineData(1,TransactionType.Income)]
    [InlineData(1.1,TransactionType.Income)]
    public void CreateWallet_AddTransaction(decimal amount, TransactionType transactionType)
    {
        var wallet = Wallet.Create("test", "test", amount);
        wallet.AddTransaction(Transaction.Create(DateTime.Now, amount, transactionType,""));
    }

    [Theory]
    [InlineData(1,TransactionType.Expense)]
    [InlineData(1.1,TransactionType.Expense)]
    public void AddTransactionExpense_WithInsufficientBalance_ShouldThrow(decimal amount, TransactionType transactionType)
    {
        var wallet = Wallet.Create("test", "test", 0);
        Assert.Throws<InvalidOperationException>(() => wallet.AddTransaction(Transaction.Create(DateTime.Now, amount, transactionType,"")));
    }

    [Fact]
    public void CurrentBalanceShouldBeZero()
    {
        var wallet = Wallet.Create("test", "test", 100);
        wallet.AddTransaction(Transaction.Create(DateTime.Now, 100, TransactionType.Income,""));
        wallet.AddTransaction(Transaction.Create(DateTime.Now, 200, TransactionType.Expense,""));
        Assert.Equal(0, wallet.CurrentBalance);
    }
}