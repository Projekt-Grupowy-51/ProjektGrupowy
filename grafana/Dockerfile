FROM grafana/grafana:11.2.0

USER root

# Copy datasource configuration
COPY provisioning/datasources/ /etc/grafana/provisioning/datasources/

# Copy dashboard provider configuration
COPY provisioning/dashboards/provider.yml /etc/grafana/provisioning/dashboards/

# Copy dashboard JSON files
COPY provisioning/dashboards/app/ /etc/grafana/provisioning/dashboards/app/

# Set proper permissions
RUN chown -R grafana:grafana /etc/grafana/provisioning

USER grafana
