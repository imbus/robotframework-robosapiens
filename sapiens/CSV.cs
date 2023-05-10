using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace SAPiens {
    public sealed record FormField(string Text, string Id, int Left, int Top, int Width, int Height): IEquatable<FormField> {
        public bool Equals(FormField? other) {
            if (other is null)
                return false;

            return this.Id == other.Id;
        }

        public override int GetHashCode() => Id.GetHashCode();
    }

    public class CSVWriter<T> {
        CsvConfiguration config;

        public CSVWriter(string delimiter) {
            config = new CsvConfiguration(CultureInfo.InvariantCulture) {
                Delimiter = delimiter
            };
        }

        public void writeRows(string filename, List<T> records) {
            using (var writer = new StreamWriter(filename))
            using (var csvWriter = new CsvWriter(writer, config)) {
                csvWriter.WriteRecords(records);
            }
        }
    }
}
