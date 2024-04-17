using System;
using System.Collections.Generic;
using sapfewse;

namespace RoboSAPiens {
    public class SAPTable: ITable {
        public string id {get;}
        int visibleRowCount;
        public int totalRows;

        public SAPTable(GuiTableControl table) {
            this.id = table.Id;
            // https://answers.sap.com/questions/11100660/how-to-get-a-correct-row-count-in-sap-table.html
            this.totalRows = table.VerticalScrollbar.Maximum + 1;
            this.visibleRowCount = table.VisibleRowCount;
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
            var numRows = getVisibleRows();

            for (int colIdx = 0; colIdx < columns.Length; colIdx++) 
            {
                var column = (GuiTableColumn)columns.ElementAt(colIdx);
                var columnTitle = column.Title;

                for (int rowIdx = 0; rowIdx < numRows; rowIdx++) 
                {
                    GuiVComponent cell;

                    // Tables are not necessarily rectangular grids
                    // A column may have a hole. Holes are skipped
                    try {
                        cell = table.GetCell(rowIdx, colIdx);
                    }
                    catch (Exception) {
                        continue;
                    }

                    switch (cell.Type) {
                        case "GuiButton":
                            repo.buttons.Add(new SAPTableButton(columnTitle, rowIdx, (GuiButton)cell, this));
                            break;
                        case "GuiCheckBox":
                            repo.checkBoxes.Add(new SAPTableCheckBox(columnTitle, rowIdx, (GuiCheckBox)cell, this));
                            break;
                        case "GuiTextField":
                        case "GuiCTextField":
                            repo.textCells.Add(new SAPTableCell(columnTitle, rowIdx, (GuiTextField)cell, this));
                            break;
                        case "GuiComboBox":
                            repo.comboBoxes.Add(new SAPTableComboBox(columnTitle, rowIdx, (GuiComboBox)cell));
                            break;
                    }
                }
            }
        }

        public int getNumRows(GuiSession session) {
            var table = (GuiTableControl)session.FindById(id);
            return table.VerticalScrollbar.Maximum + 1;
        }

        public int getVisibleRows() {
            return visibleRowCount;
        }

        public void makeSureCellIsVisible(int rowIndex, GuiSession session) {
            if (rowIndex + 1 > visibleRowCount) {
                scrollOnePage(session);
            }
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

        public void print(GuiTableControl guiTableControl)
        {
            var columnTitles = getColumnTitles(guiTableControl);
            Console.WriteLine();
            Console.WriteLine($"Rows: {totalRows}");
            Console.Write("Columns: " + string.Join(", ", columnTitles));
        }
    }
}
