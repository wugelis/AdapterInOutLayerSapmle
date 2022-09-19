using Application.port.In;
using Domain;

namespace Application
{
    public class SendMoneyService : ISendMoneyUseCase
    {
        public IEnumerable<Account> ListAccounts()
        {
            throw new NotImplementedException();
        }

        public bool SendMoney(SendMoneyCommand command)
        {
            throw new NotImplementedException();
        }
    }
}