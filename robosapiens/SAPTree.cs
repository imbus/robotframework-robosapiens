using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

    public class SAPTree: ITable
    {
        List<string> columnTitles;
        string id;
        int rowCount;
        TreeType treeType;
        TreeElementStore treeElements = new TreeElementStore();

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

        public void classifyCells(GuiSession session, CellRepository repo) 
        {
            var tree = (GuiTree)session.FindById(id);
            var nodeKeys = (GuiCollection)tree.GetAllNodeKeys();
            var columnNames = (GuiCollection)tree.GetColumnNames();

            if (nodeKeys == null) return;
            
            for (int nodeIndex = 0; nodeIndex < nodeKeys.Count; nodeIndex++) 
            {
                var nodeKey = (string)nodeKeys.ElementAt(nodeIndex);
                var nodePath = tree.GetNodePathByKey(nodeKey);
        
                if (columnNames == null)
                {
                    treeElements.Add(new SAPTreeElement(tree, nodeKey, nodePath));
                }
                else
                {
                    for (int c = 0; c < columnNames.Length; c++) 
                    {
                        if (c == 0) {
                            treeElements.Add(new SAPTreeElement(tree, nodeKey, nodePath));
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

                        var itemText = tree.GetItemText(nodeKey, columnName);

                        switch (itemType) 
                        {
                            case TreeItem.Bool:
                                repo.checkBoxes.Add(new SAPTreeCheckBox(columnName, columnTitle, nodeKey, rowNumber: nodeIndex, tree.Id));
                                break;
                            case TreeItem.Button:
                                repo.buttons.Add(new SAPTreeButton(columnName, columnTitle, itemText, nodeKey, rowNumber: nodeIndex, tree.Id));
                                break;
                            case TreeItem.Link:
                                var itemTooltip = tree.GetItemToolTip(nodeKey, columnName);
                                repo.buttons.Add(new SAPTreeLink(columnName, columnTitle, itemText, itemTooltip, nodeKey, rowNumber: nodeIndex, tree.Id));
                                repo.textCells.Add(new SAPTreeCell(columnName, columnTitle, rowIndex: nodeIndex, content: itemText, nodeKey, tree));
                                break;
                            case TreeItem.Text:
                                repo.textCells.Add(new SAPTreeCell(columnName, columnTitle, rowIndex: nodeIndex, content: itemText, nodeKey, tree));
                                break;
                        }
                    }
                }
            }
        }

        public record Node(string parent, string path, string text);

        public static string getParentPath(string path) 
        {
            var pathParts = path.Split("\\");
            var parent_path = string.Join("\\", pathParts[0..^1]);

            if (parent_path != "") {
                return parent_path;
            }

            return "ROOT";
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
    
        public SAPTreeElement? findTreeElement(string folderPath) {
            return treeElements.get(folderPath);
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

        public bool hasColumn(string column)
        {
            return columnTitles.ToImmutableHashSet().Contains(column);
        }
    }
}
