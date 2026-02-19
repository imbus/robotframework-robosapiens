using System;
using System.Collections.Generic;
using System.Linq;
using sapfewse;

namespace RoboSAPiens
{
    public enum CellType
    {
        Button,
        CheckBox,
        ComboBox,
        Label,
        Link,
        RadioButton,
        Text
    }
    
    public abstract record Cell(
        int rowIndex, 
        int colIndex,
        List<string> columnTitles,
        CellType type, 
        List<string> labels
    )
    {
        protected bool focused = false;
        public abstract void click(GuiSession session);
        public abstract void doubleClick(GuiSession session);
        public abstract string getValue(GuiSession session);
        public abstract void highlight(GuiSession session);
        public abstract bool isChangeable(GuiSession session);
        public abstract void setValue(string value, GuiSession session);
        public abstract void tickCheckbox(GuiSession session);
        public abstract void untickCheckbox(GuiSession session);

        public bool inColumn(string column)
        {
            return columnTitles.Any(title => title.Equals(column));
        }

        public bool inRow(int row)
        {
            return row == rowIndex;
        }

        public bool isLabeled(string label)
        {
            if (label.EndsWith("~"))
            {
                return labels.Any(l => l.StartsWith(label.TrimEnd('~')));
            }

            return labels.Any(l => l.Equals(label));
        }

        public bool isButtonCell()
        {
            return type == CellType.Button;
        }

        public bool isTextCell()
        {
            return type == CellType.Text;
        }

        public sealed override string ToString()
        {
            return JSON.serialize(this, GetType());
        }
    }

    public record ListCell(
        string id,
        int rowIndex,
        int colIndex,
        List<string> columnTitles,
        CellType type,
        List<string> labels
    ) : Cell(rowIndex, colIndex, columnTitles, type, labels)
    {
        public override void click(GuiSession session)
        {
            var cell = (GuiVComponent)session.FindById(id);
            cell.SetFocus();
        }

        public override void doubleClick(GuiSession session)
        {
            var cell = (GuiVComponent)session.FindById(id);
            cell.SetFocus();
            session.ActiveWindow.SendVKey(2);
        }

        public override string getValue(GuiSession session)
        {
            var cell = (GuiLabel)session.FindById(id);
            return cell.Text;
        }

        public override void highlight(GuiSession session)
        {
            focused = !focused;
            var cell = (GuiVComponent)session.FindById(id);
            cell.Visualize(focused);
        }

        public override bool isChangeable(GuiSession session)
        {
            var cell = (GuiVComponent)session.FindById(id);
            return cell.Changeable;
        }

        public override void setValue(string value, GuiSession session)
        {
        }

        public override void tickCheckbox(GuiSession session)
        {
            var cell = (GuiVComponent)session.FindById(id);
            var checkbox = (GuiCheckBox)cell;
            checkbox.Selected = true;
        }

        public override void untickCheckbox(GuiSession session)
        {
            var cell = (GuiVComponent)session.FindById(id);
            var checkbox = (GuiCheckBox)cell;
            checkbox.Selected = false;
        }
    }

    public record GridViewCell(
        int rowIndex,
        int colIndex,
        string columnId,
        List<string> columnTitles,
        CellType type,
        List<string> labels,
        string gridViewId
    ): Cell(rowIndex, colIndex, columnTitles, type, labels)
    {
        public override void click(GuiSession session)
        {
            var gridView = (GuiGridView)session.FindById(gridViewId);

            switch(type)
            {
                case CellType.Button:
                    gridView.PressButton(rowIndex, columnId);
                    break;
                case CellType.Link:
                case CellType.RadioButton:
                    gridView.Click(rowIndex, columnId);
                    break;
                case CellType.Text:
                    gridView.SetCurrentCell(rowIndex, columnId);
                    gridView.SelectedRows = rowIndex.ToString();

                    try
                    {
                        // It may fail in some cells
                        gridView.ClickCurrentCell();
                    }
                    catch
                    {
                        
                    }
                    break;
            }
        }

        public override void doubleClick(GuiSession session)
        {
            var gridView = (GuiGridView)session.FindById(gridViewId);
            gridView.DoubleClick(rowIndex, columnId);
        }

        public override string getValue(GuiSession session)
        {
            var gridView = (GuiGridView)session.FindById(gridViewId);
            return gridView.GetCellValue(rowIndex, columnId);
        }

        // The innerObject parameter of the Visualize method of GuiGridView
        // can only take the values "Toolbar" and "Cell(row,column)".
        // References:
        // https://community.sap.com/t5/technology-q-a/get-innerobject-for-visualizing/qaq-p/12150835
        // https://www.synactive.com/download/sap%20gui%20scripting/sap%20gui%20scripting%20api.pdf

        public override void highlight(GuiSession session)
        {
            focused = !focused;
            var gridView = (GuiGridView)session.FindById(gridViewId);
            gridView.Visualize(focused, $"Cell({rowIndex+1},{columnId})");
        }

        public override bool isChangeable(GuiSession session)
        {
            var gridView = (GuiGridView)session.FindById(gridViewId);

            return type switch
            {
                // For button cells GetCellChangeable cannot be used
                // to determine whether the button is enabled
                CellType.Button => true,
                _ => gridView.GetCellChangeable(rowIndex, columnId)
            };
        }

        public override void setValue(string value, GuiSession session)
        {
            var gridView = (GuiGridView)session.FindById(gridViewId);
            gridView.ModifyCell(rowIndex, columnId, value);
        }

        public override void tickCheckbox(GuiSession session)
        {
            var gridView = (GuiGridView)session.FindById(gridViewId);
            gridView.ModifyCheckBox(rowIndex, columnId, true);
        }

        public override void untickCheckbox(GuiSession session)
        {
            var gridView = (GuiGridView)session.FindById(gridViewId);
            gridView.ModifyCheckBox(rowIndex, columnId, false);
        }
    }

    public record TableCell(
        int rowIndex,
        int colIndex,
        string id,
        List<string> columnTitles,
        CellType type,
        List<string> labels,
        string tableId
    ): Cell(rowIndex, colIndex, columnTitles, type, labels)
    {
        public override void click(GuiSession session)
        {
            var cell = session.FindById(id);
            switch(type)
            {
                case CellType.Button:
                    new SAPButton((GuiButton)cell).push(session);
                    break;
                case CellType.Label:
                    ((GuiVComponent)cell).SetFocus();
                    break;
                case CellType.Text:
                    new SAPTextField((GuiTextField)cell).select(session);
                    break;
                case CellType.RadioButton:
                    new SAPRadioButton((GuiRadioButton)cell).select(session);
                    break;
            }
        }

        public override void doubleClick(GuiSession session)
        {
            var cell = session.FindById(id);
            switch(type)
            {
                case CellType.Text:
                    new SAPTextField((GuiTextField)cell).doubleClick(session);
                    break;
            }
        }

        public override string getValue(GuiSession session)
        {
            var cell = session.FindById(id);
            
            return type switch
            {
                CellType.ComboBox => new SAPComboBox((GuiComboBox)cell).getText(session),
                CellType.Label => new SAPLabel((GuiLabel)cell).text,
                CellType.Text => new SAPTextField((GuiTextField)cell).getText(session),
                _ => throw new NotImplementedException()
            };
        }

        public override void highlight(GuiSession session)
        {
            focused = !focused;
            ((GuiVComponent)session.FindById(id)).Visualize(focused);
        }

        public override bool isChangeable(GuiSession session)
        {
            return ((GuiVComponent)session.FindById(id)).Changeable;
        }

        public override void setValue(string value, GuiSession session)
        {
            var cell = session.FindById(id);
            switch(type)
            {
                case CellType.Text:
                    new SAPTextField((GuiTextField)cell).insert(value, session);
                    break;
                case CellType.ComboBox:
                    new SAPComboBox((GuiComboBox)cell).select(value, session);
                    break;
            }
        }

        public override void tickCheckbox(GuiSession session)
        {
            var cell = session.FindById(id);
            var checkbox = new SAPCheckBox((GuiCheckBox)cell);
            checkbox.select(session);
        }

        public override void untickCheckbox(GuiSession session)
        {
            var cell = session.FindById(id);
            var checkbox = new SAPCheckBox((GuiCheckBox)cell);
            checkbox.deselect(session);
        }
    }

    public record TreeCell(
        int rowIndex,
        int colIndex,
        string textPath,
        string nodeKey,
        string columnName,
        List<string> columnTitles,
        CellType type,
        List<string> labels,
        string treeId
    ) : Cell(rowIndex, colIndex, columnTitles, type, labels)
    {
        public override void click(GuiSession session)
        {
            var tree = (GuiTree)session.FindById(treeId);

            switch(type)
            {
                case CellType.Button:
                    tree.PressButton(nodeKey, columnName);
                    break;
                case CellType.Link:
                    tree.ClickLink(nodeKey, columnName);
                    break;
                case CellType.Text:
                    select(tree);
                    break;
            }
        }

        public override void doubleClick(GuiSession session)
        {
            var tree = (GuiTree)session.FindById(treeId);
            tree.DoubleClickItem(nodeKey, columnName);
        }

        public override string getValue(GuiSession session)
        {
            var tree = (GuiTree)session.FindById(treeId);
            return tree.GetItemText(nodeKey, columnName);
        }

        public override void highlight(GuiSession session)
        {
            var tree = (GuiTree)session.FindById(treeId);
            select(tree);
        }

        public override bool isChangeable(GuiSession session)
        {
            var tree = (GuiTree)session.FindById(treeId);
            return !tree.GetIsDisabled(nodeKey, columnName);
        }

        public void select(GuiTree tree)
        { 
            switch (tree.GetSelectionMode()) 
            {
                // Single node
                case 0:
                    tree.SelectNode(nodeKey);
                    break;
                // Single item
                case 2:
                    tree.SelectItem(nodeKey, columnName);
                    break;
            }
        }

        public override void setValue(string value, GuiSession session)
        {
        }

        public override void tickCheckbox(GuiSession session)
        {
            var tree = (GuiTree)session.FindById(treeId);
            tree.ChangeCheckbox(nodeKey, columnName, true);
        }

        public override void untickCheckbox(GuiSession session)
        {
            var tree = (GuiTree)session.FindById(treeId);
            tree.ChangeCheckbox(nodeKey, columnName, false);
        }
    }
}
