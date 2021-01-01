task default: %W[bundle]

task bundle: %W[dist/Hexasexy.exe dist/Hexasexy-installer.exe]

%W[game.py].each do |pyfile|
    sh "pyinstaller --noconsole --onefile #{pyfile} -n Hexasexy.exe"
end

%W[installer.nsi].each do |installerscript|
   sh "makensis #{installerscript}"
end
