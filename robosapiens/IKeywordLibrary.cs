namespace RoboSAPiens 
{
    public interface IKeywordLibrary
    {
        public RobotResult ActivateTab(string tab);
        public RobotResult CloseConnection();
        public RobotResult CloseSap();
        public RobotResult CloseWindow();
        public RobotResult ConnectToRunningSap(int sessionNumber=1, string? connectionName=null, string? client=null);
        public RobotResult ConnectToServer(string server);
        public RobotResult CountTableRows(int tableNumber=1);
        public RobotResult DoubleClickCell(string row_locator, string column, int? tableNumber=null);
        public RobotResult DoubleClickTextField(string locator);
        public RobotResult DoubleClickTreeElement(string elementPath);
        public RobotResult ExecuteTransaction(string T_Code);
        public RobotResult ExpandTreeFolder(string folderPath);
        public RobotResult ExportTree(string filepath);
        public RobotResult ExportWindow(string name, string directory);
        public RobotResult FillCell(string row_locator, string column, string content, int? tableNumber=null);
        public RobotResult FillTextEdit(string content);
        public RobotResult FillTextField(string locator, string content, bool exact=true);
        public RobotResult GetWindowText();
        public RobotResult GetWindowTitle();
        public RobotResult HighlightButton(string button, bool exact=false);
        public RobotResult MaximizeWindow();
        public RobotResult OpenSap(string path, string? sapArgs = null);
        public RobotResult PressKeyCombination(string keyCombination);
        public RobotResult PushButton(string button, bool exact=false);
        public RobotResult PushButtonCell(string row_locator, string column, int? tableNumber=null);
        public RobotResult ReadCheckBox(string locator);
        public RobotResult ReadCell(string row_locator, string column, int? tableNumber=null);
        public RobotResult ReadComboBoxEntry(string comboBox);
        public RobotResult ReadStatusbar();
        public RobotResult ReadText(string locator);
        public RobotResult ReadTextField(string locator);
        public RobotResult SaveScreenshot(string filepath);
        public RobotResult ScrollTextFieldContents(string direction, string? untilTextField=null);
        public RobotResult ScrollWindowHorizontally(string direction);
        public RobotResult SelectCell(string row_locator, string column, int? tableNumber=null);
        public RobotResult SelectCellValue(string row_locator, string column, string entry, int? tableNumber=null);
        public RobotResult SelectTableColumn(string column, int tableNumber=1);
        public RobotResult SelectComboBoxEntry(string comboBox, string entry);
        public RobotResult SelectMenuItem(string itemPath);
        public RobotResult SelectRadioButton(string locator);
        public RobotResult SelectTableRow(string row_locator, int tableNumber=1);
        public RobotResult SelectText(string locator);
        public RobotResult SelectTextField(string locator);
        public RobotResult SelectTreeElement(string elementPath);
        public RobotResult SelectTreeElementMenuEntry(string elementPath, string menuEntry);
        public RobotResult TickCheckBox(string locator);
        public RobotResult TickCheckBoxCell(string row_locator, string column, int? tableNumber=null);
        public RobotResult UntickCheckBox(string locator);
        public RobotResult UntickCheckBoxCell(string row_locator, string column, int? tableNumber=null);
    }
}