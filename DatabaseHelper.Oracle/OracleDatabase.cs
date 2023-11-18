using DatabaseHelper.Common;
using System.Data.OracleClient;

namespace DatabaseHelper.Oracle;

public sealed class OracleDatabase : Database<OracleConnection, OracleCommand, OracleParameter>
{
    public OracleDatabase(string connectionString) : base(connectionString)
    {

    }
}
