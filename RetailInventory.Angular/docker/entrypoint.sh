#!/bin/sh

echo "Generating runtime-config.json..."

envsubst '${API_URL} ${ASPNETCORE_ENVIRONMENT}' \
< /usr/share/nginx/html/assets/config/runtime-config.template.json \
> /usr/share/nginx/html/assets/config/runtime-config.json

echo "runtime-config.json generated"

exec "$@"