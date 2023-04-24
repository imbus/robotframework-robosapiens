import subprocess
import sys
import time
from os.path import realpath
from pathlib import Path
from typing import Any, Dict, List, Mapping
from robot.errors import RemoteError
from robot.libraries.Remote import ArgumentCoercer, RemoteResult, XmlRpcRemoteClient


def _cli_arg(name: str, value: object):
    name = name.replace("_", "-")

    if type(value) == bool:
        return f"--{name}"
    else:
        return f"--{name} {value}"


def is_running(cmd: str):
    process_list = subprocess.check_output(
        # Set the character page to 65001 = UTF-8
        f'chcp 65001 | TASKLIST /FI "IMAGENAME eq {cmd}"', shell=True
    ).decode('utf-8')

    return any(line.startswith(cmd) for line in process_list.splitlines())


def start_cmd(cmd: Path, args: List[str]):
    if is_running(cmd.name):
        return

    cmd_args = " ".join([str(cmd)] + args)

    return subprocess.Popen(
        cmd_args, stdout=subprocess.PIPE, stderr=subprocess.STDOUT
    )


def check_alive(proc: subprocess.Popen): # type: ignore
    timeout = 0.5 # seconds
    elapsed = 0.0 # seconds
    waiting = 0.1 # seconds

    while elapsed < timeout:
        time.sleep(waiting)
        elapsed += waiting

        returncode = proc.poll()

        if returncode and returncode > 0:
            message = "\n".join(
                line.decode('utf-8').strip() for line in proc.stdout.readlines() # type: ignore
            )
            raise RemoteError(message)


class RoboSAPiensClient(object):
    def __init__(self, args: Mapping[str, Any]):
        uri = f"http://127.0.0.1:{args['port']}"
        server = Path(realpath(__file__)).parent / "lib" / "RoboSAPiens.exe"

        self.server = self._start_server(server, args)
        self.client = self._start_client(uri)


    def _start_client(self, uri: str):
        if self.server:
            check_alive(self.server)

        return XmlRpcRemoteClient(uri)


    def _start_server(self, server: Path, args: Mapping[str, Any]):
        cli_args = [
            _cli_arg(name, value) 
            for name, value in args.items() 
            if value
        ]

        return start_cmd(server, cli_args)

    def __del__(self):
        if self.server:
            self.server.terminate()

    def _run_keyword(self, name: str, args: List[Any], kwargs: Dict[str, Any], result: Dict[str, Any]): # type: ignore
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
