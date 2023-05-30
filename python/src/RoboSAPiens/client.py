import json
from itertools import count
from subprocess import Popen, PIPE
import sys
from os.path import realpath
from pathlib import Path
from typing import Any, Dict, List, Mapping, Tuple
from robot.errors import RemoteError
from robot.libraries.Remote import RemoteResult

class RoboSAPiensClient(object):
    def __init__(self, args: Mapping[str, Any]):
        self.args = list(args.items())
        self.server_cmd = Path(realpath(__file__)).parent / "lib" / "RoboSAPiens.exe"
        self._server = self.server
        self.counter = count(1)

    @property
    def server(self):
        return self._start_server(self.server_cmd, _cli_args(self.args))

    def _start_server(self, server: Path, args: List[str]):
        return Popen(
            [str(server)] + args, 
            stdout=PIPE, 
            stdin=PIPE,
            bufsize=1,
            universal_newlines=True,
            encoding="utf-8"
        )

    def __del__(self):
        self._server.terminate()

    # All methods have to be private so that they are not interpreted as keywords by RF
    def _run_keyword(self, name: str, args: List[Any], kwargs: Dict[str, Any], result: Dict[str, Any]): # type: ignore
        request = {
            "jsonrpc": "2.0",
            "method": name,
            "params": args,
            "id": next(self.counter)
        }

        assert self._server.stdout is not None
        assert self._server.stdin is not None

        self._server.stdin.write(json.dumps(request) + '\n')
        response = []
        for line in iter(self._server.stdout.readline, '\n'):
            response.append(line)

        json_response = json.loads(''.join(response))

        if json_response["result"]:
           rf_result = RemoteResult(json_response["result"])
        else:
            rf_result = RemoteResult(json_response["error"]["data"])
        
        if rf_result.status != "PASS": # type: ignore
            error_type, error_message = rf_result.error.split("|")

            if error_type in {"SapError", "Exception"}:
                message = result[error_type].format(error_message)
            else:
                message = result[error_type].format(*args)

            raise RemoteError(
                message, 
                rf_result.traceback, 
                rf_result.fatal,
                rf_result.continuable
            )
        else:
            sys.stdout.write(result["Pass"].format(*args))
            return rf_result.return_ # type: ignore


def _cli_args(args: List[Tuple[str, Any]]) -> List[str]:
    if len(args) == 0:
        return []

    (name, value), *rest = args

    name = '--' + name.replace("_", "-")

    if type(value) == bool:
        if value:
            return [name] + _cli_args(rest)
        else:
            return _cli_args(rest)
    
    return [name, str(value)] + _cli_args(rest)
