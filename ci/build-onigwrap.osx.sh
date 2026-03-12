#!/bin/bash

set -e

export CC="clang -target $_HOST"
export CFLAGS="-O2 -s"
export LDFLAGS="-Wl,-headerpad_max_install_names"

mkdir -p buildprefix

pushd oniguruma

autoreconf -i
./configure --enable-shared=no --with-pic=yes --host="${_HOST%-macabi}" --prefix="$(realpath ../buildprefix)" || (cat config.log; exit 1)
make
make install

popd

$CC -dynamiclib onigwrap/onigwrap.c $CFLAGS $LDFLAGS -I./buildprefix/include -L./buildprefix/lib -lonig -o "$_LIBNAME"
