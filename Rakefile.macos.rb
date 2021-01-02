task default: %W[bundle]

task bundle: ["dist/What The Hex.app"]

%W[game.py].each do |pyfile|
    sh "pyinstaller --noconsole --icon icon.ico --onefile #{pyfile} -n \"What The Hex\""
end
