using AbroadConcepts.Utility.SqlServer;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAbroadConcepts.Utility.SqlServer;

[ExcludeFromCodeCoverage]
public class SqlInputParameterTests
{
    [Fact]
    public void Test_SqlInputParameterWithString()
    {
        var someString = "Test String";

        var expectedName = "Name";
        var sqlParameters = new SqlInputParameter { Name = expectedName, Value = someString };

        Assert.Equal(expectedName, sqlParameters.Name);
        Assert.Equal(someString, sqlParameters.Value);
    }

}
