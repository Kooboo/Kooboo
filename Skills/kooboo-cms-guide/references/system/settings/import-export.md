# Kooboo Site Package Import and Export

## Import Site Packages

- Import applies site-package content to the current site and may add, update, or replace existing objects.
- Confirm target site, package source, and included scope before importing; for important sites, export a recoverable version first.
- Successful import does not mean all external dependencies are restored. External databases, object storage, payment services, callback addresses, domains, and certificates may still need separate configuration.

## Export Sites

- Full export generates a more complete migratable site package and offers copy modes for different site sizes.
- Custom export selects only specific resource types such as pages, styles, scripts, or configuration.
- Ordinary mode is subject to site-package size limits; larger sites may need full mode.

## Judging Backup or Migration Readiness

- Successful file download proves only that a site package was generated.
- To claim migration is complete, import the package into the target environment and check pages, resources, data, permissions, and key business flows.
- For payments, login, email, SMS, external databases, or storage services, also verify service configuration and real call results.
- A custom export contains only selected resources and must not be described as a complete backup.
