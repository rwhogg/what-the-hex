Name "What The Hex?"
OutFile "exports\Win_x86_nsis\WhatTheHex.exe"
Icon "project\icon.ico"

InstallDir "$PROGRAMFILES\What The Hex"

Section "What The Hex"
    SetOutPath $INSTDIR
    File /r "exports\Win_x86\"
SectionEnd
