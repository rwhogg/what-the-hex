#!/usr/bin/env bash

pip install -r requirements.txt

if [ -x "$(which npm)" ]; then
    npm i -g pyright
fi
