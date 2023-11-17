import importlib
import json
import sys
from pathlib import Path
from libgen import get_str, rec_map_values
from version import __version__

localized = Path("localized")

if __name__ == "__main__":
    _, *args = sys.argv

    if len(args) == 0:
        print("Usage: python libspecgen.py lang")
        sys.exit(0)

    lang = args[0]
    lib = importlib.import_module(f"{localized}.{lang}", ".").lib
    keywords = sorted(
        [
            {
                "id": kw_name,
                "name": get_str(kw["name"]),
                "args": [
                    {
                        "name": arg_name,
                        "desc": get_str(arg["name"]).replace("_", " "),
                        "spec": {
                            spec_name: get_str(spec)
                            for spec_name, spec in arg["spec"].items()
                        },
                        "optional": arg["optional"]
                    }
                    for arg_name, arg in kw["args"].items()
                ],
                "result": {
                    result_name: get_str(result)
                    for result_name, result in kw["result"].items()
                },
                "returnValue": kw_name.startswith("Get") or kw_name.startswith("Read")
            }
            for kw_name, kw in lib["keywords"].items()
        ],
        key=lambda keyword: keyword["name"]
    )

    libspec = {
        "library": "RoboSAPiens" + (lang != "en")*f".{lang.upper()}",
        "version": __version__,
        "doc": "https://imbus.github.io/robotframework-robosapiens/",
        "keywords": {
            keyword["id"]: keyword
            for keyword in keywords
        }
    }

    with open(Path(f"{lang}.json"), "w", encoding="utf-8") as file:
        file.write(json.dumps(libspec, ensure_ascii=False, indent=4))
