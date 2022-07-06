using System;
using System.Linq;

namespace RoboSAPiens {
    public abstract class CellLocator: ILocator {
        public string cell;
        public string column {get;}
        public int rowIndex {get;}

        public CellLocator(int rowIndex, string column) {
            this.cell = $"Die Zelle am Schnittpunkt zwischen der Zeile '{rowIndex}' und der Spalte '{column}'";
            this.column = column;
            this.rowIndex = rowIndex;
        }
    }

    public class EmptyCellLocator: CellLocator {
        public string? rowLabel {get;}

        EmptyCellLocator(int rowIndex, string column): base(rowIndex, column) {}

        EmptyCellLocator(string rowLabel, string column): base(rowIndex: 0, column: column) {
            this.cell = $"Die Zelle am Schnittpunkt zwischen der Zeile '{rowLabel}' und der Spalte '{column}'";
            this.rowLabel = rowLabel;
        }

        public static EmptyCellLocator of(string rowIndexOrRowLabel, string column) {            
            int index;

            if (Int32.TryParse(rowIndexOrRowLabel, out index)) {
                return new EmptyCellLocator(rowIndex: index, column: column);
            } 
            else {
                string rowLabel = rowIndexOrRowLabel;
                return new EmptyCellLocator(rowLabel: rowLabel, column: column);
            }
        }

        public record ColumnContent(string column, string content);

        public static ColumnContent parseColumnContent(string columnEqualsContent) {
            if (!columnEqualsContent.Contains('=')) {
                return new ColumnContent(column: columnEqualsContent, 
                                         content: string.Empty);
            }

            var tokens = columnEqualsContent.Split("=")
                                            .Select(token => token.Trim())
                                            .ToList();

            return (tokens[0], tokens[1]) switch {
                ("", string content) => 
                    new ColumnContent(column: string.Empty, content: content),

                (string column, string content) => 
                    new ColumnContent(column, content),
            };
        }
    }

    public class FilledCellLocator: CellLocator {
        public string? content {get;}

        FilledCellLocator(int rowIndex, string column): base(rowIndex, column) {}

        FilledCellLocator(string content, string column): base(rowIndex: 0, column: column) {
            this.cell = $"Die Zelle in der Spalte '{column}' mit dem Inhalt '{content}'";
            this.content = content;
        }

        public static FilledCellLocator of(string rowIndexOrContent, string column) {
            int index;

            if (Int32.TryParse(rowIndexOrContent, out index)) {
                return new FilledCellLocator(rowIndex: index, column: column);
            } else {
                string content = rowIndexOrContent;
                return new FilledCellLocator(content: content, column: column);
            }
        }
    }

    public abstract class ComponentLocator {
        public string atLocation;
        public ILocator locator {get;}

        public ComponentLocator(string theComponent, string locator) {
            this.atLocation = $"{theComponent} ";
            this.locator = parse(locator);
        }

        ILocator parse(string locator) {
            if (locator.StartsWith('=')) {
                var content = locator.Substring(1).Trim();

                atLocation += $"mit dem Inhalt '{content}'";

                return new Content(content);
            }
            
            if (locator.Contains(">>")) {
                var tokens = locator.Split(">>").Select(token => token.Trim()).ToArray();
                return new HLabelHLabel(tokens[0], tokens[1]);
            }

            if (locator.Contains('@')) {
                var tokens = locator.Split("@").Select(token => token.Trim()).ToArray();

                if (tokens[0] == "" && tokens[1] != "") {
                    var vLabel = tokens[1];
                    
                    atLocation += $"mit der Beschriftung '{vLabel}' oben";

                    return new VLabel(name: vLabel);
                }

                if (tokens[0] != "" && tokens[1] != "") {
                    int index;

                    if (Int32.TryParse(tokens[0], out index)) {
                        var verticalLabel = tokens[1];

                        atLocation += $"in der Position '{index}' unter der Beschriftung '{verticalLabel}'";

                        return new HIndexVLabel(hIndex: index, verticalLabel: verticalLabel);
                    }

                    if (Int32.TryParse(tokens[1], out index)) {
                        var horizontalLabel = tokens[0];

                        atLocation += $"in der Position '{index}' rechts von der Beschriftung '{horizontalLabel}'";

                        return new HLabelVIndex(label: horizontalLabel, vIndex: index);
                    }

                    var hLabel = tokens[0];
                    var vLabel = tokens[1];

                    atLocation += $"neben der Beschriftung '{hLabel}' und unter der Beschriftung '{vLabel}'";

                    return new HLabelVLabel(label: hLabel, verticalLabel: vLabel);
                }
            }

            var label = locator;

            atLocation += $"mit der Beschriftung '{label}'";

            return new HLabel(name: label);
        }
    }

    public class ButtonLocator: ComponentLocator {
        public ButtonLocator(string locator): base("Der Knopf", locator) {}
    }

    public class CheckBoxLocator: ComponentLocator {
        public CheckBoxLocator(string locator): base("Das Formularfeld", locator) {}
    }

    public class ComboBoxLocator: ComponentLocator {
        public ComboBoxLocator(string locator): base("Auswahlsmenü", locator) {}
    }

    public class RadioButtonLocator: ComponentLocator {
        public RadioButtonLocator(string locator): base("Das Optionsfeld", locator) {}
    }

    public class TextFieldLocator: ComponentLocator {
        public TextFieldLocator(string locator): base("Das Textfeld", locator) {}
    }

    public record Content(string text): ILocator;
    public record HLabel(string name): ILocator;
    public record HIndexVLabel (int hIndex, string verticalLabel): IIndexLocator;
    public record HLabelVLabel(string label, string verticalLabel): ILabelsLocator;
    public record HLabelHLabel(string leftLabel, string rightLabel): ILabelsLocator;
    public record VLabel(string name): ILocator;
    public record HLabelVIndex(string label, int vIndex): IIndexLocator;
}
