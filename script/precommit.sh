#!/usr/bin/env bash

pushd project || exit

PROJ="WhatTheHex.csproj"

dotnet format whitespace $PROJ
dotnet format style $PROJ
dotnet format analyzers $PROJ

# Add license and address overzealous HTML-escaping of my name
# FIXME: replace with a dotnet file header format requirement
addlicense -c 'Bob "Wombat" Hogg' -l apache ./*.cs
git status | grep modified | grep -v precommit.sh | awk '{print $2}' | xargs sed -i 's/&#34;Wombat&#34;/"Wombat"/g'

../script/format.sh

popd || exit

shellcheck script/bootstrap.sh script/changelog.sh script/gendocs.sh script/lint.sh script/precommit.sh
