using System;
using System.Linq;

namespace RoboSAPiens {
    public abstract record CellLocator(string column, string location): ILocator
    {
        public static CellLocator of(string rowIndexOrLabel, string column) 
        {
            int index;
            if (Int32.TryParse(rowIndexOrLabel, out index)) {
                return new RowColumnLocator(rowIndex: index, column: column);
            } 
            else {
                // If the label is a number it must be quoted, so that it is not interpreted as a row number
                string label = rowIndexOrLabel.Trim('"');
                return new LabelColumnLocator(label: label, column: column);
            }
        }
    }

    public record RowColumnLocator(int rowIndex, string column): CellLocator(column, $"{rowIndex}, {column}");

    public record LabelColumnLocator(string label, string column): CellLocator(column, $"{label}, {column}");

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

    public class RowLocator: ComponentLocator {
        public RowLocator(string rowLabel): base(rowLabel) {}
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
        public const string Column = "Column";
        public const string Content = "Content";
        public const string HLabelHLabel = "HLabelHLabel";
        public const string HLabelVIndex = "HLabelVIndex";
        public const string HIndexVLabel = "HIndexVLabel";
    }
}
