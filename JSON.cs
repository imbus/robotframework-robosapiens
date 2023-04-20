using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace RoboSAPiens {
    public class JSON {
        public static void SaveJsonFile(string path, object content)
        {
            if (!Path.HasExtension(path))
                path += ".json";

            var options = new JsonSerializerOptions();
            options.IncludeFields = true;
            options.WriteIndented = true;
            options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

            var utf8WithoutBom = new System.Text.UTF8Encoding(false);

            try {
                File.WriteAllText(
                    path, 
                    JsonSerializer.Serialize(content, options), 
                    utf8WithoutBom
                );
            }
            catch (Exception e)
            {
                CLI.error($"An error occurred. {e.Message}");
                Environment.Exit(1);
            }
        }
    }
}