using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace SAPiens 
{
    public class JSON 
    {
        public static string serialize(object content)
        {
            var options = new JsonSerializerOptions();
            options.IncludeFields = true;
            options.WriteIndented = true;
            options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

            return JsonSerializer.Serialize(content, options);
        }
        
        public static void writeFile(string path, string content) {
            var utf8WithoutBom = new System.Text.UTF8Encoding(false);

            File.WriteAllText(
                path, 
                content,
                utf8WithoutBom
            );
        }
    }
}
