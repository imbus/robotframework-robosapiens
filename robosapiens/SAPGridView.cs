using System;
using System.Collections.Generic;
using System.Linq;
using sapfewse;

namespace RoboSAPiens 
{
    public class SAPGridView: ITable
    {
        CellRepository cells;
        Dictionary<string, List<string>> columnTitles;
        string id;
        int rowCount;

        Dictionary<string, CellType> cellType = new Dictionary<string, CellType>
        {
            {"Button", CellType.Button},
            {"CheckBox", CellType.CheckBox},
            {"Link", CellType.Link},
            {"Normal", CellType.Text},
            {"ValueList", CellType.ComboBox},
        };

        public SAPGridView(GuiGridView guiGridView)
        {
            id = guiGridView.Id;
            rowCount = guiGridView.RowCount;
            columnTitles = getColumnTitles(guiGridView);
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
                for (int column = 0; column < columnCount; column++) 
                {
                    var columnId = (string)columnIds.ElementAt(column);
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
                            columnId, 
                            columnTitles[columnId], 
                            cellType[type], 
                            labels,
                            id
                        );
                        cells.Add(cell);
                    }
                }
            }
        }

        public Cell? findCell(ILocator locator, GuiSession session)
        {
            var currentRowCount = getNumRows(session);
            if (currentRowCount != rowCount) {
                cells = new CellRepository();
            }
            if (cells.Count == 0) classifyCells(session);
            
            switch(locator)
            {
                case Content(string content):
                    return cells.findCellByContent(content);

                case RowColumnLocator(int rowIndex, string column):
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
                    return cells.findCellByRowAndColumn(rowIndex0, column);
                    
                case LabelColumnLocator(string label, string column):
                    if (!hasColumn(column)) return null;
                    var cell = cells.findCellByLabelAndColumn(label, column);
                    if (cell != null) return cell;
                    if (scrollOnePage(session))
                    {
                        cells = new CellRepository();
                        return findCell(locator, session);
                    }
                    return null;

                default:
                    return null;
            }
        }

        Dictionary<string, List<string>> getColumnTitles(GuiGridView gridView) 
        {
            var allColumnTitles = new Dictionary<string, List<string>>();
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

                allColumnTitles.Add(columnId, thisColumnTitles.ToList());
            }

            return allColumnTitles;
        }

        public int getNumRows(GuiSession session)
        {
            var gridView = (GuiGridView)session.FindById(id);
            return gridView.RowCount;
        }

        public bool hasColumn(string column)
        {
            return columnTitles.Values.SelectMany(x => x).ToHashSet().Contains(column);
        }

        public void pressKey(string key, GuiSession session) 
        {
            GuiGridView guiGridView = (GuiGridView)session.FindById(id);

            if (key == "F1") guiGridView.PressF1();
            if (key == "F4") guiGridView.PressF4();
        }

        public void print()
        {
            Console.WriteLine();
            Console.WriteLine($"Rows: {rowCount}");
            Console.Write("Columns: " + string.Join(", ", columnTitles.Values.Select(columns => "[" + string.Join(", ", columns) + "]")));
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
            var columnId = columnTitles.Where(_ => _.Value.Contains(column)).First().Key;
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
    }
}
