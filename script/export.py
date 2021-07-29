# Run like this: python3 script/export.py [platforms...]

import os.path
import re

from configparser import ConfigParser
from sys import argv, exit
from os import chdir, getenv, mkdir
from subprocess import Popen

chdir("project")

if len(argv) > 1:
    export_names = filter(lambda s: not (s.startswith("python") or s.endswith(".py")), argv)
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
