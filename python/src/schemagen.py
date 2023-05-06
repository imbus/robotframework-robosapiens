import codegen
import json
import sys
from importlib import import_module
from typing import Any, Callable, Dict, List, Tuple, Union
from zlib import crc32
from changeset import changeset

StrDict = Dict[str, Union[str, "StrDict"]]
Fields = Dict[str, str]
Func = Callable[[str, str], str]


def process_tree(
    type_name: str,
    node: StrDict,
    path: List[str],
    todo: List[Tuple[List[str], str, StrDict]],
    done: List[Tuple[str, Fields]],
    gen_str_type: Func,
):
    fields: Fields = {}

    for idx, (name, val) in enumerate(node.items(), start=1):
        name = codegen.remove_arg_prefix(name).replace("-", "_")
        field_name = str(name)

        if type_name.endswith("Args") and len(node.items()) > 1:
            # Include the index of the argument in the name
            # in order to maintain the order of the arguments
            # when using autocompletion to fill a dictionary
            # conforming with this type
            field_name = f"a{idx}" + name

        if type(val) is dict:
            field_type = type_name + name.title()
            fields[field_name] = field_type
            todo.append((path + [name], field_type, val))
        elif type(val) == str:
            if type_name.endswith("Specs"):
                fields[field_name] = f"Literal[r'{val}']"
            else:
                fields[field_name] = gen_str_type(".".join(path + [name]), val)
        else:
            fields[field_name] = f"Literal[{str(val)}]"

    done.append((type_name, fields))

    return todo, done


def gen_types(type_name: str, node: StrDict, gen_str_type: Func) -> str:
    todo: List[Tuple[List[str], str, StrDict]] = []
    done: List[Tuple[str, Fields]] = []

    # breadth-first tree traversal
    path = []
    todo, done = process_tree(type_name, node, path, todo, done, gen_str_type)

    while len(todo) > 0:
        todo_first, *todo_rest = todo
        path, name, val = todo_first
        todo, done = process_tree(name, val, path, todo_rest, done, gen_str_type)

    return codegen.pprint_sections(
        "\n".join(codegen.gen_typed_dict(name, entries)) for name, entries in reversed(done)
    )


def generate_api_code(
    api: Any,
    imports: Dict[str, List[str]] = {"typing_extensions": ["Literal", "TypedDict"]},
    gen_str_type: Func = lambda path, name: "str",
    type_name: str = "RoboSAPiens"
):
    return codegen.pprint_sections([
        codegen.gen_imports(imports),
        gen_types(type_name, api, gen_str_type),
    ])


def generate_localized_schema(api: Any):
    imports = { 
        "typing_extensions": ["Literal", "TypedDict"],
        "typing": ["Tuple"] 
    }

    def gen_str_type(path, value):
        if path in changeset:
            return f"Tuple[Literal['{crc32(value.encode('utf-8'))}'], str]"

        return "Tuple[str, str]"
    
    typename = "LocalizedRoboSAPiens"
    
    return generate_api_code(api, imports, gen_str_type, typename)


def write_schema_py(api_json: str):
    with open(api_json, "r", encoding="utf-8") as file:
        api = json.load(file)

    schema_py = generate_api_code(api)

    with open("schema.py", "w", encoding="utf-8") as file:
        file.write(schema_py)


if __name__ == "__main__":
    _, *args = sys.argv

    if len(args) < 1:
        print("Usage: python genschema.py api.json [i18n]")
        sys.exit(1)

    if len(args) == 1:
        api_json = args[0]

        write_schema_py(api_json)

        sys.exit(0)

    if len(args) == 2 and args[1] == "i18n":
        api_json = args[0]

        write_schema_py(api_json)

        schema_en = import_module("localized.en", ".").lib
        schema_py = generate_localized_schema(schema_en)

        with open("schema_i18n.py", "w", encoding="utf-8") as file:
            file.write(schema_py)
        
        sys.exit(0)
