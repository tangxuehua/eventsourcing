//Copyright (c) CodeSharp.  All rights reserved.

using System.Data;
using System.Data.SqlClient;

namespace CodeSharp.EventSourcing
{
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        public IDbConnection OpenConnection()
        {
            var connection = new SqlConnection(Configuration.Instance.GetSetting<string>("connectionString"));
            connection.Open();
            return connection;
        }
    }
}