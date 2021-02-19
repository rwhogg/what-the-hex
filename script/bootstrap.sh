#!/usr/bin/env bash

pip install -r requirements.txt

briefcase dev --no-run

if [ -x "$(which npm)" ]; then
    npm i -g pyright
fi
