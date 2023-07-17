from subprocess import Popen, PIPE
from pathlib import Path
from itertools import count
import json
import re

counter = count(1)

def request(method: str, params: list[str]) -> str:
    return json.dumps({
        "jsonrpc": "2.0", 
        "id": next(counter), 
        "method": method, 
        "params": params
    })


cmd = Path("build") / "RoboSAPiens.exe"
proc = Popen(
    cmd,
    stdin=PIPE,
    stdout=PIPE,
    bufsize=1,
    universal_newlines=True,
    encoding="utf-8"
)

assert proc.stdin
assert proc.stdout

while (req := input(">>> ")) != "quit":
    method, *params = re.split(r"\s\s+", req)
    proc.stdin.write(request(method, params) + '\n')

    for line in iter(proc.stdout.readline, '\n'):
        print(line, end='')
