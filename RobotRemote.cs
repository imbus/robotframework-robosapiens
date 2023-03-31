using System;
using System.Linq;
using Horizon.XmlRpc.Server;
using Horizon.XmlRpc.Core;

namespace RoboSAPiens {
    public sealed class RobotRemote : XmlRpcListenerService {
        RoboSAPiens roboSapiens;
        Config.Options options;

        string toPythonType(string csharpType) {
            return csharpType switch {
                "System.String" => "str",
                _ => csharpType
            };
        }

        public RobotRemote (Config.Options options) {
            this.options = options;
            this.roboSapiens = new RoboSAPiens(options);
        }

        [XmlRpcMethod("get_keyword_names")]
        public string[] getKeywordNames() {
            roboSapiens = new RoboSAPiens(this.options);
            var keywordNames = roboSapiens.getKeywordNames();
            
            return keywordNames;
        }

        [XmlRpcMethod("get_keyword_types")]
        public string[] getKeywordTypes(string keywordName) {
            var types = roboSapiens.getKeyword(keywordName).types;

            return types.Select(type => toPythonType(type)).ToArray();
        }

        [XmlRpcMethod("run_keyword")]
        public XmlRpcStruct runKeyword(string keywordName, object[] args) {
            if (this.options.debug) {
                Console.WriteLine($"Keyword: {keywordName}");
                Console.WriteLine("Arguments: " + string.Join(", ", args));
            }

            var result = new RobotResult();
            var keyword = roboSapiens.getKeyword(keywordName);
            var method = typeof(RoboSAPiens).GetMethod(keyword.method);
            
            try {
                for (int i = 0; i < args.Length; i++) {
                    var rfType = args[i].GetType().ToString();

                    if (rfType != keyword.types[i]) {
                        result = new InvalidArgumentError($"Der Datentyp vom Argument {keyword.args[i]} ist {toPythonType(rfType)}. Muss {toPythonType(keyword.types[i])} sein.");
                        return result.asXmlRpcStruct();
                    }
                }

                var returnValue = method?.Invoke(roboSapiens, args);
                if (returnValue != null) {
                    result = (RobotResult)returnValue;
                }
            } catch (Exception e) {
                result = new ExceptionError(e, $"Der Aufruf des Keywords '{keywordName}' ist fehlgeschlagen.");
            }

            return result.asXmlRpcStruct();
        }

        [XmlRpcMethod("get_keyword_arguments")]
        public string[] getKeywordArguments(string keyword) {
            return roboSapiens.getKeyword(keyword).args;
        }

        [XmlRpcMethod("get_keyword_documentation")]
        public string getKeywordDocumentation(string keyword) {
            return roboSapiens.getKeyword(keyword).doc;
        }
    }
}
