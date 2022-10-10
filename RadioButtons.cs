using sapfewse;

namespace RoboSAPiens {
    public abstract class RadioButton {
        public abstract void select(GuiSession session);
    }

    public class SAPRadioButton: RadioButton, ILabeled, ILocatable, ISelectable {
        string id;
        Position position;
        string text;

        public SAPRadioButton(GuiRadioButton radioButton) {
            this.id = radioButton.Id;
            this.position = new Position(height: radioButton.Height, 
                                         left: radioButton.ScreenLeft,
                                         top: radioButton.ScreenTop, 
                                         width: radioButton.Width);
            this.text = radioButton.Text;
        }

        public Position getPosition() {
            return position;
        }

        public bool isHorizontalAlignedWithLabel(SAPLabel? label) {
            return label switch {
                SAPLabel => label.position.horizontalAlignedWith(position),
                _ => false
            };
        }

        public bool isHorizontalAlignedWithTextField(SAPTextField? textField) {
            return textField switch {
                SAPTextField => textField.position.horizontalAlignedWith(position),
                _ => false
            };
        }

        public bool isLabeled(string label) {
            return text == label;
        }

        public bool isLocated(ILabelsLocator locator, LabelStore labels, ReadOnlyTextFieldStore textFieldLabels) {
            return locator switch {
                HLabelVLabel(var hLabel, var vLabel) =>
                    (isHorizontalAlignedWithLabel(labels.get(hLabel)) ||
                    isHorizontalAlignedWithTextField(textFieldLabels.getByContent(hLabel))) &&
                    isVerticalAlignedWithLabel(labels.get(vLabel)),
                _ => false
            };
        }

        public bool isVerticalAlignedWithLabel(SAPLabel? label) {
            return label switch {
                SAPLabel => label.position.verticalAlignedWith(position),
                _ => false
            };
        }

        public override void select(GuiSession session) {
            var guiRadioButton = (GuiRadioButton)session.FindById(id);
            guiRadioButton.Select();
        }
    }
}
