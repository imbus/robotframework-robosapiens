using System.Linq;
using sapfewse;

namespace RoboSAPiens
{
    public class SAPTreeElement: ILabeled, IDoubleClickable
    {
        SAPTree tree;
        string treeId;
        string nodeKey;
        string nodePath;
        string textPath;

        public SAPTreeElement(SAPTree tree, string nodeKey, string path, string textPath)
        {
            this.tree = tree;
            treeId = tree.id;
            this.nodeKey = nodeKey;
            nodePath = path;
            this.textPath = textPath;
        }

        void expandParentNodes(GuiTree guiTree)
        {
            var parentNodes = nodePath.Split("\\")[0..^1];
            Enumerable.Range(1, parentNodes.Length)
                .Select(i => string.Join("\\", parentNodes.Take(i)))
                .ToList()
                .ForEach(path =>
                    guiTree.ExpandNode(guiTree.GetNodeKeyByPath(path))
                );
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
                    expandParentNodes(tree);
                    tree.DoubleClickNode(nodeKey);    
                }
                else {
                    tree.DoubleClickItem(nodeKey, column);
                }
            }
            else
            {
                expandParentNodes(tree);
                tree.DoubleClickNode(nodeKey);
            }
        }

        public void expand(GuiSession session)
        {
            var guiTree = (GuiTree)session.FindById(treeId);
            guiTree.ExpandNode(nodeKey);
            // after expanding a node its subnodes could be loaded dynamically
            tree.classifyCells(session);
        }

        public void select(GuiSession session)
        {
            var guiTree = (GuiTree)session.FindById(treeId);
            expandParentNodes(guiTree);
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
                tuple => tuple switch {
                    var (pathSegment, querySegment) => 
                        pathSegment == querySegment || 
                        pathSegment.StartsWith(querySegment)
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
