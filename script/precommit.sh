#!/usr/bin/env bash

script/format.sh
script/lint.sh || exit 1
script/typecheck.sh || exit 1
script/test.sh || exit 1
