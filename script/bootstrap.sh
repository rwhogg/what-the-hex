#!/usr/bin/env bash

<<<<<<< HEAD
pip install briefcase
pip install flake8 nose pylint  yapf

# FIXME: see https://github.com/nexB/scancode-toolkit/issues/2368
pip install commoncode==20.10.20

=======
pip install -r requirements.txt
>>>>>>> 77d2125 (Snyk readyness)
if [ -x "$(which npm)" ]; then
    npm i -g pyright
fi
