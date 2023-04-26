from typing import Literal, Tuple, TypedDict

class LocalizedRoboSAPiensDoc(TypedDict):
    intro: Tuple[Literal['2676661990'], str]
    init: Tuple[Literal['0'], str]

class LocalizedRoboSAPiensArgsPort(TypedDict):
    name: Tuple[Literal['1133600204'], str]
    default: Literal[8270]
    doc: Tuple[Literal['2718491382'], str]

class LocalizedRoboSAPiensArgsPresenter_Mode(TypedDict):
    name: Tuple[Literal['781265386'], str]
    default: Literal[False]
    doc: Tuple[Literal['3421082408'], str]

class LocalizedRoboSAPiensArgs(TypedDict):
    a1port: LocalizedRoboSAPiensArgsPort
    a2presenter_mode: LocalizedRoboSAPiensArgsPresenter_Mode

class LocalizedRoboSAPiensKeywordsActivatetabArgsReiternameSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsActivatetabArgsReitername(TypedDict):
    name: Tuple[Literal['3665195662'], str]
    spec: LocalizedRoboSAPiensKeywordsActivatetabArgsReiternameSpec

class LocalizedRoboSAPiensKeywordsActivatetabArgs(TypedDict):
    Reitername: LocalizedRoboSAPiensKeywordsActivatetabArgsReitername

class LocalizedRoboSAPiensKeywordsActivatetabResult(TypedDict):
    NoSession: Tuple[Literal['2754484086'], str]
    NotFound: Tuple[Literal['2994771228'], str]
    SapError: Tuple[Literal['3246364722'], str]
    Pass: Tuple[Literal['4294349699'], str]
    Exception: Tuple[Literal['2577256712'], str]

class LocalizedRoboSAPiensKeywordsActivatetab(TypedDict):
    name: Tuple[Literal['1870139227'], str]
    args: LocalizedRoboSAPiensKeywordsActivatetabArgs
    result: LocalizedRoboSAPiensKeywordsActivatetabResult
    doc: Tuple[Literal['2453389726'], str]

class LocalizedRoboSAPiensKeywordsOpensapArgsPfadSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsOpensapArgsPfad(TypedDict):
    name: Tuple[Literal['190089999'], str]
    spec: LocalizedRoboSAPiensKeywordsOpensapArgsPfadSpec

class LocalizedRoboSAPiensKeywordsOpensapArgs(TypedDict):
    Pfad: LocalizedRoboSAPiensKeywordsOpensapArgsPfad

class LocalizedRoboSAPiensKeywordsOpensapResult(TypedDict):
    Pass: Tuple[Literal['3933791589'], str]
    SAPNotStarted: Tuple[Literal['4005776825'], str]
    Exception: Tuple[Literal['2772047805'], str]

class LocalizedRoboSAPiensKeywordsOpensap(TypedDict):
    name: Tuple[Literal['1259182241'], str]
    args: LocalizedRoboSAPiensKeywordsOpensapArgs
    result: LocalizedRoboSAPiensKeywordsOpensapResult
    doc: Tuple[Literal['309583061'], str]

class LocalizedRoboSAPiensKeywordsCloseconnectionArgs(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsCloseconnectionResult(TypedDict):
    NoSapGui: Tuple[Literal['2987622841'], str]
    NoGuiScripting: Tuple[Literal['2929771598'], str]
    NoConnection: Tuple[Literal['509780556'], str]
    NoSession: Tuple[Literal['2754484086'], str]
    Pass: Tuple[Literal['1657006605'], str]
    Exception: Tuple[Literal['2209141929'], str]

class LocalizedRoboSAPiensKeywordsCloseconnection(TypedDict):
    name: Tuple[Literal['938374979'], str]
    args: LocalizedRoboSAPiensKeywordsCloseconnectionArgs
    result: LocalizedRoboSAPiensKeywordsCloseconnectionResult
    doc: Tuple[Literal['1736796211'], str]

class LocalizedRoboSAPiensKeywordsClosesapArgs(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsClosesapResult(TypedDict):
    NoSapGui: Tuple[Literal['2987622841'], str]
    Pass: Tuple[Literal['2970606098'], str]

class LocalizedRoboSAPiensKeywordsClosesap(TypedDict):
    name: Tuple[Literal['1795765665'], str]
    args: LocalizedRoboSAPiensKeywordsClosesapArgs
    result: LocalizedRoboSAPiensKeywordsClosesapResult
    doc: Tuple[Literal['1112371689'], str]

class LocalizedRoboSAPiensKeywordsExporttreeArgsDateipfadSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsExporttreeArgsDateipfad(TypedDict):
    name: Tuple[Literal['1769741420'], str]
    spec: LocalizedRoboSAPiensKeywordsExporttreeArgsDateipfadSpec

class LocalizedRoboSAPiensKeywordsExporttreeArgs(TypedDict):
    Dateipfad: LocalizedRoboSAPiensKeywordsExporttreeArgsDateipfad

class LocalizedRoboSAPiensKeywordsExporttreeResult(TypedDict):
    NoSession: Tuple[Literal['2754484086'], str]
    NotFound: Tuple[Literal['811568965'], str]
    Pass: Tuple[Literal['176551133'], str]
    Exception: Tuple[Literal['1542087750'], str]

class LocalizedRoboSAPiensKeywordsExporttree(TypedDict):
    name: Tuple[Literal['1188312707'], str]
    args: LocalizedRoboSAPiensKeywordsExporttreeArgs
    result: LocalizedRoboSAPiensKeywordsExporttreeResult
    doc: Tuple[Literal['1719447038'], str]

class LocalizedRoboSAPiensKeywordsAttachtorunningsapArgs(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsAttachtorunningsapResult(TypedDict):
    NoSapGui: Tuple[Literal['2987622841'], str]
    NoGuiScripting: Tuple[Literal['2929771598'], str]
    NoConnection: Tuple[Literal['509780556'], str]
    NoServerScripting: Tuple[Literal['3495213352'], str]
    Pass: Tuple[Literal['2481655346'], str]
    Exception: Tuple[Literal['3120673076'], str]

class LocalizedRoboSAPiensKeywordsAttachtorunningsap(TypedDict):
    name: Tuple[Literal['4126309856'], str]
    args: LocalizedRoboSAPiensKeywordsAttachtorunningsapArgs
    result: LocalizedRoboSAPiensKeywordsAttachtorunningsapResult
    doc: Tuple[Literal['866468958'], str]

class LocalizedRoboSAPiensKeywordsConnecttoserverArgsServernameSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsConnecttoserverArgsServername(TypedDict):
    name: Tuple[Literal['763456934'], str]
    spec: LocalizedRoboSAPiensKeywordsConnecttoserverArgsServernameSpec

class LocalizedRoboSAPiensKeywordsConnecttoserverArgs(TypedDict):
    Servername: LocalizedRoboSAPiensKeywordsConnecttoserverArgsServername

class LocalizedRoboSAPiensKeywordsConnecttoserverResult(TypedDict):
    NoSapGui: Tuple[Literal['2987622841'], str]
    NoGuiScripting: Tuple[Literal['2929771598'], str]
    Pass: Tuple[Literal['1014238539'], str]
    SapError: Tuple[Literal['3246364722'], str]
    NoServerScripting: Tuple[Literal['3495213352'], str]
    Exception: Tuple[Literal['667377482'], str]

class LocalizedRoboSAPiensKeywordsConnecttoserver(TypedDict):
    name: Tuple[Literal['1377779562'], str]
    args: LocalizedRoboSAPiensKeywordsConnecttoserverArgs
    result: LocalizedRoboSAPiensKeywordsConnecttoserverResult
    doc: Tuple[Literal['287368400'], str]

class LocalizedRoboSAPiensKeywordsDoubleclickcellArgsZeilennummer_Oder_ZellinhaltSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsDoubleclickcellArgsZeilennummer_Oder_Zellinhalt(TypedDict):
    name: Tuple[Literal['3668559387'], str]
    spec: LocalizedRoboSAPiensKeywordsDoubleclickcellArgsZeilennummer_Oder_ZellinhaltSpec

class LocalizedRoboSAPiensKeywordsDoubleclickcellArgsSpaltentitelSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsDoubleclickcellArgsSpaltentitel(TypedDict):
    name: Tuple[Literal['2102626174'], str]
    spec: LocalizedRoboSAPiensKeywordsDoubleclickcellArgsSpaltentitelSpec

class LocalizedRoboSAPiensKeywordsDoubleclickcellArgs(TypedDict):
    a1Zeilennummer_oder_Zellinhalt: LocalizedRoboSAPiensKeywordsDoubleclickcellArgsZeilennummer_Oder_Zellinhalt
    a2Spaltentitel: LocalizedRoboSAPiensKeywordsDoubleclickcellArgsSpaltentitel

class LocalizedRoboSAPiensKeywordsDoubleclickcellResult(TypedDict):
    NoSession: Tuple[Literal['2754484086'], str]
    NotFound: Tuple[Literal['2770335633'], str]
    Pass: Tuple[Literal['1249017752'], str]
    Exception: Tuple[Literal['2384367029'], str]

class LocalizedRoboSAPiensKeywordsDoubleclickcell(TypedDict):
    name: Tuple[Literal['2108476291'], str]
    args: LocalizedRoboSAPiensKeywordsDoubleclickcellArgs
    result: LocalizedRoboSAPiensKeywordsDoubleclickcellResult
    doc: Tuple[Literal['1790929105'], str]

class LocalizedRoboSAPiensKeywordsDoubleclicktextfieldArgsInhaltSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsDoubleclicktextfieldArgsInhalt(TypedDict):
    name: Tuple[Literal['4274335913'], str]
    spec: LocalizedRoboSAPiensKeywordsDoubleclicktextfieldArgsInhaltSpec

class LocalizedRoboSAPiensKeywordsDoubleclicktextfieldArgs(TypedDict):
    Inhalt: LocalizedRoboSAPiensKeywordsDoubleclicktextfieldArgsInhalt

class LocalizedRoboSAPiensKeywordsDoubleclicktextfieldResult(TypedDict):
    NoSession: Tuple[Literal['2754484086'], str]
    NotFound: Tuple[Literal['3855369076'], str]
    Pass: Tuple[Literal['1611309101'], str]
    Exception: Tuple[Literal['504842288'], str]

class LocalizedRoboSAPiensKeywordsDoubleclicktextfield(TypedDict):
    name: Tuple[Literal['3737103423'], str]
    args: LocalizedRoboSAPiensKeywordsDoubleclicktextfieldArgs
    result: LocalizedRoboSAPiensKeywordsDoubleclicktextfieldResult
    doc: Tuple[Literal['2849153724'], str]

class LocalizedRoboSAPiensKeywordsExecutetransactionArgsT_CodeSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsExecutetransactionArgsT_Code(TypedDict):
    name: Tuple[Literal['1795027938'], str]
    spec: LocalizedRoboSAPiensKeywordsExecutetransactionArgsT_CodeSpec

class LocalizedRoboSAPiensKeywordsExecutetransactionArgs(TypedDict):
    T_Code: LocalizedRoboSAPiensKeywordsExecutetransactionArgsT_Code

class LocalizedRoboSAPiensKeywordsExecutetransactionResult(TypedDict):
    NoSession: Tuple[Literal['2754484086'], str]
    Pass: Tuple[Literal['468573121'], str]
    Exception: Tuple[Literal['3958687903'], str]

class LocalizedRoboSAPiensKeywordsExecutetransaction(TypedDict):
    name: Tuple[Literal['2997404008'], str]
    args: LocalizedRoboSAPiensKeywordsExecutetransactionArgs
    result: LocalizedRoboSAPiensKeywordsExecutetransactionResult
    doc: Tuple[Literal['4152429702'], str]

class LocalizedRoboSAPiensKeywordsExportformArgsNameSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsExportformArgsName(TypedDict):
    name: Tuple[Literal['1579384326'], str]
    spec: LocalizedRoboSAPiensKeywordsExportformArgsNameSpec

class LocalizedRoboSAPiensKeywordsExportformArgsVerzeichnisSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsExportformArgsVerzeichnis(TypedDict):
    name: Tuple[Literal['1182287066'], str]
    spec: LocalizedRoboSAPiensKeywordsExportformArgsVerzeichnisSpec

class LocalizedRoboSAPiensKeywordsExportformArgs(TypedDict):
    a1Name: LocalizedRoboSAPiensKeywordsExportformArgsName
    a2Verzeichnis: LocalizedRoboSAPiensKeywordsExportformArgsVerzeichnis

class LocalizedRoboSAPiensKeywordsExportformResult(TypedDict):
    NoSession: Tuple[Literal['2754484086'], str]
    Pass: Tuple[Literal['1972246596'], str]
    Exception: Tuple[Literal['487625120'], str]

class LocalizedRoboSAPiensKeywordsExportform(TypedDict):
    name: Tuple[Literal['1168873090'], str]
    args: LocalizedRoboSAPiensKeywordsExportformArgs
    result: LocalizedRoboSAPiensKeywordsExportformResult
    doc: Tuple[Literal['3527465284'], str]

class LocalizedRoboSAPiensKeywordsFilltablecellArgsZeilennummer_Oder_ZellinhaltSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsFilltablecellArgsZeilennummer_Oder_Zellinhalt(TypedDict):
    name: Tuple[Literal['3954689097'], str]
    spec: LocalizedRoboSAPiensKeywordsFilltablecellArgsZeilennummer_Oder_ZellinhaltSpec

class LocalizedRoboSAPiensKeywordsFilltablecellArgsSpaltentitel_Gleich_InhaltSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsFilltablecellArgsSpaltentitel_Gleich_Inhalt(TypedDict):
    name: Tuple[Literal['2701366812'], str]
    spec: LocalizedRoboSAPiensKeywordsFilltablecellArgsSpaltentitel_Gleich_InhaltSpec

class LocalizedRoboSAPiensKeywordsFilltablecellArgs(TypedDict):
    a1Zeilennummer_oder_Zellinhalt: LocalizedRoboSAPiensKeywordsFilltablecellArgsZeilennummer_Oder_Zellinhalt
    a2Spaltentitel_Gleich_Inhalt: LocalizedRoboSAPiensKeywordsFilltablecellArgsSpaltentitel_Gleich_Inhalt

class LocalizedRoboSAPiensKeywordsFilltablecellResult(TypedDict):
    NoSession: Tuple[Literal['2754484086'], str]
    InvalidFormat: Tuple[Literal['280110049'], str]
    NotFound: Tuple[Literal['3381319755'], str]
    NotChangeable: Tuple[Literal['2078310074'], str]
    Pass: Tuple[Literal['2876607603'], str]
    Exception: Tuple[Literal['1958379303'], str]

class LocalizedRoboSAPiensKeywordsFilltablecell(TypedDict):
    name: Tuple[Literal['1010164935'], str]
    args: LocalizedRoboSAPiensKeywordsFilltablecellArgs
    result: LocalizedRoboSAPiensKeywordsFilltablecellResult
    doc: Tuple[Literal['917168571'], str]

class LocalizedRoboSAPiensKeywordsFilltextfieldArgsBeschriftung_Oder_PositionsgeberSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsFilltextfieldArgsBeschriftung_Oder_Positionsgeber(TypedDict):
    name: Tuple[Literal['2051440239'], str]
    spec: LocalizedRoboSAPiensKeywordsFilltextfieldArgsBeschriftung_Oder_PositionsgeberSpec

class LocalizedRoboSAPiensKeywordsFilltextfieldArgsInhaltSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsFilltextfieldArgsInhalt(TypedDict):
    name: Tuple[Literal['4274335913'], str]
    spec: LocalizedRoboSAPiensKeywordsFilltextfieldArgsInhaltSpec

class LocalizedRoboSAPiensKeywordsFilltextfieldArgs(TypedDict):
    a1Beschriftung_oder_Positionsgeber: LocalizedRoboSAPiensKeywordsFilltextfieldArgsBeschriftung_Oder_Positionsgeber
    a2Inhalt: LocalizedRoboSAPiensKeywordsFilltextfieldArgsInhalt

class LocalizedRoboSAPiensKeywordsFilltextfieldResult(TypedDict):
    NoSession: Tuple[Literal['2754484086'], str]
    NotFound: Tuple[Literal['2917845132'], str]
    Pass: Tuple[Literal['1361669956'], str]
    Exception: Tuple[Literal['3667643114'], str]

class LocalizedRoboSAPiensKeywordsFilltextfield(TypedDict):
    name: Tuple[Literal['3103200585'], str]
    args: LocalizedRoboSAPiensKeywordsFilltextfieldArgs
    result: LocalizedRoboSAPiensKeywordsFilltextfieldResult
    doc: Tuple[Literal['3658065712'], str]

class LocalizedRoboSAPiensKeywordsPushbuttonArgsName_Oder_KurzinfoSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsPushbuttonArgsName_Oder_Kurzinfo(TypedDict):
    name: Tuple[Literal['894332414'], str]
    spec: LocalizedRoboSAPiensKeywordsPushbuttonArgsName_Oder_KurzinfoSpec

class LocalizedRoboSAPiensKeywordsPushbuttonArgs(TypedDict):
    Name_oder_Kurzinfo: LocalizedRoboSAPiensKeywordsPushbuttonArgsName_Oder_Kurzinfo

class LocalizedRoboSAPiensKeywordsPushbuttonResult(TypedDict):
    NoSession: Tuple[Literal['2754484086'], str]
    SapError: Tuple[Literal['3246364722'], str]
    NotFound: Tuple[Literal['3063247197'], str]
    Pass: Tuple[Literal['2346783035'], str]
    Exception: Tuple[Literal['1002997848'], str]

class LocalizedRoboSAPiensKeywordsPushbutton(TypedDict):
    name: Tuple[Literal['2326550334'], str]
    args: LocalizedRoboSAPiensKeywordsPushbuttonArgs
    result: LocalizedRoboSAPiensKeywordsPushbuttonResult
    doc: Tuple[Literal['1916622155'], str]

class LocalizedRoboSAPiensKeywordsPushbuttoncellArgsZeilennummer_Oder_Name_Oder_KurzinfoSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsPushbuttoncellArgsZeilennummer_Oder_Name_Oder_Kurzinfo(TypedDict):
    name: Tuple[Literal['1299279537'], str]
    spec: LocalizedRoboSAPiensKeywordsPushbuttoncellArgsZeilennummer_Oder_Name_Oder_KurzinfoSpec

class LocalizedRoboSAPiensKeywordsPushbuttoncellArgsSpaltentitelSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsPushbuttoncellArgsSpaltentitel(TypedDict):
    name: Tuple[Literal['2102626174'], str]
    spec: LocalizedRoboSAPiensKeywordsPushbuttoncellArgsSpaltentitelSpec

class LocalizedRoboSAPiensKeywordsPushbuttoncellArgs(TypedDict):
    a1Zeilennummer_oder_Name_oder_Kurzinfo: LocalizedRoboSAPiensKeywordsPushbuttoncellArgsZeilennummer_Oder_Name_Oder_Kurzinfo
    a2Spaltentitel: LocalizedRoboSAPiensKeywordsPushbuttoncellArgsSpaltentitel

class LocalizedRoboSAPiensKeywordsPushbuttoncellResult(TypedDict):
    NoSession: Tuple[Literal['2754484086'], str]
    NotFound: Tuple[Literal['2199892932'], str]
    Pass: Tuple[Literal['1649470590'], str]
    Exception: Tuple[Literal['1751102722'], str]

class LocalizedRoboSAPiensKeywordsPushbuttoncell(TypedDict):
    name: Tuple[Literal['349686496'], str]
    args: LocalizedRoboSAPiensKeywordsPushbuttoncellArgs
    result: LocalizedRoboSAPiensKeywordsPushbuttoncellResult
    doc: Tuple[Literal['3042146913'], str]

class LocalizedRoboSAPiensKeywordsReadtextfieldArgsBeschriftung_Oder_PositionsgeberSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsReadtextfieldArgsBeschriftung_Oder_Positionsgeber(TypedDict):
    name: Tuple[Literal['2051440239'], str]
    spec: LocalizedRoboSAPiensKeywordsReadtextfieldArgsBeschriftung_Oder_PositionsgeberSpec

class LocalizedRoboSAPiensKeywordsReadtextfieldArgs(TypedDict):
    Beschriftung_oder_Positionsgeber: LocalizedRoboSAPiensKeywordsReadtextfieldArgsBeschriftung_Oder_Positionsgeber

class LocalizedRoboSAPiensKeywordsReadtextfieldResult(TypedDict):
    NoSession: Tuple[Literal['2754484086'], str]
    NotFound: Tuple[Literal['2917845132'], str]
    Pass: Tuple[Literal['2524131110'], str]
    Exception: Tuple[Literal['2613451948'], str]

class LocalizedRoboSAPiensKeywordsReadtextfield(TypedDict):
    name: Tuple[Literal['490498248'], str]
    args: LocalizedRoboSAPiensKeywordsReadtextfieldArgs
    result: LocalizedRoboSAPiensKeywordsReadtextfieldResult
    doc: Tuple[Literal['912380966'], str]

class LocalizedRoboSAPiensKeywordsReadtextArgsInhaltSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsReadtextArgsInhalt(TypedDict):
    name: Tuple[Literal['4274335913'], str]
    spec: LocalizedRoboSAPiensKeywordsReadtextArgsInhaltSpec

class LocalizedRoboSAPiensKeywordsReadtextArgs(TypedDict):
    Inhalt: LocalizedRoboSAPiensKeywordsReadtextArgsInhalt

class LocalizedRoboSAPiensKeywordsReadtextResult(TypedDict):
    NoSession: Tuple[Literal['2754484086'], str]
    NotFound: Tuple[Literal['837183792'], str]
    Pass: Tuple[Literal['1360175273'], str]
    Exception: Tuple[Literal['3136337781'], str]

class LocalizedRoboSAPiensKeywordsReadtext(TypedDict):
    name: Tuple[Literal['3879608701'], str]
    args: LocalizedRoboSAPiensKeywordsReadtextArgs
    result: LocalizedRoboSAPiensKeywordsReadtextResult
    doc: Tuple[Literal['858400447'], str]

class LocalizedRoboSAPiensKeywordsReadtablecellArgsZeilennummer_Oder_ZellinhaltSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsReadtablecellArgsZeilennummer_Oder_Zellinhalt(TypedDict):
    name: Tuple[Literal['3954689097'], str]
    spec: LocalizedRoboSAPiensKeywordsReadtablecellArgsZeilennummer_Oder_ZellinhaltSpec

class LocalizedRoboSAPiensKeywordsReadtablecellArgsSpaltentitelSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsReadtablecellArgsSpaltentitel(TypedDict):
    name: Tuple[Literal['2102626174'], str]
    spec: LocalizedRoboSAPiensKeywordsReadtablecellArgsSpaltentitelSpec

class LocalizedRoboSAPiensKeywordsReadtablecellArgs(TypedDict):
    a1Zeilennummer_oder_Zellinhalt: LocalizedRoboSAPiensKeywordsReadtablecellArgsZeilennummer_Oder_Zellinhalt
    a2Spaltentitel: LocalizedRoboSAPiensKeywordsReadtablecellArgsSpaltentitel

class LocalizedRoboSAPiensKeywordsReadtablecellResult(TypedDict):
    NoSession: Tuple[Literal['2754484086'], str]
    NotFound: Tuple[Literal['2770335633'], str]
    Pass: Tuple[Literal['2745435444'], str]
    Exception: Tuple[Literal['1272098876'], str]

class LocalizedRoboSAPiensKeywordsReadtablecell(TypedDict):
    name: Tuple[Literal['389153112'], str]
    args: LocalizedRoboSAPiensKeywordsReadtablecellArgs
    result: LocalizedRoboSAPiensKeywordsReadtablecellResult
    doc: Tuple[Literal['666704855'], str]

class LocalizedRoboSAPiensKeywordsSavescreenshotArgsAufnahmenverzeichnisSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsSavescreenshotArgsAufnahmenverzeichnis(TypedDict):
    name: Tuple[Literal['1769741420'], str]
    spec: LocalizedRoboSAPiensKeywordsSavescreenshotArgsAufnahmenverzeichnisSpec

class LocalizedRoboSAPiensKeywordsSavescreenshotArgs(TypedDict):
    Aufnahmenverzeichnis: LocalizedRoboSAPiensKeywordsSavescreenshotArgsAufnahmenverzeichnis

class LocalizedRoboSAPiensKeywordsSavescreenshotResult(TypedDict):
    NoSession: Tuple[Literal['2754484086'], str]
    InvalidPath: Tuple[Literal['2844012395'], str]
    UNCPath: Tuple[Literal['2462162559'], str]
    NoAbsPath: Tuple[Literal['2858082864'], str]
    Pass: Tuple[Literal['1427858469'], str]
    Exception: Tuple[Literal['3250735497'], str]

class LocalizedRoboSAPiensKeywordsSavescreenshot(TypedDict):
    name: Tuple[Literal['2178392450'], str]
    args: LocalizedRoboSAPiensKeywordsSavescreenshotArgs
    result: LocalizedRoboSAPiensKeywordsSavescreenshotResult
    doc: Tuple[Literal['3488461470'], str]

class LocalizedRoboSAPiensKeywordsSelectcellArgsZeilennummer_Oder_ZellinhaltSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsSelectcellArgsZeilennummer_Oder_Zellinhalt(TypedDict):
    name: Tuple[Literal['3954689097'], str]
    spec: LocalizedRoboSAPiensKeywordsSelectcellArgsZeilennummer_Oder_ZellinhaltSpec

class LocalizedRoboSAPiensKeywordsSelectcellArgsSpaltentitelSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsSelectcellArgsSpaltentitel(TypedDict):
    name: Tuple[Literal['2102626174'], str]
    spec: LocalizedRoboSAPiensKeywordsSelectcellArgsSpaltentitelSpec

class LocalizedRoboSAPiensKeywordsSelectcellArgs(TypedDict):
    a1Zeilennummer_oder_Zellinhalt: LocalizedRoboSAPiensKeywordsSelectcellArgsZeilennummer_Oder_Zellinhalt
    a2Spaltentitel: LocalizedRoboSAPiensKeywordsSelectcellArgsSpaltentitel

class LocalizedRoboSAPiensKeywordsSelectcellResult(TypedDict):
    NoSession: Tuple[Literal['2754484086'], str]
    NotFound: Tuple[Literal['2770335633'], str]
    Pass: Tuple[Literal['3085393420'], str]
    Exception: Tuple[Literal['2355177759'], str]

class LocalizedRoboSAPiensKeywordsSelectcell(TypedDict):
    name: Tuple[Literal['1049942265'], str]
    args: LocalizedRoboSAPiensKeywordsSelectcellArgs
    result: LocalizedRoboSAPiensKeywordsSelectcellResult
    doc: Tuple[Literal['497443824'], str]

class LocalizedRoboSAPiensKeywordsSelectcomboboxentryArgsNameSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsSelectcomboboxentryArgsName(TypedDict):
    name: Tuple[Literal['3378336226'], str]
    spec: LocalizedRoboSAPiensKeywordsSelectcomboboxentryArgsNameSpec

class LocalizedRoboSAPiensKeywordsSelectcomboboxentryArgsEintragSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsSelectcomboboxentryArgsEintrag(TypedDict):
    name: Tuple[Literal['723623280'], str]
    spec: LocalizedRoboSAPiensKeywordsSelectcomboboxentryArgsEintragSpec

class LocalizedRoboSAPiensKeywordsSelectcomboboxentryArgs(TypedDict):
    a1Name: LocalizedRoboSAPiensKeywordsSelectcomboboxentryArgsName
    a2Eintrag: LocalizedRoboSAPiensKeywordsSelectcomboboxentryArgsEintrag

class LocalizedRoboSAPiensKeywordsSelectcomboboxentryResult(TypedDict):
    NoSession: Tuple[Literal['2754484086'], str]
    NotFound: Tuple[Literal['3185471891'], str]
    EntryNotFound: Tuple[Literal['1357582115'], str]
    Pass: Tuple[Literal['2235674925'], str]
    Exception: Tuple[Literal['2433413970'], str]

class LocalizedRoboSAPiensKeywordsSelectcomboboxentry(TypedDict):
    name: Tuple[Literal['2133292945'], str]
    args: LocalizedRoboSAPiensKeywordsSelectcomboboxentryArgs
    result: LocalizedRoboSAPiensKeywordsSelectcomboboxentryResult
    doc: Tuple[Literal['92484869'], str]

class LocalizedRoboSAPiensKeywordsSelectradiobuttonArgsBeschriftung_Oder_PositionsgeberSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsSelectradiobuttonArgsBeschriftung_Oder_Positionsgeber(TypedDict):
    name: Tuple[Literal['2051440239'], str]
    spec: LocalizedRoboSAPiensKeywordsSelectradiobuttonArgsBeschriftung_Oder_PositionsgeberSpec

class LocalizedRoboSAPiensKeywordsSelectradiobuttonArgs(TypedDict):
    Beschriftung_oder_Positionsgeber: LocalizedRoboSAPiensKeywordsSelectradiobuttonArgsBeschriftung_Oder_Positionsgeber

class LocalizedRoboSAPiensKeywordsSelectradiobuttonResult(TypedDict):
    NoSession: Tuple[Literal['2754484086'], str]
    NotFound: Tuple[Literal['2755548585'], str]
    Pass: Tuple[Literal['259379063'], str]
    Exception: Tuple[Literal['218028187'], str]

class LocalizedRoboSAPiensKeywordsSelectradiobutton(TypedDict):
    name: Tuple[Literal['2985728785'], str]
    args: LocalizedRoboSAPiensKeywordsSelectradiobuttonArgs
    result: LocalizedRoboSAPiensKeywordsSelectradiobuttonResult
    doc: Tuple[Literal['2939575456'], str]

class LocalizedRoboSAPiensKeywordsSelecttextfieldArgsBeschriftungen_Oder_InhaltSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsSelecttextfieldArgsBeschriftungen_Oder_Inhalt(TypedDict):
    name: Tuple[Literal['2051440239'], str]
    spec: LocalizedRoboSAPiensKeywordsSelecttextfieldArgsBeschriftungen_Oder_InhaltSpec

class LocalizedRoboSAPiensKeywordsSelecttextfieldArgs(TypedDict):
    Beschriftungen_oder_Inhalt: LocalizedRoboSAPiensKeywordsSelecttextfieldArgsBeschriftungen_Oder_Inhalt

class LocalizedRoboSAPiensKeywordsSelecttextfieldResult(TypedDict):
    NoSession: Tuple[Literal['2754484086'], str]
    NotFound: Tuple[Literal['2917845132'], str]
    Pass: Tuple[Literal['3773273557'], str]
    Exception: Tuple[Literal['1228826942'], str]

class LocalizedRoboSAPiensKeywordsSelecttextfield(TypedDict):
    name: Tuple[Literal['335907869'], str]
    args: LocalizedRoboSAPiensKeywordsSelecttextfieldArgs
    result: LocalizedRoboSAPiensKeywordsSelecttextfieldResult
    doc: Tuple[Literal['2308992901'], str]

class LocalizedRoboSAPiensKeywordsSelecttextlineArgsInhaltSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsSelecttextlineArgsInhalt(TypedDict):
    name: Tuple[Literal['4274335913'], str]
    spec: LocalizedRoboSAPiensKeywordsSelecttextlineArgsInhaltSpec

class LocalizedRoboSAPiensKeywordsSelecttextlineArgs(TypedDict):
    Inhalt: LocalizedRoboSAPiensKeywordsSelecttextlineArgsInhalt

class LocalizedRoboSAPiensKeywordsSelecttextlineResult(TypedDict):
    NoSession: Tuple[Literal['2754484086'], str]
    NotFound: Tuple[Literal['1356747844'], str]
    Pass: Tuple[Literal['792202299'], str]
    Exception: Tuple[Literal['528079567'], str]

class LocalizedRoboSAPiensKeywordsSelecttextline(TypedDict):
    name: Tuple[Literal['4264534869'], str]
    args: LocalizedRoboSAPiensKeywordsSelecttextlineArgs
    result: LocalizedRoboSAPiensKeywordsSelecttextlineResult
    doc: Tuple[Literal['4023168358'], str]

class LocalizedRoboSAPiensKeywordsTickcheckboxArgsBeschriftung_Oder_PositionsgeberSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsTickcheckboxArgsBeschriftung_Oder_Positionsgeber(TypedDict):
    name: Tuple[Literal['2051440239'], str]
    spec: LocalizedRoboSAPiensKeywordsTickcheckboxArgsBeschriftung_Oder_PositionsgeberSpec

class LocalizedRoboSAPiensKeywordsTickcheckboxArgs(TypedDict):
    Beschriftung_oder_Positionsgeber: LocalizedRoboSAPiensKeywordsTickcheckboxArgsBeschriftung_Oder_Positionsgeber

class LocalizedRoboSAPiensKeywordsTickcheckboxResult(TypedDict):
    NoSession: Tuple[Literal['2754484086'], str]
    NotFound: Tuple[Literal['3274358834'], str]
    Pass: Tuple[Literal['999358000'], str]
    Exception: Tuple[Literal['1153105219'], str]

class LocalizedRoboSAPiensKeywordsTickcheckbox(TypedDict):
    name: Tuple[Literal['2471720243'], str]
    args: LocalizedRoboSAPiensKeywordsTickcheckboxArgs
    result: LocalizedRoboSAPiensKeywordsTickcheckboxResult
    doc: Tuple[Literal['3559298022'], str]

class LocalizedRoboSAPiensKeywordsUntickcheckboxArgsBeschriftung_Oder_PositionsgeberSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsUntickcheckboxArgsBeschriftung_Oder_Positionsgeber(TypedDict):
    name: Tuple[Literal['2051440239'], str]
    spec: LocalizedRoboSAPiensKeywordsUntickcheckboxArgsBeschriftung_Oder_PositionsgeberSpec

class LocalizedRoboSAPiensKeywordsUntickcheckboxArgs(TypedDict):
    Beschriftung_oder_Positionsgeber: LocalizedRoboSAPiensKeywordsUntickcheckboxArgsBeschriftung_Oder_Positionsgeber

class LocalizedRoboSAPiensKeywordsUntickcheckboxResult(TypedDict):
    NoSession: Tuple[Literal['2754484086'], str]
    NotFound: Tuple[Literal['3274358834'], str]
    Pass: Tuple[Literal['1077869101'], str]
    Exception: Tuple[Literal['1479426504'], str]

class LocalizedRoboSAPiensKeywordsUntickcheckbox(TypedDict):
    name: Tuple[Literal['47381427'], str]
    args: LocalizedRoboSAPiensKeywordsUntickcheckboxArgs
    result: LocalizedRoboSAPiensKeywordsUntickcheckboxResult
    doc: Tuple[Literal['2834166382'], str]

class LocalizedRoboSAPiensKeywordsTickcheckboxcellArgsZeilennummerSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsTickcheckboxcellArgsZeilennummer(TypedDict):
    name: Tuple[Literal['3333868678'], str]
    spec: LocalizedRoboSAPiensKeywordsTickcheckboxcellArgsZeilennummerSpec

class LocalizedRoboSAPiensKeywordsTickcheckboxcellArgsSpaltentitelSpec(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsTickcheckboxcellArgsSpaltentitel(TypedDict):
    name: Tuple[Literal['2102626174'], str]
    spec: LocalizedRoboSAPiensKeywordsTickcheckboxcellArgsSpaltentitelSpec

class LocalizedRoboSAPiensKeywordsTickcheckboxcellArgs(TypedDict):
    a1Zeilennummer: LocalizedRoboSAPiensKeywordsTickcheckboxcellArgsZeilennummer
    a2Spaltentitel: LocalizedRoboSAPiensKeywordsTickcheckboxcellArgsSpaltentitel

class LocalizedRoboSAPiensKeywordsTickcheckboxcellResult(TypedDict):
    NoSession: Tuple[Literal['2754484086'], str]
    NotFound: Tuple[Literal['2481184945'], str]
    Pass: Tuple[Literal['1580249093'], str]
    Exception: Tuple[Literal['870126097'], str]

class LocalizedRoboSAPiensKeywordsTickcheckboxcell(TypedDict):
    name: Tuple[Literal['3286561809'], str]
    args: LocalizedRoboSAPiensKeywordsTickcheckboxcellArgs
    result: LocalizedRoboSAPiensKeywordsTickcheckboxcellResult
    doc: Tuple[Literal['3059587832'], str]

class LocalizedRoboSAPiensKeywordsGetwindowtitleArgs(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsGetwindowtitleResult(TypedDict):
    NoSession: Tuple[Literal['2754484086'], str]
    Pass: Tuple[Literal['2852411998'], str]

class LocalizedRoboSAPiensKeywordsGetwindowtitle(TypedDict):
    name: Tuple[Literal['2828980154'], str]
    args: LocalizedRoboSAPiensKeywordsGetwindowtitleArgs
    result: LocalizedRoboSAPiensKeywordsGetwindowtitleResult
    doc: Tuple[Literal['1638398427'], str]

class LocalizedRoboSAPiensKeywordsGetwindowtextArgs(TypedDict):
    ...

class LocalizedRoboSAPiensKeywordsGetwindowtextResult(TypedDict):
    NoSession: Tuple[Literal['2754484086'], str]
    Pass: Tuple[Literal['2562559050'], str]

class LocalizedRoboSAPiensKeywordsGetwindowtext(TypedDict):
    name: Tuple[Literal['1085911504'], str]
    args: LocalizedRoboSAPiensKeywordsGetwindowtextArgs
    result: LocalizedRoboSAPiensKeywordsGetwindowtextResult
    doc: Tuple[Literal['2803599762'], str]

class LocalizedRoboSAPiensKeywords(TypedDict):
    ActivateTab: LocalizedRoboSAPiensKeywordsActivatetab
    OpenSAP: LocalizedRoboSAPiensKeywordsOpensap
    CloseConnection: LocalizedRoboSAPiensKeywordsCloseconnection
    CloseSAP: LocalizedRoboSAPiensKeywordsClosesap
    ExportTree: LocalizedRoboSAPiensKeywordsExporttree
    AttachToRunningSAP: LocalizedRoboSAPiensKeywordsAttachtorunningsap
    ConnectToServer: LocalizedRoboSAPiensKeywordsConnecttoserver
    DoubleClickCell: LocalizedRoboSAPiensKeywordsDoubleclickcell
    DoubleClickTextField: LocalizedRoboSAPiensKeywordsDoubleclicktextfield
    ExecuteTransaction: LocalizedRoboSAPiensKeywordsExecutetransaction
    ExportForm: LocalizedRoboSAPiensKeywordsExportform
    FillTableCell: LocalizedRoboSAPiensKeywordsFilltablecell
    FillTextField: LocalizedRoboSAPiensKeywordsFilltextfield
    PushButton: LocalizedRoboSAPiensKeywordsPushbutton
    PushButtonCell: LocalizedRoboSAPiensKeywordsPushbuttoncell
    ReadTextField: LocalizedRoboSAPiensKeywordsReadtextfield
    ReadText: LocalizedRoboSAPiensKeywordsReadtext
    ReadTableCell: LocalizedRoboSAPiensKeywordsReadtablecell
    SaveScreenshot: LocalizedRoboSAPiensKeywordsSavescreenshot
    SelectCell: LocalizedRoboSAPiensKeywordsSelectcell
    SelectComboBoxEntry: LocalizedRoboSAPiensKeywordsSelectcomboboxentry
    SelectRadioButton: LocalizedRoboSAPiensKeywordsSelectradiobutton
    SelectTextField: LocalizedRoboSAPiensKeywordsSelecttextfield
    SelectTextLine: LocalizedRoboSAPiensKeywordsSelecttextline
    TickCheckBox: LocalizedRoboSAPiensKeywordsTickcheckbox
    UntickCheckBox: LocalizedRoboSAPiensKeywordsUntickcheckbox
    TickCheckBoxCell: LocalizedRoboSAPiensKeywordsTickcheckboxcell
    GetWindowTitle: LocalizedRoboSAPiensKeywordsGetwindowtitle
    GetWindowText: LocalizedRoboSAPiensKeywordsGetwindowtext

class LocalizedRoboSAPiensSpecs(TypedDict):
    ...

class LocalizedRoboSAPiens(TypedDict):
    doc: LocalizedRoboSAPiensDoc
    args: LocalizedRoboSAPiensArgs
    keywords: LocalizedRoboSAPiensKeywords
    specs: LocalizedRoboSAPiensSpecs