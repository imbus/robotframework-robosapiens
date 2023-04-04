import json

  
with open('cli.json') as json_file:
    cli_args = json.load(json_file)

args = [
    { 
        "name": arg['name'].replace("-", "_"),
        "type": arg['type'].lower()
    }
    for arg in cli_args
]

dict_entries = '\n'.join([
    f"    {arg['name']}: {arg['type']}"
    for arg in args
])

fileContent = f"""from typing import TypedDict

class CliArgsDict(TypedDict):
{dict_entries}
"""

with open('cli.py', 'w+') as file:
    file.write(fileContent)
