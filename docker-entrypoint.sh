#!/bin/sh
set -e

# Render (and most PaaS Docker runners) inject $PORT and expect the app to
# bind to it; Fly.io uses the fixed internal_port from fly.toml (8080).
export ASPNETCORE_URLS="http://+:${PORT:-8080}"

exec dotnet TheNoir.Api.dll
