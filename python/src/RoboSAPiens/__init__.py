from robot.api.deco import keyword
from RoboSAPiens.client import RoboSAPiensClient

class RoboSAPiens(RoboSAPiensClient):
    """
    RoboSAPiens: SAP GUI-Automation for Humans
    
    In order to use this library the following requirements must be satisfied:
    
    - Scripting on the SAP Server must be [https://help.sap.com/saphelp_aii710/helpdata/en/ba/b8710932b8c64a9e8acf5b6f65e740/content.htm|activated].
    
    - Scripting Support must be [https://help.sap.com/docs/sap_gui_for_windows/63bd20104af84112973ad59590645513/7ddb7c9c4a4c43219a65eee4ca8db001.html?locale=en-US|activated] in the SAP GUI.
    
    This library is also available in the following languages:
    - RoboSAPiens.DE (German)
    """
    
    def __init__(self, presenter_mode: bool=False):
        """
        *presenter_mode*: Highlight each GUI element acted upon
        """
        
        args = {
            'presenter_mode': presenter_mode,
        }
        
        super().__init__(args)
    

    @keyword('Select Tab') # type: ignore
    def activate_tab(self, tab_name: str): # type: ignore
        """
        Select the tab with the name provided.
        
        | ``Select Tab    tab_name``
        """
        
        args = [tab_name]
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The tab '{0}' could not be found Hint: Check the spelling",
            "SapError": "SAP Error: {0}",
            "Pass": "The tab {0} was selected",
            "Exception": "The tab could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ActivateTab', args, result) # type: ignore
    

    @keyword('Open SAP') # type: ignore
    def open_sap(self, path: str): # type: ignore
        """
        Open the SAP GUI. The standard path is
        
        | ``C:\\Program Files (x86)\\SAP\\FrontEnd\\SAPgui\\saplogon.exe``
        """
        
        args = [path]
        
        result = {
            "Pass": "The SAP GUI was opened.",
            "SAPNotStarted": "The SAP GUI could not be opened. Verify that the path is correct.",
            "Exception": "The SAP GUI could not be opened. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('OpenSap', args, result) # type: ignore
    

    @keyword('Disconnect from Server') # type: ignore
    def close_connection(self): # type: ignore
        """
        Terminate the connection to the SAP server.
        """
        
        args = []
        
        result = {
            "NoSapGui": "No open SAP GUI found. Call the keyword \"Open SAP\" first.",
            "NoGuiScripting": "The scripting support is not activated. It must be activated in the Settings of SAP Logon.",
            "NoConnection": "No existing connection to an SAP server. Call the keyword \"Connect to Server\" first.",
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "Pass": "Disconnected from the server.",
            "Exception": "Could not disconnect from the server. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('CloseConnection', args, result) # type: ignore
    

    @keyword('Close SAP') # type: ignore
    def close_sap(self): # type: ignore
        """
        Close the SAP GUI
        """
        
        args = []
        
        result = {
            "NoSapGui": "No open SAP GUI found. Call the keyword \"Open SAP\" first.",
            "Pass": "The SAP GUI was closed."
        }
        return super()._run_keyword('CloseSap', args, result) # type: ignore
    

    @keyword('Export Spreadsheet') # type: ignore
    def export_spreadsheet(self, table_index: str): # type: ignore
        """
        The export function 'Spreadsheet' will be executed for the specified table, if available.
        
        | ``Export spreadsheet    table_index``
        
        table_index: 1, 2,...
        """
        
        args = [table_index]
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "Exception": "The export function 'Spreadsheet' could not be called\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html",
            "NotFound": "No table was found that supports the export function 'Spreadsheet'",
            "Pass": "The export function 'Spreadsheet' was successfully called on the table with index {0}."
        }
        return super()._run_keyword('ExportSpreadsheet', args, result) # type: ignore
    

    @keyword('Export Function Tree') # type: ignore
    def export_tree(self, filepath: str): # type: ignore
        """
        Export the function tree in JSON format to the file provided.
        
        | ``Export Function Tree     filepath``
        """
        
        args = [filepath]
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The window contains no tree element",
            "Pass": "The function tree was exported to {0}",
            "Exception": "The function tree could not be exported. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ExportTree', args, result) # type: ignore
    

    @keyword('Connect to Running SAP') # type: ignore
    def attach_to_running_sap(self): # type: ignore
        """
        Connect to a running SAP instance and take control of it.
        """
        
        args = []
        
        result = {
            "NoSapGui": "No open SAP GUI found. Call the keyword \"Open SAP\" first.",
            "NoGuiScripting": "The scripting support is not activated. It must be activated in the Settings of SAP Logon.",
            "NoConnection": "No existing connection to an SAP server. Call the keyword \"Connect to Server\" first.",
            "NoServerScripting": "Scripting is not activated on the server side. Please consult the documentation of RoboSAPiens.",
            "Pass": "Connected to a running SAP instance.",
            "Exception": "Could not connect to a running SAP instance. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('AttachToRunningSap', args, result) # type: ignore
    

    @keyword('Connect to Server') # type: ignore
    def connect_to_server(self, server_name: str): # type: ignore
        """
        Connect to the SAP Server provided.
        
        | ``Connect to Server    servername``
        """
        
        args = [server_name]
        
        result = {
            "NoSapGui": "No open SAP GUI found. Call the keyword \"Open SAP\" first.",
            "NoGuiScripting": "The scripting support is not activated. It must be activated in the Settings of SAP Logon.",
            "Pass": "Connected to the server {0}",
            "SapError": "SAP Error: {0}",
            "NoServerScripting": "Scripting is not activated on the server side. Please consult the documentation of RoboSAPiens.",
            "Exception": "Could not establish the connection. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ConnectToServer', args, result) # type: ignore
    

    @keyword('Double-click Cell') # type: ignore
    def double_click_cell(self, row_locator: str, column: str): # type: ignore
        """
        Double-click the cell at the intersection of the row and the column provided.
        
        | ``Double-click Cell     row_locator     column``
        
        row_locator: either the row number or the content of a cell in the row.
        """
        
        args = [row_locator, column]
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The cell with the locator '{0}, {1}' could not be found. Hint: Check the spelling",
            "Pass": "The cell with the locator '{0}, {1}' was double-clicked.",
            "Exception": "The cell could not be double-clicked. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('DoubleClickCell', args, result) # type: ignore
    

    @keyword('Double-click Text Field') # type: ignore
    def double_click_text_field(self, content: str): # type: ignore
        """
        Double click the text field with the content provided.
        
        | ``Double-click Text Field     Content``
        """
        
        args = [content]
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The text field with the content '{0}' could not be found Hint: Check the spelling",
            "Pass": "The text field with the content '{0}' was double-clicked.",
            "Exception": "The text field could not be double-clicked. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('DoubleClickTextField', args, result) # type: ignore
    

    @keyword('Execute Transaction') # type: ignore
    def execute_transaction(self, T_Code: str): # type: ignore
        """
        Execute the transaction with the given T-Code.
        
        | ``Execute Transaction    T_Code``
        """
        
        args = [T_Code]
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "Pass": "The transaction with T-Code {0} was executed.",
            "Exception": "The transaction could not be executed. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ExecuteTransaction', args, result) # type: ignore
    

    @keyword('Export Dynpro') # type: ignore
    def export_form(self, name: str, directory: str): # type: ignore
        """
        Write all texts in the Dynpro to a JSON file. Also a screenshot will be saved in PNG format.
        
        | ``Export Dynpro     name     directory``
        
        directory: Absolute path to the directory where the files will be saved.
        """
        
        args = [name, directory]
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "Pass": "The Dynpro was exported to the JSON file {0} and the PNG image {1}",
            "Exception": "The Dynpro could not be exported. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ExportForm', args, result) # type: ignore
    

    @keyword('Fill Cell') # type: ignore
    def fill_table_cell(self, row_locator: str, column_content: str): # type: ignore
        """
        Fill the cell at the intersection of the row and the column specified with the content provided.
        
        | ``Fill Cell     row     column = content``
        
        row: either the row number or the contents of a cell in the row.
        
        *Hint*: Some cells can be filled using the keyword 'Fill Text Field' providing as locator the description obtained by selecting the cell and pressing F1.
        """
        
        args = [row_locator, column_content]
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "InvalidFormat": "The format of the second parameter must be 'column = content'",
            "NotFound": "The cell with the locator '{0}, {1}' could not be found Hint: Check the spelling",
            "NotChangeable": "The cell with the locator '{0}, {1}' is not changeable.",
            "Pass": "The cell with the locator '{0}, {1}' was filled.",
            "Exception": "The cell could not be filled. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('FillTableCell', args, result) # type: ignore
    

    @keyword('Fill Text Field') # type: ignore
    def fill_text_field(self, locator: str, content: str): # type: ignore
        """
        Fill the text Field specified by the locator with the content provided.
        
        *Text field with a label to its left*
        | ``Fill Text Field    label    content``
        
        *Text field with a label above*
        | ``Fill Text Field    @ label    content``
        
        *Text field at the intersection of a label to its left and a label above it (including a heading)*
        | ``Fill Text Field    label to its left @ label above it    content``
        
        *Text field without label below a text field with a label (e.g. an address line)*
        | ``Fill Text Field    position (1,2,..) @ label    content``
        
        *Text field without a label to the right of a text field with a label*
        | ``Fill Text Field    label @ position (1,2,..)    content``
        
        *Text field with a non-unique label to the right of a text field with a label*
        | ``Fill Text Field    left label >> right label    content``
        
        *Hint*: The description obtained by selecting a text field and pressing F1 can also be used as label.
        """
        
        args = [locator, content]
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The text field with the locator '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The text field with the locator '{0}' was filled.",
            "Exception": "The text field could not be filled. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('FillTextField', args, result) # type: ignore
    

    @keyword('Highlight Button') # type: ignore
    def highlight_button(self, name_or_tooltip: str): # type: ignore
        """
        Highlight the button with the given name or tooltip.
        
        | ``Highlight Button    name or tooltip``
        """
        
        args = [name_or_tooltip]
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The button '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The button '{0}' was highlighted.",
            "Exception": "The button could not be highlighted. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('HighlightButton', args, result) # type: ignore
    

    @keyword('Push Button') # type: ignore
    def push_button(self, name_or_tooltip: str): # type: ignore
        """
        Push the button with the given name or tooltip.
        
        | ``Push Button    name or tooltip``
        """
        
        args = [name_or_tooltip]
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "SapError": "SAP Error: {0}",
            "NotFound": "The button '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The button '{0}' was pushed.",
            "Exception": "The button could not be pushed. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('PushButton', args, result) # type: ignore
    

    @keyword('Push Button Cell') # type: ignore
    def push_button_cell(self, row_or_label: str, column: str): # type: ignore
        """
        Push the button cell located at the intersection of the row and column provided.
        
        | ``Push Button Cell     row_locator     column``
        
        row_locator: Row number, label or tooltip.
        """
        
        args = [row_or_label, column]
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The button cell with the locator '{0}, {1}' could not be found. Hint: Check the spelling",
            "Pass": "The button cell with the locator '{0}' was pushed.",
            "Exception": "The button cell could not be pushed. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('PushButtonCell', args, result) # type: ignore
    

    @keyword('Read Statusbar') # type: ignore
    def read_statusbar(self): # type: ignore
        """
        Read the message in the statusbar.
        
        | ``Read Statusbar``
        """
        
        args = []
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "No statusbar was found.",
            "Pass": "The statusbar was read.",
            "Exception": "The statusbar could not be read\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ReadStatusbar', args, result) # type: ignore
    

    @keyword('Read Text Field') # type: ignore
    def read_text_field(self, locator: str): # type: ignore
        """
        Read the contents of the text field specified by the locator.
        
        *Text field with a label to its left*
        | ``Read Text Field    label``
        
        *Text field with a label above it*
        | ``Read Text Field    @ label``
        
        *Text field at the intersection of a label to its left and a label above it*
        | ``Read Text Field    left label @ label above``
        
        *Text field whose content starts with a given text*
        | ``Read Text Field    = text``
        """
        
        args = [locator]
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The text field with the locator '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The text field with the locator '{0}' was read.",
            "Exception": "The text field could not be read. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ReadTextField', args, result) # type: ignore
    

    @keyword('Read Text') # type: ignore
    def read_text(self, locator: str): # type: ignore
        """
        Read the text specified by the locator.
        
        *Text starting with a given substring*
        | ``Read Text    = substring``
        
        *Text following a label*
        | ``Read Text    Label``
        """
        
        args = [locator]
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "No text with the locator '{0}' was found. Hint: Check the spelling",
            "Pass": "A text with the locator '{0}' was read.",
            "Exception": "The text could not be read. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ReadText', args, result) # type: ignore
    

    @keyword('Read Cell') # type: ignore
    def read_table_cell(self, row_locator: str, column: str): # type: ignore
        """
        Read the contents of the cell at the intersection of the row and column provided.
        
        | ``Read Cell     row_locator     column``
        
        row_locator: either the row number or the contents of a cell in the row.
        """
        
        args = [row_locator, column]
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The cell with the locator '{0}, {1}' could not be found. Hint: Check the spelling",
            "Pass": "The cell with the locator '{0}, {1}' was read.",
            "Exception": "The cell could not be read. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ReadTableCell', args, result) # type: ignore
    

    @keyword('Save Screenshot') # type: ignore
    def save_screenshot(self, filepath: str): # type: ignore
        """
        Save a screenshot of the current window in the file provided.
        
        | ``Save Screenshot     filepath``
        
        filepath: Absolute path to a .png file.
        """
        
        args = [filepath]
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "InvalidPath": "The path '{0}' is invalid.",
            "UNCPath": "UNC paths (i.e. beginning with \\\\) are not allowed",
            "NoAbsPath": "The path '{0}' is not an absolute path.",
            "Pass": "The screenshot was saved in {0}.",
            "Exception": "The screenshot could not be saved. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SaveScreenshot', args, result) # type: ignore
    

    @keyword('Select Cell') # type: ignore
    def select_cell(self, row_locator: str, column: str): # type: ignore
        """
        Select the cell at the intersection of the row and column provided.
        
        | ``Select Cell     row_locator     column``
        
        row_locator: either the row number or the contents of a cell in the row.
        """
        
        args = [row_locator, column]
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The cell with the locator '{0}, {1}' could not be found. Hint: Check the spelling",
            "Pass": "The cell with the locator '{0}, {1}' was selected.",
            "Exception": "The cell could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SelectCell', args, result) # type: ignore
    

    @keyword('Select Dropdown Menu Entry') # type: ignore
    def select_combo_box_entry(self, dropdown_menu: str, entry: str): # type: ignore
        """
        Select the specified entry from the dropdown menu provided.
        
        | ``Select Dropdown Menu Entry   dropdown menu    entry``
        """
        
        args = [dropdown_menu, entry]
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The dropdown menu '{0}' could not be found. Hint: Check the spelling",
            "EntryNotFound": "In the dropdown menu '{0}' no entry '{1}' could be found. Hint: Check the spelling",
            "Pass": "In the dropdown menu '{0}' the entry '{1}' was selected.",
            "Exception": "The entry could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SelectComboBoxEntry', args, result) # type: ignore
    

    @keyword('Select Radio Button') # type: ignore
    def select_radio_button(self, locator: str): # type: ignore
        """
        Select the radio button specified by the locator.
        
        *Radio button with a label to its left or its right*
        | ``Select Radio Button    label``
        
        *Radio button with a label above it*
        | ``Select Radio Button    @ label``
        
        *Radio button at the intersection of a label to its left or its right and a label above it*
        | ``Select Radio Button    left or right label @ label above``
        """
        
        args = [locator]
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The radio button with locator '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The radio button with locator '{0}' was selected.",
            "Exception": "The radio button could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SelectRadioButton', args, result) # type: ignore
    

    @keyword('Select Table Row') # type: ignore
    def select_table_row(self, row_number: str): # type: ignore
        """
        Select the specified table row.
        
        | ``Select Table Row    row_number``
        """
        
        args = [row_number]
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "Exception": "The row with index '{0}' could not be selected\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html",
            "NotFound": "The table contains no row with index '{0}'",
            "Pass": "The row with index '{0}' was selected"
        }
        return super()._run_keyword('SelectTableRow', args, result) # type: ignore
    

    @keyword('Select Text Field') # type: ignore
    def select_text_field(self, locator: str): # type: ignore
        """
        Select the text field specified by the locator.
        
        *Text field with a label to its left*
        | ``Select Text Field    label``
        
        *Text field with a label above it*
        | ``Select Text Field    @ label``
        
        *Text field at the intersection of a label to its left and a label above it*
        | ``Select Text Field    left label @ label above``
        
        *Text field whose content starts with the given text*
        | ``Select Text Field    = text``
        """
        
        args = [locator]
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The text field with the locator '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The text field with the locator '{0}' was selected.",
            "Exception": "The text field could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SelectTextField', args, result) # type: ignore
    

    @keyword('Select Text Line') # type: ignore
    def select_text_line(self, content: str): # type: ignore
        """
        Select the text line starting with the given content.
        
        | ``Select Text Line    content``
        """
        
        args = [content]
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The text line starting with '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The text line starting with '{0}' was selected.",
            "Exception": "The text line could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SelectTextLine', args, result) # type: ignore
    

    @keyword('Tick Checkbox') # type: ignore
    def tick_check_box(self, locator: str): # type: ignore
        """
        Tick the checkbox specified by the locator.
        
        *Checkbox with a label to its left or its right*
        | ``Tick Checkbox    label``
        
        *Checkbox with a label above it*
        | ``Tick Checkbox    @ label``
        
        *Checkbox at the intersection of a label to its left and a label above it*
        | ``Tick Checkbox    left label @ label above``
        """
        
        args = [locator]
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The checkbox with the locator '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The checkbox with the locator '{0}' was ticked.",
            "Exception": "The checkbox could not be ticked. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('TickCheckBox', args, result) # type: ignore
    

    @keyword('Untick Checkbox') # type: ignore
    def untick_check_box(self, locator: str): # type: ignore
        """
        Untick the checkbox specified by the locator.
        
        *Checkbox with a label to its left or its right*
        | ``Untick Checkbox    label``
        
        *Checkbox with a label above it*
        | ``Untick Checkbox    @ label``
        
        *Checkbox at the intersection of a label to its left and a label above it*
        | ``Untick Checkbox    left label @ label above``
        """
        
        args = [locator]
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The checkbox with the locator '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The checkbox with the locator '{0}' was unticked.",
            "Exception": "The checkbox could not be unticked. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('UntickCheckBox', args, result) # type: ignore
    

    @keyword('Tick Checkbox Cell') # type: ignore
    def tick_check_box_cell(self, row_number: str, column: str): # type: ignore
        """
        Tick the checkbox cell at the intersection of the row and the column provided.
        
        | ``Tick Checkbox Cell     row number     column``
        """
        
        args = [row_number, column]
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The checkbox cell with the locator '{0}, {1}' coud not be found. Hint: Check the spelling",
            "Pass": "The checkbox cell with the locator '{0}' was ticked.",
            "Exception": "The checkbox cell could not be ticked. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('TickCheckBoxCell', args, result) # type: ignore
    

    @keyword('Get Window Title') # type: ignore
    def get_window_title(self): # type: ignore
        """
        Get the title of the window in the foreground.
        
        | ``${Title}    Get Window Title``
        """
        
        args = []
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "Pass": "The title of the window was obtained.",
            "Exception": "The window title could not be read.\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('GetWindowTitle', args, result) # type: ignore
    

    @keyword('Get Window Text') # type: ignore
    def get_window_text(self): # type: ignore
        """
        Get the text message of the window in the foreground.
        
        | ``${Text}    Get Window Text``
        """
        
        args = []
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "Pass": "The text message of the window was obtained.",
            "Exception": "The text message of the window could not be read.\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('GetWindowText', args, result) # type: ignore
    
    ROBOT_LIBRARY_SCOPE = 'GLOBAL'
    ROBOT_LIBRARY_VERSION = '1.2.8'