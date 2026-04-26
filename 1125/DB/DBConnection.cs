using MySqlConnector;
using System.Windows;

internal class DBConnection
{
    MySqlConnection _connection;
    MySqlTransaction _transaction;

    public void Config()
    {
        MySqlConnectionStringBuilder sb = new MySqlConnectionStringBuilder();
        sb.UserID = "roor";
        sb.Password = "228358";
        sb.Server = "127.0.0.1";
        sb.Database = "1135";
        sb.CharacterSet = "utf8mb4";
        _connection = new MySqlConnection(sb.ToString());
    }

    public bool OpenConnection()
    {
        if (_connection == null)
            Config();
        try
        {
            _connection.Open();
            return true;
        }
        catch (MySqlException e)
        {
            MessageBox.Show(e.Message);
            return false;
        }
    }

    internal void CloseConnection()
    {
        if (_connection == null)
            return;
        try
        {
            _connection.Close();
        }
        catch (MySqlException e)
        {
            MessageBox.Show(e.Message);
        }
    }

    internal MySqlCommand CreateCommand(string sql)
    {
        var cmd = new MySqlCommand(sql, _connection);
        if (_transaction != null)
            cmd.Transaction = _transaction;
        return cmd;
    }

    public void BeginTransaction()
    {
        if (_connection != null)
            _transaction = _connection.BeginTransaction();
    }

    public void CommitTransaction()
    {
        try
        {
            _transaction?.Commit();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
            throw;
        }
        finally
        {
            _transaction = null;
        }
    }

    public void RollbackTransaction()
    {
        try
        {
            _transaction?.Rollback();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
            throw;
        }
        finally
        {
            _transaction = null;
        }
    }

    static DBConnection dbConnection;
    private DBConnection() { }

    public static DBConnection GetDbConnection()
    {
        if (dbConnection == null)
            dbConnection = new DBConnection();
        return dbConnection;
    }
}
