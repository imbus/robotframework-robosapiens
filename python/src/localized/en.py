from typing import Callable
from schema import RoboSAPiens

Fstr = Callable[[str], str]

sap_error = 'SAP Error: {0}'
no_session = 'No existing SAP-Session. Call the keyword "Connect To Server" first.'
no_sap_gui = 'No open SAP GUI found. Call the keyword "Open SAP" first.'
no_gui_scripting = 'The scripting support is not activated. It must be activated in the Settings of SAP Logon.'
no_connection = 'No existing connection to an SAP server. Call the keyword "Connect to Server" first.'
no_server_scripting = 'Scripting is not activated on the server side. Please consult the documentation of RoboSAPiens.'
not_found: Fstr = lambda msg: f"{msg} Hint: Check the spelling"
exception: Fstr = lambda msg: f"{msg}" + "\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
row_locator = 'row_locator: either the row number or the contents of a cell in the row. If the cell only contains a number, it must be enclosed in double quotation marks.'

HLabel = "::label"
VLabel = ":@:label above"
HLabelVLabel = "label:@:label above"
HLabelHLabel = "left label:>>:right label"
HIndexVLabel = "index:@:label above"
HLabelVIndex = "label:@:index"
Content = ":=:content"
Column = "column"

locales = {
    "DE": "German"
}

def locales_bullet_list() -> str:
    return "\n".join([f"- RoboSAPiens.{country} ({lang})" for country, lang in locales.items()])

lib: RoboSAPiens = {
    "doc": {
        "intro": f"""RoboSAPiens: SAP GUI-Automation for Humans

        In order to use this library the following requirements must be satisfied:

        - Scripting on the SAP Server must be [https://help.sap.com/saphelp_aii710/helpdata/en/ba/b8710932b8c64a9e8acf5b6f65e740/content.htm|activated].
        
        - Scripting Support must be [https://help.sap.com/docs/sap_gui_for_windows/63bd20104af84112973ad59590645513/7ddb7c9c4a4c43219a65eee4ca8db001.html?locale=en-US|activated] in the SAP GUI.

        This library is also available in the following languages:
        {locales_bullet_list()}
        """,
        "init": ""
    },
    "args": {
        "presenter_mode": {
            "name": "presenter_mode",
            "default": False,
            "doc": "Highlight each GUI element acted upon"
        }
    },
    "keywords": {
        "ActivateTab": {
            "name": "Select Tab",
            "args": {
                "tab": {
                    "name": "tab_name",
                    "spec": {},
                    "optional": False
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The tab '{0}' could not be found"),
                "SapError": sap_error,
                "Pass": "The tab {0} was selected",
                "Exception": exception("The tab could not be selected. {0}")
            },
            "doc": """
            Select the tab with the name provided.
            
            | ``Select Tab    tab_name``
            """
        },
        "OpenSap": {
            "name": "Open SAP",
            "args": {
                "path": {
                    "name": "path",
                    "spec": {},
                    "optional": False
                }
            },
            "result": {
                "Pass": "The SAP GUI was opened.",
                "NoGuiScripting": no_gui_scripting,
                "SAPAlreadyRunning": "The SAP GUI is already running. It must be closed before calling this keyword.",
                "SAPNotStarted": "The SAP GUI could not be opened. Verify that the path is correct.",
                "Exception": exception("The SAP GUI could not be opened. {0}")
            },
            "doc": r"""
            Open the SAP GUI. The standard path is
            
            | ``C:\\Program Files (x86)\\SAP\\FrontEnd\\SAPgui\\saplogon.exe``
            """
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
            "doc": """
            Terminate the connection to the SAP server.
            """
        },
        "CloseSap": {
            "name": "Close SAP",
            "args": {},
            "result": {
                "NoSapGui": no_sap_gui,
                "Pass": "The SAP GUI was closed."
            },
            "doc": """
            Close the SAP GUI
            """
        },
        "ExportSpreadsheet": {
            "name": "Export Spreadsheet",
            "args": {
                "index": {
                    "name": "table_index",
                    "spec": {},
                    "optional": False
                }
            },
            "result": {
                "NoSession": no_session,
                "Exception": exception("The export function 'Spreadsheet' could not be called"),
                "NotFound": "No table was found that supports the export function 'Spreadsheet'",
                "Pass": "The export function 'Spreadsheet' was successfully called on the table with index {0}."
            },
            "doc": """
            The export function 'Spreadsheet' will be executed for the specified table, if available.

            | ``Export spreadsheet    table_index``

            table_index: 1, 2,...
            """
        },
        "ExportTree": {
            "name": "Export Function Tree",
            "args": {
                "filepath": {
                    "name": "filepath",
                    "spec": {},
                    "optional": False
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": "The window contains no tree element",
                "Pass": "The function tree was exported to {0}",
                "Exception": exception("The function tree could not be exported. {0}")
            },
            "doc": """
            Export the function tree in JSON format to the file provided.
            
            | ``Export Function Tree     filepath``
            """
        },
        "AttachToRunningSap": {
            "name": "Connect to Running SAP",
            "args": {},
            "result": {
                "NoSapGui": no_sap_gui,
                "NoGuiScripting": no_gui_scripting,
                "NoConnection": no_connection,
                "NoSession": no_session,
                "NoServerScripting": no_server_scripting,
                "Pass": "Connected to a running SAP instance.",
                "Exception": exception("Could not connect to a running SAP instance. {0}")
            },
            "doc": """
            Connect to a running SAP instance and take control of it.
            """
        },
        "ConnectToServer": {
            "name": "Connect to Server",
            "args": {
                "server": {
                    "name": "server_name",
                    "spec": {},
                    "optional": False
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
            "doc": """
            Connect to the SAP Server provided.
            
            | ``Connect to Server    servername``
            """
        },
        "DoubleClickCell": {
            "name": "Double-click Cell",
            "args": {
                "a1row_locator": {
                    "name": "row_locator",
                    "spec": {},
                    "optional": False
                },
                "a2column": {
                    "name": "column",
                    "spec": {},
                    "optional": False
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The cell with the locator '{0}, {1}' could not be found."),
                "Pass": "The cell with the locator '{0}, {1}' was double-clicked.",
                "Exception": exception("The cell could not be double-clicked. {0}")
            },
            "doc": f"""
            Double-click the cell at the intersection of the row and the column provided.
            
            | ``Double-click Cell     row_locator     column``
            
            {row_locator}
            """
        },
        "DoubleClickTextField": {
            "name": "Double-click Text Field",
            "args": {
                "locator": {
                    "name": "locator",
                    "spec": {
                        "Content": Content,
                        "HLabel": HLabel,
                        "VLabel": VLabel,
                        "HLabelVLabel": HLabelVLabel,
                        "HLabelHLabel": HLabelHLabel,
                        "HIndexVLabel": HIndexVLabel,
                        "HLabelVIndex": HLabelVIndex
                    },
                    "optional": False
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The text field with the locator '{0}' could not be found"),
                "Pass": "The text field with the locator '{0}' was double-clicked.",
                "Exception": exception("The text field could not be double-clicked. {0}")
            },
            "doc": """
            Double click the text field specified by the locator.
            
            | ``Double-click Text Field     Locator``

            Text field locators are documented in the keyword Fill Text Field.
            """
        },
        "ExecuteTransaction": {
            "name": "Execute Transaction",
            "args": {
                "T_Code": {
                    "name": "T_Code",
                    "spec": {},
                    "optional": False
                }
            },
            "result": {
                "NoSession": no_session,
                "Pass": "The transaction with T-Code {0} was executed.",
                "Exception": exception("The transaction could not be executed. {0}")
            },
            "doc": """
            Execute the transaction with the given T-Code.
            
            | ``Execute Transaction    T_Code``
            """
        },
        "ExportForm": {
            "name": "Export Dynpro",
            "args": {
                "a1name": {
                    "name": "name",
                    "spec": {},
                    "optional": False
                },
                "a2directory": {
                    "name": "directory",
                    "spec": {},
                    "optional": False
                }
            },
            "result": {
                "NoSession": no_session,
                "Pass": "The Dynpro was exported to the JSON file {0} and the PNG image {1}",
                "Exception": exception("The Dynpro could not be exported. {0}")
            },
            "doc": """
            Write all texts in the Dynpro to a JSON file. Also a screenshot will be saved in PNG format.
            
            | ``Export Dynpro     name     directory``
            
            directory: Absolute path to the directory where the files will be saved.
            """
        },
        "FillTableCell": {
            "name": "Fill Cell",
            "args": {
                "a1row_locator": {
                    "name": "row_locator",
                    "spec": {},
                    "optional": False
                },
                "a2column": {
                    "name": "column",
                    "spec": {},
                    "optional": False
                },
                "a3content": {
                    "name": "content",
                    "spec": {},
                    "optional": False
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The cell with the locator '{0}, {1}' could not be found"),
                "NotChangeable": "The cell with the locator '{0}, {1}' is not changeable.",
                "Pass": "The cell with the locator '{0}, {1}' was filled.",
                "Exception": exception("The cell could not be filled. {0}")
            },
            "doc": f"""
            Fill the cell at the intersection of the row and column with the content provided.

            | ``Fill Cell    row_locator    column   content``

            {row_locator}

            *Hint*: To migrate from the old keyword with two arguments perform a search and replace with a regular expression.
            """
        },
        "FillTextField": {
            "name": "Fill Text Field",
            "args": {
                "a1locator": {
                    "name": "locator",
                    "spec": {
                        "HLabel": HLabel,
                        "VLabel": VLabel,
                        "HLabelVLabel": HLabelVLabel,
                        "HLabelHLabel": HLabelHLabel,
                        "HIndexVLabel": HIndexVLabel,
                        "HLabelVIndex": HLabelVIndex
                    },
                    "optional": False
                },
                "a2content": {
                    "name": "content",
                    "spec": {},
                    "optional": False
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The text field with the locator '{0}' could not be found."),
                "Pass": "The text field with the locator '{0}' was filled.",
                "Exception": exception("The text field could not be filled. {0}")
            },
            "doc": """
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
        },
        "HighlightButton": {
            "name": "Highlight Button",
            "args": {
                "button": {
                    "name": "name_or_tooltip",
                    "spec": {},
                    "optional": False
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The button '{0}' could not be found."),
                "Pass": "The button '{0}' was highlighted.",
                "Exception": exception("The button could not be highlighted. {0}")
            },
            "doc": """
            Highlight the button with the given name or tooltip.
            
            | ``Highlight Button    name or tooltip``
            """
        },
        "PressKeyCombination": {
            "name": "Press Key Combination",
            "args": {
                "keyCombination": {
                    "name": "key_combination",
                    "spec": {},
                    "optional": False
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": "The key combination '{0}' is not supported. See the keyword documentation for valid key combinations.",
                "Pass": "The key combination '{0}' was pressed.",
                "Exception": exception("The key combination '{0}' could not be pressed.")
            },
            "doc": """
            Press the given key combination. Valid key combinations are the keyboard shortcuts
            in the context menu (shown when the right mouse button is pressed). For a full list 
            of supported key combinations consult the [https://help.sap.com/docs/sap_gui_for_windows/b47d018c3b9b45e897faf66a6c0885a8/71d8c95e9c7947ffa197523a232d8143.html?version=770.01&locale=en-US|documentation].
            
            | ``Press Key Combination    key combination``
            """
        },
        "PushButton": {
            "name": "Push Button",
            "args": {
                "button": {
                    "name": "name_or_tooltip",
                    "spec": {},
                    "optional": False
                }
            },
            "result": {
                "NoSession": no_session,
                "SapError": sap_error,
                "NotFound": not_found("The button '{0}' could not be found."),
                "Pass": "The button '{0}' was pushed.",
                "Exception": exception("The button could not be pushed. {0}")
            },
            "doc": """
            Push the button with the given name or tooltip.
            
            | ``Push Button    name or tooltip``
            """
        },
        "PushButtonCell": {
            "name": "Push Button Cell",
            "args": {
                "a1row_or_label": {
                    "name": "row_locator",
                    "spec": {},
                    "optional": False
                },
                "a2column": {
                    "name": "column",
                    "spec": {},
                    "optional": False
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The button cell with the locator '{0}, {1}' could not be found."),
                "Pass": "The button cell with the locator '{0}, {1}' was pushed.",
                "Exception": exception("The button cell could not be pushed. {0}")
            },
            "doc": """
            Push the button cell located at the intersection of the row and column provided.
            
            | ``Push Button Cell     row_locator     column``
            
            row_locator: Either the row number or the button label, button tooltip, or the contents of a cell in the row. If the label, the tooltip or the contents of the cell is a number, it must be enclosed in double quotation marks.
            """
        },
        "ReadStatusbar": {
            "name": "Read Statusbar",
            "args": {},
            "result": {
                "NoSession": no_session,
                "NotFound": "No statusbar was found.",
                "Pass": "The statusbar was read.",
                "Exception": exception("The statusbar could not be read")
            },
            "doc": """
            Read the message in the statusbar.

            | ``Read Statusbar``
            """
        },
        "ReadTextField": {
            "name": "Read Text Field",
            "args": {
                "locator": {
                    "name": "locator",
                    "spec": {
                        "HLabel": HLabel,
                        "VLabel": VLabel,
                        "HLabelVLabel": HLabelVLabel,
                        "Content": Content
                    },
                    "optional": False
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The text field with the locator '{0}' could not be found."),
                "Pass": "The text field with the locator '{0}' was read.",
                "Exception": exception("The text field could not be read. {0}")
            },
            "doc": """
            Read the contents of the text field specified by the locator.
            
            | ``Read Text Field    Locator``
            
            Text field locators are documented in the keyword Fill Text Field.
            """
        },
        "ReadText": {
            "name": "Read Text",
            "args": {
                "locator": {
                    "name": "locator",
                    "spec": {
                        "HLabel": HLabel,
                        "Content": Content
                    },
                    "optional": False
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("No text with the locator '{0}' was found."),
                "Pass": "A text with the locator '{0}' was read.",
                "Exception": exception("The text could not be read. {0}")
            },
            "doc": """
            Read the text specified by the locator.
            
            *Text starting with a given substring*
            | ``Read Text    = substring``
            
            *Text following a label*
            | ``Read Text    Label``
            """
        },
        "ReadTableCell": {
            "name": "Read Cell",
            "args": {
                "a1row_locator": {
                    "name": "row_locator",
                    "spec": {},
                    "optional": False
                },
                "a2column": {
                    "name": "column",
                    "spec": {},
                    "optional": False
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The cell with the locator '{0}, {1}' could not be found."),
                "Pass": "The cell with the locator '{0}, {1}' was read.",
                "Exception": exception("The cell could not be read. {0}")
            },
            "doc": f"""
            Read the contents of the cell at the intersection of the row and column provided.

            | ``Read Cell     row_locator     column``
            
            {row_locator}
            """
        },
        "SaveScreenshot": {
            "name": "Save Screenshot",
            "args": {
                "filepath": {
                    "name": "destination",
                    "spec": {},
                    "optional": False
                }
            },
            "result": {
                "NoSession": no_session,
                "InvalidPath": "The path '{0}' is invalid.",
                "UNCPath": r"UNC paths (i.e. beginning with \\) are not allowed",
                "NoAbsPath": "The path '{0}' is not an absolute path.",
                "Log": "The return value will be written to the protocol",
                "Pass": "The screenshot was saved in {0}.",
                "Exception": exception("The screenshot could not be saved. {0}")
            },
            "doc": """
            Save a screenshot of the current window to the given destination.
            
            | ``Save Screenshot     destination``
            
            destination: Either the absolute path to a .png file or LOG to embed the image in the protocol.
            """
        },
        "SelectCell": {
            "name": "Select Cell",
            "args": {
                "a1row_locator": {
                    "name": "row_locator",
                    "spec": {},
                    "optional": False
                },
                "a2column": {
                    "name": "column",
                    "spec": {},
                    "optional": False
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The cell with the locator '{0}, {1}' could not be found."),
                "Pass": "The cell with the locator '{0}, {1}' was selected.",
                "Exception": exception("The cell could not be selected. {0}")
            },
            "doc": f"""
            Select the cell at the intersection of the row and column provided.
            
            | ``Select Cell     row_locator     column``
            
            {row_locator}
            """
        },
        "SelectCellValue": {
            "name": "Select Cell Value",
            "args": {
                "a1row_locator": {
                    "name": "row_locator",
                    "spec": {},
                    "optional": False
                },
                "a2column": {
                    "name": "column",
                    "spec": {},
                    "optional": False
                },
                "a3entry": {
                    "name": "value",
                    "spec": {},
                    "optional": False
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The cell with the locator '{0}, {1}' could not be found."),
                "EntryNotFound": not_found("The value '{2}' is not available in the cell with the locator '{0}, {1}'."),
                "Exception": exception("The value could not be selected. {0}"),
                "Pass": "The value '{2}' was selected."
            },
            "doc": f"""
            Select the specified value in the cell at the intersection of the row and column provided.
            
            | ``Select Cell Value    row_locator    column    value``

            {row_locator}
            """
        },
        "SelectComboBoxEntry": {
            "name": "Select Dropdown Menu Entry",
            "args": {
                "a1comboBox": {
                    "name": "dropdown_menu",
                    "spec": {},
                    "optional": False
                },
                "a2entry": {
                    "name": "entry",
                    "spec": {},
                    "optional": False
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The dropdown menu '{0}' could not be found."),
                "EntryNotFound": not_found("In the dropdown menu '{0}' no entry '{1}' could be found."),
                "Pass": "In the dropdown menu '{0}' the entry '{1}' was selected.",
                "Exception": exception("The entry could not be selected. {0}")
            },
            "doc": """
            Select the specified entry from the dropdown menu provided.
            
            | ``Select Dropdown Menu Entry   dropdown_menu    entry``
            """
        },
        "SelectRadioButton": {
            "name": "Select Radio Button",
            "args": {
                "locator": {
                    "name": "locator",
                    "spec": {
                        "HLabel": HLabel,
                        "VLabel": VLabel,
                        "HLabelVLabel": HLabelVLabel
                    },
                    "optional": False
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The radio button with locator '{0}' could not be found."),
                "Pass": "The radio button with locator '{0}' was selected.",
                "Exception": exception("The radio button could not be selected. {0}")
            },
            "doc": """
            Select the radio button specified by the locator.
            
            *Radio button with a label to its left or its right*
            | ``Select Radio Button    label``
            
            *Radio button with a label above it*
            | ``Select Radio Button    @ label``
            
            *Radio button at the intersection of a label to its left or its right and a label above it*
            | ``Select Radio Button    left or right label @ label above``
            """
        },
        "SelectTableRow": {
            "name": "Select Table Row",
            "args": {
                "row_number": {
                    "name": "row_number",
                    "spec": {},
                    "optional": False
                }
            },
            "result": {
                "NoSession": no_session,
                "Exception": exception("The row with index '{0}' could not be selected"),
                "NotFound": "The table contains no row with index '{0}'",
                "Pass": "The row with index '{0}' was selected"
            },
            "doc": """
            Select the specified table row.

            | ``Select Table Row    row_number``
            """
        },
        "SelectTextField": {
            "name": "Select Text Field",
            "args": {
                "locator": {
                    "name": "locator",
                    "spec": {
                        "HLabel": HLabel,
                        "VLabel": VLabel,
                        "HLabelVLabel": HLabelVLabel,
                        "HLabelHLabel": HLabelHLabel,
                        "HIndexVLabel": HIndexVLabel,
                        "HLabelVIndex": HLabelVIndex,
                        "Content": Content
                    },
                    "optional": False
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The text field with the locator '{0}' could not be found."),
                "Pass": "The text field with the locator '{0}' was selected.",
                "Exception": exception("The text field could not be selected. {0}")
            },
            "doc": """
            Select the text field specified by the locator.
            
            | ``Select Text Field    Locator``
            
            Text field locators are documented in the keyword Fill Text Field.
            """
        },
        "SelectTextLine": {
            "name": "Select Text Line",
            "args": {
                "content": {
                    "name": "content",
                    "spec": {},
                    "optional": False
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The text line starting with '{0}' could not be found."),
                "Pass": "The text line starting with '{0}' was selected.",
                "Exception": exception("The text line could not be selected. {0}")
            },
            "doc": """
            Select the text line starting with the given content.
            
            | ``Select Text Line    content``
            """
        },
        "TickCheckBox": {
            "name": "Tick Checkbox",
            "args": {
                "locator": {
                    "name": "locator",
                    "spec": {
                        "HLabel": HLabel,
                        "VLabel": VLabel,
                        "HLabelVLabel": HLabelVLabel
                    },
                    "optional": False
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The checkbox with the locator '{0}' could not be found."),
                "Pass": "The checkbox with the locator '{0}' was ticked.",
                "Exception": exception("The checkbox could not be ticked. {0}")
            },
            "doc": """
            Tick the checkbox specified by the locator.
            
            *Checkbox with a label to its left or its right*
            | ``Tick Checkbox    label``
            
            *Checkbox with a label above it*
            | ``Tick Checkbox    @ label``
            
            *Checkbox at the intersection of a label to its left and a label above it*
            | ``Tick Checkbox    left label @ label above``
            """
        },
        "UntickCheckBox": {
            "name": "Untick Checkbox",
            "args": {
                "locator": {
                    "name": "locator",
                    "spec": {
                        "HLabel": HLabel,
                        "VLabel": VLabel,
                        "HLabelVLabel": HLabelVLabel
                    },
                    "optional": False
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The checkbox with the locator '{0}' could not be found."),
                "Pass": "The checkbox with the locator '{0}' was unticked.",
                "Exception": exception("The checkbox could not be unticked. {0}")
            },
            "doc": """
            Untick the checkbox specified by the locator.
            
            *Checkbox with a label to its left or its right*
            | ``Untick Checkbox    label``
            
            *Checkbox with a label above it*
            | ``Untick Checkbox    @ label``
            
            *Checkbox at the intersection of a label to its left and a label above it*
            | ``Untick Checkbox    left label @ label above``
            """
        },
        "TickCheckBoxCell": {
            "name": "Tick Checkbox Cell",
            "args": {
                "a1row": {
                    "name": "row_locator",
                    "spec": {},
                    "optional": False
                },
                "a2column": {
                    "name": "column",
                    "spec": {},
                    "optional": False
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The checkbox cell with the locator '{0}, {1}' coud not be found."),
                "Pass": "The checkbox cell with the locator '{0}, {1}' was ticked.",
                "Exception": exception("The checkbox cell could not be ticked. {0}")
            },
            "doc": f"""
            Tick the checkbox cell at the intersection of the row and the column provided.
            
            | ``Tick Checkbox Cell     row_locator    column``

            {row_locator}
            """
        },
        "GetWindowTitle": {
            "name": "Get Window Title",
            "args": {},
            "result": {
                "NoSession": no_session,
                "Pass": "The title of the window was obtained.",
                "Exception": exception("The window title could not be read.")
            },
            "doc": """
            Get the title of the window in the foreground.
            
            | ``${Title}    Get Window Title``
            """
        },
        "GetWindowText": {
            "name": "Get Window Text",
            "args": {},
            "result": {
                "NoSession": no_session,
                "Pass": "The text message of the window was obtained.",
                "Exception": exception("The text message of the window could not be read.")
            },
            "doc": """
            Get the text message of the window in the foreground.
            
            | ``${Text}    Get Window Text``
            """
        }
    },
    "specs": {}
}
