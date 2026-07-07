from pathlib import Path
from setuptools import setup

VERSION = Path("../VERSION").read_text(encoding="utf-8")

setup(
    version=VERSION
)
