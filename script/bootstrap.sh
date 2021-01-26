#!/usr/bin/env bash

pip install briefcase
pip install flake8 nose pylint yapf

pip install -r requirements.txt

if [ -x "$(which npm)" ]; then
    npm i -g pyright
fi
