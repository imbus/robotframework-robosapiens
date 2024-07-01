from pathlib import Path
import re
import sys


def open_link_in_new_window(line: str):
    if line.startswith("libdoc"):
        return re.sub(r'<a (href=\\"http[^>]*)>', r'<a \1 target=\"_blank\">', line)
    return line


if __name__ == "__main__":
    _, *args = sys.argv

    if len(args) == 1:
        html_file = Path(args[0])

        with open(html_file, "r", encoding="utf-8") as file:
            html = "".join([
                open_link_in_new_window(line)
                for line in file
            ])

        with open(html_file, "w", encoding="utf-8") as f:
            f.write(html)
    else:
        print("Usage: python replace_links.py file.html")
        sys.exit(1)
