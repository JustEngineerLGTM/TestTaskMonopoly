using TestTaskMonopoly.Domain;
namespace TestTaskMonopoly.Services;

public interface IWalletGenerator
{
    Wallet GenerateWallet(int transactionsCount);
}