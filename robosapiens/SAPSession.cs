using sapfewse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace RoboSAPiens {
    public sealed class NoSAPSession : ISession {}
    public sealed class SAPSession : ISession {
        GuiConnection connection;
        ILogger logger;
        Options options;
        GuiSession session;
        string systemName;
        SAPWindow window;

        public SAPSession(GuiSession session, GuiConnection connection, Options options, ILogger logger) {
            this.connection = connection;
            this.logger = logger;
            this.session = session;
            this.systemName = session.Info.SystemName;
            this.options = options;
            this.window = new SAPWindow(session.ActiveWindow, session, loadComponents: true, debug: options.debug);
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
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new RobotResult.UIScanFail(e);
            }
        }

        RobotResult.UIScanFail? updateComponentsIfWindowChanged() {
            if (windowChanged()) {
                return updateWindow(updateComponents: true);
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
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
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

                switch (updateWindow(updateComponents: true)) {
                    case RobotResult.UIScanFail exceptionError:
                        return exceptionError;
                }

                return new Result.ActivateTab.Pass(tabLabel);
            } 
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.ActivateTab.Exception(e);
            }
        }

        public RobotResult closeConnection() {
            try {
                connection.CloseConnection();                
                return new Result.CloseConnection.Pass(systemName);
            }
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
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
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.DoubleClickCell.Exception(e);
            }
        }

        public RobotResult doubleClickTextField(string label) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var theTextField = new TextFieldLocator(label);
            var textField = window.components.findTextField(theTextField);

            if (textField == null) {
                return new Result.DoubleClickTextField.NotFound(theTextField.atLocation);
            }

            if (options.presenterMode) switch(highlightElement(session, textField)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }

            try {
                textField.select(session);
                window.pressKey((int)VKeys.getKeyCombination("F2")!); // Equivalent to double-click
                return new Result.DoubleClickTextField.Pass(theTextField.atLocation);
            }
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
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
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.ExecuteTransaction.Exception(e);
            }
        }

        public string toValidFilename(String filename)
        {
            var forbiddenChars = new List<char>
            {
                '/', '<', '>', ':', '"', '/', '\\', '|', '?', '*'
            };

            return String.Join("", filename.Where(c => !forbiddenChars.Contains(c)));
        }

        public RobotResult exportSpreadsheet(string index) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var gridViews = window.components.getGridViews();

            try 
            {
                int idx = Int32.Parse(index) - 1;

                if (idx > gridViews.Count - 1) return new Result.ExportSpreadsheet.NotFound();

                gridViews[idx].exportSpreadsheet(session);
                return new Result.ExportSpreadsheet.Pass();
            }
            catch (Exception e) 
            {
                return new Result.ExportSpreadsheet.Exception(e);
            }
        }

        public RobotResult exportForm(string formName, string directory) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var fileName = toValidFilename(formName);
            var formFields = new List<FormField>();

            var buttons = window.components.getAllButtons();
            var labels = window.components.getAllLabels();
            var tableCells = window.components.getAllTableCells();
            var textFields = window.components.getAllTextFields();

            buttons.ForEach(button => {
                var bottom = button.position.bottom;
                var left = button.position.left;
                var right = button.position.right;
                var top = button.position.top;
                var height = bottom - top;
                var width = right - left;
                formFields.Add(new FormField(button.text, "", button.id, left, top, width, height));
            });

            labels.ForEach(label => {
                var bottom = label.position.bottom;
                var left = label.position.left;
                var right = label.position.right;
                var top = label.position.top;
                var height = bottom - top;
                var width = right - left;
                formFields.Add(new FormField(label.getText(), "", label.id, left, top, width, height));
            });

            tableCells.ForEach(cell => {
                var bottom = cell.position.bottom;
                var left = cell.position.left;
                var right = cell.position.right;
                var top = cell.position.top;
                var height = bottom - top;
                var width = right - left;
                formFields.Add(new FormField(cell.text, "", cell.id, left, top, width, height));
            });

            textFields.ForEach(textField => {
                var bottom = textField.position.bottom;
                var left = textField.position.left;
                var right = textField.position.right;
                var top = textField.position.top;
                var height = bottom - top;
                var width = right - left;

                var sapLabel = textField.findClosestHorizontalComponent(labels.Cast<ILocatable>().ToList()) as SAPLabel;
                var label = sapLabel?.getText() ?? "";

                formFields.Add(new FormField(textField.text, label, textField.id, left, top, width, height));
            });

            try {
                Directory.CreateDirectory(directory);
                var jsonPath = Path.Combine(directory, $"{fileName}.json");
                JSON.writeFile(jsonPath, JSON.serialize(formFields, typeof(List<FormField>)));
                var pngPath = Path.Combine(directory, $"{fileName}.png");
                saveScreenshot(pngPath);
                return new Result.ExportForm.Pass(jsonPath, pngPath);
            }
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.ExportForm.Exception(e);
            }
        }

        public RobotResult exportTree(string filePath) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var tree = window.components.getTree();

            if (tree == null) return new Result.ExportTree.NotFound();

            try
            {
                var treeNodes = tree.getAllNodes(session);
                JSON.writeFile(filePath, JSON.serialize(treeNodes, typeof(List<SAPTree.Node>)));
                return new Result.ExportTree.Pass(filePath);
            }
            catch (Exception e)
            {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.ExportTree.Exception(e);
            }
        }

        public RobotResult fillTableCell(string rowIndexOrRowLabel, string columnEqualsContent, string? text) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            (var column, var content) = text switch
            {
                string => EmptyCellLocator.parseColumnContent(columnEqualsContent + " = " + text),
                null => EmptyCellLocator.parseColumnContent(columnEqualsContent)
            };

            var locator = EmptyCellLocator.of(rowIndexOrRowLabel, column);
            var cell = window.components.findEditableCell(locator);

            // The Changeable property could have been set to true after the window components were first read
            if (cell == null) {
                updateWindow(updateComponents: true);
                cell = window.components.findEditableCell(locator);
            }

            if (cell == null) {
                return new Result.FillTableCell.NotFound(locator.cell);
            }

            // Disabled because it could happen that the result of getMaxLength()
            // does not correspond with the actual maximum length.
            // var maxLength = cell.getMaxLength();
            // if (maxLength != null && content.Length > maxLength) {
            //     return new Result.FillTableCell.TooManyChars(locator.cell, maxLength);
            // }

            if (options.presenterMode) switch(highlightElement(session, cell)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }

            try {
                // The Changeable property could have been set to false after the window components were first read
                switch(cell.insert(content, session)) {
                    case RobotResult.NotChangeable: return new Result.FillTableCell.NotChangeable(locator.cell);
                    default: return new Result.FillTableCell.Pass(locator.cell);
                };
            }
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
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
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.FillTextField.Exception(e);
            }
        }

        public RobotResult highlightButton(string label) {
            switch (updateWindow(updateComponents: true)) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }
            
            var theButton = new ButtonLocator(label);
            var button = window.components.findButton(theButton);

            if (button == null) {
                return new Result.HighlightButton.NotFound(theButton.atLocation);
            }

            return highlightElement(session, button) switch {
                RobotResult.HighlightFail(var exception) => 
                    new Result.HighlightButton.Exception(exception),
                _ => new Result.HighlightButton.Pass(theButton.atLocation)
            };
        }

        public RobotResult pressKeyCombination(string keyCombination) {
            int? vkey = VKeys.getKeyCombination(keyCombination);
            
            if (vkey == null) {
                return new Result.PressKeyCombination.NotFound(keyCombination);
            }
            
            try 
            {
                if (keyCombination == "F1" || keyCombination == "F4")
                {
                    var gridView = window.components.getGridViews().FirstOrDefault();
                    if (gridView != null) gridView.pressKey(keyCombination, session);
                    else window.pressKey((int)vkey!);
                }
                else window.pressKey((int)vkey!);

               return new Result.PressKeyCombination.Pass(keyCombination);
            }
            catch (Exception ex) {
                return new Result.PressKeyCombination.Exception(ex);
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
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.PushButton.Exception(e);
            }

            // Pushing a Button may result in the window being rerendered,
            // and the properties of some components may change.
            if (!windowChanged()) {
                switch (updateWindow(updateComponents: true)) {
                    case RobotResult.UIScanFail exceptionError:
                        return exceptionError;
                }
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
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.PushButtonCell.Exception(e);
            }
        }

        public RobotResult readStatusbar() {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var statusbar = window.components.getStatusBar();

            if (statusbar == null) {
                return new Result.ReadStatusbar.NotFound();
            }

            statusbar = new SAPStatusbar((GuiStatusbar)session.FindById(statusbar.id));

            return new Result.ReadStatusbar.Pass(statusbar.getMessage());
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
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.ReadTableCell.Exception(e);
            }
        }

        public RobotResult readTextField(string labels) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var theTextField = new TextFieldLocator(labels);
            var textField = window.components.findTextField(theTextField);
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
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
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
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.ReadText.Exception(e);
            }
        }

        public RobotResult saveScreenshot(string path) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            try {
                var window = (GuiFrameWindow)session.FindById(this.window.id);
                var screenshot = (byte[])window.HardCopyToMemory(GuiImageType.PNG);

                if (path.Equals("LOG"))
                {
                    var img_b64 = Convert.ToBase64String(screenshot);
                    var log_image = $"*HTML* <img src='data:image/png;base64, {img_b64}'>";
                    return new Result.SaveScreenshot.Log(log_image);
                }
                
                if (path.StartsWith(@"\\")) return new Result.SaveScreenshot.UNCPath();

                var directory = Path.GetDirectoryName(path);

                if (directory == @"\") {
                    return new Result.SaveScreenshot.InvalidPath(path);
                }

                if (directory == null) {
                    return new Result.SaveScreenshot.InvalidPath(path);
                }

                if (directory == string.Empty) {
                    return new Result.SaveScreenshot.NoAbsPath(path);
                }

                if (!Path.HasExtension(path)) {
                    path += ".png";
                }

                Directory.CreateDirectory(directory);
                using var writer = new BinaryWriter(File.OpenWrite(path));
                writer.Write(screenshot);

                return new Result.SaveScreenshot.Pass(path);
            }
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
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
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.SelectCell.Exception(e);
            }
        }

        public RobotResult selectCellValue(string rowIndexOrCellContent, string column, string entry) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var locator = FilledCellLocator.of(rowIndexOrCellContent, column);
            var cell = window.components.findComboBoxCell(locator);

            if (cell == null) {
                return new Result.SelectCellValue.NotFound(locator.cell);
            }

            if (!cell.contains(entry)) {
                return new Result.SelectCellValue.EntryNotFound(entry);
            }

            try {
                cell.select(entry, session);
                return new Result.SelectCellValue.Pass(entry, locator.cell);
            }
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.SelectCellValue.Exception(e);
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
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.SelectComboBoxEntry.Exception(e);
            }
        }

        public RobotResult selectTableRow(string rowIndex) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var tables = window.components.getTables();
            var gridViews = window.components.getGridViews();

            if (tables.Count() == 0 && gridViews.Count() == 0)
                return new Result.SelectTableRow.NotFound();

            try {
                int intRowIndex = Int32.Parse(rowIndex);
                if (tables.Count() > 0) {
                    tables.First().selectRow(intRowIndex, session);
                }
                if (gridViews.Count() > 0) {
                    gridViews.First().selectRow(intRowIndex, session);
                }
                return new Result.SelectTableRow.Pass(intRowIndex);
            }
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.SelectTableRow.Exception(e);
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
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
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

            if (options.presenterMode) switch(highlightElement(session, textLine)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }

            try {
                textLine.select(session);
                return new Result.SelectTextLine.Pass(text);
            }
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
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
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.SelectRadioButton.Exception(e);
            }
        }

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
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
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
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
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
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
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
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            return new Result.GetWindowTitle.Pass(window.title);
        }
    }
}
