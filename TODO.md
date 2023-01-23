# C#

## Architecture

- Split RoboSAPiens in an XML-RPC server that implements RF's Remote API (RFRemote)
  and a C# library that allows automating the SAP GUI using text selectors (SAPiens)

- It should be possible to compile SAPiens to a standalone .dll using .NET 7.0. Then
  it is not necessary to install the .NET Runtime. The .dll can be used in Python using
  python.NET. RoboSAPiens would become a dynamic library.


## Design

- SAPiens is a CRUD application

- Data-oriented functional design

- Organize the code in modules
  - components
    - SAPTree: Contains static methods that deal with trees
    - SAPTable: Contains static methods that deal with tables


## Documentation

- The documentation of the keywords should reside on the Python side. This allows editing the documentation without having to recompile and also enables translating the documentation to other languages.


## Export form

Refactor this function:
    - Simple and clear
    - Do not depend on a CSV library


## GuiHTMLViewer

When a GuiShell has subtype GuiHTMLViewer, take a screenshot and use Tesseract to get the coordinates of the labels. Then we can use the usual strategies to locate labels. Or try to get the coordinates from the Browser object. That way we spare a dependency.

GuiHTMLViewer.BrowserHandle.document.all(0).innerText


## Headless mode

As described in the [SAP GUI Scripting API documentation](https://www.synactive.com/download/sap%20gui%20scripting/sap%20gui%20scripting%20api.pdf), the GUI can be embedded in another application (that supports ActiveX controls). For example Office, Internet Explorer. This can be used for headless automation:

https://help.qualibrate.com/space/QXP/3960186362/Headless+Execution


## Highlight Element

- Refactor the Keyword "Knopfhervorhebung Umschalten" to become "Highlight Element", which highlights any element.


## New implementation of the component DB

- With the right data structures the code will be simple


## Pop-up windows

- After pushing a button check if a new window was added to the session

- If a new window was created classify its components

- Define a keyword "SAP Protokoll analysieren" in Python.
  This keyword calls "Maske exportieren", reads the CSV file,
  parses the list (a bunch of labels arranged in a grid),
  and calls "Log" for each entry in the the SAP protocol.
  If an error is present it calls "Fail" with the first error.

  Using the keyword "Überschrift überprüfen" the user can 
  determine if the current window is "Protokolle anzeigen" 
  in which case the keyword "SAP Protokoll analysieren" will
  be called.


## Record & Replay

- The GuiSession object supports recording changes made to GUI elements and sends the changes to a listening server.
  This functionality can be used for recording Robot Framework scripts, which can be later refactored into keywords. 


## Refactor the code for finding components

- Optimize for adding new search strategies (functional extensibility rather than OO)


## RobotResult

- Begin each error message with the name of the current error subclass of RobotResult.
   - Useful for testing that the expected error is thrown


## Statusbar

- Always read the statusbar and return the message to the user

- Poll the statusbar of the main window after clicking a button on any window

  When clicking a button on a dialog window, an error message might
  show up in the statusbar of the main window


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

Try this:

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

  - Start SAP
    - Start SAP with a wrong path should throw an exception indicating this
    - Start SAP should be idempotent
    - Start SAP followed by Exit SAP should work


## Windows

- Check if a new window was added to the session and notify the Python client
  The client has to decide how to deal with it

  More generally, use a message-passing approach for the communication between
  the server and the client.


# Python

## Compare forms

Given two CSV files corresponding to the data models of two forms,
determine the differences between the two files.

Given the screenshots corresponding to the two forms, draw rectangles
around the elements that differ in the two forms. Attach the screenshots
to the RF report.
