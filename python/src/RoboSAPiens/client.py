import asyncio
import subprocess
import sys
from os.path import realpath
from pathlib import Path
from typing import Any, Dict, List, Mapping, Tuple
from robot.errors import RemoteError
from robot.libraries.Remote import ArgumentCoercer, RemoteResult, XmlRpcRemoteClient


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


def _is_running(cmd: str):
    process_list = subprocess.check_output(
        # Set the character page to 65001 = UTF-8
        f'chcp 65001 | TASKLIST /FI "IMAGENAME eq {cmd}"', shell=True
    ).decode()

    return any(line.startswith(cmd) for line in process_list.splitlines())


class RoboSAPiensClient(object):
    def __init__(self, args: Mapping[str, Any]):
        uri = f"http://127.0.0.1:{args['port']}"
        server_cmd = Path(realpath(__file__)).parent / "lib" / "RoboSAPiens.exe"

        self.server = self._start_server(server_cmd, _cli_args(list(args.items())))
        self.client = XmlRpcRemoteClient(uri)

    def _start_server(self, server: Path, args: List[str]):
        if _is_running(server.name):
            return

        loop = asyncio.get_event_loop()
        return loop.run_until_complete(self.start_cmd(server, args))

    async def start_cmd(self, cmd: Path, args: List[str]):
        proc = await asyncio.create_subprocess_exec(
            str(cmd), *args, 
            stdout=asyncio.subprocess.PIPE, stderr=asyncio.subprocess.PIPE
        )

        try:
            _, stderr = await asyncio.wait_for(proc.communicate(), timeout=0.5)

            if proc.returncode and proc.returncode > 0:
                raise RemoteError(stderr.decode())
        except asyncio.TimeoutError:
            return proc

    def __del__(self):
        if self.server:
            self.server.terminate()

    def run_keyword(self, name: str, args: List[Any], kwargs: Dict[str, Any], result: Dict[str, Any]): # type: ignore
        coercer = ArgumentCoercer()
        args = coercer.coerce(args) # type: ignore
        kwargs = coercer.coerce(kwargs) # type: ignore

        try:
            rf_result = RemoteResult(self.client.run_keyword(name, args, kwargs)) # type: ignore

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
        except RuntimeError as err:
            if "WinError 10061" in str(err):
                raise Exception(
                    "The RoboSAPiens keyword server terminated unexpectedly."
                )
            else:
                raise err
