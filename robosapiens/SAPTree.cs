using System;
using System.Collections.Generic;
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
        string id;
        int rowCount;
        TreeType treeType;
        TreeFolderStore treeFolders = new TreeFolderStore();

        public SAPTree(GuiTree tree)
        {
            id = tree.Id;
            treeType = (TreeType)tree.GetTreeType();
            GuiCollection nodeKeys = (GuiCollection)tree.GetAllNodeKeys();
            if (nodeKeys != null) {
                rowCount = nodeKeys.Count;
            }
            else {
                rowCount = 0;
            }
        }

        List<string> getColumnTitles(GuiTree tree)
        {
            var columnTitles = new List<string>();

            var columnNames = (GuiCollection)tree.GetColumnNames();
            if (columnNames != null)
            {
                for (int i = 0; i < columnNames.Length; i++) 
                {
                    var columnName = (string)columnNames.ElementAt(i);
                    if (columnName == null) { continue; }

                    string columnTitle;
                    try {
                        columnTitle = tree.GetColumnTitleFromName(columnName);
                        columnTitles.Add(columnTitle);
                    }
                    catch (Exception) {
                        continue;
                    }
                }
            }

            return columnTitles;
        }

        public void classifyCells(GuiSession session, CellRepository repo) 
        {
            var tree = (GuiTree)session.FindById(id);
            var columnNames = (GuiCollection)tree.GetColumnNames();

            if (columnNames != null)
            {
                var paths = getAllPaths(tree);

                for (int i = 0; i < columnNames.Length; i++) 
                {
                    var columnName = (string)columnNames.ElementAt(i);
                    if (columnName == null) { continue; }

                    string columnTitle;
                    try {
                        columnTitle = tree.GetColumnTitleFromName(columnName);
                    }
                    catch (Exception) {
                        continue;
                    }

                    for (int index = 0; index < paths.Count; index++) 
                    {
                        var nodePath = paths[index];
                        var nodeKey = tree.GetNodeKeyByPath(nodePath);

                        TreeItem itemType = (TreeItem)tree.GetItemType(nodeKey, columnName);
                        if (itemType == TreeItem.Hierarchy) continue;

                        var itemText = tree.GetItemText(nodeKey, columnName);

                        switch (itemType) 
                        {
                            case TreeItem.Bool:
                                repo.checkBoxes.Add(new SAPTreeCheckBox(columnName, columnTitle, nodeKey, rowNumber: index, tree.Id));
                                break;
                            case TreeItem.Button:
                                repo.buttons.Add(new SAPTreeButton(columnName, columnTitle, itemText, nodeKey, rowNumber: index, tree.Id));
                                break;
                            case TreeItem.Link:
                                var itemTooltip = tree.GetItemToolTip(nodeKey, columnName);
                                repo.buttons.Add(new SAPTreeLink(columnName, columnTitle, itemText, itemTooltip, nodeKey, rowNumber: index, tree.Id));
                                repo.textCells.Add(new SAPTreeCell(columnName, columnTitle, rowIndex: index, content: itemText, nodeKey, tree));
                                break;
                            case TreeItem.Text:
                                repo.textCells.Add(new SAPTreeCell(columnName, columnTitle, rowIndex: index, content: itemText, nodeKey, tree));
                                treeFolders.Add(new SAPTreeFolder(tree, nodeKey, nodePath, columnTitle));
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

        public int getNumRows(GuiSession session)
        {
            var tree = (GuiTree)session.FindById(id);
            GuiCollection nodeKeys = (GuiCollection)tree.GetAllNodeKeys();
            
            if (nodeKeys != null) {
                return nodeKeys.Count;
            }
            else {
                return 0;
            }
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
    
        public SAPTreeFolder? findTreeFolder(CellLocator locator, TextCellStore textCells) {
            return treeFolders.get(locator, textCells);
        }

        public void print(GuiTree tree)
        {
            var columnTitles = getColumnTitles(tree);
            Console.Write($" ({treeType})");
            Console.WriteLine();
            Console.WriteLine($"Rows: {rowCount}");
            Console.Write("Column titles: " + string.Join(", ", columnTitles));
        }
    }
}
