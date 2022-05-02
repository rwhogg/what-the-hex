#!/usr/bin/env bash

# No really good way to install Godot Mono version on Windows atm...
# And I don't think most Linux distros package that version...
# So just install that yourself...

if [ -z "$GODOT" ]; then
    echo "Set the environment variable GODOT"
elif $GODOT --version | tr -d '[:space:]' | grep -q 3.4.4.stable.mono.official.419e713a2 ; then
    echo "Godot Mono is available!"
else
    echo "Please install Godot Mono edition 3.4.4 if you have not done so already"
fi

if [ -x "$(command -v brew)" ]; then
    brew bundle check -v --file=script/Brewfile || brew bundle --file=script/Brewfile -v --no-lock
fi

if [ -x "$(command -v gendarme)" ]; then
    echo "Gendarme is available!"
elif [[ $(uname -r) =~ .*WSL2 ]]; then
    if [ -x "$(command -v apt)" ]; then
        sudo apt install gendarme
    else
        echo "Please install Gendarme!"
    fi
else
    echo "Please install Gendarme!"
fi
