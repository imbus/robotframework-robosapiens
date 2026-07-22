using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

namespace RoboSAPiens {
    public abstract class Repository<T>: List<T> {
        public List<U> filterBy<U>() {
            return this.Where(item => item is U)
                       .Cast<U>()
                       .ToList();
        }
    }

    public class CellRepository: Repository<Cell>
    {
        public Cell? findCellByContent(string content)
        {
            return Find(cell => cell.isLabeled(content));
        }

        public Cell? findCellByLabelAndColumn(string label, string column, bool exact=false)
        {
            var columnCells = this.Where(cell => cell.inColumn(column)).ToList();
            var searchLabel = exact ? label : label + "~";
            
            return columnCells.FirstOrDefault(cell => cell.isLabeled(searchLabel)) ??
                   columnCells.FirstOrDefault(cell => rowContainsLabel(cell.rowIndex, searchLabel));
        }

        public Cell? findCellByLabelAndColumnIndex(string label, int colIndex0)
        {
            var columnCells = this.Where(cell => cell.colIndex == colIndex0).ToList();
            
            return columnCells.FirstOrDefault(cell => cell.isLabeled(label)) ?? 
                   columnCells.FirstOrDefault(cell => rowContainsLabel(cell.rowIndex, label));
        }

        public Cell? findCellByRowAndColumn(int rowIndex0, string column)
        {
            return Find(cell => cell.inColumn(column) && cell.inRow(rowIndex0));
        }

        bool rowContainsLabel(int rowIndex, string label)
        {
            return this.Where(cell => cell.inRow(rowIndex)).Any(cell => cell.isLabeled(label));
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

        public T? getHorizontalClosestToLabel(string label, LabelStore labels, TextFieldRepository textFieldLabels) 
        {
            return labels
                        .Where(l => l.contains(label))
                        .Select(l => l.findClosestHorizontalComponent(filterBy<ILocatable>()))
                        .Where(l => l != null)
                        .FirstOrDefault() as T ??
                   textFieldLabels
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
            return Find(item => item.isHLabeled(label) || item.hasTooltip(label));
        }
    }

    public class TextFieldRepository: ComponentRepository<SAPTextField> 
    {
        SAPTextField? findInBox(ILocator locator, BoxStore boxes) {
            return locator switch {
                HLabelVLabel(var label, var boxTitle) => 
                    Find(textField => textField.isContainedInBox(boxes.get(boxTitle)) &&
                                      textField.isHLabeled(label)),
                _ => null
            };
        }

        public SAPTextField? getByContent(string text) {
            return Find(textElement => textElement.contains(text));
        }

        public SAPTextField? getByName(string name) {
            return Find(textElement => textElement.isNamed(name));
        }

        SAPTextField? getFromVerticalGrid(int rowIndex, string label, int gridIndex, LabelStore labels) 
        {
            var textFields = this
                .Where(_ => Regex.IsMatch(_.id, @"\[\d+,\d+\]$", RegexOptions.Compiled))
                .ToList();
            var firstTextField = textFields.First();
            var grid = 
                textFields
                .GroupBy(_ => _.position.left)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToList()
                          .GroupBy(_ => _.position.right - _.position.left)
                          .Select(_ => _.OrderBy(_ => _.position.top).ToList())
                          .ToList()
                );
            var columns = grid.Keys.ToHashSet();
            var columnTitles = 
                labels
                .Where(_ => _.position.top > firstTextField.position.top - 70)
                .Where(_ => _.position.top < firstTextField.position.top)
                .Select(label => new {label, col=columns.MinBy(col => Math.Abs(col - label.position.left))})
                .GroupBy(_ => _.col)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(_ => _.label.text).ToList()
                );
            var adhocGrid = 
                grid
                .SelectMany(col => 
                    col.Value.SelectMany((g, gridIndex0) =>
                        g.Select((textField, rowIndex0) =>
                            {
                                var vLabel = columnTitles.GetValueOrDefault(textField.position.left)?[gridIndex0] ?? textField.id;
                                return ((rowIndex0, gridIndex0, vLabel), textField);
                            }
                        )
                    )
                )
                .ToDictionary();

            return adhocGrid.GetValueOrDefault((rowIndex-1, gridIndex, label));
        }

        SAPTextField? getFromHorizontalGrid(int index, string label) 
        {
            var textField = Find(field => field.isHLabeled(label));

            if (textField == null) return null;

            var horizontalGrid = textField.getHorizontalGrid(this);

            if (index <= horizontalGrid.Count) {
                return horizontalGrid.ElementAt(index - 1);
            }

            return null;
        }

        public SAPTextField? getTextField(ILocator locator, LabelStore labels, TextFieldRepository nonChangeableTextFields, BoxStore boxes, bool exact) 
        {
            return locator switch 
            {
                HLabel (var label) => 
                    getByHLabel(exact? label : label + "~") ??
                    getByTooltip(exact? label : label + "~") ??
                    getHorizontalClosestToLabel(label, labels, nonChangeableTextFields) ??
                    getByName(label),
                HLabelHLabel(var leftLabel, var rightLabel) => exact switch {
                    true => getAlignedWithLabels((HLabelHLabel)locator, labels, nonChangeableTextFields),
                    _ => getAlignedWithLabels(new HLabelHLabel(leftLabel, rightLabel + "~"), labels, nonChangeableTextFields)
                },
                VLabel (var label) => 
                    getByVLabel(label) ??
                    getVerticalClosestToLabel(label, labels, nonChangeableTextFields),
                HLabelVLabel => 
                    getAlignedWithLabels((HLabelVLabel)locator, labels, nonChangeableTextFields) ??
                    findInBox(locator, boxes),
                HIndexVLabel(int rowIndex, string label, int gridIndex) => 
                    getFromVerticalGrid(rowIndex, label, gridIndex, labels),
                HLabelVIndex(string label, int columnIndex) =>
                    getFromHorizontalGrid(columnIndex, label),
                Content (var content) => 
                    getByContent(exact? content : content + "~"),
                _ => null
            };
        }
    }
}
