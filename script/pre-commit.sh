#!/usr/bin/env bash

pushd project || exit
dotnet format "WhatTheHex.csproj"
addlicense -c 'Bob "Wombat" Hogg' -l apache ./*.cs
unix2dos ./*.cs
dos2unix project.godot

popd || exit
dos2unix script/*.sh 
shellcheck script/*.sh
