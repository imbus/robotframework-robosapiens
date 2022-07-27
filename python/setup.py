#!/usr/bin/env python

from setuptools import setup

CLASSIFIERS = """
Development Status :: 5 - Production/Stable
License :: OSI Approved :: MIT License
Operating System :: OS Independent
Programming Language :: Python
Programming Language :: Python :: 3
Programming Language :: Python :: 3.6
Programming Language :: Python :: 3.7
Programming Language :: Python :: 3.8
Programming Language :: Python :: 3.9
Topic :: Software Development :: Testing
"""

setup(name='robotframework-robosapiens',
      version="0.1",
      description='Robot Framework keyword library wrapper around the SAP scripting GUI',
      long_description="readme",
      long_description_content_type='text/markdown',
      author='imbus Rheinland GmbH',
      maintainer='Marduk Bolaños',
      maintainer_email='marduk.bolanos@imbus.de',
      url='https://github.com/imbus/robotframework-robosapiens',
      license='Apache 2',
      keywords='robotframework testing test automation sap gui',
      platforms='windows',
      classifiers=CLASSIFIERS.splitlines(),
      package_data={'': ['lib/RoboSAPiens.exe']},
      package_dir={'': 'src'},
      packages=['RoboSAPiens'],
      install_requires=[
          'robotframework',
      ],
)