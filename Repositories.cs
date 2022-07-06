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
            return labels.get(label)?.findClosestHorizontalComponent(filterBy<ILocatable>()) as T ??
                   textFieldLabels.getByContent(label)?.findClosestHorizontalComponent(filterBy<ILocatable>()) as T;
        }

        public T? getVerticalClosestToLabel(string label, LabelStore labels, ReadOnlyTextFieldStore textFieldLabels) {
            return labels.get(label)?.findClosestVerticalComponent(filterBy<ILocatable>()) as T ??
                   textFieldLabels.getByContent(label)?.findClosestVerticalComponent(filterBy<ILocatable>()) as T;
        }

        public T? getNearLabel(string label, LabelStore labels, ReadOnlyTextFieldStore textFieldLabels) {
            return labels.get(label)?.findNearbyComponent(filterBy<ILocatable>()) as T ??
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
        SAPTextField? findInBox(ILocator locator, SAPBoxStore boxes) {
            return locator switch {
                HLabelVLabel(var label, var boxTitle) => 
                    items.Find(textField => textField.isContainedInBox(boxes.get(boxTitle)) &&
                                            textField.isLabeled(label)),
                _ => null
            };
        }

        SAPTextField? getByIndex(IIndexLocator locator) {
            return locator switch {
                HIndexVLabel(int rowIndex, string label) => 
                    items.Find(field => field.isLabeled(label))
                        ?.getVerticalGrid(items)
                         .ElementAt(rowIndex - 1),
                HLabelVIndex(string label, int columnIndex) =>
                    items.Find(field => field.isLabeled(label))
                        ?.getHorizontalGrid(items)
                         .ElementAt(columnIndex - 1),
                _ => null
            };
        }
        
        public SAPTextField? getByContent(string text) {
            return items.Find(textElement => textElement.contains(text));
        }

        public SAPTextField? getTextField(ILocator locator, LabelStore labels, ReadOnlyTextFieldStore textFieldLabels, SAPBoxStore boxes) {
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
