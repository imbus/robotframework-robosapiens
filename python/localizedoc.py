import sys
from pathlib import Path
from typing import Dict
from typing_extensions import TypedDict


class Headings(TypedDict):
    Introduction: str
    Importing: str
    Keywords: str
    Arguments: str
    Documentation: str


translations: Dict[str, Headings] = {
    'DE': {
        'Introduction': 'Einleitung',
        'Importing': 'Importieren',
        'Keywords': 'Schlüsselwörter',
        'Arguments': 'Parameter',
        'Documentation': 'Dokumentation'
    }
}


def translate(headings: Headings, line: str):
    def in_brackets(text: str):
        return f">{text}<"

    for heading in headings:
        if in_brackets(heading) in line:
            return line.replace(in_brackets(heading), in_brackets(headings[heading]))

    return line


if __name__ == "__main__":
    _, *args = sys.argv

    if len(args) == 1:
        html_file = Path(args[0])

        RoboSAPiens, lang, html = html_file.name.split(".")
        headings = translations[lang]

        with open(html_file, "r", encoding="utf-8") as file:
            translated = "".join([translate(headings, line) for line in file])

        with open(html_file, "w", encoding="utf-8") as f:
            f.write(translated)
