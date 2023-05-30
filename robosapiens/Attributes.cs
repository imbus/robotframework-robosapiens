using System;

namespace RoboSAPiens {
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

    [AttributeUsageAttribute(AttributeTargets.Parameter)]
    public class Locator : Attribute {
        public string[] locators;

        public Locator(params string[] locators) {
            this.locators = locators;
        }
    }
}
