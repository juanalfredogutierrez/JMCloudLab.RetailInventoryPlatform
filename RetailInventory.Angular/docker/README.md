# Angular Docker

This folder contains the Docker runtime configuration.

## Files

- nginx.conf
- entrypoint.sh

The entrypoint generates runtime-config.json using environment variables before starting Nginx.

This allows changing the API endpoint without rebuilding Angular.