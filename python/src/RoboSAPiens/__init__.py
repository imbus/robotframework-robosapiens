from os.path import realpath
from pathlib import Path
import subprocess
import time
import sys

from robot.libraries.Remote import Remote
from robot.api.logger import logging

RoboSAPiens_exe = "RoboSAPiens.exe"
RoboSAPiens_path = Path(realpath(__file__)).parent / "lib" / RoboSAPiens_exe

class RoboSAPiens:
    def is_running(self):
        process_list = subprocess.check_output(['TASKLIST', '/FI', f'imagename eq {RoboSAPiens_exe}']).decode('iso-8859-1')
        return any([line.startswith(RoboSAPiens_exe) for line in process_list.splitlines()])

    def __init__(self, port="8270"):
        self.url = f"http://127.0.0.1:{port}"

        if not self.is_running():
            proc = subprocess.Popen([RoboSAPiens_path, "-p", port], stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
        
            time.sleep(0.3)
            returncode = proc.poll()

            if returncode and returncode > 0:
                message, details = [line.decode('iso-8859-1').strip() for line in proc.stdout.readlines()]
                logging.error(f"{message} {details}")
                sys.exit(1)

        self.RoboSAPiens = Remote(self.url)

    def __del__(self):
        if self.is_running():
            subprocess.check_output(['TASKKILL', '/F', '/IM', RoboSAPiens_exe])

    def get_keyword_arguments(self, name):
        return self.RoboSAPiens.get_keyword_arguments(name)

    def get_keyword_names(self):
        return self.RoboSAPiens.get_keyword_names()

    def get_keyword_documentation(self, name):
        return self.RoboSAPiens.get_keyword_documentation(name)

    def get_keyword_types(self, name):
        return self.RoboSAPiens.get_keyword_types(name)

    def run_keyword(self, name, args, kwargs):
        return self.RoboSAPiens.run_keyword(name, args, kwargs)
