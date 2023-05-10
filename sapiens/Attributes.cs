using System;

namespace SAPiens {
    public class Keyword : Attribute {
        public string Name;
        
        public Keyword(string name) {
            Name = name;
        }
    }

    public class Doc : Attribute {
        public string DocString;

        public Doc(string docString) {
            DocString = docString;
        }
    }
}
