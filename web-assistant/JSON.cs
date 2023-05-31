using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Playground 
{
    public record RobotResult(
        string status = "PASS",
        string output = "",
        string @return = "",
        string error = "",
        string traceback = "",
        bool fatal = false,
        bool continuable = false
    );

    [JsonSerializable(typeof(JSONResponse))]
    [JsonSerializable(typeof(JSONError))]
    [JsonSerializable(typeof(RobotResult))]
    [JsonSerializable(typeof(JSONRequest))]
    internal partial class SerializerContext : JsonSerializerContext {}
    
    public record JSONError(int code, string message, RobotResult data);
    public record JSONRequest(string method, string[] @params, int id, string jsonrpc = "2.0");
    public record JSONResponse(RobotResult? result, JSONError? error, int id, string jsonrpc = "2.0");

    public class JSON
    {
        public static JSONResponse Pass(RobotResult result, int id)
        {
            return new JSONResponse(result: result, id: id, error: null);
        }

        public static JSONResponse Fail(JSONError error, int id)
        {
            return new JSONResponse(error: error, id: id, result: null);
        }

        public static JSONResponse? deserialize(string jsonString) {
            return JsonSerializer.Deserialize(jsonString, typeof(JSONResponse), SerializerContext.Default) as JSONResponse;
        }

        public static string serialize(JSONRequest request)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                IncludeFields = true,
                WriteIndented = true
            };

            return JsonSerializer.Serialize(request, typeof(JSONRequest), new SerializerContext(options));
        }
    }
}
