using System;
using System.Collections.Generic;
using System.Linq;
using sapfewse;

namespace RoboSAPiens {
    public class SAPTable: ITable {
        record Column(List<string> titles);
        CellRepository cells;
        List<Column> columns;
        int screenLeft;
        int screenTop;
        public string id {get;}
        public int rowCount;

        Dictionary<string, CellType> cellType = new Dictionary<string, CellType>
        {
            {"GuiButton", CellType.Button},
            {"GuiCheckBox", CellType.CheckBox},
            {"GuiComboBox", CellType.ComboBox},
            {"GuiLabel", CellType.Label},
            {"GuiRadioButton", CellType.RadioButton},
            {"GuiCTextField", CellType.Text},
            {"GuiTextField", CellType.Text},
        };

        public SAPTable(GuiTableControl table) {
            this.id = table.Id;
            // https://answers.sap.com/questions/11100660/how-to-get-a-correct-row-count-in-sap-table.html
            this.rowCount = table.RowCount;
            this.columns = getColumns(table);
            this.screenLeft = table.ScreenLeft;
            this.screenTop = table.ScreenTop;
            cells = new CellRepository();
        }

        public void getTextFields(GuiTableControl table, TextFieldStore textFields)
        {
            if (table.VisibleRowCount < 11 && table.Columns.Length < 11)
            {
                for (int rowIndex = 0; rowIndex < table.VisibleRowCount; rowIndex++)
                {
                    for (int colIndex = 0; colIndex < table.Columns.Length; colIndex++) 
                    {
                        GuiVComponent tableCell;
                        // Tables are not necessarily rectangular grids
                        // A column may have a hole. Holes are skipped
                        try {
                            tableCell = table.GetCell(rowIndex, colIndex);
                        }
                        catch (Exception) {
                            continue;
                        }

                        if (tableCell.Type == "GuiTextField" && tableCell.Text != "") {
                            textFields.Add(new SAPTextField((GuiTextField)tableCell));
                        }
                    }
                }
            }
        }

        public void classifyCells(GuiSession session)
        {
            var table = (GuiTableControl)session.FindById(getCurrentId(session));
            var firstRow = table.VerticalScrollbar?.Position ?? 0;
            var lastRow = firstRow + table.VisibleRowCount - 1;

            for (int relRowIndex = 0; relRowIndex <= lastRow; relRowIndex++)
            {
                for (int colIdx = 0; colIdx < table.Columns.Length; colIdx++) 
                {
                    var columnTitles = columns[colIdx].titles;
                    int absRowIndex = firstRow + relRowIndex;

                    GuiVComponent tableCell;
                    // Tables are not necessarily rectangular grids
                    // A column may have a hole. Holes are skipped
                    try {
                        tableCell = table.GetCell(relRowIndex, colIdx);
                    }
                    catch (Exception) {
                        continue;
                    }

                    if (cellType.ContainsKey(tableCell.Type))
                    {
                        var type = cellType[tableCell.Type];
                        List<string> labels = type switch
                        {
                            CellType.Button => new SAPButton((GuiButton)tableCell).getLabels(),
                            CellType.CheckBox => new SAPCheckBox((GuiCheckBox)tableCell).getLabels(),
                            CellType.ComboBox => new SAPComboBox((GuiComboBox)tableCell).getLabels(),
                            CellType.RadioButton => new SAPRadioButton((GuiRadioButton)tableCell).getLabels(),
                            CellType.Text => new SAPTextField((GuiTextField)tableCell).getLabels(),
                            CellType.Label => new SAPLabel((GuiLabel)tableCell).getLabels(),
                            _ => new List<string>()
                        };

                        var cell = new TableCell(
                            absRowIndex,
                            colIdx,
                            tableCell.Id,
                            columnTitles,
                            type,
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

                        var colIndex0 = getColumnIndex(column, colIndexOffset);

                        if (colIndex0 < 0) return null;
                        if (colIndex0 > columns.Count - 1) return null;
                        var columnTitles = columns[colIndex0].titles;
                        if (!columnTitles.Contains(column)) return null;
                        if (rowIsBelow(session, rowIndex0) && scrollOnePage(session))
                        {
                            return findCell(locator, session);
                        }
                        
                        var table = (GuiTableControl)session.FindById(getCurrentId(session));
                        // The row index is relative to the visible rows and must be adjusted
                        var firstRow = table.VerticalScrollbar?.Position ?? 0;
                        var lastRow = firstRow + table.VisibleRowCount - 1;
                        var visibleRows = lastRow - firstRow + 1;
                        var rows = Enumerable.Range(firstRow, visibleRows).Zip(Enumerable.Range(0, visibleRows)).ToDictionary();
                        var tableCell = table.GetCell(rows[rowIndex0], colIndex0);

                        if (cellType.ContainsKey(tableCell.Type))
                        {
                            return new TableCell(
                                rowIndex0,
                                colIndex0,
                                tableCell.Id,
                                columnTitles,
                                cellType[tableCell.Type],
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

                            var colIndex = columns.FindIndex(col => col.titles.Contains(atColumn));
                            var rowIndex = getRowIndex(colIndex, content, session);

                            if (rowIndex < 0) return null;
                            
                            return findCell(new RowColumnLocator(rowIndex+1, column, colIndexOffset), session);
                        }

                        var colIndex0 = getColumnIndex(column, colIndexOffset);
                        if (colIndex0 < 0) return null;
                        if (colIndex0 > columns.Count - 1) return null;
                        if (!columns[colIndex0].titles.Contains(column)) return null;

                        if (cells.Count == 0) classifyCells(session);
                        var cell = colIndexOffset switch {
                            _ when colIndexOffset > 0 => cells.findCellByLabelAndColumnIndex(label, colIndex0),
                            _ => cells.findCellByLabelAndColumn(label, column)
                        };
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

        int getColumnIndex(string column, int offset0)
        {
            var columnIndexes = 
                columns
                .Select((column, index) => new {column=column, index=index})
                .Where(columnIndex => columnIndex.column.titles.Contains(column))
                .Select(columnIndex => columnIndex.index)
                .ToList();

            if (offset0 < columnIndexes.Count)
                return columnIndexes.ElementAt(offset0);

            return -1;         
        }

        List<Column> getColumns(GuiTableControl table)
        {
            var allColumns = new List<Column>();
            var columns = table.Columns;
            
            for (int colIdx = 0; colIdx < columns.Length; colIdx++) 
            {
                var column = (GuiTableColumn)columns.ElementAt(colIdx);
                var columnTitle = column.Title.Trim();
                var defaultTooltip = column.DefaultTooltip.Trim();
                var tooltip = column.Tooltip.Trim();

                var titles = new HashSet<string>();
                if (columnTitle != "") titles.Add(columnTitle);
                if (defaultTooltip != "") titles.Add(defaultTooltip);
                if (tooltip != "") titles.Add(tooltip);

                allColumns.Add(new Column(titles.ToList()));
            }

            return allColumns;
        }

        public string getCurrentId(GuiSession session)
        {
            try
            {
                session.FindById(id);
                return id;
            }
            catch (Exception)
            {
                var coll = session.FindByPosition(screenLeft, screenTop, false);
                return (string)coll.ElementAt(0);
            }
        }

        public int getNumRows(GuiSession session) {
            var table = (GuiTableControl)session.FindById(getCurrentId(session));
            return table.RowCount;
        }

        public int getRowIndex(int columnIndex, string content, GuiSession session)
        {
            var table = (GuiTableControl)session.FindById(getCurrentId(session));
            var firstRow = table.VerticalScrollbar?.Position ?? 0;
            var lastRow = firstRow + table.VisibleRowCount;

            for (int rowIndex = 0; rowIndex < lastRow; rowIndex++)
            {
                // Tables are not necessarily rectangular grids
                // A column may have a hole. Holes are skipped
                try
                {
                    var tableCell = table.GetCell(rowIndex, columnIndex);
                    
                    if (tableCell.Text.Trim().Equals(content))
                    {
                        return firstRow + rowIndex;
                    }
                }
                catch (Exception)
                {
                    continue;
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
            return columns.SelectMany(col => col.titles).ToHashSet().Contains(column);
        }

        public void print()
        {
            Console.WriteLine();
            Console.WriteLine($"Rows: {rowCount}");
            Console.Write("Columns: " + string.Join(", ", columns.Select(column => "[" + string.Join(", ", column.titles) + "]")));
        }

        public bool rowIsAbove(GuiSession session, int rowIndex0)
        {
            var table = (GuiTableControl)session.FindById(getCurrentId(session));
            var firstRow = table.VerticalScrollbar?.Position ?? 0;
            return rowIndex0 < firstRow;
        }

        public bool rowIsBelow(GuiSession session, int rowIndex0)
        {
            var table = (GuiTableControl)session.FindById(getCurrentId(session));
            var firstRow = table.VerticalScrollbar?.Position ?? 0;
            var lastRow = firstRow + table.VisibleRowCount - 1;
            return rowIndex0 > lastRow;
        }

        public bool scrollOnePage(GuiSession session) {
            var table = (GuiTableControl)session.FindById(getCurrentId(session));

            // When a GuiTableControl is scrolled a number of times 
            // the whole table becomes write-protected, but the next
            // time it is scrolled it becomes writeable. Therefore,
            // scroll it in steps of 1.
            if (table.VerticalScrollbar.Position + 1 < table.VerticalScrollbar.Maximum) 
            {
                table.VerticalScrollbar.Position += 1;
                return true;
            }

            return false;
        }

        public void selectColumn(string column, GuiSession session)
        {
            var table = (GuiTableControl)session.FindById(getCurrentId(session));
            var columnIndex = columns.FindIndex(col => col.titles.Contains(column));
            var tableColumn = (GuiTableColumn)table.Columns.ElementAt(columnIndex);
            tableColumn.Selected = true;
        }

        public void selectRow(int rowIndex0, GuiSession session)
        {
            var table = (GuiTableControl)session.FindById(getCurrentId(session));

            if (rowIndex0 == -1)
            {
                table.SelectAllColumns();
                return;
            }
            if (rowIndex0 >= table.RowCount) return;
            if (rowIsAbove(session, rowIndex0)) return;

            while (rowIsBelow(session, rowIndex0)) {
                scrollOnePage(session);
            }
            table = (GuiTableControl)session.FindById(getCurrentId(session));
            var row = table.GetAbsoluteRow(rowIndex0);
            row.Selected = true;
        }

        public void selectRows(List<int> rowIndices, GuiSession session) {}
    }
}
