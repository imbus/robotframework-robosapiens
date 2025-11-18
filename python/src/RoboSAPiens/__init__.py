from robot.api.deco import keyword
from RoboSAPiens.client import RoboSAPiensClient

__version__ = "2.23.0"

class RoboSAPiens(RoboSAPiensClient):
    """
    RoboSAPiens: SAP GUI-Automation for Humans
    
    In order to use this library the following requirements must be satisfied:
    
    - Scripting must be [https://help.sap.com/saphelp_aii710/helpdata/en/ba/b8710932b8c64a9e8acf5b6f65e740/content.htm|enabled on the SAP server].
    
    - Scripting Support must be [https://help.sap.com/docs/sap_gui_for_windows/63bd20104af84112973ad59590645513/7ddb7c9c4a4c43219a65eee4ca8db001.html?locale=en-US|enabled in SAP GUI] .
    
    == New features in Version 2.4 ==
    
    - Support for SAP Business Client
    - Documentation for automating embedded browser controls (Edge only)
    
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
    
    In order to login to a server execute the following (adjust the path accordingly):
    
    | Open SAP             C:${/}Program Files (x86)${/}SAP${/}FrontEnd${/}SAPgui${/}saplogon.exe
    | Connect to Server    My Test Server
    | Fill Text Field      User              TESTUSER
    | Fill Text Field      Password          TESTPASSWORD
    | Push Button          Enter
    
    For a hands-on tutorial watch the talk [https://www.youtube.com/watch?v=H7fYngdY7NI|RoboSAPiens: SAP GUI Automation for Humans] presented at the Online RoboCon 2024.
    
    == Dealing with spontaneous pop-up windows ==
    
    When clicking a button it can happen that a dialog window pops up.
    The following keyword can be useful in this situation:
    
    | Click button and close pop-up window
    |     [Arguments]   ${button}   ${title}   ${close button}
    |  
    |     Push Button       ${button}
    |     ${window_title}   Get Window Title
    |  
    |     IF   $window_title == $title
    |         Log               Pop-up window: ${title}
    |         Save screenshot   LOG
    |         Push button       ${close button}
    |     END
    
    == Automating embedded browser controls using Browser Library ==
    
    Configure the following environment variable in Windows:
    
    | Name:  WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS
    | Value: --enable-features=msEdgeDevToolsWdpRemoteDebugging --remote-debugging-port=4711
    
    Start SAP Logon or SAP Business Client.
    
    In SAP Logon go to ``Options > Interaction Design > Control Settings`` and set "Browser Control" to "Edge".
    
    In SAP Business Client go to ``Settings > Browser`` and set "Primary Browser Control" to "Edge".
    
    Log in to the SAP server and execute the transaction that contains one or more browser controls.
    
    Start a Chromium-based browser, e.g. Microsoft Edge, and open the URL ``chrome://inspect``.
    
    Click on "Configure...", add the entry ``localhost:4711`` and delete all other entries.
    
    Under the heading "Remote Target" the pages from all browser controls will be shown. In order to open the Developer Tools for a page click on "inspect".
    
    In Robot Framework call the following keyword from [https://robotframework-browser.org/|Browser Library]:
    
    | ``Connect To Browser   http://localhost:4711   chromium   use_cdp=True``
    
    Get the ``id`` of the page you want to automate using the following keyword:
    
    | Get Page Id by Title
    |     [Arguments]    ${title}
    | 
    |     ${browsers}    Get Browser Catalog
    |     ${contexts}    Set Variable           ${browsers}[0][contexts]
    |     ${pages}       Set Variable           ${contexts}[0][pages]
    | 
    |     FOR  ${page}  IN  @{pages}
    |         IF  '${page}[title]' == '${title}'
    |             Return From Keyword    ${page}[id]
    |         END
    |     END
    |
    |     Fail    The page '${title}' is not open in the current browser.
    
    Activate the page with:
    
    | ``Switch Page    ${id}``
    
    *Hint*: If the element you want to automate is inside an ``iframe`` or a ``frame``, prepend the (i)frame selector to its selector.
    Example:
    
    | ``Highlight Elements    id=frameId >>> element[name=elementName]``
    
    == Keeping secrets safe ==
    In order to prevent secret leakage into the Robot Framework protocol disable the logger before calling sensitive keywords:
    
    | ${log_level}       Set Log Level    NONE
    | Fill Text Field    ${locator}       ${password}
    | Set Log Level      ${log_level}
    
    == Assertions ==
    Using the following helper keyword custom assertions can be implemented.
    Currently ``${state}`` can only be Found or Changeable.
    
    | Element should be ${state}
    |     [Arguments]    ${keyword}    @{args}    ${message}
    |     [Tags]         robot:flatten
    |     
    |     TRY
    |         Run Keyword    ${keyword}    @{args}
    |     EXCEPT  Not${state}: *    type=GLOB
    |         Fail    ${message}
    |     END
    
    For example, the following keyword asserts that a given text field should be present:
    
    | Text field should be present
    |     [Arguments]    ${locator}
    | 
    |     Element should be Found    Select Text Field    ${locator}    message=The text field '${locator}' is not present.
    
    And the following keyword asserts that a given cell should be present in a table:
    
    | Cell should be present
    |     [Arguments]     ${row}    ${col}
    | 
    |     Element should be Found    Select Cell    ${row}    ${col}    message=The cell '${row}, ${col}' is not present.
    
    == Consecutive columns with the same name ==
    If a table contains consecutive columns with the same name a given column can be specified by appending a numeric suffix.
    
    For example, if a table contains the columns Variant, Variant, Variant. They can be identified as Variant__1, Variant__2, Variant__3.
    
    == Exporting a table as a spreadsheet ==
    
    Some tables can be exported as a spreadsheet via a toolbar button with a context menu. In order to select the corresponding menu item two keyword calls are necessary:
    
    | Push Button                  Export
    | Select Dropdown Menu Entry   Export   Spreadsheet
    
    == Automatically take a screenshot when a keyword fails ==
    
    Robot Framework 7 provides the listener method ``end_library_keyword``, which allows implementing error handling for library keywords.
    In the case of RoboSAPiens whenever a keyword fails it can be useful to take a screenshot and embed it in the log. 
    To achieve this create the file Listener.py with the following code:
    
    | from robot.api.interfaces import ListenerV3 
    | from robot.libraries.BuiltIn import BuiltIn
    | 
    | class Listener(ListenerV3):
    |     def end_library_keyword(self, data, implementation, result):
    |         library = 'RoboSAPiens'
    |         if result.failed and implementation.full_name.startswith(library):
    |             robosapiens = BuiltIn().get_library_instance(library)
    |             robosapiens.save_screenshot('LOG')
    
    Then execute ``robot`` with 
    
    | ``robot -P . --listener Listener test.robot``
    
    == ABAP Lists ==
    
    Since version 2.21.0 the keywords for interacting with tables work with the [https://help.sap.com/docs/ABAP_PLATFORM_NEW/b1c834a22d05483b8a75710743b5ff26/4dd40b7ac2234be2e10000000a42189c.html?locale=en-US|SAP List Viewer (Classic)].
    The only requirement is to enable the [https://help.sap.com/docs/sap_gui_for_windows/63bd20104af84112973ad59590645513/38da185ebd1540bdbc919db7b9013c9a.html?locale=en-US|Accessibility Mode] in the SAP GUI options.
    
    === Hierarchical-sequential lists ===
    
    In the case of a hierarchical-sequential list top-level (table_number=1) and second-level (table_number=2,3,...) tables have different colors.
    
    In order to collapse or expand a second-level table use the keyword "Select Table Row". 
    
    In order to click a cell in a top-level or second-level table use the keyword "Double-click Cell".
    
    === Trees ===
    
    In order to expand or collapse a tree node use the keyword "Select Text" to select the node and the keyword "Press Key Combination" to press F2.
    
    In order to push a button in a tree node, select the node and then press the corresponding F-key. Hint: Right click on the window in order to see the available keyboard shortcuts.
    """

    def __init__(self, presenter_mode: bool=False, x64: bool=False):
        """
        RoboSAPiens has the following initialization arguments:
        
        | =Argument= | =Description= |
        | ``presenter_mode`` | Wait half a second after executing a keyword and highlight the GUI element acted upon (if applicable) |
        | ``x64`` | Execute RoboSAPiens 64-bit in order to automate SAP GUI 8 64-bit or SAP Business Client |
        """
        
        args = {
            'presenter_mode': presenter_mode,
            'x64': x64
        }
        
        super().__init__(args)
    
    
    @keyword('Select Tab') # type: ignore
    def activate_tab(self, tab_name: str): # type: ignore
        """
        Select the tab with the name provided.
        
        | ``tab_name`` | Name or tooltip of the tab |
        
        Examples:
        
        | ``Select Tab    tab_name``
        """

        args: list = [
            tab_name
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The tab '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The tab '{0}' was selected.",
            "Exception": "The tab could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ActivateTab', args, kwargs, result) # type: ignore
    
    @keyword('Double-click Tree Element') # type: ignore
    def double_click_tree_element(self, element_path: str): # type: ignore
        """
        Double-click the tree element located at the path provided.
        
        | ``element_path`` | The path to the element using '/' as separator. e.g. Engineering/Civil Engineering |
        
        Examples:
        
        | ``Double-click Tree Element    element_path``
        
        Further details about the element path are provided in the keyword [#Select Tree Element|Select Tree Element].
        """

        args: list = [
            element_path
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The tree element '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The tree element '{0}' was double-clicked.",
            "Exception": "The tree element could not be double-clicked. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('DoubleClickTreeElement', args, kwargs, result) # type: ignore
    
    @keyword('Expand Tree Folder') # type: ignore
    def expand_tree_folder(self, folder_path: str): # type: ignore
        """
        Expand the folder located at the path provided in a tree structure.
        
        | ``folder_path`` | The path to the folder using '/' as separator. e.g. Engineering/Civil Engineering |
        
        Examples:
        
        | ``Expand Tree Folder    folder_path``
        
        Further details about the folder path are provided in the keyword [#Select Tree Element|Select Tree Element].
        """

        args: list = [
            folder_path
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The tree folder '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The tree folder '{0}' was expanded.",
            "Exception": "The tree folder could not be expanded. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ExpandTreeFolder', args, kwargs, result) # type: ignore
    
    @keyword('Select Tree Element') # type: ignore
    def select_tree_element(self, element_path: str): # type: ignore
        """
        Select the tree element located at the path provided.
        
        | ``element_path`` | The path to the element using '/' as separator. e.g. Engineering/Civil Engineering |
        
        Examples:
        
        | ``Select Tree Element    element_path``
        
        *Hints*
        - A slash that is not a path separator must be written twice.
        - Each segment of the path may be partially specified. For example, IDoc instead of IDoc 1234.
        """

        args: list = [
            element_path
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The tree element '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The tree element '{0}' was selected.",
            "Exception": "The tree element could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SelectTreeElement', args, kwargs, result) # type: ignore
    
    @keyword('Select Menu Entry in Tree Element') # type: ignore
    def select_tree_element_menu_entry(self, element_path: str, menu_entry: str): # type: ignore
        """
        Select the given entry in the context menu of the tree element located at the path provided.
        
        | ``element_path`` | The path to the element using '/' as separator, e.g. Engineering/Civil Engineering. |
        | ``menu_entry`` | The menu entry. For nested menus the path to the entry using '|' as separator, e.g. Create|Business Unit. |
        
        Examples:
        
        | ``Select Menu Entry in Tree Element    element_path    menu_entry``
        
        Further details about the element path are provided in the keyword [#Select Tree Element|Select Tree Element].
        """

        args: list = [
            element_path,
            menu_entry
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The tree element '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The menu entry '{0}' was selected.",
            "Exception": "The menu entry could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SelectTreeElementMenuEntry', args, kwargs, result) # type: ignore
    
    @keyword('Open SAP') # type: ignore
    def open_sap(self, path: str, sap_args: str=None): # type: ignore
        """
        Open SAP GUI or SAP Business Client.
        
        | ``path`` | The path of the SAP executable |
        | ``sap_args`` | Command line arguments for the SAP executable |
        
        Examples:
        
        *Start the SAP client*
        
        | ``Open SAP   path``
        
        For SAP Logon 32-bit the standard path is
        
        | ``C:\\Program Files (x86)\\SAP\\FrontEnd\\SAPgui\\saplogon.exe``
        
        For SAP Logon 64-bit the standard path is
        
        | ``C:\\Program Files\\SAP\\FrontEnd\\SAPgui\\saplogon.exe``
        
        For SAP Business Client the standard path is
        
        | ``C:\\Program Files\\SAP\\NWBC800\\NWBC.exe``
        
        *Start SAP Logon logged in to a client*
        
        | ``Open SAP   C:\\Program Files\\SAP\\FrontEnd\\SAPgui\\sapshcut.exe -sysname=XXX -client=NNN -user=%{username} -pw=%{password}``
        
        *Hints*
        
        - Backslashes must be written twice. Otherwise use the RF built-in variable ${/} as path separator.
        - 64-bit SAP clients can only be used when the library is imported with ``x64=True``
        - sysname is the name of the connection in SAP Logon. If it contains spaces it must be enclosed in double quotes.
        """

        args: list = [
            path
        ]
        kwargs: dict = {
            "sap_args": sap_args
        }
        
        result = {
            "Pass": "SAP was opened.",
            "NoGuiScripting": "The scripting support is not activated. It must be activated in the Settings of the SAP client.",
            "SAPAlreadyRunning": "SAP is already running. It must be closed before calling this keyword.",
            "SAPNotStarted": "SAP could not be opened. Verify that the path and the arguments (if applicable) are correct.",
            "Exception": "SAP could not be opened. Hint: In order to start a 64-bit SAP client import RoboSAPiens with x64=True. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('OpenSap', args, kwargs, result) # type: ignore
    
    @keyword('Disconnect from Server') # type: ignore
    def close_connection(self): # type: ignore
        """
        Terminate the current connection to the SAP server.
        
        
        Examples:
        
        | ``Disconnect from server``
        """

        args: list = [
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "Pass": "Disconnected from the server.",
            "Exception": "Could not disconnect from the server. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('CloseConnection', args, kwargs, result) # type: ignore
    
    @keyword('Close SAP') # type: ignore
    def close_sap(self): # type: ignore
        """
        Close the SAP GUI and terminate its process.
        
        
        Examples:
        
        | ``Close SAP``
        
        *Hint*: This keyword only works if SAP GUI was started with the keyword [#Open SAP|Open SAP].
        """

        args: list = [
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSapGui": "No open SAP GUI found. Call the keyword \"Open SAP\" first.",
            "Pass": "The SAP GUI was closed."
        }
        return super()._run_keyword('CloseSap', args, kwargs, result) # type: ignore
    
    @keyword('Close Window') # type: ignore
    def close_window(self): # type: ignore
        """
        Close the window in the foreground.
        
        
        Examples:
        
        | ``Close Window``
        """

        args: list = [
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "Exception": "The window could not be closed. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html",
            "Pass": "The window in the foreground was closed."
        }
        return super()._run_keyword('CloseWindow', args, kwargs, result) # type: ignore
    
    @keyword('Get Row Count') # type: ignore
    def count_table_rows(self, table_number: int=1): # type: ignore
        """
        Count the rows in a table.
        
        | ``table_number`` | Specify which table: 1, 2, ... |
        
        Examples:
        
        | ``${row_count}   Get Row Count``
        """

        args: list = [
        ]
        kwargs: dict = {
            "table_number": table_number
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "Exception": "Could not count the rows in the table. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html",
            "NotFound": "The window contains no table.",
            "InvalidTable": "The window contains no table with index {0}.",
            "Pass": "Counted the number of rows in the table."
        }
        return super()._run_keyword('CountTableRows', args, kwargs, result) # type: ignore
    
    @keyword('Export Tree Structure') # type: ignore
    def export_tree(self, filepath: str): # type: ignore
        """
        Export the tree structure in the current window to the provided JSON file.
        
        | ``filepath`` | Absolute path to a file with extension .json |
        
        Examples:
        
        | ``Export Tree Structure     filepath``
        
        *Hint*: Backslashes must be written twice. Otherwise use the RF built-in variable ${/} as path separator.
        """

        args: list = [
            filepath
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The window contains no tree structure",
            "Pass": "The tree structure was exported to the file '{0}'",
            "Exception": "The tree structure could not be exported. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ExportTree', args, kwargs, result) # type: ignore
    
    @keyword('Connect to Running SAP') # type: ignore
    def connect_to_running_sap(self, session_number: int=1, connection: str=None, client: str=None): # type: ignore
        """
        Connect to an already running SAP instance and take control of it.
        
        | ``session_number`` | The session number shown in the upper right or lower right corner of the window |
        | ``connection`` | The name of the connection in SAP Logon |
        | ``client`` | The three-digit number of the client |
        
        Examples:
        
        | ``Connect to Running SAP``
        
        By default the session number 1 will be used. To use a different session specify the session number.
        
        | ``Connect to Running SAP    session_number=N``
        
        In order to connect to a session on a specific connection:
        
        | ``Connect to Running SAP    connection=Test Connection   session_number=N``
        
        In order to connect to a client on a specific connection:
        
        | ``Connect to Running SAP    connection=Test Connection   client=NNN``
        
        The return value contains session information such as client number and system ID.
        """

        args: list = [
        ]
        kwargs: dict = {
            "session_number": session_number,
            "connection": connection,
            "client": client
        }
        
        result = {
            "NoSapGui": "No running SAP GUI found.",
            "NoGuiScripting": "The scripting support is not activated. It must be activated in the Settings of the SAP client.",
            "NoConnection": "No existing connection to an SAP server. Call the keyword \"Connect to Server\" first.",
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NoServerScripting": "Scripting is not activated on the server side. Please consult the documentation of RoboSAPiens.",
            "InvalidClient": "There is no client '{client}' in the current connection.",
            "InvalidConnection": "There is no connection with the name '{connection}'.",
            "InvalidConnectionClient": "There is no client '{client}' in the connection '{connection}'.",
            "InvalidSession": "There is no session number {session_number} for the current connection.",
            "SapError": "SAP Error: {0}",
            "Json": "The return value is in JSON format",
            "Pass": "Connected to a running SAP instance.",
            "Exception": "Could not connect to a running SAP instance. Hint: In order to connect to a 64-bit SAP client import RoboSAPiens with x64=True. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ConnectToRunningSap', args, kwargs, result) # type: ignore
    
    @keyword('Connect to Server') # type: ignore
    def connect_to_server(self, server_name: str): # type: ignore
        """
        Connect to the SAP server using the provided connection.
        
        | ``server_name`` | The name of the connection in SAP Logon (not the SID). |
        
        Examples:
        
        | ``Connect to Server    server_name``
        
        The return value contains session information such as client number and system ID.
        """

        args: list = [
            server_name
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSapGui": "No open SAP GUI found. Call the keyword \"Open SAP\" first.",
            "NoGuiScripting": "The scripting support is not activated. It must be activated in the Settings of the SAP client.",
            "Pass": "Connected to '{0}'",
            "Json": "The return value is in JSON format",
            "SapError": "SAP Error: {0}",
            "NoServerScripting": "Scripting is not activated on the server side. Please consult the documentation of RoboSAPiens.",
            "InvalidSession": "There is no session number '{0}' for the current connection.",
            "Exception": "Could not establish the connection. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ConnectToServer', args, kwargs, result) # type: ignore
    
    @keyword('Double-click Cell') # type: ignore
    def double_click_cell(self, row_locator: str, column: str, table_number: int=None): # type: ignore
        """
        Double-click the cell at the intersection of the provided row and column.
        
        | ``row_locator`` | Either the row number or the contents of a cell in the row. If the cell only contains a number, it must be enclosed in double quotation marks. |
        | ``column`` | Column title or tooltip |
        | ``table_number`` | Specify which table: 1, 2, ... |
        
        Examples:
        
        | ``Double-click Cell     row_locator     column``
        """

        args: list = [
            row_locator,
            column
        ]
        kwargs: dict = {
            "table_number": table_number
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The cell with the locator '{0}, {1}' could not be found. Hints: Check the spelling, maximize the SAP window",
            "InvalidTable": "The window contains no table with index {0}.",
            "Pass": "The cell with the locator '{0}, {1}' was double-clicked.",
            "Exception": "The cell could not be double-clicked. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('DoubleClickCell', args, kwargs, result) # type: ignore
    
    @keyword('Double-click Text Field') # type: ignore
    def double_click_text_field(self, locator: str): # type: ignore
        """
        Double-click the text field specified by the locator or with the given content.
        
        | ``locator`` | Text field locators are documented in the keyword [#Fill Text Field|Fill Text Field]. |
        
        Examples:
        
        *Find the text field using a locator*
        | ``Double-click Text Field     locator``
        
        *Find the text field by its content*
        | ``Double-click Text Field    = content``
        """

        args: list = [
            locator
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The text field with the locator '{0}' could not be found Hint: Check the spelling",
            "Pass": "The text field with the locator '{0}' was double-clicked.",
            "Exception": "The text field could not be double-clicked. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('DoubleClickTextField', args, kwargs, result) # type: ignore
    
    @keyword('Execute Transaction') # type: ignore
    def execute_transaction(self, T_Code: str): # type: ignore
        """
        Execute the transaction with the given T-Code.
        
        | ``T_Code`` | The transaction code |
        
        Examples:
        
        | ``Execute Transaction    T_Code``
        """

        args: list = [
            T_Code
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "Pass": "The transaction with T-Code {0} was executed.",
            "Exception": "The transaction could not be executed. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ExecuteTransaction', args, kwargs, result) # type: ignore
    
    @keyword('Export Window') # type: ignore
    def export_window(self, name: str, directory: str): # type: ignore
        """
        Export the window contents to a JSON file. A screenshot will automatically be saved in PNG format.
        
        | ``name`` | Name of the output files |
        | ``directory`` | Absolute path to the directory where the files will be saved |
        
        Examples:
        
        | ``Export Window     name     directory``
        
        *Hint*: Backslashes must be written twice. Otherwise use the RF built-in variable ${/} as path separator.
        
        *Note*: Currently not all GUI elements are exported.
        """

        args: list = [
            name,
            directory
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "Pass": "The window contents were exported to {0} and a screenshot was saved to {1}.",
            "Exception": "The window contents could not be exported. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ExportWindow', args, kwargs, result) # type: ignore
    
    @keyword('Fill Cell') # type: ignore
    def fill_cell(self, row_locator: str, column: str, content: str, table_number: int=None): # type: ignore
        """
        Fill the cell at the intersection of the row and column with the content provided.
        
        | ``row_locator`` | Either the row number or the contents of a cell in the row. If the cell only contains a number, it must be enclosed in double quotation marks. |
        | ``column`` | Column title or tooltip |
        | ``content`` | The new contents of the cell |
        | ``table_number`` | Specify which table: 1, 2, ... |
        
        Examples:
        
        | ``Fill Cell    row_locator    column   content``
        
        *Hint*: To migrate from the old keyword with two arguments perform a search and replace with a regular expression.
        """

        args: list = [
            row_locator,
            column,
            content
        ]
        kwargs: dict = {
            "table_number": table_number
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The cell with the locator '{0}, {1}' could not be found Hints: Check the spelling, maximize the SAP window",
            "NotChangeable": "The cell with the locator '{0}, {1}' is not editable.",
            "NoTable": "The window contains no table.",
            "InvalidTable": "The window contains no table with index {0}.",
            "Pass": "The cell with the locator '{0}, {1}' was filled.",
            "Exception": "The cell could not be filled. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('FillCell', args, kwargs, result) # type: ignore
    
    @keyword('Fill Multiline Text Field') # type: ignore
    def fill_text_edit(self, content: str): # type: ignore
        """
        Fill the multiline text field in the window with the content provided.
        
        | ``content`` | The new contents of the multiline text field |
        
        Examples:
        
        | ``Fill Multiline Text Field    A long text. With two sentences.``
        """

        args: list = [
            content
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The window contains no multiline text field. Hint: Check the spelling",
            "NotChangeable": "The multiline text field is not editable.",
            "Pass": "The multiline text field was filled.",
            "Exception": "The multiline text field could not be filled. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('FillTextEdit', args, kwargs, result) # type: ignore
    
    @keyword('Fill Text Field') # type: ignore
    def fill_text_field(self, locator: str, content: str, exact: bool=True): # type: ignore
        """
        Fill the text field specified by the locator with the content provided.
        
        | ``locator`` | The locator used to find the text field. |
        | ``content`` | The new contents of the text field |
        | ``exact`` | Whether to perform an exact search |
        
        Examples:
        
        *Text field with a label to its left*
        | ``Fill Text Field    label    content``
        
        *Hint*: The description obtained by selecting a text field and pressing F1 can usually be used as label. If it is too long, the beginning can be used by setting exact=False. 
        
        *Text field with a label above*
        | ``Fill Text Field    @ label    content``
        
        *Text field at the intersection of a label to its left and a label above it (including a heading)*
        | ``Fill Text Field    label to its left @ label above it    content``
        
        *Text field in a vertical grid below a label*
        | ``Fill Text Field    position (1,2,..) @ label    content``
        
        *Text field in a horizontal grid following a label*
        | ``Fill Text Field    label @ position (1,2,..)    content``
        
        *Text field with a non-unique label to the left or right of a unique label*
        | ``Fill Text Field    unique label >> text field label    content``
        
        *Text field without a label to the left or right of a unique label*
        | ``Fill Text Field    unique label >> F1 help text    content``
        
        *As a last resort the name obtained using [https://tracker.stschnell.de/|Scripting Tracker] can be used*
        | ``Fill Text Field    name    content``
        """

        args: list = [
            locator,
            content
        ]
        kwargs: dict = {
            "exact": exact
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The text field with the locator '{0}' could not be found. Hint: Check the spelling",
            "NotChangeable": "The text field with the locator '{0}' is not editable.",
            "Pass": "The text field with the locator '{0}' was filled.",
            "Exception": "The text field could not be filled. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('FillTextField', args, kwargs, result) # type: ignore
    
    @keyword('Highlight Button') # type: ignore
    def highlight_button(self, locator: str, exact: bool=False): # type: ignore
        """
        Highlight the button with the given locator.
        
        | ``locator`` | The name or tooltip of the button |
        | ``exact`` | `True` if the locator matches exactly the tooltip, `False` otherwise. |
        
        Examples:
        
        | ``Highlight Button    locator``
        
        *Hint*: Tooltips ending with a keyboard shortcut are common.
        By default ``exact=False`` in order to match the tooltip ignoring the shortcut.
        For tooltips without a keyboard shortcut an exact match (``exact=True``) is preferable.
        
        """

        args: list = [
            locator
        ]
        kwargs: dict = {
            "exact": exact
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The button '{0}' could not be found. Hints: Check the spelling, maximize the SAP window",
            "Pass": "The button '{0}' was highlighted.",
            "Exception": "The button could not be highlighted. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('HighlightButton', args, kwargs, result) # type: ignore
    
    @keyword('Press Key Combination') # type: ignore
    def press_key_combination(self, key_combination: str, table_number: int=None): # type: ignore
        """
        Press the given key combination.
        
        | ``key_combination`` | Either one key or several keys separated by a + sign. |
        | ``table_number`` | The table (1, 2, ...) that should receive the key press. |
        
        Examples:
        
        | ``Press Key Combination    key_combination``
        
        Among the valid key combinations are the keyboard shortcuts in the context menu (shown when the right mouse button is pressed). 
        For a full list of supported key combinations consult the [https://help.sap.com/docs/sap_gui_for_windows/b47d018c3b9b45e897faf66a6c0885a8/71d8c95e9c7947ffa197523a232d8143.html?locale=en-US|documentation of SAP GUI].
        
        *Hint*: Pressing F2 is equivalent to a double-click.
        """

        args: list = [
            key_combination
        ]
        kwargs: dict = {
            "table_number": table_number
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The key combination '{0}' is not supported. See the keyword documentation for valid key combinations.",
            "InvalidTable": "The window contains no table with index {0}.",
            "Pass": "The key combination '{0}' was pressed.",
            "Exception": "The key combination could not be pressed. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('PressKeyCombination', args, kwargs, result) # type: ignore
    
    @keyword('Push Button') # type: ignore
    def push_button(self, locator: str, exact: bool=False, table_number: int=None): # type: ignore
        """
        Push the button with the given locator.
        
        | ``locator`` | The name or tooltip of the button |
        | ``exact`` | `True` if the locator matches exactly the tooltip, `False` otherwise. |
        | ``table_number`` | The table (1, 2, ...) that contains the button in its toolbar. |
        
        Examples:
        
        *Button with a name or tooltip*
        
        | ``Push Button    name or tooltip``
        
        *Button with a non-unique name or tooltip to the left or right of a unique label*
        
        | ``Push Button    unique label >> name or tooltip``
        
        *Hint*: Tooltips ending with a keyboard shortcut are common.
        By default ``exact=False`` in order to match the tooltip ignoring the shortcut.
        For tooltips without a keyboard shortcut an exact match (``exact=True``) is preferable.
        
        """

        args: list = [
            locator
        ]
        kwargs: dict = {
            "exact": exact,
            "table_number": table_number
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The button '{0}' could not be found. Hints: Check the spelling, maximize the SAP window",
            "NotChangeable": "The button '{0}' is disabled.",
            "Pass": "The button '{0}' was pushed.",
            "Exception": "The button could not be pushed. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('PushButton', args, kwargs, result) # type: ignore
    
    @keyword('Push Button Cell') # type: ignore
    def push_button_cell(self, row_locator: str, column: str, table_number: int=None): # type: ignore
        """
        Push the button cell located at the intersection of the row and column provided.
        
        | ``row_locator`` | Either the row number or the button label, button tooltip, or the contents of a cell in the row. If the label, the tooltip or the contents of the cell is a number, it must be enclosed in double quotation marks. |
        | ``column`` | Column title or tooltip |
        | ``table_number`` | Specify which table: 1, 2, ... |
        
        Examples:
        
        | ``Push Button Cell     row_locator     column``
        """

        args: list = [
            row_locator,
            column
        ]
        kwargs: dict = {
            "table_number": table_number
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The button cell with the locator '{0}, {1}' could not be found. Hints: Check the spelling, maximize the SAP window",
            "NotAButton": "The cell with the locator '{0}, {1}' is not a button.",
            "NotChangeable": "The button cell with the locator '{0}, {1}' is disabled.",
            "InvalidTable": "The window contains no table with index {0}.",
            "Pass": "The button cell with the locator '{0}, {1}' was pushed.",
            "Exception": "The button cell could not be pushed. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('PushButtonCell', args, kwargs, result) # type: ignore
    
    @keyword('Read Statusbar') # type: ignore
    def read_statusbar(self): # type: ignore
        """
        Read the contents of the statusbar. The return value is a dictionary with the entries 'status' and 'message'.
        
        
        Examples:
        
        | ``${statusbar}   Read Statusbar``
        
        """

        args: list = [
        ]
        kwargs: dict = {
        }
        
        result = {
            "Json": "The return value is in JSON format",
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "No statusbar was found.",
            "Pass": "The statusbar was read.",
            "Exception": "The statusbar could not be read. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ReadStatusbar', args, kwargs, result) # type: ignore
    
    @keyword('Read Text Field') # type: ignore
    def read_text_field(self, locator: str): # type: ignore
        """
        Read the contents of the text field specified by the locator.
        
        | ``locator`` | Text field locators are documented in the keyword [#Fill Text Field|Fill Text Field]. |
        
        Examples:
        
        | ${contents}   ``Read Text Field    locator``
        """

        args: list = [
            locator
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The text field with the locator '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The text field with the locator '{0}' was read.",
            "Exception": "The text field could not be read. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ReadTextField', args, kwargs, result) # type: ignore
    
    @keyword('Read Text') # type: ignore
    def read_text(self, locator: str): # type: ignore
        """
        Read the text specified by the locator.
        
        | ``locator`` | The locator used to find the text |
        
        Examples:
        
        *Text starting with a given substring*
        | ${text}   ``Read Text    = substring``
        
        *Text following a label*
        | ${text}   ``Read Text    label``
        """

        args: list = [
            locator
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "No text with the locator '{0}' was found. Hint: Check the spelling",
            "Pass": "A text with the locator '{0}' was read.",
            "Exception": "The text could not be read. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ReadText', args, kwargs, result) # type: ignore
    
    @keyword('Read Cell') # type: ignore
    def read_cell(self, row_locator: str, column: str, table_number: int=None): # type: ignore
        """
        Read the contents of the cell at the intersection of the row and column provided.
        
        | ``row_locator`` | Either the row number or the contents of a cell in the row. If the cell only contains a number, it must be enclosed in double quotation marks. |
        | ``column`` | Column title or tooltip |
        | ``table_number`` | Specify which table: 1, 2, ... |
        
        Examples:
        
        | ``Read Cell     row_locator     column``
        """

        args: list = [
            row_locator,
            column
        ]
        kwargs: dict = {
            "table_number": table_number
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The cell with the locator '{0}, {1}' could not be found. Hints: Check the spelling, maximize the SAP window",
            "NoTable": "The window contains no table.",
            "InvalidTable": "The window contains no table with index {0}.",
            "Pass": "The cell with the locator '{0}, {1}' was read.",
            "Exception": "The cell could not be read. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ReadCell', args, kwargs, result) # type: ignore
    
    @keyword('Save Screenshot') # type: ignore
    def save_screenshot(self, destination: str): # type: ignore
        """
        Save a screenshot of the current window to the given destination.
        
        | ``destination`` | Either the absolute path to a .png file or LOG to embed the image in the protocol. |
        
        Examples:
        
        | ``Save Screenshot     destination``
        
        *Hint*: Backslashes must be written twice. Otherwise use the RF built-in variable ${/} as path separator.
        """

        args: list = [
            destination
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "InvalidPath": "The path '{0}' is invalid.",
            "UNCPath": "UNC paths (i.e. beginning with \\) are not allowed",
            "NoAbsPath": "The path '{0}' is not an absolute path.",
            "Log": "The return value will be written to the protocol",
            "Pass": "The screenshot was saved in {0}.",
            "Exception": "The screenshot could not be saved. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SaveScreenshot', args, kwargs, result) # type: ignore
    
    @keyword('Scroll Contents') # type: ignore
    def scroll_text_field_contents(self, direction: str, until_textfield: str=None): # type: ignore
        """
        Scroll the contents of the text fields within an area with a scrollbar.
        
        | ``direction`` | UP, DOWN, BEGIN, END |
        | ``until_textfield`` | Text field locators are documented in the keyword [#Fill Text Field|Fill Text Field]. |
        
        Examples:
        
        | ``Scroll Contents    direction``
        
        If the parameter "until_textfield" is provided, the contents are scrolled until that text field is found.
        
        | ``Scroll Contents    direction    until_textfield``
        """

        args: list = [
            direction
        ]
        kwargs: dict = {
            "until_textfield": until_textfield
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "Exception": "The contents of the text fields could not be scrolled. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html",
            "NoScrollbar": "The window contains no scrollable text fields.",
            "MaximumReached": "The contents of the text fields cannot be scrolled any further.",
            "InvalidDirection": "Invalid direction. The direction must be one of: UP, DOWN, BEGIN, END",
            "Pass": "The contents of the text fields were scrolled in the direction '{0}'."
        }
        return super()._run_keyword('ScrollTextFieldContents', args, kwargs, result) # type: ignore
    
    @keyword('Scroll Window Horizontally') # type: ignore
    def scroll_window_horizontally(self, direction: str): # type: ignore
        """
        Displace the horizontal scrollbar of the window in the given direction.
        
        | ``direction`` | LEFT, RIGHT, BEGIN, END |
        
        Examples:
        
        | ``Scroll Window Horizontally    direction``
        """

        args: list = [
            direction
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "Exception": "The window could not be scrolled horizontally. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html",
            "NoScrollbar": "The window contains no horizontal scrollbar.",
            "MaximumReached": "The window cannot be scrolled any further.",
            "InvalidDirection": "Invalid direction. The direction must be one of: LEFT, RIGHT, BEGIN, END",
            "Pass": "The window was scrolled horizontally in the direction '{0}'."
        }
        return super()._run_keyword('ScrollWindowHorizontally', args, kwargs, result) # type: ignore
    
    @keyword('Select Cell') # type: ignore
    def select_cell(self, row_locator: str, column: str, table_number: int=None): # type: ignore
        """
        Select the cell at the intersection of the row and column provided.
        
        | ``row_locator`` | Either the row number or the contents of a cell in the row. If the cell only contains a number, it must be enclosed in double quotation marks. |
        | ``column`` | Column title or tooltip |
        | ``table_number`` | Specify which table: 1, 2, ... |
        
        Examples:
        
        | ``Select Cell     row_locator     column``
        
        *Hint*: This keyword can be used to click a link (underlined text or icon) or a radio button in a cell.
        """

        args: list = [
            row_locator,
            column
        ]
        kwargs: dict = {
            "table_number": table_number
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The cell with the locator '{0}, {1}' could not be found. Hints: Check the spelling, maximize the SAP window",
            "NoTable": "The window contains no table.",
            "InvalidTable": "The window contains no table with index {0}.",
            "Pass": "The cell with the locator '{0}, {1}' was selected.",
            "Exception": "The cell could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SelectCell', args, kwargs, result) # type: ignore
    
    @keyword('Select Cell Value') # type: ignore
    def select_cell_value(self, row_locator: str, column: str, value: str, table_number: int=None): # type: ignore
        """
        Select the specified value in the cell at the intersection of the row and column provided.
        
        | ``row_locator`` | Either the row number or the contents of a cell in the row. If the cell only contains a number, it must be enclosed in double quotation marks. |
        | ``column`` | Column title or tooltip |
        | ``value`` | An entry from the dropdown menu |
        | ``table_number`` | Specify which table: 1, 2, ... |
        
        Examples:
        
        | ``Select Cell Value    row_locator    column    value``
        """

        args: list = [
            row_locator,
            column,
            value
        ]
        kwargs: dict = {
            "table_number": table_number
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The cell with the locator '{0}, {1}' could not be found. Hints: Check the spelling, maximize the SAP window",
            "EntryNotFound": "The value '{2}' is not available in the cell with the locator '{0}, {1}'. Hint: Check the spelling",
            "InvalidTable": "The window contains no table with index {0}.",
            "Exception": "The value could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html",
            "Pass": "The value '{2}' was selected."
        }
        return super()._run_keyword('SelectCellValue', args, kwargs, result) # type: ignore
    
    @keyword('Read Checkbox Status') # type: ignore
    def read_check_box(self, locator: str): # type: ignore
        """
        Read the status of the checkbox specified by the locator.
        
        | ``locator`` | A locator used to find the checkbox |
        
        Examples:
        
        *Checkbox with a label to its left or its right*
        | ``Read Checkbox Status    label``
        
        *Checkbox with a label above it*
        | ``Read Checkbox Status    @ label``
        
        *Checkbox at the intersection of a label to its left and a label above it*
        | ``Read Checkbox Status    left label @ label above``
        """

        args: list = [
            locator
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The checkbox with the locator '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The status of the checkbox with the locator '{0}' was read.",
            "Exception": "The status of the checkbox could not be read. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ReadCheckBox', args, kwargs, result) # type: ignore
    
    @keyword('Read Dropdown Menu Entry') # type: ignore
    def read_combo_box_entry(self, locator: str): # type: ignore
        """
        Read the current entry from the given dropdown menu.
        
        | ``locator`` | The label or tooltip of the dropdown menu |
        
        Examples:
        
        | ``${entry}   Read Dropdown Menu Entry   locator``
        """

        args: list = [
            locator
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The dropdown menu '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The current entry of the dropdown menu '{0}' was read.",
            "Exception": "The entry could not be read. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ReadComboBoxEntry', args, kwargs, result) # type: ignore
    
    @keyword('Select Dropdown Menu Entry') # type: ignore
    def select_combo_box_entry(self, locator: str, entry: str): # type: ignore
        """
        Select the specified entry from the given dropdown menu.
        
        | ``locator`` | The label or tooltip of the dropdown menu |
        | ``entry`` | An entry from the dropdown menu |
        
        Examples:
        
        | ``Select Dropdown Menu Entry   locator    entry``
        
        *Hints*: 
        - If the entry name is not unique use the key shown when "Show keys within dropdown lists" is activated in the SAP GUI options.
        - To select a value from a toolbar button with a dropdown menu use the button's label or tooltip as selector. 
        """

        args: list = [
            locator,
            entry
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The dropdown menu '{0}' could not be found. Hint: Check the spelling",
            "EntryNotFound": "In the dropdown menu '{0}' the entry '{1}' could not be found. Hint: Check the spelling",
            "Pass": "In the dropdown menu '{0}' the entry '{1}' was selected.",
            "Exception": "The entry could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SelectComboBoxEntry', args, kwargs, result) # type: ignore
    
    @keyword('Select Menu Entry') # type: ignore
    def select_menu_item(self, menu_entry_path: str): # type: ignore
        """
        Select the menu entry with the path provided.
        
        | ``menu_entry_path`` | The path to the entry with '/' as separator (e.g. System/User Profile/Own Data) |
        
        Examples:
        
        | ``Select Menu Entry    menu_entry_path``
        """

        args: list = [
            menu_entry_path
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The menu entry '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The menu entry '{0}' was selected.",
            "Exception": "The menu entry could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SelectMenuItem', args, kwargs, result) # type: ignore
    
    @keyword('Select Radio Button') # type: ignore
    def select_radio_button(self, locator: str): # type: ignore
        """
        Select the radio button specified by the locator.
        
        | ``locator`` | A locator used to find the radio button |
        
        Examples:
        
        *Radio button with a label to its left or its right*
        | ``Select Radio Button    label``
        
        *Radio button with a label above it*
        | ``Select Radio Button    @ label``
        
        *Radio button at the intersection of a label to its left or its right and a label above it*
        | ``Select Radio Button    left or right label @ label above``
        """

        args: list = [
            locator
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The radio button with locator '{0}' could not be found. Hint: Check the spelling",
            "NotChangeable": "The radio button with the locator '{0}' is disabled.",
            "Pass": "The radio button with locator '{0}' was selected.",
            "Exception": "The radio button could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SelectRadioButton', args, kwargs, result) # type: ignore
    
    @keyword('Select Table Column') # type: ignore
    def select_table_column(self, column: str, table_number: int=1): # type: ignore
        """
        Select the specified column in the given table.
        
        | ``column`` | The title or tooltip of the column |
        | ``table_number`` | Specify which table: 1, 2, ... |
        
        Examples:
        
        | ``Select Table Column    column``
        """

        args: list = [
            column
        ]
        kwargs: dict = {
            "table_number": table_number
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "Exception": "The column could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html",
            "NoTable": "The window contains no table",
            "InvalidTable": "The window contains no table with index {0}.",
            "NotFound": "The table does not contain the column '{0}'",
            "Pass": "The column '{0}' was selected"
        }
        return super()._run_keyword('SelectTableColumn', args, kwargs, result) # type: ignore
    
    @keyword('Select Table Row') # type: ignore
    def select_table_row(self, row_locator: str, table_number: int=1): # type: ignore
        """
        Select the specified table row(s) and return the corresponding row index(es).
        
        | ``row_locator`` | Either the row number or the contents of a cell in the row. If the cell only contains a number, it must be enclosed in double quotation marks. |
        | ``table_number`` | Specify which table: 1, 2, ... |
        
        Examples:
        
        *Select a single row*
        | ``Select Table Row    row_locator``
        
        *Select multiple rows in an ALV grid (a table with a toolbar)*
        | ``Select Table Row    1,2,3``
        
        *Hint*: Use the row number 0 to select the whole table.
        """

        args: list = [
            row_locator
        ]
        kwargs: dict = {
            "table_number": table_number
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "Exception": "The row could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html",
            "NoTable": "The window contains no table",
            "InvalidTable": "The window contains no table with index {0}.",
            "InvalidIndex": "The table does not have a row with index '{0}'",
            "NotFound": "The table does not contain a cell with the contents '{0}'",
            "Pass": "The row with the locator '{0}' was selected"
        }
        return super()._run_keyword('SelectTableRow', args, kwargs, result) # type: ignore
    
    @keyword('Select Text Field') # type: ignore
    def select_text_field(self, locator: str): # type: ignore
        """
        Select the text field specified by the locator or with the given content.
        
        | ``locator`` | Text field locators are documented in the keyword [#Fill Text Field|Fill Text Field]. |
        
        Examples:
        
        *Find the text field using a locator*
        | ``Select Text Field    locator``
        
        *Find the text field by its content*
        | ``Select Text Field    = content``
        """

        args: list = [
            locator
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The text field with the locator '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The text field with the locator '{0}' was selected.",
            "Exception": "The text field could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SelectTextField', args, kwargs, result) # type: ignore
    
    @keyword('Select Text') # type: ignore
    def select_text(self, locator: str): # type: ignore
        """
        Select the text specified by the locator.
        
        | ``locator`` | The locator used to find the text |
        
        Examples:
        
        *Text starting with a given substring*
        | ``Select Text    = substring``
        
        *Text following a label*
        | ``Select Text    Label``
        """

        args: list = [
            locator
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The text with the locator '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The text with the locator '{0}' was selected.",
            "Exception": "The text could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SelectText', args, kwargs, result) # type: ignore
    
    @keyword('Tick Checkbox') # type: ignore
    def tick_check_box(self, locator: str): # type: ignore
        """
        Tick the checkbox specified by the locator.
        
        | ``locator`` | A locator used to find the checkbox |
        
        Examples:
        
        *Checkbox with a label to its left or its right*
        | ``Tick Checkbox    label``
        
        *Checkbox with a label above it*
        | ``Tick Checkbox    @ label``
        
        *Checkbox at the intersection of a label to its left and a label above it*
        | ``Tick Checkbox    left label @ label above``
        """

        args: list = [
            locator
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The checkbox with the locator '{0}' could not be found. Hint: Check the spelling",
            "NotChangeable": "The checkbox with the locator '{0}' is disabled.",
            "Pass": "The checkbox with the locator '{0}' was ticked.",
            "Exception": "The checkbox could not be ticked. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('TickCheckBox', args, kwargs, result) # type: ignore
    
    @keyword('Untick Checkbox') # type: ignore
    def untick_check_box(self, locator: str): # type: ignore
        """
        Untick the checkbox specified by the locator.
        
        | ``locator`` | A locator used to find the checkbox |
        
        Examples:
        
        *Checkbox with a label to its left or its right*
        | ``Untick Checkbox    label``
        
        *Checkbox with a label above it*
        | ``Untick Checkbox    @ label``
        
        *Checkbox at the intersection of a label to its left and a label above it*
        | ``Untick Checkbox    left label @ label above``
        """

        args: list = [
            locator
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The checkbox with the locator '{0}' could not be found. Hint: Check the spelling",
            "NotChangeable": "The checkbox with the locator '{0}' is disabled.",
            "Pass": "The checkbox with the locator '{0}' was unticked.",
            "Exception": "The checkbox could not be unticked. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('UntickCheckBox', args, kwargs, result) # type: ignore
    
    @keyword('Tick Checkbox Cell') # type: ignore
    def tick_check_box_cell(self, row_locator: str, column: str, table_number: int=None): # type: ignore
        """
        Tick the checkbox cell at the intersection of the row and the column provided.
        
        | ``row_locator`` | Either the row number or the contents of a cell in the row. If the cell only contains a number, it must be enclosed in double quotation marks. |
        | ``column`` | Column title or tooltip |
        | ``table_number`` | Specify which table: 1, 2, ... |
        
        Examples:
        
        | ``Tick Checkbox Cell     row_locator    column``
        
        *Hint*: To tick the checkbox in the leftmost column with no title, select the row and press the "Enter" key.
        """

        args: list = [
            row_locator,
            column
        ]
        kwargs: dict = {
            "table_number": table_number
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The checkbox cell with the locator '{0}, {1}' could not be found. Hints: Check the spelling, maximize the SAP window",
            "NotChangeable": "The checkbox cell with the locator '{0}, {1}' is disabled.",
            "InvalidTable": "The window contains no table with index {0}.",
            "Pass": "The checkbox cell with the locator '{0}, {1}' was ticked.",
            "Exception": "The checkbox cell could not be ticked. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('TickCheckBoxCell', args, kwargs, result) # type: ignore
    
    @keyword('Untick Checkbox Cell') # type: ignore
    def untick_check_box_cell(self, row_locator: str, column: str, table_number: int=None): # type: ignore
        """
        Untick the checkbox cell at the intersection of the row and the column provided.
        
        | ``row_locator`` | Either the row number or the contents of a cell in the row. If the cell only contains a number, it must be enclosed in double quotation marks. |
        | ``column`` | Column title or tooltip |
        | ``table_number`` | Specify which table: 1, 2, ... |
        
        Examples:
        
        | ``Untick Checkbox Cell     row_locator    column``
        """

        args: list = [
            row_locator,
            column
        ]
        kwargs: dict = {
            "table_number": table_number
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "NotFound": "The checkbox cell with the locator '{0}, {1}' could not be found. Hints: Check the spelling, maximize the SAP window",
            "NotChangeable": "The checkbox cell with the locator '{0}, {1}' is disabled.",
            "InvalidTable": "The window contains no table with index {0}.",
            "Pass": "The checkbox cell with the locator '{0}, {1}' was unticked.",
            "Exception": "The checkbox cell could not be unticked. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('UntickCheckBoxCell', args, kwargs, result) # type: ignore
    
    @keyword('Get Window Title') # type: ignore
    def get_window_title(self): # type: ignore
        """
        Get the title of the currently active window.
        
        
        Examples:
        
        | ``${title}    Get Window Title``
        """

        args: list = [
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "Pass": "The title of the window was obtained.",
            "Exception": "The window title could not be read. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('GetWindowTitle', args, kwargs, result) # type: ignore
    
    @keyword('Get Window Text') # type: ignore
    def get_window_text(self): # type: ignore
        """
        Get the text message of the currently active window.
        
        
        Examples:
        
        | ``${text}    Get Window Text``
        """

        args: list = [
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "Pass": "The text message of the window was obtained.",
            "Exception": "The text message of the window could not be read. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('GetWindowText', args, kwargs, result) # type: ignore
    
    @keyword('Maximize window') # type: ignore
    def maximize_window(self): # type: ignore
        """
        Maximize the window in the foreground.
        
        
        Examples:
        
        | ``Maximize window``
        """

        args: list = [
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "No active SAP-Session. Call the keyword \"Connect To Server\" or \"Connect To Running SAP\" first.",
            "Pass": "The window in the foreground was maximized.",
            "Exception": "The window in the foreground could not be maximized. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('MaximizeWindow', args, kwargs, result) # type: ignore
    
    ROBOT_LIBRARY_SCOPE = 'GLOBAL'
    ROBOT_LIBRARY_VERSION = '2.23.0'