using System.Linq;
using Horizon.XmlRpc.Server;
using Horizon.XmlRpc.Core;
using SAPiens;
using System.Collections.Generic;
using System.Reflection;

namespace RoboSAPiens
{
    record RobotKeyword(string name, string method, string[] args, string[] types, string doc) {}

    public class RobotRemote : XmlRpcListenerService 
    {
        KeywordLibrary keywordLibrary;
        ILogger logger;
        Options options;
        List<RobotKeyword> robotKeywords;

        public RobotRemote (Options options, ILogger logger) 
        {
            this.logger = logger;
            this.options = options;
            this.keywordLibrary = new KeywordLibrary(options, logger);
            this.robotKeywords = new List<RobotKeyword>();

            typeof(KeywordLibrary)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .ToList()
            .ForEach(methodInfo => 
            {
                var attrKeyword = methodInfo.GetCustomAttribute(typeof(Keyword));
                var attrDoc = methodInfo.GetCustomAttribute(typeof(Doc));

                if (attrKeyword != null && attrDoc != null) 
                {
                    var name = ((Keyword)attrKeyword).Name;
                    var method = methodInfo.Name;
                    var args = methodInfo.GetParameters()
                                            .Select(param => param.Name ?? "").ToArray();
                    var types = methodInfo.GetParameters()
                                            .Select(param => param.ParameterType.ToString())
                                            .ToArray();
                    var doc = ((Doc)attrDoc).DocString;
                    robotKeywords.Add(new RobotKeyword(name, method, args, types, doc));
                }
            });
        }

        XmlRpcStruct asXmlRpcStruct(RobotResult robotResult) 
        {
            var result = new XmlRpcStruct();
            result.Add("status", robotResult.status);
            result.Add("output", robotResult.output);
            result.Add("return", robotResult.returnValue);
            result.Add("error", robotResult.error);
            result.Add("traceback", robotResult.stacktrace);
            result.Add("continuable", robotResult.continuable);
            result.Add("fatal", robotResult.fatal);
            return result;
        }

        string toPythonType(string csharpType) 
        {
            return csharpType switch {
                "System.String" => "str",
                _ => csharpType
            };
        }

        RobotKeyword getKeyword(string name) {
            return robotKeywords.Single(keyword => keyword.method == name);
        }

        [XmlRpcMethod("get_keyword_names")]
        public string[] getKeywordNames() {
            return robotKeywords.Select(keyword => keyword.name).ToArray();
        }

        [XmlRpcMethod("get_keyword_types")]
        public string[] getKeywordTypes(string keywordName) 
        {
            var types = getKeyword(keywordName).types;

            return types.Select(type => toPythonType(type)).ToArray();
        }

        [XmlRpcMethod("run_keyword")]
        public XmlRpcStruct runKeyword(string keywordName, object[] args) 
        {
            if (this.options.debug) 
            {
                logger.info($"Keyword: {keywordName}");
                logger.info("Arguments: " + string.Join(", ", args));
            }

            var result = new RobotResult();
            var keywordTypes = getKeywordTypes(keywordName);
            var keywordArgs = getKeywordArguments(keywordName);
            
            for (int i = 0; i < args.Length; i++) 
            {
                var rfType = toPythonType(args[i].GetType().ToString());

                if (rfType != keywordTypes[i]) 
                {
                    result = new Result.RobotRemote.InvalidArgumentType(keywordArgs[i], rfType, keywordTypes[i]);
                    return asXmlRpcStruct(result);
                }
            }

            result = keywordLibrary.callKeyword(keywordName, args);
            return asXmlRpcStruct(result);
        }

        [XmlRpcMethod("get_keyword_arguments")]
        public string[] getKeywordArguments(string keyword) {
            return getKeyword(keyword).args;
        }

        [XmlRpcMethod("get_keyword_documentation")]
        public string getKeywordDocumentation(string keyword) {
            return getKeyword(keyword).doc;
        }
    }
}
