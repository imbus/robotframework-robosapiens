import codegen
import importlib
import sys
from pathlib import Path
from typing import Any, Callable, Dict, List, Tuple, Union, cast
from version import __version__


ArgsDict = Dict[str, Dict[str, Any]]
StrDict = Dict[str, Union[str, "StrDict"]]


def get_str(arg: Union[str, Tuple[str, str]]):
    if type(arg) != str:
        _, name = cast(Tuple[str, str], arg)
        return name
    else:
        return arg


def gen_call_args(args: ArgsDict):
    args_list: List[str] = []

    for arg in args:
        name = get_str(args[arg]['name'])

        if "default" in args[arg]:
            default = args[arg]["default"]
            arg_type = type(default).__name__
            args_list.append(f"{name}: {arg_type}={default}")
        else:
            args_list.append(f"{name}: str")

    return ", ".join(args_list)


def gen_args_doc(args: Dict[str, Dict[str, Union[str, Tuple[str,str]]]]):
    return "\n\n".join([
        f"*{get_str(args[arg]['name'])}*" + ": " + get_str(args[arg]["doc"])
        for arg in args
    ])


def rec_map_values_in_place(d: StrDict, f: Callable[[str, str], str]):
    for k, v in d.items():
        if isinstance(v, dict):
            rec_map_values_in_place(v, f)
        else:
            d[k] = f(k,v)


def gen_result(result: StrDict):
    rec_map_values_in_place(result, lambda k, v: get_str(v))

    return codegen.gen_str_dict("result", result)


def gen_args(args: ArgsDict):
    args_dict = {
        codegen.remove_arg_prefix(arg): get_str(args[arg]['name'])
        for arg in args
    }

    return codegen.gen_any_dict("args", args_dict)


def gen_methods(keywords: Dict[str, Dict[str, Any]]):
    methods: List[str] = []

    for keyword in keywords:
        args = keywords[keyword]['args']
        result = keywords[keyword]['result']
        methods += ["\n"] + codegen.pprint_code_block(
            [f"@keyword('{get_str(keywords[keyword]['name'])}') # type: ignore",
            f"def {codegen.camel_to_snake(keyword)}(self, {gen_call_args(args)}): # type: ignore"],
            codegen.gen_doc(get_str(keywords[keyword]['doc'])) +
            [""] +
            # TODO: validate that the arguments satisfy their spec
            gen_args(args) +
            [""] +
            gen_result(result) +
            [
            f"return super()._run_keyword('{keyword}', list(args.values()), dict(), result) # type: ignore"
            ]
        )

    return methods


def generate_rf_lib(lib_name: str, lib: Any):
    base_class = "RoboSAPiensClient"
    imports = { 
        "robot.api.deco": ["keyword"],
        "RoboSAPiens.client": [base_class]
    }
    properties = [
        "ROBOT_LIBRARY_SCOPE = 'GLOBAL'",
        f"ROBOT_LIBRARY_VERSION = '{__version__}'"
    ]
    doc = codegen.gen_doc(get_str(lib["doc"]["intro"]))
    init = codegen.gen_init(gen_call_args(lib["args"]), 
        codegen.gen_doc(get_str(lib["doc"]["init"]) + "\n\n" + gen_args_doc(lib["args"])),
        [""] + 
        gen_args(lib["args"]) +
        [""] + 
        ["super().__init__(args)"]
    )
    methods = init + gen_methods(lib['keywords'])

    return codegen.pprint_sections([
        codegen.gen_imports(imports),
        "\n".join(codegen.gen_class(lib_name, properties, doc, methods, base_class))
    ])


if __name__ == "__main__":
    _, *args = sys.argv

    if not args:
        print(f"Usage: python libgen.py language")
        sys.exit(1)

    lang = args[0]
    lib = importlib.import_module("localized." + lang, ".").lib

    if lang == "en":
        filepath = Path("RoboSAPiens")
        lib_name = "RoboSAPiens"
    else:
        filepath = Path("RoboSAPiens") / Path(lang.upper())
        lib_name = lang.upper()

    rf_lib = generate_rf_lib(lib_name, lib)
    
    with open(filepath/"__init__.py", "w", encoding="utf-8") as file:
        file.write(rf_lib)

    sys.exit(0)
