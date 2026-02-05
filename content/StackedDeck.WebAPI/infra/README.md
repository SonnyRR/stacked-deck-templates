# 🏗️ Infrastructure

This directory contains Infrastructure as Code (IaC) definitions for deploying and
managing the application's infrastructure resources.

## 📁 Directory Structure

```txt
infra/
├── local/          # 🖥️ Local development environment resources
└── (extensible)    # Add your environment-specific IaC here
```

## 🎯 Purpose

This directory is designed to be extensible. You can add subdirectories for different
environments or IaC tools:

- **Terraform** configurations for cloud resources
- **Pulumi** projects for infrastructure
- **Azure Bicep** templates
- **AWS CloudFormation** templates
- **Ansible** playbooks
- **Helm** charts for Kubernetes

## 🚀 Getting Started

Choose the appropriate subdirectory for your target environment:

| Directory | Purpose |
|-----------|---------|
<!--#if(UsePrometheusScrape)-->
| `local/` | Local development with Prometheus via Docker Compose |
<!--#endif-->
<!--#if(UseOTELCollector)-->
| `local/` | Local development with full observability stack via Docker Compose |
<!--#endif-->
| (your-env)/ | Add your staging/production IaC here |

## 📝 Guidelines

- Keep environment-specific configurations in separate subdirectories
- Use environment variables or configuration files for sensitive data
- Document any prerequisites in the respective subdirectory README
- Consider using the same naming conventions across environments for consistency
