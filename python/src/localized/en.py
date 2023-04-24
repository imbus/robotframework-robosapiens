from schema import RoboSAPiens
from typing import Callable

Fstr = Callable[[str], str]

sap_error = 'SAP Error: {0}'
no_session = 'No existing SAP-Session. Call the keyword "Connect To Server" first.'
no_sap_gui = 'No open SAP GUI found. Call the keyword "Open SAP" first.'
no_gui_scripting = 'The scripting support is not activated. It must be activated in the Settings of SAP Logon.'
no_connection = 'No existing connection to an SAP server. Call the keyword "Connect to Server" first.'
no_server_scripting = 'Scripting is not activated on the server side. Please consult the documentation of RoboSAPiens.'
not_found: Fstr = lambda msg: f"{msg} Hint: Check the spelling"
exception: Fstr = lambda msg: f"*ERROR* {msg}" + "\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"

locales = {
    "DE": "German"
}

locales_bullet_list = lambda: "\n".join([f"- RoboSAPiens.{country} ({lang})" for country, lang in locales.items()])

lib: RoboSAPiens = {
    "doc": {
        "intro": f"""RoboSAPiens: SAP GUI-Automation for Humans

        In order to use this library three requirements must be satisfied:

        - .NET Runtime 7.0 x86 must be [https://dotnet.microsoft.com/en-us/download/dotnet/7.0|installed].
        
        - Scripting on the SAP Server must be [https://help.sap.com/saphelp_aii710/helpdata/en/ba/b8710932b8c64a9e8acf5b6f65e740/content.htm|activated].
        
        - Scripting Support must be [https://help.sap.com/docs/sap_gui_for_windows/63bd20104af84112973ad59590645513/7ddb7c9c4a4c43219a65eee4ca8db001.html?locale=en-US|activated] in the SAP GUI.

        This library implements the [https://robotframework.org/robotframework/latest/RobotFrameworkUserGuide.html#remote-library-interface|Remote Library Interface] of Robot Framework.
        That is, an HTTP server is started in the background and Robot Framework communicates with it. The standard port is 8270.
        A different port can be chosen when importing the library:
        | ``Library   RoboSAPiens    port=1234``

        This library is also available in the following languages:
        {locales_bullet_list()}
        """,
        "init": ""
    },
    "args": {
        "a1port": {
        "name": "port",
        "default": 8270,
        "doc": "Set the port of the HTTP server implementing the Remote interface."
        },
        "a2presenter_mode": {
        "name": "presenter_mode",
        "default": False,
        "doc": "Highlight each GUI element acted upon"
        }
    },
    "keywords": {
        "ActivateTab": {
            "name": "Select Tab",
            "args": {
                "Reitername": {
                "name": "tabname",
                "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The tab '{0}' could not be found"),
                "SapError": sap_error,
                "Pass": "The tab {0} was selected",
                "Exception": exception("The tab could not be selected. {0}")
            },
            "doc": "Select the tab with the name provided.\n\n| ``Select Tab    tabname``"
        },
        "OpenSAP": {
            "name": "Open SAP",
            "args": {
                "Pfad": {
                "name": "path",
                "spec": {}
                }
            },
            "result": {
                "Pass": "The SAP GUI was opened.",
                "SAPNotStarted": "The SAP GUI could not be opened. Verify that the path is correct.",
                "Exception": exception("The SAP GUI could not be opened. {0}")
            },
            "doc": r"Open the SAP GUI. The standard path is\n\n| ``C:\\Program Files (x86)\\SAP\\FrontEnd\\SAPgui\\saplogon.exe``"
        },
        "CloseConnection": {
            "name": "Disconnect from Server",
            "args": {},
            "result": {
                "NoSapGui": no_sap_gui,
                "NoGuiScripting": no_gui_scripting,
                "NoConnection": no_connection,
                "NoSession": no_session,
                "Pass": "Disconnected from the server.",
                "Exception": exception("Could not disconnect from the server. {0}")
            },
            "doc": "Terminate the connection to the SAP server."
        },
        "CloseSAP": {
            "name": "Close SAP",
            "args": {},
            "result": {
                "NoSapGui": no_sap_gui,
                "Pass": "The SAP GUI was closed."
            },
            "doc": "Close the SAP GUI"
        },
        "ExportTree": {
            "name": "Export Function Tree",
            "args": {
                "Dateipfad": {
                "name": "filepath",
                "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": "The window contains no tree element",
                "Pass": "The function tree was exported to {0}",
                "Exception": exception("The function tree could not be exported. {0}")
            },
            "doc": "Export the function tree in JSON format to the file provided.\n\n| ``Export Function Tree     filepath``"
        },
        "AttachToRunningSAP": {
            "name": "Connect to Running SAP",
            "args": {},
            "result": {
                "NoSapGui": no_sap_gui,
                "NoGuiScripting": no_gui_scripting,
                "NoConnection": no_connection,
                "NoServerScripting": no_server_scripting,
                "Pass": "Connected to a running SAP instance.",
                "Exception": exception("Could not connect to a running SAP instance. {0}")
            },
            "doc": "Connect to a running SAP instance and take control of it."
        },
        "ConnectToServer": {
            "name": "Connect to Server",
            "args": {
                "Servername": {
                "name": "servername",
                "spec": {}
                }
            },
            "result": {
                "NoSapGui": no_sap_gui,
                "NoGuiScripting": no_gui_scripting,
                "Pass": "Connected to the server {0}",
                "SapError": sap_error,
                "NoServerScripting": no_server_scripting,
                "Exception": exception("Could not establish the connection. {0}")
            },
            "doc": "Connect to the SAP Server provided.\n\n| ``Connect to Server    servername``"
        },
        "DoubleClickCell": {
            "name": "Double-click Cell",
            "args": {
                "a1Zeilennummer_oder_Zellinhalt": {
                "name": "row_number_or_cell_content",
                "spec": {}
                },
                "a2Spaltentitel": {
                "name": "column",
                "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The cell with the locator '{0}, {1}' could not be found."),
                "Pass": "The cell with the locator {0} was double-clicked.",
                "Exception": exception("The cell could not be double-clicked. {0}")
            },
            "doc": "Double-click the cell at the intersection of the row and the column provided.\n\n| ``Double-click Cell     row_locator     column``\nrow_locator: either the row number or the content of a cell in the row."
        },
        "DoubleClickTextField": {
            "name": "Double-click Text Field",
            "args": {
                "Inhalt": {
                "name": "content",
                "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The text field with the content '{0}' could not be found"),
                "Pass": "The text field with the content '{0}' was double-clicked.",
                "Exception": exception("The text field could not be double-clicked. {0}")
            },
            "doc": "Double click the text field with the content provided.\n\n| ``Double-click Text Field     Content``"
        },
        "ExecuteTransaction": {
            "name": "Execute Transaction",
            "args": {
                "T_Code": {
                "name": "T_Code",
                "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "Pass": "The transaction with T-Code {0} was executed.",
                "Exception": exception("The transaction could not be executed. {0}")
            },
            "doc": "Execute the transaction with the given T-Code.\n\n| ``Execute Transaction    T_Code``"
        },
        "ExportForm": {
            "name": "Export Dynpro",
            "args": {
                "a1Name": {
                "name": "name",
                "spec": {}
                },
                "a2Verzeichnis": {
                "name": "directory",
                "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "Pass": "The Dynpro was exported to the CSV file {0} and the PNG image {1}",
                "Exception": exception("The Dynpro could not be exported. {0}")
            },
            "doc": "Write all texts in the Dynpro to a CSV file. Also a screenshot will be saved in PNG format.\n\n| ``Export Dynpro     name     directory``\ndirectory: Absolute path to the directory where the files will be saved."
        },
        "FillTableCell": {
            "name": "Fill Cell",
            "args": {
                "a1Zeilennummer_oder_Zellinhalt": {
                "name": "row_number_or_content",
                "spec": {}
                },
                "a2Spaltentitel_Gleich_Inhalt": {
                "name": "column_equals_content",
                "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "InvalidFormat": "The format of the second parameter must be 'column = content'",
                "NotFound": not_found("The cell with the locator '{0}, {1}' could not be found"),
                "Pass": "The cell with the locator {0} was filled.",
                "Exception": exception("The cell could not be filled. {0}")
            },
            "doc": "Fill the cell at the intersection of the row and the column specified with the content provided.\n\n| ``Fill Cell     row     column = content``\nrow: either the row number or the contents of a cell in the row.\n\n*Hint*: Some cells can be filled using the keyword 'Fill Text Field' providing as locator the description obtained by selecting the cell and pressing F1."
        },
        "FillTextField": {
            "name": "Fill Text Field",
            "args": {
                "a1Beschriftung_oder_Positionsgeber": {
                "name": "locator",
                "spec": {}
                },
                "a2Inhalt": {
                "name": "content",
                "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The text field with the locator '{0}' could not be found."),
                "Pass": "The text field with the locator '{0}' was filled.",
                "Exception": exception("The text field could not be filled. {0}")
            },
            "doc": "Fill the text Field specified by the locator with the content provided.\n\n*Text field with a label to its left*\n| ``Fill Text Field    label    content``\n*Text field with a label above*\n| ``Fill Text Field    @ label    content``\n*Text field at the intersection of a label to its left and a label above it (including a heading)*\n| ``Fill Text Field    label to its left @ label above it    content``\n*Text field without label below a text field with a label (e.g. an address line)*\n| ``Fill Text Field    position (1,2,..) @ label    content``\n\n*Text field without a label to the right of a text field with a label*\n| ``Fill Text Field    label @ position (1,2,..)    content``\n\n*Text field with a non-unique label to the right of a text field with a label*\n| ``Fill Text Field    left label >> right label    content``\n\n*Hint*: The description obtained by selecting a text field and pressing F1 can also be used as label."
        },
        "PushButton": {
            "name": "Push Button",
            "args": {
                "Name_oder_Kurzinfo": {
                "name": "name_or_tooltip",
                "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "SapError": sap_error,
                "NotFound": not_found("The button '{0}' could not be found."),
                "Pass": "The button '{0}' was pushed.",
                "Exception": exception("The button could not be pushed. {0}")
            },
            "doc": "Push the button with the given name or tooltip.\n\n| ``Push Button    name or tooltip``"
        },
        "PushButtonCell": {
            "name": "Push Button Cell",
            "args": {
                "a1Zeilennummer_oder_Name_oder_Kurzinfo": {
                "name": "row_index_label_tooltip",
                "spec": {}
                },
                "a2Spaltentitel": {
                "name": "column",
                "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The button cell with the locator '{0}, {1}' could not be found."),
                "Pass": "The button cell with the locator '{0}' was pushed.",
                "Exception": exception("The button cell could not be pushed. {0}")
            },
            "doc": "Push the button cell located at the intersection of the row and column provided.\n\n| ``Push Button Cell     row_locator     column``\nrow_locator: Row number, label or tooltip."
        },
        "ReadTextField": {
            "name": "Read Text Field",
            "args": {
                "Beschriftung_oder_Positionsgeber": {
                "name": "locator",
                "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The text field with the locator '{0}' could not be found."),
                "Pass": "The text field with the locator '{0}' was read.",
                "Exception": exception("The text field could not be read. {0}")
            },
            "doc": "Read the contents of the text field specified by the locator.\n\n*Text field with a label to its left*\n| ``Read Text Field    label``\n*Text field with a label above it*\n| ``Read Text Field    @ label``\n*Text field at the intersection of a label to its left and a label above it*\n| ``Read Text Field    left label @ label above``\n*Text field whose content starts with a given text*\n| ``Read Text Field    = text``"
        },
        "ReadText": {
            "name": "Read Text",
            "args": {
                "Inhalt": {
                "name": "content",
                "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("No text with the locator '{0}' was found."),
                "Pass": "A text with the locator '{0}' was read.",
                "Exception": exception("The text could not be read. {0}")
            },
            "doc": "Read the text specified by the locator.\n\n*Text starting with a given substring*\n| ``Read Text    = substring``\n*Text following a label*\n| ``Read Text    Label``"
        },
        "ReadTableCell": {
            "name": "Read Cell",
            "args": {
                "a1Zeilennummer_oder_Zellinhalt": {
                "name": "row_number_or_content",
                "spec": {}
                },
                "a2Spaltentitel": {
                "name": "column",
                "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The cell with the locator '{0}, {1}' could not be found."),
                "Pass": "The cell with the locator '{0}' was read.",
                "Exception": exception("The cell could not be read. {0}")
            },
            "doc": "Read the contents of the cell at the intersection of the row and column provided.\n\n| ``Read Cell     row_locator     column``\nrow_locator: either the row number or the contents of a cell in the row."
        },
        "SaveScreenshot": {
            "name": "Save Screenshot",
            "args": {
                "Aufnahmenverzeichnis": {
                "name": "filepath",
                "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "UNCPath": "UNC paths (i.e. beginning with \\) are not allowed",
                "NoAbsPath": "The path is not an absolute path.",
                "Pass": "The screenshot was saved in {0}.",
                "Exception": exception("The screenshot could not be saved. {0}")
            },
            "doc": "Save a screenshot of the current window in the file provided.\n\n| ``Save Screenshot     filepath``\nfilepath: Absolute path to a .png file."
        },
        "SelectCell": {
            "name": "Select Cell",
            "args": {
                "a1Zeilennummer_oder_Zellinhalt": {
                "name": "row_number_or_content",
                "spec": {}
                },
                "a2Spaltentitel": {
                "name": "column",
                "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The cell with the locator '{0}, {1}' could not be found."),
                "Pass": "The cell with the locator '{0}' was selected.",
                "Exception": exception("The cell could not be selected. {0}")
            },
            "doc": "Select the cell at the intersection of the row and column provided.\n\n| ``Select Cell     row_locator     column``\nrow_locator: either the row number or the contents of a cell in the row."
        },
        "SelectComboBoxEntry": {
            "name": "Select Dropdown Menu Entry",
            "args": {
                "a1Name": {
                "name": "dropdown_menu",
                "spec": {}
                },
                "a2Eintrag": {
                "name": "entry",
                "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The dropdown menu '{0}' could not be found."),
                "EntryNotFound": not_found("In the dropdown menu '{0}' no entry '{1}' could be found."),
                "Pass": "In the dropdown menu '{0}' the entry '{1}' was selected.",
                "Exception": exception("The entry could not be selected. {0}")
            },
            "doc": "Select the specified entry from the dropdown menu provided.\n\n| ``Select Dropdown Menu Entry   dropdown menu    entry``"
        },
        "SelectRadioButton": {
            "name": "Select Radio Button",
            "args": {
                "Beschriftung_oder_Positionsgeber": {
                "name": "locator",
                "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The radio button with locator '{0}' could not be found."),
                "Pass": "The radio button with locator '{0}' was selected.",
                "Exception": exception("The radio button could not be selected. {0}")
            },
            "doc": "Select the radio button specified by the locator.\n\n*Radio button with a label to its left or its right*\n| ``Select Radio Button    label``\n*Radio button with a label above it*\n| ``Select Radio Button    @ label``\n*Radio button at the intersection of a label to its left or its right and a label above it*\n| ``Select Radio Button    left or right label @ label above``"
        },
        "SelectTextField": {
            "name": "Select Text Field",
            "args": {
                "Beschriftungen_oder_Inhalt": {
                "name": "locator",
                "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The text field with the locator '{0}' could not be found."),
                "Pass": "The text field with the locator '{0}' was selected.",
                "Exception": exception("The text field could not be selected. {0}")
            },
            "doc": "Select the text field specified by the locator.\n\n*Text field with a label to its left*\n| ``Select Text Field    label``\n*Text field with a label above it*\n| ``Select Text Field    @ label``\n*Text field at the intersection of a label to its left and a label above it*\n| ``Select Text Field    left label @ label above``\n*Text field whose content starts with the given text*\n| ``Select Text Field    = text``"
        },
        "SelectTextLine": {
            "name": "Select Text Line",
            "args": {
                "Inhalt": {
                "name": "content",
                "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The text line starting with '{0}' could not be found."),
                "Pass": "The text line starting with '{0}' was selected.",
                "Exception": exception("The text line could not be selected. {0}")
            },
            "doc": "Select the text line starting with the given content.\n| ``Select Text Line    content``"
        },
        "TickCheckBox": {
            "name": "Tick Checkbox",
            "args": {
                "Beschriftung_oder_Positionsgeber": {
                "name": "locator",
                "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The checkbox with the locator '{0}' could not be found."),
                "Pass": "The checkbox with the locator '{0}' was ticked.",
                "Exception": exception("The checkbox could not be ticked. {0}")
            },
            "doc": "Tick the checkbox specified by the locator.\n\n*Checkbox with a label to its left or its right*\n| ``Tick Checkbox    label``\n*Checkbox with a label above it*\n| ``Tick Checkbox    @ label``\n*Checkbox at the intersection of a label to its left and a label above it*\n| ``Tick Checkbox    left label @ label above``"
        },
        "UntickCheckBox": {
            "name": "Untick Checkbox",
            "args": {
                "Beschriftung_oder_Positionsgeber": {
                "name": "locator",
                "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The checkbox with the locator '{0}' could not be found."),
                "Pass": "The checkbox with the locator '{0}' was unticked.",
                "Exception": exception("The checkbox could not be unticked. {0}")
            },
            "doc": "Untick the checkbox specified by the locator.\n\n*Checkbox with a label to its left or its right*\n| ``Untick Checkbox    label``\n*Checkbox with a label above it*\n| ``Untick Checkbox    @ label``\n*Checkbox at the intersection of a label to its left and a label above it*\n| ``Untick Checkbox    left label @ label above``"
        },
        "TickCheckBoxCell": {
            "name": "Tick Checkbox Cell",
            "args": {
                "a1Zeilennummer": {
                "name": "row_number",
                "spec": {}
                },
                "a2Spaltentitel": {
                "name": "column",
                "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The checkbox cell with the locator '{0}, {1}' coud not be found."),
                "Pass": "The checkbox cell with the locator '{0}' was ticked.",
                "Exception": exception("The checkbox cell could not be ticked. {0}")
            },
            "doc": "Tick the checkbox cell at the intersection of the row and the column provided.\n\n| ``Tick Checkbox Cell     row number     column``"
        },
        "GetWindowTitle": {
            "name": "Get Window Title",
            "args": {},
            "result": {
                "NoSession": no_session,
                "Pass": "The title of the window was obtained."
            },
            "doc": "Get the title of the window in the foreground.\n\n| ``${Title}    Get Window Title``"
        },
        "GetWindowText": {
            "name": "Get Window Text",
            "args": {},
            "result": {
                "NoSession": no_session,
                "Pass": "The text message of the window was obtained."
            },
            "doc": "Get the text message of the window in the foreground.\n\n| ``${Text}    Get Window Text``"
        }
    },
    "specs": {}
}
