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

HLabel = "::label"
VLabel = ":@:label above"
HLabelVLabel = "label:@:label above"
HLabelHLabel = "left label:>>:right label"
HIndexVLabel = "index:@:label above"
HLabelVIndex = "label:@:index"
Content = ":=:content"
ColumnContent = "column:=:content"

locales = {
    "DE": "German"
}

locales_bullet_list = lambda: "\n".join([f"- RoboSAPiens.{country} ({lang})" for country, lang in locales.items()])

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
                    "spec": {}
                }
            },
            "result": {
                "Pass": "The SAP GUI was opened.",
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
                    "spec": {}
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
                    "spec": {}
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
                    "spec": {}
                },
                "a2column": {
                    "name": "column",
                    "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The cell with the locator '{0}, {1}' could not be found."),
                "Pass": "The cell with the locator '{0}, {1}' was double-clicked.",
                "Exception": exception("The cell could not be double-clicked. {0}")
            },
            "doc": """
            Double-click the cell at the intersection of the row and the column provided.
            
            | ``Double-click Cell     row_locator     column``
            
            row_locator: either the row number or the content of a cell in the row.
            """
        },
        "DoubleClickTextField": {
            "name": "Double-click Text Field",
            "args": {
                "content": {
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
            "doc": """
            Double click the text field with the content provided.
            
            | ``Double-click Text Field     Content``
            """
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
                    "spec": {}
                },
                "a2directory": {
                    "name": "directory",
                    "spec": {}
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
                    "spec": {}
                },
                "a2column_content": {
                    "name": "column_content",
                    "spec": {
                        "ColumnContent": ColumnContent
                    }
                }
            },
            "result": {
                "NoSession": no_session,
                "InvalidFormat": "The format of the second parameter must be 'column = content'",
                "NotFound": not_found("The cell with the locator '{0}, {1}' could not be found"),
                "NotChangeable": "The cell with the locator '{0}, {1}' is not changeable.",
                "Pass": "The cell with the locator '{0}, {1}' was filled.",
                "Exception": exception("The cell could not be filled. {0}")
            },
            "doc": """
            Fill the cell at the intersection of the row and the column specified with the content provided.
            
            | ``Fill Cell     row     column = content``
            
            row: either the row number or the contents of a cell in the row.
            
            *Hint*: Some cells can be filled using the keyword 'Fill Text Field' providing as locator the description obtained by selecting the cell and pressing F1.
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
                    }
                },
                "a2content": {
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
                    "spec": {}
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
        "PushButton": {
            "name": "Push Button",
            "args": {
                "button": {
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
            "doc": """
            Push the button with the given name or tooltip.
            
            | ``Push Button    name or tooltip``
            """
        },
        "PushButtonCell": {
            "name": "Push Button Cell",
            "args": {
                "a1row_or_label": {
                    "name": "row_or_label",
                    "spec": {}
                },
                "a2column": {
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
            "doc": """
            Push the button cell located at the intersection of the row and column provided.
            
            | ``Push Button Cell     row_locator     column``
            
            row_locator: Row number, label or tooltip.
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
                    }
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
            
            *Text field with a label to its left*
            | ``Read Text Field    label``
            
            *Text field with a label above it*
            | ``Read Text Field    @ label``
            
            *Text field at the intersection of a label to its left and a label above it*
            | ``Read Text Field    left label @ label above``
            
            *Text field whose content starts with a given text*
            | ``Read Text Field    = text``
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
                    }
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
                    "spec": {}
                },
                "a2column": {
                    "name": "column",
                    "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The cell with the locator '{0}, {1}' could not be found."),
                "Pass": "The cell with the locator '{0}, {1}' was read.",
                "Exception": exception("The cell could not be read. {0}")
            },
            "doc": """
            Read the contents of the cell at the intersection of the row and column provided.

            | ``Read Cell     row_locator     column``
            
            row_locator: either the row number or the contents of a cell in the row.
            """
        },
        "SaveScreenshot": {
            "name": "Save Screenshot",
            "args": {
                "filepath": {
                    "name": "filepath",
                    "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "InvalidPath": "The path '{0}' is invalid.",
                "UNCPath": r"UNC paths (i.e. beginning with \\) are not allowed",
                "NoAbsPath": "The path '{0}' is not an absolute path.",
                "Pass": "The screenshot was saved in {0}.",
                "Exception": exception("The screenshot could not be saved. {0}")
            },
            "doc": """
            Save a screenshot of the current window in the file provided.
            
            | ``Save Screenshot     filepath``
            
            filepath: Absolute path to a .png file.
            """
        },
        "SelectCell": {
            "name": "Select Cell",
            "args": {
                "a1row_locator": {
                    "name": "row_locator",
                    "spec": {}
                },
                "a2column": {
                    "name": "column",
                    "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The cell with the locator '{0}, {1}' could not be found."),
                "Pass": "The cell with the locator '{0}, {1}' was selected.",
                "Exception": exception("The cell could not be selected. {0}")
            },
            "doc": """
            Select the cell at the intersection of the row and column provided.
            
            | ``Select Cell     row_locator     column``
            
            row_locator: either the row number or the contents of a cell in the row.
            """
        },
        "SelectComboBoxEntry": {
            "name": "Select Dropdown Menu Entry",
            "args": {
                "a1comboBox": {
                    "name": "dropdown_menu",
                    "spec": {}
                },
                "a2entry": {
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
            "doc": """
            Select the specified entry from the dropdown menu provided.
            
            | ``Select Dropdown Menu Entry   dropdown menu    entry``
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
                    }
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
                    "spec": {}
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
                        "Content": Content
                    }
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
            
            *Text field with a label to its left*
            | ``Select Text Field    label``
            
            *Text field with a label above it*
            | ``Select Text Field    @ label``
            
            *Text field at the intersection of a label to its left and a label above it*
            | ``Select Text Field    left label @ label above``
            
            *Text field whose content starts with the given text*
            | ``Select Text Field    = text``
            """
        },
        "SelectTextLine": {
            "name": "Select Text Line",
            "args": {
                "content": {
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
                    }
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
                    }
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
                    "name": "row_number",
                    "spec": {}
                },
                "a2column": {
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
            "doc": """
            Tick the checkbox cell at the intersection of the row and the column provided.
            
            | ``Tick Checkbox Cell     row number     column``
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
