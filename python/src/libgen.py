import importlib
import os
import sys
from dataclasses import dataclass
from pathlib import Path
from typing import Any, Callable, Dict, List, Tuple, Union, cast
import codegen
from version import __version__


ArgsDict = Dict[str, Dict[str, Any]]
StrDict = Dict[str, Union[str, "StrDict"]]

@dataclass
class LocalizedLib:
    name: str
    path: Path
    spec: Dict


def get_value(arg: Union[Any, Tuple[str, str]]):
    if not isinstance(arg, tuple):
        return arg

    _, value = cast(Tuple[str, str], arg)
    return value


def gen_call_args(args: ArgsDict):
    args_list: List[str] = []

    for arg in args:
        name = get_value(args[arg]['name'])

        if "default" in args[arg]:
            default = get_value(args[arg]["default"])
            if default is not None:
                arg_type = type(default).__name__
                if arg_type == "str":
                    args_list.append(f"{name}: {arg_type}='{default}'")
                else:
                    args_list.append(f"{name}: {arg_type}={default}")
            else:
                args_list.append(f"{name}: str=None")
        else:
            args_list.append(f"{name}: str")

    return args_list


def gen_args_doc(args: Dict[str, Dict[str, Union[str, Tuple[str,str]]]]):
    return "\n".join([
        f'| ``{get_value(args[arg]["name"])}`` | {get_value(args[arg]["desc"])} |'
        for arg in args
    ])


def rec_map_values(d: StrDict, f: Callable[[str, str], str]):
    return {
        k: f(k, v) if not isinstance(v, dict) else rec_map_values(v, f)
        for k, v in d.items()
    }


def gen_result(result: StrDict):
    return codegen.gen_str_dict("result", rec_map_values(result, lambda k, v: get_value(v)))


def gen_args(args: ArgsDict):
    args_dict = {
        codegen.remove_arg_prefix(arg): get_value(args[arg]['name'])
        for arg in args
    }

    return codegen.gen_any_dict("args", args_dict)


def gen_methods(keywords: Dict[str, Dict[str, Any]]):
    methods: List[str] = []

    for keyword in keywords:
        args = keywords[keyword]['args']
        result = keywords[keyword]['result']
        methods += ["\n"] + codegen.pprint_code_block(
            [f"@keyword('{get_value(keywords[keyword]['name'])}') # type: ignore",
            f"def {codegen.camel_to_snake(keyword)}({', '.join(['self'] + gen_call_args(args))}): # type: ignore"],
       codegen.gen_doc("\n\n".join(
                [get_value(keywords[keyword]['doc']['desc'])] +
                ([gen_args_doc(keywords[keyword]["args"])] if keywords[keyword]["args"] else []) +
                [get_value(keywords[keyword]['doc']['examples'])]
            )) +
            [""] +
            ["args = [" + ", ".join([get_value(arg['name']) for _, arg in args.items()]) + "]"] +
            [""] +
            gen_result(result) +
            [
            f"return super()._run_keyword('{keyword}', args, result) # type: ignore"
            ]
        )

    return methods


def generate_rf_lib(lib: LocalizedLib, version: str):
    spec = lib.spec
    base_class = "RoboSAPiensClient"
    imports = {
        "robot.api.deco": ["keyword"],
        "RoboSAPiens.client": [base_class]
    }
    properties = [
        "ROBOT_LIBRARY_SCOPE = 'SUITE'",
        f"ROBOT_LIBRARY_VERSION = '{version}'"
    ]
    doc = codegen.gen_doc(get_value(spec["doc"]["intro"]))
    init = codegen.gen_init(gen_call_args(spec["args"]),
        codegen.gen_doc(get_value(spec["doc"]["init"]) + "\n" + gen_args_doc(spec["args"])),
        [""] +
        gen_args(spec["args"]) +
        [""] +
        ["super().__init__(args)"]
    )
    methods = init + gen_methods(spec['keywords'])

    return codegen.pprint_sections([
        codegen.gen_imports(imports),
        "\n".join(codegen.gen_class(lib.name, properties, doc, methods, base_class))
    ])


def get_localizations(path: Path):
    return [
        LocalizedLib(
            name = get_lib_name(lang),
            path = get_lib_path(lang),
            spec = importlib.import_module(f"{path}.{lang}", ".").lib
        )
        for file in os.listdir(path) if file.endswith(".py") and not file.startswith("__")
        for lang in [Path(file).stem]
    ]


def get_lib_path(lang: str):
    if lang == "en":
        return Path("RoboSAPiens") / "__init__.py"
    else:
        return Path("RoboSAPiens") / Path(lang.upper()) / "__init__.py"


def get_lib_name(lang: str):
    if lang == "en":
        return "RoboSAPiens"
    else:
        return lang.upper()


if __name__ == "__main__":
    _, *args = sys.argv

    localized_libs = get_localizations(Path("localized"))

    for lib in localized_libs:
        rf_lib = generate_rf_lib(lib, __version__)

        with open(lib.path, "w", encoding="utf-8") as file:
            file.write(rf_lib)

    sys.exit(0)
