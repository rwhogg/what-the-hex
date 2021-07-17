#!/usr/bin/env bash

if [ -r project/.mono/temp/bin/Debug/WhatTheHex.dll ]; then
    gendarme project/.mono/temp/bin/Debug/WhatTheHex.dll -html gendarme.html --config rules.xml --set WhatTheHex
else
    echo "Please compile the assemblies!"
    exit 1
fi
