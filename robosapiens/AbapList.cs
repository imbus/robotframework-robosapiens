using System.Collections.Generic;
using sapfewse;

namespace RoboSAPiens
{
    public class AbapList : ITable
    {
        CellRepository cells;
        public List<string> columnTitles;
        public string id { get; }
        public int rowCount;

        public AbapList(GuiSimpleContainer container)
        {
            this.cells = new CellRepository();
            this.columnTitles = getColumnTitles(container);
            this.id = container.Id;
            this.rowCount = getRowCount(container);
        }

        public void classifyCells(GuiSession session)
        {
            var table = (GuiSimpleContainer)session.FindById(id);

            for (int i = 0; i < table.Children.Length; i++)
            {
                var component = table.Children.ElementAt(i);

                if (component.Type == "GuiSimpleContainer")
                {
                    var container = (GuiSimpleContainer)component;
                    classifyRow(container);
                } 
            }
        }

        public void classifyRow(GuiSimpleContainer container)
        {
            var containerType = container.GetListProperty("ContainerType");

            for (int i = 0; i < container.Children.Length; i++)
            {
                var element = container.Children.ElementAt(i);

                if (element.Type == "GuiCheckBox")
                {
                    var checkbox = (GuiCheckBox)element;
                    var colTitle = checkbox.GetListProperty("FieldHeader").Trim();

                    if (colTitle == "")
                    {
                        colTitle = columnTitles[i];
                    }

                    int rowNumber = -1;

                    if (containerType == "R")
                    {
                        var rowNo = checkbox.GetListProperty("RowNo");
                        if (rowNo != "") rowNumber = int.Parse(rowNo);
                    }

                    if (colTitle != "")
                    {
                        var colIndex = columnTitles.IndexOf(colTitle);

                        cells.Add(new ListCell(
                            checkbox.Id,
                            rowNumber - 1,
                            colIndex,
                            new List<string> { colTitle },
                            CellType.CheckBox,
                            new List<string>()
                        ));
                    }
                }

                if (element.Type == "GuiLabel")
                {
                    var label = (GuiLabel)element;
                    var colTitle = label.GetListProperty("FieldHeader").Trim();
                    var labels = new List<string>();

                    if (label.Text != "")
                        labels.Add(label.Text.Trim());

                    if (label.Tooltip != "")
                        labels.Add(label.Tooltip.Trim());

                    int rowNumber = -1;

                    if (containerType == "G")
                    {
                        var groupNo = label.GetListProperty("GroupNo");
                        if (groupNo != "") rowNumber = int.Parse(groupNo);
                    }

                    if (containerType == "R")
                    {
                        var rowNo = label.GetListProperty("RowNo");
                        if (rowNo != "") rowNumber = int.Parse(rowNo);
                    }

                    if (colTitle != "")
                    {
                        var colIndex = columnTitles.IndexOf(colTitle);

                        cells.Add(new ListCell(
                            label.Id,
                            rowNumber - 1,
                            colIndex,
                            new List<string> { colTitle },
                            CellType.Text,
                            labels
                        ));
                    }
                }
            }
        }

        public Cell? findCell(ILocator locator, GuiSession session)
        {
            if (cells.Count == 0) classifyCells(session);

            return locator switch
            {
                Content(string content) =>
                    cells.findCellByContent(content),
                LabelColumnLocator(string label, string column, int offset) =>
                    cells.findCellByLabelAndColumn(label, column),
                RowColumnLocator(int row, string column, int offset) =>
                    cells.findCellByRowAndColumn(row, column),
                _ => null
            };
        }
        
        List<string> getColumnTitles(GuiSimpleContainer container)
        {
            var columnTitles = new List<string>();
            var containerType = container.GetListProperty("ContainerType");

            if (containerType == "T")
            {
                for (int i = 0; i < container.Children.Length; i++)
                {
                    var component = container.Children.ElementAt(i);

                    if (component.Type == "GuiLabel")
                    {
                        var label = (GuiLabel)component;
                        string columnTitle = "";

                        if (label.Text != "")
                        {
                            columnTitle = label.Text.Trim();
                        }
                        else
                        {
                            if (label.Tooltip != "")
                            {
                                columnTitle = label.Tooltip.Trim();
                            }
                        }

                        var tableGroupsTotal = container.GetListProperty("TableGroupsTotal");
                        var labelType = label.GetListProperty("LabelType");

                        if (tableGroupsTotal != "")
                        {
                            if (labelType == "A") columnTitles.Add(columnTitle);
                        }
                        else
                        {
                            if (labelType == "N") columnTitles.Add(columnTitle);
                        }
                    }
                }
            }

            if (containerType == "G")
            {
                for (int i = 0; i < container.Children.Length; i++)
                {
                    var component = container.Children.ElementAt(i);

                    if (component.Type == "GuiSimpleContainer")
                    {
                        var cont = (GuiSimpleContainer)component;
                        var contType = cont.GetListProperty("ContainerType");

                        if (contType == "R")
                        {
                            for (int colIndex = 0; colIndex < cont.Children.Length; colIndex++)
                            {
                                var col = cont.Children.ElementAt(colIndex);

                                if (col.Type == "GuiLabel")
                                {
                                    var label = (GuiLabel)col;
                                    var colTitle = label.GetListProperty("FieldHeader").Trim();

                                    if (colTitle != "")
                                    {
                                        columnTitles.Add(colTitle);
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }

            return columnTitles;
        }

        public int getNumRows(GuiSession session)
        {
            return rowCount;
        }

        int getRowCount(GuiSimpleContainer container)
        {
            var containerType = container.GetListProperty("ContainerType");
            int rowCount = 0;

            if (containerType == "G")
            {
                var rowsTotal = container.GetListProperty("RowsTotal");
                if (rowsTotal != "")
                {
                    rowCount = int.Parse(rowsTotal);
                }
            }

            if (containerType == "T")
            {
                var tableGroupsTotal = container.GetListProperty("TableGroupsTotal");

                if (tableGroupsTotal != "")
                {
                    rowCount = int.Parse(tableGroupsTotal);
                }
                else
                {
                    var rowsTotal = container.GetListProperty("RowsTotal");

                    if (rowsTotal != "")
                    {
                        rowCount = int.Parse(rowsTotal);
                    }
                }
            }

            return rowCount;
        }

        public bool hasColumn(string column)
        {
            return columnTitles.Contains(column);
        }

        public bool rowIsAbove(GuiSession session, int rowIndex)
        {
            return false;
        }

        public bool rowIsBelow(GuiSession session, int rowIndex)
        {
            return false;
        }

        public bool scrollOnePage(GuiSession session)
        {
            return false;
        }

        public void selectColumn(string column, GuiSession session)
        {
            var table = (GuiSimpleContainer)session.FindById(id);

            for (int i = 0; i < table.Children.Length; i++)
            {
                var component = table.Children.ElementAt(i);

                if (component.Type == "GuiLabel")
                {
                    var label = (GuiLabel)component;

                    if (label.Text.Trim() == column || label.Tooltip.Trim() == column)
                    {
                        label.SetFocus();
                        session.ActiveWindow.SendVKey(2);
                        return;
                    }
                }
            }
        }

        public void selectRow(int rowNumber0, GuiSession session)
        {
            var table = (GuiSimpleContainer)session.FindById(id);

            for (int i = 0; i < table.Children.Length; i++)
            {
                var component = table.Children.ElementAt(i);

                if (component.Type == "GuiSimpleContainer")
                {
                    var container = (GuiSimpleContainer)component;
                    var containerType = container.GetListProperty("ContainerType");

                    if (containerType == "R" || containerType == "G")
                    {
                        var row = container;
                        var firstColumn = row.Children.ElementAt(0);

                        if (firstColumn.Type == "GuiLabel")
                        {
                            var firstCell = (GuiLabel)firstColumn;

                            int rowIndex = -1;

                            if (containerType == "R")
                            {
                                var rowNo = firstCell.GetListProperty("RowNo");
                                if (rowNo != "") rowIndex = int.Parse(rowNo);
                            }

                            if (containerType == "G")
                            {
                                var groupNo = firstCell.GetListProperty("GroupNo");
                                if (groupNo != "") rowIndex = int.Parse(groupNo);
                            }

                            if (rowIndex == rowNumber0 + 1)
                            {
                                firstCell.SetFocus();
                                session.ActiveWindow.SendVKey(2);
                                return;
                            }
                        }
                    }
                }
            }
        }

        public void selectRows(List<int> rowIndices, GuiSession session)
        {
        }
    }
}
