from typing_extensions import Literal, TypedDict

class RoboSAPiensKeywordsTickcheckboxcellArgsColumnSpec(TypedDict):
    ...

class RoboSAPiensKeywordsTickcheckboxcellArgsRowSpec(TypedDict):
    ...

class RoboSAPiensKeywordsUntickcheckboxArgsLocatorSpec(TypedDict):
    HLabel: str
    VLabel: str
    HLabelVLabel: str

class RoboSAPiensKeywordsTickcheckboxArgsLocatorSpec(TypedDict):
    HLabel: str
    VLabel: str
    HLabelVLabel: str

class RoboSAPiensKeywordsSelecttextlineArgsContentSpec(TypedDict):
    ...

class RoboSAPiensKeywordsSelecttextfieldArgsLocatorSpec(TypedDict):
    HLabel: str
    VLabel: str
    HLabelVLabel: str
    Content: str

class RoboSAPiensKeywordsSelectradiobuttonArgsLocatorSpec(TypedDict):
    HLabel: str
    VLabel: str
    HLabelVLabel: str

class RoboSAPiensKeywordsSelectcomboboxentryArgsEntrySpec(TypedDict):
    ...

class RoboSAPiensKeywordsSelectcomboboxentryArgsComboboxSpec(TypedDict):
    ...

class RoboSAPiensKeywordsSelectcellArgsColumnSpec(TypedDict):
    ...

class RoboSAPiensKeywordsSelectcellArgsRow_LocatorSpec(TypedDict):
    ...

class RoboSAPiensKeywordsSavescreenshotArgsFilepathSpec(TypedDict):
    ...

class RoboSAPiensKeywordsReadtablecellArgsColumnSpec(TypedDict):
    ...

class RoboSAPiensKeywordsReadtablecellArgsRow_LocatorSpec(TypedDict):
    ...

class RoboSAPiensKeywordsReadtextArgsLocatorSpec(TypedDict):
    Content: str
    HLabel: str

class RoboSAPiensKeywordsReadtextfieldArgsLocatorSpec(TypedDict):
    HLabel: str
    VLabel: str
    HLabelVLabel: str
    Content: str

class RoboSAPiensKeywordsPresskeycombinationArgsKeycombinationSpec(TypedDict):
    ...

class RoboSAPiensKeywordsSelecttablerowArgsRow_NumberSpec(TypedDict):
    ...

class RoboSAPiensKeywordsPushbuttoncellArgsColumnSpec(TypedDict):
    ...

class RoboSAPiensKeywordsPushbuttoncellArgsRow_Or_LabelSpec(TypedDict):
    ...

class RoboSAPiensKeywordsHighlightbuttonArgsButtonSpec(TypedDict):
    ...

class RoboSAPiensKeywordsPushbuttonArgsButtonSpec(TypedDict):
    ...

class RoboSAPiensKeywordsFilltextfieldArgsContentSpec(TypedDict):
    ...

class RoboSAPiensKeywordsFilltextfieldArgsLocatorSpec(TypedDict):
    HLabel: str
    VLabel: str
    HLabelVLabel: str
    HIndexVLabel: str
    HLabelVIndex: str
    HLabelHLabel: str

class RoboSAPiensKeywordsFilltablecellArgsColumn_ContentSpec(TypedDict):
    ColumnContent: str

class RoboSAPiensKeywordsFilltablecellArgsRow_LocatorSpec(TypedDict):
    ...

class RoboSAPiensKeywordsExportformArgsDirectorySpec(TypedDict):
    ...

class RoboSAPiensKeywordsExportformArgsNameSpec(TypedDict):
    ...

class RoboSAPiensKeywordsExportspreadsheetArgsIndexSpec(TypedDict):
    ...

class RoboSAPiensKeywordsExecutetransactionArgsT_CodeSpec(TypedDict):
    ...

class RoboSAPiensKeywordsDoubleclicktextfieldArgsContentSpec(TypedDict):
    ...

class RoboSAPiensKeywordsDoubleclickcellArgsColumnSpec(TypedDict):
    ...

class RoboSAPiensKeywordsDoubleclickcellArgsRow_LocatorSpec(TypedDict):
    ...

class RoboSAPiensKeywordsConnecttoserverArgsServerSpec(TypedDict):
    ...

class RoboSAPiensKeywordsExporttreeArgsFilepathSpec(TypedDict):
    ...

class RoboSAPiensKeywordsOpensapArgsPathSpec(TypedDict):
    ...

class RoboSAPiensKeywordsActivatetabArgsTabSpec(TypedDict):
    ...

class RoboSAPiensKeywordsTickcheckboxcellArgsColumn(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsTickcheckboxcellArgsColumnSpec

class RoboSAPiensKeywordsTickcheckboxcellArgsRow(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsTickcheckboxcellArgsRowSpec

class RoboSAPiensKeywordsUntickcheckboxArgsLocator(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsUntickcheckboxArgsLocatorSpec

class RoboSAPiensKeywordsTickcheckboxArgsLocator(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsTickcheckboxArgsLocatorSpec

class RoboSAPiensKeywordsSelecttextlineArgsContent(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsSelecttextlineArgsContentSpec

class RoboSAPiensKeywordsSelecttextfieldArgsLocator(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsSelecttextfieldArgsLocatorSpec

class RoboSAPiensKeywordsSelectradiobuttonArgsLocator(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsSelectradiobuttonArgsLocatorSpec

class RoboSAPiensKeywordsSelectcomboboxentryArgsEntry(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsSelectcomboboxentryArgsEntrySpec

class RoboSAPiensKeywordsSelectcomboboxentryArgsCombobox(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsSelectcomboboxentryArgsComboboxSpec

class RoboSAPiensKeywordsSelectcellArgsColumn(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsSelectcellArgsColumnSpec

class RoboSAPiensKeywordsSelectcellArgsRow_Locator(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsSelectcellArgsRow_LocatorSpec

class RoboSAPiensKeywordsSavescreenshotArgsFilepath(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsSavescreenshotArgsFilepathSpec

class RoboSAPiensKeywordsReadtablecellArgsColumn(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsReadtablecellArgsColumnSpec

class RoboSAPiensKeywordsReadtablecellArgsRow_Locator(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsReadtablecellArgsRow_LocatorSpec

class RoboSAPiensKeywordsReadtextArgsLocator(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsReadtextArgsLocatorSpec

class RoboSAPiensKeywordsReadtextfieldArgsLocator(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsReadtextfieldArgsLocatorSpec

class RoboSAPiensKeywordsPresskeycombinationArgsKeycombination(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsPresskeycombinationArgsKeycombinationSpec

class RoboSAPiensKeywordsSelecttablerowArgsRow_Number(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsSelecttablerowArgsRow_NumberSpec

class RoboSAPiensKeywordsPushbuttoncellArgsColumn(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsPushbuttoncellArgsColumnSpec

class RoboSAPiensKeywordsPushbuttoncellArgsRow_Or_Label(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsPushbuttoncellArgsRow_Or_LabelSpec

class RoboSAPiensKeywordsHighlightbuttonArgsButton(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsHighlightbuttonArgsButtonSpec

class RoboSAPiensKeywordsPushbuttonArgsButton(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsPushbuttonArgsButtonSpec

class RoboSAPiensKeywordsFilltextfieldArgsContent(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsFilltextfieldArgsContentSpec

class RoboSAPiensKeywordsFilltextfieldArgsLocator(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsFilltextfieldArgsLocatorSpec

class RoboSAPiensKeywordsFilltablecellArgsColumn_Content(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsFilltablecellArgsColumn_ContentSpec

class RoboSAPiensKeywordsFilltablecellArgsRow_Locator(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsFilltablecellArgsRow_LocatorSpec

class RoboSAPiensKeywordsExportformArgsDirectory(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsExportformArgsDirectorySpec

class RoboSAPiensKeywordsExportformArgsName(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsExportformArgsNameSpec

class RoboSAPiensKeywordsExportspreadsheetArgsIndex(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsExportspreadsheetArgsIndexSpec

class RoboSAPiensKeywordsExecutetransactionArgsT_Code(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsExecutetransactionArgsT_CodeSpec

class RoboSAPiensKeywordsDoubleclicktextfieldArgsContent(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsDoubleclicktextfieldArgsContentSpec

class RoboSAPiensKeywordsDoubleclickcellArgsColumn(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsDoubleclickcellArgsColumnSpec

class RoboSAPiensKeywordsDoubleclickcellArgsRow_Locator(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsDoubleclickcellArgsRow_LocatorSpec

class RoboSAPiensKeywordsConnecttoserverArgsServer(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsConnecttoserverArgsServerSpec

class RoboSAPiensKeywordsExporttreeArgsFilepath(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsExporttreeArgsFilepathSpec

class RoboSAPiensKeywordsOpensapArgsPath(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsOpensapArgsPathSpec

class RoboSAPiensKeywordsActivatetabArgsTab(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsActivatetabArgsTabSpec

class RoboSAPiensKeywordsGetwindowtextResult(TypedDict):
    NoSession: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsGetwindowtextArgs(TypedDict):
    ...

class RoboSAPiensKeywordsGetwindowtitleResult(TypedDict):
    NoSession: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsGetwindowtitleArgs(TypedDict):
    ...

class RoboSAPiensKeywordsTickcheckboxcellResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsTickcheckboxcellArgs(TypedDict):
    a1row: RoboSAPiensKeywordsTickcheckboxcellArgsRow
    a2column: RoboSAPiensKeywordsTickcheckboxcellArgsColumn

class RoboSAPiensKeywordsUntickcheckboxResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsUntickcheckboxArgs(TypedDict):
    locator: RoboSAPiensKeywordsUntickcheckboxArgsLocator

class RoboSAPiensKeywordsTickcheckboxResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsTickcheckboxArgs(TypedDict):
    locator: RoboSAPiensKeywordsTickcheckboxArgsLocator

class RoboSAPiensKeywordsSelecttextlineResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSelecttextlineArgs(TypedDict):
    content: RoboSAPiensKeywordsSelecttextlineArgsContent

class RoboSAPiensKeywordsSelecttextfieldResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSelecttextfieldArgs(TypedDict):
    locator: RoboSAPiensKeywordsSelecttextfieldArgsLocator

class RoboSAPiensKeywordsSelectradiobuttonResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSelectradiobuttonArgs(TypedDict):
    locator: RoboSAPiensKeywordsSelectradiobuttonArgsLocator

class RoboSAPiensKeywordsSelectcomboboxentryResult(TypedDict):
    NoSession: str
    NotFound: str
    EntryNotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSelectcomboboxentryArgs(TypedDict):
    a1comboBox: RoboSAPiensKeywordsSelectcomboboxentryArgsCombobox
    a2entry: RoboSAPiensKeywordsSelectcomboboxentryArgsEntry

class RoboSAPiensKeywordsSelectcellResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSelectcellArgs(TypedDict):
    a1row_locator: RoboSAPiensKeywordsSelectcellArgsRow_Locator
    a2column: RoboSAPiensKeywordsSelectcellArgsColumn

class RoboSAPiensKeywordsSavescreenshotResult(TypedDict):
    NoSession: str
    UNCPath: str
    NoAbsPath: str
    InvalidPath: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSavescreenshotArgs(TypedDict):
    filepath: RoboSAPiensKeywordsSavescreenshotArgsFilepath

class RoboSAPiensKeywordsReadtablecellResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsReadtablecellArgs(TypedDict):
    a1row_locator: RoboSAPiensKeywordsReadtablecellArgsRow_Locator
    a2column: RoboSAPiensKeywordsReadtablecellArgsColumn

class RoboSAPiensKeywordsReadtextResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsReadtextArgs(TypedDict):
    locator: RoboSAPiensKeywordsReadtextArgsLocator

class RoboSAPiensKeywordsReadtextfieldResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsReadtextfieldArgs(TypedDict):
    locator: RoboSAPiensKeywordsReadtextfieldArgsLocator

class RoboSAPiensKeywordsPresskeycombinationResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsPresskeycombinationArgs(TypedDict):
    keyCombination: RoboSAPiensKeywordsPresskeycombinationArgsKeycombination

class RoboSAPiensKeywordsSelecttablerowResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSelecttablerowArgs(TypedDict):
    row_number: RoboSAPiensKeywordsSelecttablerowArgsRow_Number

class RoboSAPiensKeywordsPushbuttoncellResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsPushbuttoncellArgs(TypedDict):
    a1row_or_label: RoboSAPiensKeywordsPushbuttoncellArgsRow_Or_Label
    a2column: RoboSAPiensKeywordsPushbuttoncellArgsColumn

class RoboSAPiensKeywordsReadstatusbarResult(TypedDict):
    NoSession: str
    Pass: str
    Exception: str
    NotFound: str

class RoboSAPiensKeywordsReadstatusbarArgs(TypedDict):
    ...

class RoboSAPiensKeywordsHighlightbuttonResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsHighlightbuttonArgs(TypedDict):
    button: RoboSAPiensKeywordsHighlightbuttonArgsButton

class RoboSAPiensKeywordsPushbuttonResult(TypedDict):
    NoSession: str
    SapError: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsPushbuttonArgs(TypedDict):
    button: RoboSAPiensKeywordsPushbuttonArgsButton

class RoboSAPiensKeywordsFilltextfieldResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsFilltextfieldArgs(TypedDict):
    a1locator: RoboSAPiensKeywordsFilltextfieldArgsLocator
    a2content: RoboSAPiensKeywordsFilltextfieldArgsContent

class RoboSAPiensKeywordsFilltablecellResult(TypedDict):
    NoSession: str
    InvalidFormat: str
    NotFound: str
    NotChangeable: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsFilltablecellArgs(TypedDict):
    a1row_locator: RoboSAPiensKeywordsFilltablecellArgsRow_Locator
    a2column_content: RoboSAPiensKeywordsFilltablecellArgsColumn_Content

class RoboSAPiensKeywordsExportformResult(TypedDict):
    NoSession: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsExportformArgs(TypedDict):
    a1name: RoboSAPiensKeywordsExportformArgsName
    a2directory: RoboSAPiensKeywordsExportformArgsDirectory

class RoboSAPiensKeywordsExportspreadsheetResult(TypedDict):
    NoSession: str
    Pass: str
    Exception: str
    NotFound: str

class RoboSAPiensKeywordsExportspreadsheetArgs(TypedDict):
    index: RoboSAPiensKeywordsExportspreadsheetArgsIndex

class RoboSAPiensKeywordsExecutetransactionResult(TypedDict):
    NoSession: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsExecutetransactionArgs(TypedDict):
    T_Code: RoboSAPiensKeywordsExecutetransactionArgsT_Code

class RoboSAPiensKeywordsDoubleclicktextfieldResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsDoubleclicktextfieldArgs(TypedDict):
    content: RoboSAPiensKeywordsDoubleclicktextfieldArgsContent

class RoboSAPiensKeywordsDoubleclickcellResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsDoubleclickcellArgs(TypedDict):
    a1row_locator: RoboSAPiensKeywordsDoubleclickcellArgsRow_Locator
    a2column: RoboSAPiensKeywordsDoubleclickcellArgsColumn

class RoboSAPiensKeywordsConnecttoserverResult(TypedDict):
    NoSapGui: str
    NoGuiScripting: str
    Pass: str
    SapError: str
    NoServerScripting: str
    Exception: str

class RoboSAPiensKeywordsConnecttoserverArgs(TypedDict):
    server: RoboSAPiensKeywordsConnecttoserverArgsServer

class RoboSAPiensKeywordsAttachtorunningsapResult(TypedDict):
    NoSapGui: str
    NoGuiScripting: str
    NoConnection: str
    NoServerScripting: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsAttachtorunningsapArgs(TypedDict):
    ...

class RoboSAPiensKeywordsExporttreeResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsExporttreeArgs(TypedDict):
    filepath: RoboSAPiensKeywordsExporttreeArgsFilepath

class RoboSAPiensKeywordsClosesapResult(TypedDict):
    NoSapGui: str
    Pass: str

class RoboSAPiensKeywordsClosesapArgs(TypedDict):
    ...

class RoboSAPiensKeywordsCloseconnectionResult(TypedDict):
    NoSapGui: str
    NoGuiScripting: str
    NoConnection: str
    NoSession: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsCloseconnectionArgs(TypedDict):
    ...

class RoboSAPiensKeywordsOpensapResult(TypedDict):
    Pass: str
    SAPNotStarted: str
    Exception: str

class RoboSAPiensKeywordsOpensapArgs(TypedDict):
    path: RoboSAPiensKeywordsOpensapArgsPath

class RoboSAPiensKeywordsActivatetabResult(TypedDict):
    NoSession: str
    NotFound: str
    SapError: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsActivatetabArgs(TypedDict):
    tab: RoboSAPiensKeywordsActivatetabArgsTab

class RoboSAPiensKeywordsGetwindowtext(TypedDict):
    name: str
    args: RoboSAPiensKeywordsGetwindowtextArgs
    result: RoboSAPiensKeywordsGetwindowtextResult
    doc: str

class RoboSAPiensKeywordsGetwindowtitle(TypedDict):
    name: str
    args: RoboSAPiensKeywordsGetwindowtitleArgs
    result: RoboSAPiensKeywordsGetwindowtitleResult
    doc: str

class RoboSAPiensKeywordsTickcheckboxcell(TypedDict):
    name: str
    args: RoboSAPiensKeywordsTickcheckboxcellArgs
    result: RoboSAPiensKeywordsTickcheckboxcellResult
    doc: str

class RoboSAPiensKeywordsUntickcheckbox(TypedDict):
    name: str
    args: RoboSAPiensKeywordsUntickcheckboxArgs
    result: RoboSAPiensKeywordsUntickcheckboxResult
    doc: str

class RoboSAPiensKeywordsTickcheckbox(TypedDict):
    name: str
    args: RoboSAPiensKeywordsTickcheckboxArgs
    result: RoboSAPiensKeywordsTickcheckboxResult
    doc: str

class RoboSAPiensKeywordsSelecttextline(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSelecttextlineArgs
    result: RoboSAPiensKeywordsSelecttextlineResult
    doc: str

class RoboSAPiensKeywordsSelecttextfield(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSelecttextfieldArgs
    result: RoboSAPiensKeywordsSelecttextfieldResult
    doc: str

class RoboSAPiensKeywordsSelectradiobutton(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSelectradiobuttonArgs
    result: RoboSAPiensKeywordsSelectradiobuttonResult
    doc: str

class RoboSAPiensKeywordsSelectcomboboxentry(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSelectcomboboxentryArgs
    result: RoboSAPiensKeywordsSelectcomboboxentryResult
    doc: str

class RoboSAPiensKeywordsSelectcell(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSelectcellArgs
    result: RoboSAPiensKeywordsSelectcellResult
    doc: str

class RoboSAPiensKeywordsSavescreenshot(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSavescreenshotArgs
    result: RoboSAPiensKeywordsSavescreenshotResult
    doc: str

class RoboSAPiensKeywordsReadtablecell(TypedDict):
    name: str
    args: RoboSAPiensKeywordsReadtablecellArgs
    result: RoboSAPiensKeywordsReadtablecellResult
    doc: str

class RoboSAPiensKeywordsReadtext(TypedDict):
    name: str
    args: RoboSAPiensKeywordsReadtextArgs
    result: RoboSAPiensKeywordsReadtextResult
    doc: str

class RoboSAPiensKeywordsReadtextfield(TypedDict):
    name: str
    args: RoboSAPiensKeywordsReadtextfieldArgs
    result: RoboSAPiensKeywordsReadtextfieldResult
    doc: str

class RoboSAPiensKeywordsPresskeycombination(TypedDict):
    name: str
    args: RoboSAPiensKeywordsPresskeycombinationArgs
    result: RoboSAPiensKeywordsPresskeycombinationResult
    doc: str

class RoboSAPiensKeywordsSelecttablerow(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSelecttablerowArgs
    result: RoboSAPiensKeywordsSelecttablerowResult
    doc: str

class RoboSAPiensKeywordsPushbuttoncell(TypedDict):
    name: str
    args: RoboSAPiensKeywordsPushbuttoncellArgs
    result: RoboSAPiensKeywordsPushbuttoncellResult
    doc: str

class RoboSAPiensKeywordsReadstatusbar(TypedDict):
    name: str
    args: RoboSAPiensKeywordsReadstatusbarArgs
    result: RoboSAPiensKeywordsReadstatusbarResult
    doc: str

class RoboSAPiensKeywordsHighlightbutton(TypedDict):
    name: str
    args: RoboSAPiensKeywordsHighlightbuttonArgs
    result: RoboSAPiensKeywordsHighlightbuttonResult
    doc: str

class RoboSAPiensKeywordsPushbutton(TypedDict):
    name: str
    args: RoboSAPiensKeywordsPushbuttonArgs
    result: RoboSAPiensKeywordsPushbuttonResult
    doc: str

class RoboSAPiensKeywordsFilltextfield(TypedDict):
    name: str
    args: RoboSAPiensKeywordsFilltextfieldArgs
    result: RoboSAPiensKeywordsFilltextfieldResult
    doc: str

class RoboSAPiensKeywordsFilltablecell(TypedDict):
    name: str
    args: RoboSAPiensKeywordsFilltablecellArgs
    result: RoboSAPiensKeywordsFilltablecellResult
    doc: str

class RoboSAPiensKeywordsExportform(TypedDict):
    name: str
    args: RoboSAPiensKeywordsExportformArgs
    result: RoboSAPiensKeywordsExportformResult
    doc: str

class RoboSAPiensKeywordsExportspreadsheet(TypedDict):
    name: str
    args: RoboSAPiensKeywordsExportspreadsheetArgs
    result: RoboSAPiensKeywordsExportspreadsheetResult
    doc: str

class RoboSAPiensKeywordsExecutetransaction(TypedDict):
    name: str
    args: RoboSAPiensKeywordsExecutetransactionArgs
    result: RoboSAPiensKeywordsExecutetransactionResult
    doc: str

class RoboSAPiensKeywordsDoubleclicktextfield(TypedDict):
    name: str
    args: RoboSAPiensKeywordsDoubleclicktextfieldArgs
    result: RoboSAPiensKeywordsDoubleclicktextfieldResult
    doc: str

class RoboSAPiensKeywordsDoubleclickcell(TypedDict):
    name: str
    args: RoboSAPiensKeywordsDoubleclickcellArgs
    result: RoboSAPiensKeywordsDoubleclickcellResult
    doc: str

class RoboSAPiensKeywordsConnecttoserver(TypedDict):
    name: str
    args: RoboSAPiensKeywordsConnecttoserverArgs
    result: RoboSAPiensKeywordsConnecttoserverResult
    doc: str

class RoboSAPiensKeywordsAttachtorunningsap(TypedDict):
    name: str
    args: RoboSAPiensKeywordsAttachtorunningsapArgs
    result: RoboSAPiensKeywordsAttachtorunningsapResult
    doc: str

class RoboSAPiensKeywordsExporttree(TypedDict):
    name: str
    args: RoboSAPiensKeywordsExporttreeArgs
    result: RoboSAPiensKeywordsExporttreeResult
    doc: str

class RoboSAPiensKeywordsClosesap(TypedDict):
    name: str
    args: RoboSAPiensKeywordsClosesapArgs
    result: RoboSAPiensKeywordsClosesapResult
    doc: str

class RoboSAPiensKeywordsCloseconnection(TypedDict):
    name: str
    args: RoboSAPiensKeywordsCloseconnectionArgs
    result: RoboSAPiensKeywordsCloseconnectionResult
    doc: str

class RoboSAPiensKeywordsOpensap(TypedDict):
    name: str
    args: RoboSAPiensKeywordsOpensapArgs
    result: RoboSAPiensKeywordsOpensapResult
    doc: str

class RoboSAPiensKeywordsActivatetab(TypedDict):
    name: str
    args: RoboSAPiensKeywordsActivatetabArgs
    result: RoboSAPiensKeywordsActivatetabResult
    doc: str

class RoboSAPiensArgsPresenter_Mode(TypedDict):
    name: str
    default: Literal[False]
    doc: str

class RoboSAPiensSpecs(TypedDict):
    ...

class RoboSAPiensKeywords(TypedDict):
    ActivateTab: RoboSAPiensKeywordsActivatetab
    OpenSap: RoboSAPiensKeywordsOpensap
    CloseConnection: RoboSAPiensKeywordsCloseconnection
    CloseSap: RoboSAPiensKeywordsClosesap
    ExportTree: RoboSAPiensKeywordsExporttree
    AttachToRunningSap: RoboSAPiensKeywordsAttachtorunningsap
    ConnectToServer: RoboSAPiensKeywordsConnecttoserver
    DoubleClickCell: RoboSAPiensKeywordsDoubleclickcell
    DoubleClickTextField: RoboSAPiensKeywordsDoubleclicktextfield
    ExecuteTransaction: RoboSAPiensKeywordsExecutetransaction
    ExportSpreadsheet: RoboSAPiensKeywordsExportspreadsheet
    ExportForm: RoboSAPiensKeywordsExportform
    FillTableCell: RoboSAPiensKeywordsFilltablecell
    FillTextField: RoboSAPiensKeywordsFilltextfield
    PushButton: RoboSAPiensKeywordsPushbutton
    HighlightButton: RoboSAPiensKeywordsHighlightbutton
    ReadStatusbar: RoboSAPiensKeywordsReadstatusbar
    PushButtonCell: RoboSAPiensKeywordsPushbuttoncell
    SelectTableRow: RoboSAPiensKeywordsSelecttablerow
    PressKeyCombination: RoboSAPiensKeywordsPresskeycombination
    ReadTextField: RoboSAPiensKeywordsReadtextfield
    ReadText: RoboSAPiensKeywordsReadtext
    ReadTableCell: RoboSAPiensKeywordsReadtablecell
    SaveScreenshot: RoboSAPiensKeywordsSavescreenshot
    SelectCell: RoboSAPiensKeywordsSelectcell
    SelectComboBoxEntry: RoboSAPiensKeywordsSelectcomboboxentry
    SelectRadioButton: RoboSAPiensKeywordsSelectradiobutton
    SelectTextField: RoboSAPiensKeywordsSelecttextfield
    SelectTextLine: RoboSAPiensKeywordsSelecttextline
    TickCheckBox: RoboSAPiensKeywordsTickcheckbox
    UntickCheckBox: RoboSAPiensKeywordsUntickcheckbox
    TickCheckBoxCell: RoboSAPiensKeywordsTickcheckboxcell
    GetWindowTitle: RoboSAPiensKeywordsGetwindowtitle
    GetWindowText: RoboSAPiensKeywordsGetwindowtext

class RoboSAPiensArgs(TypedDict):
    presenter_mode: RoboSAPiensArgsPresenter_Mode

class RoboSAPiensDoc(TypedDict):
    intro: str
    init: str

class RoboSAPiens(TypedDict):
    doc: RoboSAPiensDoc
    args: RoboSAPiensArgs
    keywords: RoboSAPiensKeywords
    specs: RoboSAPiensSpecs