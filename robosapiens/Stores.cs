using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace RoboSAPiens {
    public sealed class ButtonStore: ComponentRepository<Button> {
        Button? findInTab(string label, string tabTitle, TabStore tabs) {
            var tab = tabs.get(tabTitle);

            return Find(
                button => (button.isHLabeled(label) || button.hasTooltip(label)) && 
                          (tab?.contains(button.position) ?? false));
        }

        public Button? get(ILocator locator, LabelStore labels, TextFieldRepository textFieldLabels, TabStore tabs, bool exact) {
            return locator switch {
                HLabel(var label) => 
                    getByHLabel(label) ??
                    getByTooltip(exact? label : label + "~"),
                HLabelHLabel =>
                    getAlignedWithLabels((HLabelHLabel)locator, labels, textFieldLabels),
                HLabelVLabel(var label, var tabTitle) => 
                    findInTab(label, tabTitle, tabs),
                _ => null
            };
        }
    }

    public sealed class CheckBoxStore: ComponentRepository<SAPCheckBox> {
        public SAPCheckBox? get(ILocator locator, LabelStore labels, TextFieldRepository textFieldLabels, BoxStore boxes) {
            return locator switch {
                HLabel(var label) => 
                    getByHLabel(label) ?? 
                    getByTooltip(label) ??
                    getHorizontalClosestToLabel(label, labels, textFieldLabels),
                VLabel(var label) => 
                    getByVLabel(label) ??
                    getVerticalClosestToLabel(label, labels, textFieldLabels) ??
                    getNearLabel(label, labels, textFieldLabels),
                HLabelVLabel => getAlignedWithLabels((HLabelVLabel)locator, labels, textFieldLabels),
                HIndexVLabel(int rowIndex, string label, int gridIndex) => getFromVerticalGrid(rowIndex, gridIndex, label, labels, boxes),
                _ => null
            };
        }

        SAPCheckBox? getFromVerticalGrid(int rowIndex, int gridIndex, string label, LabelStore labels, BoxStore boxes) 
        {
            var checkBoxes = 
                this
                .Where(_ => Regex.IsMatch(_.id, @"\[\d+,\d+\]$", RegexOptions.Compiled))
                .ToList();
            var firstCheckBox = checkBoxes.First();
            var columnTitles = 
                labels
                .Where(label => label.text != "")
                .Where(label => label.position.top > boxes.First().position.top)
                .Where(label => label.position.top < firstCheckBox.position.top)
                .GroupBy(label => label.position.top)
                .Select(group => group.ToList())
                .ToList();
            var columnTitle = 
                columnTitles
                .SelectMany((grid, gridIndex) => grid.Select(colTitle => new {colTitle, gridIndex}))
                .FirstOrDefault(_ => _.gridIndex == gridIndex && _.colTitle.text == label)
                ?.colTitle;

            if (columnTitle == null) return null;

            var column = 
                checkBoxes
                .Where(_ => Math.Abs(_.position.left - columnTitle.position.left) < 4)
                .GroupBy(_ => new {width=_.position.right - _.position.left})
                .Select(g => g.OrderBy(_ => _.position.top).ToList())
                .ToList();
            var checkBox = 
                column
                .SelectMany((grid, gridIndex) => grid.Select((checkBox, rowIndex) => new {checkBox, rowIndex, gridIndex}))
                .FirstOrDefault(_ => _.rowIndex == rowIndex-1 && _.gridIndex == gridIndex)
                ?.checkBox;
            
            return checkBox;
        }
    }

    public sealed class ComboBoxStore: ComponentRepository<ComboBox> {
        public ComboBox? get(ILocator locator, LabelStore labels, TextFieldRepository textFieldLabels) {
            return locator switch {
                HLabel(var label) => 
                    getByHLabel(label) ?? 
                    getByTooltip(label) ??
                    getHorizontalClosestToLabel(label, labels, textFieldLabels),
                HLabelVLabel => getAlignedWithLabels((HLabelVLabel)locator, labels, textFieldLabels),
                _ => null
            };
        }
    }

    public sealed class LabelStore: ComponentRepository<SAPLabel> {
        public SAPLabel? get(ILocator locator, LabelStore labels, TextFieldRepository textFieldLabels) {
            return locator switch {
                HLabel(var label) => getHorizontalClosestToLabel(label, labels, textFieldLabels),
                VLabel(var label) => getVerticalClosestToLabel(label, labels, textFieldLabels),
                Content(var name) => getByName(name),
                _ => null
            };
        }

        public SAPLabel? getByName(string name)
        {
            (string label, int labelIndex) = name.Split("__") switch
            {
                [string text, string stringIndex] when int.TryParse(stringIndex, out int intIndex) => (text, intIndex - 1),
                _ => (name, 0)
            };

            var labels = FindAll(_ => _.contains(label));

            if (labels.Count > labelIndex) return labels[labelIndex];

            return null;
        }
    }

    public sealed class RadioButtonStore: ComponentRepository<RadioButton> {
        public RadioButton? get(ILocator locator, LabelStore labels, TextFieldRepository textFieldLabels) {
            return locator switch {
                HLabel(var label) => 
                    getByHLabel(label) ??
                    getByTooltip(label) ?? 
                    getHorizontalClosestToLabel(label, labels, textFieldLabels),
                VLabel(var label) => 
                    getByVLabel(label) ??
                    getVerticalClosestToLabel(label, labels, textFieldLabels),
                HLabelVLabel => getAlignedWithLabels((HLabelVLabel)locator, labels, textFieldLabels),
                _ => null
            };
        }
    }

    public sealed class BoxStore: ContainerRepository<SAPBox> {}

    public sealed class TabStore: ContainerRepository<SAPTab> {}

    public sealed class MenuItemStore: ContainerRepository<SAPMenu> {}

    public sealed class TableStore: Repository<ITable> {}

    public sealed class TextFieldStore
    {
        TextFieldRepository nonChangeableTextFields = new TextFieldRepository();
        TextFieldRepository textFields = new TextFieldRepository();

        public void Add(SAPTextField textField)
        {
            textFields.Add(textField);

            if (!textField.changeable)
            {
                nonChangeableTextFields.Add(textField);
            }
        }

        public TextFieldRepository GetAll()
        {
            return textFields;
        }
    
        public TextFieldRepository NonChangeable()
        {
            return nonChangeableTextFields;
        }

        public SAPTextField? get(ILocator locator, LabelStore labels, BoxStore boxes, bool exact) 
        {
            return textFields.getTextField(locator, labels, nonChangeableTextFields, boxes, exact);
        }
    }

    public sealed class TreeElementStore: ContainerRepository<SAPTreeElement> {}
}
