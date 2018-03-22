#!/usr/bin/env sh
set -o errexit
set -o nounset

if [ "${WAIT_FOR_FILE:-}" != "" ]; then
    echo "Checking for file ${WAIT_FOR_FILE}"
    while [ ! -f "${WAIT_FOR_FILE}" ]; then
        echo "File not found, sleeping"
        sleep 10
    done
fi

echo "change ownership of ${HOME} to ${USER}:${GROUP}"
chown -R ${USER}:${GROUP} "${HOME}"

echo "execute run-user.sh as ${USER}"
gosu ${USER} run-user.sh
