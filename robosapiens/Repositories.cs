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
        public T? getAlignedWithLabels(ILabelsLocator locator, LabelStore labels, ReadOnlyTextFieldStore textFieldLabels) {
            return filterBy<ILocatable>().Find(item => item.isLocated(locator, labels, textFieldLabels)) as T;
        }

        public T? getByLabelOrTooltip(string label) {
            return filterBy<ILabeled>().Find(item => item.isLabeled(label)) as T;
        }

        public T? getCell(FilledCellLocator locator) {
            return filterBy<IFilledCell>().Find(cell => cell.isLocated(locator)) as T;
        }

        public T? getHorizontalClosestToLabel(string label, LabelStore labels, ReadOnlyTextFieldStore textFieldLabels) {
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

        public T? getVerticalClosestToLabel(string label, LabelStore labels, ReadOnlyTextFieldStore textFieldLabels) {
            return labels.getByName(label)?.findClosestVerticalComponent(filterBy<ILocatable>()) as T ??
                   textFieldLabels.getByContent(label)?.findClosestVerticalComponent(filterBy<ILocatable>()) as T;
        }

        public T? getNearLabel(string label, LabelStore labels, ReadOnlyTextFieldStore textFieldLabels) {
            return labels.getByName(label)?.findNearbyComponent(filterBy<ILocatable>()) as T ??
                   textFieldLabels.getByContent(label)?.findNearbyComponent(filterBy<ILocatable>()) as T;
        }
    }

    public abstract class ContainerRepository<T>: Repository<T> where T: ILabeled {
        public T? get(string label) {
            return items.Find(item => item.isLabeled(label));
        }
    }

    public abstract class EditableTextCellRepository: TextCellRepository {
        public Cell? getEmptyCell(EmptyCellLocator locator, LabelCellStore rowLabels) {
            return filterBy<IEditableCell>().Find(cell => cell.isLocated(locator, rowLabels)) as Cell;
        }
    }

    public abstract class TextCellRepository: Repository<Cell> {
        public Cell? getByContent(string text) {
            return items.Find(cell => cell.contains(text));
        }

        public Cell? getFilledCell(FilledCellLocator locator) {
            return filterBy<IFilledCell>().Find(cell => cell.isLocated(locator)) as Cell;
        }
    }

    public abstract class TextFieldRepository<T>: ComponentRepository<SAPTextField> {
        SAPTextField? findInBox(ILocator locator, BoxStore boxes) {
            return locator switch {
                HLabelVLabel(var label, var boxTitle) => 
                    items.Find(textField => textField.isContainedInBox(boxes.get(boxTitle)) &&
                                            textField.isLabeled(label)),
                _ => null
            };
        }

        SAPTextField? getFromVerticalGrid(int index, string label) {
            var textField = items.Find(
                field => field.isLabeled(label) || 
                field.contains(label)
            );

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
            var textField = items.Find(field => field.isLabeled(label));

            if (textField == null) return null;

            var horizontalGrid = textField.getHorizontalGrid(items);

            if (index <= horizontalGrid.Count) {
                return horizontalGrid.ElementAt(index - 1);
            }

            return null;
        }

        SAPTextField? getByIndex(IIndexLocator locator) {
            return locator switch {
                HIndexVLabel(int rowIndex, string label) => 
                    getFromVerticalGrid(rowIndex, label),
                HLabelVIndex(string label, int columnIndex) =>
                    getFromHorizontalGrid(columnIndex, label),
                _ => null
            };
        }
        
        public SAPTextField? getByContent(string text) {
            return items.Find(textElement => textElement.contains(text));
        }

        public SAPTextField? getTextField(ILocator locator, LabelStore labels, ReadOnlyTextFieldStore textFieldLabels, BoxStore boxes) {
            return locator switch {
                HLabel (var label) => 
                    getByLabelOrTooltip(label) ??
                    getHorizontalClosestToLabel(label, labels, textFieldLabels),
                HLabelHLabel =>
                    getAlignedWithLabels((HLabelHLabel)locator, labels, textFieldLabels),
                VLabel (var label) => 
                    getByLabelOrTooltip(label) ??
                    getVerticalClosestToLabel(label, labels, textFieldLabels),
                HLabelVLabel => 
                    getAlignedWithLabels((HLabelVLabel)locator, labels, textFieldLabels) ??
                    findInBox(locator, boxes),
                IIndexLocator indexLocator => 
                    getByIndex(indexLocator),
                Content (var content) => 
                    getByContent(content),
                _ => null
            };
        }
    }
}
