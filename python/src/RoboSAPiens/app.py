import sys

from os.path import realpath
from pathlib import Path
from subprocess import run


def RoboSAPiens(arch: str):
    return str(Path(realpath(__file__)).parent / f"win-{arch}" / "RoboSAPiens.exe")

def x64():
    run([RoboSAPiens('x64'), *sys.argv[1:]])

def x86():
    run([RoboSAPiens('x86'), *sys.argv[1:]])
