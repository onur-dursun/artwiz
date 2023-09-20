ffmpeg -i %~dpnx1 -vcodec h264 -acodec aac %~dpn1_compressed.mp4
explorer.exe /select, %~dpn1_compressed.mp4