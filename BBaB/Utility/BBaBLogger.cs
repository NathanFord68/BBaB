using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BBaB.Utility.Interfaces;

namespace BBaB.Utility
{
    public class BBaBLogger : IBBaBLogger
    {
        private Logger logger;
        public BBaBLogger()
        {
            this.logger = LogManager.GetLogger("BBaBLogsRules");
        }


        public void Debug(String i, String m)
        {
            this.logger.Debug(i + " " + m);
        }

        public void Info(String i, String m)
        {
            this.logger.Info(i + " " + m);
        }

        public void Warning(String i, String m)
        {
            this.logger.Warn(i + " " + m);
        }

        public void Error(String i, String m)
        {
            this.logger.Error(i + " " + m);
        }

        public void Error(String i, String m, Exception e)
        {
            this.logger.Error(e, i + " " + m);
        }
    }
}