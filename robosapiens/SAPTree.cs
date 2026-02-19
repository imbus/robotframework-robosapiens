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
        record Column(int index, string name, string title);
        List<Column> columns;
        public string id;
        int rowCount;
        TreeType treeType;
        TreeElementStore treeElements;
        int visibleRowCount = 20;

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
            columns = getColumns(tree);
            cells = new CellRepository();
            treeElements = new TreeElementStore();
        }

        public void classifyCells(GuiSession session) 
        {
            treeElements = new TreeElementStore();
            var tree = (GuiTree)session.FindById(id);
            var columnNames = (GuiCollection)tree.GetColumnNames();
            var firstVisibleRow = tree.GetNodeIndex(tree.TopNode);
            var nodeKeys = (GuiCollection)tree.GetAllNodeKeys();
            var rows = Math.Min(rowCount, firstVisibleRow + visibleRowCount);

            for (int row = firstVisibleRow; row < rows; row++) 
            {
                var nodeKey = (string)nodeKeys.ElementAt(row-1);
                var nodePath = tree.GetNodePathByKey(nodeKey);
                var text = getText(tree, nodeKey);
                var textPath = getTextPath(tree, nodePath, text);
        
                if (columnNames == null)
                {
                    treeElements.Add(new SAPTreeElement(this, nodeKey, nodePath, textPath));
                }
                else
                {
                    for (int colIndex0 = 0; colIndex0 < columnNames.Length; colIndex0++) 
                    {
                        if (colIndex0 == 0) {
                            treeElements.Add(new SAPTreeElement(this, nodeKey, nodePath, textPath));
                        }

                        var columnName = (string)columnNames.ElementAt(colIndex0);
                        if (columnName == null) continue;

                        string columnTitle;
                        try {
                            columnTitle = tree.GetColumnTitleFromName(columnName).Trim();
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
                            var labels = getLabels(tree, nodeKey, columnName);
                            var cell = new TreeCell(
                                row,
                                colIndex0,
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
            switch(locator)
            {
                case Content(string labelOrPath):
                    if (cells.Count == 0) classifyCells(session);
                    return cells.filterBy<TreeCell>().Find(
                        cell => cell.isLabeled(labelOrPath)
                    );

                case RowColumnLocator(int rowIndex, string columnTitle, int colIndexOffset):
                    {
                        if (!hasColumn(columnTitle)) return null;
                        if (rowIndex > rowCount) return null;

                        var tree = (GuiTree)session.FindById(id);
                        var nodeKeys = (GuiCollection)tree.GetAllNodeKeys();
                        var nodeKey = (string)nodeKeys.ElementAt(rowIndex - 1);
                        var column = columns.Find(c => c.title == columnTitle);
                        var columnName = column!.name;
                        var itemType = (TreeItem)tree.GetItemType(nodeKey, columnName);
                        var labels = getLabels(tree, nodeKey, columnName);

                        var nodePath = tree.GetNodePathByKey(nodeKey);
                        var text = getText(tree, nodeKey);
                        var textPath = getTextPath(tree, nodePath, text);

                        return new TreeCell(
                            rowIndex,
                            column!.index,
                            textPath,
                            nodeKey,
                            columnName,
                            new List<string> { columnTitle },
                            cellType[itemType],
                            labels,
                            id
                        );
                    }

                case LabelColumnLocator(string labelOrPath, string column, int colIndexOffset):
                    {
                        if (!hasColumn(column)) return null;
                        if (cells.Count == 0) classifyCells(session);

                        var cell = cells.findCellByLabelAndColumn(labelOrPath, column, exact: true);
                        if (cell != null) return cell;
                        
                        if (scrollOnePage(session))
                        {
                            cells = new CellRepository();
                            return findCell(locator, session);
                        }

                        return null;
                    }
            }

            return null;
        }

        public SAPTreeElement? findTreeElement(string folderPath, GuiSession session) {
            if (treeElements.Count == 0) classifyCells(session);
            return treeElements.get(folderPath);
        }

        List<Column> getColumns(GuiTree tree)
        {
            var columnNames = (GuiCollection)tree.GetColumnNames();

            if (columnNames == null) {
                return new List<Column>();
            }

            var columns = new List<Column>();

            for (int i = 0; i < columnNames.Length; i++) 
            {
                var columnName = (string)columnNames.ElementAt(i);
                if (columnName == null) { 
                    continue; 
                }

                try {
                    var columnTitle = tree.GetColumnTitleFromName(columnName).Trim();
                    columns.Add(new Column(i, columnName, columnTitle));
                }
                catch (Exception) {
                    continue;
                }
            }

            return columns;
        }

        public static List<string> getLabels(GuiTree tree, string nodeKey, string columnName)
        {
            var labels = new List<string>();
            var itemText = tree.GetItemText(nodeKey, columnName);
            var itemTooltip = tree.GetItemToolTip(nodeKey, columnName);

            if (itemText != "") labels.Add(itemText);
            if (itemTooltip != "") labels.Add(itemTooltip);

            return labels;
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

            return tree.GetNodeTextByKey(nodeKey).Replace("/", "|");
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
            return columns.Select(c => c.title).ToHashSet().Contains(column);
        }

        public void print()
        {
            Console.Write($" ({treeType})");
            Console.WriteLine();
            Console.WriteLine($"Rows: {rowCount}");
            Console.Write("Column titles: " + string.Join(", ", columns.Select(c => c.title).ToList()));
        }

        public bool rowIsAbove(GuiSession session, int rowIndex)
        {
            var tree = (GuiTree)session.FindById(id);
            return rowIndex < tree.GetNodeIndex(tree.TopNode);
        }

        public bool rowIsBelow(GuiSession session, int rowIndex)
        {
            var tree = (GuiTree)session.FindById(id);
            return rowIndex >= tree.GetNodeIndex(tree.TopNode) + visibleRowCount;
        }

        public bool scrollOnePage(GuiSession session)
        {
            var tree = (GuiTree)session.FindById(id);
            var currentIndex = tree.GetNodeIndex(tree.TopNode);
            var nodeKeys = (GuiCollection)tree.GetAllNodeKeys();

            if (currentIndex + visibleRowCount > nodeKeys.Count) return false;

            if (nodeKeys != null)
            {
                tree.TopNode = (string)nodeKeys.ElementAt(currentIndex + visibleRowCount - 1);
                return true;
            }

            return false;
        }

        public void selectColumn(string column, GuiSession session)
        {
            GuiTree tree = (GuiTree)session.FindById(id);
            var columnName = columns.First(c => c.title == column).name;
            tree.SelectColumn(columnName);
        }

        public void selectRow(int rowNumber, GuiSession session)
        {
            GuiTree tree = (GuiTree)session.FindById(id);
            var path = getAllPaths(tree)[rowNumber];
            var nodeKey = tree.FindNodeKeyByPath(path);
            tree.SelectedNode = nodeKey;
        }

        public void selectRows(List<int> rowIndices, GuiSession session) {}
    }
}
