using Microsoft.Data.SqlClient;
using System.Data;

namespace AbroadConcepts.Utility.SqlServer;

public interface ISqlDatabase : IDisposable
{
    void Open(string connectionString);
    Task OpenAsync(string connectionString, CancellationToken ct);

    void BeginTransaction();
    void CommitTransaction();
    void RollbackTransaction();

    void CreateDependency(OnChangeEventHandler method, string sql, CommandType commandType = CommandType.StoredProcedure, List<SqlInputParameter>? parameters = null);

    int ExecuteNonQuery(string sql, CommandType commandType = CommandType.StoredProcedure, List<SqlInputParameter>? parameters = null);
    Task<int> ExecuteNonQueryAsync(string sql, CommandType commandType = CommandType.StoredProcedure, List<SqlInputParameter>? parameters = null, CancellationToken ct = default);

    IEnumerable<T> ExecuteReader<T>(string sql, CommandType commandType = CommandType.StoredProcedure, List<SqlInputParameter>? parameters = null) where T : new();
    IAsyncEnumerable<T> ExecuteReaderAsync<T>(string sql, CommandType commandType = CommandType.StoredProcedure, List<SqlInputParameter>? parameters = null, CancellationToken ct = default) where T : new();

    object? ExecuteScalar(string sql, CommandType commandType = CommandType.StoredProcedure, List<SqlInputParameter>? parameters = null);
    Task<object?> ExecuteScalarAsync(string sql, CommandType commandType = CommandType.StoredProcedure, List<SqlInputParameter>? parameters = null, CancellationToken ct = default);
}
