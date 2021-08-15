#!/usr/bin/env bash

# No really good way to install Godot Mono version on Windows atm...
# And I don't think most Linux distros package that version...
# So just install that yourself...

if [ -z "$GODOT" ]; then
    echo "Set the environment variable GODOT"
elif [ "$($GODOT --version | tr -d '[:space:]')" = "3.3.3.rc1.mono.official.dec840452" ]; then
    echo "Godot Mono is available!"
else
    echo "Please install Godot Mono edition 3.3.3-rc1 if you have not done so already"
fi

if [ -x "$(command -v brew)" ]; then
    brew bundle --file=script/Brewfile --no-lock
fi

if [ -x "$(command -v dotnet-format)" ]; then
    echo "dotnet-format is available!"
elif [ -x "$(command -v dotnet.exe)" ]; then
    dotnet.exe tool install -g dotnet-format
else
    echo "Please install .NET"
fi

if [ -x "$(command -v gendarme)" ]; then
    echo "Gendarme is available!"
elif [[ $(uname -r) =~ .*WSL2 ]]; then
    sudo apt install gendarme
else
    echo "Please install Gendarme!"
fi

if [ -x "$(command -v addlicense)" ]; then
    echo "addlicense is available!"
elif [ -x "$(command -v go)" ]; then
    go get -u github.com/google/addlicense
else
    echo "Please install Golang"
fi
