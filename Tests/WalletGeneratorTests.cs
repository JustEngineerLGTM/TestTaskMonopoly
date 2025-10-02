using TestTaskMonopoly.Domain;
using TestTaskMonopoly.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Tests;

public class WalletGeneratorTests
{
    private readonly IWalletGenerator _walletGenerator;

    public WalletGeneratorTests()
    {
        var builder = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton<IWalletGenerator, WalletGenerator>();
            });

        using var host = builder.Build();
        _walletGenerator = host.Services.GetRequiredService<IWalletGenerator>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(100)]
    public void GenerateWallet_ShouldContainExpectedNumberOfTransactions(int transactionCount)
    {
        var wallet = _walletGenerator.GenerateWallet(transactionCount);

        Assert.NotNull(wallet);
        Assert.Equal(transactionCount, wallet.Transactions.Count);
    }

    [Fact]
    public void GenerateWallet_ShouldHaveUniqueId()
    {
        var wallet1 = _walletGenerator.GenerateWallet(10);
        var wallet2 = _walletGenerator.GenerateWallet(10);

        Assert.NotEqual(wallet1.Id, wallet2.Id);
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    public void GenerateWallet_WithNegativeTransactionCount_ShouldThrow(int transactionCount)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _walletGenerator.GenerateWallet(transactionCount));
    }

}