using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using sapfewse;

namespace RoboSAPiens {
    public class SAPTable: ITable {
        CellRepository cells;
        List<string> columnTitles;
        public string id {get;}
        public int rowCount;

        Dictionary<string, CellType> cellType = new Dictionary<string, CellType>
        {
            {"GuiButton", CellType.Button},
            {"GuiCheckBox", CellType.CheckBox},
            {"GuiLabel", CellType.Label},
            {"GuiTextField", CellType.Text},
            {"GuiCTextField", CellType.Text},
            {"GuiComboBox", CellType.ComboBox},
        };

        public SAPTable(GuiTableControl table) {
            this.id = table.Id;
            // https://answers.sap.com/questions/11100660/how-to-get-a-correct-row-count-in-sap-table.html
            this.rowCount = table.RowCount;
            this.columnTitles = getColumnTitles(table);
            cells = new CellRepository();
        }

        public void classifyCells(GuiSession session)
        {
            var table = (GuiTableControl)session.FindById(id);
            var columns = table.Columns;
            var firstRow = table.VerticalScrollbar?.Position ?? 0;
            var lastRow = firstRow + table.VisibleRowCount - 1;

            for (int relRowIndex = 0; relRowIndex <= lastRow; relRowIndex++)
            {
                for (int colIdx = 0; colIdx < columns.Length; colIdx++) 
                {
                    var columnTitle = columnTitles[colIdx];
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
                            tableCell.Id,
                            new List<string>{columnTitle},
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
            if (rowCountChanged(session))
            {
                rowCount = getNumRows(session);
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

        List<string> getColumnTitles(GuiTableControl table)
        {
            var columnTitles = new List<string>();
            var columns = table.Columns;
            
            for (int colIdx = 0; colIdx < columns.Length; colIdx++) 
            {
                var column = (GuiTableColumn)columns.ElementAt(colIdx);
                columnTitles.Add(column.Title);
            }

            return columnTitles;
        }


        public int getNumRows(GuiSession session) {
            var table = (GuiTableControl)session.FindById(id);
            return table.RowCount;
        }

        public bool hasColumn(string column)
        {
            return columnTitles.ToImmutableHashSet().Contains(column);
        }

        public void print()
        {
            Console.WriteLine();
            Console.WriteLine($"Rows: {rowCount}");
            Console.Write("Columns: " + string.Join(", ", columnTitles));
        }

        public bool rowCountChanged(GuiSession session)
        {
            return getNumRows(session) != rowCount;
        }

        public bool rowIsAbove(GuiSession session, int rowIndex0)
        {
            var table = (GuiTableControl)session.FindById(id);
            var firstRow = table.VerticalScrollbar?.Position ?? 0;
            return rowIndex0 < firstRow;
        }

        public bool rowIsBelow(GuiSession session, int rowIndex0)
        {
            var table = (GuiTableControl)session.FindById(id);
            var firstRow = table.VerticalScrollbar?.Position ?? 0;
            var lastRow = firstRow + table.VisibleRowCount - 1;
            return rowIndex0 > lastRow;
        }

        public bool scrollOnePage(GuiSession session) {
            var table = (GuiTableControl)session.FindById(id);

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

        public void selectRow(int rowIndex0, GuiSession session)
        {
            var table = (GuiTableControl)session.FindById(id);

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
            table = (GuiTableControl)session.FindById(id);
            var row = table.GetAbsoluteRow(rowIndex0);
            row.Selected = true;
        }
    }
}
