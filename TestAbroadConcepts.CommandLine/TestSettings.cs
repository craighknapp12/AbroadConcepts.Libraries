using System.Diagnostics.CodeAnalysis;
using AbroadConcepts.CommandLine;

namespace TestAbroadConcepts.CommandLine;
[ExcludeFromCodeCoverage]
public class TestSettings : IArgument
{
    public string? StringValue { get; set; }
    public int IntValue { get; set; }
    public decimal DecimalValue { get; set; }
    public Color Color { get; set; }
    public bool BoolValue { get; set; } 
}
