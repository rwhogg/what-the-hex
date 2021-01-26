#!/usr/bin/env bash

pip install briefcase
pip install flake8 nose pylint scancode-toolkit[full] yapf

# FIXME: see https://github.com/nexB/scancode-toolkit/issues/2368
pip install commoncode==20.10.20

if [ -x "$(which npm)" ]; then
    npm i -g pyright
fi

# This should install from pyproject.toml,
# but until https://github.com/pypa/pip/issues/8049 is resolved, not happening...
pip install pygame

# So, two things here. One, waiting on briefcase to support tkinter (https://github.com/beeware/briefcase/issues/383),
# so that I can use guizero. Toga is overkill for this. Two, https://github.com/beeware/toga/pull/769 seems to have
# caused the game to fail to load when running from briefcase.
pip install toga==0.3.0-dev16
