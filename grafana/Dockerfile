FROM grafana/grafana:11.2.0

# Copy datasource configuration
COPY provisioning/datasources/ /etc/grafana/provisioning/datasources/

# Copy dashboard provider configuration
COPY provisioning/dashboards/provider.yml /etc/grafana/provisioning/dashboards/

# Copy dashboard JSON files
COPY provisioning/dashboards/app/ /etc/grafana/provisioning/dashboards/app/
