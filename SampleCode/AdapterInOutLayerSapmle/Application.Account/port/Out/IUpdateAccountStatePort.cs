using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.port.Out
{
    public interface IUpdateAccountStatePort
    {
        void UpdateActivity(Account account);
    }
}
