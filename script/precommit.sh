#!/usr/bin/env bash

pushd project || exit

PROJ="WhatTheHex.csproj"

dotnet format whitespace $PROJ
dotnet format style $PROJ
dotnet format analyzers $PROJ

# Address overzealous HTML-escaping of my name and handle the fact that semicolons indicate comments in Editorconfig files
git status | grep modified | grep -v precommit.sh | awk '{print $2}' | xargs sed -i 's/&#34;Wombat&#34;/"Wombat"/g'
git status | grep modified | grep -v precommit.sh | awk '{print $2}' | xargs sed -i 's/License")/License");/g'

../script/format.sh

popd || exit

shellcheck script/bootstrap.sh script/changelog.sh script/gendocs.sh script/lint.sh script/precommit.sh
