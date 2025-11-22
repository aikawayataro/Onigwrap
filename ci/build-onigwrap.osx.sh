#!/bin/bash

set -e

export CC="clang -target $_HOST"
export CFLAGS="-O2 -s"

mkdir -p buildprefix

pushd oniguruma

autoreconf -i
./configure --enable-shared=no --with-pic=yes --host="${_HOST%-macabi}" --prefix="$(realpath ../buildprefix)" || (cat config.log; exit 1)
make
make install

popd

$CC -dynamiclib onigwrap/onigwrap.c $CFLAGS -I./buildprefix/include -L./buildprefix/lib -lonig -o "$_LIBNAME"
