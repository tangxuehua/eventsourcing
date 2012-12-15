using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.MoneyTransferSample
{
    /// <summary>
    /// 转账领域服务
    /// </summary>
    public interface ITransferMoneyService
    {
        void TransferMoney(BankAccount sourceAccount, BankAccount targetAccount, double amount);
    }
    public class TransferMoneyService : ITransferMoneyService
    {
        public void TransferMoney(BankAccount sourceAccount, BankAccount targetAccount, double amount)
        {
            sourceAccount.TransferOut(targetAccount, amount);
            targetAccount.TransferIn(sourceAccount, amount);
        }
    }
}
