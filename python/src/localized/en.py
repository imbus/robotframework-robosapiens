from typing import Callable
from schema import RoboSAPiens

Fstr = Callable[[str], str]

sap_error = 'SAP Error: {0}'
no_session = 'No active SAP-Session. Call the keyword "Connect To Server" or "Connect To Running SAP" first.'
no_sap_gui = 'No open SAP GUI found. Call the keyword "Open SAP" first.'
no_gui_scripting = 'The scripting support is not activated. It must be activated in the Settings of SAP Logon.'
no_connection = 'No existing connection to an SAP server. Call the keyword "Connect to Server" first.'
no_server_scripting = 'Scripting is not activated on the server side. Please consult the documentation of RoboSAPiens.'
not_found: Fstr = lambda msg: f"{msg} Hint: Check the spelling"
button_or_cell_not_found: Fstr = lambda msg: f"{msg} Hints: Check the spelling, maximize the SAP window"
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

        New features in Version 2.0:

        - Support for SAP GUI 8.0 64-bit
        - New keyword "Expand Tree Element"
        - New keyword "Get Row Count"
        - New keyword "Scroll Contents"
        - New keyword "Select Menu Entry"
        - New keyword "Untick Checkbox Cell"

        Breaking changes with respect to Version 1.0:

        - The keyword "Fill Cell" takes three positional arguments instead of two
        - The keyword "Read Statusbar" returns a dictionary instead of a string
        - Renamed the keyword "Export Dynpro" to "Export Window"
        - Renamed the keyword "Export Function Tree" to "Export Tree Structure"
        - Renamed the keyword "Select Text Line" to "Select Text"
        """,
        "init": ""
    },
    "args": {
        "a1presenter_mode": {
            "name": "presenter_mode",
            "default": False,
            "doc": "Highlight each GUI element acted upon"
        },
        "a2x64": {
            "name": "x64",
            "default": False,
            "doc": "Execute RoboSAPiens 64-bit"
        }
    },
    "keywords": {
        "ActivateTab": {
            "name": "Select Tab",
            "args": {
                "tab": {
                    "name": "tab_name",
                    "spec": {},

                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The tab '{0}' could not be found."),
                "Pass": "The tab '{0}' was selected.",
                "Exception": exception("The tab could not be selected. {0}")
            },
            "doc": """
            Select the tab with the name provided.
            
            | ``Select Tab    tab_name``
            """
        },
        "ExpandTreeElement": {
            "name": "Expand Tree Element",
            "args": {
                "a1elementPath": {
                    "name": "element_path",
                    "spec": {},

                },
                "a2column": {
                    "name": "column",
                    "spec": {},

                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The tree element '{0}, {1}' could not be found."),
                "Pass": "The tree element '{0}, {1}' was expanded.",
                "Exception": exception("The tree element could not be expanded. {0}")
            },
            "doc": """
            Expand the tree element located at the path and column provided.
            
            | ``Expand Tree Element    element_path    column``

            element_path: The path to the element using '/' as separator. e.g. Engineering/Civil Engineering
            """
        },
        "OpenSap": {
            "name": "Open SAP",
            "args": {
                "path": {
                    "name": "path",
                    "spec": {},

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
        "CountTableRows": {
            "name": "Get Row Count",
            "args": {},
            "result": {
                "NoSession": no_session,
                "Exception": exception("Could not count the rows in the table."),
                "NotFound": "The window contains no table.",
                "Pass": "Counted the number of rows in the table."
            },
            "doc": """
            Count the rows in a table.
            
            | ``${row_count}   Get Row Count``
            """
        },
        "ExportSpreadsheet": {
            "name": "Export Spreadsheet",
            "args": {
                "index": {
                    "name": "table_index",
                    "spec": {},

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
            "name": "Export Tree Structure",
            "args": {
                "filepath": {
                    "name": "filepath",
                    "spec": {},

                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": "The window contains no tree structure",
                "Pass": "The tree structure was exported to the file '{0}'",
                "Exception": exception("The tree structure could not be exported. {0}")
            },
            "doc": """
            Export the tree structure in the current window to the file provided in JSON format.
            
            | ``Export Tree Structure     filepath``

            filepath: Absolute path to a file with extension .json
            """
        },
        "AttachToRunningSap": {
            "name": "Connect to Running SAP",
            "args": {
                "sessionNumber": {
                    "name": "session_number",
                    "default": "1",
                    "spec": {}
                }
            },
            "result": {
                "NoSapGui": no_sap_gui,
                "NoGuiScripting": no_gui_scripting,
                "NoConnection": no_connection,
                "NoSession": no_session,
                "NoServerScripting": no_server_scripting,
                "InvalidSessionId": "There is no session number {0}",
                "Pass": "Connected to a running SAP instance.",
                "Exception": exception("Could not connect to a running SAP instance. {0}")
            },
            "doc": """
            Connect to a running SAP instance and take control of it. By default the session number 1 will be used. 
            To use a different session specify the session number.

            | ``Connect to Running SAP    session_number``
            """
        },
        "ConnectToServer": {
            "name": "Connect to Server",
            "args": {
                "server": {
                    "name": "server_name",
                    "spec": {},

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
            
            | ``Connect to Server    server_name``

            server_name: The name of the server in SAP Logon (not the SID).
            """
        },
        "DoubleClickCell": {
            "name": "Double-click Cell",
            "args": {
                "a1row_locator": {
                    "name": "row_locator",
                    "spec": {},

                },
                "a2column": {
                    "name": "column",
                    "spec": {},

                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": button_or_cell_not_found("The cell with the locator '{0}, {1}' could not be found."),
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
        "ExportWindow": {
            "name": "Export Window",
            "args": {
                "a1name": {
                    "name": "name",
                    "spec": {},

                },
                "a2directory": {
                    "name": "directory",
                    "spec": {},

                }
            },
            "result": {
                "NoSession": no_session,
                "Pass": "The window contents were exported to {0} and a screenshot was saved to {1}.",
                "Exception": exception("The window contents could not be exported. {0}")
            },
            "doc": """
            Export the window contents to a JSON file. Also a screenshot will be saved in PNG format.
            
            | ``Export Window     name     directory``
            
            directory: Absolute path to the directory where the files will be saved.

            *Note*: Currently not all GUI-elements are exported.
            """
        },
        "FillTableCell": {
            "name": "Fill Cell",
            "args": {
                "a1row_locator": {
                    "name": "row_locator",
                    "spec": {},

                },
                "a2column": {
                    "name": "column",
                    "spec": {},

                },
                "a3content": {
                    "name": "content",
                    "spec": {},

                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": button_or_cell_not_found("The cell with the locator '{0}, {1}' could not be found"),
                "NotChangeable": "The cell with the locator '{0}, {1}' is not editable.",
                "NoTable": "The window contains no table.",
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

                },
                "a2content": {
                    "name": "content",
                    "spec": {},

                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The text field with the locator '{0}' could not be found."),
                "NotChangeable": "The text field with the locator '{0}' is not editable.",
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

                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": button_or_cell_not_found("The button '{0}' could not be found."),
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
                    "name": "locator",
                    "spec": {},

                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": button_or_cell_not_found("The button '{0}' could not be found."),
                "NotChangeable": "The button '{0}' is disabled.",
                "Pass": "The button '{0}' was pushed.",
                "Exception": exception("The button could not be pushed. {0}")
            },
            "doc": """
            Push the button with the given name or tooltip.
            
            | ``Push Button    locator``

            locator: name or tooltip. 
            
            *Hint*: Some tooltips consist of a name followed by several spaces and a keyboard shortcut.
            The name may be used as locator as long as it is unique.
            When using the full tooltip text remember to escape the spaces (e.g. ``Back \\\\ \\\\ (F3)``)
            """
        },
        "PushButtonCell": {
            "name": "Push Button Cell",
            "args": {
                "a1row_or_label": {
                    "name": "row_locator",
                    "spec": {},

                },
                "a2column": {
                    "name": "column",
                    "spec": {},

                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": button_or_cell_not_found("The button cell with the locator '{0}, {1}' could not be found."),
                "NotChangeable": "The button cell with the locator '{0}, {1}' is disabled.",
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
                "Json": "The return value is in JSON format", 
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

                },
                "a2column": {
                    "name": "column",
                    "spec": {},

                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": button_or_cell_not_found("The cell with the locator '{0}, {1}' could not be found."),
                "NoTable": "The window contains no table.",
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
        "ScrollTextFieldContents": {
            "name": "Scroll Contents",
            "args": {
                "a1direction": {
                    "name": "direction",
                    "spec": {}
                },
                "a2untilTextField": {
                    "name": "until_textfield",
                    "default": None,
                    "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "Exception": exception("The contents of the text fields could not be scrolled."),
                "NoScrollbar": "The window contains no scrollable text fields.",
                "MaximumReached": "The contents of the text fields cannot be scrolled any further.",
                "InvalidDirection": "Invalid direction. The direction must be one of: UP, DOWN, BEGIN, END",
                "Pass": "The contents of the text fields were scrolled in the direction '{0}'."
            },
            "doc": """
            Scroll the contents of the text fields within an area with a scrollbar.

            | ``Scroll Contents    direction``

            direction: UP, DOWN, BEGIN, END

            If the parameter "until_textfield" is provided, the contents are scrolled until that text field is found.

            | ``Scroll Contents    direction    until_textfield``

            until_textfield: locator to find a text field
            """
        },
        "SelectCell": {
            "name": "Select Cell",
            "args": {
                "a1row_locator": {
                    "name": "row_locator",
                    "spec": {},

                },
                "a2column": {
                    "name": "column",
                    "spec": {},

                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": button_or_cell_not_found("The cell with the locator '{0}, {1}' could not be found."),
                "NoTable": "The window contains no table.",
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

                },
                "a2column": {
                    "name": "column",
                    "spec": {},

                },
                "a3entry": {
                    "name": "value",
                    "spec": {},

                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": button_or_cell_not_found("The cell with the locator '{0}, {1}' could not be found."),
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

                },
                "a2entry": {
                    "name": "entry",
                    "spec": {},

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

            *Hints*: The numeric key that enables simplified keyboard input is not part of the entry name.

            To select a value from a toolbar button with a dropdown menu, first push the button and then use this keyword. 
            """
        },
        "SelectMenuItem": {
            "name": "Select Menu Entry",
            "args": {
                "itemPath": {
                    "name": "menu_entry_path",
                    "spec": {},

                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The menu entry '{0}' could not be found."),
                "Pass": "The menu entry '{0}' was selected.",
                "Exception": exception("The menu entry could not be selected. {0}")
            },
            "doc": """
            Select the menu entry with the path provided.
            
            | ``Select Menu Entry    menu_entry_path``

            menu_entry_path: For an entry in a top-level menu, the entry name. 
            For an entry in a sub-menu, its path from the top-level with '/' as path separator.
            e.g. System/User Profile/Own Data
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

                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The radio button with locator '{0}' could not be found."),
                "NotChangeable": "The radio button with the locator '{0}' is disabled.",
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

            *Hint*: Use the row number 0 to select the whole table.
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
        "SelectText": {
            "name": "Select Text",
            "args": {
                "locator": {
                    "name": "locator",
                    "spec": {},

                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The text with the locator '{0}' could not be found."),
                "Pass": "The text with the locator '{0}' was selected.",
                "Exception": exception("The text could not be selected. {0}")
            },
            "doc": """
            Select the text specified by the locator.
            
            *Text starting with a given substring*
            | ``Select Text    = substring``
            
            *Text following a label*
            | ``Select Text    Label``
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

                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The checkbox with the locator '{0}' could not be found."),
                "NotChangeable": "The checkbox with the locator '{0}' is disabled.",
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

                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The checkbox with the locator '{0}' could not be found."),
                "NotChangeable": "The checkbox with the locator '{0}' is disabled.",
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

                },
                "a2column": {
                    "name": "column",
                    "spec": {},

                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": button_or_cell_not_found("The checkbox cell with the locator '{0}, {1}' could not be found."),
                "NotChangeable": "The checkbox cell with the locator '{0}, {1}' is disabled.",
                "Pass": "The checkbox cell with the locator '{0}, {1}' was ticked.",
                "Exception": exception("The checkbox cell could not be ticked. {0}")
            },
            "doc": f"""
            Tick the checkbox cell at the intersection of the row and the column provided.
            
            | ``Tick Checkbox Cell     row_locator    column``

            {row_locator}

            *Hint*: To tick the checkbox in the leftmost column with no title, select the row and press the "Enter" key.
            """
        },
        "UntickCheckBoxCell": {
            "name": "Untick Checkbox Cell",
            "args": {
                "a1row": {
                    "name": "row_locator",
                    "spec": {},

                },
                "a2column": {
                    "name": "column",
                    "spec": {},

                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": button_or_cell_not_found("The checkbox cell with the locator '{0}, {1}' could not be found."),
                "NotChangeable": "The checkbox cell with the locator '{0}, {1}' is disabled.",
                "Pass": "The checkbox cell with the locator '{0}, {1}' was unticked.",
                "Exception": exception("The checkbox cell could not be unticked. {0}")
            },
            "doc": f"""
            Untick the checkbox cell at the intersection of the row and the column provided.
            
            | ``Untick Checkbox Cell     row_locator    column``

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
