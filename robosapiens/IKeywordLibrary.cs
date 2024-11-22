namespace RoboSAPiens 
{
    public interface IKeywordLibrary
    {
        public RobotResult ActivateTab(string tab);
        public RobotResult SelectTreeElement(string elementPath);
        public RobotResult DoubleClickTreeElement(string elementPath);
        public RobotResult SelectMenuItem(string itemPath);
        public RobotResult OpenSap(string path, string? sapArgs=null);
        public RobotResult CloseConnection();
        public RobotResult CloseSap();
        public RobotResult CloseWindow();
        public RobotResult ExportTree(string filepath);
        public RobotResult AttachToRunningSap(int sessionNumber=1);
        public RobotResult ConnectToServer(string server);
        public RobotResult DoubleClickCell(string row_locator, string column);
        public RobotResult DoubleClickTextField(string locator);
        public RobotResult ExecuteTransaction(string T_Code);
        public RobotResult ExportWindow(string name, string directory);
        public RobotResult FillCell(string row_locator, string column, string content);
        public RobotResult FillTextField(string locator, string content);
        public RobotResult FillTextEdit(string content);
        public RobotResult ReadCheckBox(string locator);
        public RobotResult PushButton(string button);
        public RobotResult HighlightButton(string button);
        public RobotResult ReadStatusbar();
        public RobotResult PushButtonCell(string row_locator, string column);
        public RobotResult SelectTableRow(string row_locator);
        public RobotResult CountTableRows();
        public RobotResult PressKeyCombination(string keyCombination);
        public RobotResult ReadTextField(string locator);
        public RobotResult ReadText(string locator);
        public RobotResult ReadCell(string row_locator, string column);
        public RobotResult SaveScreenshot(string filepath);
        public RobotResult ScrollTextFieldContents(string direction, string? untilTextField=null);
        public RobotResult ScrollWindowHorizontally(string direction);
        public RobotResult SelectCell(string row_locator, string column);
        public RobotResult SelectCellValue(string row_locator, string column, string entry);
        public RobotResult SelectTreeElementMenuEntry(string elementPath, string menuEntry);
        public RobotResult ReadComboBoxEntry(string comboBox);
        public RobotResult SelectComboBoxEntry(string comboBox, string entry);
        public RobotResult SelectRadioButton(string locator);
        public RobotResult SelectTextField(string locator);
        public RobotResult SelectText(string locator);
        public RobotResult TickCheckBox(string locator);
        public RobotResult UntickCheckBox(string locator);
        public RobotResult TickCheckBoxCell(string row_locator, string column);
        public RobotResult UntickCheckBoxCell(string row_locator, string column);
        public RobotResult GetWindowTitle();
        public RobotResult GetWindowText();
    }
}