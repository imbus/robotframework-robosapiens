using sapfewse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace RoboSAPiens {
    public record SessionInfo(string? server, string? client);
    
    public sealed class NoSAPSession : ISession
    {
        public SessionInfo getSessionInfo()
        {
            return new SessionInfo(null, null);
        }
    }

    public sealed class SAPSession : ISession {
        string sapClient;
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
            this.window = new SAPWindow(session.ActiveWindow, debug: options.debug);
            sapClient = session.Info.Client;
        }

        public SessionInfo getSessionInfo()
        {
            return new SessionInfo(systemName, sapClient);
        }

        bool windowChanged() {
            var currentWindow = session.ActiveWindow;
            
            return window.title != currentWindow.Text || 
                   window.id != currentWindow.Id;
        }

        RobotResult.UIScanFail? updateWindow() {
            try {
                var currentStatusbarId = window.components.getStatusBar()?.id;
                var activeWindow = session.ActiveWindow;
                window = new SAPWindow(activeWindow);

                if (currentStatusbarId != null && activeWindow.Type == "GuiModalWindow") {
                    window.setStatusbar(session, currentStatusbarId);
                }

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

        void highlightCell(Cell cell)
        {
            cell.highlight(session);
            Thread.Sleep(500);
            cell.highlight(session);
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

        public RobotResult doubleClickTreeElement(string elementPath) 
        {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var treeElement = window.components.findTreeElement(elementPath, session);
            
            if (treeElement == null) {
                return new Result.DoubleClickTreeElement.NotFound(elementPath);
            }

            try {
                treeElement.doubleClick(session);

                switch (updateWindow()) {
                    case RobotResult.UIScanFail exceptionError:
                        return exceptionError;
                }
                
                return new Result.DoubleClickTreeElement.Pass(elementPath);
            } 
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.DoubleClickTreeElement.Exception(e);
            }
        }

        public RobotResult expandTreeFolder(string folderPath) 
        {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var treeElement = window.components.findTreeElement(folderPath, session);
            
            if (treeElement == null)
                return new Result.ExpandTreeFolder.NotFound(folderPath);

            try {
                treeElement.expand(session);
                return new Result.ExpandTreeFolder.Pass(folderPath);
            } 
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.ExpandTreeFolder.Exception(e);
            }
        }

        public RobotResult selectTreeElement(string elementPath) 
        {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var treeElement = window.components.findTreeElement(elementPath, session);
            
            if (treeElement == null) {
                return new Result.SelectTreeElement.NotFound(elementPath);
            }

            try {
                treeElement.select(session);
                return new Result.SelectTreeElement.Pass(elementPath);
            } 
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.SelectTreeElement.Exception(e);
            }
        }

        public RobotResult selectTreeElementMenuEntry(string elementPath, string menuEntry)
        {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var treeElement = window.components.findTreeElement(elementPath, session);
            
            if (treeElement == null) {
                return new Result.SelectTreeElementMenuEntry.NotFound(elementPath);
            }

            try {
                treeElement.selectMenuEntry(session, menuEntry);
                return new Result.SelectTreeElementMenuEntry.Pass(menuEntry);
            } 
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.SelectTreeElementMenuEntry.Exception(e);
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

        public RobotResult closeWindow()
        {
            try {
                session.ActiveWindow.Close();
                return new Result.CloseWindow.Pass();
            }
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.CloseWindow.Exception(e);
            }
        }

        public RobotResult doubleClickCell(string rowIndexOrContent, string column, int? tableNumber)
        {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var tables = window.components.getTables();
            if (tableNumber != null && tableNumber > tables.Count)
                return new Result.DoubleClickCell.InvalidTable((int)tableNumber);

            var locator = CellLocator.of(rowIndexOrContent, column);
            var cell = window.components.findCell(locator, session, tableNumber);

            if (cell == null) {
                return new Result.DoubleClickCell.NotFound(locator.location);
            }

            if (options.presenterMode) highlightCell(cell);

            try {
                cell.doubleClick(session);
                // After double-clicking a cell the window contents may change
                switch (updateWindow()) {
                    case RobotResult.UIScanFail exceptionError:
                        return exceptionError;
                }
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

            var tree = window.components.getTrees().FirstOrDefault();

            if (tree == null) return new Result.ExportTree.NotFound();

            try
            {
                var treeNodes = tree.getAllNodes(session);
                JSON.writeFile(filePath, JSON.serialize(treeNodes, typeof(List<TreeNode>)));
                return new Result.ExportTree.Pass(filePath);
            }
            catch (Exception e)
            {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.ExportTree.Exception(e);
            }
        }

        public RobotResult fillCell(string rowIndexOrLabel, string column, string content, int? tableNumber)
        {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var tables = window.components.getTables();
            if (tableNumber != null && tableNumber > tables.Count)
                return new Result.FillCell.InvalidTable((int)tableNumber);

            var locator = CellLocator.of(rowIndexOrLabel, column);
            var cell = window.components.findCell(locator, session, tableNumber);

            if (cell == null) {
                return new Result.FillCell.NotFound(locator.location);
            }

            if (options.presenterMode) highlightCell(cell);

            try {
                if (cell.isChangeable(session))
                {
                    cell.setValue(content, session);
                    return new Result.FillCell.Pass(locator.location);
                }
                return new Result.FillCell.NotChangeable(locator.location);
            }
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.FillCell.Exception(e);
            }
        }

        public RobotResult fillTextEdit(string content) 
        {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var textEdit = window.components.getTextEdit();

            if (textEdit == null) {
                return new Result.FillTextEdit.NotFound();
            }

            if (options.presenterMode) switch(highlightElement(session, textEdit)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }

            try {
                if (textEdit.isChangeable(session)) 
                {
                    textEdit.insert(session, content);
                    return new Result.FillTextEdit.Pass();
                }
                return new Result.FillTextEdit.NotChangeable();
            }
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.FillTextEdit.Exception(e);
            }
        }

        public RobotResult fillTextField(string labels, string content) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var theTextField = new TextFieldLocator(labels);
            var textField = window.components.findTextField(theTextField, changeable: true);

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
                switch(keyCombination)
                {
                    case "PageDown":
                        window.pressPageDown();
                        break;
                    case "F1":
                    case "F4":
                        var gridView = window.components.getGridViews().FirstOrDefault();
                        if (gridView != null) gridView.pressKey(keyCombination, session);
                        else window.pressKey((int)vkey!);
                        break;
                    default:
                        window.pressKey((int)vkey!);
                        break;
                }
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

        public RobotResult pushButtonCell(string rowNumberOrButtonLabel, string column, int? tableNumber)
        {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var tables = window.components.getTables();
            if (tableNumber != null && tableNumber > tables.Count)
                return new Result.PushButtonCell.InvalidTable((int)tableNumber);

            var locator = CellLocator.of(rowNumberOrButtonLabel, column);
            var cell = window.components.findCell(locator, session, tableNumber);

            if (cell == null) {
                return new Result.PushButtonCell.NotFound(locator.location);
            }

            if (options.presenterMode) highlightCell(cell);

            if (!cell.isButtonCell()) {
                return new Result.PushButtonCell.NotAButton(locator.location);
            }

            try {
                if (cell.isChangeable(session))
                {
                    cell.click(session);
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

        public RobotResult countTableRows(int tableNumber)
        {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var tables = window.components.getTables();

            if (tableNumber > tables.Count)
                return new Result.CountTableRows.InvalidTable(tableNumber);

            var table = tables[tableNumber-1];

            try {
                int rowCount = table.getNumRows(session);
                return new Result.CountTableRows.Pass(rowCount);
            }
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.CountTableRows.Exception(e);
            }
        }

        public RobotResult readCell(string rowNumberOrButtonLabel, string column, int? tableNumber)
        {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var tables = window.components.getTables();
            if (tableNumber != null && tableNumber > tables.Count)
                return new Result.ReadCell.InvalidTable((int)tableNumber);

            var locator = CellLocator.of(rowNumberOrButtonLabel, column);
            var cell = window.components.findCell(locator, session, tableNumber);

            if (cell == null) {
                return new Result.ReadCell.NotFound(locator.location);
            }

            if (options.presenterMode) highlightCell(cell);

            try {
                var text = cell.getValue(session);
                return new Result.ReadCell.Pass(text, locator.location);
            }
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.ReadCell.Exception(e);
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

        public RobotResult scrollWindowHorizontally(string direction)
        {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var directions = new HashSet<string> {"LEFT", "RIGHT", "BEGIN", "END"};

            if (!directions.Contains(direction))
            {
                return new Result.ScrollWindowHorizontally.InvalidDirection();
            }

            var horizontalScrollbar = window.components.getHorizontalScrollbar();

            if (horizontalScrollbar == null) {
                return new Result.ScrollWindowHorizontally.NoScrollbar();
            }

            try
            {
                var scrolled = horizontalScrollbar.scroll(session, direction);
                if (scrolled)
                {
                    return new Result.ScrollWindowHorizontally.Pass();
                }
                else 
                {
                    return new Result.ScrollWindowHorizontally.MaximumReached();
                }
            }
            catch (Exception e)
            {
                return new Result.ScrollWindowHorizontally.Exception(e);
            }
        }

        public RobotResult selectCell(string rowIndexOrCellContent, string column, int? tableNumber)
        {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var tables = window.components.getTables();
            if (tableNumber != null && tableNumber > tables.Count)
                return new Result.SelectCell.InvalidTable((int)tableNumber);

            var locator = CellLocator.of(rowIndexOrCellContent, column);
            var cell = window.components.findCell(locator, session, tableNumber);

            if (cell == null) {
                return new Result.SelectCell.NotFound(locator.location);
            }

            if (options.presenterMode) highlightCell(cell);

            try {
                cell.click(session);
                return new Result.SelectCell.Pass(locator.location);
            }
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.SelectCell.Exception(e);
            }
        }

        public RobotResult selectCellValue(string rowIndexOrCellContent, string column, string entry, int? tableNumber)
        {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var tables = window.components.getTables();
            if (tableNumber != null && tableNumber > tables.Count)
                return new Result.SelectCellValue.InvalidTable((int)tableNumber);

            var locator = CellLocator.of(rowIndexOrCellContent, column);
            var cell = window.components.findCell(locator, session, tableNumber);

            if (cell == null) {
                return new Result.SelectCellValue.NotFound(locator.location);
            }

            if (options.presenterMode) highlightCell(cell);

            try {
                cell.setValue(entry, session);
                var value = cell.getValue(session);

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

        public RobotResult readComboBoxEntry(string labels) {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var theComboBox = new ComboBoxLocator(labels);
            var comboBox = window.components.findComboBox(theComboBox);
            
            if (comboBox == null) {
                return new Result.ReadComboBoxEntry.NotFound(theComboBox.atLocation);
            }

            if (options.presenterMode) switch(highlightElement(session, comboBox)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }

            try {
                var entry = comboBox.getText(session);
                return new Result.ReadComboBoxEntry.Pass(entry);
            }
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.ReadComboBoxEntry.Exception(e);
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
                comboBox.select(entry, session);

                switch (updateWindow()) {
                    case RobotResult.UIScanFail exceptionError:
                        return exceptionError;
                }
                
                return new Result.SelectComboBoxEntry.Pass(entry);
            }
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.SelectComboBoxEntry.Exception(e);
            }
        }

        public RobotResult selectTableColumn(string column, int tableNumber)
        {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var tables = window.components.getTables();

            if (tables.Count == 0) {
                return new Result.SelectTableColumn.NoTable();
            }

            if (tableNumber > tables.Count)
                return new Result.SelectTableColumn.InvalidTable(tableNumber);

            var table = tables[tableNumber-1];

            if (!table.hasColumn(column)) {
                return new Result.SelectTableColumn.NotFound(column);
            }

            try {
                table.selectColumn(column, session);
                return new Result.SelectTableColumn.Pass(column);
            }
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.SelectTableColumn.Exception(e);
            }
        }

        public RobotResult selectTableRow(string rowIndexOrLabel, int tableNumber)
        {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var tables = window.components.getTables();

            if (tableNumber > tables.Count)
                return new Result.SelectTableRow.InvalidTable(tableNumber);

            var table = tables[tableNumber-1];

            int rowIndex;
            if (Int32.TryParse(rowIndexOrLabel, out rowIndex)) {
                if (rowIndex > table.getNumRows(session)) {
                    return new Result.SelectTableRow.InvalidIndex(rowIndex);
                }
                rowIndex--;
            }
            else {
                var rowLocator = new RowLocator($"= {rowIndexOrLabel}");
                var cell = table.findCell(rowLocator.locator, session);

                if (cell == null) return new Result.SelectTableRow.NotFound(rowIndexOrLabel);
                rowIndex = cell.rowIndex;
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

        public RobotResult readCheckBox(string locator) 
        {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var theCheckBox = new CheckBoxLocator(locator);
            var checkBox = window.components.findCheckBox(theCheckBox);
            
            if (checkBox == null) {
                return new Result.ReadCheckBox.NotFound(theCheckBox.atLocation);
            }

            if (options.presenterMode) switch(highlightElement(session, checkBox)) {
                case RobotResult.HighlightFail exceptionError: return exceptionError;
            }
            
            return new Result.ReadCheckBox.Pass(theCheckBox.atLocation, checkBox.isSelected(session));
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

        public RobotResult tickCheckBoxCell(string rowIndexOrLabel, string column, int? tableNumber)
        {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var tables = window.components.getTables();
            if (tableNumber != null && tableNumber > tables.Count)
                return new Result.TickCheckBoxCell.InvalidTable((int)tableNumber);

            var locator = CellLocator.of(rowIndexOrLabel, column);
            var cell = window.components.findCell(locator, session, tableNumber);
            
            if (cell == null) {
                return new Result.TickCheckBoxCell.NotFound(locator.location);
            }

            if (options.presenterMode) highlightCell(cell);

            try {
                if (cell.isChangeable(session))
                {
                    cell.click(session);
                    return new Result.TickCheckBoxCell.Pass(locator.location);
                }
                return new Result.TickCheckBoxCell.NotChangeable(locator.location);
            }
            catch (Exception e) {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.TickCheckBoxCell.Exception(e);
            }
        }

        public RobotResult untickCheckBoxCell(string rowIndexOrLabel, string column, int? tableNumber)
        {
            switch (updateComponentsIfWindowChanged()) {
                case RobotResult.UIScanFail exceptionError: return exceptionError;
            }

            var tables = window.components.getTables();
            if (tableNumber != null && tableNumber > tables.Count)
                return new Result.UntickCheckBoxCell.InvalidTable((int)tableNumber);

            var locator = CellLocator.of(rowIndexOrLabel, column);
            var cell = window.components.findCell(locator, session, tableNumber);
            
            if (cell == null) {
                return new Result.UntickCheckBoxCell.NotFound(locator.location);
            }

            if (options.presenterMode) highlightCell(cell);

            try {
                if (cell.isChangeable(session))
                {
                    cell.click(session);
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
