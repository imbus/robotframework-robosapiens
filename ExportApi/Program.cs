using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using RoboSAPiens;

public class _
{
    public static void Main(string[] args) 
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "api.json");
        var libArgs = new CLI().arguments.get()
                .ToDictionary(
                    arg => arg.name, 
                    arg => new {
                        name = arg.name,
                        @default = arg.default_value, 
                        desc = arg.doc
                    }
                );
        libArgs.Add("x64", new {
            name = "64bit",
            @default = false,
            desc = "Execute RoboSAPiens 64-bit"
        });
        var api = new 
        {
            doc = new 
            {
                intro = "This is the introduction at the beginning of the documentation",
                init = "This is the section 'Importing' in the documentation"
            },
            args = libArgs,
            keywords = getKeywordSpecs(),
            specs = new {}
        };

        JSON.writeFile(filePath, serialize(api));
    }

    static string serialize(object content)
    {
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            IncludeFields = true,
            WriteIndented = true
        };

        return JsonSerializer.Serialize(content, options);
    }

    static Dictionary<string, string> getSpec(ParameterInfo param)
    {
        return param.GetCustomAttribute(typeof(Locator)) switch {
            Locator locator => locator.locators.ToDictionary(key => key, value => value),
            _ => new Dictionary<string, string>()
        };
    }

    static object getParam(ParameterInfo param)
    {
        return param.DefaultValue switch {
            DBNull => new {
                name = param.Name,
                desc = "",
                spec = getSpec(param)
            },
            var value => new {
                name = param.Name,
                desc = "",
                @default = value,
                type = param.ParameterType switch {
                    var type when type.IsGenericType => type.GenericTypeArguments.First().Name,
                    var type => type.Name
                },
                spec = getSpec(param)
            }
        };
    }

    static object getKeywordSpecs() 
    {
        return typeof(KeywordLibrary)
            .GetMethods()
            .Where(method => 
                method.GetCustomAttribute(typeof(Keyword)) != null &&
                method.GetCustomAttribute(typeof(Doc)) != null
            )
            .ToDictionary(
                method => method.Name,
                method => new 
                    {
                        name = ((Keyword)method.GetCustomAttribute(typeof(Keyword))!).Name,
                        args = method.GetParameters().Where(param => !param.HasDefaultValue).ToDictionary(
                            param => param.Name!,
                            param => getParam(param)
                        ),
                        kwargs = method.GetParameters().Where(param => param.HasDefaultValue).ToDictionary(
                            param => param.Name!,
                            param => getParam(param)
                        ),
                        result = typeof(Result)
                            .GetNestedType(method.Name)!
                            .GetNestedTypes()
                            .ToDictionary(type => type.Name, type => type.Name),
                        doc = new {
                            desc = ((Doc)method.GetCustomAttribute(typeof(Doc))!).DocString,
                            examples = ""
                        }
                    }
            );
    }
}