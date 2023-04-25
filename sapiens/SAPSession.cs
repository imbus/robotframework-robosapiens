using sapfewse;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Text.RegularExpressions;

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

        RobotResult.UIScanFail? updateWindow(bool updateComponents) {
            try {
                window = new SAPWindow(session.ActiveWindow, session, updateComponents);
                return null;
            } 
            catch (Exception e){
                return new RobotResult.UIScanFail(e);
            }
        }

        RobotResult.UIScanFail? updateComponentsIfWindowChanged() {
            if (windowChanged()) {
                return updateWindow(updateComponents: true);
            }

            return null;
        }

        string? getStatusbarError() {
            var statusbar = window.components.getStatusBar();

            if (statusbar == null) {
                return null;
            }

            statusbar = new SAPStatusbar((GuiStatusbar)session.FindById(statusbar.id));

            if (statusbar.hasErrorMessage()) {
                return statusbar.getMessage();
            }

            return null;
        }

        private RobotResult.HighlightFail? highlightElement(GuiSession session, IHighlightable element) {
            try
            {
                element.toggleHighlight(session);
                Thread.Sleep(500);
                element.toggleHighlight(session);

                return null;
            }
            catch (Exception e)
            {
                return new RobotResult.HighlightFail(e);
            }
        }

        public RobotResult activateTab(string tabLabel) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var tab = window.components.findTab(tabLabel);
            
            if (tab == null) {
                return new Result.ActivateTab.NotFound(tabLabel);
            }

            if (options.presenterMode) switch(highlightElement(session, tab)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }

            try {
                tab.select(session);

                switch (getStatusbarError()) {
                    case string sapError:
                        return new Result.ActivateTab.SapError(sapError);
                }

                switch (updateWindow(updateComponents: true)) {
                    case RobotResult.UIScanFail exceptionError:
                        return exceptionError;
                }

                return new Result.ActivateTab.Pass(tabLabel);
            } 
            catch (Exception e) {
                return new Result.ActivateTab.Exception(e);
            }
        }

        public RobotResult closeConnection() {
            try {
                connection.CloseConnection();                
                return new Result.CloseConnection.Pass(systemName);
            }
            catch (Exception e) {
                return new Result.CloseConnection.Exception(e);
            }
        }

        public RobotResult doubleClickCell(string rowIndexOrContent, string column) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var locator = FilledCellLocator.of(rowIndexOrContent, column);
            var cell = window.components.findDoubleClickableCell(locator);

            if (cell == null) {
                return new Result.DoubleClickCell.NotFound(locator.cell);
            }

            try {
                cell.doubleClick(session);
                return new Result.DoubleClickCell.Pass(locator.cell);
            }
            catch (Exception e) {
                return new Result.DoubleClickCell.Exception(e);
            }
        }

        public RobotResult doubleClickTextField(string content) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var theTextField = new TextFieldLocator($"= {content}");
            var textField = window.components.findTextField(theTextField);

            if (textField == null) {
                return new Result.DoubleClickTextField.NotFound(theTextField.atLocation);
            }

            if (options.presenterMode) switch(highlightElement(session, textField)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }

            try {
                textField.select(session);
                window.pressKey((int)VKey.F2); // Equivalent to double-click
                return new Result.DoubleClickTextField.Pass(theTextField.atLocation);
            }
            catch (Exception e) {
                return new Result.DoubleClickTextField.Exception(e);
            }
        }

        public RobotResult executeTransaction(string tCode) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }
            
            try {
                session.SendCommand(tCode);
                return new Result.ExecuteTransaction.Pass(tCode);
            }
            catch (Exception e) {
                return new Result.ExecuteTransaction.Exception(e);
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
                return new Result.ExportForm.Pass(csvPath, pngPath);
            }
            catch (Exception e) {
                return new Result.ExportForm.Exception(e);
            }
        }

        public RobotResult exportTree(string filePath) {
            var tree = window.components.getTree();

            if (tree == null) return new Result.ExportTree.NotFound();

            try
            {
                var treeNodes = tree.getAllNodes(session);
                JSON.SaveJsonFile(filePath, treeNodes);

                return new Result.ExportTree.Pass(filePath);
            }
            catch (Exception e)
            {
                return new Result.ExportTree.Exception(e);
            }
        }

        public RobotResult fillTableCell(string rowIndexOrRowLabel, string columnEqualsContent) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            if (!columnEqualsContent.Contains('=')) {
                return new Result.FillTableCell.InvalidFormat();
            }

            (var column, var content) = EmptyCellLocator.parseColumnContent(columnEqualsContent);

            var locator = EmptyCellLocator.of(rowIndexOrRowLabel, column);
            var cell = window.components.findEditableCell(locator);
            if (cell == null) {
                return new Result.FillTableCell.NotFound(locator.cell);
            }

            // TODO: If SAP checks this, get the error message from the statusbar
            // var maxLength = cell.getMaxLength();
            // if (maxLength != null && content.Length > maxLength) {
            //     return new InvalidValueError($"The cell {locator.cell} may contain at most {maxLength} characters.");
            // }

            if (options.presenterMode) switch(highlightElement(session, cell)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }

            try {
                cell.insert(content, session);
                return new Result.FillTableCell.Pass(locator.cell);
            }
            catch (Exception e) {
                return new Result.FillTableCell.Exception(e);
            }
        }

        public RobotResult fillTextField(string labels, string content) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var theTextField = new TextFieldLocator(labels);
            var textField = window.components.findEditableTextField(theTextField);

            // The Changeable property could have been set to true after the window components were first read
            if (textField == null) {
                updateWindow(updateComponents: true);
                textField = window.components.findEditableTextField(theTextField);
            }

            if (textField == null) {
                return new Result.FillTextField.NotFound(theTextField.atLocation);
            }

            if (options.presenterMode) switch(highlightElement(session, textField)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }

            try {
                textField.insert(content, session);
                return new Result.FillTextField.Pass(theTextField.atLocation);
            }
            catch (Exception e) {
                return new Result.FillTextField.Exception(e);
            }
        }

        public RobotResult pushButton(string label) {
            switch (updateWindow(updateComponents: true)) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }
            
            var theButton = new ButtonLocator(label);
            var button = window.components.findButton(theButton);

            if (button == null) {
                return new Result.PushButton.NotFound(theButton.atLocation);
            }

            if (options.presenterMode) switch(highlightElement(session, button)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }

            try {
                button.push(session);
            } 
            catch (Exception e) {
                return new Result.PushButton.Exception(e);
            }

            switch (getStatusbarError()) {
                case string sapError : return new Result.PushButton.SapError(sapError);
            }

            switch (updateWindow(updateComponents: true)) {
                case RobotResult.UIScanFail exceptionError:
                    return exceptionError;
            }

            return new Result.PushButton.Pass(theButton.atLocation);
        }

        public RobotResult pushButtonCell(string rowNumberOrButtonLabel, string column) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var locator = FilledCellLocator.of(rowNumberOrButtonLabel, column);
            var button = window.components.findButtonCell(locator);

            if (button == null) {
                return new Result.PushButtonCell.NotFound(locator.cell);
            }

            if (options.presenterMode) switch(highlightElement(session, button)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }

            try {
                button.push(session);
                return new Result.PushButtonCell.Pass(locator.cell);
            }
            catch (Exception e) {
                return new Result.PushButtonCell.Exception(e);
            }
        }

        public RobotResult readTableCell(string rowNumberOrButtonLabel, string column) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var locator = FilledCellLocator.of(rowNumberOrButtonLabel, column);
            var cell = window.components.findCell(locator);

            if (cell == null) {
                return new Result.ReadTableCell.NotFound(locator.cell);
            }

            try {
                var text = cell.getText();
                return new Result.ReadTableCell.Pass(text, locator.cell);
            }
            catch (Exception e) {
                return new Result.ReadTableCell.Exception(e);
            }
        }

        public RobotResult readTextField(string labels) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var theTextField = new TextFieldLocator(labels);
            var textField = window.components.findReadOnlyTextField(theTextField);
            if (textField == null) {
                return new Result.ReadTextField.NotFound(theTextField.atLocation);
            }

            if (options.presenterMode) switch(highlightElement(session, textField)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }

            try {
                var text = textField.getText();
                return new Result.ReadTextField.Pass(text, theTextField.atLocation);
            }
            catch (Exception e) {
                return new Result.ReadTextField.Exception(e);
            }
        }

        public RobotResult readText(string content) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var text = window.components.findLabel(new LabelLocator(content)) ?? 
                       window.components.findReadOnlyTextField(new TextFieldLocator(content));

            if (text == null) {
                return new Result.ReadText.NotFound(content);
            }

            try {
                return new Result.ReadText.Pass(text.getText());
            }
            catch (Exception e) {
                return new Result.ReadText.Exception(e);
            }
        }

        public RobotResult saveScreenshot(string path) {
            if (path.StartsWith(@"\\")) return new Result.SaveScreenshot.UNCPath();

            var pathRegex = new Regex(@"^(?:[a-zA-Z]\:)\\(?:[\w\s\.]+\\)*[\w\s\.]+?$");

            if (!pathRegex.Match(path).Success) return new Result.SaveScreenshot.InvalidPath(path);

            var directory = Path.GetDirectoryName(path);

            if (directory == @"\") {
                return new Result.SaveScreenshot.InvalidPath(path);
            }

            if (directory == null) {
                return new Result.SaveScreenshot.UNCPath();
            }

            if (directory == string.Empty) {
                return new Result.SaveScreenshot.NoAbsPath(path);
            }

            if (!Path.HasExtension(path)) {
                path += ".png";
            }

            try {
                Directory.CreateDirectory(directory);
                var window = (GuiFrameWindow)session.FindById(this.window.id);
                var screenshot = (byte[])window.HardCopyToMemory(GuiImageType.PNG);

                using var writer = new BinaryWriter(File.OpenWrite(path));
                writer.Write(screenshot);

                return new Result.SaveScreenshot.Pass(path);
            }
            catch (Exception e) {
                return new Result.SaveScreenshot.Exception(e);
            }
        }

        public RobotResult selectCell(string rowIndexOrCellContent, string column) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var locator = FilledCellLocator.of(rowIndexOrCellContent, column);
            var cell = window.components.findCell(locator);

            if (cell == null) {
                return new Result.SelectCell.NotFound(locator.cell);
            }

            try {
                cell.select(session);
                return new Result.SelectCell.Pass(locator.cell);
            }
            catch (Exception e) {
                return new Result.SelectCell.Exception(e);
            }
        }

        public RobotResult selectComboBoxEntry(string labels, string entry) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var theComboBox = new ComboBoxLocator(labels);
            var comboBox = window.components.findComboBox(theComboBox);
            
            if (comboBox == null) {
                return new Result.SelectComboBoxEntry.NotFound(theComboBox.atLocation);
            }

            if (!comboBox.contains(entry)) {
                return new Result.SelectComboBoxEntry.EntryNotFound(entry);
            }

            if (options.presenterMode) switch(highlightElement(session, comboBox)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }

            try {
                comboBox.select(entry, session);
                return new Result.SelectComboBoxEntry.Pass(entry);
            }
            catch (Exception e) {
                return new Result.SelectComboBoxEntry.Exception(e);
            }
        }

        public RobotResult selectTextField(string labels) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var theTextField = new TextFieldLocator(labels);
            var textField = window.components.findTextField(theTextField);

            if (textField == null) {
                return new Result.SelectTextField.NotFound(theTextField.atLocation);
            }

            if (options.presenterMode) switch(highlightElement(session, textField)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }

            try {
                textField.select(session);
                return new Result.SelectTextField.Pass(theTextField.atLocation);
            }
            catch (Exception e) {
                return new Result.SelectTextField.Exception(e);
            }
        }
        
        public RobotResult selectTextLine(string text) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var textLine = window.components.findLabel(new LabelLocator($"= {text}"));

            if (textLine == null) {
                return new Result.SelectTextLine.NotFound(text);
            }

            try {
                textLine.select(session);
                return new Result.SelectTextLine.Pass(text);
            }
            catch (Exception e) {
                return new Result.SelectTextLine.Exception(e);
            }
        }

        public RobotResult selectRadioButton(string labels) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var theRadioButton = new RadioButtonLocator(labels);
            var radioButton = window.components.findRadioButton(theRadioButton);

            if (radioButton == null) {
                return new Result.SelectRadioButton.NotFound(theRadioButton.atLocation);
            }

            if (options.presenterMode) switch(highlightElement(session, radioButton)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }

            try {
                radioButton.select(session);
                return new Result.SelectRadioButton.Pass(theRadioButton.atLocation);
            }
            catch (Exception e) {
                return new Result.SelectRadioButton.Exception(e);
            }
        }

        // TODO: Define a keyword "Select Table Row"
        // Store the tables. When this keyword is called:
        // 1. If there is more than one table, find the table using the title of the enclosing box.
        // 2. Get the row and select it.

        public RobotResult tickCheckBox(string labels) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var theCheckBox = new CheckBoxLocator(labels);
            var checkBox = window.components.findCheckBox(theCheckBox);
            
            if (checkBox == null) {
                return new Result.TickCheckBox.NotFound(theCheckBox.atLocation);
            }

            if (options.presenterMode) switch(highlightElement(session, checkBox)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }
            
            try {
                checkBox.select(session);
                return new Result.TickCheckBox.Pass(theCheckBox.atLocation);
            }
            catch (Exception e) {
                return new Result.TickCheckBox.Exception(e);
            }
        }

        public RobotResult untickCheckBox(string labels) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var theCheckBox = new CheckBoxLocator(labels);
            var checkBox = window.components.findCheckBox(theCheckBox);
            
            if (checkBox == null) {
                return new Result.UntickCheckBox.NotFound(theCheckBox.atLocation);
            }

            if (options.presenterMode) switch(highlightElement(session, checkBox)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }
            
            try {
                checkBox.deselect(session);
                return new Result.UntickCheckBox.Pass(theCheckBox.atLocation);
            }
            catch (Exception e) {
                return new Result.UntickCheckBox.Exception(e);
            }
        }

        public RobotResult tickCheckBoxCell(string rowIndex, string column) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var locator = FilledCellLocator.of(rowIndex, column);
            var checkBox = window.components.findCheckBoxCell(locator);
            
            if (checkBox == null) {
                return new Result.TickCheckBoxCell.NotFound(locator.cell);
            }

            if (options.presenterMode) switch(highlightElement(session, checkBox)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }

            try {
                checkBox.select(session);
                return new Result.TickCheckBoxCell.Pass(locator.cell);
            }
            catch (Exception e) {
                return new Result.TickCheckBoxCell.Exception(e);
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
