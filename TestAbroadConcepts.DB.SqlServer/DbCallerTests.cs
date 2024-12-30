using AbroadConcepts.Utility.SqlServer;
using Autofac.Extras.Moq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace TestAbroadConcepts.Utility.SqlServer;

[ExcludeFromCodeCoverage]
public class DbCallerTests
{
    private const string TEXT = "7450";

    [Fact]
    public void Test_ExecuteNonQuery()
    {
        var expected = 7450;

        using var mock = AutoMock.GetLoose();

        mock.Mock<ISqlDatabase>().Setup(x => x.ExecuteNonQuery("[dbo].[ExecuteNonQuery]", CommandType.StoredProcedure, null))
            .Returns(expected);
        var cls = mock.Create<DBCaller>();

        var actual = cls.ExecuteNonQuery("[dbo].[ExecuteNonQuery]", CommandType.StoredProcedure, null);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task Test_ExecuteNonQueryAsync()
    {
        var expected = 7450;

        using var mock = AutoMock.GetLoose();

        mock.Mock<ISqlDatabase>().Setup(x => x.ExecuteNonQueryAsync("[dbo].[ExecuteNonQueryAsync]", CommandType.StoredProcedure, null,  default))
            .Returns(Task.FromResult(expected));

        var cls = mock.Create<DBCaller>();

        var actual = await cls.ExecuteNonQueryAsync("[dbo].[ExecuteNonQueryAsync]", CommandType.StoredProcedure, null);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task Test_ExecuteNonQueryWithCancellationTokenAsync()
    {
        var expected = 7450;

        CancellationToken ct = new CancellationToken();

        using var mock = AutoMock.GetLoose();

        mock.Mock<ISqlDatabase>().Setup(x => x.ExecuteNonQueryAsync("[dbo].[ExecuteNonQueryAsync]", CommandType.StoredProcedure, null, ct))
            .Returns(Task.FromResult(expected));
        var cls = mock.Create<DBCaller>();

        var actual = await cls.ExecuteNonQueryAsync("[dbo].[ExecuteNonQueryAsync]", CommandType.StoredProcedure, null, ct);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Test_ExecuteReader()
    {
        using var mock = AutoMock.GetLoose();

        mock.Mock<ISqlDatabase>().Setup(x => x.ExecuteReader<Customer>("[dbo].[Customer_Read]", CommandType.StoredProcedure, null))
            .Returns(GetCustomers);
        var cls = mock.Create<DBCaller>();

        var expected = GetCustomers().ToList();

        var actual = cls.ExecuteReader<Customer>("[dbo].[Customer_Read]", CommandType.StoredProcedure, null).ToList();

        Assert.NotNull(expected);
        Assert.NotNull(actual);
        for (var i = 0; i < expected.Count; i++)
        {
            Assert.Equal(expected[i].Id, actual[i].Id);
            Assert.Equal(expected[i].Name, actual[i].Name);
        }
    }

    [Fact]
    public async Task Test_ExecuteReaderAsync()
    {
        using var mock = AutoMock.GetLoose();

        mock.Mock<ISqlDatabase>().Setup(x => x.ExecuteReaderAsync<Customer>("[dbo].[Customer_Read]", CommandType.StoredProcedure, null, default))
        .Returns(GetCustomersAsync);
        var cls = mock.Create<DBCaller>();

        var expected = GetCustomersAsync().ToBlockingEnumerable().ToList();
        
        await Task.Delay(100);
        var actual = cls.ExecuteReaderAsync<Customer>("[dbo].[Customer_Read]", CommandType.StoredProcedure, null);
        var actualEnum = actual.ToBlockingEnumerable().ToList();

        Assert.NotNull(expected);
        Assert.NotNull(actualEnum);
        for (var i = 0; i < expected.Count; i++)
        {
            Assert.Equal(expected[i].Id, actualEnum[i].Id);
            Assert.Equal(expected[i].Name, actualEnum[i].Name);
        }
    }

    [Fact]
    public async Task Test_ExecuteReaderWithCancellationTokenAsync()
    {
        CancellationToken ct = new CancellationToken();
        using var mock = AutoMock.GetLoose();

        mock.Mock<ISqlDatabase>().Setup(x => x.ExecuteReaderAsync<Customer>("[dbo].[Customer_Read]", CommandType.StoredProcedure, null, ct))
        .Returns(GetCustomersAsync);
        var cls = mock.Create<DBCaller>();

        var expected = GetCustomersAsync().ToBlockingEnumerable().ToList();

        await Task.Delay(100);
        var actual = cls.ExecuteReaderAsync<Customer>("[dbo].[Customer_Read]", CommandType.StoredProcedure, null, ct);
        var actualEnum = actual.ToBlockingEnumerable().ToList();

        Assert.NotNull(expected);
        Assert.NotNull(actualEnum);
        for (var i = 0; i < expected.Count; i++)
        {
            Assert.Equal(expected[i].Id, actualEnum[i].Id);
            Assert.Equal(expected[i].Name, actualEnum[i].Name);
        }
    }

    [Fact]
    public void Test_ExecuteScalar()
    {
        var expected = 7450;
        using var mock = AutoMock.GetLoose();
        mock.Mock<ISqlDatabase>().Setup(x => x.ExecuteScalar("[dbo].[ExecuteScalar]", CommandType.StoredProcedure,null ))
            .Returns(expected);
        var cls = mock.Create<DBCaller>();

        var actual = cls.ExecuteScalar("[dbo].[ExecuteScalar]", CommandType.StoredProcedure, null);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task Test_ExecuteScalarAsync()
    {
        var expected = TEXT;
        using var mock = AutoMock.GetLoose();

        mock.Mock<ISqlDatabase>().Setup(x => x.ExecuteScalarAsync("[dbo].[ExecuteScalarAsync]", CommandType.StoredProcedure, null,  default))
            .Returns(GetScalarValueAsync!);
        var cls = mock.Create<DBCaller>();

        var actual = await cls.ExecuteScalarAsync("[dbo].[ExecuteScalarAsync]", CommandType.StoredProcedure, null);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task Test_ExecuteScalarWithCancellationTokenAsync()
    {
        var expected = TEXT;

        CancellationToken ct = new CancellationToken();
        using var mock = AutoMock.GetLoose();

        mock.Mock<ISqlDatabase>().Setup(x => x.ExecuteScalarAsync("[dbo].[ExecuteScalarAsync]", CommandType.StoredProcedure,null, ct))
            .Returns(GetScalarValueAsync!);
        var cls = mock.Create<DBCaller>();

        var actual = await cls.ExecuteScalarAsync("[dbo].[ExecuteScalarAsync]", CommandType.StoredProcedure, null, ct);

        Assert.Equal(expected, actual);
    }

    private async Task<object> GetScalarValueAsync()
    {
        return await Task.FromResult(TEXT);
    }

    private IEnumerable<Customer> GetCustomers()
    {
        yield return new Customer { Id = 1, Name = "test 1" };
        yield return new Customer { Id = 2, Name = "test 2" };
        yield return new Customer { Id = 3, Name = "test 3" };
        yield return new Customer { Id = 4, Name = "test 4" };
    }
    private async IAsyncEnumerable<Customer> GetCustomersAsync()
    {
        for (int i = 0; i < 5; i++)
        {
            await Task.Delay(500);
            yield return new Customer { Id = i, Name = "test " + i.ToString() }; 
        }
    }

}