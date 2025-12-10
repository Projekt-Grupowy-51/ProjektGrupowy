FROM grafana/loki:2.9.6

USER root

# Copy custom loki configuration
COPY loki-config.yml /etc/loki/loki-config.yml

USER loki
