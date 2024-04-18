from typing_extensions import Literal, TypedDict

class RoboSAPiensKeywordsUntickcheckboxcellArgsColumnSpec(TypedDict):
    ...

class RoboSAPiensKeywordsUntickcheckboxcellArgsRowSpec(TypedDict):
    ...

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

class RoboSAPiensKeywordsSelecttextArgsLocatorSpec(TypedDict):
    ...

class RoboSAPiensKeywordsSelecttextfieldArgsLocatorSpec(TypedDict):
    HLabel: str
    VLabel: str
    HLabelVLabel: str
    HIndexVLabel: str
    HLabelVIndex: str
    HLabelHLabel: str
    Content: str

class RoboSAPiensKeywordsSelectradiobuttonArgsLocatorSpec(TypedDict):
    HLabel: str
    VLabel: str
    HLabelVLabel: str

class RoboSAPiensKeywordsSelectcomboboxentryArgsEntrySpec(TypedDict):
    ...

class RoboSAPiensKeywordsSelectcomboboxentryArgsComboboxSpec(TypedDict):
    ...

class RoboSAPiensKeywordsSelectcellvalueArgsEntrySpec(TypedDict):
    ...

class RoboSAPiensKeywordsSelectcellvalueArgsColumnSpec(TypedDict):
    ...

class RoboSAPiensKeywordsSelectcellvalueArgsRow_LocatorSpec(TypedDict):
    ...

class RoboSAPiensKeywordsSelectcellArgsColumnSpec(TypedDict):
    ...

class RoboSAPiensKeywordsSelectcellArgsRow_LocatorSpec(TypedDict):
    ...

class RoboSAPiensKeywordsScrolltextfieldcontentsArgsUntiltextfieldSpec(TypedDict):
    ...

class RoboSAPiensKeywordsScrolltextfieldcontentsArgsDirectionSpec(TypedDict):
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

class RoboSAPiensKeywordsSelecttablerowArgsRow_LocatorSpec(TypedDict):
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

class RoboSAPiensKeywordsFilltablecellArgsContentSpec(TypedDict):
    ...

class RoboSAPiensKeywordsFilltablecellArgsColumnSpec(TypedDict):
    ...

class RoboSAPiensKeywordsFilltablecellArgsRow_LocatorSpec(TypedDict):
    ...

class RoboSAPiensKeywordsExportwindowArgsDirectorySpec(TypedDict):
    ...

class RoboSAPiensKeywordsExportwindowArgsNameSpec(TypedDict):
    ...

class RoboSAPiensKeywordsExecutetransactionArgsT_CodeSpec(TypedDict):
    ...

class RoboSAPiensKeywordsDoubleclicktextfieldArgsLocatorSpec(TypedDict):
    HLabel: str
    VLabel: str
    HLabelVLabel: str
    HIndexVLabel: str
    HLabelVIndex: str
    HLabelHLabel: str
    Content: str

class RoboSAPiensKeywordsDoubleclickcellArgsColumnSpec(TypedDict):
    ...

class RoboSAPiensKeywordsDoubleclickcellArgsRow_LocatorSpec(TypedDict):
    ...

class RoboSAPiensKeywordsConnecttoserverArgsServerSpec(TypedDict):
    ...

class RoboSAPiensKeywordsAttachtorunningsapArgsSessionnumberSpec(TypedDict):
    ...

class RoboSAPiensKeywordsExporttreeArgsFilepathSpec(TypedDict):
    ...

class RoboSAPiensKeywordsOpensapArgsPathSpec(TypedDict):
    ...

class RoboSAPiensKeywordsSelectmenuitemArgsItempathSpec(TypedDict):
    ...

class RoboSAPiensKeywordsSelecttreeelementArgsElementpathSpec(TypedDict):
    ...

class RoboSAPiensKeywordsActivatetabArgsTabSpec(TypedDict):
    ...

class RoboSAPiensKeywordsUntickcheckboxcellArgsColumn(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsUntickcheckboxcellArgsColumnSpec

class RoboSAPiensKeywordsUntickcheckboxcellArgsRow(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsUntickcheckboxcellArgsRowSpec

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

class RoboSAPiensKeywordsSelecttextArgsLocator(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsSelecttextArgsLocatorSpec

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

class RoboSAPiensKeywordsSelectcellvalueArgsEntry(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsSelectcellvalueArgsEntrySpec

class RoboSAPiensKeywordsSelectcellvalueArgsColumn(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsSelectcellvalueArgsColumnSpec

class RoboSAPiensKeywordsSelectcellvalueArgsRow_Locator(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsSelectcellvalueArgsRow_LocatorSpec

class RoboSAPiensKeywordsSelectcellArgsColumn(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsSelectcellArgsColumnSpec

class RoboSAPiensKeywordsSelectcellArgsRow_Locator(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsSelectcellArgsRow_LocatorSpec

class RoboSAPiensKeywordsScrolltextfieldcontentsArgsUntiltextfield(TypedDict):
    name: str
    default: Literal[None]
    spec: RoboSAPiensKeywordsScrolltextfieldcontentsArgsUntiltextfieldSpec

class RoboSAPiensKeywordsScrolltextfieldcontentsArgsDirection(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsScrolltextfieldcontentsArgsDirectionSpec

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

class RoboSAPiensKeywordsSelecttablerowArgsRow_Locator(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsSelecttablerowArgsRow_LocatorSpec

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

class RoboSAPiensKeywordsFilltablecellArgsContent(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsFilltablecellArgsContentSpec

class RoboSAPiensKeywordsFilltablecellArgsColumn(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsFilltablecellArgsColumnSpec

class RoboSAPiensKeywordsFilltablecellArgsRow_Locator(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsFilltablecellArgsRow_LocatorSpec

class RoboSAPiensKeywordsExportwindowArgsDirectory(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsExportwindowArgsDirectorySpec

class RoboSAPiensKeywordsExportwindowArgsName(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsExportwindowArgsNameSpec

class RoboSAPiensKeywordsExecutetransactionArgsT_Code(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsExecutetransactionArgsT_CodeSpec

class RoboSAPiensKeywordsDoubleclicktextfieldArgsLocator(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsDoubleclicktextfieldArgsLocatorSpec

class RoboSAPiensKeywordsDoubleclickcellArgsColumn(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsDoubleclickcellArgsColumnSpec

class RoboSAPiensKeywordsDoubleclickcellArgsRow_Locator(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsDoubleclickcellArgsRow_LocatorSpec

class RoboSAPiensKeywordsConnecttoserverArgsServer(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsConnecttoserverArgsServerSpec

class RoboSAPiensKeywordsAttachtorunningsapArgsSessionnumber(TypedDict):
    name: str
    default: str
    spec: RoboSAPiensKeywordsAttachtorunningsapArgsSessionnumberSpec

class RoboSAPiensKeywordsExporttreeArgsFilepath(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsExporttreeArgsFilepathSpec

class RoboSAPiensKeywordsOpensapArgsPath(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsOpensapArgsPathSpec

class RoboSAPiensKeywordsSelectmenuitemArgsItempath(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsSelectmenuitemArgsItempathSpec

class RoboSAPiensKeywordsSelecttreeelementArgsElementpath(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsSelecttreeelementArgsElementpathSpec

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

class RoboSAPiensKeywordsUntickcheckboxcellResult(TypedDict):
    NoSession: str
    NotFound: str
    NotChangeable: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsUntickcheckboxcellArgs(TypedDict):
    a1row: RoboSAPiensKeywordsUntickcheckboxcellArgsRow
    a2column: RoboSAPiensKeywordsUntickcheckboxcellArgsColumn

class RoboSAPiensKeywordsTickcheckboxcellResult(TypedDict):
    NoSession: str
    NotFound: str
    NotChangeable: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsTickcheckboxcellArgs(TypedDict):
    a1row: RoboSAPiensKeywordsTickcheckboxcellArgsRow
    a2column: RoboSAPiensKeywordsTickcheckboxcellArgsColumn

class RoboSAPiensKeywordsUntickcheckboxResult(TypedDict):
    NoSession: str
    NotFound: str
    NotChangeable: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsUntickcheckboxArgs(TypedDict):
    locator: RoboSAPiensKeywordsUntickcheckboxArgsLocator

class RoboSAPiensKeywordsTickcheckboxResult(TypedDict):
    NoSession: str
    NotFound: str
    NotChangeable: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsTickcheckboxArgs(TypedDict):
    locator: RoboSAPiensKeywordsTickcheckboxArgsLocator

class RoboSAPiensKeywordsSelecttextResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSelecttextArgs(TypedDict):
    locator: RoboSAPiensKeywordsSelecttextArgsLocator

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
    NotChangeable: str
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

class RoboSAPiensKeywordsSelectcellvalueResult(TypedDict):
    NoSession: str
    NotFound: str
    EntryNotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSelectcellvalueArgs(TypedDict):
    a1row_locator: RoboSAPiensKeywordsSelectcellvalueArgsRow_Locator
    a2column: RoboSAPiensKeywordsSelectcellvalueArgsColumn
    a3entry: RoboSAPiensKeywordsSelectcellvalueArgsEntry

class RoboSAPiensKeywordsSelectcellResult(TypedDict):
    NoSession: str
    NotFound: str
    NoTable: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSelectcellArgs(TypedDict):
    a1row_locator: RoboSAPiensKeywordsSelectcellArgsRow_Locator
    a2column: RoboSAPiensKeywordsSelectcellArgsColumn

class RoboSAPiensKeywordsScrolltextfieldcontentsResult(TypedDict):
    NoSession: str
    Pass: str
    NoScrollbar: str
    InvalidDirection: str
    MaximumReached: str
    Exception: str

class RoboSAPiensKeywordsScrolltextfieldcontentsArgs(TypedDict):
    a1direction: RoboSAPiensKeywordsScrolltextfieldcontentsArgsDirection
    a2untilTextField: RoboSAPiensKeywordsScrolltextfieldcontentsArgsUntiltextfield

class RoboSAPiensKeywordsSavescreenshotResult(TypedDict):
    NoSession: str
    UNCPath: str
    NoAbsPath: str
    InvalidPath: str
    Log: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSavescreenshotArgs(TypedDict):
    filepath: RoboSAPiensKeywordsSavescreenshotArgsFilepath

class RoboSAPiensKeywordsReadtablecellResult(TypedDict):
    NoSession: str
    NotFound: str
    NoTable: str
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

class RoboSAPiensKeywordsCounttablerowsResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsCounttablerowsArgs(TypedDict):
    ...

class RoboSAPiensKeywordsSelecttablerowResult(TypedDict):
    NoSession: str
    NoTable: str
    InvalidIndex: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSelecttablerowArgs(TypedDict):
    row_locator: RoboSAPiensKeywordsSelecttablerowArgsRow_Locator

class RoboSAPiensKeywordsPushbuttoncellResult(TypedDict):
    NoSession: str
    NotFound: str
    NotChangeable: str
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
    Json: str

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
    NotFound: str
    NotChangeable: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsPushbuttonArgs(TypedDict):
    button: RoboSAPiensKeywordsPushbuttonArgsButton

class RoboSAPiensKeywordsFilltextfieldResult(TypedDict):
    NoSession: str
    NotFound: str
    NotChangeable: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsFilltextfieldArgs(TypedDict):
    a1locator: RoboSAPiensKeywordsFilltextfieldArgsLocator
    a2content: RoboSAPiensKeywordsFilltextfieldArgsContent

class RoboSAPiensKeywordsFilltablecellResult(TypedDict):
    NoSession: str
    NotFound: str
    NotChangeable: str
    NoTable: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsFilltablecellArgs(TypedDict):
    a1row_locator: RoboSAPiensKeywordsFilltablecellArgsRow_Locator
    a2column: RoboSAPiensKeywordsFilltablecellArgsColumn
    a3content: RoboSAPiensKeywordsFilltablecellArgsContent

class RoboSAPiensKeywordsExportwindowResult(TypedDict):
    NoSession: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsExportwindowArgs(TypedDict):
    a1name: RoboSAPiensKeywordsExportwindowArgsName
    a2directory: RoboSAPiensKeywordsExportwindowArgsDirectory

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
    locator: RoboSAPiensKeywordsDoubleclicktextfieldArgsLocator

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
    NoSession: str
    InvalidSessionId: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsAttachtorunningsapArgs(TypedDict):
    sessionNumber: RoboSAPiensKeywordsAttachtorunningsapArgsSessionnumber

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
    NoGuiScripting: str
    SAPAlreadyRunning: str
    Exception: str

class RoboSAPiensKeywordsOpensapArgs(TypedDict):
    path: RoboSAPiensKeywordsOpensapArgsPath

class RoboSAPiensKeywordsSelectmenuitemResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSelectmenuitemArgs(TypedDict):
    itemPath: RoboSAPiensKeywordsSelectmenuitemArgsItempath

class RoboSAPiensKeywordsSelecttreeelementResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSelecttreeelementArgs(TypedDict):
    elementPath: RoboSAPiensKeywordsSelecttreeelementArgsElementpath

class RoboSAPiensKeywordsActivatetabResult(TypedDict):
    NoSession: str
    NotFound: str
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

class RoboSAPiensKeywordsUntickcheckboxcell(TypedDict):
    name: str
    args: RoboSAPiensKeywordsUntickcheckboxcellArgs
    result: RoboSAPiensKeywordsUntickcheckboxcellResult
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

class RoboSAPiensKeywordsSelecttext(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSelecttextArgs
    result: RoboSAPiensKeywordsSelecttextResult
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

class RoboSAPiensKeywordsSelectcellvalue(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSelectcellvalueArgs
    result: RoboSAPiensKeywordsSelectcellvalueResult
    doc: str

class RoboSAPiensKeywordsSelectcell(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSelectcellArgs
    result: RoboSAPiensKeywordsSelectcellResult
    doc: str

class RoboSAPiensKeywordsScrolltextfieldcontents(TypedDict):
    name: str
    args: RoboSAPiensKeywordsScrolltextfieldcontentsArgs
    result: RoboSAPiensKeywordsScrolltextfieldcontentsResult
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

class RoboSAPiensKeywordsCounttablerows(TypedDict):
    name: str
    args: RoboSAPiensKeywordsCounttablerowsArgs
    result: RoboSAPiensKeywordsCounttablerowsResult
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

class RoboSAPiensKeywordsExportwindow(TypedDict):
    name: str
    args: RoboSAPiensKeywordsExportwindowArgs
    result: RoboSAPiensKeywordsExportwindowResult
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

class RoboSAPiensKeywordsSelectmenuitem(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSelectmenuitemArgs
    result: RoboSAPiensKeywordsSelectmenuitemResult
    doc: str

class RoboSAPiensKeywordsSelecttreeelement(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSelecttreeelementArgs
    result: RoboSAPiensKeywordsSelecttreeelementResult
    doc: str

class RoboSAPiensKeywordsActivatetab(TypedDict):
    name: str
    args: RoboSAPiensKeywordsActivatetabArgs
    result: RoboSAPiensKeywordsActivatetabResult
    doc: str

class RoboSAPiensArgsX64(TypedDict):
    name: str
    default: Literal[False]
    doc: str

class RoboSAPiensArgsPresenter_Mode(TypedDict):
    name: str
    default: Literal[False]
    doc: str

class RoboSAPiensSpecs(TypedDict):
    ...

class RoboSAPiensKeywords(TypedDict):
    ActivateTab: RoboSAPiensKeywordsActivatetab
    SelectTreeElement: RoboSAPiensKeywordsSelecttreeelement
    SelectMenuItem: RoboSAPiensKeywordsSelectmenuitem
    OpenSap: RoboSAPiensKeywordsOpensap
    CloseConnection: RoboSAPiensKeywordsCloseconnection
    CloseSap: RoboSAPiensKeywordsClosesap
    ExportTree: RoboSAPiensKeywordsExporttree
    AttachToRunningSap: RoboSAPiensKeywordsAttachtorunningsap
    ConnectToServer: RoboSAPiensKeywordsConnecttoserver
    DoubleClickCell: RoboSAPiensKeywordsDoubleclickcell
    DoubleClickTextField: RoboSAPiensKeywordsDoubleclicktextfield
    ExecuteTransaction: RoboSAPiensKeywordsExecutetransaction
    ExportWindow: RoboSAPiensKeywordsExportwindow
    FillTableCell: RoboSAPiensKeywordsFilltablecell
    FillTextField: RoboSAPiensKeywordsFilltextfield
    PushButton: RoboSAPiensKeywordsPushbutton
    HighlightButton: RoboSAPiensKeywordsHighlightbutton
    ReadStatusbar: RoboSAPiensKeywordsReadstatusbar
    PushButtonCell: RoboSAPiensKeywordsPushbuttoncell
    SelectTableRow: RoboSAPiensKeywordsSelecttablerow
    CountTableRows: RoboSAPiensKeywordsCounttablerows
    PressKeyCombination: RoboSAPiensKeywordsPresskeycombination
    ReadTextField: RoboSAPiensKeywordsReadtextfield
    ReadText: RoboSAPiensKeywordsReadtext
    ReadTableCell: RoboSAPiensKeywordsReadtablecell
    SaveScreenshot: RoboSAPiensKeywordsSavescreenshot
    ScrollTextFieldContents: RoboSAPiensKeywordsScrolltextfieldcontents
    SelectCell: RoboSAPiensKeywordsSelectcell
    SelectCellValue: RoboSAPiensKeywordsSelectcellvalue
    SelectComboBoxEntry: RoboSAPiensKeywordsSelectcomboboxentry
    SelectRadioButton: RoboSAPiensKeywordsSelectradiobutton
    SelectTextField: RoboSAPiensKeywordsSelecttextfield
    SelectText: RoboSAPiensKeywordsSelecttext
    TickCheckBox: RoboSAPiensKeywordsTickcheckbox
    UntickCheckBox: RoboSAPiensKeywordsUntickcheckbox
    TickCheckBoxCell: RoboSAPiensKeywordsTickcheckboxcell
    UntickCheckBoxCell: RoboSAPiensKeywordsUntickcheckboxcell
    GetWindowTitle: RoboSAPiensKeywordsGetwindowtitle
    GetWindowText: RoboSAPiensKeywordsGetwindowtext

class RoboSAPiensArgs(TypedDict):
    a1presenter_mode: RoboSAPiensArgsPresenter_Mode
    a2x64: RoboSAPiensArgsX64

class RoboSAPiensDoc(TypedDict):
    intro: str
    init: str

class RoboSAPiens(TypedDict):
    doc: RoboSAPiensDoc
    args: RoboSAPiensArgs
    keywords: RoboSAPiensKeywords
    specs: RoboSAPiensSpecs