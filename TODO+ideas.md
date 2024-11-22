
## Continuous Integration

- Static analysis
- Linter
- Tests
- Build: RoboSAPiens 32-bit and 64-bit, Wheel, Documentation
- Publish on Pypi

- Setup the build environment: Appveyor


## Landing page

- GIF showing RF and SAP side-by-side. Using presenter-mode one can see what each keywords does to the GUI.
  - English
  - German

- Link to the docs


## Screenshot on error

For SapError and NotFound set the output to:

*HTML* <img f'src="data:image/png;base64,{screenshot_as_base64}"' width=f"{width}px">


## Non-unique locators

Handle the case when a locator corresponds to more than one component.
The current solution is to always return the first match.


# C#

## Remove invalid characters from filenames

Keywords affected:

- Save Screenshot
- Export Form


## Keywords

### Export form

Refactor this function:
  - Simple and clear


## Headless mode

See [Why you should run UI testing in Windows containers](https://www.pdq.com/blog/ui-testing-in-windows-containers-why/).


## Record & Replay: RF Code Generator

The GuiSession object supports recording changes made to GUI elements and sends the changes to a listening server.

This functionality can be used for recording Robot Framework scripts, which can be later refactored into keywords. 


## ABAP Lists

An ABAP List constists of a bunch of labels arranged in a grid

- Some of them can be exported as Markdown tables using the Menu Item: 
  System -> List -> Save to file -> Unconverted


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
