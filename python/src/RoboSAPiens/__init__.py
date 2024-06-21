from robot.api.deco import keyword
from RoboSAPiens.client import RoboSAPiensClient

class RoboSAPiens(RoboSAPiensClient):
    """
    RoboSAPiens: SAP GUI-Automation for Humans
    
    In order to use this library the following requirements must be satisfied:
    
    - Scripting on the SAP Server must be [https://help.sap.com/saphelp_aii710/helpdata/en/ba/b8710932b8c64a9e8acf5b6f65e740/content.htm|activated].
    
    - Scripting Support must be [https://help.sap.com/docs/sap_gui_for_windows/63bd20104af84112973ad59590645513/7ddb7c9c4a4c43219a65eee4ca8db001.html?locale=en-US|activated] in the SAP GUI.
    
    == New features in Version 2.0 ==
    
    - Support for SAP GUI 8.0 64-bit
    - New keyword "Select Tree Element"
    - New keyword "Get Row Count"
    - New keyword "Scroll Contents"
    - New keyword "Select Menu Entry"
    - New keyword "Untick Checkbox Cell"
    
    == Breaking changes with respect to Version 1.0 ==
    
    - The keyword "Fill Cell" takes three positional arguments instead of two
    - The keyword "Read Statusbar" returns a dictionary instead of a string
    - Renamed the keyword "Export Dynpro" to "Export Window"
    - Renamed the keyword "Export Function Tree" to "Export Tree Structure"
    - Renamed the keyword "Select Text Line" to "Select Text"
    - Removed the keyword "Export Spreadsheet"
    
    == Getting started ==
    
    In order to login to a server execute the following:
    
    | Open SAP             C:${/}Program Files (x86)${/}SAP${/}FrontEnd${/}SAPgui${/}saplogon.exe
    | Connect to Server    My Test Server
    | Fill Text Field      User              TESTUSER
    | Fill Text Field      Password          TESTPASSWORD
    | Push Button          Enter
    
    == Dealing with spontaneous pop-up windows ==
    
    When clicking a button it can happen that a dialog window pops up.
    The following keyword can be useful in this situation:
    
    | Click button and close pop-up window
    |   [Arguments]   ${button}   ${title}   ${close button}
    |
    |   Push Button       ${button}
    |   ${window_title}   Get Window Title
    |
    |   IF   $window_title == $title
    |       Log               Pop-up window: ${title}
    |       Save screenshot   LOG
    |       Push button       ${close button}
    |   END
    """
    
    def __init__(self, presenter_mode: bool=False, x64: bool=False):
        """
        *presenter_mode*: Highlight each GUI element acted upon
        
        *x64*: Execute RoboSAPiens 64-bit
        """
        
        args = {
            'presenter_mode': presenter_mode,
            'x64': x64,
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
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The tab '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The tab '{0}' was selected.",
            "Exception": "The tab could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ActivateTab', args, result) # type: ignore
    

    @keyword('Select Tree Element') # type: ignore
    def select_tree_element(self, element_path: str): # type: ignore
        """
        Select the tree element located at the path provided.
        
        | ``Select Tree Element    element_path``
        
        element_path: The path to the element using '/' as separator. e.g. Engineering/Civil Engineering
        """
        
        args = [element_path]
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The tree element '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The tree element '{0}' was selected.",
            "Exception": "The tree element could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SelectTreeElement', args, result) # type: ignore
    

    @keyword('Select Menu Entry in Tree Element') # type: ignore
    def select_tree_element_menu_entry(self, element_path: str, menu_entry: str): # type: ignore
        """
        Select the given entry in the context menu of the tree element located at the path provided.
        
        | ``Select Menu Entry in Tree Element    element_path    menu_entry``
        
        element_path: The path to the element using '/' as separator, e.g. Engineering/Civil Engineering.
        
        menu_entry: The menu entry. For nested menus the path to the entry using '|' as separator, e.g. Create|Business Unit.
        """
        
        args = [element_path, menu_entry]
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The tree element '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The menu entry '{0}' was selected.",
            "Exception": "The menu entry could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SelectTreeElementMenuEntry', args, result) # type: ignore
    

    @keyword('Open SAP') # type: ignore
    def open_sap(self, path: str): # type: ignore
        """
        Open the SAP GUI. 
        
        | ``Open SAP   path``
        
        The standard path is
        
        | ``C:\\Program Files (x86)\\SAP\\FrontEnd\\SAPgui\\saplogon.exe``
        
        *Hint*: Use ${/} as path separator. Otherwise the backslashes must be escaped.
        """
        
        args = [path]
        
        result = {
            "Pass": "The SAP GUI was opened.",
            "NoGuiScripting": "The scripting support is not activated. It must be activated in the Settings of SAP Logon.",
            "SAPAlreadyRunning": "The SAP GUI is already running. It must be closed before calling this keyword.",
            "SAPNotStarted": "The SAP GUI could not be opened. Verify that the path is correct.",
            "Exception": "The SAP GUI could not be opened. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('OpenSap', args, result) # type: ignore
    

    @keyword('Disconnect from Server') # type: ignore
    def close_connection(self): # type: ignore
        """
        Terminate the connection to the SAP server.
        
        | ``Disconnect from server``
        """
        
        args = []
        
        result = {
            "NoSapGui": "No open SAP GUI found. Call the keyword \"Open SAP\" first.",
            "NoGuiScripting": "The scripting support is not activated. It must be activated in the Settings of SAP Logon.",
            "NoConnection": "No existing connection to an SAP server. Call the keyword \"Connect to Server\" first.",
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "Pass": "Disconnected from the server.",
            "Exception": "Could not disconnect from the server. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('CloseConnection', args, result) # type: ignore
    

    @keyword('Close SAP') # type: ignore
    def close_sap(self): # type: ignore
        """
        Close the SAP GUI.
        
        | ``Close SAP``
        
        *Hint*: This keyword only works if SAP GUI was started with the keyword "Open SAP".
        """
        
        args = []
        
        result = {
            "NoSapGui": "No open SAP GUI found. Call the keyword \"Open SAP\" first.",
            "Pass": "The SAP GUI was closed."
        }
        return super()._run_keyword('CloseSap', args, result) # type: ignore
    

    @keyword('Get Row Count') # type: ignore
    def count_table_rows(self): # type: ignore
        """
        Count the rows in a table.
        
        | ``${row_count}   Get Row Count``
        """
        
        args = []
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "Exception": "Could not count the rows in the table.\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html",
            "NotFound": "The window contains no table.",
            "Pass": "Counted the number of rows in the table."
        }
        return super()._run_keyword('CountTableRows', args, result) # type: ignore
    

    @keyword('Export Tree Structure') # type: ignore
    def export_tree(self, filepath: str): # type: ignore
        """
        Export the tree structure in the current window to the file provided.
        
        | ``Export Tree Structure     filepath``
        
        filepath: Absolute path to a file with extension .json
        
        *Hint*: Use ${/} as path separator. Otherwise the backslashes must be escaped.
        """
        
        args = [filepath]
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The window contains no tree structure",
            "Pass": "The tree structure was exported to the file '{0}'",
            "Exception": "The tree structure could not be exported. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ExportTree', args, result) # type: ignore
    

    @keyword('Connect to Running SAP') # type: ignore
    def attach_to_running_sap(self, session_number: str='1'): # type: ignore
        """
        Connect to a running SAP instance and take control of it. By default the session number 1 will be used. 
        To use a different session specify the session number.
        
        | ``Connect to Running SAP    session_number``
        """
        
        args = [session_number]
        
        result = {
            "NoSapGui": "No open SAP GUI found. Call the keyword \"Open SAP\" first.",
            "NoGuiScripting": "The scripting support is not activated. It must be activated in the Settings of SAP Logon.",
            "NoConnection": "No existing connection to an SAP server. Call the keyword \"Connect to Server\" first.",
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NoServerScripting": "Scripting is not activated on the server side. Please consult the documentation of RoboSAPiens.",
            "InvalidSessionId": "There is no session number {0}",
            "Pass": "Connected to a running SAP instance.",
            "Exception": "Could not connect to a running SAP instance. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('AttachToRunningSap', args, result) # type: ignore
    

    @keyword('Connect to Server') # type: ignore
    def connect_to_server(self, server_name: str): # type: ignore
        """
        Connect to the SAP Server provided.
        
        | ``Connect to Server    server_name``
        
        server_name: The name of the server in SAP Logon (not the SID).
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
        
        row_locator: either the row number or the contents of a cell in the row. If the cell only contains a number, it must be enclosed in double quotation marks.
        """
        
        args = [row_locator, column]
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The cell with the locator '{0}, {1}' could not be found. Hints: Check the spelling, maximize the SAP window",
            "Pass": "The cell with the locator '{0}, {1}' was double-clicked.",
            "Exception": "The cell could not be double-clicked. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('DoubleClickCell', args, result) # type: ignore
    

    @keyword('Double-click Text Field') # type: ignore
    def double_click_text_field(self, locator: str): # type: ignore
        """
        Double click the text field specified by the locator.
        
        | ``Double-click Text Field     locator``
        
        Text field locators are documented in the keyword Fill Text Field.
        """
        
        args = [locator]
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The text field with the locator '{0}' could not be found Hint: Check the spelling",
            "Pass": "The text field with the locator '{0}' was double-clicked.",
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
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "Pass": "The transaction with T-Code {0} was executed.",
            "Exception": "The transaction could not be executed. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ExecuteTransaction', args, result) # type: ignore
    

    @keyword('Export Window') # type: ignore
    def export_window(self, name: str, directory: str): # type: ignore
        """
        Export the window contents to a JSON file. Also a screenshot will be saved in PNG format.
        
        | ``Export Window     name     directory``
        
        directory: Absolute path to the directory where the files will be saved.
        
        *Hint*: Use ${/} as path separator. Otherwise the backslashes must be escaped.
        
        *Note*: Currently not all GUI elements are exported.
        """
        
        args = [name, directory]
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "Pass": "The window contents were exported to {0} and a screenshot was saved to {1}.",
            "Exception": "The window contents could not be exported. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ExportWindow', args, result) # type: ignore
    

    @keyword('Fill Cell') # type: ignore
    def fill_table_cell(self, row_locator: str, column: str, content: str): # type: ignore
        """
        Fill the cell at the intersection of the row and column with the content provided.
        
        | ``Fill Cell    row_locator    column   content``
        
        row_locator: either the row number or the contents of a cell in the row. If the cell only contains a number, it must be enclosed in double quotation marks.
        
        *Hint*: To migrate from the old keyword with two arguments perform a search and replace with a regular expression.
        """
        
        args = [row_locator, column, content]
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The cell with the locator '{0}, {1}' could not be found Hints: Check the spelling, maximize the SAP window",
            "NotChangeable": "The cell with the locator '{0}, {1}' is not editable.",
            "NoTable": "The window contains no table.",
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
        
        *Hint*: The help text obtained by selecting the text field and pressing F1 can usually be used as label.
        
        *Text field with a label above*
        | ``Fill Text Field    @ label    content``
        
        *Text field at the intersection of a label to its left and a label above it (including a heading)*
        | ``Fill Text Field    label to its left @ label above it    content``
        
        *Text field in a vertical grid below a label*
        | ``Fill Text Field    position (1,2,..) @ label    content``
        
        *Text field in a horizontal grid following a label*
        | ``Fill Text Field    label @ position (1,2,..)    content``
        
        *Text field with a non-unique label to the right of a text field with a label*
        | ``Fill Text Field    left label >> right label    content``
        
        *Text field without a label to the right of a text field with a label*
        | ``Fill Text Field    label >> F1 help text    content``
        
        As a last resort the name obtained using [https://tracker.stschnell.de/|Scripting Tracker] can be used:
        | ``Fill Text Field    name    content``
        """
        
        args = [locator, content]
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The text field with the locator '{0}' could not be found. Hint: Check the spelling",
            "NotChangeable": "The text field with the locator '{0}' is not editable.",
            "Pass": "The text field with the locator '{0}' was filled.",
            "Exception": "The text field could not be filled. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('FillTextField', args, result) # type: ignore
    

    @keyword('Highlight Button') # type: ignore
    def highlight_button(self, locator: str): # type: ignore
        """
        Highlight the button with the given locator.
        
        | ``Highlight Button    locator``
        
        locator: name or tooltip.
        
        *Hint*: Some tooltips consist of a name followed by several spaces and a keyboard shortcut.
        The name may be used as locator as long as it is unique.
        When using the full tooltip text, remember to escape the spaces (e.g. ``Back \\ \\ (F3)``).
        """
        
        args = [locator]
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The button '{0}' could not be found. Hints: Check the spelling, maximize the SAP window",
            "Pass": "The button '{0}' was highlighted.",
            "Exception": "The button could not be highlighted. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('HighlightButton', args, result) # type: ignore
    

    @keyword('Press Key Combination') # type: ignore
    def press_key_combination(self, key_combination: str): # type: ignore
        """
        Press the given key combination.
        
        | ``Press Key Combination    key_combination``
        
        For a full list of supported key combinations consult the [https://help.sap.com/docs/sap_gui_for_windows/b47d018c3b9b45e897faf66a6c0885a8/71d8c95e9c7947ffa197523a232d8143.html?version=770.01&locale=en-US|documentation of SAP GUI].
        
        *Hint*: Pressing F2 is equivalent to a double-click.
        """
        
        args = [key_combination]
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The key combination '{0}' is not supported. See the keyword documentation for valid key combinations.",
            "Pass": "The key combination '{0}' was pressed.",
            "Exception": "The key combination '{0}' could not be pressed.\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('PressKeyCombination', args, result) # type: ignore
    

    @keyword('Push Button') # type: ignore
    def push_button(self, locator: str): # type: ignore
        """
        Push the button with the given locator.
        
        | ``Push Button    locator``
        
        locator: name or tooltip. 
        
        *Hint*: Some tooltips consist of a name followed by several spaces and a keyboard shortcut.
        The name may be used as locator as long as it is unique.
        When using the full tooltip text remember to escape the spaces (e.g. ``Back \\ \\ (F3)``)
        """
        
        args = [locator]
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The button '{0}' could not be found. Hints: Check the spelling, maximize the SAP window",
            "NotChangeable": "The button '{0}' is disabled.",
            "Pass": "The button '{0}' was pushed.",
            "Exception": "The button could not be pushed. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('PushButton', args, result) # type: ignore
    

    @keyword('Push Button Cell') # type: ignore
    def push_button_cell(self, row_locator: str, column: str): # type: ignore
        """
        Push the button cell located at the intersection of the row and column provided.
        
        | ``Push Button Cell     row_locator     column``
        
        row_locator: Either the row number or the button label, button tooltip, or the contents of a cell in the row. If the label, the tooltip or the contents of the cell is a number, it must be enclosed in double quotation marks.
        """
        
        args = [row_locator, column]
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The button cell with the locator '{0}, {1}' could not be found. Hints: Check the spelling, maximize the SAP window",
            "NotChangeable": "The button cell with the locator '{0}, {1}' is disabled.",
            "Pass": "The button cell with the locator '{0}, {1}' was pushed.",
            "Exception": "The button cell could not be pushed. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('PushButtonCell', args, result) # type: ignore
    

    @keyword('Read Statusbar') # type: ignore
    def read_statusbar(self): # type: ignore
        """
        Read the contents of the statusbar.
        
        | ``${statusbar}   Read Statusbar``
        
        The return value is a dictionary with the entries "status" and "message".
        """
        
        args = []
        
        result = {
            "Json": "The return value is in JSON format",
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "No statusbar was found.",
            "Pass": "The statusbar was read.",
            "Exception": "The statusbar could not be read\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ReadStatusbar', args, result) # type: ignore
    

    @keyword('Read Text Field') # type: ignore
    def read_text_field(self, locator: str): # type: ignore
        """
        Read the contents of the text field specified by the locator.
        
        | ${contents}   ``Read Text Field    locator``
        
        Text field locators are documented in the keyword Fill Text Field.
        """
        
        args = [locator]
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
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
        | ${text}   ``Read Text    = substring``
        
        *Text following a label*
        | ${text}   ``Read Text    label``
        """
        
        args = [locator]
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
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
        
        row_locator: either the row number or the contents of a cell in the row. If the cell only contains a number, it must be enclosed in double quotation marks.
        """
        
        args = [row_locator, column]
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The cell with the locator '{0}, {1}' could not be found. Hints: Check the spelling, maximize the SAP window",
            "NoTable": "The window contains no table.",
            "Pass": "The cell with the locator '{0}, {1}' was read.",
            "Exception": "The cell could not be read. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ReadTableCell', args, result) # type: ignore
    

    @keyword('Save Screenshot') # type: ignore
    def save_screenshot(self, destination: str): # type: ignore
        """
        Save a screenshot of the current window to the given destination.
        
        | ``Save Screenshot     destination``
        
        destination: Either the absolute path to a .png file or LOG to embed the image in the protocol.
        
        *Hint*: Use ${/} as path separator. Otherwise the backslashes must be escaped.
        """
        
        args = [destination]
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "InvalidPath": "The path '{0}' is invalid.",
            "UNCPath": "UNC paths (i.e. beginning with \\\\) are not allowed",
            "NoAbsPath": "The path '{0}' is not an absolute path.",
            "Log": "The return value will be written to the protocol",
            "Pass": "The screenshot was saved in {0}.",
            "Exception": "The screenshot could not be saved. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SaveScreenshot', args, result) # type: ignore
    

    @keyword('Scroll Contents') # type: ignore
    def scroll_text_field_contents(self, direction: str, until_textfield: str=None): # type: ignore
        """
        Scroll the contents of the text fields within an area with a scrollbar.
        
        | ``Scroll Contents    direction``
        
        direction: UP, DOWN, BEGIN, END
        
        If the parameter "until_textfield" is provided, the contents are scrolled until that text field is found.
        
        | ``Scroll Contents    direction    until_textfield``
        
        until_textfield: locator to find a text field
        """
        
        args = [direction, until_textfield]
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "Exception": "The contents of the text fields could not be scrolled.\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html",
            "NoScrollbar": "The window contains no scrollable text fields.",
            "MaximumReached": "The contents of the text fields cannot be scrolled any further.",
            "InvalidDirection": "Invalid direction. The direction must be one of: UP, DOWN, BEGIN, END",
            "Pass": "The contents of the text fields were scrolled in the direction '{0}'."
        }
        return super()._run_keyword('ScrollTextFieldContents', args, result) # type: ignore
    

    @keyword('Select Cell') # type: ignore
    def select_cell(self, row_locator: str, column: str): # type: ignore
        """
        Select the cell at the intersection of the row and column provided.
        
        | ``Select Cell     row_locator     column``
        
        row_locator: either the row number or the contents of a cell in the row. If the cell only contains a number, it must be enclosed in double quotation marks.
        """
        
        args = [row_locator, column]
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The cell with the locator '{0}, {1}' could not be found. Hints: Check the spelling, maximize the SAP window",
            "NoTable": "The window contains no table.",
            "Pass": "The cell with the locator '{0}, {1}' was selected.",
            "Exception": "The cell could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SelectCell', args, result) # type: ignore
    

    @keyword('Select Cell Value') # type: ignore
    def select_cell_value(self, row_locator: str, column: str, value: str): # type: ignore
        """
        Select the specified value in the cell at the intersection of the row and column provided.
        
        | ``Select Cell Value    row_locator    column    value``
        
        row_locator: either the row number or the contents of a cell in the row. If the cell only contains a number, it must be enclosed in double quotation marks.
        """
        
        args = [row_locator, column, value]
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The cell with the locator '{0}, {1}' could not be found. Hints: Check the spelling, maximize the SAP window",
            "EntryNotFound": "The value '{2}' is not available in the cell with the locator '{0}, {1}'. Hint: Check the spelling",
            "Exception": "The value could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html",
            "Pass": "The value '{2}' was selected."
        }
        return super()._run_keyword('SelectCellValue', args, result) # type: ignore
    

    @keyword('Select Dropdown Menu Entry') # type: ignore
    def select_combo_box_entry(self, dropdown_menu: str, entry: str): # type: ignore
        """
        Select the specified entry from the dropdown menu provided.
        
        | ``Select Dropdown Menu Entry   dropdown_menu    entry``
        
        *Hints*: The numeric key that enables simplified keyboard input is not part of the entry name.
        
        To select a value from a toolbar button with a dropdown menu, first push the button and then use this keyword. 
        """
        
        args = [dropdown_menu, entry]
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The dropdown menu '{0}' could not be found. Hint: Check the spelling",
            "EntryNotFound": "In the dropdown menu '{0}' no entry '{1}' could be found. Hint: Check the spelling",
            "Pass": "In the dropdown menu '{0}' the entry '{1}' was selected.",
            "Exception": "The entry could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SelectComboBoxEntry', args, result) # type: ignore
    

    @keyword('Select Menu Entry') # type: ignore
    def select_menu_item(self, menu_entry_path: str): # type: ignore
        """
        Select the menu entry with the path provided.
        
        | ``Select Menu Entry    menu_entry_path``
        
        menu_entry_path: The path to the entry with '/' as separator (e.g. System/User Profile/Own Data).
        """
        
        args = [menu_entry_path]
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The menu entry '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The menu entry '{0}' was selected.",
            "Exception": "The menu entry could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SelectMenuItem', args, result) # type: ignore
    

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
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The radio button with locator '{0}' could not be found. Hint: Check the spelling",
            "NotChangeable": "The radio button with the locator '{0}' is disabled.",
            "Pass": "The radio button with locator '{0}' was selected.",
            "Exception": "The radio button could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SelectRadioButton', args, result) # type: ignore
    

    @keyword('Select Table Row') # type: ignore
    def select_table_row(self, row_locator: str): # type: ignore
        """
        Select the specified table row.
        
        | ``Select Table Row    row_locator``
        
        row_locator: row_locator: either the row number or the contents of a cell in the row. If the cell only contains a number, it must be enclosed in double quotation marks.
        
        *Hint*: Use the row number 0 to select the whole table.
        """
        
        args = [row_locator]
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "Exception": "The row could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html",
            "NoTable": "The window contains no table",
            "InvalidIndex": "The table does not have a row with index '{0}'",
            "NotFound": "The table does not contain a cell with the contents '{0}'",
            "Pass": "The row with the locator '{0}' was selected"
        }
        return super()._run_keyword('SelectTableRow', args, result) # type: ignore
    

    @keyword('Select Text Field') # type: ignore
    def select_text_field(self, locator: str): # type: ignore
        """
        Select the text field specified by the locator.
        
        | ``Select Text Field    locator``
        
        Text field locators are documented in the keyword Fill Text Field.
        """
        
        args = [locator]
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The text field with the locator '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The text field with the locator '{0}' was selected.",
            "Exception": "The text field could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SelectTextField', args, result) # type: ignore
    

    @keyword('Select Text') # type: ignore
    def select_text(self, locator: str): # type: ignore
        """
        Select the text specified by the locator.
        
        *Text starting with a given substring*
        | ``Select Text    = substring``
        
        *Text following a label*
        | ``Select Text    Label``
        """
        
        args = [locator]
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The text with the locator '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The text with the locator '{0}' was selected.",
            "Exception": "The text could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SelectText', args, result) # type: ignore
    

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
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The checkbox with the locator '{0}' could not be found. Hint: Check the spelling",
            "NotChangeable": "The checkbox with the locator '{0}' is disabled.",
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
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The checkbox with the locator '{0}' could not be found. Hint: Check the spelling",
            "NotChangeable": "The checkbox with the locator '{0}' is disabled.",
            "Pass": "The checkbox with the locator '{0}' was unticked.",
            "Exception": "The checkbox could not be unticked. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('UntickCheckBox', args, result) # type: ignore
    

    @keyword('Tick Checkbox Cell') # type: ignore
    def tick_check_box_cell(self, row_locator: str, column: str): # type: ignore
        """
        Tick the checkbox cell at the intersection of the row and the column provided.
        
        | ``Tick Checkbox Cell     row_locator    column``
        
        row_locator: either the row number or the contents of a cell in the row. If the cell only contains a number, it must be enclosed in double quotation marks.
        
        *Hint*: To tick the checkbox in the leftmost column with no title, select the row and press the "Enter" key.
        """
        
        args = [row_locator, column]
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The checkbox cell with the locator '{0}, {1}' could not be found. Hints: Check the spelling, maximize the SAP window",
            "NotChangeable": "The checkbox cell with the locator '{0}, {1}' is disabled.",
            "Pass": "The checkbox cell with the locator '{0}, {1}' was ticked.",
            "Exception": "The checkbox cell could not be ticked. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('TickCheckBoxCell', args, result) # type: ignore
    

    @keyword('Untick Checkbox Cell') # type: ignore
    def untick_check_box_cell(self, row_locator: str, column: str): # type: ignore
        """
        Untick the checkbox cell at the intersection of the row and the column provided.
        
        | ``Untick Checkbox Cell     row_locator    column``
        
        row_locator: either the row number or the contents of a cell in the row. If the cell only contains a number, it must be enclosed in double quotation marks.
        """
        
        args = [row_locator, column]
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The checkbox cell with the locator '{0}, {1}' could not be found. Hints: Check the spelling, maximize the SAP window",
            "NotChangeable": "The checkbox cell with the locator '{0}, {1}' is disabled.",
            "Pass": "The checkbox cell with the locator '{0}, {1}' was unticked.",
            "Exception": "The checkbox cell could not be unticked. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('UntickCheckBoxCell', args, result) # type: ignore
    

    @keyword('Get Window Title') # type: ignore
    def get_window_title(self): # type: ignore
        """
        Get the title of the window in the foreground.
        
        | ``${title}    Get Window Title``
        """
        
        args = []
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "Pass": "The title of the window was obtained.",
            "Exception": "The window title could not be read.\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('GetWindowTitle', args, result) # type: ignore
    

    @keyword('Get Window Text') # type: ignore
    def get_window_text(self): # type: ignore
        """
        Get the text message of the window in the foreground.
        
        | ``${text}    Get Window Text``
        """
        
        args = []
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "Pass": "The text message of the window was obtained.",
            "Exception": "The text message of the window could not be read.\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('GetWindowText', args, result) # type: ignore
    
    ROBOT_LIBRARY_SCOPE = 'SUITE'
    ROBOT_LIBRARY_VERSION = '2.2.0'