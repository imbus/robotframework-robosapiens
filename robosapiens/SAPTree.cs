using System;
using System.Collections.Generic;
using System.Linq;
using sapfewse;

namespace RoboSAPiens 
{
    public enum TreeItem 
    {
        Hierarchy,
        Image,
        Text,
        Bool,
        Button,
        Link,
    }

    public enum TreeType 
    {
        Simple,
        List,
        Column
    }
    public record TreeNode(string parent, string path, string text);

    public class SAPTree: ITable
    {
        CellRepository cells;
        List<string> columnTitles;
        public string id;
        int rowCount;
        TreeType treeType;
        TreeElementStore treeElements;

        Dictionary<TreeItem, CellType> cellType = new Dictionary<TreeItem, CellType>
        {
            {TreeItem.Button, CellType.Button},
            {TreeItem.Bool, CellType.CheckBox},
            {TreeItem.Text, CellType.Text},
            {TreeItem.Link, CellType.Link},
            {TreeItem.Image, CellType.Button},
        };

        public SAPTree(GuiTree tree)
        {
            id = tree.Id;
            treeType = (TreeType)tree.GetTreeType();
            var nodeKeys = (GuiCollection)tree.GetAllNodeKeys();
            if (nodeKeys != null) {
                rowCount = nodeKeys.Count;
            }
            else {
                rowCount = 0;
            }
            columnTitles = getColumnTitles(tree);
            cells = new CellRepository();
            treeElements = new TreeElementStore();
        }

        public void classifyCells(GuiSession session) 
        {
            cells = new CellRepository();
            treeElements = new TreeElementStore();
            var tree = (GuiTree)session.FindById(id);
            var nodeKeys = (GuiCollection)tree.GetAllNodeKeys();
            var columnNames = (GuiCollection)tree.GetColumnNames();

            if (nodeKeys == null) return;
            
            for (int nodeIndex = 0; nodeIndex < nodeKeys.Count; nodeIndex++) 
            {
                var nodeKey = (string)nodeKeys.ElementAt(nodeIndex);
                var nodePath = tree.GetNodePathByKey(nodeKey);
                var text = getText(tree, nodeKey);
                var textPath = getTextPath(tree, nodePath, text);
        
                if (columnNames == null)
                {
                    treeElements.Add(new SAPTreeElement(this, nodeKey, nodePath, textPath));
                }
                else
                {
                    for (int c = 0; c < columnNames.Length; c++) 
                    {
                        if (c == 0) {
                            treeElements.Add(new SAPTreeElement(this, nodeKey, nodePath, textPath));
                        }

                        var columnName = (string)columnNames.ElementAt(c);
                        if (columnName == null) continue;

                        string columnTitle;
                        try {
                            columnTitle = tree.GetColumnTitleFromName(columnName);
                        }
                        catch (Exception) {
                            continue;
                        }

                        TreeItem itemType = (TreeItem)tree.GetItemType(nodeKey, columnName);
                        if (itemType == TreeItem.Hierarchy) {
                            continue;
                        }

                        if (cellType.ContainsKey(itemType))
                        {
                            var itemText = tree.GetItemText(nodeKey, columnName);
                            var itemTooltip = tree.GetItemToolTip(nodeKey, columnName);
                            var labels = new List<string>();

                            if (itemText != null) labels.Add(itemText);
                            if (itemTooltip != null) labels.Add(itemTooltip);

                            var cell = new TreeCell(
                                nodeIndex,
                                textPath,
                                nodeKey,
                                columnName,
                                new List<string>{columnTitle},
                                cellType[itemType],
                                labels,
                                id
                            );
                            cells.Add(cell);
                        }
                    }
                }
            }
        }

        public Cell? findCell(ILocator locator, GuiSession session)
        {
            if (cells.Count == 0) classifyCells(session);

            switch(locator)
            {
                case Content(string labelOrPath):
                    return cells.filterBy<TreeCell>().Find(
                        cell => cell.isLabeled(labelOrPath)
                    );

                case RowColumnLocator(int rowIndex, string column):
                    if (!hasColumn(column)) return null;
                    return cells.findCellByRowAndColumn(rowIndex-1, column);

                case LabelColumnLocator(string labelOrPath, string column):
                    if (!hasColumn(column)) return null;
                    return cells.filterBy<TreeCell>().Find(cell => 
                        cell.inColumn(column) && (cell.isLabeled(labelOrPath) || cell.textPath == labelOrPath)
                    );
            }

            return null;
        }

        public SAPTreeElement? findTreeElement(string folderPath, GuiSession session) {
            if (treeElements.Count == 0) classifyCells(session);
            return treeElements.get(folderPath);
        }

        List<string> getColumnTitles(GuiTree tree)
        {
            var columnNames = (GuiCollection)tree.GetColumnNames();

            if (columnNames == null) {
                return new List<string>();
            }

            var columnTitles = new List<string>();

            for (int i = 0; i < columnNames.Length; i++) 
            {
                var columnName = (string)columnNames.ElementAt(i);
                if (columnName == null) { 
                    continue; 
                }

                try {
                    var columnTitle = tree.GetColumnTitleFromName(columnName);
                    columnTitles.Add(columnTitle);
                }
                catch (Exception) {
                    continue;
                }
            }

            return columnTitles;
        }

        public static string getParentPath(string path) 
        {
            var pathParts = path.Split("\\");
            var parent_path = string.Join("\\", pathParts[0..^1]);

            if (parent_path != "") {
                return parent_path;
            }

            return "ROOT";
        }

        public static string getText(GuiTree tree, string nodeKey)
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

        public static string getTextPath(GuiTree guiTree, string path, string textPath)
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

        public List<TreeNode> getAllNodes(GuiSession session) 
        {
            GuiTree tree = (GuiTree)session.FindById(id);
            var nodes = getAll(tree, (tree, nodeKey) => new TreeNode(
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
            var nodeKeys = (GuiCollection)tree.GetAllNodeKeys();
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

        public int getNumRows(GuiSession session)
        {
            var tree = (GuiTree)session.FindById(id);
            var nodeKeys = (GuiCollection)tree.GetAllNodeKeys();

            if (nodeKeys != null) {
                return nodeKeys.Count;
            }

            return 0;
        }

        public bool hasColumn(string column)
        {
            return columnTitles.ToHashSet().Contains(column);
        }

        public void print()
        {
            Console.Write($" ({treeType})");
            Console.WriteLine();
            Console.WriteLine($"Rows: {rowCount}");
            Console.Write("Column titles: " + string.Join(", ", columnTitles));
        }

        public bool rowCountChanged(GuiSession session)
        {
            return false;
        }

        public bool rowIsAbove(GuiSession session, int rowIndex)
        {
            return false;
        }

        public bool rowIsBelow(GuiSession session, int rowIndex)
        {
            return false;
        }

        public bool scrollOnePage(GuiSession session)
        {
            return false;
        }

        public void selectRow(int rowNumber, GuiSession session)
        {
            GuiTree tree = (GuiTree)session.FindById(id);
            var path = getAllPaths(tree)[rowNumber];
            var nodeKey = tree.FindNodeKeyByPath(path);
            tree.SelectedNode = nodeKey;
        }
    }
}
