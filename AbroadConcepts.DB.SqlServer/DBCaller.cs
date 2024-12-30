using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AbroadConcepts.Utility.SqlServer
{
    public class DBCaller : IDBCaller
    {
        private readonly ISqlDatabase _database;

        public DBCaller(ISqlDatabase database)
        {
            _database = database;
        }

        [ExcludeFromCodeCoverage]
        public void BeginTransaction()
        {
            _database.BeginTransaction();
        }

        [ExcludeFromCodeCoverage]
        public void CommitTransaction()
        {
            _database.CommitTransaction();
        }

        [ExcludeFromCodeCoverage]
        public void CreateDependency(OnChangeEventHandler method, string sql, CommandType commandType = CommandType.StoredProcedure, List<SqlInputParameter>? parameters = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(sql);
            _database.CreateDependency(method, sql, commandType, parameters);
        }

        public void Dispose()
        {
            _database.Dispose();
            GC.SuppressFinalize(this);
        }

        [ExcludeFromCodeCoverage]
        public void Open(string connectionString)
        {
            ArgumentException.ThrowIfNullOrEmpty(connectionString);
            _database.Open(connectionString);
        }

        [ExcludeFromCodeCoverage]
        public Task OpenAsync(string connectionString, CancellationToken ct = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(connectionString);
            return _database.OpenAsync(connectionString, ct);
        }

        public int ExecuteNonQuery(string sql, CommandType commandType = CommandType.StoredProcedure, List<SqlInputParameter>? parameters = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(sql);

            return _database.ExecuteNonQuery(sql, commandType, parameters);
        }

        public async Task<int> ExecuteNonQueryAsync(string sql, CommandType commandType = CommandType.StoredProcedure, List<SqlInputParameter>? parameters = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(sql);

            return await _database.ExecuteNonQueryAsync(sql, commandType, parameters);
        }

        public async Task<int> ExecuteNonQueryAsync(string sql, CommandType commandType = CommandType.StoredProcedure, List<SqlInputParameter>? parameters = null, CancellationToken ct = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(sql);

            return await _database.ExecuteNonQueryAsync(sql, commandType, parameters, ct );
        }

        public IEnumerable<T> ExecuteReader<T>(string sql, CommandType commandType = CommandType.StoredProcedure, List<SqlInputParameter>? parameters = null) where T : new()
        {
            ArgumentException.ThrowIfNullOrEmpty(sql);

            return _database.ExecuteReader<T>(sql, commandType, parameters);
        }

        public async IAsyncEnumerable<T> ExecuteReaderAsync<T>(string sql, CommandType commandType = CommandType.StoredProcedure, List<SqlInputParameter>? parameters = null, [EnumeratorCancellation] CancellationToken ct = default) where T : new()
        {
            ArgumentException.ThrowIfNullOrEmpty(sql);

            var items = _database.ExecuteReaderAsync<T>(sql, commandType, parameters, ct);
            await foreach (var item in items)
            {
                yield return item;
            } 
        }

        public object? ExecuteScalar(string sql, CommandType commandType = CommandType.StoredProcedure, List<SqlInputParameter>? parameters = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(sql);

            return _database.ExecuteScalar(sql, commandType, parameters);
        }

        public async Task<object?> ExecuteScalarAsync(string sql, CommandType commandType = CommandType.StoredProcedure, List<SqlInputParameter>? parameters = null,  CancellationToken ct = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(sql);

            return await _database.ExecuteScalarAsync(sql, commandType, parameters, ct);
        }

        [ExcludeFromCodeCoverage]
        public void RollbackTransaction()
        {
            _database.RollbackTransaction();
        }
    }
}
