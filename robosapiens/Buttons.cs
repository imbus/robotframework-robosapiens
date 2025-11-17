using System.Collections.Generic;
using System.Text.RegularExpressions;
using sapfewse;

namespace RoboSAPiens {
    public abstract class Button: IHighlightable, ILabeled {
        protected bool focused;
        public Position position = new Position(0, 0, 0, 0);
        public abstract bool hasTooltip(string tooltip);
        public abstract bool isEnabled(GuiSession session);
        public abstract bool isHLabeled(string label);
        public abstract bool isVLabeled(string label);
        public abstract void push(GuiSession session);
        public abstract void toggleHighlight(GuiSession session);
    }

    public class SAPButton: Button, ILocatable {
        protected string defaultTooltip;
        public string id;
        public string text;
        protected string tooltip;

        public SAPButton(GuiButton button) {
            this.defaultTooltip = Regex.Replace(button.DefaultTooltip, @"\s\s+", " ").Trim();
            this.focused = false;
            this.id = button.Id;
            this.position = new Position(height: button.Height, 
                                left: button.ScreenLeft,
                                top: button.ScreenTop, 
                                width: button.Width
                            );
            this.text = button.Text.Trim();
            this.tooltip = Regex.Replace(button.Tooltip, @"\s\s+", " ").Trim();
        }

        public List<string> getLabels()
        {
            return new List<string>
            {
                defaultTooltip,
                text,
                tooltip
            };
        }

        public override bool isEnabled(GuiSession session)
        {
            var button = (GuiButton)session.FindById(id);
            return button.Changeable;
        }

        public override bool isHLabeled(string label) {
            return this.text == label;
        }

        public override bool isVLabeled(string label) {
            return false;
        }

        public override bool hasTooltip(string tooltip) {
            if (tooltip.EndsWith("~")) {
                return this.tooltip.StartsWith(tooltip.TrimEnd('~'));
            }

            return this.tooltip == tooltip;
        }

        public override void push(GuiSession session) {
            var guiButton = (GuiButton)session.FindById(id);
            guiButton.SetFocus();
            guiButton.Press();
        }

        public override void toggleHighlight(GuiSession session) {
            focused = !focused;
            var guiButton = (GuiButton)session.FindById(id);
            guiButton.Visualize(focused);
        }


        // To Do: Factor out this common function
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

        public bool isLocated(ILocator locator, LabelStore labels, TextFieldRepository textFieldLabels)
        {
            return locator switch {
                HLabelHLabel(var leftLabel, var rightLabel) =>
                    (isHorizontalAlignedWithLabel(labels.getByName(leftLabel)) || 
                    isHorizontalAlignedWithTextField(textFieldLabels.getByContent(leftLabel))) &&
                    (isHLabeled(rightLabel) || hasTooltip(rightLabel)),
                _ => false
            };
        }

        public Position getPosition()
        {
            return position;
        }
    }

    public sealed class SAPGridViewToolbarButton: Button {
        string gridViewId;
        string id;
        int buttonPos;
        string label;
        string tooltip;

        public SAPGridViewToolbarButton(GuiGridView gridView, int buttonPos) {
            this.focused = false;
            this.gridViewId = gridView.Id;
            this.id = gridView.GetToolbarButtonId(buttonPos);
            this.buttonPos = buttonPos;
            this.tooltip = gridView.GetToolbarButtonTooltip(buttonPos);
            this.label = gridView.GetToolbarButtonText(buttonPos);
        }

        public override bool isEnabled(GuiSession session)
        {
            var gridView = (GuiGridView)session.FindById(gridViewId);
            return gridView.GetToolbarButtonEnabled(buttonPos);
        }

        public override bool isHLabeled(string label) {
            return this.label.Equals(label);
        }

        public override bool isVLabeled(string label) {
            return false;
        }

        public override bool hasTooltip(string tooltip) {
            if (tooltip.EndsWith("~")) {
                return this.tooltip.StartsWith(tooltip.TrimEnd('~'));
            }

            return this.tooltip == tooltip;
        }
    
        public override void push(GuiSession session) {
            var gridView = (GuiGridView)session.FindById(gridViewId);
            gridView.PressToolbarButton(id);
        }

        // The innerObject parameter of the Visualize method of GuiGridView
        // can only take the values "Toolbar" and "Cell(row,column)".
        // Therefore only the whole toolbar can be highlighted.
        // References:
        // https://community.sap.com/t5/technology-q-a/get-innerobject-for-visualizing/qaq-p/12150835
        // https://www.synactive.com/download/sap%20gui%20scripting/sap%20gui%20scripting%20api.pdf
        public override void toggleHighlight(GuiSession session)
        {
            focused = !focused;
            var gridView = (GuiGridView)session.FindById(gridViewId);
            // Highlight the toolbar in order to know where the button is, even when the toolbar is hidden
            gridView.Visualize(focused, "Toolbar");
        }
    }

    public sealed class SAPGridViewToolbarButtonMenu: Button 
    {
        string gridViewId;
        string id;
        int buttonPos;
        string tooltip;

        public SAPGridViewToolbarButtonMenu(GuiGridView gridView, int buttonPos) 
        {
            this.buttonPos = buttonPos;
            this.gridViewId = gridView.Id;
            this.id = gridView.GetToolbarButtonId(buttonPos);
            this.tooltip = gridView.GetToolbarButtonTooltip(buttonPos);
        }

        public override bool isEnabled(GuiSession session)
        {
            var gridView = (GuiGridView)session.FindById(gridViewId);
            return gridView.GetToolbarButtonEnabled(buttonPos);
        }

        public override bool isHLabeled(string label) {
            return false;
        }

        public override bool isVLabeled(string label) {
            return false;
        }

        public override bool hasTooltip(string tooltip) {
            if (tooltip.EndsWith("~")) {
                return this.tooltip.StartsWith(tooltip.TrimEnd('~'));
            }

            return this.tooltip == tooltip;
        }
    
        public override void push(GuiSession session) 
        {
            var gridView = (GuiGridView)session.FindById(gridViewId);
            gridView.PressToolbarContextButton(id);
        }

        public override void toggleHighlight(GuiSession session) {}
    }

    public sealed class SAPToolbarButton: Button {
        string toolbarId;
        string id;
        int buttonPos;
        string text;
        string tooltip;

        public SAPToolbarButton(GuiToolbarControl toolbar, int buttonPos) {
            this.buttonPos = buttonPos;
            this.focused = false;
            this.id = toolbar.GetButtonId(buttonPos);
            this.position = new Position(
                height: toolbar.Height, 
                left: toolbar.ScreenLeft,
                top: toolbar.ScreenTop, 
                width: toolbar.Width
            );
            this.text = toolbar.GetButtonText(buttonPos);
            this.toolbarId = toolbar.Id;
            this.tooltip = toolbar.GetButtonTooltip(buttonPos).Trim();
        }

        public override bool isHLabeled(string label) {
            return text == label;
        }

        public override bool isVLabeled(string label) {
            return false;
        }

        public override bool hasTooltip(string tooltip) {
            if (tooltip.EndsWith("~")) {
                return this.tooltip.StartsWith(tooltip.TrimEnd('~'));
            }

            return this.tooltip == tooltip;
        }

        public override bool isEnabled(GuiSession session) 
        {
            var toolbar = (GuiToolbarControl)session.FindById(toolbarId);
            return toolbar.GetButtonEnabled(buttonPos);
        }

        public override void push(GuiSession session) {
            var toolbar = (GuiToolbarControl)session.FindById(toolbarId);
            toolbar.PressButton(id);
        }

        public override void toggleHighlight(GuiSession session)
        {
            focused = !focused;
            var toolbar = (GuiToolbarControl)session.FindById(toolbarId);
            toolbar.Visualize(focused);
        }
    }
}
