using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;

namespace RoboSAPiens {
    public abstract class CSVReaderWriter {
        protected CsvConfiguration config;

        public CSVReaderWriter(string delimiter) {
            config = new CsvConfiguration(CultureInfo.InvariantCulture) {
                Delimiter = delimiter
            };
        }
    }

    public class CSVReader<T>: CSVReaderWriter {
        public CSVReader(string delimiter): base(delimiter) {}

        public List<T> readRows(string filename) {
            using (var reader = new StreamReader(filename))
            using (var csv = new CsvReader(reader, config)) {
                return csv.GetRecords<T>().ToList();
            }
        }
    }

    public class CSVWriter<T>: CSVReaderWriter {
        public CSVWriter(string delimiter): base(delimiter) {}

        public void writeRows(string filename, List<T> records) {
            using (var writer = new StreamWriter(filename))
            using (var csvWriter = new CsvWriter(writer, config)) {
                csvWriter.WriteRecords(records);
            }
        }
    }
}
