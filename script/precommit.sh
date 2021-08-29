#!/usr/bin/env bash

pushd project || exit

dotnet format --fix-style info --fix-whitespace --fix-analyzers info "WhatTheHex.csproj"

# Add license and address overzealous HTML-escaping of my name
addlicense -c 'Bob "Wombat" Hogg' -l apache ./*.cs
git status | grep modified | awk '{print $2}' | xargs sed -i 's/&#34;Wombat&#34;/"Wombat"/g'

../script/format.sh

popd || exit

shellcheck script/bootstrap.sh script/changelog.sh script/gendocs.sh script/lint.sh script/precommit.sh
