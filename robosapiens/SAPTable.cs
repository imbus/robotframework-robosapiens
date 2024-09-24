using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using sapfewse;

namespace RoboSAPiens {
    public class SAPTable: ITable {
        List<string> columnTitles;
        public string id {get;}
        public int totalRows;

        public SAPTable(GuiTableControl table) {
            this.id = table.Id;
            // https://answers.sap.com/questions/11100660/how-to-get-a-correct-row-count-in-sap-table.html
            this.totalRows = table.RowCount;
            this.columnTitles = getColumnTitles(table);
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

        public void classifyCells(GuiSession session, CellRepository repo)
        {
            var table = (GuiTableControl)session.FindById(id);
            var columns = table.Columns;
            var firstRow = table.VerticalScrollbar?.Position ?? 0;
            var lastRow = firstRow + table.VisibleRowCount - 1;

            for (int relRowIndex = 0; relRowIndex <= lastRow; relRowIndex++)
            {
                for (int colIdx = 0; colIdx < columns.Length; colIdx++) 
                {
                    var column = (GuiTableColumn)columns.ElementAt(colIdx);
                    var columnTitle = column.Title;
                    int absRowIndex = firstRow + relRowIndex;

                    GuiVComponent cell;

                    // Tables are not necessarily rectangular grids
                    // A column may have a hole. Holes are skipped
                    try {
                        cell = table.GetCell(relRowIndex, colIdx);
                    }
                    catch (Exception) {
                        continue;
                    }

                    switch (cell.Type)
                    {
                        case "GuiButton":
                            repo.buttons.Add(new SAPTableButton(columnTitle, absRowIndex, (GuiButton)cell, this));
                            break;
                        case "GuiCheckBox":
                            repo.checkBoxes.Add(new SAPTableCheckBox(columnTitle, absRowIndex, (GuiCheckBox)cell, this));
                            break;
                        case "GuiTextField":
                        case "GuiCTextField":
                            repo.textCells.Add(new SAPTableCell(columnTitle, absRowIndex, (GuiTextField)cell, this));
                            break;
                        case "GuiComboBox":
                            repo.comboBoxes.Add(new SAPTableComboBox(columnTitle, absRowIndex, (GuiComboBox)cell));
                            break;
                    }
                }
            }
        }

        public int getNumRows(GuiSession session) {
            var table = (GuiTableControl)session.FindById(id);
            return table.RowCount;
        }

        public bool hasColumn(string column)
        {
            return columnTitles.ToImmutableHashSet().Contains(column);
        }

        public bool rowCountChanged(GuiSession session)
        {
            return getNumRows(session) != totalRows;
        }

        public bool rowIsAbove(GuiSession session, int rowIndex)
        {
            var table = (GuiTableControl)session.FindById(id);
            var firstRow = table.VerticalScrollbar?.Position ?? 0;
            return rowIndex < firstRow;
        }

        public bool rowIsBelow(GuiSession session, int rowIndex)
        {
            var table = (GuiTableControl)session.FindById(id);
            var firstRow = table.VerticalScrollbar?.Position ?? 0;
            var lastRow = firstRow + table.VisibleRowCount - 1;
            return rowIndex > lastRow;
        }

        public void selectRow(int rowIndex, GuiSession session) {
            var table = (GuiTableControl)session.FindById(id);
            var rows = table.Rows;
            var row = (GuiTableRow)rows.ElementAt(rowIndex);
            row.Selected = true;
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

        public void print()
        {
            Console.WriteLine();
            Console.WriteLine($"Rows: {totalRows}");
            Console.Write("Columns: " + string.Join(", ", columnTitles));
        }
    }
}
