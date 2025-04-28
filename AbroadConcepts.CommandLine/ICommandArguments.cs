
namespace AbroadConcepts.CommandLine;

public interface ICommandArguments
{
    string Message { get; set; }
    List<string> Options { get; set; }

    T GetNextArgument<T>() where T : IArgument;
    bool Parse<T>(IArgument setting, IEnumerable<T> commandArguments) where T : ICommandArgument;
    string ShowArguments();
}