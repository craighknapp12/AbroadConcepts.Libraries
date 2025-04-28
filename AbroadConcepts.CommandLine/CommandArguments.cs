using System.Reflection;

namespace AbroadConcepts.CommandLine;
public class CommandArguments(string[] args) : ICommandArguments
{
    public string Message { get; set; } = string.Empty;
    public List<string> Options { get; set; } = new List<string>();

    private readonly List<IArgument> _arguments = new List<IArgument>();
    private int _argumentIndex = 0;

    public T GetNextArgument<T>() where T : IArgument
    {
        return (T)_arguments[_argumentIndex++];
    }

    public bool Parse<T>(IArgument setting, IEnumerable<T> commandArguments) where T : ICommandArgument
    {
        try
        {
            int i = 0;
            while (i < args.Length)
            {
                var props = setting.GetType().GetProperties();
                for (var p = 0; p < props.Length; p++)
                {
                    var option = args[i];

                    var match = commandArguments.FirstOrDefault(s => s.CommandOption.Equals(option, StringComparison.OrdinalIgnoreCase));
                    if (match is not null)
                    {
                        if (match.ArgumentType is not null)
                        {
                            IArgument argument = (IArgument)Activator.CreateInstance(match.ArgumentType)!;
                            Options.Add(option);
                            _arguments.Add(argument);
                            setting = argument;
                        }

                        i++;
                        break;
                    }

                    SetProperty(setting, args, i, props, p);
                    i++;
                    if (i >= args.Length)
                    {
                        break;
                    }
                }
            }

            return true;
        }
        catch (Exception e)
        {
            Message = e.Message;
            return false;
        }
    }

    public string ShowArguments()
    {
        return string.Join(" ", args);
    }

    private static void SetProperty(object setting, string[] args, int i, PropertyInfo[] props, int p)
    {
        Type t = props[p].PropertyType;
        switch (t.Name)
        {
            case "String":
                props[p].SetValue(setting, args[i]);
                break;
            case "Int32":
                props[p].SetValue(setting, int.Parse(args[i]));
                break;
            case "Boolean":
                props[p].SetValue(setting, bool.Parse(args[i]));
                break;
            case "Decimal":
                props[p].SetValue(setting, decimal.Parse(args[i]));
                break;
            default:
                if (t.BaseType!.Name == "Enum")
                {
                    props[p].SetValue(setting, Enum.Parse(t, args[i]));
                }
                break;
        }
    }
}
