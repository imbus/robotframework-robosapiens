using sapfewse;

namespace RoboSAPiens
{
    public class SAPTreeFolder: ILocatableCell
    {
        string treeId;
        string nodeKey;
        string textPath;
        string columnTitle;

        public SAPTreeFolder(GuiTree guiTree, string nodeKey, string path, string columnTitle)
        {
            treeId = guiTree.Id;
            this.columnTitle = columnTitle;
            this.nodeKey = nodeKey;
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

        public void open(GuiSession session)
        {
            var guiTree = (GuiTree)session.FindById(treeId);
            guiTree.ExpandNode(nodeKey);
        }

        public bool isLocated(CellLocator locator, TextCellStore labelCells)
        {
            return columnTitle == locator.column && locator switch {
                LabelCellLocator labelLocator => textPath == labelLocator.label,
                _ => false
            };
        }
    }
}
