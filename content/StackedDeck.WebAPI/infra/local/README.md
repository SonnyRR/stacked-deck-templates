# 🖥️ Local Development Infrastructure

This directory contains the Docker Compose configuration for running a
single-container observability stack locally during development, based on the
[`grafana/otel-lgtm`](https://github.com/grafana/docker-otel-lgtm) image
(Grafana + Prometheus + Tempo + Loki + Pyroscope + OpenTelemetry Collector).
<!--#if(UsePrometheusScrape)-->

## 📦 Services

| Service | Port   | Description                                        |
| ------- | ------ | -------------------------------------------------- |
| 📊 **Grafana**    | `3000` | Visualization dashboards (datasources pre-provisioned) |
| 🎯 **Prometheus** | `9090` | Metrics collection and storage (scrapes the API)   |
| 📡 **OTLP**       | `4317` `4318` | OpenTelemetry ingestion (gRPC / HTTP)       |

## 🚀 Quick Start

### 📝 Prerequisites

- [Docker](https://docs.docker.com/get-docker/)
- [Docker Compose](https://docs.docker.com/compose/install/)

### 🪛 Usage

Start the observability stack:

```bash
docker compose up
```

Access the services:

- 📊 **Grafana**: <http://localhost:3000> (`admin`/`admin`)
- 🎯 **Prometheus**: <http://localhost:9090>

Your application should expose metrics at the `/metrics` endpoint
(default: <http://localhost:sd-api-http-port/metrics>) for Prometheus to scrape.

> [!NOTE]
> Prometheus scrapes your API via `host.docker.internal:sd-api-https-port`. This works automatically
> on macOS and Windows. Linux users may need to add the `--add-host host.docker.internal:host-gateway`
> flag when running Docker Compose, or alternatively bind the API to all interfaces
> (`https://*:sd-api-https-port`).

### ⚙️ Configuration

- `docker-compose.yml` — Service definitions (ports & credentials via env vars)
- `.env.example` — Documented environment variables with defaults; copy to `.env` to customize
- `prometheus.yml` — Prometheus scrape configuration (template-generated)

## ⚠️ Important Notes

- ⚡ **Local Use Only**: These configurations are optimized for local development and should not be used in production
- 🔒 **Default Credentials**: Grafana uses default credentials (`admin`/`admin`) — change these via `.env` if exposing externally
- 🗄️ **Data Persistence**: Telemetry data is persisted in the `lgtm-data` Docker volume across container restarts
- 🌐 **Network**: Services communicate via the `observability` Docker network
- 🐘 **Footprint**: The single container runs all backends and needs roughly 1 GB+ of memory

## 🔧 Customization

- Change ports or Grafana credentials in `.env` (see `.env.example` for the available variables)
- Adjust the image version via `SD_LGTM_IMAGE_TAG` in `.env`
- Tweak scrape targets/intervals in `prometheus.yml`
  > [!NOTE]
  > `prometheus.yml` overrides the image's default Prometheus configuration — when bumping
  > `SD_LGTM_IMAGE_TAG`, re-sync the non-scrape sections with the
  > [upstream default](https://github.com/grafana/docker-otel-lgtm/blob/main/docker/prometheus.yaml)

## 🐛 Troubleshooting

| Issue                    | Solution                                                                                              |
| ------------------------ | ----------------------------------------------------------------------------------------------------- |
| Port conflicts           | Change the port mappings via `.env` (e.g., `SD_LGTM_GRAFANA_PORT=3001`)                               |
| Service not starting     | Check logs: `docker compose logs lgtm` (enable per-backend logs via `ENABLE_LOGS_*` env vars)          |
| No metrics in Prometheus | Verify target is reachable at `http://host.docker.internal:sd-api-https-port/sd-api-route-prefix/metrics` |
<!--#endif-->
<!--#if(UseOTELCollector)-->

## 📦 Services

| Service | Port   | Description                                        |
| ------- | ------ | -------------------------------------------------- |
| 📊 **Grafana**    | `3000` | Visualization dashboards (datasources pre-provisioned) |
| 🎯 **Prometheus** | `9090` | Metrics collection and storage (receives OTLP push) |
| 🔍 **Tempo**      | —      | Distributed tracing backend (internal, no ports exposed) |
| 📡 **OTLP**       | `4317` `4318` | OpenTelemetry ingestion (gRPC / HTTP)       |

## 🚀 Quick Start

### 📝 Prerequisites

- [Docker](https://docs.docker.com/get-docker/)
- [Docker Compose](https://docs.docker.com/compose/install/)

### 🪛 Usage

Start the observability stack:

```bash
docker compose up
```

Access the services:

- 📊 **Grafana**: <http://localhost:3000> (`admin`/`admin`)
- 🎯 **Prometheus**: <http://localhost:9090>

Your application will automatically send traces and metrics to the OpenTelemetry
receiver at `http://localhost:4317` via OTLP/gRPC — no collector or backend
configuration required.

### ⚙️ Configuration

- `docker-compose.yml` — Service definitions (ports & credentials via env vars)
- `.env.example` — Documented environment variables with defaults; copy to `.env` to customize

## ⚠️ Important Notes

- ⚡ **Local Use Only**: These configurations are optimized for local development and should not be used in production
- 🔒 **Default Credentials**: Grafana uses default credentials (`admin`/`admin`) — change these via `.env` if exposing externally
- 🗄️ **Data Persistence**: Telemetry data is persisted in the `lgtm-data` Docker volume across container restarts
- 🌐 **Network**: Services communicate via the `observability` Docker network
- 🐘 **Footprint**: The single container runs all backends and needs roughly 1 GB+ of memory

## 🔧 Customization

- Change ports or Grafana credentials in `.env` (see `.env.example` for the available variables)
- Adjust the image version via `SD_LGTM_IMAGE_TAG` in `.env`
- Add custom Grafana dashboards by mounting provisioning files (see the
  [`otel-lgtm` docs](https://github.com/grafana/docker-otel-lgtm#add-custom-dashboards))

## 🐛 Troubleshooting

| Issue                    | Solution                                                                                     |
| ------------------------ | -------------------------------------------------------------------------------------------- |
| Port conflicts           | Change the port mappings via `.env` (e.g., `SD_OTLP_GRPC_PORT=4337`)                          |
| Service not starting     | Check logs: `docker compose logs lgtm` (enable per-backend logs via `ENABLE_LOGS_*` env vars) |
| No metrics in Grafana    | Verify the app is sending OTLP to `http://localhost:4317` and check `docker compose logs lgtm` |
| No traces in Tempo       | Check OTLP endpoint configuration in application (should be `http://localhost:4317`)          |
<!--#endif-->
