#!/bin/bash
# Local Windows build of chiaki-ng with keyboard + mouse support and the KBM launcher.
# Run from an MSYS2 MINGW64 shell, from the repository root:
#   scripts/build-windows-kbm.sh [output-dir]
# Needs the same MSYS2 packages as .github/workflows/build-windows-kbm.yml.
set -e
cd "$(dirname "${BASH_SOURCE[0]}")/.."
ROOT="$(pwd)"
OUT="${1:-$ROOT/dist/chiaki-ng-kbm}"

# Meson's Python needs a Windows home directory
if [ -z "$USERPROFILE" ]; then
	export USERPROFILE="$(cygpath -w "$HOME")"
fi

if [ ! -f /mingw64/lib/pkgconfig/libplacebo.pc ]; then
	LIBPLACEBO_VERSION=3be579e6c7f6b421d9ac3ab4860edc437c50b3eb scripts/build-libplacebo-windows.sh
fi

if [ ! -f /mingw64/include/libavcodec/avcodec.h ]; then
	mkdir -p deps && cd deps
	curl -LO https://github.com/streetpea/FFmpeg-Builds/releases/download/latest/ffmpeg-n9.0-latest-win64-gpl-shared-9.0.zip
	unzip -oq ffmpeg-n9.0-latest-win64-gpl-shared-9.0.zip
	cp -a "ffmpeg-n9.0-latest-win64-gpl-shared-9.0/bin/." /mingw64/bin
	cp -a "ffmpeg-n9.0-latest-win64-gpl-shared-9.0/include/." /mingw64/include
	cp -a "ffmpeg-n9.0-latest-win64-gpl-shared-9.0/lib/." /mingw64/lib
	cd "$ROOT"
fi

if [ ! -f /mingw64/lib/pkgconfig/sdl2.pc ]; then
	INSTALL_PREFIX=/mingw64 scripts/build-sdl2-compat.sh .
	INSTALL_PREFIX=/mingw64 scripts/build-hidapi.sh .
	cp /mingw64/lib/pkgconfig/sdl2-compat.pc /mingw64/lib/pkgconfig/sdl2.pc
fi

cmake -S . -B build -G "Ninja" -DCMAKE_BUILD_TYPE=Release -DCHIAKI_ENABLE_CLI=OFF
cmake --build build --config Release --target chiaki

# Deleting files of a running chiaki.exe would pull them out from under it
if tasklist //FI "IMAGENAME eq chiaki.exe" 2>/dev/null | grep -qi 'chiaki.exe'; then
	echo "chiaki.exe is running: close it first."
	exit 1
fi
rm -rf "$OUT"
mkdir -p "$OUT"
./scripts/deploy-windows-msys2.sh "$OUT/chiaki-ng-Win" build/gui/chiaki.exe "$ROOT/build/third-party/cpp-steam-tools" /mingw64 gui/src/qml

CSC="$(cygpath -u "$WINDIR")/Microsoft.NET/Framework64/v4.0.30319/csc.exe"
"$CSC" -nologo -target:winexe -codepage:65001 "-out:$(cygpath -w "$OUT/KBM Launcher.exe")" \
	"$(cygpath -w kbm-launcher/Launcher.cs)" "$(cygpath -w kbm-launcher/Strings.cs)" "$(cygpath -w kbm-launcher/Controller.cs)"

cp COPYING "$OUT/LICENSE.txt"
cp README.md "$OUT/README.md"
echo "Done: $OUT"
