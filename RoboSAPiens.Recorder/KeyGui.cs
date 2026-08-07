namespace RoboSAPiens.Recorder
{
    static class KeyGuiActions
    {
        public const string Check = "check";
        public const string Click = "click";
        public const string Connect = "connect";
        public const string DoubleClick = "double_click";
        public const string Execute = "execute";
        public const string Expand = "expand";
        public const string Fill = "fill";
        public const string PressKey = "press_key";
        public const string Push = "push";
        public const string Select = "select";
        public const string SelectRow = "select_row";
        public const string Uncheck = "uncheck";
    }

    static class KeyGuiRoles
    {
        public const string Button = "button";
        public const string Cell = "cell";
        public const string Checkbox = "checkbox";
        public const string Combobox = "combobox";
        public const string Label = "label";
        public const string MultiLineTextField = "multiline_textfield";
        public const string Radio = "radio";
        public const string Tab = "tab";
        public const string TextField = "textfield";
        public const string TreeElement = "tree_element";
    }
    
    public record KeyGuiEvent(string componentId, long window, string action, string? role, Locator? locator, string? value)
    {
        public override string ToString()
        {
            return $"Action: {action} | Role: {role} | Locator: {locator} | Value: {value}";
        }

        public KeywordCall toKeywordCall(string lang)
        {
            var robosapiens = new Robosapiens(lang);
            return (action, role, value) switch
            {
                (KeyGuiActions.Connect, _, string connection) => robosapiens.ConnectToServer(connection),
                (KeyGuiActions.Connect, _, null) => robosapiens.ConnectToSap(),
                (KeyGuiActions.Check, KeyGuiRoles.Cell, _) => robosapiens.TickCheckBoxCell(locator!),
                (KeyGuiActions.Check, KeyGuiRoles.Checkbox, _) => robosapiens.TickCheckBox(locator!),
                (KeyGuiActions.Click, KeyGuiRoles.Cell, _) => robosapiens.SelectCell(locator!),
                (KeyGuiActions.Click, KeyGuiRoles.Label, _) => robosapiens.SelectText(locator!),
                (KeyGuiActions.Click, KeyGuiRoles.Radio, _) => robosapiens.SelectRadio(locator!),
                (KeyGuiActions.Click, KeyGuiRoles.Tab, _) => robosapiens.SelectTab(locator!),
                (KeyGuiActions.Click, KeyGuiRoles.TextField, _) => robosapiens.SelectTextField(locator!),
                (KeyGuiActions.DoubleClick, KeyGuiRoles.Cell, _) => robosapiens.DoubleClickCell(locator!),
                (KeyGuiActions.DoubleClick, KeyGuiRoles.TextField, _) => robosapiens.DoubleClickTextField(locator!),
                (KeyGuiActions.DoubleClick, KeyGuiRoles.TreeElement, _) => robosapiens.DoubleClickTreeElement(locator!),
                (KeyGuiActions.Execute, _, string tCode) => robosapiens.ExecuteTransaction(tCode),
                (KeyGuiActions.Expand, KeyGuiRoles.TreeElement, _) => robosapiens.ExpandTreeFolder(locator!),
                (KeyGuiActions.Fill, KeyGuiRoles.Cell, string contents) => robosapiens.FillCell(locator!, contents),
                (KeyGuiActions.Fill, KeyGuiRoles.TextField, string contents) => robosapiens.FillTextField(locator!, contents),
                (KeyGuiActions.PressKey, _, string keyCombination) => robosapiens.PressKeyCombination(keyCombination),
                (KeyGuiActions.Push, KeyGuiRoles.Button, _) => robosapiens.PushButton(locator!),
                (KeyGuiActions.Push, KeyGuiRoles.Cell, _) => robosapiens.PushButtonCell(locator!),
                (KeyGuiActions.Select, KeyGuiRoles.Cell, string value) => robosapiens.SelectCellValue(locator!, value),
                (KeyGuiActions.Select, KeyGuiRoles.Combobox, string option) => robosapiens.SelectComboBox(locator!, option),
                (KeyGuiActions.Uncheck, KeyGuiRoles.Cell, _) => robosapiens.UntickCheckBoxCell(locator!),
                (KeyGuiActions.Uncheck, KeyGuiRoles.Checkbox, _) => robosapiens.UntickCheckBox(locator!),
                _ => new KeywordCall("Fail", [new KeywordCallArg("message", $"Unknown Keyword: {action} {role}", type: KeywordCallArgType.ARG)])
            };
        }
    }
}
