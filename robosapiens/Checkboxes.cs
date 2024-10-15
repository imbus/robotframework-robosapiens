using System.Collections.Generic;
using sapfewse;

namespace RoboSAPiens {
    public abstract class CheckBox: IHighlightable {
        protected bool focused;
        public abstract bool isEnabled(GuiSession session);
        public abstract bool isSelected(GuiSession session);
        public abstract void select(GuiSession session);
        public abstract void deselect(GuiSession session);
        public abstract void toggleHighlight(GuiSession session);
    }

    public class SAPCheckBox: CheckBox, ILabeled, ILocatable, ISelectable {
        string defaultTooltip;
        string id;
        Position position;
        string text;

        public SAPCheckBox(GuiCheckBox checkBox) {
            this.defaultTooltip = checkBox.DefaultTooltip.Trim();
            this.id = checkBox.Id;
            this.position = new Position(height: checkBox.Height, 
                                left: checkBox.ScreenLeft,
                                top: checkBox.ScreenTop, 
                                width: checkBox.Width);
            this.text = checkBox.Text;
        }

        public List<string> getLabels()
        {
            return new List<string>
            {
                defaultTooltip
            };
        }

        public Position getPosition() {
            return position;
        }

        public override bool isEnabled(GuiSession session)
        {
            var checkbox = (GuiCheckBox)session.FindById(id);
            return checkbox.Changeable;
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

        public bool isVerticalAlignedWithLabel(SAPLabel? label) {
            return label switch {
                SAPLabel => label.position.verticalAlignedWith(position),
                _ => false
            };
        }

        public bool isHLabeled(string label) {
            return text == label;
        }

        public bool isVLabeled(string label) {
            return false;
        }

        public bool hasTooltip(string tooltip) {
            return defaultTooltip == tooltip;
        }

        public virtual bool isLocated(ILocator locator, LabelStore labels, TextFieldRepository textFieldLabels) {
            return locator switch {
                HLabelVLabel(var hLabel, var vLabel) =>
                isHorizontalAlignedWithLabel(labels.getByName(hLabel)) &&
                isVerticalAlignedWithLabel(labels.getByName(vLabel)),
                _ => false
            };
        }

        public override bool isSelected(GuiSession session) {
            var guiCheckBox = (GuiCheckBox)session.FindById(id);
            return guiCheckBox.Selected;
        }

        public override void select(GuiSession session) {
            var guiCheckBox = (GuiCheckBox)session.FindById(id);
            guiCheckBox.Selected = true;
        }

        public override void deselect(GuiSession session) {
            var guiCheckBox = (GuiCheckBox)session.FindById(id);
            guiCheckBox.Selected = false;
        }

        public override void toggleHighlight(GuiSession session) {
            focused = !focused;
            var guiCheckBox = (GuiCheckBox)session.FindById(id);
            guiCheckBox.Visualize(focused);
        }
    }
}
