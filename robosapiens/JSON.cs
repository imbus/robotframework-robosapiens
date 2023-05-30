using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RoboSAPiens 
{
    [JsonSerializable(typeof(List<FormField>))]
    [JsonSerializable(typeof(List<SAPTree.Node>))]
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

        public static JSONRequest? deserialize(string jsonString) {
            return JsonSerializer.Deserialize(jsonString, typeof(JSONRequest), SerializerContext.Default) as JSONRequest;
        }

        public static string serialize(object content, Type type)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                IncludeFields = true,
                WriteIndented = true
            };

            return JsonSerializer.Serialize(content, type, new SerializerContext(options));
        }
        
        public static void writeFile(string path, string content) {
            var utf8WithoutBom = new System.Text.UTF8Encoding(false);
            File.WriteAllText(path, content, utf8WithoutBom);
        }
    }
}
