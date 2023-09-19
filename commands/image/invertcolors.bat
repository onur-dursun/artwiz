magick "%~dpnx1" -channel RGB -negate "%~dpn1_inverted.png"
explorer.exe /select, "%~dpn1_inverted.png"