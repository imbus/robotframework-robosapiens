using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace RoboSAPiens {
    public class Form {
        string csvFile;
        List<FormField> fields;
        string pngFile;

        public Form(string name, string testServer, string directory) {
            var csv = new CSVReader<FormField>(delimiter: ";");
            this.csvFile = Path.Combine(directory, $"{name}_{testServer}.csv".Replace("/", "_"));
            fields = csv.readRows(csvFile);
            this.pngFile = Path.Combine(directory, $"{name}_{testServer}.png".Replace("/", "_"));
        }

        void visualDiff(Pen pen, Position position, string directory, string fileName) {
            var diffFolder = Path.Combine(directory, "Unterschiede");
            var fullPath = Path.Combine(diffFolder, fileName);
            Directory.CreateDirectory(diffFolder);

            var image = new Bitmap(pngFile);
            var graphics = Graphics.FromImage(image);
            var left = position.left;
            var top = position.top;
            var width = position.right - position.left;
            var height = position.bottom - position.top;
            var rect = new Rectangle(left, top, width, height);

            graphics.DrawRectangle(pen, rect);
            image.Save(fullPath);
        }

        public (List<string>, List<string>) compareTo(Form other, string directory) {
            var differences = new List<string>();
            var matches = new List<string>();

            var thickness = 3;
            var redPen = new Pen(Color.Red, thickness);
            var bluePen = new Pen(Color.Blue, thickness);

            // A button, a text, a label may still be present, but with different position and id
            var inForm1NotForm2 = this.fields.Except(other.fields).ToList();
            var inForm2NotForm1 = other.fields.Except(this.fields).ToList();
            var inForm1AndForm2 = this.fields.Intersect(other.fields)
                                             .OrderByDescending(field => field.Id)
                                             .ToList();
            var inForm2AndForm1 = other.fields.Intersect(this.fields)
                                              .OrderByDescending(field => field.Id)
                                              .ToList();

            inForm1NotForm2.ForEach((Action<FormField>)(formField => {
                differences.Add($"Das Element {formField.Id} ist nur in der Datei {this.csvFile} enthalten");
                var formFieldId = formField.Id.Split("/").Last<string>();
                var baseFileName = Path.GetFileNameWithoutExtension(pngFile);
                var otherBaseFileName = Path.GetFileNameWithoutExtension(other.pngFile);
                var position = new Position(height: formField.Height, 
                                            left: (int)formField.Left, 
                                            top: formField.Top,
                                            width: formField.Width
                                           );
                visualDiff(redPen, position, directory, $"{baseFileName}_{formFieldId}.png");
                other.visualDiff(bluePen, position, directory, $"{otherBaseFileName}_{formFieldId}.png");
            }));

            inForm2NotForm1.ForEach((Action<FormField>)(formField => {
                differences.Add($"Das Element {formField.Id} ist nur in der Datei {other.csvFile} enthalten");
                var formFieldId = formField.Id.Split("/").Last<string>();
                var baseFileName = Path.GetFileNameWithoutExtension(pngFile);
                var otherBaseFileName = Path.GetFileNameWithoutExtension(other.pngFile);
                var position = new Position(height: formField.Height, 
                                            left: (int)formField.Left, 
                                            top: formField.Top,
                                            width: formField.Width
                                           );
                other.visualDiff(bluePen, position, directory, $"{otherBaseFileName}_{formFieldId}.png");
                visualDiff(redPen, position, directory, $"{baseFileName}_{formFieldId}.png");
            }));

            bool isTimeSpan(string str) {
                TimeSpan interval;
                return TimeSpan.TryParseExact(str, @"hh\:mm\:ss", CultureInfo.InvariantCulture, TimeSpanStyles.None, out interval);
            }

            foreach ((var formField1, var formField2) in inForm1AndForm2.Zip(inForm2AndForm1)) {
                var comparison = $"{this.csvFile}: {formField1.Id} => {formField1.Text}\n" + 
                                 $"{other.csvFile}: {formField2.Id} => {formField2.Text}";

                if (formField1.Text != formField2.Text) {
                    if (!isTimeSpan(formField1.Text) && !isTimeSpan(formField2.Text)) {
                        differences.Add(comparison);

                        var formFieldId = formField1.Id.Split("/").Last();
                        var baseFileName = Path.GetFileNameWithoutExtension(pngFile);
                        var otherBaseFileName = Path.GetFileNameWithoutExtension(other.pngFile);
                        var position1 = new Position(height: formField1.Height, 
                                                     left: formField1.Left, 
                                                     top: formField1.Top,
                                                     width: formField1.Width);
                        visualDiff(redPen, position1, directory, $"{baseFileName}_{formFieldId}.png");

                        formFieldId = formField2.Id.Split("/").Last();
                        baseFileName = Path.GetFileNameWithoutExtension(other.pngFile);
                        var position2 = new Position(height: formField2.Height, 
                                                     left: formField2.Left, 
                                                     top: formField2.Top,
                                                     width: formField2.Width);
                        other.visualDiff(bluePen, position2, directory, $"{otherBaseFileName}_{formFieldId}.png");
                    }
                }
                else {
                    matches.Add(comparison);
                }
            }

            return (differences, matches);
        }
    }
    
    public sealed record FormField(string Text, string Id, int Left, int Top, int Width, int Height): IEquatable<FormField> {
        public bool Equals(FormField? other) {
            if (other is null)
                return false;

            return this.Id == other.Id;
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}
