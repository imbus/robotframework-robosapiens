using System;
using System.Collections.Generic;
using System.Linq;
using sapfewse;

namespace RoboSAPiens 
{
    public class SAPGridView: ITable
    {
        string id;
        int rowCount;

        public SAPGridView(GuiGridView guiGridView) {
            id = guiGridView.Id;
            rowCount = guiGridView.RowCount;
        }

        List<List<string>> getColumnTitles(GuiGridView gridView) 
        {
            var allColumnTitles = new List<List<string>>();
            var columnCount = gridView.ColumnCount;
            var columnIds = (GuiCollection)gridView.ColumnOrder;

            for (int column = 0; column < columnCount; column++) 
            {
                var thisColumnTitles = new List<string>();
                var columnId = (string)columnIds.ElementAt(column);
                thisColumnTitles.Add(gridView.GetColumnTooltip(columnId).Trim());

                GuiCollection columnTitles = (GuiCollection)gridView.GetColumnTitles(columnId);
                for (int i = 0; i < columnTitles.Length; i++) 
                {
                    thisColumnTitles.Add((string)columnTitles.ElementAt(i));
                }

                allColumnTitles.Add(thisColumnTitles);
            }

            return allColumnTitles;
        }

        public int getNumRows(GuiSession session)
        {
            var gridView = (GuiGridView)session.FindById(id);
            return gridView.RowCount;
        }

        // https://answers.sap.com/questions/290321/saving-extracted-sap-report-in-excel-format-into-s.html
        public void exportSpreadsheet(GuiSession session) 
        {
            GuiGridView guiGridView = (GuiGridView)session.FindById(id);
            guiGridView.PressToolbarContextButton("&MB_EXPORT");
            guiGridView.SelectContextMenuItem("&XXL");
        }

        public void classifyCells(GuiSession session, CellRepository repo) 
        {
            var gridView = (GuiGridView)session.FindById(id);
            var columnCount = gridView.ColumnCount;
            var firstRow = gridView.FirstVisibleRow;
            var rowCount = gridView.VisibleRowCount;
            var columnIds = (GuiCollection)gridView.ColumnOrder;

            for (int row = firstRow; row < firstRow + rowCount; row++) 
            {
                for (int column = 0; column < columnCount; column++) 
                {
                    var columnId = (string)columnIds.ElementAt(column);
                    string type;
                    
                    try {
                        type = gridView.GetCellType(row, columnId);
                    }
                    // When a row is empty an exception is thrown
                    catch (Exception) {
                        continue;
                    }

                    switch (type) 
                    {
                        case "Button":
                            repo.buttons.Add(new SAPGridViewButton(columnId, gridView, row));
                            break;
                        case "CheckBox":
                            repo.checkBoxes.Add(new SAPGridViewCheckBox(columnId, gridView, row));
                            break;
                        case "Normal":
                            repo.textCells.Add(new SAPGridViewCell(columnId, gridView, row));
                            break;
                        case "ValueList":
                            repo.comboBoxes.Add(new GridViewValueList(columnId, gridView, row));
                            break;
                    }
                }
            }
        }

        public void pressKey(string key, GuiSession session) 
        {
            GuiGridView guiGridView = (GuiGridView)session.FindById(id);

            if (key == "F1") guiGridView.PressF1();
            if (key == "F4") guiGridView.PressF4();
        }

        public void selectRow(int rowIdx, GuiSession session)
        {
            GuiGridView guiGridView = (GuiGridView)session.FindById(id);
            guiGridView.CurrentCellRow = rowIdx;
            guiGridView.SelectedRows = $"{rowIdx}";
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

        public void print(GuiGridView gridView)
        {
            var allColumnTitles = getColumnTitles(gridView);
            Console.WriteLine();
            Console.WriteLine($"Rows: {rowCount}");
            Console.Write("Columns: " + string.Join(", ", allColumnTitles.Select(columnTitles => $"[{string.Join(", ", columnTitles)}]")));
        }
    }
}
