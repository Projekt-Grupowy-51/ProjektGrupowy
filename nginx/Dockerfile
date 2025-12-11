FROM nginx:alpine

RUN apk add --no-cache gettext

RUN rm /etc/nginx/conf.d/default.conf

COPY nginx.conf.template /etc/nginx/nginx.conf.template

RUN mkdir -p /mnt/videos

COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

# Send Nginx logs to the Docker log collector
RUN ln -sf /dev/stdout /var/log/nginx/access.log \
 && ln -sf /dev/stderr /var/log/nginx/error.log

ENTRYPOINT ["/entrypoint.sh"]
