from os.path import realpath
from pathlib import Path
import subprocess
import time

from .version import __version__
from robot.libraries.Remote import Remote

encoding = 'iso-8859-1'
RoboSAPiens_exe = "RoboSAPiens.exe"
RoboSAPiens_path = Path(realpath(__file__)).parent / "lib" / RoboSAPiens_exe

class RoboSAPiens:
    def is_running(self):
        process_list = subprocess.check_output(['TASKLIST', '/FI', f'imagename eq {RoboSAPiens_exe}']).decode(encoding)
        return any([line.startswith(RoboSAPiens_exe) for line in process_list.splitlines()])

    def __init__(self, port="8270"):
        self.port = port
        self.url = f"http://127.0.0.1:{port}"

        if not self.is_running():
            proc = subprocess.Popen([RoboSAPiens_path, "-p", port], stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
            time.sleep(0.3)
            returncode = proc.poll()

            if returncode and returncode > 0:
                assert proc.stdout != None
                message = "\n".join(line.decode(encoding).strip() for line in proc.stdout.readlines())               
                raise Exception(message)

        self.RoboSAPiens = Remote(self.url)

    def __del__(self):
        if self.is_running():
            subprocess.check_output(['TASKKILL', '/F', '/IM', RoboSAPiens_exe])

    def get_keyword_arguments(self, name):
        return self.RoboSAPiens.get_keyword_arguments(name)

    def get_keyword_names(self):
        return self.RoboSAPiens.get_keyword_names()

    def get_keyword_documentation(self, name):
        if name == "__intro__": 
            return "RoboSAPiens: SAP GUI-Automatisierung für Menschen"
        
        if name == "__init__":
            return "\n".join([
                "Um diese Bibliothek zu verwenden, muss das [https://help.sap.com/saphelp_aii710/helpdata/de/ba/b8710932b8c64a9e8acf5b6f65e740/content.htm|Scripting auf dem SAP Server] aktiviert werden.",
                "Außerdem muss die [https://help.sap.com/docs/sap_gui_for_windows/63bd20104af84112973ad59590645513/7ddb7c9c4a4c43219a65eee4ca8db001.html|Skriptunterstützung in der SAP GUI] aktiviert werden.\n\n"
                "Diese Bibliothek implementiert die [https://robotframework.org/robotframework/latest/RobotFrameworkUserGuide.html#remote-library-interface|Remote Library Interface] von Robot Framework. Das heißt, ein HTTP Server läuft im Hintergrund und Robot Framework kommuniziert mit ihm. Standardmäßig lauscht der HTTP Server auf dem Port 8270. Der Port kann beim Import der Bibliothek angepasst werden:\n\n"
                "| ``Library   RoboSAPiens  port=1234``"
            ])

        return self.RoboSAPiens.get_keyword_documentation(name)

    def get_keyword_types(self, name):
        return self.RoboSAPiens.get_keyword_types(name)

    def run_keyword(self, name, args, kwargs):
        try:
            return self.RoboSAPiens.run_keyword(name, args, kwargs)
        except RuntimeError as err:
            if "WinError 10061" in str(err):
                raise Exception("The RoboSAPiens keyword server terminated unexpectedly")
            else:
                raise err

    ROBOT_LIBRARY_SCOPE = 'GLOBAL'
    ROBOT_LIBRARY_VERSION = __version__