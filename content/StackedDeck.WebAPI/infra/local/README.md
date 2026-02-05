# 🖥️ Local Development Infrastructure

This directory contains Docker Compose configurations and supporting files for running observability services locally during development.

//#if (UsePrometheusScrape)
## 📦 Services

| Service | Port | Description |
|---------|------|-------------|
| 🎯 **Prometheus** | `9090` | Metrics collection and storage |

## 🚀 Quick Start

### Prerequisites

- [Docker](https://docs.docker.com/get-docker/)
- [Docker Compose](https://docs.docker.com/compose/install/)

### Usage

Start Prometheus for local metrics collection:

```bash
docker compose up prometheus
```

Access Prometheus UI at: http://localhost:9090

Your application should expose metrics at the `/metrics` endpoint (default: http://localhost:5133/metrics) for Prometheus to scrape.

## 📁 Configuration Files

- `docker-compose.yml` - Service definitions
- `prometheus.yml` - Prometheus scrape configuration (template-generated)

## ⚠️ Important Notes

- ⚡ **Local Use Only**: These configurations are optimized for local development and should not be used in production
- 🗄️ **Data Persistence**: Data is not persisted across container restarts by default
- 🌐 **Network**: Services communicate via the `observability` Docker network

## 🔧 Customization

Modify the configuration files as needed for your local setup:

- Change scrape intervals in `prometheus.yml`
- Adjust target endpoints if your application runs on different ports

## 🐛 Troubleshooting

| Issue | Solution |
|-------|----------|
| Port conflicts | Modify port mappings in `docker-compose.yml` |
| Service not starting | Check logs: `docker compose logs prometheus` |
| No metrics in Prometheus | Verify target is reachable at `http://host.docker.internal:5133/metrics` |
//#endif
//#if (UseOTELCollector)
## 📦 Services

| Service | Port | Description |
|---------|------|-------------|
| 🎯 **Prometheus** | `9090` | Metrics collection and storage |
| 📊 **Grafana** | `3000` | Visualization dashboards |
| 🔍 **Tempo** | `3200` | Distributed tracing backend |
| 📡 **OTEL Collector** | `4317` `4318` `8889` | OpenTelemetry data collection |

## 🚀 Quick Start

### Prerequisites

- [Docker](https://docs.docker.com/get-docker/)
- [Docker Compose](https://docs.docker.com/compose/install/)

### Usage

Start the full observability stack with OTEL Collector:

```bash
docker compose --profile otel up
```

Access the services:
- 📊 **Grafana**: http://localhost:3000 (admin/admin)
- 🎯 **Prometheus**: http://localhost:9090
- 📡 **Collector Metrics**: http://localhost:8889/metrics

Your application will automatically send traces and metrics to the OTEL Collector at `http://localhost:4317` via OTLP/gRPC.

## 📁 Configuration Files

- `docker-compose.yml` - Service definitions with profiles
- `prometheus.yml` - Prometheus configuration (template-generated)
- `otel-collector-config.yml` - Collector receivers, processors, and exporters
- `tempo-config.yml` - Trace storage configuration
- `grafana-datasources.yml` - Pre-configured data sources

## ⚠️ Important Notes

- ⚡ **Local Use Only**: These configurations are optimized for local development and should not be used in production
- 🔒 **Default Credentials**: Grafana uses default credentials (admin/admin) - change these if exposing externally
- 🗄️ **Data Persistence**: Data is not persisted across container restarts by default
- 🌐 **Network**: All services communicate via the `observability` Docker network

## 🔧 Customization

Modify the configuration files as needed for your local setup:

- Change scrape intervals in `prometheus.yml`
- Adjust batch sizes in `otel-collector-config.yml`
- Add custom dashboards in Grafana (persist to volume for reuse)

## 🐛 Troubleshooting

| Issue | Solution |
|-------|----------|
| Port conflicts | Modify port mappings in `docker-compose.yml` |
| Service not starting | Check logs: `docker compose logs <service-name>` |
| No metrics in Prometheus | Verify Collector metrics endpoint is reachable at `:8889/metrics` |
| No traces in Tempo | Check OTLP endpoint configuration in application (should be `http://localhost:4317`) |
//#endif
