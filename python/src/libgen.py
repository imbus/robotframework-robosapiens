import os
import re
from glob import glob
from importlib import import_module
from pathlib import Path

from jinja2 import Environment, FileSystemLoader

from version import __version__


def drop_prefix(arg_name: str):
    return re.sub(r'a\d', '', arg_name) 


def escape_chars(string: str) -> str:
    return string.replace('\n', '\\n').replace('"', '\\"')


def format_arg(arg: dict) -> str:
    name = get_value(arg["name"])
    
    if 'default' in arg:
        default = get_value(arg['default'])
    
        if default is None:
            return "%s: str=None" % name
        
        if isinstance(default, bool):
            return "%s: bool=%s" % (name, default)
        else:
            return "%s: str='%s'" % (name, default)

    return "%s: str" % name


def get_value(value: str|tuple[str, str]) -> str:
    if isinstance(value, tuple): return value[1]
    return value


def snake_case(name: str):
    return ''.join(['_' + c.lower() if c.isupper() else c for c in name]).lstrip('_')


def splitlines(string: str) -> list[str]:
    return [
        line.lstrip()
        for line in string.splitlines()
    ]


def module_name(lang: str) -> str:
    return '' if lang == 'en' else lang.upper()


if __name__ == "__main__":
    env = Environment(loader=FileSystemLoader('localized'))
    env.filters['drop_prefix'] = drop_prefix
    env.filters['escape_chars'] = escape_chars
    env.filters['format_arg'] = format_arg
    env.filters['get_value'] = get_value
    env.filters['snake_case'] = snake_case
    env.filters['splitlines'] = splitlines
    jinja_template = env.get_template('template.py.jinja')

    for filename in glob("localized/[a-z]*.py"):
        spec_module = filename.strip('.py').replace(os.sep, '.')
        lib = import_module(spec_module).lib
        _, lang = spec_module.split('.')
        lib['module_name'] = module_name(lang) or 'RoboSAPiens'
        lib['version'] = __version__
        init_file = Path('RoboSAPiens') / module_name(lang) / '__init__.py'

        with open(init_file, "w", encoding="utf-8") as file:
            file.write(jinja_template.render(lib))
