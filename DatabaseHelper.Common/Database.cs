using System.Data;

namespace DatabaseHelper.Common;

public abstract class Database<TConnection, TCommand, TParameters> : IDisposable
    where TConnection : IDbConnection, new()
    where TCommand : IDbCommand
    where TParameters : IDbDataParameter
{
    protected string _connectionString;
    protected TConnection? _connection;
    protected IDbTransaction? _transaction;

    public bool IsTransactionActive => _transaction != null;

    protected Database(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public void OpenConnection()
    {
        IDbConnection connection = GetConnection();

        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }
    }

    public void CloseConnection()
    {
        IDbConnection connection = GetConnection();

        if (connection.State == ConnectionState.Open)
        {
            connection.Close();
        }
    }

    public TConnection GetConnection()
    {
        if (_connection == null)
        {
            _connection = new TConnection();
            _connection.ConnectionString = _connectionString;
        }

        return _connection;
    }

    public TCommand GetCommand(string commandText, CommandType commandType, params TParameters[] parameters)
    {
        TCommand command = (TCommand)GetConnection().CreateCommand();
        command.CommandText = commandText;
        command.CommandType = commandType;

        foreach (var p in parameters)
        {
            command.Parameters.Add(p);
        }

        return command;
    }

    public TCommand GetCommand(string commandText, params TParameters[] parameters)
    {
        return GetCommand(commandText, CommandType.Text, parameters);
    }

    public DataTable GetDataTable(string commandText, CommandType commandType, params TParameters[] parameters)
    {
        using IDataReader reader = ExecuteReader(commandText, commandType, parameters);
        DataTable table = new();
        table.Load(reader);

        return table;
    }

    public DataTable GetDataTable(string commandText, params TParameters[] parameters)
    {
        return GetDataTable(commandText, CommandType.Text, parameters);
    }

    public void BeginTransaction()
    {
        if (IsTransactionActive)
        {
            throw new InvalidOperationException("Transaction is already started!");
        }
        _transaction = GetConnection().BeginTransaction();
    }

    public void CommitTransaction()
    {
        if (!IsTransactionActive)
        {
            throw new InvalidOperationException("Transaction is not started!");
        }

        _transaction?.Commit();
        _transaction = null;
    }

    public void RollBackTransaction()
    {
        if (!IsTransactionActive)
        {
            throw new InvalidOperationException("Transaction is not started!");
        }

        _transaction?.Rollback();
        _transaction = null;
    }

    public int? ExecuteNonQuery(string commandText, CommandType commandType, params TParameters[] parameters)
    {
        TCommand command = GetCommand(commandText, commandType, parameters);

        try
        {
            OpenConnection();
            return command.ExecuteNonQuery();
        }
        finally
        {
            if (!IsTransactionActive)
            {
                CloseConnection();
            }
        }
    }

    public int? ExecuteNonQuery(string commandText, params TParameters[] parameters)
    {
        return ExecuteNonQuery(commandText, CommandType.Text, parameters);
    }

    public object? ExecuteScalar(string commandText, CommandType commandType, params TParameters[] parameters)
    {
        TCommand command = GetCommand(commandText, commandType, parameters);

        try
        {
            OpenConnection();
            return command.ExecuteScalar();
        }
        finally
        {
            if (!IsTransactionActive)
            {
                CloseConnection();
            }
        }
    }

    public object? ExecuteScalar(string commandText, params TParameters[] parameters)
    {
        return ExecuteScalar(commandText, CommandType.Text, parameters);
    }

    public IDataReader ExecuteReader(string commandText, CommandType commandType, params TParameters[] parameters)
    {
        TCommand command = GetCommand(commandText, commandType, parameters);

        OpenConnection();
        return command.ExecuteReader();
    }

    public IDataReader ExecuteReader(string commandText, params TParameters[] parameters)
    {
        return ExecuteReader(commandText, CommandType.Text, parameters);
    }

    public virtual void Dispose()
    {
        CloseConnection();
        _transaction = null;
        GC.SuppressFinalize(this);
    }
}