from typing import Callable
from schema import RoboSAPiens

Fstr = Callable[[str], str]

sap_error = 'SAP Error: {0}'
no_session = 'No active SAP-Session. Call the keyword "Connect To Server" or "Connect To Running SAP" first.'
no_sap_gui = 'No open SAP GUI found. Call the keyword "Open SAP" first.'
no_gui_scripting = 'The scripting support is not activated. It must be activated in the Settings of the SAP client.'
no_connection = 'No existing connection to an SAP server. Call the keyword "Connect to Server" first.'
no_server_scripting = 'Scripting is not activated on the server side. Please consult the documentation of RoboSAPiens.'
not_found: Fstr = lambda msg: f"{msg} Hint: Check the spelling"
button_or_cell_not_found: Fstr = lambda msg: f"{msg} Hints: Check the spelling, maximize the SAP window"
exception: Fstr = lambda msg: f"{msg}" + "\nFor more details run 'robot --loglevel DEBUG test.robot' and consult the file log.html"
row_locator = 'Either the row number or the contents of a cell in the row. If the cell only contains a number, it must be enclosed in double quotation marks.'
column = "Column title or tooltip"
textfield_locator = "Text field locators are documented in the keyword [#Fill Text Field|Fill Text Field]."
path = "Backslashes must be written twice. Otherwise use the RF built-in variable ${/} as path separator."
tooltip_hint = """Some tooltips consist of a name followed by several spaces and a keyboard shortcut.
The name may be used as locator as long as it is unique.
When using the full tooltip text enter only one space (e.g. ``Back (F3)``).
"""

HLabel = "::label"
VLabel = ":@:label above"
HLabelVLabel = "label:@:label above"
HLabelHLabel = "left label:>>:right label"
HIndexVLabel = "index:@:label above"
HLabelVIndex = "label:@:index"
Content = ":=:content"
Column = "column"

lib: RoboSAPiens = {
    "doc": {
        "intro": """RoboSAPiens: SAP GUI-Automation for Humans

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
        """,
        "init": "RoboSAPiens has the following initialization arguments:\n\n| =Argument= | =Description= |"
    },
    "args": {
        "a1presenter_mode": {
            "name": "presenter_mode",
            "default": False,
            "desc": "Wait half a second after executing a keyword and highlight the GUI element acted upon (if applicable)"
        },
        "a2x64": {
            "name": "x64",
            "default": False,
            "desc": "Execute RoboSAPiens 64-bit in order to automate SAP GUI 8 64-bit or SAP Business Client"
        }
    },
    "keywords": {
        "ActivateTab": {
            "name": "Select Tab",
            "args": {
                "tab": {
                    "name": "tab_name",
                    "desc": "Name or tooltip of the tab",
                    "spec": {},
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The tab '{0}' could not be found."),
                "Pass": "The tab '{0}' was selected.",
                "Exception": exception("The tab could not be selected. {0}")
            },
            "doc": {
                "desc": "Select the tab with the name provided.",
                "examples":  
                """
                Examples:

                | ``Select Tab    tab_name``
                """
            }
        },
        "DoubleClickTreeElement": {
            "name": "Double-click Tree Element",
            "args": {
                "elementPath": {
                    "name": "element_path",
                    "desc": "The path to the element using '/' as separator. e.g. Engineering/Civil Engineering",
                    "spec": {},
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The tree element '{0}' could not be found."),
                "Pass": "The tree element '{0}' was double-clicked.",
                "Exception": exception("The tree element could not be double-clicked. {0}")
            },
            "doc": {
                "desc": "Double-click the tree element located at the path provided.",
                "examples": 
                """
                Examples:

                | ``Double-click Tree Element    element_path``
                """
            }
        },
        "SelectTreeElement": {
            "name": "Select Tree Element",
            "args": {
                "elementPath": {
                    "name": "element_path",
                    "desc": "The path to the element using '/' as separator. e.g. Engineering/Civil Engineering",
                    "spec": {},
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The tree element '{0}' could not be found."),
                "Pass": "The tree element '{0}' was selected.",
                "Exception": exception("The tree element could not be selected. {0}")
            },
            "doc": {
                "desc": "Select the tree element located at the path provided.",
                "examples": 
                """
                Examples:

                | ``Select Tree Element    element_path``
                """
            }
        },
        "SelectTreeElementMenuEntry": {
            "name": "Select Menu Entry in Tree Element",
            "args": {
                "a1elementPath": {
                    "name": "element_path",
                    "desc": "The path to the element using '/' as separator, e.g. Engineering/Civil Engineering.",
                    "spec": {},

                },
                "a2menuEntry": {
                    "name": "menu_entry",
                    "desc": "The menu entry. For nested menus the path to the entry using '|' as separator, e.g. Create|Business Unit.",
                    "spec": {},
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The tree element '{0}' could not be found."),
                "Pass": "The menu entry '{0}' was selected.",
                "Exception": exception("The menu entry could not be selected. {0}")
            },
            "doc": {
                "desc": "Select the given entry in the context menu of the tree element located at the path provided.",
                "examples":
                """
                Examples:

                | ``Select Menu Entry in Tree Element    element_path    menu_entry``
                """
            }
        },
        "OpenSap": {
            "name": "Open SAP",
            "args": {
                "a1path": {
                    "name": "path",
                    "desc": "The path of the SAP executable",
                    "spec": {},
                },
                "a2sapArgs": {
                     "name": "sap_args",
                     "desc": "Command line arguments for the SAP executable",
                     "default": None,
                     "spec": {}
                }
            },
            "result": {
                "Pass": "SAP was opened.",
                "NoGuiScripting": no_gui_scripting,
                "SAPAlreadyRunning": "SAP is already running. It must be closed before calling this keyword.",
                "SAPNotStarted": "SAP could not be opened. Verify that the path and the arguments (if applicable) are correct.",
                "Exception": exception("SAP could not be opened. {0}")
            },
            "doc": {
                "desc": "Open SAP GUI or SAP Business Client.",
                 "examples":
                 rf"""
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

                 | ``Open SAP   C:\\Program Files\\SAP\\FrontEnd\\SAPgui\\sapshcut.exe -system=XXX -client=NNN -user=%{{username}} -pw{{password}}``

                 *Hint*: {path}
                 """
            }
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
            "doc": {
                "desc": "Terminate the connection to the SAP server.",
                "examples":
                """
                Examples:
                
                | ``Disconnect from server``
                """
            }
        },
        "CloseSap": {
            "name": "Close SAP",
            "args": {},
            "result": {
                "NoSapGui": no_sap_gui,
                "Pass": "The SAP GUI was closed."
            },
            "doc": {
                "desc": "Close the SAP GUI.",
                "examples": 
                """
                Examples:
                
                | ``Close SAP``

                *Hint*: This keyword only works if SAP GUI was started with the keyword [#Open SAP|Open SAP].
                """
            }
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
            "doc": {
                "desc": "Count the rows in a table.",
                "examples":
                """
                Examples:
                
                | ``${row_count}   Get Row Count``
                """
            }
        },
        "ExportTree": {
            "name": "Export Tree Structure",
            "args": {
                "filepath": {
                    "name": "filepath",
                    "desc": "Absolute path to a file with extension .json",
                    "spec": {},
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": "The window contains no tree structure",
                "Pass": "The tree structure was exported to the file '{0}'",
                "Exception": exception("The tree structure could not be exported. {0}")
            },
            "doc": {
                "desc": "Export the tree structure in the current window to the file provided.",
                "examples":
                f"""
                Examples:
                
                | ``Export Tree Structure     filepath``

                *Hint*: {path}
                """
            }
        },
        "AttachToRunningSap": {
            "name": "Connect to Running SAP",
            "args": {
                "sessionNumber": {
                    "name": "session_number",
                    "desc": "The session number in the lower right corner of the window",
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
                "Json": "The return value is in JSON format",
                "Pass": "Connected to a running SAP instance.",
                "Exception": exception("Could not connect to a running SAP instance. Hint: In order to connect to a 64-bit SAP client import RoboSAPiens with x64=True. {0}")
            },
            "doc": {
                "desc": "Connect to a running SAP instance and take control of it.",
                "examples":
                """
                Examples:
                
                | ``Connect to Running SAP``

                By default the session number 1 will be used. To use a different session specify the session number.

                | ``Connect to Running SAP    session_number``

                The return value contains session information such as client number and system ID:

                | ``${session_info}    Connect to Running SAP    session_number``
                """
            }
        },
        "ConnectToServer": {
            "name": "Connect to Server",
            "args": {
                "server": {
                    "name": "server_name",
                    "desc": "The name of the server in SAP Logon (not the SID).",
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
            "doc": {
                "desc": "Connect to the SAP Server provided.",
                "examples":
                """
                Examples:
                
                | ``Connect to Server    server_name``
                """
            }
        },
        "DoubleClickCell": {
            "name": "Double-click Cell",
            "args": {
                "a1row_locator": {
                    "name": "row_locator",
                    "desc": row_locator,
                    "spec": {},
                },
                "a2column": {
                    "name": "column",
                    "desc": column,
                    "spec": {},
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": button_or_cell_not_found("The cell with the locator '{0}, {1}' could not be found."),
                "Pass": "The cell with the locator '{0}, {1}' was double-clicked.",
                "Exception": exception("The cell could not be double-clicked. {0}")
            },
            "doc": {
                "desc": "Double-click the cell at the intersection of the row and the column provided.",
                "examples":
                """
                Examples:
                
                | ``Double-click Cell     row_locator     column``
                """
            }
        },
        "DoubleClickTextField": {
            "name": "Double-click Text Field",
            "args": {
                "locator": {
                    "name": "locator",
                    "desc": textfield_locator,
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
            "doc": {
                "desc": "Double click the text field specified by the locator.",
                "examples":
                """
                Examples:
                
                | ``Double-click Text Field     locator``
                """
            }
        },
        "ExecuteTransaction": {
            "name": "Execute Transaction",
            "args": {
                "T_Code": {
                    "name": "T_Code",
                    "desc": "The transaction code",
                    "spec": {},

                }
            },
            "result": {
                "NoSession": no_session,
                "Pass": "The transaction with T-Code {0} was executed.",
                "Exception": exception("The transaction could not be executed. {0}")
            },
            "doc": {
                "desc": "Execute the transaction with the given T-Code.",
                "examples":
                """
                Examples:
                
                | ``Execute Transaction    T_Code``
                """
            }
        },
        "ExportWindow": {
            "name": "Export Window",
            "args": {
                "a1name": {
                    "name": "name",
                    "desc": "Name of the output files",
                    "spec": {},

                },
                "a2directory": {
                    "name": "directory",
                    "desc": "Absolute path to the directory where the files will be saved",
                    "spec": {},

                }
            },
            "result": {
                "NoSession": no_session,
                "Pass": "The window contents were exported to {0} and a screenshot was saved to {1}.",
                "Exception": exception("The window contents could not be exported. {0}")
            },
            "doc": {
                "desc": "Export the window contents to a JSON file. Also a screenshot will be saved in PNG format.",
                "examples":
                f"""
                Examples:
                
                | ``Export Window     name     directory``
                
                *Hint*: {path}

                *Note*: Currently not all GUI elements are exported.
                """
            }
        },
        "FillTableCell": {
            "name": "Fill Cell",
            "args": {
                "a1row_locator": {
                    "name": "row_locator",
                    "desc": row_locator,
                    "spec": {},

                },
                "a2column": {
                    "name": "column",
                    "desc": column,
                    "spec": {},

                },
                "a3content": {
                    "name": "content",
                    "desc": "The new contents of the cell",
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
            "doc": {
                "desc": "Fill the cell at the intersection of the row and column with the content provided.",
                "examples":
                """
                Examples:
                
                | ``Fill Cell    row_locator    column   content``

                *Hint*: To migrate from the old keyword with two arguments perform a search and replace with a regular expression.
                """
            }
        },
        "FillTextField": {
            "name": "Fill Text Field",
            "args": {
                "a1locator": {
                    "name": "locator",
                    "desc": "The locator used to find the text field.",
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
                    "desc": "The new contents of the text field",
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
            "doc": {
                "desc": "Fill the text field specified by the locator with the content provided.",
                "examples":
                """
                Examples:
                
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
                
                *Text field with a non-unique label to the left or right of a unique label*
                | ``Fill Text Field    unique label >> text field label    content``
                
                *Text field without a label to the left or right of a unique label*
                | ``Fill Text Field    unique label >> F1 help text    content``

                *As a last resort the name obtained using [https://tracker.stschnell.de/|Scripting Tracker] can be used*
                | ``Fill Text Field    name    content``
                """
            }
        },
        "HighlightButton": {
            "name": "Highlight Button",
            "args": {
                "button": {
                    "name": "locator",
                    "desc": "The name or tooltip of the button",
                    "spec": {},
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": button_or_cell_not_found("The button '{0}' could not be found."),
                "Pass": "The button '{0}' was highlighted.",
                "Exception": exception("The button could not be highlighted. {0}")
            },
            "doc": {
                "desc": "Highlight the button with the given locator.",
                "examples":
                f"""
                Examples:
                
                | ``Highlight Button    locator``

                *Hint*: {tooltip_hint}
                """
            }
        },
        "PressKeyCombination": {
            "name": "Press Key Combination",
            "args": {
                "keyCombination": {
                    "name": "key_combination",
                    "desc": "Either one key or several keys separated by a + sign.",
                    "spec": {},
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": "The key combination '{0}' is not supported. See the keyword documentation for valid key combinations.",
                "Pass": "The key combination '{0}' was pressed.",
                "Exception": exception("The key combination '{0}' could not be pressed.")
            },
            "doc": {
                "desc": "Press the given key combination.",
                "examples": 
                """
                Examples:
                
                | ``Press Key Combination    key_combination``
                
                For a full list of supported key combinations consult the [https://help.sap.com/docs/sap_gui_for_windows/b47d018c3b9b45e897faf66a6c0885a8/71d8c95e9c7947ffa197523a232d8143.html?version=770.01&locale=en-US|documentation of SAP GUI].

                *Hint*: Pressing F2 is equivalent to a double-click.
                """
            }
        },
        "PushButton": {
            "name": "Push Button",
            "args": {
                "button": {
                    "name": "locator",
                    "desc": "The locator used to find the button.",
                    "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": button_or_cell_not_found("The button '{0}' could not be found."),
                "NotChangeable": "The button '{0}' is disabled.",
                "Pass": "The button '{0}' was pushed.",
                "Exception": exception("The button could not be pushed. {0}")
            },
            "doc": {
                "desc": "Push the button with the given locator.",
                "examples":
                f"""
                Examples:
                
                *Button with a name or tooltip*

                | ``Push Button    name or tooltip``

                *Button with a non-unique name or tooltip to the left or right of a unique label*

                | ``Push Button    unique label >> name or tooltip``

                *Hint*: {tooltip_hint}
                """
            }
        },
        "PushButtonCell": {
            "name": "Push Button Cell",
            "args": {
                "a1row_or_label": {
                    "name": "row_locator",
                    "desc": "Either the row number or the button label, button tooltip, or the contents of a cell in the row. If the label, the tooltip or the contents of the cell is a number, it must be enclosed in double quotation marks.",
                    "spec": {},

                },
                "a2column": {
                    "name": "column",
                    "desc": column,
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
            "doc": {
                "desc": "Push the button cell located at the intersection of the row and column provided.",
                "examples":
                """
                Examples:
                
                | ``Push Button Cell     row_locator     column``
                """
            }
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
            "doc": {
                "desc": "Read the contents of the statusbar. The return value is a dictionary with the entries 'status' and 'message'.",
                "examples":
                """
                Examples:
                
                | ``${statusbar}   Read Statusbar``

                """
            }
        },
        "ReadTextField": {
            "name": "Read Text Field",
            "args": {
                "locator": {
                    "name": "locator",
                    "desc": textfield_locator,
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
            "doc": {
                "desc": "Read the contents of the text field specified by the locator.",
                "examples":
                """
                Examples:
                
                | ${contents}   ``Read Text Field    locator``
                """
            }
        },
        "ReadText": {
            "name": "Read Text",
            "args": {
                "locator": {
                    "name": "locator",
                    "desc": "The locator used to find the text",
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
            "doc": {
                "desc": "Read the text specified by the locator.",
                "examples":
                """
                Examples:
                
                *Text starting with a given substring*
                | ${text}   ``Read Text    = substring``
                
                *Text following a label*
                | ${text}   ``Read Text    label``
                """
            }
        },
        "ReadTableCell": {
            "name": "Read Cell",
            "args": {
                "a1row_locator": {
                    "name": "row_locator",
                    "desc": row_locator,
                    "spec": {},

                },
                "a2column": {
                    "name": "column",
                    "desc": column,
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
            "doc": {
                "desc": "Read the contents of the cell at the intersection of the row and column provided.",
                "examples":
                """
                Examples:
                
                | ``Read Cell     row_locator     column``
                """
            }
        },
        "SaveScreenshot": {
            "name": "Save Screenshot",
            "args": {
                "filepath": {
                    "name": "destination",
                    "desc": "Either the absolute path to a .png file or LOG to embed the image in the protocol.",
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
            "doc": {
                "desc": "Save a screenshot of the current window to the given destination.",
                "examples":
                f"""
                Examples:
                
                | ``Save Screenshot     destination``
                
                *Hint*: {path}
                """
            }
        },
        "ScrollTextFieldContents": {
            "name": "Scroll Contents",
            "args": {
                "a1direction": {
                    "name": "direction",
                    "desc": "UP, DOWN, BEGIN, END",
                    "spec": {}
                },
                "a2untilTextField": {
                    "name": "until_textfield",
                    "desc": textfield_locator,
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
            "doc": {
                "desc": "Scroll the contents of the text fields within an area with a scrollbar.",
                "examples":
                """
                Examples:
                
                | ``Scroll Contents    direction``

                If the parameter "until_textfield" is provided, the contents are scrolled until that text field is found.

                | ``Scroll Contents    direction    until_textfield``
                """
            }
        },
        "ScrollWindowHorizontally": {
            "name": "Scroll Window Horizontally",
            "args": {
                "direction": {
                    "name": "direction",
                    "desc": "LEFT, RIGHT, BEGIN, END",
                    "spec": {}
                }
            },
            "result": {
                "NoSession": no_session,
                "Exception": exception("The window could not be scrolled horizontally."),
                "NoScrollbar": "The window contains no horizontal scrollbar.",
                "MaximumReached": "The window cannot be scrolled any further.",
                "InvalidDirection": "Invalid direction. The direction must be one of: LEFT, RIGHT, BEGIN, END",
                "Pass": "The window was scrolled horizontally in the direction '{0}'."
            },
            "doc": {
                "desc": "Displace the horizontal scrollbar of the window in the given direction.",
                "examples":
                """
                Examples:
                
                | ``Scroll Window Horizontally    direction``
                """
            }
        },
        "SelectCell": {
            "name": "Select Cell",
            "args": {
                "a1row_locator": {
                    "name": "row_locator",
                    "desc": row_locator,
                    "spec": {},

                },
                "a2column": {
                    "name": "column",
                    "desc": column,
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
            "doc": {
                "desc": "Select the cell at the intersection of the row and column provided.",
                "examples":
                """
                Examples:
                
                | ``Select Cell     row_locator     column``
                """
            }
        },
        "SelectCellValue": {
            "name": "Select Cell Value",
            "args": {
                "a1row_locator": {
                    "name": "row_locator",
                    "desc": row_locator,
                    "spec": {},

                },
                "a2column": {
                    "name": "column",
                    "desc": column,
                    "spec": {},

                },
                "a3entry": {
                    "name": "value",
                    "desc": "An entry from the dropdown menu",
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
            "doc": {
                "desc": "Select the specified value in the cell at the intersection of the row and column provided.",
                "examples":
                f"""
                Examples:
                
                | ``Select Cell Value    row_locator    column    value``
                """
            }
        },
        "SelectComboBoxEntry": {
            "name": "Select Dropdown Menu Entry",
            "args": {
                "a1comboBox": {
                    "name": "dropdown_menu",
                    "desc": "The label of the dropdown menu",
                    "spec": {},

                },
                "a2entry": {
                    "name": "entry",
                    "desc": "An entry from the dropdown menu",
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
            "doc": {
                "desc": "Select the specified entry from the dropdown menu provided.",
                "examples":
                """
                Examples:
                
                | ``Select Dropdown Menu Entry   dropdown_menu    entry``

                *Hints*: The numeric key that enables simplified keyboard input is not part of the entry name.

                To select a value from a toolbar button with a dropdown menu, first push the button and then use this keyword. 
                """
            }
        },
        "SelectMenuItem": {
            "name": "Select Menu Entry",
            "args": {
                "itemPath": {
                    "name": "menu_entry_path",
                    "desc": "The path to the entry with '/' as separator (e.g. System/User Profile/Own Data)",
                    "spec": {},

                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The menu entry '{0}' could not be found."),
                "Pass": "The menu entry '{0}' was selected.",
                "Exception": exception("The menu entry could not be selected. {0}")
            },
            "doc": {
                "desc": "Select the menu entry with the path provided.",
                "examples":
                """
                Examples:
                
                | ``Select Menu Entry    menu_entry_path``
                """
            }
        },
        "SelectRadioButton": {
            "name": "Select Radio Button",
            "args": {
                "locator": {
                    "name": "locator",
                    "desc": "A locator used to find the radio button",
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
            "doc": {
                "desc": "Select the radio button specified by the locator.",
                "examples":
                """
                Examples:
                
                *Radio button with a label to its left or its right*
                | ``Select Radio Button    label``
                
                *Radio button with a label above it*
                | ``Select Radio Button    @ label``
                
                *Radio button at the intersection of a label to its left or its right and a label above it*
                | ``Select Radio Button    left or right label @ label above``
                """
            }
        },
        "SelectTableRow": {
            "name": "Select Table Row",
            "args": {
                "row_locator": {
                    "name": "row_locator",
                    "desc": row_locator,
                    "spec": {},
                }
            },
            "result": {
                "NoSession": no_session,
                "Exception": exception("The row could not be selected. {0}"),
                "NoTable": "The window contains no table",
                "InvalidIndex": "The table does not have a row with index '{0}'",
                "NotFound": "The table does not contain a cell with the contents '{0}'",
                "Pass": "The row with the locator '{0}' was selected"
            },
            "doc": {
                "desc": "Select the specified table row.",
                "examples":
                f"""
                Examples:
                
                | ``Select Table Row    row_locator``

                *Hint*: Use the row number 0 to select the whole table.
                """
            }
        },
        "SelectTextField": {
            "name": "Select Text Field",
            "args": {
                "locator": {
                    "name": "locator",
                    "desc": textfield_locator,
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
            "doc": {
                "desc": "Select the text field specified by the locator.",
                "examples":
                """
                Examples:
                
                | ``Select Text Field    locator``
                """
            }
        },
        "SelectText": {
            "name": "Select Text",
            "args": {
                "locator": {
                    "name": "locator",
                    "desc": "The locator used to find the text",
                    "spec": {},

                }
            },
            "result": {
                "NoSession": no_session,
                "NotFound": not_found("The text with the locator '{0}' could not be found."),
                "Pass": "The text with the locator '{0}' was selected.",
                "Exception": exception("The text could not be selected. {0}")
            },
            "doc": {
                "desc": "Select the text specified by the locator.",
                "examples":
                """
                Examples:
                
                *Text starting with a given substring*
                | ``Select Text    = substring``
                
                *Text following a label*
                | ``Select Text    Label``
                """
            }
        },
        "TickCheckBox": {
            "name": "Tick Checkbox",
            "args": {
                "locator": {
                    "name": "locator",
                    "desc": "A locator used to find the checkbox",
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
            "doc": {
                "desc": "Tick the checkbox specified by the locator.",
                "examples":
                """
                Examples:
                
                *Checkbox with a label to its left or its right*
                | ``Tick Checkbox    label``
                
                *Checkbox with a label above it*
                | ``Tick Checkbox    @ label``
                
                *Checkbox at the intersection of a label to its left and a label above it*
                | ``Tick Checkbox    left label @ label above``
                """
            }
        },
        "UntickCheckBox": {
            "name": "Untick Checkbox",
            "args": {
                "locator": {
                    "name": "locator",
                    "desc": "A locator used to find the checkbox",
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
            "doc": {
                "desc": "Untick the checkbox specified by the locator.",
                "examples":
                """
                Examples:
                
                *Checkbox with a label to its left or its right*
                | ``Untick Checkbox    label``
                
                *Checkbox with a label above it*
                | ``Untick Checkbox    @ label``
                
                *Checkbox at the intersection of a label to its left and a label above it*
                | ``Untick Checkbox    left label @ label above``
                """
            }
        },
        "TickCheckBoxCell": {
            "name": "Tick Checkbox Cell",
            "args": {
                "a1row": {
                    "name": "row_locator",
                    "desc": row_locator,
                    "spec": {},
                },
                "a2column": {
                    "name": "column",
                    "desc": column,
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
            "doc": {
                "desc": "Tick the checkbox cell at the intersection of the row and the column provided.",
                "examples":
                f"""
                Examples:
                
                | ``Tick Checkbox Cell     row_locator    column``

                *Hint*: To tick the checkbox in the leftmost column with no title, select the row and press the "Enter" key.
                """
            }
        },
        "UntickCheckBoxCell": {
            "name": "Untick Checkbox Cell",
            "args": {
                "a1row": {
                    "name": "row_locator",
                    "desc": row_locator,
                    "spec": {},

                },
                "a2column": {
                    "name": "column",
                    "desc": column,
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
            "doc": {
                "desc": "Untick the checkbox cell at the intersection of the row and the column provided.",
                "examples":
                """
                Examples:
                
                | ``Untick Checkbox Cell     row_locator    column``
                """
            }
        },
        "GetWindowTitle": {
            "name": "Get Window Title",
            "args": {},
            "result": {
                "NoSession": no_session,
                "Pass": "The title of the window was obtained.",
                "Exception": exception("The window title could not be read.")
            },
            "doc": {
                "desc": "Get the title of the window in the foreground.",
                "examples":
                """
                Examples:
                
                | ``${title}    Get Window Title``
                """
            }
        },
        "GetWindowText": {
            "name": "Get Window Text",
            "args": {},
            "result": {
                "NoSession": no_session,
                "Pass": "The text message of the window was obtained.",
                "Exception": exception("The text message of the window could not be read.")
            },
            "doc": {
                "desc": "Get the text message of the window in the foreground.",
                "examples":
                """
                Examples:
                
                | ``${text}    Get Window Text``
                """
            }
        }
    },
    "specs": {}
}
