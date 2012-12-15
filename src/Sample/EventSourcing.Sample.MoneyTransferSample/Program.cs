using System;
using System.Reflection;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.MoneyTransferSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();
            Configuration.Config("EventSourcing.Sample.MoneyTransferSample", assembly, assembly);

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

            Console.Write("Press Enter to exit...");
            Console.ReadLine();
        }
    }
}
