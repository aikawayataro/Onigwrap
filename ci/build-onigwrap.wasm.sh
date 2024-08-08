#!/bin/bash

set -e

# https://github.com/emscripten-core/emscripten/issues/16915
export NODE_OPTIONS=--no-experimental-fetch

source ./emsdk/emsdk_env.sh

mkdir -p buildprefix

pushd oniguruma

autoreconf -i
emconfigure ./configure --enable-shared=no --with-pic=yes --prefix="$(realpath ../buildprefix)" || (cat config.log; exit 1)
emmake make
emmake make install

popd

emmake libtool --tag=CC --mode=compile emcc -c onigwrap/onigwrap.c -O2 -s -I./buildprefix/include -o wrap.lo
emmake libtool --tag=CC --mode=link emcc -static wrap.lo -o wrap.a
emmake libtool --tag=CC --mode=link emcc -all-static wrap.a ./buildprefix/lib/libonig.a -o "$_LIBNAME"
