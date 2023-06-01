# -*- mode: python ; coding: utf-8 -*-

import sys
from os import path
site_packages = next(p for p in sys.path if 'site-packages' in p)
block_cipher = None


a = Analysis(
    ['playground\\manage.py'],
    pathex=[],
    binaries=[],
    datas=[
        (path.join(site_packages, "RoboSAPiens"), "RoboSAPiens"), 
        (path.join("playground", "core"), "core"), 
        (path.join("playground", "endpoints"), "endpoints"),
        (path.join("playground", "static"), "static"),
        (path.join("playground", "components"), "components"),
        (path.join("playground", "templates"), "templates")
    ],
    hiddenimports=['RoboSAPiens', 'core', 'endpoints', 'static', 'components', 'templates'],
    hookspath=[],
    hooksconfig={},
    runtime_hooks=[],
    excludes=[],
    win_no_prefer_redirects=False,
    win_private_assemblies=False,
    cipher=block_cipher,
    noarchive=False,
)
pyz = PYZ(a.pure, a.zipped_data, cipher=block_cipher)

exe = EXE(
    pyz,
    a.scripts,
    [],
    exclude_binaries=True,
    name='playground',
    debug=False,
    bootloader_ignore_signals=False,
    icon='RoboSAPiens.ico',
    strip=False,
    upx=True,
    console=False,
    disable_windowed_traceback=False,
    argv_emulation=False,
    target_arch=None,
    codesign_identity=None,
    entitlements_file=None,
)
coll = COLLECT(
    exe,
    a.binaries,
    a.zipfiles,
    a.datas,
    strip=False,
    upx=True,
    upx_exclude=[],
    name='playground',
)
