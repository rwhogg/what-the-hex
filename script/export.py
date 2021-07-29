# Run like this: python3 script/export.py [platforms...]

import os.path
import re
import sys

if sys.version_info < (3, 6):
    print("Run with Python 3 please")
    sys.exit(1)

from configparser import ConfigParser
from os import chdir, getenv, mkdir
from subprocess import Popen

chdir("project")

if len(sys.argv) > 1:
    export_names = filter(lambda s: not (s.startswith("python") or s.endswith(".py")), sys.argv)
else:
    print("Exporting all")
    cfg = ConfigParser()
    cfg.read("export_presets.cfg")
    exports = filter(lambda section: re.match("^preset.\d$", section) is not None, cfg.sections())
    export_names = map(lambda export: cfg.get(export, "name").replace('"', ''), exports)

godot = getenv("GODOT")

for export_name in export_names:
    try:
        mkdir(os.path.join("..", "exports", export_name))
    except Exception:
        print("Directory already exists")
    print("Exporting for " + export_name)
    process = Popen([godot, "--no-window", "--export", export_name])
    process.wait(30)
