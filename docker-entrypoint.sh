#!/bin/sh
# Points wwwroot/uploads at a /data directory (a Fly.io volume mount, or just
# ephemeral container storage on hosts without persistent disks) so the app
# always finds it at the same path regardless of host.
set -e

mkdir -p /data/uploads
rm -rf /app/wwwroot/uploads
ln -s /data/uploads /app/wwwroot/uploads

# Render (and most PaaS Docker runners) inject $PORT and expect the app to
# bind to it; Fly.io uses the fixed internal_port from fly.toml (8080).
export ASPNETCORE_URLS="http://+:${PORT:-8080}"

exec dotnet TheNoir.Api.dll
