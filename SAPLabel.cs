using sapfewse;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoboSAPiens {
    public sealed class SAPLabel: ITextElement, ILocatable {
        const int maxHorizontalDistance = 20;
        const int maxVerticalDistance = 20;

        public string id;
        public Position position {get;}
        string text;

        public SAPLabel(GuiLabel label) {
            this.id = label.Id;
            this.position = new Position(height: label.Height, 
                                         left: label.ScreenLeft,
                                         top: label.ScreenTop, 
                                         width: label.Width
                                        );
            this.text = label.Text;
        }

        public bool contains(string content) {
            return text.Equals(content) || text.StartsWith(content);
        }

        public bool isLocated(ILocator locator, LabelStore labels, ReadOnlyTextFieldStore textFieldLabels) {
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
            var componentsAfterLabel = components.Where(component => component.getPosition().horizontalAlignedWith(position))
                                                     .Where(component => component.getPosition().left > position.right);

            var componentsBeforeLabel = components.Where(component => component.getPosition().horizontalAlignedWith(position))
                                                      .Where(component => component.getPosition().right < position.left);

            if (componentsAfterLabel.Count() > 0){                          
                var (distance, closestComponent) = 
                    componentsAfterLabel.Select(component => (component.getPosition().left - position.right, component))
                                        .Min();
                return closestComponent;
            }

            if (componentsBeforeLabel.Count() > 0){                          
                var (distance, closestComponent) = 
                    componentsBeforeLabel.Select(component => (component.getPosition().right - position.left, component))
                                         .Min();
                if (Math.Abs(distance) < maxHorizontalDistance) return closestComponent;
            }

            return null;
        }

        public ILocatable? findClosestVerticalComponent(List<ILocatable> components) {
            var componentsBelowLabel = components.Where(component => component.getPosition().verticalAlignedWith(position))
                                                 .Where(component => component.getPosition().top > position.bottom);

            if (componentsBelowLabel.Count() > 0) {
                var (distance, closestComponent) = 
                    componentsBelowLabel.Select(component => (component.getPosition().top - position.bottom, component))
                                        .Min();

                if (Math.Abs(distance) < maxVerticalDistance) return closestComponent;
            }

            return null;
        }

        public ILocatable? findNearbyComponent(List<ILocatable> components) {
            return components.Find(component => component.getPosition().verticalAndHorizontalNeighborOf(position));
        }

        public string getText() {
            return text;
        }

        public void select(GuiSession session) {
            var guiLabel = (GuiLabel)session.FindById(id);
            guiLabel.SetFocus();
            guiLabel.Visualize(true);
        }
    }
}
