using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBaB.Utility.Interfaces
{
    public interface IBBaBLogger
    {

        void Debug(string m);
        void Debug(string i, string m);

        void Info(string m);
        void Info(string i, string m);

        void Warning(string m);
        void Warning(string i, string m);

        void Error(string m, Exception e);

        void Error(string i, string m, Exception e);

    }
}
