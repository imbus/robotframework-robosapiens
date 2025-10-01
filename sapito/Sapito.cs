using System.Reflection;

using sapfewse;
using saprotwr.net;

namespace Sapito
{
    public static class Sapito
    {
        static GuiApplication getGuiApplication()
        {
            var rot = new CSapROTWrapper();
            var sapGui = rot.GetROTEntry("SAPGUI") ?? rot.GetROTEntry("SAPGUISERVER");

            if (sapGui == null) throw new Exception("SAP is not running");

            var scriptingEngine = sapGui.GetType().InvokeMember(
                "GetScriptingEngine",
                BindingFlags.InvokeMethod,
                null,
                sapGui,
                null
            );

            if (scriptingEngine == null) throw new Exception("Scripting Support is not activated");

            return (GuiApplication)scriptingEngine;
        }

        public static GuiSession ConnectToRunningSap()
        {
            var sapGui = getGuiApplication();
            var connections = sapGui!.Connections;
            if (connections.Length == 0) throw new Exception("SAP is not connected to a server.");
            var connection = (GuiConnection)connections.ElementAt(0);
            var session = (GuiSession)connection!.Sessions.ElementAt(0);

            Console.WriteLine("Connected to SAP");
            return session;
        }

        public static GuiVComponent FindElement(GuiSession session, string id)
        {
            return (GuiVComponent)session.FindById(id);
        }
    }
}
