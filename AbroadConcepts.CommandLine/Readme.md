# AbroadConcepts.CommandLine

This package allows for easy parameter parsing into an application.   It works by registering various class containers that get loaded when Parse is called.

# Description

The current limitation is this class does not support requirement for all parameters in a container to be set before moving to optional parameters.   In sample below if "hello -o test" is the parameters, then mainArg.FirstParameter is set to "hello" and optionalArg.FirstOptionArg is set to "test".   Note there is no error given for mainArg.SecondParameter not being passed in.

## Properties
```
        public string Message {get; set;}        // Set to any error message if Parse fails. 

        public List<string> Options {get; set;}  // List of Options that wheere set.   The order of the list
                                                 // determines the order of parameters parsed.
```

## Method
```
        public bool Parse (object setting, string[] args)    // Parses the command line args identify by args.
                                                             // setting is the container class for expected parameters.

        public void Register(string option, object setting)  // Register an option, like "-o" to a setting container.
```


## Example Code C#

The following sample demonstrates usage:

### defining class containers:

```
public class MainArg
{
    public string FirstParameter { get; set; } = string.Empty;
    public int SecondParameter { get; set; } = 0;
}

public class OptionalArg
{
    public string FirstOptionArg { get; set; } = string.Empty;
}
```

### In Program.cs declare these containers and register the optional parameter to  
```
using AbroadConcepts.CommandLine;

MainArg mainArg = new();
OptionalArg optionalArg = new();
var cmdLine = new CommandArguements();
cmdLine.Register("-o", optionalArg);      // This register the parameter  
if (cmdLine.Parse(mainArg, args))
{
    // at this point mainArg will be set as well as any values parsed for optional arguments.
    // cmdLine.Options will be a list with entry of "-o".
}
else
{
    Console.WriteLine ($"Message is {cmdLine.Message}")
}
```


