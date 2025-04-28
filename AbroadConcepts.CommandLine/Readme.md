# AbroadConcepts.CommandLine

This package allows for easy parameter parsing into an application.   It works by registering various class containers that get loaded when Parse is called.

# Description

The current limitation is this class does not support requirement for all parameters in a container to be set before moving to optional parameters.   In sample below if "hello -o test" is the parameters, then mainArg.FirstParameter is set to "hello" and optionalArg.FirstOptionArg is set to "test".   Note there is no error given for mainArg.SecondParameter not being passed in.

## Properties
```
        public string Message {get; set;}        // Set to any error message if Parse fails. 

        public List<string> Options {get; set;}  // List of Options that where set.   The order of the list
                                                 // determines the order of parameters parsed.
```

## Method
```
        public bool Parse (IArgument setting, IEnumerable<ICommandArgument> arguments)    // Parses the command line into arguments

                                                                                       // setting is the container class for expected parameters.

        public T GetNextArgument<T>() where T : IArgument   // Returns the next argument

```


## Example Code C#

The following sample demonstrates usage:

### defining class containers:

```
public class MainArg : IArgument
{
    public string FirstParameter { get; set; } = string.Empty;
    public int SecondParameter { get; set; } = 0;
}

public class OptionalArg : IArgument
{
    public string FirstOptionArg { get; set; } = string.Empty;
}

public class SettingOptionalArg : ICommandArgument
{
    public string CommandOption => "-a";

    public Type ArgumentType => typeof(OptionalArg);

}


```

### In Program.cs declare these containers and register the optional parameter to  
```
using AbroadConcepts.CommandLine;

MainArg mainArg = new();
var argSettings = [{new SettingsOptionalArg()}];
var cmdLine = new CommandArguements(args);
if (cmdLine.Parse(mainArg, argSettings))
{
    // at this point mainArg will be set as well as any values parsed for optional arguments.
    // cmdLine.Options will be a list with entry of "-o".
    var optionalArg = cmdLine.GetNextArgument<OptionalArg> ();
    if (optionalArg.FirstParameter  == "key")
    {
        // do something unique with option being specified. 
    }
}
else
{
    Console.WriteLine ($"Message is {cmdLine.Message}")
}
```


