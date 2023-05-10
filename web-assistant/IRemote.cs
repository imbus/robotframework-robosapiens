using Horizon.XmlRpc.Core;

namespace WebAssistant 
{
    public interface IRemote 
    {
        [XmlRpcMethod("get_keyword_names")]
        public string[] getKeywordNames();

        [XmlRpcMethod("get_keyword_types")]
        public string[] getKeywordTypes(string keywordName);

        [XmlRpcMethod("run_keyword")]
        public XmlRpcStruct runKeyword(string keywordName, object[] args);

        [XmlRpcMethod("get_keyword_arguments")]
        public string[] getKeywordArguments(string keyword);

        [XmlRpcMethod("get_keyword_documentation")]
        public string getKeywordDocumentation(string keyword);
    }
}