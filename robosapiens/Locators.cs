using System;
using System.Linq;

namespace RoboSAPiens {
    public abstract class CellLocator: ILocator {
        public string cell;
        public string column {get;}
        public int rowIndex {get;}

        public CellLocator(int rowIndex, string column) {
            this.cell = $"{rowIndex}, {column}";
            this.column = column;
            this.rowIndex = rowIndex;
        }
    }

    public class EmptyCellLocator: CellLocator {
        public string? rowLabel {get;}

        EmptyCellLocator(int rowIndex, string column): base(rowIndex, column) {}

        EmptyCellLocator(string rowLabel, string column): base(rowIndex: 0, column: column) {
            this.cell = $"{rowLabel}, {column}";
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
            this.cell = $"{column} = {content}";
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

        public ComponentLocator(string locator) {
            this.atLocation = "";
            this.locator = parse(locator);
        }

        ILocator parse(string locator) {
            if (locator.StartsWith('=')) {
                var content = locator.Substring(1).Trim();

                atLocation += $"= {content}";

                return new Content(content);
            }
            
            if (locator.Contains(">>")) {
                var tokens = locator.Split(">>").Select(token => token.Trim()).ToArray();
                atLocation += $"{tokens[0]} >> {tokens[1]}";
                
                return new HLabelHLabel(tokens[0], tokens[1]);
            }

            if (locator.Contains('@')) {
                var tokens = locator.Split("@").Select(token => token.Trim()).ToArray();

                if (tokens[0] == "" && tokens[1] != "") {
                    var vLabel = tokens[1];
                    
                    atLocation += $"@ {vLabel}";

                    return new VLabel(name: vLabel);
                }

                if (tokens[0] != "" && tokens[1] != "") {
                    int index;

                    if (Int32.TryParse(tokens[0], out index)) {
                        var verticalLabel = tokens[1];

                        atLocation += $"{index} @ {verticalLabel}";

                        return new HIndexVLabel(hIndex: index, verticalLabel: verticalLabel);
                    }

                    if (Int32.TryParse(tokens[1], out index)) {
                        var horizontalLabel = tokens[0];

                        atLocation += $"{horizontalLabel} @ {index}";

                        return new HLabelVIndex(label: horizontalLabel, vIndex: index);
                    }

                    var hLabel = tokens[0];
                    var vLabel = tokens[1];

                    atLocation += $"{hLabel} @ {vLabel}";

                    return new HLabelVLabel(label: hLabel, verticalLabel: vLabel);
                }
            }

            var label = locator;

            atLocation += label;

            return new HLabel(name: label);
        }
    }

    public class ButtonLocator: ComponentLocator {
        public ButtonLocator(string locator): base(locator) {}
    }

    public class CheckBoxLocator: ComponentLocator {
        public CheckBoxLocator(string locator): base(locator) {}
    }

    public class ComboBoxLocator: ComponentLocator {
        public ComboBoxLocator(string locator): base(locator) {}
    }

    public class LabelLocator: ComponentLocator {
        public LabelLocator(string locator): base(locator) {}
    }

    public class RadioButtonLocator: ComponentLocator {
        public RadioButtonLocator(string locator): base(locator) {}
    }

    public class TextFieldLocator: ComponentLocator {
        public TextFieldLocator(string locator): base(locator) {}
    }

    public record Content(string text): ILocator;
    public record HLabel(string name): ILocator;
    public record HIndexVLabel (int hIndex, string verticalLabel): IIndexLocator;
    public record HLabelVLabel(string label, string verticalLabel): ILabelsLocator;
    public record HLabelHLabel(string leftLabel, string rightLabel): ILabelsLocator;
    public record VLabel(string name): ILocator;
    public record HLabelVIndex(string label, int vIndex): IIndexLocator;

    public static class Loc {
        public const string HLabel = "HLabel";
        public const string VLabel = "VLabel";
        public const string HLabelVLabel = "HLabelVLabel";
        public const string ColumnContent = "ColumnContent";
        public const string Content = "Content";
        public const string HLabelHLabel = "HLabelHLabel";
        public const string HLabelVIndex = "HLabelVIndex";
        public const string HIndexVLabel = "HIndexVLabel";
    }
}
