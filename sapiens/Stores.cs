namespace SAPiens {
    public sealed class ButtonStore: ComponentRepository<Button> {
        public Button? get(ILocator locator) {
            return locator switch {
                HLabel(var label) => getByLabelOrTooltip(label),
                FilledCellLocator cellLocator => getCell(cellLocator),
                _ => null
            };
        }
    }

    public sealed class CheckBoxStore: ComponentRepository<CheckBox> {
        public CheckBox? get(ILocator locator, LabelStore labels, ReadOnlyTextFieldStore textFieldLabels) {
            return locator switch {
                HLabel(var label) => getByLabelOrTooltip(label) ?? 
                                     getHorizontalClosestToLabel(label, labels, textFieldLabels),
                VLabel(var label) => getVerticalClosestToLabel(label, labels, textFieldLabels) ??
                                     getNearLabel(label, labels, textFieldLabels),
                HLabelVLabel => getAlignedWithLabels((HLabelVLabel)locator, labels, textFieldLabels),
                FilledCellLocator cellLocator => getCell(cellLocator),
                _ => null
            };
        }
    }

    public sealed class ComboBoxStore: ComponentRepository<ComboBox> {
        public ComboBox? get(ILocator locator) {
            return locator switch {
                HLabel(var label) => getByLabelOrTooltip(label),
                FilledCellLocator cellLocator => getCell(cellLocator),
                _ => null
            };
        }
    }

    public sealed class LabelStore: ComponentRepository<SAPLabel> {
        public SAPLabel? get(ILocator locator, LabelStore labels, ReadOnlyTextFieldStore textFieldLabels) {
            return locator switch {
                HLabel(var label) => getHorizontalClosestToLabel(label, labels, textFieldLabels),
                VLabel(var label) => getVerticalClosestToLabel(label, labels, textFieldLabels),
                Content(var name) => getByName(name),
                _ => null
            };
        }

        public SAPLabel? getByName(string name){
            return items.Find(label => label.contains(name));
        }
    }

    public sealed class RadioButtonStore: ComponentRepository<RadioButton> {
        public RadioButton? get(ILocator locator, LabelStore labels, ReadOnlyTextFieldStore textFieldLabels) {
            return locator switch {
                HLabel(var label) => getByLabelOrTooltip(label) ?? 
                                     getHorizontalClosestToLabel(label, labels, textFieldLabels),
                VLabel(var label) => getVerticalClosestToLabel(label, labels, textFieldLabels),
                HLabelVLabel => getAlignedWithLabels((HLabelVLabel)locator, labels, textFieldLabels),
                FilledCellLocator cellLocator => getCell(cellLocator),
                _ => null
            };
        }
    }

    public sealed class BoxStore: ContainerRepository<SAPBox> {}

    public sealed class TabStore: ContainerRepository<SAPTab> {}

    public sealed class TableStore: Repository<SAPTable> {}

    public sealed class EditableCellStore: EditableTextCellRepository {
        public Cell? get(ILocator locator, LabelCellStore rowLabels) {
            return locator switch {
                EmptyCellLocator cellLocator => getEmptyCell(cellLocator, rowLabels),
                FilledCellLocator cellLocator => getFilledCell(cellLocator),
                _ => null
            };
        }
    }

    public sealed class LabelCellStore: TextCellRepository {
        public Cell? get(ILocator locator) {
            return locator switch {
                Content(var content) => getByContent(content),
                FilledCellLocator cellLocator => getFilledCell(cellLocator),
                _ => null
            };
        }
    }

    public sealed class EditableTextFieldStore: TextFieldRepository<EditableTextField> {
        public EditableTextField? get(ILocator locator, LabelStore labels, ReadOnlyTextFieldStore textFieldLabels, BoxStore boxes) {
            return getTextField(locator, labels, textFieldLabels, boxes) as EditableTextField;
        }
    }

    public sealed class ReadOnlyTextFieldStore: TextFieldRepository<SAPTextField> {
        public SAPTextField? get(ILocator locator, LabelStore labels, ReadOnlyTextFieldStore textFieldLabels, BoxStore boxes) {
            return getTextField(locator, labels, textFieldLabels, boxes) as SAPTextField;
        }
    }
}
