using CodeSharp.EventSourcing;
using EventSourcing.Sample.Application;
using EventSourcing.Sample.Model.MoneyTransfer;
using NUnit.Framework;

namespace EventSourcing.Sample.Test
{
    [TestFixture]
    public class MoneyTransferTest : TestBase
    {
        [Test]
        public void TestMoneyTransfer()
        {
            var bankAccountService = ObjectContainer.Resolve<IBankAccountService>();
            var contextManager = ObjectContainer.Resolve<IContextManager>();

            //创建两个测试银行账号
            var sourceAccount = bankAccountService.CreateBankAccount("tangxuehua1", "0001");
            var targetAccount = bankAccountService.CreateBankAccount("tangxuehua2", "0002");

            //存钱
            bankAccountService.DepositMoney(sourceAccount.Id, 1000);
            bankAccountService.DepositMoney(targetAccount.Id, 1000);

            //取钱
            bankAccountService.WithdrawMoney(sourceAccount.Id, 200);
            bankAccountService.WithdrawMoney(targetAccount.Id, 600);

            //转账
            bankAccountService.TransferMoney(sourceAccount.Id, targetAccount.Id, 400);

            //重新获取银行账号
            using (var context = contextManager.GetContext())
            {
                sourceAccount = context.Load<BankAccount>(sourceAccount.Id);
                targetAccount = context.Load<BankAccount>(targetAccount.Id);
            }
            //Assert结果
            Assert.AreEqual(sourceAccount.Balance, 400);
            Assert.AreEqual(targetAccount.Balance, 800);
        }
    }
}
