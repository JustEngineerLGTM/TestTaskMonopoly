using TestTaskMonopoly.Domain;
using TestTaskMonopoly.Models;
namespace TestTaskMonopoly.Services;

public interface IWalletGenerator
{
    Wallet GenerateWallet(int transactionsCount);
}