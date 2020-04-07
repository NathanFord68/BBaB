using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBaB.Utility.Interfaces
{
    public interface IBBaBLogger
    {
        void Debug(String i, String m);

        void Info(String i, String m);

        void Warning(String i, String m);

        void Error(String i, String m);

        void Error(String i, String m, Exception e);

    }
}
