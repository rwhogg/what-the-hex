!define VERSION "1.0.0"
Name "Hexasexy"
OutFile "dist\Hexasexy-installer.exe"

InstallDir "$PROGRAMFILES64\Hexasexy"

InstallButtonText "Install!"

Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

Section "Hexasexy"
    SetOutPath $INSTDIR
    File "dist\Hexasexy.exe"
    File "kenney-pixel-square.ttf"
    File "bg_music.ogg"
    File "game_over-sound.wav"
    File "game_over-voice.ogg"
    File "match.wav"
    File "rotate.ogg"
    File "pygame_powered.gif"
    File /r "licenses"
    File "LICENSE.txt"
    File "README.md"

    WriteUninstaller $INSTDIR\uninstaller.exe
SectionEnd

Section "Uninstall"
    Delete $INSTDIR\uninstaller.exe
    Delete $INSTDIR\Hexasexy.exe
    Delete $INSTDIR\kenney-pixel-square.ttf
    Delete $INSTDIR\bg_music.ogg
    Delete $INSTDIR\game_over-sound.wav
    Delete $INSTDIR\game_over-voice.ogg
    Delete $INSTDIR\match.wav
    Delete $INSTDIR\rotate.ogg
    Delete $INSTDIR\pygame_powered.gif
    RMDir /r $INSTDIR\licenses
    Delete $INSTDIR\LICENSE.txt
    Delete $INSTDIR\README.md
    RMDir $INSTDIR
SectionEnd
