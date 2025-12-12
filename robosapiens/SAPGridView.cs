using System;
using System.Collections.Generic;
using System.Linq;
using sapfewse;

namespace RoboSAPiens 
{
    public class SAPGridView: ITable
    {
        record Column(string columnId, List<string> columnTitles);
        public ButtonStore buttons = new ButtonStore();
        public ComboBoxStore comboBoxes = new ComboBoxStore();
        CellRepository cells;
        List<Column> columns;
        string id;
        int rowCount;

        Dictionary<string, CellType> cellType = new Dictionary<string, CellType>
        {
            {"Button", CellType.Button},
            {"CheckBox", CellType.CheckBox},
            {"Link", CellType.Link},
            {"Normal", CellType.Text},
            {"RadioButton", CellType.RadioButton},
            {"ValueList", CellType.ComboBox},
        };

        public SAPGridView(GuiGridView guiGridView)
        {
            id = guiGridView.Id;
            rowCount = guiGridView.RowCount;
            columns = getColumns(guiGridView);
            cells = new CellRepository();
        }

        public void classifyCells(GuiSession session) 
        {
            var gridView = (GuiGridView)session.FindById(id);
            var columnCount = gridView.ColumnCount;
            var firstVisibleRow = gridView.FirstVisibleRow;
            var visibleRowCount = gridView.VisibleRowCount;
            var columnIds = (GuiCollection)gridView.ColumnOrder;

            for (int row = firstVisibleRow; row < firstVisibleRow + visibleRowCount; row++) 
            {
                for (int colIndex0 = 0; colIndex0 < columnCount; colIndex0++) 
                {
                    var columnId = (string)columnIds.ElementAt(colIndex0);
                    string type;
                    try {
                        type = gridView.GetCellType(row, columnId);

                        if (gridView.IsCellHotspot(row, columnId)) {
                            type = "Link";
                        }
                    }
                    // When a row is empty an exception is thrown
                    catch (Exception) {
                        continue;
                    }

                    if (cellType.ContainsKey(type))
                    {
                        var labels = new List<string>
                        {
                            gridView.GetCellTooltip(row, columnId),
                            gridView.GetCellValue(row, columnId),
                        };

                        var cell = new GridViewCell(
                            row, 
                            colIndex0,
                            columnId, 
                            columns[colIndex0].columnTitles, 
                            cellType[type], 
                            labels,
                            id
                        );
                        cells.Add(cell);
                    }
                }
            }
        }

        public void classifyToolbar(GuiGridView gridView)
        {
            for (int i = 0; i < gridView.ToolbarButtonCount; i++)
            {
                var type = gridView.GetToolbarButtonType(i);
                switch (type)
                {
                    case "Button":
                        buttons.Add(new SAPGridViewToolbarButton(gridView, i));
                        break;
                    case "ButtonAndMenu":
                    case "Menu":
                        buttons.Add(new SAPGridViewToolbarButtonMenu(gridView, i));
                        comboBoxes.Add(new SAPGridViewToolbarButtonMenuComboBox(gridView, i));
                        break;
                        // case "CheckBox"
                }
            }
        }
        
        public Cell? findCell(ILocator locator, GuiSession session)
        {
            var currentRowCount = getNumRows(session);
            if (currentRowCount != rowCount) {
                cells = new CellRepository();
            }
            
            switch(locator)
            {
                case Content(string content):
                    if (cells.Count == 0) classifyCells(session);
                    return cells.findCellByContent(content);

                case RowColumnLocator(int rowIndex, string column, int colIndexOffset):
                    {
                        int rowIndex0 = rowIndex - 1;
                        if (rowIndex > rowCount) return null;
                        if (rowIsAbove(session, rowIndex0)) return null;
                        if (!hasColumn(column)) return null;
                        if (rowIsBelow(session, rowIndex0))
                        {
                            if (scrollOnePage(session))
                            {
                                cells = new CellRepository();
                                return findCell(locator, session);
                            }
                        }
                        
                        var gridView = (GuiGridView)session.FindById(id);
                        var col = columns.Find(col => col.columnTitles.Contains(column));
                        var colIndex = columns.FindIndex(col => col.columnTitles.Contains(column));
                        var columnId = col!.columnId;
                        var type = gridView.GetCellType(rowIndex0, columnId);

                        if (cellType.ContainsKey(type))
                        {
                            return new GridViewCell(
                                rowIndex0, 
                                colIndex,
                                columnId, 
                                col.columnTitles, 
                                cellType[type], 
                                new List<string>(),
                                id
                            );
                        }
                        else {
                            return null;
                        }
                    }
                    
                case LabelColumnLocator(string label, string column, int colIndexOffset):
                    {
                        if (!hasColumn(column)) return null;

                        (string content, string? atColumn) = label.Split("@") switch
                        {
                            [string _content, string _atColumn] => (_content.Trim(), _atColumn.Trim()),
                            _ => (label, null)
                        };

                        if (atColumn != null)
                        {
                            if (!hasColumn(atColumn)) return null;

                            var colIndex = columns.FindIndex(col => col.columnTitles.Contains(atColumn));
                            var rowIndex = getRowIndex(colIndex, content, session);

                            if (rowIndex < 0) return null;
                            
                            return findCell(new RowColumnLocator(rowIndex+1, column, colIndexOffset), session);
                        }

                        if (cells.Count == 0) classifyCells(session);
                        var cell = cells.findCellByLabelAndColumn(label, column);
                        if (cell != null) return cell;
                        if (scrollOnePage(session))
                        {
                            cells = new CellRepository();
                            return findCell(locator, session);
                        }
                        return null;   
                    }

                default:
                    return null;
            }
        }

        List<Column> getColumns(GuiGridView gridView) 
        {
            var allColumns = new List<Column>();
            var columnCount = gridView.ColumnCount;
            var columnIds = (GuiCollection)gridView.ColumnOrder;

            for (int column = 0; column < columnCount; column++) 
            {
                var columnId = (string)columnIds.ElementAt(column);
                var thisColumnTitles = new HashSet<string>();

                GuiCollection columnTitles = (GuiCollection)gridView.GetColumnTitles(columnId);
                for (int i = 0; i < columnTitles.Length; i++) 
                {
                    var columnTitle = (string)columnTitles.ElementAt(i);
                    thisColumnTitles.Add(columnTitle.Trim());
                }
                thisColumnTitles.Add(gridView.GetColumnTooltip(columnId).Trim());

                allColumns.Add(new Column(columnId, thisColumnTitles.ToList()));
            }

            return allColumns;
        }

        public int getNumRows(GuiSession session)
        {
            var gridView = (GuiGridView)session.FindById(id);
            return gridView.RowCount;
        }

        public int getRowIndex(int columnIndex, string content, GuiSession session)
        {
            var gridView = (GuiGridView)session.FindById(id);
            var firstVisibleRow = gridView.FirstVisibleRow;
            var visibleRowCount = gridView.VisibleRowCount;
            var columnIds = (GuiCollection)gridView.ColumnOrder;
            var columnId = (string)columnIds.ElementAt(columnIndex);

            for (int rowIndex = firstVisibleRow; rowIndex < firstVisibleRow + visibleRowCount; rowIndex++) 
            {
                if (gridView.GetCellValue(rowIndex, columnId).Trim().Equals(content))
                {
                    return rowIndex;
                }
            }

            if (scrollOnePage(session))
            {
                return getRowIndex(columnIndex, content, session);
            }

            return -1;
        }

        public bool hasColumn(string column)
        {
            return columns.SelectMany(col => col.columnTitles).ToHashSet().Contains(column);
        }

        public bool cellIsSelected(GuiSession session)
        {
            var guiGridView = (GuiGridView)session.FindById(id);
            return guiGridView.CurrentCellRow > -1;
        }

        public void pressKey(string key, GuiSession session) 
        {
            var guiGridView = (GuiGridView)session.FindById(id);

            if (key == "Enter") guiGridView.PressEnter();
            if (key == "F1") guiGridView.PressF1();
            if (key == "F4") guiGridView.PressF4();
        }

        public void print()
        {
            Console.WriteLine();
            Console.WriteLine($"Rows: {rowCount}");
            Console.Write("Columns: " + string.Join(", ", columns.Select(column => "[" + string.Join(", ", column.columnTitles) + "]")));
        }

        public bool rowIsAbove(GuiSession session, int rowIndex)
        {
            var gridView = (GuiGridView)session.FindById(id);
            return rowIndex < gridView.FirstVisibleRow;
        }

        public bool rowIsBelow(GuiSession session, int rowIndex)
        {
            var gridView = (GuiGridView)session.FindById(id);
            var firstRow = gridView.FirstVisibleRow;
            var lastRow = firstRow + gridView.VisibleRowCount - 1;
            return rowIndex > lastRow;
        }

        public bool scrollOnePage(GuiSession session)
        {
            GuiGridView guiGridView = (GuiGridView)session.FindById(id);

            if (guiGridView.FirstVisibleRow + guiGridView.VisibleRowCount < guiGridView.RowCount)
            {
                guiGridView.FirstVisibleRow += guiGridView.VisibleRowCount;
                return true;
            }

            return false;
        }

        public void selectColumn(string column, GuiSession session)
        {
            var gridView = (GuiGridView)session.FindById(id);
            var columnId = columns.Find(col => col.columnTitles.Contains(column))!.columnId;
            gridView.SelectColumn(columnId);
        }

        public void selectRow(int rowIdx0, GuiSession session)
        {
            var gridView = (GuiGridView)session.FindById(id);

            if (rowIdx0 == -1)
            {
                gridView.SelectAll();
                return;
            }

            gridView.CurrentCellRow = rowIdx0;
            gridView.SelectedRows = rowIdx0.ToString();
        }

        public void selectRows(List<int> rowIndices, GuiSession session)
        {
            var gridView = (GuiGridView)session.FindById(id);
            gridView.SelectedRows = string.Join(",", rowIndices);
        }
    }
}
