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


## Export tree

- As a list of lists


## Export form

Refactor this function:
    - Simple and clear
    - Do not depend on a CSV library


## GuiHTMLViewer

GuiHTMLViewer.BrowserHandle.document.all(0).innerText


## Headless mode

As described in the [SAP GUI Scripting API documentation](https://www.synactive.com/download/sap%20gui%20scripting/sap%20gui%20scripting%20api.pdf), the GUI can be embedded in another application (that supports ActiveX controls). For example Office, Internet Explorer. This can be used for headless automation:

https://help.qualibrate.com/space/QXP/3960186362/Headless+Execution


## Highlight Element

- Refactor the Keyword "Knopfhervorhebung Umschalten" to become "Highlight Element", which highlights any element.


## New implementation of the component DB

- With the right data structures the code will be simple


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

- Read the statusbar of the main window, not of the current window

  When clicking a button on a dialog window, an error message might
  show up in the statusbar of the main window


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

- Check if a new window was added to session/window and notify the Python client
  The client has to decide how to deal with it

  More generally, use a message-passing approach for the communication between
  the server and the client


# Python

## Compare forms

Given two CSV files corresponding to the data models of two forms,
determine the differences between the two files.

Given the screenshots corresponding to the two forms, draw rectangles
around the elements that differ in the two forms. Attach the screenshots
to the RF report.
