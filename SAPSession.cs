using sapfewse;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Collections.Generic;
using System.Threading;

namespace RoboSAPiens {
    public sealed class NoSAPSession : ISession {}
    public sealed class SAPSession : ISession {
        GuiConnection connection;
        string systemName;
        GuiSession session;
        SAPWindow window;
        CLI.Options options;

        public SAPSession(GuiSession session, GuiConnection connection, CLI.Options options) {
            this.connection = connection;
            this.session = session;
            this.systemName = session.Info.SystemName;
            this.options = options;
            this.window = new SAPWindow(session.ActiveWindow, session, loadComponents: true);
        }

        bool windowChanged() {
            var currentWindow = session.ActiveWindow;
            
            return window.title != currentWindow.Text || 
                   window.id != currentWindow.Id;
        }

        ExceptionError? updateComponents() {
            try {
                window = new SAPWindow(session.ActiveWindow, session, loadComponents: true);
                return null;
            } 
            catch (Exception e){
                return new ExceptionError(e, "Die Erkennung der GUI-Elementen ist fehlgeschlagen.");
            }
        }

        ExceptionError? updateComponentsIfWindowChanged() {
            if (windowChanged()) {
                return updateComponents();
            }

            return null;
        }

        SapError? getStatusbarError() {
            var statusbar = window.components.getStatusBar();

            if (statusbar == null) {
                return null;
            }

            statusbar = new SAPStatusbar((GuiStatusbar)session.FindById(statusbar.id));

            return statusbar.getErrorMessage();
        }

        public RobotResult activateTab(string tabLabel) {
            switch (updateComponentsIfWindowChanged()) {
                case ExceptionError exceptionError: return exceptionError;
            }

            string theTab = $"Der Reiter '{tabLabel}'";            
            var tab = window.components.findTab(tabLabel);
            
            if (tab == null) {
                return new SpellingError($"{theTab} konnte nicht gefunden werden.");
            }

            if (options.presenterMode) switch(highlightElement(session, tab)) {
                case ExceptionError exceptionError: return exceptionError;
            }

            try {
                tab.select(session);

                switch (getStatusbarError()) {
                    case SapError sapError:
                        return sapError;
                }

                switch (updateComponents()) {
                    case ExceptionError exceptionError:
                        return exceptionError;
                }

                return new Success($"{theTab} wurde ausgewählt.");
            } 
            catch (Exception e) {
                return new ExceptionError(e, $"{theTab} konnte nicht ausgewählt werden.");
            }
        }

        public RobotResult closeConnection() {
            try {
                connection.CloseConnection();                
                return new Success($"Die Verbindung zum Server '{systemName}' wurde getrennt.");
            } 
            catch (Exception e) {
                return new ExceptionError(e, "Die Verbindung zum Server konnte nicht getrennt werden.");
            }
        }

        public RobotResult doubleClickCell(string rowIndexOrContent, string column) {
            switch (updateComponentsIfWindowChanged()) {
                case ExceptionError exceptionError: return exceptionError;
            }

            var locator = FilledCellLocator.of(rowIndexOrContent, column);
            var cell = window.components.findDoubleClickableCell(locator);

            if (cell == null) {
                return new SpellingError($"{locator.cell} konnte nicht gefunden werden.");
            }

            try {
                cell.doubleClick(session);
                return new Success($"{locator.cell} wurde doppelgeklickt.");
            }
            catch (Exception e) {
                return new ExceptionError(e, $"{locator.cell} konnte nicht doppelgeklickt werden.");
            }
        }

        public RobotResult doubleClickTextField(string content) {
            switch (updateComponentsIfWindowChanged()) {
                case ExceptionError exceptionError: return exceptionError;
            }

            var theTextField = new TextFieldLocator($"= {content}");
            var textField = window.components.findTextField(theTextField);

            if (textField == null) {
                return new SpellingError($"{theTextField.atLocation} konnte nicht gefunden werden.");
            }

            if (options.presenterMode) switch(highlightElement(session, textField)) {
                case ExceptionError exceptionError: return exceptionError;
            }

            try {
                textField.select(session);
                window.pressKey((int)VKey.F2); // Equivalent to double-click
                return new Success($"{theTextField.atLocation} wurde doppelgeklickt.");
            }
            catch (Exception e) {
                return new ExceptionError(e, $"{theTextField.atLocation} konnte nicht markiert werden.");
            }
        }

        public RobotResult executeTransaction(string tCode) {
            switch (updateComponentsIfWindowChanged()) {
                case ExceptionError exceptionError: return exceptionError;
            }

            if (window.isErrorWindow()) {
                return new SapError(window.getMessage());
            }
            
            string theTransaction = $"Die Transaktion mit T-Code '{tCode}'";

            try {
                session.SendCommand(tCode);
                return new Success($"{theTransaction} wurde erfolgreich ausgeführt.");
            }
            catch (Exception e) {
                return new ExceptionError(e, $"{theTransaction} konnte nicht ausgeführt werden.");
            }
        }

        public RobotResult exportForm(string formName, string directory) {
            // TODO: Write a function to replace invalid chars in the formName
            var fileName = $"{formName}_{systemName}".Replace("/", "_");
            var csv = new CSVWriter<FormField>(delimiter: ";");
            var formFields = new List<FormField>();

            var buttons = window.components.getAllButtons();
            var gridViewCells = window.components.getAllGridViewCells();
            var labels = window.components.getAllLabels();
            var tableCells = window.components.getAllTableCells();
            var textFields = window.components.getAllTextFields();

            buttons.ForEach(button => {
                var bottom = button.position.bottom - window.position.top;
                var left = button.position.left - window.position.left;
                var right = button.position.right - window.position.left;
                var top = button.position.top - window.position.top;
                var height = bottom - top;
                var width = right - left;
                formFields.Add(new FormField(button.text, button.id, left, top, width, height));
            });

            labels.ForEach(label => {
                var bottom = label.position.bottom - window.position.top;
                var left = label.position.left - window.position.left;
                var right = label.position.right - window.position.left;
                var top = label.position.top - window.position.top;
                var height = bottom - top;
                var width = right - left;
                formFields.Add(new FormField(label.getText(), label.id, left, top, width, height));
            });

            tableCells.ForEach(cell => {
                var bottom = cell.position.bottom - window.position.top;
                var left = cell.position.left - window.position.left;
                var right = cell.position.right - window.position.left;
                var top = cell.position.top - window.position.top;
                var height = bottom - top;
                var width = right - left;
                formFields.Add(new FormField(cell.text, cell.id, left, top, width, height));
            });

            gridViewCells.ForEach(cell => {
                var bottom = cell.position.bottom - window.position.top;
                var left = cell.position.left - window.position.left;
                var right = cell.position.right - window.position.left;
                var top = cell.position.top - window.position.top;
                var height = bottom - top;
                var width = right - left;
                formFields.Add(new FormField(cell.text, cell.columnId, left, top, width, height));
            });

            textFields.ForEach(textField => {
                var bottom = textField.position.bottom - window.position.top;
                var left = textField.position.left - window.position.left;
                var right = textField.position.right - window.position.left;
                var top = textField.position.top - window.position.top;
                var height = bottom - top;
                var width = right - left;
                formFields.Add(new FormField(textField.text, textField.id, left, top, width, height));
            });

            try {
                var csvPath = Path.Combine(directory, $"{fileName}.csv");
                csv.writeRows(csvPath, formFields);
                var pngPath = Path.Combine(directory, $"{fileName}.png");
                saveScreenshot(pngPath);
                return new Success($"Die Maske wurde in den Dateien {csvPath.ToString()} und {pngPath.ToString()} gespeichert");
            }
            catch (Exception e) {
                return new ExceptionError(e, $"Die Maske konnte nicht exportiert werden.");
            }
        }

        public RobotResult exportTree(string filePath) {
            var tree = window.components.getTree();

            if (tree == null)
            {
                // Define ComponentNotFoundError
                return new SapError("Die Maske enthält keine Baumstruktur");
            }

            try
            {
                var treeNodes = tree.getAllNodes(session);
                JSON.SaveJsonFile(filePath, treeNodes);

                return new Success($"Die Baumstruktur wurde in JSON Format in der Datei '{filePath}' gespeichert");
            }
            catch (Exception e)
            {
                return new ExceptionError(e, "Die Baumstruktur konnte nicht exportiert werden.");
            }
        }

        public RobotResult fillTableCell(string rowIndexOrRowLabel, string columnEqualsContent) {
            switch (updateComponentsIfWindowChanged()) {
                case ExceptionError exceptionError: return exceptionError;
            }

            if (!columnEqualsContent.Contains('=')) {
                return new InvalidFormatError("Das zweite Argument muss dem Muster `Spalte = Inhalt` entsprechen");
            }

            (var column, var content) = EmptyCellLocator.parseColumnContent(columnEqualsContent);

            var locator = EmptyCellLocator.of(rowIndexOrRowLabel, column);
            var cell = window.components.findEditableCell(locator);
            if (cell == null) {
                return new SpellingError($"{locator.cell} konnte nicht gefunden werden.");
            }

            var maxLength = cell.getMaxLength();
            if (maxLength != null && content.Length > maxLength) {
                return new InvalidValueError($"{locator.cell} kann maximal {maxLength} Zeichen enthalten.");
            }

            if (options.presenterMode) switch(highlightElement(session, cell)) {
                case ExceptionError exceptionError: return exceptionError;
            }

            try {
                cell.insert(content, session);
                return new Success($"{locator.cell} wurde ausgefüllt. Neuer Inhalt: {content}.");
            }
            catch (Exception e) {
                return new ExceptionError(e, $"{locator.cell} konnte nicht ausgefüllt werden.");
            }
        }

        public RobotResult fillTextField(string labels, string content) {
            switch (updateComponentsIfWindowChanged()) {
                case ExceptionError exceptionError: return exceptionError;
            }

            var theTextField = new TextFieldLocator(labels);
            var textField = window.components.findEditableTextField(theTextField);

            // The Changeable property could have been set to true after the window components were first read
            if (textField == null) {
                updateComponents();
                textField = window.components.findEditableTextField(theTextField);
            }

            if (textField == null) {
                return new SpellingError($"{theTextField.atLocation} konnte nicht gefunden werden.");
            }

            if (options.presenterMode) switch(highlightElement(session, textField)) {
                case ExceptionError exceptionError: return exceptionError;
            }

            try {
                textField.insert(content, session);
                return new Success($"{theTextField.atLocation} wurde ausgefüllt.");
            }
            catch (Exception e) {
                return new ExceptionError(e, $"{theTextField.atLocation} konnte nicht ausgefüllt werden. Möglicherweise, weil der Inhalt nicht dazu passt.");
            }
        }

        private RobotResult highlightElement(GuiSession session, IHighlightable element) {
            try
            {
                element.toggleHighlight(session);
                Thread.Sleep(500);
                element.toggleHighlight(session);

                return new Success("Das angegebene Element wurde markiert.");
            }
            catch (Exception e)
            {
                return new ExceptionError(e, "Das angegebene Element konnte nicht markiert werden.");
            }
        }

        public RobotResult pushButton(string label) {
            switch (updateComponents()) {
                case ExceptionError exceptionError: return exceptionError;
            }
            
            var theButton = new ButtonLocator(label);
            var button = window.components.findButton(theButton);

            if (button == null) {
                return new SpellingError($"{theButton.atLocation} konnte nicht gefunden werden.");
            }

            if (options.presenterMode) switch(highlightElement(session, button)) {
                    case ExceptionError exceptionError: return exceptionError;
            }

            try {
                button.push(session);
            } 
            catch (Exception e) {
                return new ExceptionError(e, $"{theButton.atLocation} konnte nicht gedrückt werden.");
            }

            switch (getStatusbarError()) {
                case SapError sapError : return sapError;
            }

            switch (updateComponents()) {
                case ExceptionError exceptionError:
                    return exceptionError;
            }

            return new Success($"{theButton.atLocation} wurde geklickt.");
        }

        public RobotResult pushButtonCell(string rowNumberOrButtonLabel, string column) {
            switch (updateComponentsIfWindowChanged()) {
                case ExceptionError exceptionError: return exceptionError;
            }

            var locator = FilledCellLocator.of(rowNumberOrButtonLabel, column);
            var button = window.components.findButtonCell(locator);

            if (button == null) {
                return new SpellingError($"{locator.cell} konnte nicht gefunden werden.");
            }

            if (options.presenterMode) switch(highlightElement(session, button)) {
                case ExceptionError exceptionError: return exceptionError;
            }

            try {
                button.push(session);
                return new Success($"{locator.cell} wurde gedrückt.");
            }
            catch (Exception e) {
                return new ExceptionError(e, $"{locator.cell} konnte nicht gedrückt werden.");
            }
        }

        public RobotResult readTableCell(string rowNumberOrButtonLabel, string column) {
            switch (updateComponentsIfWindowChanged()) {
                case ExceptionError exceptionError: return exceptionError;
            }

            var locator = FilledCellLocator.of(rowNumberOrButtonLabel, column);
            var cell = window.components.findCell(locator);

            if (cell == null) {
                return new SpellingError($"{locator.cell} konnte nicht gefunden werden.");
            }

            try {
                var text = cell.getText();
                return new Success(text, $"{locator.cell} wurde abgelesen.");
            }
            catch (Exception e) {
                return new ExceptionError(e, $"{locator.cell} konnte nicht abgelesen werden.");
            }
        }

        public RobotResult readTextField(string labels) {
            switch (updateComponentsIfWindowChanged()) {
                case ExceptionError exceptionError: return exceptionError;
            }

            var theTextField = new TextFieldLocator(labels);
            var textField = window.components.findReadOnlyTextField(theTextField);
            if (textField == null) {
                return new SpellingError($"{theTextField.atLocation} konnte nicht gefunden werden.");
            }

            if (options.presenterMode) switch(highlightElement(session, textField)) {
                case ExceptionError exceptionError: return exceptionError;
            }

            try {
                var text = textField.getText();
                return new Success(text, $"{theTextField.atLocation} wurde ausgelesen.");
            }
            catch (Exception e) {
                return new ExceptionError(e, $"{theTextField.atLocation} konnte nicht ausgelesen werden.");
            }
        }

        public RobotResult readText(string content) {
            switch (updateComponentsIfWindowChanged()) {
                case ExceptionError exceptionError: return exceptionError;
            }

            var text = window.components.findLabel(new LabelLocator(content)) ?? 
                       window.components.findReadOnlyTextField(new TextFieldLocator(content));

            if (text == null) {
                return new SpellingError($"Der Text '{content}' wurde nicht gefunden.");
            }

            try {
                return new Success(text.getText(), $"Der Text '{content}' wurde ausgelesen.");
            }
            catch (Exception e) {
                return new ExceptionError(e, $"Der Text '{content}' konnte nicht ausgelesen werden.");
            }
        }

        public RobotResult saveScreenshot(string path) {
            try {
                Path.GetFullPath(path);
            }
            catch (PathTooLongException e) {
                return new ExceptionError(e, $"Ein Pfad darf maximal 260 Zeichen enthalten. Der '{path}' enthält {path.Length} Zeichen.");
            }
            catch (System.Security.SecurityException e) {
                return new ExceptionError(e, $"Der Zugang zum Pfad '{path}' ist nicht zulässig.");
            }
            catch (Exception e) {
                return new ExceptionError(e, $"Der Pfad '{path}' ist ungültig.");
            }  
           
            var directory = Path.GetDirectoryName(path);

            if (directory == null) {
                return new InvalidValueError($"C: und Netzwerkverzeichnise sind nicht zulässige Ablageorte.");
            }

            if (directory == string.Empty) {
                return new InvalidValueError($"'{path}' ist kein absoluter Pfad.");
            }

            if (!Path.HasExtension(path)) {
                path += ".png";
            }

            try {
                Directory.CreateDirectory(directory);
                var window = (GuiFrameWindow)session.FindById(this.window.id);
                string outputPath = window.HardCopy(path, GuiImageType.PNG);

                return new Success($"Eine Aufnahme des Fensters wurde in '{outputPath}' gespeichert.");
            }
            catch (Exception e) {
                return new ExceptionError(e, "Eine Aufnahme des Fensters konnte nicht gespeichert werden");
            }
        }

        public RobotResult selectCell(string rowIndexOrCellContent, string column) {
            switch (updateComponentsIfWindowChanged()) {
                case ExceptionError exceptionError: return exceptionError;
            }

            var locator = FilledCellLocator.of(rowIndexOrCellContent, column);
            var cell = window.components.findCell(locator);

            if (cell == null) {
                return new SpellingError($"{locator.cell} konnte nicht gefunden werden.");
            }

            try {
                cell.select(session);
                return new Success($"{locator.cell} wurde markiert.");
            }
            catch (Exception e) {
                return new ExceptionError(e, $"{locator.cell} konnte nicht markiert werden.");
            }
        }

        public RobotResult selectComboBoxEntry(string labels, string entry) {
            switch (updateComponentsIfWindowChanged()) {
                case ExceptionError exceptionError: return exceptionError;
            }

            string theEntry = $"Der Eintrag '{entry}'";
            var theComboBox = new ComboBoxLocator(labels);
            var comboBox = window.components.findComboBox(theComboBox);
            
            if (comboBox == null) {
                return new SpellingError($"Das {theComboBox.atLocation} konnte nicht gefunden werden.");
            }

            if (!comboBox.contains(entry)) {
                return new SpellingError($"{theEntry} wurde im {theComboBox.atLocation} nicht gefunden.");
            }

            if (options.presenterMode) switch(highlightElement(session, comboBox)) {
                case ExceptionError exceptionError: return exceptionError;
            }

            try {
                comboBox.select(entry, session);
                return new Success($"{theEntry} aus dem {theComboBox.atLocation} wurde ausgewählt.");
            }
            catch (Exception e) {
                return new ExceptionError(e, $"{theEntry} aus dem {theComboBox.atLocation} konnte nicht ausgewählt werden.");
            }
        }

        public RobotResult selectTextField(string labels) {
            switch (updateComponentsIfWindowChanged()) {
                case ExceptionError exceptionError: return exceptionError;
            }

            var theTextField = new TextFieldLocator(labels);
            var textField = window.components.findTextField(theTextField);

            if (textField == null) {
                return new SpellingError($"{theTextField.atLocation} konnte nicht gefunden werden.");
            }

            if (options.presenterMode) switch(highlightElement(session, textField)) {
                case ExceptionError exceptionError: return exceptionError;
            }

            try {
                textField.select(session);
                return new Success($"{theTextField.atLocation} wurde markiert.");
            }
            catch (Exception e) {
                return new ExceptionError(e, $"{theTextField.atLocation} konnte nicht markiert werden.");
            }
        }
        
        public RobotResult selectTextLine(string text) {
            switch (updateComponentsIfWindowChanged()) {
                case ExceptionError exceptionError: return exceptionError;
            }

            string theLine = $"Die Textzeile '{text}'";
            var textLine = window.components.findLabel(new LabelLocator($"= {text}"));

            if (textLine == null) {
                return new SpellingError($"{theLine} konnte nicht gefunden werden.");
            }

            try {
                textLine.select(session);
                return new Success($"{theLine} wurde markiert.");
            }
            catch (Exception e) {
                return new ExceptionError(e, $"{theLine} konnte nicht markiert werden.");
            }
        }

        public RobotResult selectRadioButton(string labels) {
            switch (updateComponentsIfWindowChanged()) {
                case ExceptionError exceptionError: return exceptionError;
            }

            var theRadioButton = new RadioButtonLocator(labels);
            var radioButton = window.components.findRadioButton(theRadioButton);

            if (radioButton == null) {
                return new SpellingError($"{theRadioButton.atLocation} konnte nicht gefunden werden.");
            }

            if (options.presenterMode) switch(highlightElement(session, radioButton)) {
                case ExceptionError exceptionError: return exceptionError;
            }

            try {
                radioButton.select(session);
                return new Success($"{theRadioButton.atLocation} wurde ausgewählt.");
            }
            catch (Exception e) {
                return new ExceptionError(e, $"{theRadioButton.atLocation} konnte nicht ausgewählt werden.");
            }
        }

        // TODO: Define a keyword "Tabellenzeile markieren"
        // Store the tables. When this keyword is called:
        // 1. If there is more than one table, find the table using the title of the enclosing box.
        // 2. Get the row and select it.

        public RobotResult tickCheckBox(string labels) {
            switch (updateComponentsIfWindowChanged()) {
                case ExceptionError exceptionError: return exceptionError;
            }

            var theCheckBox = new CheckBoxLocator(labels);
            var checkBox = window.components.findCheckBox(theCheckBox);
            
            if (checkBox == null) {
                return new SpellingError($"{theCheckBox.atLocation} konnte nicht gefunden werden.");
            }

            if (options.presenterMode) switch(highlightElement(session, checkBox)) {
                case ExceptionError exceptionError: return exceptionError;
            }
            
            try {
                checkBox.select(session);
                return new Success($"{theCheckBox.atLocation} wurde angekreuzt.");
            }
            catch (Exception e) {
                return new ExceptionError(e, $"{theCheckBox.atLocation} konnte nicht angekreuzt werden.");
            }
        }

        public RobotResult untickCheckBox(string labels) {
            switch (updateComponentsIfWindowChanged()) {
                case ExceptionError exceptionError: return exceptionError;
            }

            var theCheckBox = new CheckBoxLocator(labels);
            var checkBox = window.components.findCheckBox(theCheckBox);
            
            if (checkBox == null) {
                return new SpellingError($"{theCheckBox.atLocation} konnte nicht gefunden werden.");
            }

            if (options.presenterMode) switch(highlightElement(session, checkBox)) {
                case ExceptionError exceptionError: return exceptionError;
            }
            
            try {
                checkBox.deselect(session);
                return new Success($"{theCheckBox.atLocation} wurde abgewählt.");
            }
            catch (Exception e) {
                return new ExceptionError(e, $"{theCheckBox.atLocation} konnte nicht abgewählt werden.");
            }
        }

        public RobotResult tickCheckBoxCell(string rowIndex, string column) {
            switch (updateComponentsIfWindowChanged()) {
                case ExceptionError exceptionError: return exceptionError;
            }

            var locator = FilledCellLocator.of(rowIndex, column);
            var checkBox = window.components.findCheckBoxCell(locator);
            
            if (checkBox == null) {
                return new SpellingError($"{locator.cell} konnte nicht gefunden werden.");
            }

            if (options.presenterMode) switch(highlightElement(session, checkBox)) {
                case ExceptionError exceptionError: return exceptionError;
            }

            try {
                checkBox.select(session);
                return new Success($"{locator.cell} wurde angekreuzt.");
            }
            catch (Exception e) {
                return new ExceptionError(e, $"{locator.cell} konnte nicht angekreuzt werden.");
            }
        }

        public RobotResult getWindowText() {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            return new Result.GetWindowText.Pass(window.getMessage());
        }

        public RobotResult getWindowTitle() {
            if (windowChanged()) {
                switch (updateWindow(updateComponents: false)) {
                    case RobotResult.UIScanFail exceptionError: return exceptionError;
                }
            }

            return new Result.GetWindowTitle.Pass(window.title);
        }
    }
}
