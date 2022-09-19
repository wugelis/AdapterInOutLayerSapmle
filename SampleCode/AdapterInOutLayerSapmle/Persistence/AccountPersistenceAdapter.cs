using Application.port.Out;
using Application.Port.Out;
using Domain;
using System.Diagnostics;

namespace Persistence
{
    public class AccountPersistenceAdapter : ILoadAccountPort, IUpdateAccountStatePort
    {
        public bool UpdateAccount(Account account)
        {
            // DIP 依賴反轉 (Output Port / Persistence Layer)
            Trace.WriteLine($"{this.GetType().Name} UpdateAccount is Called!!");
            return true;
        }

        public void UpdateActivity(Account account)
        {
            throw new NotImplementedException();
        }
    }
}