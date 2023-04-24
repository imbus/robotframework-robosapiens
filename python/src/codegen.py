from typing import Any, Dict, Iterable, List
import json
import re


def pprint_code_block(header: List[str], body: Iterable[str]):
    return header + pprint_indented(body)


def pprint_indented(code_block: Iterable[str]):
    indent = "    "

    return [indent + line for line in code_block]


def pprint_sections(sections: Iterable[str]):
    return "\n\n".join(sections)


def gen_class(name: str, properties: List[str], doc: List[str], methods: List[str], superclass: str):
    if superclass:
        header = f"class {name}({superclass}):"
    else:
        header = f"class {name}:"

    return pprint_code_block(
        [header], 
        doc + 
        [""] +
        methods +
        [""] +
        properties
    )


def gen_init(args: str, doc: List[str], body: List[str]):
    return pprint_code_block(
        [f"def __init__(self, {args}):"],
        doc +
        body
    )


def gen_any_dict(name: str, input: Dict[str, str]):
    return [f"{name} = " + "{"] + [
        f"    '{key}': {val},"
        for key, val in input.items()
    ] + ["}"]


def gen_str_dict(name: str, input: Any):
    return (f"{name} = " + json.dumps(
        input, 
        indent=4, 
        ensure_ascii=False)).split("\n")


def gen_doc(doc: str):
    doc_lines = (line.lstrip() for line in doc.split("\n"))

    while (first := next(doc_lines)) == "":
        ...

    return ['"""'] + [first] + list(doc_lines) + ['"""']


def gen_typed_dict(name: str, dict_entries: Dict[str, str]):
    dict_entries_lines = [f"{key}: {value}" for key, value in dict_entries.items()]

    return pprint_code_block(
        [f"class {name}(TypedDict):"], 
        ["..."] if not dict_entries_lines else dict_entries_lines
    )


def gen_imports(imports: Dict[str, List[str]]):
    return "\n".join([
        f"from {module} import " + ", ".join(imports)
        for module, imports in imports.items()
    ])


def remove_arg_prefix(name: str):
    return re.sub(r"^a\d", "", name)
