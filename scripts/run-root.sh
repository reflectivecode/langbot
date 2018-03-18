#!/usr/bin/env sh
set -o errexit
set -o nounset

echo "change ownership of ${HOME} to ${USER}:${GROUP}"
chown -R ${USER}:${GROUP} "${HOME}"

echo "execute run-user.sh as ${USER}"
gosu ${USER} run-user.sh
