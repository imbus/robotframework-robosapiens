using sapfewse;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoboSAPiens {
    public sealed class SAPLabel: ITextElement, ILocatable, IHighlightable {
        const int maxHorizontalDistance = 20;
        const int maxVerticalDistance = 25;

        public string id;
        public Position position {get;}
        public string text;
        public List<string> tooltips;
        private bool focused;

        public SAPLabel(GuiLabel label) {
            this.id = label.Id;
            this.position = new Position(height: label.Height, 
                                         left: label.ScreenLeft,
                                         top: label.ScreenTop, 
                                         width: label.Width
                                        );
            this.text = label.Text.Trim();
            this.tooltips = new List<string>
            {
                label.AccTooltip.Trim(),
                label.DefaultTooltip.Trim(),
                label.Tooltip.Trim()
            };
        }

        public bool contains(string content) {
            return text.Equals(content);
        }

        public List<string> getLabels()
        {
            return new List<string>(tooltips)
            {
                text,
            };
        }

        public bool isLocated(ILocator locator, LabelStore labels, TextFieldRepository textFieldLabels) {
            // TODO: Implement getHorizontalGrid in order to support the HLabelVIndex locator

            return locator switch {
                Content(string content) => content.Equals(text),
                _ => false
            };
        }

        public Position getPosition() {
            return position;
        }

        public ILocatable? findClosestHorizontalComponent(List<ILocatable> components) {
            var afterLabel = (ILocatable component) => component.getPosition().left - position.right;
            var beforeLabel = (ILocatable component) => component.getPosition().right - position.left;
            var horizontalAligned = components.Where(_ => _.getPosition().horizontalAlignedWith(position));

            return horizontalAligned.Where(_ => afterLabel(_) > 0).MinBy(afterLabel) ??
                   horizontalAligned.Where(_ => beforeLabel(_) < 0 && Math.Abs(beforeLabel(_)) < maxHorizontalDistance)
                                    .MinBy(beforeLabel);
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

        public string getText(GuiSession session) {
            return text;
        }

        public void select(GuiSession session) {
            var guiLabel = (GuiLabel)session.FindById(id);
            guiLabel.SetFocus();
        }

        public void toggleHighlight(GuiSession session) {
            focused = !focused;
            var guiLabel = (GuiLabel)session.FindById(id);
            guiLabel.Visualize(focused);
        }
    }
}
