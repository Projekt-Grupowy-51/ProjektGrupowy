FROM nginx:1.25-alpine

# Remove default config
RUN rm /etc/nginx/nginx.conf /etc/nginx/conf.d/default.conf

# Copy custom config template
COPY nginx.conf.template /etc/nginx/nginx.conf.template

# Copy entrypoint script
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

# Install envsubst (gettext package)
RUN apk add --no-cache gettext

EXPOSE 80

ENTRYPOINT ["/entrypoint.sh"]
