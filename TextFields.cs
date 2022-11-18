using sapfewse;
using System.Linq;
using System.Collections.Generic;

namespace RoboSAPiens {
    public class SAPTextField: ILabeled, ILocatable, ITextElement {
        string defaultTooltip;
        int height;
        public string id;
        public string label;
        List<SAPTextField> grid;
        public Position position {get;}
        public string text;
        string tooltip;
        int width;

        public SAPTextField(GuiTextField textField) {
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
            var componentsAfterTextField = components.Where(component => component.getPosition().horizontalAlignedWith(position))
                                                     .Where(component => component.getPosition().left > position.right);

            var componentsBeforeTextField = components.Where(component => component.getPosition().horizontalAlignedWith(position))
                                                      .Where(component => component.getPosition().right < position.left);

            if (componentsAfterTextField.Count() > 0){                          
                var (distance, closestComponent) = 
                    componentsAfterTextField.Select(component => (component.getPosition().left - position.right, component))
                                            .Min();
                return closestComponent;
            }

            if (componentsBeforeTextField.Count() > 0){                          
                var (distance, closestComponent) = 
                    componentsBeforeTextField.Select(component => (component.getPosition().right - position.left, component))
                                            .Min();
                return closestComponent;
            }

            return null;
        }

        public ILocatable? findClosestVerticalComponent(List<ILocatable> components) {
            var componentsBelowTextField = components.Where(component => component.getPosition().verticalAlignedWith(position))
                                                     .Where(component => component.getPosition().top > position.bottom);

            if (componentsBelowTextField.Count() > 0){                          
                var (distance, closestComponent) = 
                    componentsBelowTextField.Select(component => (component.getPosition().top - position.bottom, component))
                                        .Min();
                return closestComponent;
            }

            return null;
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
                   defaultTooltip.StartsWith(label);
        }    

        public bool isLocated(ILabelsLocator locator, LabelStore labels, ReadOnlyTextFieldStore textFieldLabels) {
            return locator switch {
                HLabelVLabel(var hLabel, var vLabel) =>
                    (isHorizontalAlignedWithLabel(labels.get(hLabel)) || 
                    isHorizontalAlignedWithTextField(textFieldLabels.getByContent(hLabel))) &&
                    isVerticalAlignedWithLabel(labels.get(vLabel)),
                HLabelHLabel(var leftLabel, var rightLabel) =>
                    (isHorizontalAlignedWithLabel(labels.get(leftLabel)) || 
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
            guiTextField.Visualize(true);
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
