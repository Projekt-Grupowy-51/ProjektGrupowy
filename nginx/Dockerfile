FROM nginx:alpine

# Remove default config
RUN rm /etc/nginx/conf.d/default.conf

# Copy our custom config
COPY nginx.conf /etc/nginx/nginx.conf

# Create mount point directory (doesn't create volume itself)
RUN mkdir -p /mnt/videos
