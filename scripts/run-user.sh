#!/usr/bin/env sh
set -o errexit
set -o nounset

echo "start LangBot"
dotnet LangBot.Web.dll
