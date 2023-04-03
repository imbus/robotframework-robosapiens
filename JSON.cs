using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace RoboSAPiens {
    public class JSON {
        public static void SaveJsonFile(string fileName, IEnumerable<object> content)
        {
            if (!Path.HasExtension(fileName))
                fileName += ".json";

            string path = AppDomain.CurrentDomain.BaseDirectory;

            File.WriteAllText(Path.Combine(path, fileName), JsonSerializer.Serialize(content));
        }
    }
}