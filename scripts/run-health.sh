#!/usr/bin/env sh
set -o errexit
set -o pipefail
set -o nounset
set -o xtrace

wget --tries=1 --spider "${PING}"
