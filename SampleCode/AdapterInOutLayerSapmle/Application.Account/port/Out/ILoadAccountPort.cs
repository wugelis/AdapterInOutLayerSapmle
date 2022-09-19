using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Port.Out
{
    public interface ILoadAccountPort
    {
        bool UpdateAccount(Account account);
    }
}
