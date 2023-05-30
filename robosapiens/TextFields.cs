using sapfewse;
using System.Linq;
using System.Collections.Generic;
using System;

namespace RoboSAPiens {
    public class SAPTextField: ILabeled, ILocatable, ITextElement, IHighlightable {
        const int maxHorizontalDistance = 20;
        const int maxVerticalDistance = 20;
        const int overlapTolerance = 3;

        string accTooltip;
        string defaultTooltip;
        int height;
        public string id;
        public string label;
        protected bool focused;
        List<SAPTextField> grid;
        public Position position {get;}
        public string text;
        string tooltip;
        int width;

        public SAPTextField(GuiTextField textField) {
            this.accTooltip = textField.AccTooltip.Trim();
            this.defaultTooltip = textField.DefaultTooltip;
            this.grid = new List<SAPTextField>();
            this.height = textField.Height;
            this.id = textField.Id;
            this.label = getLeftLabel(textField);
            this.position = new Position(height: textField.Height, 
                                         left: textField.ScreenLeft,
                                         top: textField.ScreenTop, 
                                         width: textField.Width);
            this.text = textField.Text.Trim();
            this.tooltip = textField.Tooltip;
            this.width = textField.Width;
        }

        // To Do: Factor out this common function
        string getLeftLabel(GuiTextField textField) {
            var leftLabel = (GuiComponent)textField.LeftLabel;

            if (leftLabel != null) {
                if (leftLabel.Type == "GuiLabel") {
                    var label = (GuiLabel)leftLabel;
                    return label.Text;
                }

                if (leftLabel.Type == "GuiTextField") {
                    var label = (GuiTextField)leftLabel;
                    return label.Text;
                }
            }
            return "";
        }

    	public bool contains(string content) {
            return text.Equals(content) || text.StartsWith(content);
        }

        public ILocatable? findClosestHorizontalComponent(List<ILocatable> components) {
            var afterTextField = (ILocatable component) => component.getPosition().left - position.right + overlapTolerance;
            var beforeTextField = (ILocatable component) => component.getPosition().right - position.left - overlapTolerance;
            var horizontalAligned = components.Where(_ => _.getPosition().horizontalAlignedWith(position));

            return horizontalAligned
                    .Where(_ => afterTextField(_) > 0 && afterTextField(_) < maxHorizontalDistance)
                    .MinBy(afterTextField) ??
                   horizontalAligned
                    .Where(_ => beforeTextField(_) < 0 && Math.Abs(beforeTextField(_)) < maxHorizontalDistance)
                    .MinBy(beforeTextField);
        }

        public ILocatable? findClosestVerticalComponent(List<ILocatable> components) {
            var verticalDistance = (ILocatable component) => component.getPosition().top - position.bottom;

            return components
                    .Where(_ => _.getPosition().verticalAlignedWith(position))
                    .Where(_ => verticalDistance(_) > 0 && verticalDistance(_) < maxVerticalDistance)
                    .MinBy(verticalDistance);
        }

        public ILocatable? findNearbyComponent(List<ILocatable> components) {
            return components.Find(component => component.getPosition().verticalAndHorizontalNeighborOf(position));
        }

        // 
        //         _______      ________      _______
        // Label  |       |    |        |    |       |
        //         -------      --------      -------
        //
        //         _______      ________      _______
        // Label  |       |    |        |    |       |
        //         -------      --------      -------
        //
        //  1. Find all textfields in the leftmost column
        //  2. For each one of these fields find all fields to its right that are: horizontally aligned with it, of the same width
        //  All fields are stored in a 1D List. Therefore, the grid is flattened and thus accessible with a single index.

        public List<SAPTextField> getHorizontalGrid(List<SAPTextField> textFields) {
            if (grid.Count == 0) {
                textFields
                    .Where(textField => textField.position.left == position.left &&
                                        textField.width == width)
                    .ToList().ForEach(columnTextField => 
                        textFields
                            .Where(textField => textField.position.top == columnTextField.position.top &&
                                                textField.width == columnTextField.width)
                            .ToList().ForEach(textField => grid.Add(textField)));
            }
            return grid;
        }

        public Position getPosition() {
            return position;
        }

        public string getText() {
            return text;
        }

        public List<SAPTextField> getVerticalGrid(List<SAPTextField> textFields) {
            if (grid.Count == 0) {
                textFields
                    .Where(textField => textField.position.left == position.left &&
                                        textField.width == width)
                    .ToList().ForEach(textField => grid.Add(textField));
            }
            return grid;
        }

        public bool isContainedInBox(SAPBox? box) {
            return box switch {
                SAPBox => box.contains(position),
                _ => false
            };
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

        public bool isLabeled(string label) {
            return this.label == label || 
                   accTooltip == label ||
                   defaultTooltip == label ||
                   tooltip == label ||
                   defaultTooltip.StartsWith(label);
        }

        public bool isLocated(ILocator locator, LabelStore labels, ReadOnlyTextFieldStore textFieldLabels) {
            return locator switch {
                HLabelVLabel(var hLabel, var vLabel) =>
                    (isHorizontalAlignedWithLabel(labels.getByName(hLabel)) || 
                    isHorizontalAlignedWithTextField(textFieldLabels.getByContent(hLabel))) &&
                    isVerticalAlignedWithLabel(labels.getByName(vLabel)),
                HLabelHLabel(var leftLabel, var rightLabel) =>
                    (isHorizontalAlignedWithLabel(labels.getByName(leftLabel)) || 
                    isHorizontalAlignedWithTextField(textFieldLabels.getByContent(leftLabel))) &&
                    isLabeled(rightLabel),
                _ => false
            };
        }

        // To Do: Factor out this common function
        public bool isVerticalAlignedWithLabel(SAPLabel? label) {
            return label switch {
                SAPLabel => label.position.verticalAlignedWith(position),
                _ => false
            };
        }

        public void select(GuiSession session) {
            var guiTextField = (GuiTextField)session.FindById(id);
            guiTextField.SetFocus();
        }

        public void toggleHighlight(GuiSession session){
            focused = !focused;
            var guiTextField = (GuiTextField)session.FindById(id);
            guiTextField.Visualize(focused);
        }
    }

    public sealed class EditableTextField: SAPTextField {
        int maxLength;

        public EditableTextField(GuiTextField textField): base(textField) {
            this.maxLength = textField.MaxLength;
        }

        public int getMaxLength() {
            return maxLength;
        }

        public void insert(string content, GuiSession session) {
            var guiTextField = (GuiTextField)session.FindById(id);
            guiTextField.Text = content;
            text = content;
        }
    }
}
