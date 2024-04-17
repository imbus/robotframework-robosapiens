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
            this.window = new SAPWindow(session.ActiveWindow, session, debug: options.debug);
        }

        bool windowChanged() {
            var currentWindow = session.ActiveWindow;
            
            return window.title != currentWindow.Text || 
                   window.id != currentWindow.Id;
        }

        RobotResult.UIScanFail? updateWindow() {
            try {
                window = new SAPWindow(session.ActiveWindow, session);
                return null;
            } 
            catch (Exception e){
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new RobotResult.UIScanFail(e);
            }
        }

        RobotResult.UIScanFail? updateComponentsIfWindowChanged() {
            if (windowChanged()) {
                return updateWindow();
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

                switch (updateWindow()) {
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

        public RobotResult selectMenuItem(string itemPath) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }
            
            var menu = window.components.findMenuItem(itemPath);
            
            if (menu == null) {
                return new Result.SelectMenuItem.NotFound(itemPath);
            }

            try {
                menu.select(session);

                switch (updateWindow()) {
                    case RobotResult.UIScanFail exceptionError:
                        return exceptionError;
                }

                return new Result.SelectMenuItem.Pass(itemPath);
            } 
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.SelectMenuItem.Exception(e);
            }
        }

        public RobotResult expandTreeElement(string elementPath, string column) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var locator = CellLocator.of(elementPath, column);
            var folder = window.components.findTreeFolder(locator, session);
            
            if (folder == null) {
                return new Result.ExpandTreeElement.NotFound(elementPath);
            }

            try {
                folder.open(session);

                switch (updateWindow()) {
                    case RobotResult.UIScanFail exceptionError:
                        return exceptionError;
                }

                return new Result.ExpandTreeElement.Pass(elementPath);
            } 
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.ExpandTreeElement.Exception(e);
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

            var locator = CellLocator.of(rowIndexOrContent, column);
            var cell = window.components.findTextCell(locator, session);

            if (cell == null) {
                return new Result.DoubleClickCell.NotFound(locator.location);
            }

            if (options.presenterMode) switch(highlightElement(session, cell)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }

            try {
                cell.doubleClick(session);
                return new Result.DoubleClickCell.Pass(locator.location);
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

        public RobotResult exportWindow(string formName, string directory) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var fileName = toValidFilename(formName);
            var formFields = new List<FormField>();

            var buttons = window.components.getAllButtons();
            var labels = window.components.getAllLabels();
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
                formFields.Add(new FormField(label.text, "", label.id, left, top, width, height));
            });

            textFields.ForEach(textField => {
                var bottom = textField.position.bottom;
                var left = textField.position.left;
                var right = textField.position.right;
                var top = textField.position.top;
                var height = bottom - top;
                var width = right - left;

                var label = textField.hLabel;
                if (label == "") {
                    var closestLabel = textField.findClosestHorizontalComponent(labels.Cast<ILocatable>().ToList()) as SAPLabel;
                    label = closestLabel?.text ?? "";
                }

                formFields.Add(new FormField(textField.text, label, textField.id, left, top, width, height));
            });

            try {
                Directory.CreateDirectory(directory);
                var jsonPath = Path.Combine(directory, $"{fileName}.json");
                JSON.writeFile(jsonPath, JSON.serialize(formFields, typeof(List<FormField>)));
                var pngPath = Path.Combine(directory, $"{fileName}.png");
                saveScreenshot(pngPath);
                return new Result.ExportWindow.Pass(jsonPath, pngPath);
            }
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.ExportWindow.Exception(e);
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

        public RobotResult fillTableCell(string rowIndexOrLabel, string column, string content) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var locator = CellLocator.of(rowIndexOrLabel, column);
            var table = window.components.getFirstTable();

            if (table == null)
                return new Result.FillTableCell.NoTable();

            var cell = findTextCell(table, locator);

            if (cell == null) {
                return new Result.FillTableCell.NotFound(locator.location);
            }

            if (options.presenterMode) switch(highlightElement(session, cell)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }

            try {
                if (cell.isChangeable(session)) {
                    cell.insert(content, session);
                    return new Result.FillTableCell.Pass(locator.location);
                }
                return new Result.FillTableCell.NotChangeable(locator.location);
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
            var textField = window.components.findTextField(theTextField);

            if (textField == null) {
                return new Result.FillTextField.NotFound(theTextField.atLocation);
            }

            if (options.presenterMode) switch(highlightElement(session, textField)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }

            try {
                if (textField.isChangeable(session)) {
                    textField.insert(content, session);
                    return new Result.FillTextField.Pass(theTextField.atLocation);
                }
                return new Result.FillTextField.NotChangeable(theTextField.atLocation);
            }
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.FillTextField.Exception(e);
            }
        }

        public RobotResult highlightButton(string label) {
            switch (updateComponentsIfWindowChanged()) {
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
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var theButton = new ButtonLocator(label);
            var button = window.components.findButton(theButton);

            if (button == null) 
            {
                switch (updateWindow()) {
                    case RobotResult.UIScanFail exceptionError: return exceptionError;
                }
            
                button = window.components.findButton(theButton);
            
                if (button == null) {
                    return new Result.PushButton.NotFound(theButton.atLocation);
                }
            }

            if (options.presenterMode) switch(highlightElement(session, button)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }

            try {
                if (button.isEnabled(session)) {
                    button.push(session);
                }
                else {
                    return new Result.PushButton.NotChangeable(theButton.atLocation);
                }
            } 
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.PushButton.Exception(e);
            }

            // Pushing a Button may result in the window being rerendered,
            // and the properties of some components may change.
            if (!windowChanged()) {
                switch (updateWindow()) {
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

            var locator = CellLocator.of(rowNumberOrButtonLabel, column);
            var button = window.components.findButtonCell(locator, session);

            if (button == null) {
                return new Result.PushButtonCell.NotFound(locator.location);
            }

            if (options.presenterMode) switch(highlightElement(session, button)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }

            try {
                if (button.isEnabled(session))
                {
                    button.push(session);
                    return new Result.PushButtonCell.Pass(locator.location);
                }
                return new Result.PushButtonCell.NotChangeable(locator.location);
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

            return new Result.ReadStatusbar.Json(statusbar.getMessage());
        }

        public RobotResult countTableRows() {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var table = window.components.getFirstTable();

            if (table == null)
                return new Result.CountTableRows.NotFound();

            try {
                int rowCount = table.getNumRows(session);
                return new Result.CountTableRows.Pass(rowCount);
            }
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.CountTableRows.Exception(e);
            }
        }

        public RobotResult readTableCell(string rowNumberOrButtonLabel, string column) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var locator = CellLocator.of(rowNumberOrButtonLabel, column);
            var table = window.components.getFirstTable();

            if (table == null)
                return new Result.ReadTableCell.NoTable();

            var cell = findTextCell(table, locator) as ITextElement ??
                       window.components.findComboBoxCell(locator, session);

            if (cell == null) {
                return new Result.ReadTableCell.NotFound(locator.location);
            }

            if (options.presenterMode) switch(highlightElement(session, cell)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }

            try {
                var text = cell.getText(session);
                return new Result.ReadTableCell.Pass(text, locator.location);
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
                var text = textField.getText(session);
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
                       window.components.findTextField(new TextFieldLocator(content));

            if (text == null) {
                return new Result.ReadText.NotFound(content);
            }

            if (options.presenterMode) switch(highlightElement(session, text)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }

            try {
                return new Result.ReadText.Pass(text.getText(session));
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
                var screenshot = ScreenCapture.saveWindowImage(window.Handle);

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

        public RobotResult scrollTextFieldContents(string direction, string? untilTextField)
        {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var directions = new HashSet<string> {"UP", "DOWN", "BEGIN", "END"};

            if (!directions.Contains(direction))
            {
                return new Result.ScrollTextFieldContents.InvalidDirection();
            }

            var verticalScrollbar = window.components.getVerticalScrollbar();

            if (verticalScrollbar == null) {
                return new Result.ScrollTextFieldContents.NoScrollbar();
            }

            if (untilTextField != null) {
                var textField = window.components.findTextField(new TextFieldLocator(untilTextField));

                if (textField != null) {
                   if (options.presenterMode) switch(highlightElement(session, textField)) {
                        case RobotResult.HighlightFail exceptionError: return exceptionError;
                    }
        
                    return new Result.ScrollTextFieldContents.Pass();
                }
            }

            try
            {
                var scrolled = verticalScrollbar.scroll(session, direction);
                if (scrolled)
                {
                    // Scrolling the window changes the values of some components,
                    // therefore the components have to be scanned to read their current value
                    switch (updateWindow()) {
                        case RobotResult.UIScanFail exceptionError:
                            return exceptionError;
                    }

                    if (untilTextField != null) {
                        return scrollTextFieldContents(direction, untilTextField);
                    }

                    return new Result.ScrollTextFieldContents.Pass();
                }
                else 
                {
                    return new Result.ScrollTextFieldContents.MaximumReached();
                }
            }
            catch (Exception e)
            {
                return new Result.ScrollTextFieldContents.Exception(e);
            }
        }

        public TextCell? findTextCell(ITable table, CellLocator locator)
        {
            var cell = window.components.findTextCell(locator, session);
            if (cell != null)
                return cell;

            bool scrolled = table.scrollOnePage(session);
            if (scrolled)
            {
                // Scrolling the table redraws the screen,
                // therefore the components have to be scanned
                updateWindow();
                return findTextCell(table, locator);
            }

            return null;
        }

        public RobotResult selectCell(string rowIndexOrCellContent, string column) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var table = window.components.getFirstTable();

            if (table == null)
                return new Result.SelectCell.NoTable();

            var locator = CellLocator.of(rowIndexOrCellContent, column);
            
            try {
                var cell = findTextCell(table, locator);
                if (cell == null) {
                    return new Result.SelectCell.NotFound(locator.location);
                }

                if (options.presenterMode) switch(highlightElement(session, cell)) {
                    case RobotResult.HighlightFail exceptionError: return exceptionError;
                }

                cell.select(session);
                return new Result.SelectCell.Pass(locator.location);
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

            var locator = CellLocator.of(rowIndexOrCellContent, column);
            var cell = window.components.findComboBoxCell(locator, session);

            if (cell == null) {
                return new Result.SelectCellValue.NotFound(locator.location);
            }

            if (!cell.contains(entry)) {
                return new Result.SelectCellValue.EntryNotFound(entry, locator.location);
            }

            if (options.presenterMode) switch(highlightElement(session, cell)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }

            try {
                cell.setValue(entry, session);
                var value = cell.getText(session);

                if (value.Equals(entry)) {
                    return new Result.SelectCellValue.Pass(entry, locator.location);
                }

                return new Result.SelectCellValue.EntryNotFound(entry, locator.location);
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
                return new Result.SelectComboBoxEntry.EntryNotFound(entry, theComboBox.atLocation);
            }

            if (options.presenterMode) switch(highlightElement(session, comboBox)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }

            try {
                comboBox.setValue(entry, session);
                return new Result.SelectComboBoxEntry.Pass(entry);
            }
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.SelectComboBoxEntry.Exception(e);
            }
        }

        public RobotResult selectTableRow(string rowIndexOrLabel) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var table = window.components.getFirstTable();

            if (table == null) {
                return new Result.SelectTableRow.NoTable();
            }

            int rowIndex;
            if (Int32.TryParse(rowIndexOrLabel, out rowIndex)) {
                if (rowIndex > table.getNumRows(session)) {
                    return new Result.SelectTableRow.InvalidIndex(rowIndex);
                }
                rowIndex--;
            }
            else {
                var rowLocator = new RowLocator($"= {rowIndexOrLabel}");
                var cell = window.components.findTextCell(rowLocator.locator, session);
                if (cell != null) {
                    rowIndex = cell.rowIndex;
                }
                else {
                    return new Result.SelectTableRow.NotFound(rowIndexOrLabel);
                }
            }

            try {
                table.selectRow(rowIndex, session);
                return new Result.SelectTableRow.Pass(rowIndexOrLabel);
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
        
        public RobotResult selectText(string content) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var text = window.components.findLabel(new LabelLocator(content)) ?? 
                       window.components.findTextField(new TextFieldLocator(content));

            if (text == null) {
                return new Result.SelectText.NotFound(content);
            }

            if (options.presenterMode) switch(highlightElement(session, text)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }

            try {
                text.select(session);
                return new Result.SelectText.Pass(content);
            }
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.SelectText.Exception(e);
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
                if (radioButton.isEnabled(session))
                {
                    radioButton.select(session);
                    return new Result.SelectRadioButton.Pass(theRadioButton.atLocation);
                }
                return new Result.SelectRadioButton.NotChangeable(theRadioButton.atLocation);
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
                if (checkBox.isEnabled(session))
                {
                    checkBox.select(session);
                    return new Result.TickCheckBox.Pass(theCheckBox.atLocation);
                }
                return new Result.TickCheckBox.NotChangeable(theCheckBox.atLocation);
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
                if (checkBox.isEnabled(session))
                {
                    checkBox.deselect(session);
                    return new Result.UntickCheckBox.Pass(theCheckBox.atLocation);
                }
                return new Result.UntickCheckBox.NotChangeable(theCheckBox.atLocation);
            }
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.UntickCheckBox.Exception(e);
            }
        }

        public RobotResult tickCheckBoxCell(string rowIndexOrLabel, string column) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var locator = CellLocator.of(rowIndexOrLabel, column);
            var checkBox = window.components.findCheckBoxCell(locator, session);
            
            if (checkBox == null) {
                return new Result.TickCheckBoxCell.NotFound(locator.location);
            }

            if (options.presenterMode) switch(highlightElement(session, checkBox)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }

            try {
                if (checkBox.isEnabled(session))
                {
                    checkBox.select(session);
                    return new Result.TickCheckBoxCell.Pass(locator.location);
                }
                return new Result.TickCheckBoxCell.NotChangeable(locator.location);
            }
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.TickCheckBoxCell.Exception(e);
            }
        }

        public RobotResult untickCheckBoxCell(string rowIndexOrLabel, string column) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var locator = CellLocator.of(rowIndexOrLabel, column);
            var checkBox = window.components.findCheckBoxCell(locator, session);
            
            if (checkBox == null) {
                return new Result.UntickCheckBoxCell.NotFound(locator.location);
            }

            if (options.presenterMode) switch(highlightElement(session, checkBox)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }

            try {
                if (checkBox.isEnabled(session))
                {
                    checkBox.deselect(session);
                    return new Result.UntickCheckBoxCell.Pass(locator.location);
                }
                return new Result.UntickCheckBoxCell.NotChangeable(locator.location);
            }
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.UntickCheckBoxCell.Exception(e);
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
