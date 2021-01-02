!define VERSION "1.0.0"
Name "Hexasexy"
OutFile "dist\Hexasexy-installer.exe"

InstallDir "$PROGRAMFILES64\Hexasexy"

InstallButtonText "Install!"

Page directory
Page instfiles

Section "Hexasexy"
    SetOutPath $INSTDIR
    File "dist\Hexasexy.exe"
    File "kenney-pixel-square.ttf"
    File "drumming-sticks.ogg"
    File "game_over-sound.wav"
    File "game_over-voice.ogg"
    File "match.wav"
    File "rotate.ogg"
    File /r "licenses"
    File "LICENSE.txt"
    File "README.md"
SectionEnd
