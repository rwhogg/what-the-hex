#!/usr/bin/env bash

echo "Reformatting"
script/format.sh

echo "Linting"
script/lint.sh || exit 1

echo "Typechecking"
script/typecheck.sh || exit 1

echo "Running tests"
script/test.sh || exit 1
