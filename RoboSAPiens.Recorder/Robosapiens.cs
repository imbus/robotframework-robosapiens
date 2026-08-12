namespace RoboSAPiens.Recorder
{
    record Robosapiens(string language)
    {
        string lang = language.ToUpper();

        public KeywordCall? callKeyword(string returnValue, string keyword, params string[] args)
        {
            return keyword.ToLower() switch
            {
                "get window text" or "fenstertext auslesen" => GetWindowText(returnValue),
                "get window title" or "fenstertitel auslesen" => GetWindowTitle(returnValue),
                "read cell" or "tabellenzelle auslesen" => args switch
                {
                    [string row_locator, string column] 
                        => ReadCell(new Locator(row: row_locator, col: column), returnValue),
                    [string row_locator, string column, string tableNumber] when int.TryParse(tableNumber, out int tableNumber_) 
                        => ReadCell(new Locator(row: row_locator, col: column), returnValue),
                    _ => null
                },
                "read statusbar" or "statusleiste auslesen" => ReadStatusbar(returnValue),
                "read text" or "text auslesen" => args switch
                {
                    [string hLabel] => ReadText(new Locator(hLabel), returnValue),
                    _ => null
                },
                "read text field" or "textfeld auslesen" => args switch
                {
                    [string hLabel] => ReadTextField(new Locator(hLabel), returnValue),
                    _ => null
                },
                "read tree element" or "baumelement auslesen" => args switch
                {
                    [string elementPath] 
                        => ReadTreeElement(new Locator(hLabel: elementPath), returnValue),
                    [string elementPath, string tooltip] when bool.TryParse(tooltip, out bool tooltip_) 
                        => ReadTreeElement(new Locator(hLabel: elementPath), returnValue, tooltip: tooltip_),
                    [string elementPath, string tooltip, string icon] when bool.TryParse(tooltip, out bool tooltip_) && bool.TryParse(icon, out bool icon_) 
                        => ReadTreeElement(new Locator(hLabel: elementPath), returnValue, tooltip: tooltip_, icon: icon_),
                    _ => null
                },
                _ => new KeywordCall(keyword, args: [.. args.Select(arg => new KeywordCallArg(name: "name", value: arg, type: KeywordCallArgType.ARG))], returnValue: returnValue)
            };
        }

        public KeywordCall libraryImport(bool x64=false)
        {
            string library = lang switch
            {
                "DE" => "RoboSAPiens.DE",
                _ => "RoboSAPiens",
            };

            List<KeywordCallArg> args = [
                new ("x64", x64.ToString().Capitalize(), KeywordCallArgType.KWARG)
            ];

            return new KeywordCall(library, args);
        }

        public KeywordCall CloseSap()
        {
            string name = lang switch
            {
                "DE" => "SAP beenden",
                _ => "Close SAP"
            };
           
            return new KeywordCall(name, args: []);
        }

        public KeywordCall ConnectToSap()
        {
            string name = lang switch
            {
                "DE" => "Laufende SAP GUI übernehmen",
                _ => "Connect to running SAP"
            };

            return new KeywordCall(name, args: []);
        }

        public KeywordCall ConnectToServer(string server)
        {
            string name = lang switch
            {
                "DE" => "Verbindung zum Server herstellen",
                _ => "Connect to Server"
            };
            List<KeywordCallArg> args =
            [
                new KeywordCallArg(
                    name: lang switch
                    {
                        "DE" => "Servername",
                        _ => "server_name"
                    },
                    value: server,
                    type: "ARG"
                )
            ];
            return new KeywordCall(name, args);
        }

        public KeywordCall DoubleClickCell(Locator locator)
        {
            string name = lang switch
            {
                "DE" => "Tabellenzelle doppelklicken",
                _ => "Double-click Cell"
            };
            List<KeywordCallArg> args = [
                locator.toKeywordCallArg(lang)
            ];
            return new KeywordCall(name, args);
        }

        public KeywordCall DoubleClickTextField(Locator locator)
        {
            string name = lang switch
            {
                "DE" => "Textfeld doppelklicken",
                _ => "Double-click Text Field"
            };
            List<KeywordCallArg> args = [
                locator.toKeywordCallArg(lang)
            ];
            return new KeywordCall(name, args);
        }

        public KeywordCall DoubleClickTreeElement(Locator locator)
        {
            string name = lang switch
            {
                "DE" => "Baumelement doppelklicken",
                _ => "Double-click Tree Element"
            };
            List<KeywordCallArg> args = [
                locator.toKeywordCallArg(lang)
            ];
            return new KeywordCall(name, args);
        }

        public KeywordCall ExecuteTransaction(string tCode)
        {
            string name = lang switch
            {
                "DE" => "Transaktion ausführen",
                _ => "Execute Transaction"
            };
            List<KeywordCallArg> args =
            [
                new KeywordCallArg(
                    name: lang switch
                    {
                        "DE" => "Transaktion",
                        _ => "transaction"
                    },
                    value: tCode,
                    type: "ARG"
                )
            ];
            return new KeywordCall(name, args);
        }

        public KeywordCall ExpandTreeFolder(Locator locator)
        {
            string name = lang switch
            {
                "DE" => "Baumelement aufklappen",
                _ => "Expand Tree Folder"
            };
            List<KeywordCallArg> args = [
                locator.toKeywordCallArg(lang)
            ];
            return new KeywordCall(name, args);
        }

        public KeywordCall FillCell(Locator locator, string contents)
        {
            string name = lang switch
            {
                "DE" => "Tabellenzelle ausfüllen",
                _ => "Fill Cell"
            };
            List<KeywordCallArg> args =
            [
                locator.toKeywordCallArg(lang),
                new KeywordCallArg(
                    name: lang switch
                    {
                        "DE" => "Inhalt",
                        _ => "contents"
                    },
                    value: contents,
                    type: "ARG"
                )
            ];
            return new KeywordCall(name, args);
        }

        public KeywordCall FillTextField(Locator locator, string contents)
        {
            string name = lang switch
            {
                "DE" => "Textfeld ausfüllen",
                _ => "Fill Text Field"
            };
            List<KeywordCallArg> args =
            [
                locator.toKeywordCallArg(lang),
                new KeywordCallArg(
                    name: lang switch
                    {
                        "DE" => "Inhalt",
                        _ => "contents"
                    },
                    value: contents,
                    type: "ARG"
                )
            ];
            return new KeywordCall(name, args);
        }

        public KeywordCall GetWindowText(string returnValue)
        {
            string name = lang switch
            {
                "DE" => "Fenstertext auslesen",
                _ => "Get Window Text"
            };
            return new KeywordCall(name, args: [], returnValue: returnValue);
        }

        public KeywordCall GetWindowTitle(string returnValue)
        {
            string name = lang switch
            {
                "DE" => "Fenstertitel auslesen",
                _ => "Get Window Title"
            };
            return new KeywordCall(name, args: [], returnValue: returnValue);
        }

        public KeywordCall OpenSap(string path)
        {
            string name = lang switch
            {
                "DE" => "SAP starten",
                _ => "Open SAP"
            };
            List<KeywordCallArg> args =
            [
                new KeywordCallArg(
                    name: lang switch
                    {
                        "DE" => "Pfad",
                        _ => "path"
                    },
                    value: path.Replace("\\", "\\\\"),
                    type: "ARG"
                )
            ];
            return new KeywordCall(name, args);
        }

        public KeywordCall PressKeyCombination(string keyCombination)
        {
            string name = lang switch
            {
                "DE" => "Tastenkombination drücken",
                _ => "Press Key Combination"
            };
            List<KeywordCallArg> args =
            [
                new KeywordCallArg(
                    name: lang switch
                    {
                        "DE" => "Tastenkombination",
                        _ => "key_combination"
                    },
                    value: keyCombination,
                    type: "ARG"
                )
            ];
            return new KeywordCall(name, args);
        }

        public KeywordCall PushButton(Locator locator)
        {
            string name = lang switch
            {
                "DE" => "Knopf drücken",
                _ => "Push Button"
            };
            List<KeywordCallArg> args =
            [
                locator.toKeywordCallArg(lang),
                new KeywordCallArg(
                    name: lang switch
                    {
                        "DE" => "exakt",
                        _ => "exact"
                    },
                    value: "True",
                    type: "KWARG"
                )
            ];
            return new KeywordCall(name, args);
        }

        public KeywordCall PushButtonCell(Locator locator)
        {
            string name = lang switch
            {
                "DE" => "Tabellenzelle drücken",
                _ => "Push Button Cell"
            };
            List<KeywordCallArg> args = [
                locator.toKeywordCallArg(lang)
            ];
            return new KeywordCall(name, args);
        }

        public KeywordCall ReadCell(Locator locator, string returnValue)
        {
            string name = lang switch
            {
                "DE" => "Tabellenzelle auslesen",
                _ => "Read Cell"
            };
            List<KeywordCallArg> args = [
                locator.toKeywordCallArg(lang)
            ];
            return new KeywordCall(name, args, returnValue: returnValue);
        }

        public KeywordCall ReadStatusbar(string returnValue)
        {
            string name = lang switch
            {
                "DE" => "Statusleiste auslesen",
                _ => "Read Statusbar"
            };
            return new KeywordCall(name, args: [], returnValue: returnValue);
        }

        public KeywordCall ReadText(Locator locator, string returnValue)
        {
            string name = lang switch
            {
                "DE" => "Text auslesen",
                _ => "Read Text"
            };
            List<KeywordCallArg> args = [
                locator.toKeywordCallArg(lang)
            ];
            return new KeywordCall(name, args, returnValue: returnValue);
        }

        public KeywordCall ReadTextField(Locator locator, string returnValue)
        {
            string name = lang switch
            {
                "DE" => "Textfeld auslesen",
                _ => "Read Text Field"
            };
            List<KeywordCallArg> args = [
                locator.toKeywordCallArg(lang)
            ];
            return new KeywordCall(name, args, returnValue: returnValue);
        }

        public KeywordCall ReadTreeElement(Locator locator, string returnValue, bool tooltip=false, bool icon=false)
        {
            string name = lang switch
            {
                "DE" => "Baumelement auslesen",
                _ => "Read Tree Element"
            };
            List<KeywordCallArg> args =
            [
                locator.toKeywordCallArg(lang),
                new KeywordCallArg(
                    name: lang switch
                    {
                        "DE" => "Tooltip",
                        _ => "tooltip"
                    },
                    value: tooltip.ToString().Capitalize(),
                    type: "KWARG"
                ),
                new KeywordCallArg(
                    name: lang switch
                    {
                        "DE" => "Icon",
                        _ => "icon"
                    },
                    value: icon.ToString().Capitalize(),
                    type: "KWARG"
                )
            ];
            return new KeywordCall(name, args, returnValue: returnValue);
        }

        public KeywordCall SelectCell(Locator locator)
        {
            string name = lang switch
            {
                "DE" => "Tabellenzelle markieren",
                _ => "Select Cell"
            };
            List<KeywordCallArg> args = [
                locator.toKeywordCallArg(lang)
            ];
            return new KeywordCall(name, args);
        }

        public KeywordCall SelectCellValue(Locator locator, string value)
        {
            string name = lang switch
            {
                "DE" => "Tabellenzellenwert auswählen",
                _ => "Select Cell Value"
            };
            List<KeywordCallArg> args =
            [
                locator.toKeywordCallArg(lang),
                new KeywordCallArg(
                    name: lang switch
                    {
                        "DE" => "Wert",
                        _ => "value"
                    },
                    value: value,
                    type: "ARG"
                )
            ];
            return new KeywordCall(name, args);
        }

        public KeywordCall SelectComboBox(Locator locator, string value)
        {
            string name = lang switch
            {
                "DE" => "Auswahlmenüeintrag auswählen",
                _ => "Select Dropdown Menu Entry"
            };
            List<KeywordCallArg> args =
            [
                locator.toKeywordCallArg(lang),
                new KeywordCallArg(
                    name: lang switch
                    {
                        "DE" => "Wert",
                        _ => "value"
                    },
                    value: value,
                    type: "ARG"
                )
            ];
            return new KeywordCall(name, args);
        }

        public KeywordCall SelectRadio(Locator locator)
        {
            string name = lang switch
            {
                "DE" => "Optionsfeld auswählen",
                _ => "Select Radio Button"
            };
            List<KeywordCallArg> args = [
                locator.toKeywordCallArg(lang)
            ];
            return new KeywordCall(name, args);
        }

        public KeywordCall SelectTab(Locator locator)
        {
            string name = lang switch
            {
                "DE" => "Reiter auswählen",
                _ => "Select Tab"
            };
            List<KeywordCallArg> args = [
                locator.toKeywordCallArg(lang)
            ];
            return new KeywordCall(name, args);
        }

        public KeywordCall SelectText(Locator locator)
        {
            string name = lang switch
            {
                "DE" => "Text markieren",
                _ => "Select Text"
            };
            List<KeywordCallArg> args = [
                locator.toKeywordCallArg(lang)
            ];
            return new KeywordCall(name, args);
        }

        public KeywordCall SelectTextField(Locator locator)
        {
            string name = lang switch
            {
                "DE" => "Textfeld markieren",
                _ => "Select Text Field"
            };
            List<KeywordCallArg> args = [
                locator.toKeywordCallArg(lang)
            ];
            return new KeywordCall(name, args);
        }

        public KeywordCall TickCheckBox(Locator locator)
        {
            string name = lang switch
            {
                "DE" => "Formularfeld ankreuzen",
                _ => "Tick Checkbox"
            };
            List<KeywordCallArg> args = [
                locator.toKeywordCallArg(lang)
            ];
            return new KeywordCall(name, args);
        }

        public KeywordCall TickCheckBoxCell(Locator locator)
        {
            string name = lang switch
            {
                "DE" => "Tabellenzelle ankreuzen",
                _ => "Tick Checkbox Cell"
            };
            List<KeywordCallArg> args = [
                locator.toKeywordCallArg(lang)
            ];
            return new KeywordCall(name, args);
        }

        public KeywordCall UntickCheckBox(Locator locator)
        {
            string name = lang switch
            {
                "DE" => "Formularfeld abwählen",
                _ => "Untick Checkbox"
            };
            List<KeywordCallArg> args = [
                locator.toKeywordCallArg(lang)
            ];
            return new KeywordCall(name, args);
        }

        public KeywordCall UntickCheckBoxCell(Locator locator)
        {
            string name = lang switch
            {
                "DE" => "Tabellenzelle abwählen",
                _ => "Untick Checkbox Cell"
            };
            List<KeywordCallArg> args = [
                locator.toKeywordCallArg(lang)
            ];
            return new KeywordCall(name, args);
        }
    }
}