import importlib
import json
import sys
from pathlib import Path
from libgen import get_str, rec_map_values


localized = Path("localized")

if __name__ == "__main__":
    _, *args = sys.argv

    if len(args) == 0:
        print("Usage: python libspecgen.py lang")
        sys.exit(0)

    lang = args[0]
    lib = importlib.import_module(f"{localized}.{lang}", ".").lib
    libspec = {
        "name": "RoboSAPiens" + (lang != "en")*f".{lang.upper()}",
        "keywords": sorted([
            {
                "id": kw_name,
                "name": get_str(kw["name"]),
                "args": [
                    dict({"id": arg_name, **rec_map_values(arg, lambda k, v: get_str(v))})
                    for arg_name, arg in kw["args"].items()
                ]
            }
            for kw_name, kw in lib["keywords"].items()
        ], key=lambda keyword: keyword["name"])
    }

    with open(Path(f"{lang}.json"), "w", encoding="utf-8") as file:
        file.write(json.dumps(libspec, ensure_ascii=False, indent=4))
