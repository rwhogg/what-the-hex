#!/usr/bin/env bash

pushd project || exit
dotnet.exe format --fix-style info --fix-whitespace --fix-analyzers info "WhatTheHex.csproj"
addlicense -c 'Bob "Wombat" Hogg' -l apache ./*.cs
../script/format.sh
popd || exit

shellcheck script/bootstrap.sh script/changelog.sh script/gendocs.sh script/lint.sh script/pre-commit.sh
