using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace AbroadConcepts.Utility.SqlServer;

[ExcludeFromCodeCoverage]
public class SqlDatabase : ISqlDatabase
{
    private SqlConnection? _connection;
    private SqlTransaction? _transaction;
    private SqlCommand? _command;
    private SqlDependency? _dependency;
    public void BeginTransaction()
    {
        ArgumentNullException.ThrowIfNull(_connection);
        if (_transaction != null)
        {
            RollbackTransaction();
        }

        _transaction = _connection.BeginTransaction();
    }

    public void CommitTransaction()
    {
        ArgumentNullException.ThrowIfNull(_transaction);
        lock (this)
        {
            _transaction.Commit();
            _transaction.Dispose();
            _transaction = null;
        }
    }

    public void CreateDependency(OnChangeEventHandler method, string sql, CommandType commandType = CommandType.StoredProcedure, List<SqlInputParameter>? parameters = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(sql);
        ArgumentNullException.ThrowIfNull(_connection);

        _command = _connection.CreateCommand();
        _command.CommandText = sql;
        _command.CommandType = commandType;
        LoadParameters(parameters, _command.Parameters);
        _dependency = new SqlDependency(_command);
        _dependency.OnChange += method;
    }

    public void Dispose()
    {
        if (_command != null)
        {
            _command.Dispose();
            _command = null;
        }

        if (_transaction != null)
        {
            RollbackTransaction();
        }

        if (_connection != null)
        {
            _connection.Close();
            _connection.Dispose();
            _connection = null;
        }

        GC.SuppressFinalize(this);

    }

    public int ExecuteNonQuery(string sql, CommandType commandType = CommandType.StoredProcedure, List<SqlInputParameter>? parameters = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(sql);
        ArgumentNullException.ThrowIfNull(_connection);

        using SqlCommand sqlCommand = _connection.CreateCommand();
        sqlCommand.CommandText = sql;
        sqlCommand.CommandType = commandType;
        LoadParameters(parameters, sqlCommand.Parameters);
        return sqlCommand.ExecuteNonQuery();
    }

    public async Task<int> ExecuteNonQueryAsync(string sql, CommandType commandType = CommandType.StoredProcedure,List<SqlInputParameter>? parameters = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(sql);
        ArgumentNullException.ThrowIfNull(_connection);

        using SqlCommand sqlCommand = _connection.CreateCommand();
        sqlCommand.CommandText = sql;
        sqlCommand.CommandType = commandType;
        LoadParameters(parameters, sqlCommand.Parameters);
        return await sqlCommand.ExecuteNonQueryAsync();
    }

    public async Task<int> ExecuteNonQueryAsync(string sql, CommandType commandType = CommandType.StoredProcedure, List<SqlInputParameter>? parameters = null, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(sql);
        ArgumentNullException.ThrowIfNull(_connection);

        using SqlCommand sqlCommand = _connection.CreateCommand();
        sqlCommand.CommandText = sql;
        sqlCommand.CommandType = commandType;
        LoadParameters(parameters, sqlCommand.Parameters);
        return await sqlCommand.ExecuteNonQueryAsync(ct);
    }

    public IEnumerable<T> ExecuteReader<T>(string sql, CommandType commandType = CommandType.StoredProcedure,List<SqlInputParameter>? parameters = null) where T :  new()
    {
        ArgumentException.ThrowIfNullOrEmpty(sql);
        ArgumentNullException.ThrowIfNull(_connection);

        var jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        using SqlCommand sqlCommand = _connection.CreateCommand();
        sqlCommand.CommandText = sql;
        sqlCommand.CommandType = commandType;
        LoadParameters(parameters, sqlCommand.Parameters);
        using SqlDataReader reader = sqlCommand.ExecuteReader();
        bool useJson = false;
        while (reader.Read())
        {
            if (useJson || (reader.FieldCount == 1 && reader.GetName(0) == "json"))
            {
                yield return JsonSerializer.Deserialize<T>(reader.GetString(0), jsonOptions)!;
            }
            else
            {
                T t = Activator.CreateInstance<T>();
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var name = reader.GetName(i);
                    var propInfo = t!.GetType().GetProperty(name);
                    propInfo?.SetValue(t, reader.GetValue(i));
                }

                yield return t;
            }
        }
    }

    public async IAsyncEnumerable<T> ExecuteReaderAsync<T>(string sql, CommandType commandType = CommandType.StoredProcedure, List<SqlInputParameter>? parameters = null, [EnumeratorCancellation] CancellationToken ct = default) where T :  new()
    {
        ArgumentException.ThrowIfNullOrEmpty(sql);
        ArgumentNullException.ThrowIfNull(_connection);

        var jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        using SqlCommand sqlCommand = _connection.CreateCommand();
        sqlCommand.CommandText = sql;
        sqlCommand.CommandType = commandType;
        LoadParameters(parameters, sqlCommand.Parameters);

        using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync(ct);
        bool useJson = false;
        while (await reader.ReadAsync(ct))
        {
            if (useJson || (reader.FieldCount == 1 && reader.GetName(0) == "json"))
            {
                using Stream stream = reader.GetStream(0);
                if (stream != null)
                {
                    var item = await JsonSerializer.DeserializeAsync<T>(stream, jsonOptions, ct);
                    yield return item!;
                }
            }
            else
            {
                T t = Activator.CreateInstance<T>();
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var name = reader.GetName(i);
                    var propInfo = t!.GetType().GetProperty(name);
                    propInfo?.SetValue(t, reader.GetValue(i));
                }

                yield return t;
            }
        }
    }

    public object? ExecuteScalar(string sql, CommandType commandType = CommandType.StoredProcedure, List<SqlInputParameter>? parameters = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(sql);
        ArgumentNullException.ThrowIfNull(_connection);

        using SqlCommand sqlCommand = _connection.CreateCommand();
        sqlCommand.CommandText = sql;
        sqlCommand.CommandType = commandType;
        LoadParameters(parameters, sqlCommand.Parameters);
        return sqlCommand.ExecuteScalar();
    }

    public async Task<object?> ExecuteScalarAsync(string sql, CommandType commandType = CommandType.StoredProcedure, List<SqlInputParameter>? parameters = null, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(sql);
        ArgumentNullException.ThrowIfNull(_connection);

        using SqlCommand sqlCommand = _connection.CreateCommand();
        sqlCommand.CommandText = sql;
        sqlCommand.CommandType = commandType;
        LoadParameters(parameters, sqlCommand.Parameters);
        return await sqlCommand.ExecuteScalarAsync(ct);
    }

    public void Open(string connectionString)
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionString);
        _connection = new SqlConnection(connectionString);
        _connection.Open();
    }


    public async Task OpenAsync(string connectionString, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionString);
        _connection = new SqlConnection(connectionString);
        await _connection.OpenAsync(ct);
    }

    public void RollbackTransaction()
    {
        ArgumentNullException.ThrowIfNull(_transaction);
        lock (this)
        {
            _transaction.Rollback();
            _transaction.Dispose();
            _transaction = null;
        }

    }

    private void LoadParameters(List<SqlInputParameter>? parameters, SqlParameterCollection sqlParameters)
    {
        if (parameters != null)
        {
            foreach (var parameter in parameters)
            {
                sqlParameters.Add(new SqlParameter(parameter.Name, parameter.Value));
            }
        }
    }


}
