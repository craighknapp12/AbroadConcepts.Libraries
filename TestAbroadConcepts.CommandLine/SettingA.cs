using System.Diagnostics.CodeAnalysis;
using AbroadConcepts.CommandLine;

namespace TestAbroadConcepts.CommandLine;

[ExcludeFromCodeCoverage]
public class SettingA : ICommandArgument
{
    public string CommandOption => "-a";

    public Type ArgumentType => typeof(OptionA);

}
