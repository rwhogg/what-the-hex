!define VERSION "1.0.0"
Name "Hexasexy"
OutFile "dist\Hexasexy-installer.exe"

InstallDir "$PROGRAMFILES64\Hexasexy"

Section "Hexasexy"
    SetOutPath $INSTDIR
    File "dist\Hexasexy.exe"
    File "kenney-pixel-square.ttf"
    File "drumming-sticks.ogg"
    File "game_over-sound.wav"
    File "game_over-voice.ogg"
    File "rotate.ogg"
SectionEnd
