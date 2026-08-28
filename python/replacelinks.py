import re
import sys
from functools import reduce
from pathlib import Path


replacements = {
    r'<a (href=\\"http[^>]*)>': r'<a \1 target=\"_blank\">',
    '"name": "RoboSAPiens"': '"name": "robosapiens"',
    r'"name": "RoboSAPiens\.DE"': '"name": "robosapiens.de"',
}


def replace(replacements: dict, line: str) -> str:
    if line.startswith("libdoc"):
        return reduce(
            lambda acc, pattern: re.sub(pattern, replacements[pattern], acc), 
            replacements, 
            line
        )

    return line


if __name__ == "__main__":
    _, *args = sys.argv

    if len(args) == 1:
        html_file = Path(args[0])

        with open(html_file, "r", encoding="utf-8") as file:
            html = "".join([
                replace(replacements, line)
                for line in file
            ])

        with open(html_file, "w", encoding="utf-8") as f:
            f.write(html)
    else:
        print("Usage: python replace_links.py file.html")
        sys.exit(1)
