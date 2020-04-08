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

        public void Debug(string m)
        {
            this.logger.Debug(m);
        }
        public void Debug(string i, string m)
        {
            this.logger.Debug(i + " " + m);
        }

        public void Info(string m)
        {
            this.logger.Info(m);
        }

        public void Info(string i, string m)
        {
            this.logger.Info(i + " " + m);
        }
        public void Warning(string m)
        {
            this.logger.Warn(m);
        }

        public void Warning(string i, string m)
        {
            this.logger.Warn(i + " " + m);
        }

        public void Error(string m, Exception e)
        {
            this.logger.Error(e, m);
        }

        public void Error(string i, string m, Exception e)
        {
            this.logger.Error(e, i + " " + m);
        }
    }
}