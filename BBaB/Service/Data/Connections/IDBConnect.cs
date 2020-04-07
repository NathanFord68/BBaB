using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBaB.Service.Data
{
    public interface IDBConnect<T>
    {
        T GetConnection();
    }
}
