from robot.api.deco import keyword
from RoboSAPiens.client import RoboSAPiensClient

class RoboSAPiens(RoboSAPiensClient):
    """
    RoboSAPiens: SAP GUI-Automation for Humans
    
    In order to use this library three requirements must be satisfied:
    
    - .NET Runtime 7.0 x86 must be [https://dotnet.microsoft.com/en-us/download/dotnet/7.0|installed].
    
    - Scripting on the SAP Server must be [https://help.sap.com/saphelp_aii710/helpdata/en/ba/b8710932b8c64a9e8acf5b6f65e740/content.htm|activated].
    
    - Scripting Support must be [https://help.sap.com/docs/sap_gui_for_windows/63bd20104af84112973ad59590645513/7ddb7c9c4a4c43219a65eee4ca8db001.html?locale=en-US|activated] in the SAP GUI.
    
    This library implements the [https://robotframework.org/robotframework/latest/RobotFrameworkUserGuide.html#remote-library-interface|Remote Library Interface] of Robot Framework.
    That is, an HTTP server is started in the background and Robot Framework communicates with it. The standard port is 8270.
    A different port can be chosen when importing the library:
    | ``Library   RoboSAPiens    port=1234``
    
    This library is also available in the following languages:
    - RoboSAPiens.DE (German)
    
    """
    
    def __init__(self, port: int=8270, presenter_mode: bool=False):
        """
        *port*: Set the port of the HTTP server implementing the Remote interface.
        
        *presenter_mode*: Highlight each GUI element acted upon
        """
        
        args = {
            'port': port,
            'presenter_mode': presenter_mode,
        }
        
        super().__init__(args)
    

    @keyword('Select Tab') # type: ignore
    def ActivateTab(self, tabname: str): # type: ignore
        """
        Select the tab with the name provided.
        
        | ``Select Tab    tabname``
        """
        
        args = {
            'Reitername': tabname,
        }
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The tab '{0}' could not be found Hint: Check the spelling",
            "SapError": "SAP Error: {0}",
            "Pass": "The tab {0} was selected",
            "Exception": "*ERROR* The tab could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ActivateTab', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Open SAP') # type: ignore
    def OpenSAP(self, path: str): # type: ignore
        """
        Open the SAP GUI. The standard path is\n\n| ``C:\\Program Files (x86)\\SAP\\FrontEnd\\SAPgui\\saplogon.exe``
        """
        
        args = {
            'Pfad': path,
        }
        
        result = {
            "Pass": "The SAP GUI was opened.",
            "SAPNotStarted": "The SAP GUI could not be opened. Verify that the path is correct.",
            "Exception": "*ERROR* The SAP GUI could not be opened. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('OpenSAP', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Disconnect from Server') # type: ignore
    def CloseConnection(self, ): # type: ignore
        """
        Terminate the connection to the SAP server.
        """
        
        args = {
        }
        
        result = {
            "NoSapGui": "No open SAP GUI found. Call the keyword \"Open SAP\" first.",
            "NoGuiScripting": "The scripting support is not activated. It must be activated in the Settings of SAP Logon.",
            "NoConnection": "No existing connection to an SAP server. Call the keyword \"Connect to Server\" first.",
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "Pass": "Disconnected from the server.",
            "Exception": "*ERROR* Could not disconnect from the server. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('CloseConnection', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Close SAP') # type: ignore
    def CloseSAP(self, ): # type: ignore
        """
        Close the SAP GUI
        """
        
        args = {
        }
        
        result = {
            "NoSapGui": "No open SAP GUI found. Call the keyword \"Open SAP\" first.",
            "Pass": "The SAP GUI was closed."
        }
        return super()._run_keyword('CloseSAP', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Export Function Tree') # type: ignore
    def ExportTree(self, filepath: str): # type: ignore
        """
        Export the function tree in JSON format to the file provided.
        
        | ``Export Function Tree     filepath``
        """
        
        args = {
            'Dateipfad': filepath,
        }
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The window contains no tree element",
            "Pass": "The function tree was exported to {0}",
            "Exception": "*ERROR* The function tree could not be exported. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ExportTree', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Connect to Running SAP') # type: ignore
    def AttachToRunningSAP(self, ): # type: ignore
        """
        Connect to a running SAP instance and take control of it.
        """
        
        args = {
        }
        
        result = {
            "NoSapGui": "No open SAP GUI found. Call the keyword \"Open SAP\" first.",
            "NoGuiScripting": "The scripting support is not activated. It must be activated in the Settings of SAP Logon.",
            "NoConnection": "No existing connection to an SAP server. Call the keyword \"Connect to Server\" first.",
            "NoServerScripting": "Scripting is not activated on the server side. Please consult the documentation of RoboSAPiens.",
            "Pass": "Connected to a running SAP instance.",
            "Exception": "*ERROR* Could not connect to a running SAP instance. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('AttachToRunningSAP', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Connect to Server') # type: ignore
    def ConnectToServer(self, servername: str): # type: ignore
        """
        Connect to the SAP Server provided.
        
        | ``Connect to Server    servername``
        """
        
        args = {
            'Servername': servername,
        }
        
        result = {
            "NoSapGui": "No open SAP GUI found. Call the keyword \"Open SAP\" first.",
            "NoGuiScripting": "The scripting support is not activated. It must be activated in the Settings of SAP Logon.",
            "Pass": "Connected to the server {0}",
            "SapError": "SAP Error: {0}",
            "NoServerScripting": "Scripting is not activated on the server side. Please consult the documentation of RoboSAPiens.",
            "Exception": "*ERROR* Could not establish the connection. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ConnectToServer', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Double-click Cell') # type: ignore
    def DoubleClickCell(self, row_number_or_cell_content: str, column: str): # type: ignore
        """
        Double-click the cell at the intersection of the row and the column provided.
        
        | ``Double-click Cell     row_locator     column``
        row_locator: either the row number or the content of a cell in the row.
        """
        
        args = {
            'Zeilennummer_oder_Zellinhalt': row_number_or_cell_content,
            'Spaltentitel': column,
        }
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The cell with the locator '{0}, {1}' could not be found. Hint: Check the spelling",
            "Pass": "The cell with the locator {0} was double-clicked.",
            "Exception": "*ERROR* The cell could not be double-clicked. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('DoubleClickCell', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Double-click Text Field') # type: ignore
    def DoubleClickTextField(self, content: str): # type: ignore
        """
        Double click the text field with the content provided.
        
        | ``Double-click Text Field     Content``
        """
        
        args = {
            'Inhalt': content,
        }
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The text field with the content '{0}' could not be found Hint: Check the spelling",
            "Pass": "The text field with the content '{0}' was double-clicked.",
            "Exception": "*ERROR* The text field could not be double-clicked. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('DoubleClickTextField', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Execute Transaction') # type: ignore
    def ExecuteTransaction(self, T_Code: str): # type: ignore
        """
        Execute the transaction with the given T-Code.
        
        | ``Execute Transaction    T_Code``
        """
        
        args = {
            'T_Code': T_Code,
        }
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "Pass": "The transaction with T-Code {0} was executed.",
            "Exception": "*ERROR* The transaction could not be executed. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ExecuteTransaction', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Export Dynpro') # type: ignore
    def ExportForm(self, name: str, directory: str): # type: ignore
        """
        Write all texts in the Dynpro to a CSV file. Also a screenshot will be saved in PNG format.
        
        | ``Export Dynpro     name     directory``
        directory: Absolute path to the directory where the files will be saved.
        """
        
        args = {
            'Name': name,
            'Verzeichnis': directory,
        }
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "Pass": "The Dynpro was exported to the CSV file {0} and the PNG image {1}",
            "Exception": "*ERROR* The Dynpro could not be exported. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ExportForm', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Fill Cell') # type: ignore
    def FillTableCell(self, row_number_or_content: str, column_equals_content: str): # type: ignore
        """
        Fill the cell at the intersection of the row and the column specified with the content provided.
        
        | ``Fill Cell     row     column = content``
        row: either the row number or the contents of a cell in the row.
        
        *Hint*: Some cells can be filled using the keyword 'Fill Text Field' providing as locator the description obtained by selecting the cell and pressing F1.
        """
        
        args = {
            'Zeilennummer_oder_Zellinhalt': row_number_or_content,
            'Spaltentitel_Gleich_Inhalt': column_equals_content,
        }
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "InvalidFormat": "The format of the second parameter must be 'column = content'",
            "NotFound": "The cell with the locator '{0}, {1}' could not be found Hint: Check the spelling",
            "Pass": "The cell with the locator {0} was filled.",
            "Exception": "*ERROR* The cell could not be filled. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('FillTableCell', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Fill Text Field') # type: ignore
    def FillTextField(self, locator: str, content: str): # type: ignore
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
        
        args = {
            'Beschriftung_oder_Positionsgeber': locator,
            'Inhalt': content,
        }
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The text field with the locator '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The text field with the locator '{0}' was filled.",
            "Exception": "*ERROR* The text field could not be filled. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('FillTextField', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Push Button') # type: ignore
    def PushButton(self, name_or_tooltip: str): # type: ignore
        """
        Push the button with the given name or tooltip.
        
        | ``Push Button    name or tooltip``
        """
        
        args = {
            'Name_oder_Kurzinfo': name_or_tooltip,
        }
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "SapError": "SAP Error: {0}",
            "NotFound": "The button '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The button '{0}' was pushed.",
            "Exception": "*ERROR* The button could not be pushed. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('PushButton', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Push Button Cell') # type: ignore
    def PushButtonCell(self, row_index_label_tooltip: str, column: str): # type: ignore
        """
        Push the button cell located at the intersection of the row and column provided.
        
        | ``Push Button Cell     row_locator     column``
        row_locator: Row number, label or tooltip.
        """
        
        args = {
            'Zeilennummer_oder_Name_oder_Kurzinfo': row_index_label_tooltip,
            'Spaltentitel': column,
        }
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The button cell with the locator '{0}, {1}' could not be found. Hint: Check the spelling",
            "Pass": "The button cell with the locator '{0}' was pushed.",
            "Exception": "*ERROR* The button cell could not be pushed. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('PushButtonCell', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Read Text Field') # type: ignore
    def ReadTextField(self, locator: str): # type: ignore
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
        
        args = {
            'Beschriftung_oder_Positionsgeber': locator,
        }
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The text field with the locator '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The text field with the locator '{0}' was read.",
            "Exception": "*ERROR* The text field could not be read. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ReadTextField', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Read Text') # type: ignore
    def ReadText(self, content: str): # type: ignore
        """
        Read the text specified by the locator.
        
        *Text starting with a given substring*
        | ``Read Text    = substring``
        *Text following a label*
        | ``Read Text    Label``
        """
        
        args = {
            'Inhalt': content,
        }
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "No text with the locator '{0}' was found. Hint: Check the spelling",
            "Pass": "A text with the locator '{0}' was read.",
            "Exception": "*ERROR* The text could not be read. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ReadText', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Read Cell') # type: ignore
    def ReadTableCell(self, row_number_or_content: str, column: str): # type: ignore
        """
        Read the contents of the cell at the intersection of the row and column provided.
        
        | ``Read Cell     row_locator     column``
        row_locator: either the row number or the contents of a cell in the row.
        """
        
        args = {
            'Zeilennummer_oder_Zellinhalt': row_number_or_content,
            'Spaltentitel': column,
        }
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The cell with the locator '{0}, {1}' could not be found. Hint: Check the spelling",
            "Pass": "The cell with the locator '{0}' was read.",
            "Exception": "*ERROR* The cell could not be read. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('ReadTableCell', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Save Screenshot') # type: ignore
    def SaveScreenshot(self, filepath: str): # type: ignore
        """
        Save a screenshot of the current window in the file provided.
        
        | ``Save Screenshot     filepath``
        filepath: Absolute path to a .png file.
        """
        
        args = {
            'Aufnahmenverzeichnis': filepath,
        }
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "UNCPath": "UNC paths (i.e. beginning with \\) are not allowed",
            "NoAbsPath": "The path is not an absolute path.",
            "Pass": "The screenshot was saved in {0}.",
            "Exception": "*ERROR* The screenshot could not be saved. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SaveScreenshot', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Select Cell') # type: ignore
    def SelectCell(self, row_number_or_content: str, column: str): # type: ignore
        """
        Select the cell at the intersection of the row and column provided.
        
        | ``Select Cell     row_locator     column``
        row_locator: either the row number or the contents of a cell in the row.
        """
        
        args = {
            'Zeilennummer_oder_Zellinhalt': row_number_or_content,
            'Spaltentitel': column,
        }
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The cell with the locator '{0}, {1}' could not be found. Hint: Check the spelling",
            "Pass": "The cell with the locator '{0}' was selected.",
            "Exception": "*ERROR* The cell could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SelectCell', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Select Dropdown Menu Entry') # type: ignore
    def SelectComboBoxEntry(self, dropdown_menu: str, entry: str): # type: ignore
        """
        Select the specified entry from the dropdown menu provided.
        
        | ``Select Dropdown Menu Entry   dropdown menu    entry``
        """
        
        args = {
            'Name': dropdown_menu,
            'Eintrag': entry,
        }
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The dropdown menu '{0}' could not be found. Hint: Check the spelling",
            "EntryNotFound": "In the dropdown menu '{0}' no entry '{1}' could be found. Hint: Check the spelling",
            "Pass": "In the dropdown menu '{0}' the entry '{1}' was selected.",
            "Exception": "*ERROR* The entry could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SelectComboBoxEntry', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Select Radio Button') # type: ignore
    def SelectRadioButton(self, locator: str): # type: ignore
        """
        Select the radio button specified by the locator.
        
        *Radio button with a label to its left or its right*
        | ``Select Radio Button    label``
        *Radio button with a label above it*
        | ``Select Radio Button    @ label``
        *Radio button at the intersection of a label to its left or its right and a label above it*
        | ``Select Radio Button    left or right label @ label above``
        """
        
        args = {
            'Beschriftung_oder_Positionsgeber': locator,
        }
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The radio button with locator '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The radio button with locator '{0}' was selected.",
            "Exception": "*ERROR* The radio button could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SelectRadioButton', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Select Text Field') # type: ignore
    def SelectTextField(self, locator: str): # type: ignore
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
        
        args = {
            'Beschriftungen_oder_Inhalt': locator,
        }
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The text field with the locator '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The text field with the locator '{0}' was selected.",
            "Exception": "*ERROR* The text field could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SelectTextField', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Select Text Line') # type: ignore
    def SelectTextLine(self, content: str): # type: ignore
        """
        Select the text line starting with the given content.
        | ``Select Text Line    content``
        """
        
        args = {
            'Inhalt': content,
        }
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The text line starting with '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The text line starting with '{0}' was selected.",
            "Exception": "*ERROR* The text line could not be selected. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('SelectTextLine', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Tick Checkbox') # type: ignore
    def TickCheckBox(self, locator: str): # type: ignore
        """
        Tick the checkbox specified by the locator.
        
        *Checkbox with a label to its left or its right*
        | ``Tick Checkbox    label``
        *Checkbox with a label above it*
        | ``Tick Checkbox    @ label``
        *Checkbox at the intersection of a label to its left and a label above it*
        | ``Tick Checkbox    left label @ label above``
        """
        
        args = {
            'Beschriftung_oder_Positionsgeber': locator,
        }
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The checkbox with the locator '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The checkbox with the locator '{0}' was ticked.",
            "Exception": "*ERROR* The checkbox could not be ticked. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('TickCheckBox', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Untick Checkbox') # type: ignore
    def UntickCheckBox(self, locator: str): # type: ignore
        """
        Untick the checkbox specified by the locator.
        
        *Checkbox with a label to its left or its right*
        | ``Untick Checkbox    label``
        *Checkbox with a label above it*
        | ``Untick Checkbox    @ label``
        *Checkbox at the intersection of a label to its left and a label above it*
        | ``Untick Checkbox    left label @ label above``
        """
        
        args = {
            'Beschriftung_oder_Positionsgeber': locator,
        }
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The checkbox with the locator '{0}' could not be found. Hint: Check the spelling",
            "Pass": "The checkbox with the locator '{0}' was unticked.",
            "Exception": "*ERROR* The checkbox could not be unticked. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('UntickCheckBox', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Tick Checkbox Cell') # type: ignore
    def TickCheckBoxCell(self, row_number: str, column: str): # type: ignore
        """
        Tick the checkbox cell at the intersection of the row and the column provided.
        
        | ``Tick Checkbox Cell     row number     column``
        """
        
        args = {
            'Zeilennummer': row_number,
            'Spaltentitel': column,
        }
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "NotFound": "The checkbox cell with the locator '{0}, {1}' coud not be found. Hint: Check the spelling",
            "Pass": "The checkbox cell with the locator '{0}' was ticked.",
            "Exception": "*ERROR* The checkbox cell could not be ticked. {0}\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
        }
        return super()._run_keyword('TickCheckBoxCell', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Get Window Title') # type: ignore
    def GetWindowTitle(self, ): # type: ignore
        """
        Get the title of the window in the foreground.
        
        | ``${Title}    Get Window Title``
        """
        
        args = {
        }
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "Pass": "The title of the window was obtained."
        }
        return super()._run_keyword('GetWindowTitle', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Get Window Text') # type: ignore
    def GetWindowText(self, ): # type: ignore
        """
        Get the text message of the window in the foreground.
        
        | ``${Text}    Get Window Text``
        """
        
        args = {
        }
        
        result = {
            "NoSession": "No existing SAP-Session. Call the keyword \"Connect To Server\" first.",
            "Pass": "The text message of the window was obtained."
        }
        return super()._run_keyword('GetWindowText', list(args.values()), dict(), result) # type: ignore
    
    ROBOT_LIBRARY_SCOPE = 'GLOBAL'
    ROBOT_LIBRARY_VERSION = '1.1.3'