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

class RoboSAPiensKeywordsReadcomboboxentryArgsComboboxSpec(TypedDict):
    ...

class RoboSAPiensKeywordsSelecttreeelementmenuentryArgsMenuentrySpec(TypedDict):
    ...

class RoboSAPiensKeywordsSelecttreeelementmenuentryArgsElementpathSpec(TypedDict):
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

class RoboSAPiensKeywordsScrollwindowhorizontallyArgsDirectionSpec(TypedDict):
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

class RoboSAPiensKeywordsFilltexteditArgsContentSpec(TypedDict):
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

class RoboSAPiensKeywordsOpensapArgsSapargsSpec(TypedDict):
    ...

class RoboSAPiensKeywordsOpensapArgsPathSpec(TypedDict):
    ...

class RoboSAPiensKeywordsSelectmenuitemArgsItempathSpec(TypedDict):
    ...

class RoboSAPiensKeywordsDoubleclicktreeelementArgsElementpathSpec(TypedDict):
    ...

class RoboSAPiensKeywordsSelecttreeelementArgsElementpathSpec(TypedDict):
    ...

class RoboSAPiensKeywordsActivatetabArgsTabSpec(TypedDict):
    ...

class RoboSAPiensKeywordsUntickcheckboxcellArgsColumn(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsUntickcheckboxcellArgsColumnSpec

class RoboSAPiensKeywordsUntickcheckboxcellArgsRow(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsUntickcheckboxcellArgsRowSpec

class RoboSAPiensKeywordsTickcheckboxcellArgsColumn(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsTickcheckboxcellArgsColumnSpec

class RoboSAPiensKeywordsTickcheckboxcellArgsRow(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsTickcheckboxcellArgsRowSpec

class RoboSAPiensKeywordsUntickcheckboxArgsLocator(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsUntickcheckboxArgsLocatorSpec

class RoboSAPiensKeywordsTickcheckboxArgsLocator(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsTickcheckboxArgsLocatorSpec

class RoboSAPiensKeywordsSelecttextArgsLocator(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsSelecttextArgsLocatorSpec

class RoboSAPiensKeywordsSelecttextfieldArgsLocator(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsSelecttextfieldArgsLocatorSpec

class RoboSAPiensKeywordsSelectradiobuttonArgsLocator(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsSelectradiobuttonArgsLocatorSpec

class RoboSAPiensKeywordsSelectcomboboxentryArgsEntry(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsSelectcomboboxentryArgsEntrySpec

class RoboSAPiensKeywordsSelectcomboboxentryArgsCombobox(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsSelectcomboboxentryArgsComboboxSpec

class RoboSAPiensKeywordsReadcomboboxentryArgsCombobox(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsReadcomboboxentryArgsComboboxSpec

class RoboSAPiensKeywordsSelecttreeelementmenuentryArgsMenuentry(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsSelecttreeelementmenuentryArgsMenuentrySpec

class RoboSAPiensKeywordsSelecttreeelementmenuentryArgsElementpath(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsSelecttreeelementmenuentryArgsElementpathSpec

class RoboSAPiensKeywordsSelectcellvalueArgsEntry(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsSelectcellvalueArgsEntrySpec

class RoboSAPiensKeywordsSelectcellvalueArgsColumn(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsSelectcellvalueArgsColumnSpec

class RoboSAPiensKeywordsSelectcellvalueArgsRow_Locator(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsSelectcellvalueArgsRow_LocatorSpec

class RoboSAPiensKeywordsSelectcellArgsColumn(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsSelectcellArgsColumnSpec

class RoboSAPiensKeywordsSelectcellArgsRow_Locator(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsSelectcellArgsRow_LocatorSpec

class RoboSAPiensKeywordsScrollwindowhorizontallyArgsDirection(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsScrollwindowhorizontallyArgsDirectionSpec

class RoboSAPiensKeywordsScrolltextfieldcontentsArgsUntiltextfield(TypedDict):
    name: str
    desc: str
    default: Literal[None]
    spec: RoboSAPiensKeywordsScrolltextfieldcontentsArgsUntiltextfieldSpec

class RoboSAPiensKeywordsScrolltextfieldcontentsArgsDirection(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsScrolltextfieldcontentsArgsDirectionSpec

class RoboSAPiensKeywordsSavescreenshotArgsFilepath(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsSavescreenshotArgsFilepathSpec

class RoboSAPiensKeywordsReadtablecellArgsColumn(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsReadtablecellArgsColumnSpec

class RoboSAPiensKeywordsReadtablecellArgsRow_Locator(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsReadtablecellArgsRow_LocatorSpec

class RoboSAPiensKeywordsReadtextArgsLocator(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsReadtextArgsLocatorSpec

class RoboSAPiensKeywordsReadtextfieldArgsLocator(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsReadtextfieldArgsLocatorSpec

class RoboSAPiensKeywordsPresskeycombinationArgsKeycombination(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsPresskeycombinationArgsKeycombinationSpec

class RoboSAPiensKeywordsSelecttablerowArgsRow_Locator(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsSelecttablerowArgsRow_LocatorSpec

class RoboSAPiensKeywordsPushbuttoncellArgsColumn(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsPushbuttoncellArgsColumnSpec

class RoboSAPiensKeywordsPushbuttoncellArgsRow_Or_Label(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsPushbuttoncellArgsRow_Or_LabelSpec

class RoboSAPiensKeywordsHighlightbuttonArgsButton(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsHighlightbuttonArgsButtonSpec

class RoboSAPiensKeywordsPushbuttonArgsButton(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsPushbuttonArgsButtonSpec

class RoboSAPiensKeywordsFilltexteditArgsContent(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsFilltexteditArgsContentSpec

class RoboSAPiensKeywordsFilltextfieldArgsContent(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsFilltextfieldArgsContentSpec

class RoboSAPiensKeywordsFilltextfieldArgsLocator(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsFilltextfieldArgsLocatorSpec

class RoboSAPiensKeywordsFilltablecellArgsContent(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsFilltablecellArgsContentSpec

class RoboSAPiensKeywordsFilltablecellArgsColumn(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsFilltablecellArgsColumnSpec

class RoboSAPiensKeywordsFilltablecellArgsRow_Locator(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsFilltablecellArgsRow_LocatorSpec

class RoboSAPiensKeywordsExportwindowArgsDirectory(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsExportwindowArgsDirectorySpec

class RoboSAPiensKeywordsExportwindowArgsName(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsExportwindowArgsNameSpec

class RoboSAPiensKeywordsExecutetransactionArgsT_Code(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsExecutetransactionArgsT_CodeSpec

class RoboSAPiensKeywordsDoubleclicktextfieldArgsLocator(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsDoubleclicktextfieldArgsLocatorSpec

class RoboSAPiensKeywordsDoubleclickcellArgsColumn(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsDoubleclickcellArgsColumnSpec

class RoboSAPiensKeywordsDoubleclickcellArgsRow_Locator(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsDoubleclickcellArgsRow_LocatorSpec

class RoboSAPiensKeywordsConnecttoserverArgsServer(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsConnecttoserverArgsServerSpec

class RoboSAPiensKeywordsAttachtorunningsapArgsSessionnumber(TypedDict):
    name: str
    desc: str
    default: str
    spec: RoboSAPiensKeywordsAttachtorunningsapArgsSessionnumberSpec

class RoboSAPiensKeywordsExporttreeArgsFilepath(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsExporttreeArgsFilepathSpec

class RoboSAPiensKeywordsOpensapArgsSapargs(TypedDict):
    name: str
    desc: str
    default: Literal[None]
    spec: RoboSAPiensKeywordsOpensapArgsSapargsSpec

class RoboSAPiensKeywordsOpensapArgsPath(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsOpensapArgsPathSpec

class RoboSAPiensKeywordsSelectmenuitemArgsItempath(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsSelectmenuitemArgsItempathSpec

class RoboSAPiensKeywordsDoubleclicktreeelementArgsElementpath(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsDoubleclicktreeelementArgsElementpathSpec

class RoboSAPiensKeywordsSelecttreeelementArgsElementpath(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsSelecttreeelementArgsElementpathSpec

class RoboSAPiensKeywordsActivatetabArgsTab(TypedDict):
    name: str
    desc: str
    spec: RoboSAPiensKeywordsActivatetabArgsTabSpec

class RoboSAPiensKeywordsGetwindowtextDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsGetwindowtextResult(TypedDict):
    NoSession: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsGetwindowtextArgs(TypedDict):
    ...

class RoboSAPiensKeywordsGetwindowtitleDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsGetwindowtitleResult(TypedDict):
    NoSession: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsGetwindowtitleArgs(TypedDict):
    ...

class RoboSAPiensKeywordsUntickcheckboxcellDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsUntickcheckboxcellResult(TypedDict):
    NoSession: str
    NotFound: str
    NotChangeable: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsUntickcheckboxcellArgs(TypedDict):
    a1row: RoboSAPiensKeywordsUntickcheckboxcellArgsRow
    a2column: RoboSAPiensKeywordsUntickcheckboxcellArgsColumn

class RoboSAPiensKeywordsTickcheckboxcellDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsTickcheckboxcellResult(TypedDict):
    NoSession: str
    NotFound: str
    NotChangeable: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsTickcheckboxcellArgs(TypedDict):
    a1row: RoboSAPiensKeywordsTickcheckboxcellArgsRow
    a2column: RoboSAPiensKeywordsTickcheckboxcellArgsColumn

class RoboSAPiensKeywordsUntickcheckboxDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsUntickcheckboxResult(TypedDict):
    NoSession: str
    NotFound: str
    NotChangeable: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsUntickcheckboxArgs(TypedDict):
    locator: RoboSAPiensKeywordsUntickcheckboxArgsLocator

class RoboSAPiensKeywordsTickcheckboxDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsTickcheckboxResult(TypedDict):
    NoSession: str
    NotFound: str
    NotChangeable: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsTickcheckboxArgs(TypedDict):
    locator: RoboSAPiensKeywordsTickcheckboxArgsLocator

class RoboSAPiensKeywordsSelecttextDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsSelecttextResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSelecttextArgs(TypedDict):
    locator: RoboSAPiensKeywordsSelecttextArgsLocator

class RoboSAPiensKeywordsSelecttextfieldDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsSelecttextfieldResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSelecttextfieldArgs(TypedDict):
    locator: RoboSAPiensKeywordsSelecttextfieldArgsLocator

class RoboSAPiensKeywordsSelectradiobuttonDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsSelectradiobuttonResult(TypedDict):
    NoSession: str
    NotFound: str
    NotChangeable: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSelectradiobuttonArgs(TypedDict):
    locator: RoboSAPiensKeywordsSelectradiobuttonArgsLocator

class RoboSAPiensKeywordsSelectcomboboxentryDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsSelectcomboboxentryResult(TypedDict):
    NoSession: str
    NotFound: str
    EntryNotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSelectcomboboxentryArgs(TypedDict):
    a1comboBox: RoboSAPiensKeywordsSelectcomboboxentryArgsCombobox
    a2entry: RoboSAPiensKeywordsSelectcomboboxentryArgsEntry

class RoboSAPiensKeywordsReadcomboboxentryDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsReadcomboboxentryResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsReadcomboboxentryArgs(TypedDict):
    comboBox: RoboSAPiensKeywordsReadcomboboxentryArgsCombobox

class RoboSAPiensKeywordsSelecttreeelementmenuentryDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsSelecttreeelementmenuentryResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSelecttreeelementmenuentryArgs(TypedDict):
    a1elementPath: RoboSAPiensKeywordsSelecttreeelementmenuentryArgsElementpath
    a2menuEntry: RoboSAPiensKeywordsSelecttreeelementmenuentryArgsMenuentry

class RoboSAPiensKeywordsSelectcellvalueDoc(TypedDict):
    desc: str
    examples: str

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

class RoboSAPiensKeywordsSelectcellDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsSelectcellResult(TypedDict):
    NoSession: str
    NotFound: str
    NoTable: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSelectcellArgs(TypedDict):
    a1row_locator: RoboSAPiensKeywordsSelectcellArgsRow_Locator
    a2column: RoboSAPiensKeywordsSelectcellArgsColumn

class RoboSAPiensKeywordsScrollwindowhorizontallyDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsScrollwindowhorizontallyResult(TypedDict):
    NoSession: str
    Pass: str
    NoScrollbar: str
    InvalidDirection: str
    MaximumReached: str
    Exception: str

class RoboSAPiensKeywordsScrollwindowhorizontallyArgs(TypedDict):
    direction: RoboSAPiensKeywordsScrollwindowhorizontallyArgsDirection

class RoboSAPiensKeywordsScrolltextfieldcontentsDoc(TypedDict):
    desc: str
    examples: str

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

class RoboSAPiensKeywordsSavescreenshotDoc(TypedDict):
    desc: str
    examples: str

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

class RoboSAPiensKeywordsReadtablecellDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsReadtablecellResult(TypedDict):
    NoSession: str
    NotFound: str
    NoTable: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsReadtablecellArgs(TypedDict):
    a1row_locator: RoboSAPiensKeywordsReadtablecellArgsRow_Locator
    a2column: RoboSAPiensKeywordsReadtablecellArgsColumn

class RoboSAPiensKeywordsReadtextDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsReadtextResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsReadtextArgs(TypedDict):
    locator: RoboSAPiensKeywordsReadtextArgsLocator

class RoboSAPiensKeywordsReadtextfieldDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsReadtextfieldResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsReadtextfieldArgs(TypedDict):
    locator: RoboSAPiensKeywordsReadtextfieldArgsLocator

class RoboSAPiensKeywordsPresskeycombinationDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsPresskeycombinationResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsPresskeycombinationArgs(TypedDict):
    keyCombination: RoboSAPiensKeywordsPresskeycombinationArgsKeycombination

class RoboSAPiensKeywordsCounttablerowsDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsCounttablerowsResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsCounttablerowsArgs(TypedDict):
    ...

class RoboSAPiensKeywordsSelecttablerowDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsSelecttablerowResult(TypedDict):
    NoSession: str
    NoTable: str
    InvalidIndex: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSelecttablerowArgs(TypedDict):
    row_locator: RoboSAPiensKeywordsSelecttablerowArgsRow_Locator

class RoboSAPiensKeywordsPushbuttoncellDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsPushbuttoncellResult(TypedDict):
    NoSession: str
    NotFound: str
    NotChangeable: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsPushbuttoncellArgs(TypedDict):
    a1row_or_label: RoboSAPiensKeywordsPushbuttoncellArgsRow_Or_Label
    a2column: RoboSAPiensKeywordsPushbuttoncellArgsColumn

class RoboSAPiensKeywordsReadstatusbarDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsReadstatusbarResult(TypedDict):
    NoSession: str
    Pass: str
    Exception: str
    NotFound: str
    Json: str

class RoboSAPiensKeywordsReadstatusbarArgs(TypedDict):
    ...

class RoboSAPiensKeywordsHighlightbuttonDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsHighlightbuttonResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsHighlightbuttonArgs(TypedDict):
    button: RoboSAPiensKeywordsHighlightbuttonArgsButton

class RoboSAPiensKeywordsPushbuttonDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsPushbuttonResult(TypedDict):
    NoSession: str
    NotFound: str
    NotChangeable: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsPushbuttonArgs(TypedDict):
    button: RoboSAPiensKeywordsPushbuttonArgsButton

class RoboSAPiensKeywordsFilltexteditDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsFilltexteditResult(TypedDict):
    NoSession: str
    NotFound: str
    NotChangeable: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsFilltexteditArgs(TypedDict):
    content: RoboSAPiensKeywordsFilltexteditArgsContent

class RoboSAPiensKeywordsFilltextfieldDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsFilltextfieldResult(TypedDict):
    NoSession: str
    NotFound: str
    NotChangeable: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsFilltextfieldArgs(TypedDict):
    a1locator: RoboSAPiensKeywordsFilltextfieldArgsLocator
    a2content: RoboSAPiensKeywordsFilltextfieldArgsContent

class RoboSAPiensKeywordsFilltablecellDoc(TypedDict):
    desc: str
    examples: str

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

class RoboSAPiensKeywordsExportwindowDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsExportwindowResult(TypedDict):
    NoSession: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsExportwindowArgs(TypedDict):
    a1name: RoboSAPiensKeywordsExportwindowArgsName
    a2directory: RoboSAPiensKeywordsExportwindowArgsDirectory

class RoboSAPiensKeywordsExecutetransactionDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsExecutetransactionResult(TypedDict):
    NoSession: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsExecutetransactionArgs(TypedDict):
    T_Code: RoboSAPiensKeywordsExecutetransactionArgsT_Code

class RoboSAPiensKeywordsDoubleclicktextfieldDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsDoubleclicktextfieldResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsDoubleclicktextfieldArgs(TypedDict):
    locator: RoboSAPiensKeywordsDoubleclicktextfieldArgsLocator

class RoboSAPiensKeywordsDoubleclickcellDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsDoubleclickcellResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsDoubleclickcellArgs(TypedDict):
    a1row_locator: RoboSAPiensKeywordsDoubleclickcellArgsRow_Locator
    a2column: RoboSAPiensKeywordsDoubleclickcellArgsColumn

class RoboSAPiensKeywordsConnecttoserverDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsConnecttoserverResult(TypedDict):
    NoSapGui: str
    NoGuiScripting: str
    Pass: str
    SapError: str
    NoServerScripting: str
    Exception: str

class RoboSAPiensKeywordsConnecttoserverArgs(TypedDict):
    server: RoboSAPiensKeywordsConnecttoserverArgsServer

class RoboSAPiensKeywordsAttachtorunningsapDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsAttachtorunningsapResult(TypedDict):
    NoSapGui: str
    NoGuiScripting: str
    NoConnection: str
    NoServerScripting: str
    NoSession: str
    InvalidSessionId: str
    Json: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsAttachtorunningsapArgs(TypedDict):
    sessionNumber: RoboSAPiensKeywordsAttachtorunningsapArgsSessionnumber

class RoboSAPiensKeywordsExporttreeDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsExporttreeResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsExporttreeArgs(TypedDict):
    filepath: RoboSAPiensKeywordsExporttreeArgsFilepath

class RoboSAPiensKeywordsClosesapDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsClosesapResult(TypedDict):
    NoSapGui: str
    Pass: str

class RoboSAPiensKeywordsClosesapArgs(TypedDict):
    ...

class RoboSAPiensKeywordsCloseconnectionDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsCloseconnectionResult(TypedDict):
    NoSapGui: str
    NoGuiScripting: str
    NoConnection: str
    NoSession: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsCloseconnectionArgs(TypedDict):
    ...

class RoboSAPiensKeywordsOpensapDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsOpensapResult(TypedDict):
    Pass: str
    SAPNotStarted: str
    NoGuiScripting: str
    SAPAlreadyRunning: str
    Exception: str

class RoboSAPiensKeywordsOpensapArgs(TypedDict):
    a1path: RoboSAPiensKeywordsOpensapArgsPath
    a2sapArgs: RoboSAPiensKeywordsOpensapArgsSapargs

class RoboSAPiensKeywordsSelectmenuitemDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsSelectmenuitemResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSelectmenuitemArgs(TypedDict):
    itemPath: RoboSAPiensKeywordsSelectmenuitemArgsItempath

class RoboSAPiensKeywordsDoubleclicktreeelementDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsDoubleclicktreeelementResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsDoubleclicktreeelementArgs(TypedDict):
    elementPath: RoboSAPiensKeywordsDoubleclicktreeelementArgsElementpath

class RoboSAPiensKeywordsSelecttreeelementDoc(TypedDict):
    desc: str
    examples: str

class RoboSAPiensKeywordsSelecttreeelementResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSelecttreeelementArgs(TypedDict):
    elementPath: RoboSAPiensKeywordsSelecttreeelementArgsElementpath

class RoboSAPiensKeywordsActivatetabDoc(TypedDict):
    desc: str
    examples: str

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
    doc: RoboSAPiensKeywordsGetwindowtextDoc

class RoboSAPiensKeywordsGetwindowtitle(TypedDict):
    name: str
    args: RoboSAPiensKeywordsGetwindowtitleArgs
    result: RoboSAPiensKeywordsGetwindowtitleResult
    doc: RoboSAPiensKeywordsGetwindowtitleDoc

class RoboSAPiensKeywordsUntickcheckboxcell(TypedDict):
    name: str
    args: RoboSAPiensKeywordsUntickcheckboxcellArgs
    result: RoboSAPiensKeywordsUntickcheckboxcellResult
    doc: RoboSAPiensKeywordsUntickcheckboxcellDoc

class RoboSAPiensKeywordsTickcheckboxcell(TypedDict):
    name: str
    args: RoboSAPiensKeywordsTickcheckboxcellArgs
    result: RoboSAPiensKeywordsTickcheckboxcellResult
    doc: RoboSAPiensKeywordsTickcheckboxcellDoc

class RoboSAPiensKeywordsUntickcheckbox(TypedDict):
    name: str
    args: RoboSAPiensKeywordsUntickcheckboxArgs
    result: RoboSAPiensKeywordsUntickcheckboxResult
    doc: RoboSAPiensKeywordsUntickcheckboxDoc

class RoboSAPiensKeywordsTickcheckbox(TypedDict):
    name: str
    args: RoboSAPiensKeywordsTickcheckboxArgs
    result: RoboSAPiensKeywordsTickcheckboxResult
    doc: RoboSAPiensKeywordsTickcheckboxDoc

class RoboSAPiensKeywordsSelecttext(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSelecttextArgs
    result: RoboSAPiensKeywordsSelecttextResult
    doc: RoboSAPiensKeywordsSelecttextDoc

class RoboSAPiensKeywordsSelecttextfield(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSelecttextfieldArgs
    result: RoboSAPiensKeywordsSelecttextfieldResult
    doc: RoboSAPiensKeywordsSelecttextfieldDoc

class RoboSAPiensKeywordsSelectradiobutton(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSelectradiobuttonArgs
    result: RoboSAPiensKeywordsSelectradiobuttonResult
    doc: RoboSAPiensKeywordsSelectradiobuttonDoc

class RoboSAPiensKeywordsSelectcomboboxentry(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSelectcomboboxentryArgs
    result: RoboSAPiensKeywordsSelectcomboboxentryResult
    doc: RoboSAPiensKeywordsSelectcomboboxentryDoc

class RoboSAPiensKeywordsReadcomboboxentry(TypedDict):
    name: str
    args: RoboSAPiensKeywordsReadcomboboxentryArgs
    result: RoboSAPiensKeywordsReadcomboboxentryResult
    doc: RoboSAPiensKeywordsReadcomboboxentryDoc

class RoboSAPiensKeywordsSelecttreeelementmenuentry(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSelecttreeelementmenuentryArgs
    result: RoboSAPiensKeywordsSelecttreeelementmenuentryResult
    doc: RoboSAPiensKeywordsSelecttreeelementmenuentryDoc

class RoboSAPiensKeywordsSelectcellvalue(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSelectcellvalueArgs
    result: RoboSAPiensKeywordsSelectcellvalueResult
    doc: RoboSAPiensKeywordsSelectcellvalueDoc

class RoboSAPiensKeywordsSelectcell(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSelectcellArgs
    result: RoboSAPiensKeywordsSelectcellResult
    doc: RoboSAPiensKeywordsSelectcellDoc

class RoboSAPiensKeywordsScrollwindowhorizontally(TypedDict):
    name: str
    args: RoboSAPiensKeywordsScrollwindowhorizontallyArgs
    result: RoboSAPiensKeywordsScrollwindowhorizontallyResult
    doc: RoboSAPiensKeywordsScrollwindowhorizontallyDoc

class RoboSAPiensKeywordsScrolltextfieldcontents(TypedDict):
    name: str
    args: RoboSAPiensKeywordsScrolltextfieldcontentsArgs
    result: RoboSAPiensKeywordsScrolltextfieldcontentsResult
    doc: RoboSAPiensKeywordsScrolltextfieldcontentsDoc

class RoboSAPiensKeywordsSavescreenshot(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSavescreenshotArgs
    result: RoboSAPiensKeywordsSavescreenshotResult
    doc: RoboSAPiensKeywordsSavescreenshotDoc

class RoboSAPiensKeywordsReadtablecell(TypedDict):
    name: str
    args: RoboSAPiensKeywordsReadtablecellArgs
    result: RoboSAPiensKeywordsReadtablecellResult
    doc: RoboSAPiensKeywordsReadtablecellDoc

class RoboSAPiensKeywordsReadtext(TypedDict):
    name: str
    args: RoboSAPiensKeywordsReadtextArgs
    result: RoboSAPiensKeywordsReadtextResult
    doc: RoboSAPiensKeywordsReadtextDoc

class RoboSAPiensKeywordsReadtextfield(TypedDict):
    name: str
    args: RoboSAPiensKeywordsReadtextfieldArgs
    result: RoboSAPiensKeywordsReadtextfieldResult
    doc: RoboSAPiensKeywordsReadtextfieldDoc

class RoboSAPiensKeywordsPresskeycombination(TypedDict):
    name: str
    args: RoboSAPiensKeywordsPresskeycombinationArgs
    result: RoboSAPiensKeywordsPresskeycombinationResult
    doc: RoboSAPiensKeywordsPresskeycombinationDoc

class RoboSAPiensKeywordsCounttablerows(TypedDict):
    name: str
    args: RoboSAPiensKeywordsCounttablerowsArgs
    result: RoboSAPiensKeywordsCounttablerowsResult
    doc: RoboSAPiensKeywordsCounttablerowsDoc

class RoboSAPiensKeywordsSelecttablerow(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSelecttablerowArgs
    result: RoboSAPiensKeywordsSelecttablerowResult
    doc: RoboSAPiensKeywordsSelecttablerowDoc

class RoboSAPiensKeywordsPushbuttoncell(TypedDict):
    name: str
    args: RoboSAPiensKeywordsPushbuttoncellArgs
    result: RoboSAPiensKeywordsPushbuttoncellResult
    doc: RoboSAPiensKeywordsPushbuttoncellDoc

class RoboSAPiensKeywordsReadstatusbar(TypedDict):
    name: str
    args: RoboSAPiensKeywordsReadstatusbarArgs
    result: RoboSAPiensKeywordsReadstatusbarResult
    doc: RoboSAPiensKeywordsReadstatusbarDoc

class RoboSAPiensKeywordsHighlightbutton(TypedDict):
    name: str
    args: RoboSAPiensKeywordsHighlightbuttonArgs
    result: RoboSAPiensKeywordsHighlightbuttonResult
    doc: RoboSAPiensKeywordsHighlightbuttonDoc

class RoboSAPiensKeywordsPushbutton(TypedDict):
    name: str
    args: RoboSAPiensKeywordsPushbuttonArgs
    result: RoboSAPiensKeywordsPushbuttonResult
    doc: RoboSAPiensKeywordsPushbuttonDoc

class RoboSAPiensKeywordsFilltextedit(TypedDict):
    name: str
    args: RoboSAPiensKeywordsFilltexteditArgs
    result: RoboSAPiensKeywordsFilltexteditResult
    doc: RoboSAPiensKeywordsFilltexteditDoc

class RoboSAPiensKeywordsFilltextfield(TypedDict):
    name: str
    args: RoboSAPiensKeywordsFilltextfieldArgs
    result: RoboSAPiensKeywordsFilltextfieldResult
    doc: RoboSAPiensKeywordsFilltextfieldDoc

class RoboSAPiensKeywordsFilltablecell(TypedDict):
    name: str
    args: RoboSAPiensKeywordsFilltablecellArgs
    result: RoboSAPiensKeywordsFilltablecellResult
    doc: RoboSAPiensKeywordsFilltablecellDoc

class RoboSAPiensKeywordsExportwindow(TypedDict):
    name: str
    args: RoboSAPiensKeywordsExportwindowArgs
    result: RoboSAPiensKeywordsExportwindowResult
    doc: RoboSAPiensKeywordsExportwindowDoc

class RoboSAPiensKeywordsExecutetransaction(TypedDict):
    name: str
    args: RoboSAPiensKeywordsExecutetransactionArgs
    result: RoboSAPiensKeywordsExecutetransactionResult
    doc: RoboSAPiensKeywordsExecutetransactionDoc

class RoboSAPiensKeywordsDoubleclicktextfield(TypedDict):
    name: str
    args: RoboSAPiensKeywordsDoubleclicktextfieldArgs
    result: RoboSAPiensKeywordsDoubleclicktextfieldResult
    doc: RoboSAPiensKeywordsDoubleclicktextfieldDoc

class RoboSAPiensKeywordsDoubleclickcell(TypedDict):
    name: str
    args: RoboSAPiensKeywordsDoubleclickcellArgs
    result: RoboSAPiensKeywordsDoubleclickcellResult
    doc: RoboSAPiensKeywordsDoubleclickcellDoc

class RoboSAPiensKeywordsConnecttoserver(TypedDict):
    name: str
    args: RoboSAPiensKeywordsConnecttoserverArgs
    result: RoboSAPiensKeywordsConnecttoserverResult
    doc: RoboSAPiensKeywordsConnecttoserverDoc

class RoboSAPiensKeywordsAttachtorunningsap(TypedDict):
    name: str
    args: RoboSAPiensKeywordsAttachtorunningsapArgs
    result: RoboSAPiensKeywordsAttachtorunningsapResult
    doc: RoboSAPiensKeywordsAttachtorunningsapDoc

class RoboSAPiensKeywordsExporttree(TypedDict):
    name: str
    args: RoboSAPiensKeywordsExporttreeArgs
    result: RoboSAPiensKeywordsExporttreeResult
    doc: RoboSAPiensKeywordsExporttreeDoc

class RoboSAPiensKeywordsClosesap(TypedDict):
    name: str
    args: RoboSAPiensKeywordsClosesapArgs
    result: RoboSAPiensKeywordsClosesapResult
    doc: RoboSAPiensKeywordsClosesapDoc

class RoboSAPiensKeywordsCloseconnection(TypedDict):
    name: str
    args: RoboSAPiensKeywordsCloseconnectionArgs
    result: RoboSAPiensKeywordsCloseconnectionResult
    doc: RoboSAPiensKeywordsCloseconnectionDoc

class RoboSAPiensKeywordsOpensap(TypedDict):
    name: str
    args: RoboSAPiensKeywordsOpensapArgs
    result: RoboSAPiensKeywordsOpensapResult
    doc: RoboSAPiensKeywordsOpensapDoc

class RoboSAPiensKeywordsSelectmenuitem(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSelectmenuitemArgs
    result: RoboSAPiensKeywordsSelectmenuitemResult
    doc: RoboSAPiensKeywordsSelectmenuitemDoc

class RoboSAPiensKeywordsDoubleclicktreeelement(TypedDict):
    name: str
    args: RoboSAPiensKeywordsDoubleclicktreeelementArgs
    result: RoboSAPiensKeywordsDoubleclicktreeelementResult
    doc: RoboSAPiensKeywordsDoubleclicktreeelementDoc

class RoboSAPiensKeywordsSelecttreeelement(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSelecttreeelementArgs
    result: RoboSAPiensKeywordsSelecttreeelementResult
    doc: RoboSAPiensKeywordsSelecttreeelementDoc

class RoboSAPiensKeywordsActivatetab(TypedDict):
    name: str
    args: RoboSAPiensKeywordsActivatetabArgs
    result: RoboSAPiensKeywordsActivatetabResult
    doc: RoboSAPiensKeywordsActivatetabDoc

class RoboSAPiensArgsX64(TypedDict):
    name: str
    default: Literal[False]
    desc: str

class RoboSAPiensArgsPresenter_Mode(TypedDict):
    name: str
    default: Literal[False]
    desc: str

class RoboSAPiensSpecs(TypedDict):
    ...

class RoboSAPiensKeywords(TypedDict):
    ActivateTab: RoboSAPiensKeywordsActivatetab
    SelectTreeElement: RoboSAPiensKeywordsSelecttreeelement
    DoubleClickTreeElement: RoboSAPiensKeywordsDoubleclicktreeelement
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
    FillTextEdit: RoboSAPiensKeywordsFilltextedit
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
    ScrollWindowHorizontally: RoboSAPiensKeywordsScrollwindowhorizontally
    SelectCell: RoboSAPiensKeywordsSelectcell
    SelectCellValue: RoboSAPiensKeywordsSelectcellvalue
    SelectTreeElementMenuEntry: RoboSAPiensKeywordsSelecttreeelementmenuentry
    ReadComboBoxEntry: RoboSAPiensKeywordsReadcomboboxentry
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