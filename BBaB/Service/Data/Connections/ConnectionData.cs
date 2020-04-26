using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BBaB.Service.Data
{
    public class ConnectionData : IDBConnect<SqlConnection>
    {
        private string _sqlConnectionString;

        public ConnectionData()
        {
            //this._sqlConnectionString = "Data Source=DESKTOP-S1TTCNK;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"; //Nathan's Laptop
            //this._sqlConnectionString = "Server=localhost;Database=master;Trusted_Connection=True;";    //Nathan's desktop computer
            this._sqlConnectionString = "Server=bbabdb.cejy5o3ljc1k.us-east-2.rds.amazonaws.com;Database=bbab;User ID=BBaBAdmin;Password=PlY%e$O8$bsNiyMc5WS;";
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(this._sqlConnectionString);
        }
    }
}