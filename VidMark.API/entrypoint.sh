#!/bin/sh
set -e

# Fix volume-mounted directory ownership at runtime so the 'app' user can write
# to directories that may have been created with root ownership by Docker.
chown -R app:app /app/videos /app/reports /app/Logs

exec gosu app dotnet VidMark.API.dll
