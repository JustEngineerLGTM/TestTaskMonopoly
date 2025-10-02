
using TestTaskMonopoly.Domain;
using TestTaskMonopoly.Models;
namespace Tests;

public class TransactionTests
{
    [Theory]
    [InlineData(0,TransactionType.Expense)]
    [InlineData(-0.1,TransactionType.Expense)]
    [InlineData(-1,TransactionType.Expense)]
    [InlineData(-1.1,TransactionType.Expense)]
    [InlineData(0,TransactionType.Income)]
    [InlineData(-0.1,TransactionType.Income)]
    [InlineData(-1,TransactionType.Income)]
    [InlineData(-1.1,TransactionType.Income)]
    public void CreateTransaction_WhenNegativeAmount_ShouldThrow(decimal amount,TransactionType transactionType)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Transaction.Create(DateTime.Now, amount, transactionType,"") );
    }

    [Theory]
    [InlineData(1,TransactionType.Expense)]
    [InlineData(1.1,TransactionType.Expense)]
    [InlineData(1,TransactionType.Income)]
    [InlineData(1.1,TransactionType.Income)]
    public void CreateTransaction_WithValidAmount_ShouldNotThrow(decimal amount, TransactionType transactionType)
    {
        Transaction.Create(DateTime.Now, amount, transactionType, "");
    }
}