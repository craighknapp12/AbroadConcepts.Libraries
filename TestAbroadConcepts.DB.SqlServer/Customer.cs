using System.Diagnostics.CodeAnalysis;

namespace TestAbroadConcepts.Utility.SqlServer;

[ExcludeFromCodeCoverage]
public class Customer
{
    public int Id { get; set; }
    public string? Name { get; set; }
}