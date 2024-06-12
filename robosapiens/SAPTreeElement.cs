using System.Linq;
using sapfewse;

namespace RoboSAPiens
{
    public class SAPTreeElement: ILabeled
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
            var text = guiTree.GetNodeTextByPath(path);
            textPath = getTextPath(guiTree, path, text);
        }

        private string getTextPath(GuiTree guiTree, string path, string textPath)
        {
            var parentPath = SAPTree.getParentPath(path);

            if (parentPath == "ROOT") {
                return textPath;
            }
            else {
                var parentText = guiTree.GetNodeTextByPath(parentPath);
                return getTextPath(guiTree, parentPath, $"{parentText}/{textPath}");
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
            return textPath == label;
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
