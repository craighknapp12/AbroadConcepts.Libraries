using System.Reflection;

namespace AbroadConcepts.CommandLine;
public class CommandArguments
{
    public string Message { get; set; } = string.Empty;
    public List<string> Options { get; set; } = new List<string>();

    private readonly Dictionary<string, object> _settings = new Dictionary<string, object>();
    
    public void Register(string option, object setting)
    {
        _settings[option] = setting; 
    }

    public bool Parse(object setting, string[] args)
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
                    var match = _settings.ContainsKey(option);
                    if (match)
                    {
                        Options.Add(option);
                        setting = _settings[option];

                        i++;
                        break;
                    }
                    if (option == "-h")
                    {
                        Message = "User passed help option";
                        return false;
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
