# C#

## Architecture

- Split RoboSAPiens into an XML-RPC server that implements RF's Remote API (RFRemote)
  and a C# library that allows automating the SAP GUI using text selectors (SAPiens)

- It should be possible to compile SAPiens to a standalone .dll using .NET 7.0. Then
  it is not necessary to install the .NET Runtime nor use RPC. The .dll can be used 
  in Python using python.NET.


## Design

- SAPiens is a CRUD application

- Data-oriented functional design (with the right data structures the code will be simple)

- Organize the code in modules
    - SAPTree: Contains static methods that deal with trees
    - SAPTable: Contains static methods that deal with tables

- Optimize for adding new search strategies (functional extensibility rather than OO)

- DRY


## SAP.GUI.Scripting.net is no longer maintained

Consider forking it.

The DLLs are generated with Tlbimp.exe, which is part of Windows SDK and .NET 4.8.


## Use attributes to associate locators with keyword arguments

See http://www.codinginstinct.com/2008/05/argument-validation-using-attributes.html

When exporting the API it would be possible to specify which locators are supported
by each keyword. Each locator can be associated with a regex using a dictionary.
This would allow verifying on the Python side whether a keyword argument complies 
with the locator's format.


## Remove invalid characters from filenames

Keywords affected:

- Save Screenshot
- Export Form


## Before acting on an element check if it is changeable

If it is not, fail with NotChangeable


## Find buttons, checkboxes, etc. in a tree using row and column labels


# Python

## Consider reading doc.init from a Markdown file

It has to be converted to Restructured Text
