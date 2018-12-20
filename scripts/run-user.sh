#!/usr/bin/env sh
set -o errexit
set -o pipefail
set -o nounset
set -o xtrace

echo "start LangBot"
dotnet LangBot.Web.dll
