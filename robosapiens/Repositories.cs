using System.Linq;
using System.Collections.Generic;

namespace RoboSAPiens {
    public abstract class Repository<T> {
        protected List<T> items;

        public Repository() {
            items = new List<T>();
        }

        public void add(T item) {
            items.Add(item);
        }

        public List<U> filterBy<U>() {
            return items.Where(item => item is U)
                        .Cast<U>()
                        .ToList();
        }

        public List<T> getAll() {
            return items;
        }
    }

    public abstract class ComponentRepository<T>: Repository<T> where T: class {
        public T? getAlignedWithLabels(ILabelsLocator locator, LabelStore labels, TextFieldRepository textFieldLabels) {
            return filterBy<ILocatable>().Find(item => item.isLocated(locator, labels, textFieldLabels)) as T;
        }

        public T? getByTooltip(string label) {
            return filterBy<ILabeled>().Find(item => item.hasTooltip(label)) as T;
        }

        public T? getByHLabel(string label) {
            return filterBy<ILabeled>().Find(item => item.isHLabeled(label)) as T;
        }

        public T? getByVLabel(string label) {
            return filterBy<ILabeled>().Find(item => item.isVLabeled(label)) as T;
        }

        public T? getCell(CellLocator locator, TextCellStore rowLabels) {
            return filterBy<ILocatableCell>().Find(cell => cell.isLocated(locator, rowLabels)) as T;
        }

        public T? getHorizontalClosestToLabel(string label, LabelStore labels, TextFieldRepository textFieldLabels) {
            return labels.getAll()
                         .Where(l => l.contains(label))
                         .Select(l => l.findClosestHorizontalComponent(filterBy<ILocatable>()))
                         .Where(l => l != null)
                         .FirstOrDefault() as T ??
                   textFieldLabels.getAll()
                         .Where(l => l.contains(label))
                         .Select(l => l.findClosestHorizontalComponent(filterBy<ILocatable>()))
                         .Where(l => l != null)
                         .FirstOrDefault() as T;
        }

        public T? getVerticalClosestToLabel(string label, LabelStore labels, TextFieldRepository textFieldLabels) {
            return labels.getByName(label)?.findClosestVerticalComponent(filterBy<ILocatable>()) as T ??
                   textFieldLabels.getByContent(label)?.findClosestVerticalComponent(filterBy<ILocatable>()) as T;
        }

        public T? getNearLabel(string label, LabelStore labels, TextFieldRepository textFieldLabels) {
            return labels.getByName(label)?.findNearbyComponent(filterBy<ILocatable>()) as T ??
                   textFieldLabels.getByContent(label)?.findNearbyComponent(filterBy<ILocatable>()) as T;
        }
    }

    public abstract class ContainerRepository<T>: Repository<T> where T: ILabeled {
        public T? get(string label) {
            return items.Find(item => item.isHLabeled(label));
        }
    }

    public abstract class TextFieldRepository: ComponentRepository<SAPTextField> {
        SAPTextField? findInBox(ILocator locator, BoxStore boxes) {
            return locator switch {
                HLabelVLabel(var label, var boxTitle) => 
                    items.Find(textField => textField.isContainedInBox(boxes.get(boxTitle)) &&
                                            textField.isHLabeled(label)),
                _ => null
            };
        }

        public SAPTextField? getByContent(string text) {
            return items.Find(textElement => textElement.contains(text));
        }

        SAPTextField? getByIndex(IIndexLocator locator, LabelStore labels) {
            return locator switch {
                HIndexVLabel(int rowIndex, string label) => 
                    getFromVerticalGrid(rowIndex, label, labels),
                HLabelVIndex(string label, int columnIndex) =>
                    getFromHorizontalGrid(columnIndex, label),
                _ => null
            };
        }

        SAPTextField? getFromVerticalGrid(int index, string label, LabelStore labels) {
            var textField = items.Find(field => 
                field.isHLabeled(label) || 
                field.contains(label) ||
                field.isNamed(label)
            ) ?? getVerticalClosestToLabel(label, labels, this);

            if (textField == null) return null;

            var verticalGrid = textField.contains(label) switch {
                true => textField.getVerticalGrid(items.Where(item => !item.Equals(textField)).ToList()),
                false => textField.getVerticalGrid(items)
            };

            if (index <= verticalGrid.Count) {
                return verticalGrid.ElementAt(index - 1);
            }

            return null;
        }

        SAPTextField? getFromHorizontalGrid(int index, string label) {
            var textField = items.Find(field => field.isHLabeled(label));

            if (textField == null) return null;

            var horizontalGrid = textField.getHorizontalGrid(items);

            if (index <= horizontalGrid.Count) {
                return horizontalGrid.ElementAt(index - 1);
            }

            return null;
        }

        public SAPTextField? getTextField(ILocator locator, LabelStore labels, BoxStore boxes) {
            return locator switch {
                HLabel (var label) => 
                    getByHLabel(label) ??
                    getByTooltip(label) ??
                    getHorizontalClosestToLabel(label, labels, this),
                HLabelHLabel =>
                    getAlignedWithLabels((HLabelHLabel)locator, labels, this),
                VLabel (var label) => 
                    getByVLabel(label) ??
                    getVerticalClosestToLabel(label, labels, this),
                HLabelVLabel => 
                    getAlignedWithLabels((HLabelVLabel)locator, labels, this) ??
                    findInBox(locator, boxes),
                IIndexLocator indexLocator => 
                    getByIndex(indexLocator, labels),
                Content (var content) => 
                    getByContent(content),
                _ => null
            };
        }
    }
}
