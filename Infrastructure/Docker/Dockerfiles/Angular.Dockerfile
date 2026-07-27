############################################################
# Stage 1 - Build Angular Application
############################################################
FROM node:22-alpine AS build

LABEL stage="build"

WORKDIR /app

ARG APP_VERSION=1.0.0

#
# Copy package files
#
COPY RetailInventory.Angular/package*.json ./

#
# Install dependencies
#
RUN npm ci

#
# Copy Angular source
#
COPY RetailInventory.Angular/ .

#
# Build Angular
#
RUN npm run build -- --configuration production

############################################################
# Stage 2 - Runtime
############################################################
FROM nginx:1.29-alpine AS runtime

ARG APP_VERSION=1.0.0

LABEL maintainer="Juan Alfredo Gutierrez"
LABEL application="JMCloudLab Retail Inventory Platform"
LABEL component="Frontend"
LABEL framework="Angular 21"
LABEL version="${APP_VERSION}"

#
# Runtime tools
#
RUN apk add --no-cache gettext

#
# Nginx
#
COPY RetailInventory.Angular/docker/nginx.conf \
     /etc/nginx/conf.d/default.conf

#
# Startup script
#
COPY RetailInventory.Angular/docker/entrypoint.sh \
     /entrypoint.sh

RUN chmod +x /entrypoint.sh

#
# Angular application
#
COPY --from=build \
     /app/dist/JMCloudLab.RetailInventoryPlatform.Web/browser \
     /usr/share/nginx/html

EXPOSE 80

ENTRYPOINT ["/entrypoint.sh"]

CMD ["nginx","-g","daemon off;"]