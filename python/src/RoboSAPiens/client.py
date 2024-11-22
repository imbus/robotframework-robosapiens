import json
import sys

from itertools import count
from os.path import realpath
from pathlib import Path
from subprocess import Popen, PIPE
from typing import Any, Dict, List, Tuple

from robot.errors import RemoteError
from robot.libraries.Remote import RemoteResult


class NotChangeable(Exception):
    def __init__(self, message='', details=''):
        super().__init__(message)
        self.details = details

    @property
    def message(self):
        return str(self)


class NotFound(Exception):
    def __init__(self, message='', details=''):
        super().__init__(message)
        self.details = details

    @property
    def message(self):
        return str(self)


class RoboSAPiensClient(object):
    def __init__(self, args: Dict[str, Any]):
        if args.pop("x64"):
            self.server_cmd = Path(realpath(__file__)).parent / "lib64" / "RoboSAPiens.exe"
        else:
            self.server_cmd = Path(realpath(__file__)).parent / "lib32" / "RoboSAPiens.exe"
        self.args = list(args.items())
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
    def _run_keyword(self, name: str, args: List[str], result: Dict[str, str]): # type: ignore
        request = {
            "jsonrpc": "2.0",
            "method": name,
            "args": args,
            "id": next(self.counter)
        }

        assert self._server.stdout is not None
        assert self._server.stdin is not None

        self._server.stdin.write(json.dumps(request) + '\n')
        response = ''.join(list(iter(self._server.stdout.readline, '\n')))
        json_response = json.loads(response)

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

            if error_type == 'NotFound':
                raise NotFound(message, rf_result.traceback)
            
            if error_type == 'NotChangeable':
                raise NotChangeable(message, rf_result.traceback)

            raise RemoteError(
                message,
                rf_result.traceback
            )

        if "Log" in result:
            sys.stdout.write(str(rf_result.return_))
        else:
            sys.stdout.write(result["Pass"].format(*args) + '\n')

        if "Json" in result:
            return json.loads(str(rf_result.return_))

        return rf_result.return_


def _cli_args(args: List[Tuple[str, Any]]) -> List[str]:
    if len(args) == 0:
        return []

    (name, value), *rest = args

    name = '--' + name.replace("_", "-")

    if isinstance(value, bool):
        if value:
            return [name] + _cli_args(rest)

        return _cli_args(rest)

    return [name, str(value)] + _cli_args(rest)
