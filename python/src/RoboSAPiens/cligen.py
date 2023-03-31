
import json
  
# Opening JSON file and returns object as a dictionary
with open('cli.json') as json_file:
    data = json.load(json_file)
json_file.close()

# generate python-methods (with correct format) out of json-dict
methods = ""
for method in data:
    methods += "\t" + str(method['name']).replace("-", "_") + ": " + str(method['type']).lower() + "\n"

fileContent = f"""
from typing import TypedDict

class CliArgsDict(TypedDict):
{methods}
"""

newFile = open('cli.py', 'w+')
newFile.write(fileContent)
newFile.close()