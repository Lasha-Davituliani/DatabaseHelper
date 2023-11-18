using Microsoft.Data.SqlClient;
using DatabaseHelper.Common;

namespace DatabaseHelper.MsSql;

public sealed class SqlDatabase : Database<SqlConnection, SqlCommand, SqlParameter>
{
    public SqlDatabase(string connectionString) : base(connectionString)
    {

    }
}