using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using RoboSAPiens;

public class _
{
    public static void Main(string[] args) 
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "api.json");
        var api = new 
        {
            doc = new 
            {
                intro = "This is the introduction at the beginning of the documentation",
                init = "This is the section 'Importing' in the documentation"
            },
            args = new CLI().arguments.get()
                .ToDictionary(
                    arg => arg.name, 
                    arg => new {
                        name = arg.name,
                        @default = arg.default_value, 
                        doc = arg.doc
                    }
                ),
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
                        args = method.GetParameters().ToDictionary(
                            param => param.Name!,
                            param => new 
                            {
                                name = param.Name,
                                spec = param.GetCustomAttribute(typeof(Locator)) switch {
                                    Locator locator => locator.locators.ToDictionary(key => key, value => value),
                                    _ => new Dictionary<string, string>()
                                }
                            }
                        ),
                        result = typeof(Result)
                            .GetNestedType(method.Name)!
                            .GetNestedTypes()
                            .ToDictionary(type => type.Name, type => type.Name),
                        doc = ((Doc)method.GetCustomAttribute(typeof(Doc))!).DocString
                    }
            );
    }
}