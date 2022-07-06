using System;
using System.Collections.Generic;
using System.Linq;

namespace RoboSAPiens {
    public struct Options {
        public int port;
        public bool debug;

        public Options(string[] options) {
            this.debug = false;
            this.port = 8270;

            var optionStack = new Stack<string>(options.Reverse());
            while (optionStack.Count > 0) {
                var option = optionStack.Pop();
                switch (option) {
                    case "--debug":
                        this.debug = true;
                        break;
                    case "-p":
                        string? maybePort;
                        if (!optionStack.TryPop(out maybePort)) {
                            Console.Error.WriteLine("The option -p must be followed by a port.");
                            Environment.Exit(1);
                        }
                        if (!Int32.TryParse(maybePort, out this.port)) {
                            Console.Error.WriteLine("The port must be a number");
                            Environment.Exit(1);
                        }
                        break;
                    default:
                        Console.Error.WriteLine($"Unsupported option `{option}`");
                        Environment.Exit(1);
                        break;
                }        
            }
        }
    }
}
