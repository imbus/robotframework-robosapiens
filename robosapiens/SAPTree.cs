using System;
using System.Collections.Generic;
using sapfewse;

namespace RoboSAPiens 
{
    public class SAPTree 
    {
        string id;
        public int rowCount;

        public SAPTree(GuiTree tree)
        {
            id = tree.Id;
            GuiCollection nodeKeys = (GuiCollection)tree.GetAllNodeKeys();
            if (nodeKeys != null) {
                rowCount = nodeKeys.Count;
            }
            else {
                rowCount = 0;
            }
        }

        public record Node(string parent, string path, string text);

        private static string getParentPath(string path) 
        {
            var pathParts = path.Split("\\");
            var parent_path = String.Join("\\", pathParts[0..^1]);

            if (parent_path == "") 
            {
                return "ROOT";
            }
            else 
            {
                return parent_path;
            }
        }

        public List<Node> getAllNodes(GuiSession session) 
        {
            GuiTree tree = (GuiTree)session.FindById(id);
            var nodes = getAll(tree, (tree, nodeKey) => new Node(
                getParentPath(tree.GetNodePathByKey(nodeKey)),
                tree.GetNodePathByKey(nodeKey),
                tree.GetNodeTextByKey(nodeKey)
            ));
            return nodes;
        }

        public static List<string> getAllPaths(GuiTree tree) 
        {
            return getAll(tree, (tree, nodeKey) => tree.GetNodePathByKey(nodeKey));
        }

        private static List<T> getAll<T>(GuiTree tree, Func<GuiTree, string, T> getByKey) 
        {
            GuiCollection nodeKeys = (GuiCollection)tree.GetAllNodeKeys();
            if (nodeKeys == null) return new List<T>();

            int numNodeKeys = nodeKeys.Count;

            List<T> result = new List<T>(numNodeKeys);

            for (int i = 0; i < numNodeKeys; i++) 
            {
                var nodeKey = (string)nodeKeys.ElementAt(i);
                result.Add(getByKey(tree, nodeKey));
            }

            return result;
        }
    }
}
