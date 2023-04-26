
## Continuous Integration

- Static analysis
- Tests
- Linter
- Build: RoboSAPiens 32-bit and 64-bit, Wheel, Documentation
- Publish on Pypi

- Chooose a build tool: just, cake, tox, nox, invoke
- Setup the build environment: Appveyor


## Support SAP GUI 8.0

It is 64-bit. It will probably be necessary to generate the DLLs.

Compile the project once for 32-bit and once for 64-bit.

Copy the resulting artifacts to python/src/RoboSAPiens/lib32,lib64

Add the parameter `32_bit: bool = True` to the Python library. It will be used to determine which RoboSAPiens.exe to execute. Delete it from args before passing args to start_server.te

When exporting the API in C# add

new Arg("32-bit", "Whether to launch the 32-bit server or the 64-bit server", default: true, export: true).


# C#

## Keywords

### Export form

Refactor this function:
  - Simple and clear
  - Do not depend on a CSV library

### Highlight Elements

- Implement the keyword "Highlight Elements", which highlights all the elements matching a locator.

Should accept the following selectors: HLabel, @ VLabel, HLabel @ VLabel, HLabel >> HLabel, etc.

### Press Key

### Read Statusbar

- Read the statusbar and return the message to the user

- Usually only the main window has a statusbar. If the current window is a modal window read the statusbar of the main window.

### Select Row

Args: row_index: int, table=None

1. If there is more than one table, find the table using the title of the enclosing box.
2. Get the row and select it.


## GuiHTMLViewer

When a GuiShell has subtype GuiHTMLViewer, take a screenshot and use Tesseract to get the coordinates of the labels. Then we can use the usual strategies to locate labels. Or try to get the coordinates from the Browser object. That way we spare a dependency.

GuiHTMLViewer.BrowserHandle.document.all(0).innerText


## Headless mode

As described in the [SAP GUI Scripting API documentation](https://www.synactive.com/download/sap%20gui%20scripting/sap%20gui%20scripting%20api.pdf), the GUI can be embedded in another application (that supports ActiveX controls). For example Office, Internet Explorer. This can be used for headless automation:

https://help.qualibrate.com/space/QXP/3960186362/Headless+Execution


## Record & Replay: RF Code Generator

The GuiSession object supports recording changes made to GUI elements and sends the changes to a listening server.

This functionality can be used for recording Robot Framework scripts, which can be later refactored into keywords. 


## Tables

- Add an abstract class Table

- Add the following implementations of the Table class:
  - SAPTable
  - GridView
  - List (a bunch of labels arranged in a grid)

- Lists can be exported as Markdown tables using the Menu Item: 
  System -> List -> Save to file -> Unconverted

- Some tables can be exported as Excel spreadsheets


## TestToolMode

GuiSession.TestToolMode = 1

While success (S), warning (W) and error (E) messages are always displayed
in the statusbar, information (I) and abort (A) messages are displayed as pop-up
windows unless testToolMode is set. 

System messages are ignored so that they do not interrupt the recording or
playback of scripts.


## Tests

- Add unit tests
  - Functions that implement search strategies

- Add acceptance tests implemented in Robot Framework
  - One testsuite per keyword (using its English name from C#)
  
  - Create a test for each issue encountered by users.


  - Start SAP
    - Start SAP with a wrong path should throw an exception indicating this
    - Start SAP should be idempotent
    - Start SAP followed by Exit SAP should work


## SAPito: SAP GUI Mock

- Use draw.io to define a library of GUI components, having the same attributes as the GUI components from the SAP GUI Scripting API

- Design UIs for tests using draw.io and export them to XML

- Useful for testing search strategies

  - Find a text field when the label and the text field overlap


# Python

## Assistant

A small window in the lower left or upper left corner of the screen that shows the last keyword called. (Similar to the option `show_keyword_call_banner` of the Browser library)

A small window that shows the RF code generated after each user action in the GUI. (Similar to playwright code-gen)


## Keywords

### Support extension through Python keywords

- contrib is a Python package that contains Python modules implementing keywords:

    - SAP2TestBench
    - VisualDiff

- These keywords are not translated


### Analyze SAP Protocol

Define a keyword "Analyze SAP Protocol" in Python.
This keyword calls "Export Dynpro", reads the CSV file,
parses the list (a bunch of labels arranged in a grid),
and calls "Log" for each entry in the SAP protocol.
If an error is present it calls "Fail" with the first error.

### Compare forms

Given two CSV files corresponding to the data models of two forms,
determine the differences between the two files.

Given the screenshots corresponding to the two forms, draw rectangles
around the elements that differ in the two forms. Attach the screenshots
to the RF report.

Support passing a list of fields to ignore (e.g. system)
