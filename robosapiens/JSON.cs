using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RoboSAPiens
{
    [JsonSerializable(typeof(TableCell))]
    [JsonSerializable(typeof(GridViewCell))]
    [JsonSerializable(typeof(TreeCell))]
    [JsonSerializable(typeof(List<FormField>))]
    [JsonSerializable(typeof(List<TreeNode>))]
    [JsonSerializable(typeof(StatusbarMessage))]
    [JsonSerializable(typeof(SessionInfo))]
    [JsonSerializable(typeof(RobotResult))]
    [JsonSerializable(typeof(JSONResponse))]
    [JsonSerializable(typeof(JSONError))]
    [JsonSerializable(typeof(JSONRequest))]
    internal partial class SerializerContext : JsonSerializerContext {}
    
    public record JSONError(int code, string message, RobotResult data);
    public record JSONRequest(string method, object[] args, int id, string jsonrpc = "2.0");
    public record JSONResponse(RobotResult? result, JSONError? error, int id, string jsonrpc = "2.0");

    public class ObjectToInferredTypesConverter: JsonConverter<object>
    {
        public override object Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) => reader.TokenType switch
            {
                JsonTokenType.True => true,
                JsonTokenType.False => false,
                JsonTokenType.Number when reader.TryGetInt32(out int l) => l,
                JsonTokenType.String => reader.GetString()!,
                _ => JsonDocument.ParseValue(ref reader).RootElement.Clone()
            };

        public override void Write(Utf8JsonWriter writer, object objectToWrite, JsonSerializerOptions options) {}  
    }

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
            var options = new JsonSerializerOptions();
            options.Converters.Add(new ObjectToInferredTypesConverter());
            return JsonSerializer.Deserialize(jsonString, typeof(JSONRequest), new SerializerContext(options)) as JSONRequest;
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
