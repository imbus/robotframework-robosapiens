using sapfewse;
using System.Linq;
using System.Collections.Generic;
using System;

namespace RoboSAPiens {
    public class SAPTextField: ILabeled, ILocatable, ITextElement, IHighlightable {
        const int maxHorizontalDistance = 22;
        const int maxVerticalDistance = 20;
        const int overlapTolerance = 3;

        string accTooltip;
        string defaultTooltip;
        int height;
        public string id;
        public string hLabel;
        public string vLabel;
        public bool changeable;
        protected bool focused;
        List<SAPTextField> grid;
        public string name;
        public Position position {get;}
        public string text;
        string tooltip;
        int width;

        public SAPTextField(GuiTextField textField) {
            this.accTooltip = textField.AccTooltip.Trim();
            this.changeable = textField.Changeable;
            this.defaultTooltip = textField.DefaultTooltip;
            this.grid = new List<SAPTextField>();
            this.height = textField.Height;
            this.id = textField.Id;
            (this.hLabel, this.vLabel) = getLabel(textField);
            this.name = textField.Name;
            this.position = new Position(height: textField.Height, 
                                         left: textField.ScreenLeft,
                                         top: textField.ScreenTop, 
                                         width: textField.Width);
            this.text = textField.Text.Trim();
            this.tooltip = textField.Tooltip;
            this.width = textField.Width;
        }

        // To Do: Factor out this common function
        Tuple<string, string> getLabel(GuiTextField textField) {
            var leftLabel = textField.LeftLabel;

            if (leftLabel != null) 
            {
                if (leftLabel.ScreenLeft < textField.ScreenLeft) {
                    return new Tuple<string, string>(leftLabel.Text, "");
                }
            
                if (leftLabel.ScreenTop < textField.ScreenTop) {
                    return new Tuple<string, string>("", leftLabel.Text);
                }    
            }
            
            return new Tuple<string, string>("", "");
        }

    	public bool contains(string content) {
            return text.Equals(content);
        }


        int distanceBeforeTextField(ILocatable component) {
            var distance = component.getPosition().right - position.left;

            return distance switch {
                <= 0 => distance,
                > 0 => distance - overlapTolerance
            };
        }

        int distanceAfterTextField(ILocatable component) {
            var distance = component.getPosition().left - position.right;

            return distance switch {
                >= 0 => distance,
                < 0 => distance + overlapTolerance
            };
        }

        public ILocatable? findClosestHorizontalComponent(List<ILocatable> components) {
            var horizontalAligned = components.Where(_ => _.getPosition().horizontalAlignedWith(position));

            return horizontalAligned
                    .Where(_ => distanceBeforeTextField(_) <= 0 && Math.Abs(distanceBeforeTextField(_)) <= maxHorizontalDistance)
                    .MaxBy(distanceBeforeTextField) ??
                   horizontalAligned
                    .Where(_ => distanceAfterTextField(_) >= 0 && distanceAfterTextField(_) <= maxHorizontalDistance)
                    .MinBy(distanceAfterTextField);
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

        public string getText(GuiSession session) {
            var guiTextField = (GuiTextField)session.FindById(id);
            return guiTextField.Text;
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

        public bool hasTooltip(string tooltip) {
            return accTooltip == tooltip ||
                   defaultTooltip == tooltip ||
                   this.tooltip == tooltip;
        }

        public void insert(string content, GuiSession session) {
            var guiTextField = (GuiTextField)session.FindById(id);
            guiTextField.Text = content;
            text = content;
        }

        public bool isChangeable(GuiSession session) {
            var guiTextField = (GuiTextField)session.FindById(id);
            return guiTextField.Changeable;
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

        public bool isHLabeled(string label) {
            return this.hLabel == label;
        }

        public bool isVLabeled(string label) {
            return this.vLabel == label;
        }

        public bool isLocated(ILocator locator, LabelStore labels, TextFieldRepository textFieldLabels) {
            return locator switch {
                HLabelVLabel(var hLabel, var vLabel) =>
                    (isHorizontalAlignedWithLabel(labels.getByName(hLabel)) || 
                    isHorizontalAlignedWithTextField(textFieldLabels.getByContent(hLabel))) &&
                    isVerticalAlignedWithLabel(labels.getByName(vLabel)),
                HLabelHLabel(var leftLabel, var rightLabel) =>
                    (isHorizontalAlignedWithLabel(labels.getByName(leftLabel)) || 
                    isHorizontalAlignedWithTextField(textFieldLabels.getByContent(leftLabel))) &&
                    (isHLabeled(rightLabel) || hasTooltip(rightLabel)),
                _ => false
            };
        }

        public bool isNamed(string name) {
            return this.name.Equals(name);
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
}
