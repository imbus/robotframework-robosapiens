using System.Collections.Generic;
using System.Text.RegularExpressions;
using sapfewse;

namespace RoboSAPiens {
    public abstract class Button: IHighlightable {
        protected bool focused;
        public abstract bool isEnabled(GuiSession session);
        public abstract void push(GuiSession session);
        public abstract void toggleHighlight(GuiSession session);
    }

    public class SAPButton: Button, ILabeled, ILocatable {
        protected string defaultTooltip;
        public string id;
        public Position position {get;}
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

        public bool isHLabeled(string label) {
            return this.text == label;
        }

        public bool isVLabeled(string label) {
            return false;
        }

        public bool hasTooltip(string tooltip) {
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

    public sealed class SAPGridViewToolbarButton: Button, ILabeled {
        string gridViewId;
        string id;
        int position;
        string label;
        string tooltip;

        public SAPGridViewToolbarButton(GuiGridView gridView, int position) {
            this.focused = false;
            this.gridViewId = gridView.Id;
            this.id = gridView.GetToolbarButtonId(position);
            this.position = position;
            this.tooltip = gridView.GetToolbarButtonTooltip(position);
            this.label = gridView.GetToolbarButtonText(position);
        }

        public override bool isEnabled(GuiSession session)
        {
            var gridView = (GuiGridView)session.FindById(gridViewId);
            return gridView.GetToolbarButtonEnabled(position);
        }

        public bool isHLabeled(string label) {
            return this.label.Equals(label);
        }

        public bool isVLabeled(string label) {
            return false;
        }

        public bool hasTooltip(string tooltip) {
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

    public sealed class SAPGridViewToolbarButtonMenu: Button, ILabeled 
    {
        string gridViewId;
        string id;
        int position;
        string tooltip;

        public SAPGridViewToolbarButtonMenu(GuiGridView gridView, int position) 
        {
            this.position = position;
            this.gridViewId = gridView.Id;
            this.id = gridView.GetToolbarButtonId(position);
            this.tooltip = gridView.GetToolbarButtonTooltip(position);
        }

        public override bool isEnabled(GuiSession session)
        {
            var gridView = (GuiGridView)session.FindById(gridViewId);
            return gridView.GetToolbarButtonEnabled(position);
        }

        public bool isHLabeled(string label) {
            return false;
        }

        public bool isVLabeled(string label) {
            return false;
        }

        public bool hasTooltip(string tooltip) {
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

    public sealed class SAPToolbarButton: Button, ILabeled {
        string toolbarId;
        string id;
        int position;
        string text;
        string tooltip;

        public SAPToolbarButton(GuiToolbarControl toolbar, int position) {
            this.position = position;
            this.toolbarId = toolbar.Id;
            this.id = toolbar.GetButtonId(position);
            this.text = toolbar.GetButtonText(position);
            this.tooltip = toolbar.GetButtonTooltip(position).Trim();
        }

        public bool isHLabeled(string label) {
            return text == label;
        }

        public bool isVLabeled(string label) {
            return false;
        }

        public bool hasTooltip(string tooltip) {
            if (tooltip.EndsWith("~")) {
                return this.tooltip.StartsWith(tooltip.TrimEnd('~'));
            }

            return this.tooltip == tooltip;
        }

        public override bool isEnabled(GuiSession session) 
        {
            var toolbar = (GuiToolbarControl)session.FindById(toolbarId);
            return toolbar.GetButtonEnabled(position);
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
