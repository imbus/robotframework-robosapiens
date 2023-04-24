from typing import Literal, TypedDict

class RoboSAPiensDoc(TypedDict):
    intro: str
    init: str

class RoboSAPiensArgsPort(TypedDict):
    name: str
    default: Literal[8270]
    doc: str

class RoboSAPiensArgsPresenter_Mode(TypedDict):
    name: str
    default: Literal[False]
    doc: str

class RoboSAPiensArgs(TypedDict):
    a1port: RoboSAPiensArgsPort
    a2presenter_mode: RoboSAPiensArgsPresenter_Mode

class RoboSAPiensKeywordsActivatetabArgsReiternameSpec(TypedDict):
    ...

class RoboSAPiensKeywordsActivatetabArgsReitername(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsActivatetabArgsReiternameSpec

class RoboSAPiensKeywordsActivatetabArgs(TypedDict):
    Reitername: RoboSAPiensKeywordsActivatetabArgsReitername

class RoboSAPiensKeywordsActivatetabResult(TypedDict):
    NoSession: str
    NotFound: str
    SapError: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsActivatetab(TypedDict):
    name: str
    args: RoboSAPiensKeywordsActivatetabArgs
    result: RoboSAPiensKeywordsActivatetabResult
    doc: str

class RoboSAPiensKeywordsOpensapArgsPfadSpec(TypedDict):
    ...

class RoboSAPiensKeywordsOpensapArgsPfad(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsOpensapArgsPfadSpec

class RoboSAPiensKeywordsOpensapArgs(TypedDict):
    Pfad: RoboSAPiensKeywordsOpensapArgsPfad

class RoboSAPiensKeywordsOpensapResult(TypedDict):
    Pass: str
    SAPNotStarted: str
    Exception: str

class RoboSAPiensKeywordsOpensap(TypedDict):
    name: str
    args: RoboSAPiensKeywordsOpensapArgs
    result: RoboSAPiensKeywordsOpensapResult
    doc: str

class RoboSAPiensKeywordsCloseconnectionArgs(TypedDict):
    ...

class RoboSAPiensKeywordsCloseconnectionResult(TypedDict):
    NoSapGui: str
    NoGuiScripting: str
    NoConnection: str
    NoSession: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsCloseconnection(TypedDict):
    name: str
    args: RoboSAPiensKeywordsCloseconnectionArgs
    result: RoboSAPiensKeywordsCloseconnectionResult
    doc: str

class RoboSAPiensKeywordsClosesapArgs(TypedDict):
    ...

class RoboSAPiensKeywordsClosesapResult(TypedDict):
    NoSapGui: str
    Pass: str

class RoboSAPiensKeywordsClosesap(TypedDict):
    name: str
    args: RoboSAPiensKeywordsClosesapArgs
    result: RoboSAPiensKeywordsClosesapResult
    doc: str

class RoboSAPiensKeywordsExporttreeArgsDateipfadSpec(TypedDict):
    ...

class RoboSAPiensKeywordsExporttreeArgsDateipfad(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsExporttreeArgsDateipfadSpec

class RoboSAPiensKeywordsExporttreeArgs(TypedDict):
    Dateipfad: RoboSAPiensKeywordsExporttreeArgsDateipfad

class RoboSAPiensKeywordsExporttreeResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsExporttree(TypedDict):
    name: str
    args: RoboSAPiensKeywordsExporttreeArgs
    result: RoboSAPiensKeywordsExporttreeResult
    doc: str

class RoboSAPiensKeywordsAttachtorunningsapArgs(TypedDict):
    ...

class RoboSAPiensKeywordsAttachtorunningsapResult(TypedDict):
    NoSapGui: str
    NoGuiScripting: str
    NoConnection: str
    NoServerScripting: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsAttachtorunningsap(TypedDict):
    name: str
    args: RoboSAPiensKeywordsAttachtorunningsapArgs
    result: RoboSAPiensKeywordsAttachtorunningsapResult
    doc: str

class RoboSAPiensKeywordsConnecttoserverArgsServernameSpec(TypedDict):
    ...

class RoboSAPiensKeywordsConnecttoserverArgsServername(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsConnecttoserverArgsServernameSpec

class RoboSAPiensKeywordsConnecttoserverArgs(TypedDict):
    Servername: RoboSAPiensKeywordsConnecttoserverArgsServername

class RoboSAPiensKeywordsConnecttoserverResult(TypedDict):
    NoSapGui: str
    NoGuiScripting: str
    Pass: str
    SapError: str
    NoServerScripting: str
    Exception: str

class RoboSAPiensKeywordsConnecttoserver(TypedDict):
    name: str
    args: RoboSAPiensKeywordsConnecttoserverArgs
    result: RoboSAPiensKeywordsConnecttoserverResult
    doc: str

class RoboSAPiensKeywordsDoubleclickcellArgsZeilennummer_Oder_ZellinhaltSpec(TypedDict):
    ...

class RoboSAPiensKeywordsDoubleclickcellArgsZeilennummer_Oder_Zellinhalt(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsDoubleclickcellArgsZeilennummer_Oder_ZellinhaltSpec

class RoboSAPiensKeywordsDoubleclickcellArgsSpaltentitelSpec(TypedDict):
    ...

class RoboSAPiensKeywordsDoubleclickcellArgsSpaltentitel(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsDoubleclickcellArgsSpaltentitelSpec

class RoboSAPiensKeywordsDoubleclickcellArgs(TypedDict):
    a1Zeilennummer_oder_Zellinhalt: RoboSAPiensKeywordsDoubleclickcellArgsZeilennummer_Oder_Zellinhalt
    a2Spaltentitel: RoboSAPiensKeywordsDoubleclickcellArgsSpaltentitel

class RoboSAPiensKeywordsDoubleclickcellResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsDoubleclickcell(TypedDict):
    name: str
    args: RoboSAPiensKeywordsDoubleclickcellArgs
    result: RoboSAPiensKeywordsDoubleclickcellResult
    doc: str

class RoboSAPiensKeywordsDoubleclicktextfieldArgsInhaltSpec(TypedDict):
    ...

class RoboSAPiensKeywordsDoubleclicktextfieldArgsInhalt(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsDoubleclicktextfieldArgsInhaltSpec

class RoboSAPiensKeywordsDoubleclicktextfieldArgs(TypedDict):
    Inhalt: RoboSAPiensKeywordsDoubleclicktextfieldArgsInhalt

class RoboSAPiensKeywordsDoubleclicktextfieldResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsDoubleclicktextfield(TypedDict):
    name: str
    args: RoboSAPiensKeywordsDoubleclicktextfieldArgs
    result: RoboSAPiensKeywordsDoubleclicktextfieldResult
    doc: str

class RoboSAPiensKeywordsExecutetransactionArgsT_CodeSpec(TypedDict):
    ...

class RoboSAPiensKeywordsExecutetransactionArgsT_Code(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsExecutetransactionArgsT_CodeSpec

class RoboSAPiensKeywordsExecutetransactionArgs(TypedDict):
    T_Code: RoboSAPiensKeywordsExecutetransactionArgsT_Code

class RoboSAPiensKeywordsExecutetransactionResult(TypedDict):
    NoSession: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsExecutetransaction(TypedDict):
    name: str
    args: RoboSAPiensKeywordsExecutetransactionArgs
    result: RoboSAPiensKeywordsExecutetransactionResult
    doc: str

class RoboSAPiensKeywordsExportformArgsNameSpec(TypedDict):
    ...

class RoboSAPiensKeywordsExportformArgsName(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsExportformArgsNameSpec

class RoboSAPiensKeywordsExportformArgsVerzeichnisSpec(TypedDict):
    ...

class RoboSAPiensKeywordsExportformArgsVerzeichnis(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsExportformArgsVerzeichnisSpec

class RoboSAPiensKeywordsExportformArgs(TypedDict):
    a1Name: RoboSAPiensKeywordsExportformArgsName
    a2Verzeichnis: RoboSAPiensKeywordsExportformArgsVerzeichnis

class RoboSAPiensKeywordsExportformResult(TypedDict):
    NoSession: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsExportform(TypedDict):
    name: str
    args: RoboSAPiensKeywordsExportformArgs
    result: RoboSAPiensKeywordsExportformResult
    doc: str

class RoboSAPiensKeywordsFilltablecellArgsZeilennummer_Oder_ZellinhaltSpec(TypedDict):
    ...

class RoboSAPiensKeywordsFilltablecellArgsZeilennummer_Oder_Zellinhalt(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsFilltablecellArgsZeilennummer_Oder_ZellinhaltSpec

class RoboSAPiensKeywordsFilltablecellArgsSpaltentitel_Gleich_InhaltSpec(TypedDict):
    ...

class RoboSAPiensKeywordsFilltablecellArgsSpaltentitel_Gleich_Inhalt(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsFilltablecellArgsSpaltentitel_Gleich_InhaltSpec

class RoboSAPiensKeywordsFilltablecellArgs(TypedDict):
    a1Zeilennummer_oder_Zellinhalt: RoboSAPiensKeywordsFilltablecellArgsZeilennummer_Oder_Zellinhalt
    a2Spaltentitel_Gleich_Inhalt: RoboSAPiensKeywordsFilltablecellArgsSpaltentitel_Gleich_Inhalt

class RoboSAPiensKeywordsFilltablecellResult(TypedDict):
    NoSession: str
    InvalidFormat: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsFilltablecell(TypedDict):
    name: str
    args: RoboSAPiensKeywordsFilltablecellArgs
    result: RoboSAPiensKeywordsFilltablecellResult
    doc: str

class RoboSAPiensKeywordsFilltextfieldArgsBeschriftung_Oder_PositionsgeberSpec(TypedDict):
    ...

class RoboSAPiensKeywordsFilltextfieldArgsBeschriftung_Oder_Positionsgeber(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsFilltextfieldArgsBeschriftung_Oder_PositionsgeberSpec

class RoboSAPiensKeywordsFilltextfieldArgsInhaltSpec(TypedDict):
    ...

class RoboSAPiensKeywordsFilltextfieldArgsInhalt(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsFilltextfieldArgsInhaltSpec

class RoboSAPiensKeywordsFilltextfieldArgs(TypedDict):
    a1Beschriftung_oder_Positionsgeber: RoboSAPiensKeywordsFilltextfieldArgsBeschriftung_Oder_Positionsgeber
    a2Inhalt: RoboSAPiensKeywordsFilltextfieldArgsInhalt

class RoboSAPiensKeywordsFilltextfieldResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsFilltextfield(TypedDict):
    name: str
    args: RoboSAPiensKeywordsFilltextfieldArgs
    result: RoboSAPiensKeywordsFilltextfieldResult
    doc: str

class RoboSAPiensKeywordsPushbuttonArgsName_Oder_KurzinfoSpec(TypedDict):
    ...

class RoboSAPiensKeywordsPushbuttonArgsName_Oder_Kurzinfo(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsPushbuttonArgsName_Oder_KurzinfoSpec

class RoboSAPiensKeywordsPushbuttonArgs(TypedDict):
    Name_oder_Kurzinfo: RoboSAPiensKeywordsPushbuttonArgsName_Oder_Kurzinfo

class RoboSAPiensKeywordsPushbuttonResult(TypedDict):
    NoSession: str
    SapError: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsPushbutton(TypedDict):
    name: str
    args: RoboSAPiensKeywordsPushbuttonArgs
    result: RoboSAPiensKeywordsPushbuttonResult
    doc: str

class RoboSAPiensKeywordsPushbuttoncellArgsZeilennummer_Oder_Name_Oder_KurzinfoSpec(TypedDict):
    ...

class RoboSAPiensKeywordsPushbuttoncellArgsZeilennummer_Oder_Name_Oder_Kurzinfo(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsPushbuttoncellArgsZeilennummer_Oder_Name_Oder_KurzinfoSpec

class RoboSAPiensKeywordsPushbuttoncellArgsSpaltentitelSpec(TypedDict):
    ...

class RoboSAPiensKeywordsPushbuttoncellArgsSpaltentitel(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsPushbuttoncellArgsSpaltentitelSpec

class RoboSAPiensKeywordsPushbuttoncellArgs(TypedDict):
    a1Zeilennummer_oder_Name_oder_Kurzinfo: RoboSAPiensKeywordsPushbuttoncellArgsZeilennummer_Oder_Name_Oder_Kurzinfo
    a2Spaltentitel: RoboSAPiensKeywordsPushbuttoncellArgsSpaltentitel

class RoboSAPiensKeywordsPushbuttoncellResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsPushbuttoncell(TypedDict):
    name: str
    args: RoboSAPiensKeywordsPushbuttoncellArgs
    result: RoboSAPiensKeywordsPushbuttoncellResult
    doc: str

class RoboSAPiensKeywordsReadtextfieldArgsBeschriftung_Oder_PositionsgeberSpec(TypedDict):
    ...

class RoboSAPiensKeywordsReadtextfieldArgsBeschriftung_Oder_Positionsgeber(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsReadtextfieldArgsBeschriftung_Oder_PositionsgeberSpec

class RoboSAPiensKeywordsReadtextfieldArgs(TypedDict):
    Beschriftung_oder_Positionsgeber: RoboSAPiensKeywordsReadtextfieldArgsBeschriftung_Oder_Positionsgeber

class RoboSAPiensKeywordsReadtextfieldResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsReadtextfield(TypedDict):
    name: str
    args: RoboSAPiensKeywordsReadtextfieldArgs
    result: RoboSAPiensKeywordsReadtextfieldResult
    doc: str

class RoboSAPiensKeywordsReadtextArgsInhaltSpec(TypedDict):
    ...

class RoboSAPiensKeywordsReadtextArgsInhalt(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsReadtextArgsInhaltSpec

class RoboSAPiensKeywordsReadtextArgs(TypedDict):
    Inhalt: RoboSAPiensKeywordsReadtextArgsInhalt

class RoboSAPiensKeywordsReadtextResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsReadtext(TypedDict):
    name: str
    args: RoboSAPiensKeywordsReadtextArgs
    result: RoboSAPiensKeywordsReadtextResult
    doc: str

class RoboSAPiensKeywordsReadtablecellArgsZeilennummer_Oder_ZellinhaltSpec(TypedDict):
    ...

class RoboSAPiensKeywordsReadtablecellArgsZeilennummer_Oder_Zellinhalt(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsReadtablecellArgsZeilennummer_Oder_ZellinhaltSpec

class RoboSAPiensKeywordsReadtablecellArgsSpaltentitelSpec(TypedDict):
    ...

class RoboSAPiensKeywordsReadtablecellArgsSpaltentitel(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsReadtablecellArgsSpaltentitelSpec

class RoboSAPiensKeywordsReadtablecellArgs(TypedDict):
    a1Zeilennummer_oder_Zellinhalt: RoboSAPiensKeywordsReadtablecellArgsZeilennummer_Oder_Zellinhalt
    a2Spaltentitel: RoboSAPiensKeywordsReadtablecellArgsSpaltentitel

class RoboSAPiensKeywordsReadtablecellResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsReadtablecell(TypedDict):
    name: str
    args: RoboSAPiensKeywordsReadtablecellArgs
    result: RoboSAPiensKeywordsReadtablecellResult
    doc: str

class RoboSAPiensKeywordsSavescreenshotArgsAufnahmenverzeichnisSpec(TypedDict):
    ...

class RoboSAPiensKeywordsSavescreenshotArgsAufnahmenverzeichnis(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsSavescreenshotArgsAufnahmenverzeichnisSpec

class RoboSAPiensKeywordsSavescreenshotArgs(TypedDict):
    Aufnahmenverzeichnis: RoboSAPiensKeywordsSavescreenshotArgsAufnahmenverzeichnis

class RoboSAPiensKeywordsSavescreenshotResult(TypedDict):
    NoSession: str
    UNCPath: str
    NoAbsPath: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSavescreenshot(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSavescreenshotArgs
    result: RoboSAPiensKeywordsSavescreenshotResult
    doc: str

class RoboSAPiensKeywordsSelectcellArgsZeilennummer_Oder_ZellinhaltSpec(TypedDict):
    ...

class RoboSAPiensKeywordsSelectcellArgsZeilennummer_Oder_Zellinhalt(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsSelectcellArgsZeilennummer_Oder_ZellinhaltSpec

class RoboSAPiensKeywordsSelectcellArgsSpaltentitelSpec(TypedDict):
    ...

class RoboSAPiensKeywordsSelectcellArgsSpaltentitel(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsSelectcellArgsSpaltentitelSpec

class RoboSAPiensKeywordsSelectcellArgs(TypedDict):
    a1Zeilennummer_oder_Zellinhalt: RoboSAPiensKeywordsSelectcellArgsZeilennummer_Oder_Zellinhalt
    a2Spaltentitel: RoboSAPiensKeywordsSelectcellArgsSpaltentitel

class RoboSAPiensKeywordsSelectcellResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSelectcell(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSelectcellArgs
    result: RoboSAPiensKeywordsSelectcellResult
    doc: str

class RoboSAPiensKeywordsSelectcomboboxentryArgsNameSpec(TypedDict):
    ...

class RoboSAPiensKeywordsSelectcomboboxentryArgsName(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsSelectcomboboxentryArgsNameSpec

class RoboSAPiensKeywordsSelectcomboboxentryArgsEintragSpec(TypedDict):
    ...

class RoboSAPiensKeywordsSelectcomboboxentryArgsEintrag(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsSelectcomboboxentryArgsEintragSpec

class RoboSAPiensKeywordsSelectcomboboxentryArgs(TypedDict):
    a1Name: RoboSAPiensKeywordsSelectcomboboxentryArgsName
    a2Eintrag: RoboSAPiensKeywordsSelectcomboboxentryArgsEintrag

class RoboSAPiensKeywordsSelectcomboboxentryResult(TypedDict):
    NoSession: str
    NotFound: str
    EntryNotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSelectcomboboxentry(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSelectcomboboxentryArgs
    result: RoboSAPiensKeywordsSelectcomboboxentryResult
    doc: str

class RoboSAPiensKeywordsSelectradiobuttonArgsBeschriftung_Oder_PositionsgeberSpec(TypedDict):
    ...

class RoboSAPiensKeywordsSelectradiobuttonArgsBeschriftung_Oder_Positionsgeber(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsSelectradiobuttonArgsBeschriftung_Oder_PositionsgeberSpec

class RoboSAPiensKeywordsSelectradiobuttonArgs(TypedDict):
    Beschriftung_oder_Positionsgeber: RoboSAPiensKeywordsSelectradiobuttonArgsBeschriftung_Oder_Positionsgeber

class RoboSAPiensKeywordsSelectradiobuttonResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSelectradiobutton(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSelectradiobuttonArgs
    result: RoboSAPiensKeywordsSelectradiobuttonResult
    doc: str

class RoboSAPiensKeywordsSelecttextfieldArgsBeschriftungen_Oder_InhaltSpec(TypedDict):
    ...

class RoboSAPiensKeywordsSelecttextfieldArgsBeschriftungen_Oder_Inhalt(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsSelecttextfieldArgsBeschriftungen_Oder_InhaltSpec

class RoboSAPiensKeywordsSelecttextfieldArgs(TypedDict):
    Beschriftungen_oder_Inhalt: RoboSAPiensKeywordsSelecttextfieldArgsBeschriftungen_Oder_Inhalt

class RoboSAPiensKeywordsSelecttextfieldResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSelecttextfield(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSelecttextfieldArgs
    result: RoboSAPiensKeywordsSelecttextfieldResult
    doc: str

class RoboSAPiensKeywordsSelecttextlineArgsInhaltSpec(TypedDict):
    ...

class RoboSAPiensKeywordsSelecttextlineArgsInhalt(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsSelecttextlineArgsInhaltSpec

class RoboSAPiensKeywordsSelecttextlineArgs(TypedDict):
    Inhalt: RoboSAPiensKeywordsSelecttextlineArgsInhalt

class RoboSAPiensKeywordsSelecttextlineResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsSelecttextline(TypedDict):
    name: str
    args: RoboSAPiensKeywordsSelecttextlineArgs
    result: RoboSAPiensKeywordsSelecttextlineResult
    doc: str

class RoboSAPiensKeywordsTickcheckboxArgsBeschriftung_Oder_PositionsgeberSpec(TypedDict):
    ...

class RoboSAPiensKeywordsTickcheckboxArgsBeschriftung_Oder_Positionsgeber(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsTickcheckboxArgsBeschriftung_Oder_PositionsgeberSpec

class RoboSAPiensKeywordsTickcheckboxArgs(TypedDict):
    Beschriftung_oder_Positionsgeber: RoboSAPiensKeywordsTickcheckboxArgsBeschriftung_Oder_Positionsgeber

class RoboSAPiensKeywordsTickcheckboxResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsTickcheckbox(TypedDict):
    name: str
    args: RoboSAPiensKeywordsTickcheckboxArgs
    result: RoboSAPiensKeywordsTickcheckboxResult
    doc: str

class RoboSAPiensKeywordsUntickcheckboxArgsBeschriftung_Oder_PositionsgeberSpec(TypedDict):
    ...

class RoboSAPiensKeywordsUntickcheckboxArgsBeschriftung_Oder_Positionsgeber(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsUntickcheckboxArgsBeschriftung_Oder_PositionsgeberSpec

class RoboSAPiensKeywordsUntickcheckboxArgs(TypedDict):
    Beschriftung_oder_Positionsgeber: RoboSAPiensKeywordsUntickcheckboxArgsBeschriftung_Oder_Positionsgeber

class RoboSAPiensKeywordsUntickcheckboxResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsUntickcheckbox(TypedDict):
    name: str
    args: RoboSAPiensKeywordsUntickcheckboxArgs
    result: RoboSAPiensKeywordsUntickcheckboxResult
    doc: str

class RoboSAPiensKeywordsTickcheckboxcellArgsZeilennummerSpec(TypedDict):
    ...

class RoboSAPiensKeywordsTickcheckboxcellArgsZeilennummer(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsTickcheckboxcellArgsZeilennummerSpec

class RoboSAPiensKeywordsTickcheckboxcellArgsSpaltentitelSpec(TypedDict):
    ...

class RoboSAPiensKeywordsTickcheckboxcellArgsSpaltentitel(TypedDict):
    name: str
    spec: RoboSAPiensKeywordsTickcheckboxcellArgsSpaltentitelSpec

class RoboSAPiensKeywordsTickcheckboxcellArgs(TypedDict):
    a1Zeilennummer: RoboSAPiensKeywordsTickcheckboxcellArgsZeilennummer
    a2Spaltentitel: RoboSAPiensKeywordsTickcheckboxcellArgsSpaltentitel

class RoboSAPiensKeywordsTickcheckboxcellResult(TypedDict):
    NoSession: str
    NotFound: str
    Pass: str
    Exception: str

class RoboSAPiensKeywordsTickcheckboxcell(TypedDict):
    name: str
    args: RoboSAPiensKeywordsTickcheckboxcellArgs
    result: RoboSAPiensKeywordsTickcheckboxcellResult
    doc: str

class RoboSAPiensKeywordsGetwindowtitleArgs(TypedDict):
    ...

class RoboSAPiensKeywordsGetwindowtitleResult(TypedDict):
    NoSession: str
    Pass: str

class RoboSAPiensKeywordsGetwindowtitle(TypedDict):
    name: str
    args: RoboSAPiensKeywordsGetwindowtitleArgs
    result: RoboSAPiensKeywordsGetwindowtitleResult
    doc: str

class RoboSAPiensKeywordsGetwindowtextArgs(TypedDict):
    ...

class RoboSAPiensKeywordsGetwindowtextResult(TypedDict):
    NoSession: str
    Pass: str

class RoboSAPiensKeywordsGetwindowtext(TypedDict):
    name: str
    args: RoboSAPiensKeywordsGetwindowtextArgs
    result: RoboSAPiensKeywordsGetwindowtextResult
    doc: str

class RoboSAPiensKeywords(TypedDict):
    ActivateTab: RoboSAPiensKeywordsActivatetab
    OpenSAP: RoboSAPiensKeywordsOpensap
    CloseConnection: RoboSAPiensKeywordsCloseconnection
    CloseSAP: RoboSAPiensKeywordsClosesap
    ExportTree: RoboSAPiensKeywordsExporttree
    AttachToRunningSAP: RoboSAPiensKeywordsAttachtorunningsap
    ConnectToServer: RoboSAPiensKeywordsConnecttoserver
    DoubleClickCell: RoboSAPiensKeywordsDoubleclickcell
    DoubleClickTextField: RoboSAPiensKeywordsDoubleclicktextfield
    ExecuteTransaction: RoboSAPiensKeywordsExecutetransaction
    ExportForm: RoboSAPiensKeywordsExportform
    FillTableCell: RoboSAPiensKeywordsFilltablecell
    FillTextField: RoboSAPiensKeywordsFilltextfield
    PushButton: RoboSAPiensKeywordsPushbutton
    PushButtonCell: RoboSAPiensKeywordsPushbuttoncell
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

class RoboSAPiensSpecs(TypedDict):
    ...

class RoboSAPiens(TypedDict):
    doc: RoboSAPiensDoc
    args: RoboSAPiensArgs
    keywords: RoboSAPiensKeywords
    specs: RoboSAPiensSpecs