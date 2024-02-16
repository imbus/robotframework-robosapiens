namespace RoboSAPiens {
    public sealed class ButtonStore: ComponentRepository<Button> {
        public Button? get(ILocator locator) {
            return locator switch {
                HLabel(var label) => 
                    getByHLabel(label) ??
                    getByTooltip(label),
                _ => null
            };
        }

        public Button? get(CellLocator locator, TextCellStore rowLabels) {
            return getCell(locator, rowLabels);
        }
    }

    public sealed class CheckBoxStore: ComponentRepository<CheckBox> {
        public CheckBox? get(ILocator locator, LabelStore labels, TextFieldStore textFieldLabels) {
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
                _ => null
            };
        }

        public CheckBox? get(CellLocator locator, TextCellStore rowLabels) {
            return getCell(locator, rowLabels);
        }
    }

    public sealed class ComboBoxStore: ComponentRepository<ComboBox> {
        public ComboBox? get(ILocator locator, LabelStore labels, TextFieldStore textFieldLabels) {
            return locator switch {
                HLabel(var label) => 
                    getByHLabel(label) ?? 
                    getByTooltip(label) ??
                    getHorizontalClosestToLabel(label, labels, textFieldLabels),
                _ => null
            };
        }

        public ComboBox? get(CellLocator locator, TextCellStore rowLabels) {
            return getCell(locator, rowLabels);
        }
    }

    public sealed class LabelStore: ComponentRepository<SAPLabel> {
        public SAPLabel? get(ILocator locator, LabelStore labels, TextFieldStore textFieldLabels) {
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
        public RadioButton? get(ILocator locator, LabelStore labels, TextFieldStore textFieldLabels) {
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

        public RadioButton? get(CellLocator locator, TextCellStore rowLabels){
            return getCell(locator, rowLabels);
        }
    }

    public sealed class BoxStore: ContainerRepository<SAPBox> {}

    public sealed class GridViewStore: Repository<SAPGridView> {}

    public sealed class TabStore: ContainerRepository<SAPTab> {}

    public sealed class TableStore: Repository<SAPTable> {}

    public sealed class TextCellStore: Repository<TextCell> {
        public TextCell? getByContent(string text) {
            return items.Find(cell => cell.contains(text));
        }

        public TextCell? get(ILocator locator) {
            return locator switch {
                Content(var content) => getByContent(content),
                CellLocator cellLocator => filterBy<ILocatableCell>().Find(cell => cell.isLocated(cellLocator, this)) as TextCell,
                _ => null
            };
        }
    }

    public sealed class TextFieldStore: TextFieldRepository {
        public SAPTextField? get(ILocator locator, LabelStore labels, BoxStore boxes) {
            return getTextField(locator, labels, boxes);
        }
    }
}
