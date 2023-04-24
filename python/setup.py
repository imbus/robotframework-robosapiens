#!/usr/bin/env python

from pathlib import Path
from setuptools import setup

VERSION = Path("../VERSION").read_text(encoding="utf-8")

with open(Path("src/version.py"), "w+") as file:
    file.write(f"__version__ = '{VERSION}'")

CLASSIFIERS = """
Development Status :: 5 - Production/Stable
License :: OSI Approved :: Apache Software License
Operating System :: Microsoft :: Windows :: Windows 10
Programming Language :: Python
Programming Language :: Python :: 3
Programming Language :: Python :: 3.7
Programming Language :: Python :: 3.8
Programming Language :: Python :: 3.9
Programming Language :: Python :: 3.10
Programming Language :: Python :: 3.11
Topic :: Software Development :: Testing
"""

setup(
    name='robotframework-robosapiens',
    version=VERSION,
    author='imbus Rheinland GmbH',
    maintainer='Marduk Bolaños',
    maintainer_email='marduk.bolanos@imbus.de',
    description='Fully localized Robot Framework library for automating the SAP GUI using text selectors.',
    long_description=Path("./README.md").read_text(encoding="utf-8"),
    long_description_content_type='text/markdown',
    url='https://github.com/imbus/robotframework-robosapiens',
    license='Apache 2',
    keywords='robotframework testing test automation sap gui',
    platforms='windows',
    classifiers=CLASSIFIERS.splitlines(),
    package_data={'': ['lib/*', 'DE/*']},
    package_dir={'': 'src'},
    packages=['RoboSAPiens'],
    install_requires=[
        'robotframework',
    ],
)