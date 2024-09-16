using System.Collections.Generic;
using System.Linq;
using sapfewse;

namespace RoboSAPiens
{
    public class SAPTreeElement: ILabeled, IDoubleClickable
    {
        string treeId;
        string nodeKey;
        string nodePath;
        string textPath;

        public SAPTreeElement(GuiTree guiTree, string nodeKey, string path)
        {
            treeId = guiTree.Id;
            this.nodeKey = nodeKey;
            nodePath = path;
            var text = getText(guiTree, nodeKey);
            textPath = getTextPath(guiTree, path, text);
        }

        private string getText(GuiTree tree, string nodeKey)
        {
            var treeType = (TreeType)tree.GetTreeType();
            if (treeType == TreeType.List) 
            {
                var texts = new List<string>();
                for (int i = 1; i < tree.GetListTreeNodeItemCount(nodeKey)+1; i++)
                {
                    var itemText = tree.GetItemText(nodeKey, i.ToString());
                    if (itemText != null && itemText.Trim() != "") {
                        texts.Add(itemText.Replace("/", "|"));
                    }
                }
                return string.Join(" ", texts);
            }

            return tree.GetNodeTextByKey(nodeKey);
        }

        private string getTextPath(GuiTree guiTree, string path, string textPath)
        {
            var parentPath = SAPTree.getParentPath(path);

            if (parentPath == "ROOT") {
                return textPath;
            }
            else {
                var parentText = getText(guiTree, guiTree.GetNodeKeyByPath(parentPath));
                return getTextPath(guiTree, parentPath, $"{parentText}/{textPath}");
            }
        }

        public void doubleClick(GuiSession session) 
        {
            var tree = (GuiTree)session.FindById(treeId);
            var treeType = (TreeType)tree.GetTreeType();
            tree.SelectedNode = nodeKey;

            if (treeType == TreeType.List) 
            {
                var column = tree.GetListTreeNodeItemCount(nodeKey).ToString();
                if (column == "1") {
                    tree.DoubleClickNode(nodeKey);    
                }
                else {
                    tree.DoubleClickItem(nodeKey, column);
                }
            }
            else
            {
                tree.DoubleClickNode(nodeKey);
            }
        }

        public void select(GuiSession session)
        {
            var guiTree = (GuiTree)session.FindById(treeId);
            var parentNodes = nodePath.Split("\\")[0..^1];
            Enumerable.Range(1, parentNodes.Length)
                .Select(i => string.Join("\\", parentNodes.Take(i)))
                .ToList()
                .ForEach(path =>
                    guiTree.ExpandNode(guiTree.GetNodeKeyByPath(path))
                );
            guiTree.SelectedNode = nodeKey;
        }

        // menuEntry: The pipe character '|' is the separator for sub-menus
        public void selectMenuEntry(GuiSession session, string menuEntry)
        {
            var guiTree = (GuiTree)session.FindById(treeId);
            select(session);
            guiTree.NodeContextMenu(nodeKey);
            guiTree.SelectContextMenuItemByText(menuEntry);
        }

        public bool isHLabeled(string label)
        {
            var pathParts = textPath.Split("/");
            var queryParts = label.Replace("//", "|").Split("/");

            if (pathParts.Length != queryParts.Length) return false;

            return pathParts.Zip(queryParts).All(
                tuple => {
                    (string first, string second) = tuple;
                    return first == second || first.StartsWith(second);
                }
            );
        }

        public bool isVLabeled(string label)
        {
            return false;
        }

        public bool hasTooltip(string tooltip)
        {
            return false;
        }
    }
}
