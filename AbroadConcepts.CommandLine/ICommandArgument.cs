namespace AbroadConcepts.CommandLine;

public interface ICommandArgument
{
    string CommandOption { get; }

    Type ArgumentType { get; }
}