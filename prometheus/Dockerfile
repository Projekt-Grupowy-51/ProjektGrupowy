FROM prom/prometheus:v2.54.1

USER root

# Copy custom prometheus configuration
COPY prometheus.yml /etc/prometheus/prometheus.yml

# Set proper permissions
RUN chown -R prometheus:prometheus /etc/prometheus

USER prometheus
