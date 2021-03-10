#!/usr/bin/env bash

yapf --style='{based_on_style: google, indent_width: 4, column_limit: 120}' -i game/*.py game/levels/*.py
