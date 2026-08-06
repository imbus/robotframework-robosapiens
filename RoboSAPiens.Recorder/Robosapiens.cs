namespace RoboSAPiens.Recorder
{
    record Robosapiens(string language)
    {
        string lang = language.ToUpper();

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