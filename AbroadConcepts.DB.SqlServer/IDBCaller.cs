using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbroadConcepts.Utility.SqlServer
{

    public interface IDBCaller: IDisposable
    {
        void Open(string connectionString);
        Task OpenAsync(string connectionString, CancellationToken ct = default);

        void BeginTransaction();
        void CommitTransaction();
        void CreateDependency(OnChangeEventHandler method, string sql, CommandType commandType = CommandType.StoredProcedure, List<SqlInputParameter>? parameters = null);

        int ExecuteNonQuery(string sql, CommandType commandType = CommandType.StoredProcedure, List<SqlInputParameter>? parameters = null);
        Task<int> ExecuteNonQueryAsync(string sql, CommandType commandType = CommandType.StoredProcedure, List<SqlInputParameter>? parameters = null, CancellationToken ct = default);

        IEnumerable<T> ExecuteReader<T>(string sql, CommandType commandType = CommandType.StoredProcedure, List<SqlInputParameter>? parameters = null) where T : new();
        IAsyncEnumerable<T> ExecuteReaderAsync<T>(string sql, CommandType commandType = CommandType.StoredProcedure, List<SqlInputParameter>? parameters = null, CancellationToken ct = default) where T : new();
 
        object? ExecuteScalar(string sql, CommandType commandType = CommandType.StoredProcedure, List<SqlInputParameter>? parameters = null);
        Task<object?> ExecuteScalarAsync(string sql, CommandType commandType = CommandType.StoredProcedure, List<SqlInputParameter>? parameters = null, CancellationToken ct = default);
        void RollbackTransaction();

    }
}
