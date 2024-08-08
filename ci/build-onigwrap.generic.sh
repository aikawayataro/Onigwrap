#!/bin/bash

set -e

export CC=$_HOST-gcc

mkdir -p buildprefix

pushd oniguruma

autoreconf -i
./configure --enable-shared=no --with-pic=yes --host="$_HOST" --prefix="$(realpath ../buildprefix)" || (cat config.log; exit 1)
make
make install

popd

$CC -shared -fPIC onigwrap/onigwrap.c -O2 -s -I./buildprefix/include -L./buildprefix/lib -lonig -o "$_LIBNAME"
