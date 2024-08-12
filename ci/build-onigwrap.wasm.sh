#!/bin/bash

set -e

export CFLAGS="-O2 -s"
# https://github.com/emscripten-core/emscripten/issues/16915
export NODE_OPTIONS=--no-experimental-fetch

if [[ "$_HOST" =~ "-mt"$ ]]; then
    export CFLAGS="$CFLAGS -pthread"
    export LIBS=-pthread
fi

source ./emsdk/emsdk_env.sh

mkdir -p buildprefix

pushd oniguruma

autoreconf -i
emconfigure ./configure --enable-shared=no --with-pic=yes --prefix="$(realpath ../buildprefix)" || (cat config.log; exit 1)
emmake make
emmake make install

popd

emcc -c onigwrap/onigwrap.c $CFLAGS $LIBS -I./buildprefix/include -o onigwrap.o
cp ./buildprefix/lib/libonig.a "$_LIBNAME"
emar -rs "$_LIBNAME" onigwrap.o
