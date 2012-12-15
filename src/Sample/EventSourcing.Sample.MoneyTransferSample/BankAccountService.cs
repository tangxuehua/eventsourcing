using System;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.MoneyTransferSample
{
    public interface IBankAccountService
    {
        BankAccount CreateBankAccount(string customer, string accountNumber);
        void DepositMoney(Guid bankAccountId, double amount);
        void WithdrawMoney(Guid bankAccountId, double amount);
        void TransferMoney(Guid sourceBankAccountId, Guid targetBankAccountId, double amount);
    }
    public class BankAccountService : IBankAccountService
    {
        private IContextManager _contextManager;
        private ITransferMoneyService _transferMoneyService;

        public BankAccountService(IContextManager contextManager, ITransferMoneyService transferMoneyService)
        {
            _contextManager = contextManager;
            _transferMoneyService = transferMoneyService;
        }

        public BankAccount CreateBankAccount(string customer, string accountNumber)
        {
            using (var context = _contextManager.GetContext())
            {
                var bankAccount = new BankAccount(customer, accountNumber);
                context.Add(bankAccount);
                context.SaveChanges();
                return bankAccount;
            }
        }
        public void DepositMoney(Guid bankAccountId, double amount)
        {
            using (var context = _contextManager.GetContext())
            {
                var bankAccount = context.Load<BankAccount>(bankAccountId);
                bankAccount.Deposit(amount);
                context.SaveChanges();
            }
        }
        public void WithdrawMoney(Guid bankAccountId, double amount)
        {
            using (var context = _contextManager.GetContext())
            {
                var bankAccount = context.Load<BankAccount>(bankAccountId);
                bankAccount.Withdraw(amount);
                context.SaveChanges();
            }
        }
        public void TransferMoney(Guid sourceBankAccountId, Guid targetBankAccountId, double amount)
        {
            using (var context = _contextManager.GetContext())
            {
                var sourceAccount = context.Load<BankAccount>(sourceBankAccountId);
                var targetAccount = context.Load<BankAccount>(targetBankAccountId);
                _transferMoneyService.TransferMoney(sourceAccount, targetAccount, amount);
                context.SaveChanges();
            }
        }
    }
}
