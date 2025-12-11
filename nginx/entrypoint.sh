#!/bin/sh

# Specify only the variables to substitute
envsubst '${NGINX_PORT} ${NGINX_HOST} ${NGINX_SECURELINK_SECRET}' < /etc/nginx/nginx.conf.template > /etc/nginx/nginx.conf

# delete comment
exec nginx -g 'daemon off;'
