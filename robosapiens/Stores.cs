namespace RoboSAPiens {
    public sealed class ButtonStore: ComponentRepository<Button> {
        public Button? get(ILocator locator, LabelStore labels, TextFieldStore textFieldLabels) {
            return locator switch {
                HLabel(var label) => 
                    getByHLabel(label) ??
                    getByTooltip(label),
                HLabelHLabel =>
                    getAlignedWithLabels((HLabelHLabel)locator, labels, textFieldLabels),
                _ => null
            };
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
            return Find(label => label.contains(name));
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
    }

    public sealed class BoxStore: ContainerRepository<SAPBox> {}

    public sealed class TabStore: ContainerRepository<SAPTab> {}

    public sealed class MenuItemStore: ContainerRepository<SAPMenu> {}

    public sealed class TableStore: Repository<ITable> {}

    public sealed class TextFieldStore: TextFieldRepository {
        public SAPTextField? get(ILocator locator, LabelStore labels, BoxStore boxes) {
            return getTextField(locator, labels, boxes);
        }
    }

    public sealed class TreeElementStore: ContainerRepository<SAPTreeElement> {}
}
