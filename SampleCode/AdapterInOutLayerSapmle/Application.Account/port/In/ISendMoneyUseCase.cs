using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.port.In
{
    public interface ISendMoneyUseCase
    {
        IEnumerable<Account> ListAccounts();
        bool SendMoney(SendMoneyCommand command);
    }
}
