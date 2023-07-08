from subprocess import Popen, PIPE
import json

request = {"jsonrpc": "2.0","id": 1,"method": "OpenSap","params": ["locator"]}

cmd = "RoboSAPiens.exe"
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
    proc.stdin.write(req + '\n')

    for line in iter(proc.stdout.readline, '\n'):
        print(line, end='')
