!define VERSION "1.0.0"
Name "What The Hex"
OutFile "dist\What The Hex Installer.exe"
Icon "icon.ico"

InstallDir "$PROGRAMFILES64\What The Hex"

InstallButtonText "Install!"

Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

Section "What The Hex"
    SetOutPath $INSTDIR
    File "dist\What The Hex.exe"
    File "kenney-pixel-square.ttf"
    File "bg_music.ogg"
    File "game_over-sound.wav"
    File "game_over-voice.ogg"
    File "icon.ico"
    File "icon.png"
    File "mouseLeft.png"
    File "mouseRight.png"
    File "rotate_clockwise.png"
    File "rotate_counterclockwise.png"
    File "match.wav"
    File "refresh.ogg"
    File "rotate.ogg"
    File "pygame_powered.gif"
    File /r "licenses"
    File "LICENSE.txt"
    File "README.md"

    WriteUninstaller $INSTDIR\uninstaller.exe
SectionEnd

Section "Uninstall"
    Delete $INSTDIR\uninstaller.exe
    Delete "$INSTDIR\What The Hex.exe"
    Delete $INSTDIR\kenney-pixel-square.ttf
    Delete $INSTDIR\bg_music.ogg
    Delete $INSTDIR\game_over-sound.wav
    Delete $INSTDIR\game_over-voice.ogg
    Delete $INSTDIR\icon.ico
    Delete $INSTDIR\icon.png
    Delete $INSTDIR\mouseLeft.png
    Delete $INSTDIR\mouseRight.png
    Delete $INSTDIR\rotate_clockwise.png
    Delete $INSTDIR\rotate_counterclockwise.png
    Delete $INSTDIR\match.wav
    Delete $INSTDIR\refresh.ogg
    Delete $INSTDIR\rotate.ogg
    Delete $INSTDIR\pygame_powered.gif
    RMDir /r $INSTDIR\licenses
    Delete $INSTDIR\LICENSE.txt
    Delete $INSTDIR\README.md
    RMDir $INSTDIR
SectionEnd
