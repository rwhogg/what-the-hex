task default: %W[bundle]

task bundle: ["dist/What The Hex.zip"]

%W[game.py].each do |pyfile|
    sh "pyinstaller --noconsole --icon icon.ico --onefile #{pyfile} -n \"What The Hex\""
    sh "7z a \"dist/What The Hex.zip\" \"dist/What The Hex.app\""
end
