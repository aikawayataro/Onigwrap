#!/bin/bash

set -e

export CC="clang -arch x86_64 -arch arm64 -mmacosx-version-min=10.12"

mkdir -p buildprefix

pushd oniguruma

autoreconf -i
./configure --enable-shared=no --with-pic=yes --prefix="$(realpath ../buildprefix)" || (cat config.log; exit 1)
make
make install

popd

clang -dynamiclib -target x86_64-apple-macos10.12 onigwrap/onigwrap.c -O2 -s -I./buildprefix/include -L./buildprefix/lib -lonig -o x86_64.dylib
clang -dynamiclib -target arm64-apple-macos11 onigwrap/onigwrap.c -O2 -s -I./buildprefix/include -L./buildprefix/lib -lonig -o arm64.dylib

lipo -create -output "$_LIBNAME" x86_64.dylib arm64.dylib
lipo -archs "$_LIBNAME"
