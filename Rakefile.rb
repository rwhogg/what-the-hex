task default: %W[bundle]

task bundle: ["dist/What The Hex.exe", "dist/What The Hex Installer.exe"]

%W[game.py].each do |pyfile|
    sh "pyinstaller --noconsole --icon icon.ico --onefile #{pyfile} -n \"What The Hex.exe\""
end

%W[installer.nsi].each do |installerscript|
   sh "makensis #{installerscript}"
end
