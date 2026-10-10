# Content Management - Files

## Page Purpose

- Admin page: `/content/files`
- Manages general site file resources, including uploading, creating folders, searching, downloading, and deleting.
- Images and icons displayed on a site are usually better managed in the media library; the file page does not provide the same image metadata and management experience as the media library.

## Paths and Folders

- Files are organized by paths and folders, and the breadcrumb shows the current directory.
- Creating a folder and uploading a file act in the current directory and current storage provider.
- A folder path is part of a resource address. Moving, deleting, or re-uploading under a different path can invalidate existing links.
- Search mode finds files across the current storage provider. You cannot upload or create a folder directly while searching; return to a specific directory first.

## Storage Providers

- The page can switch between the default storage and other configured storage providers.
- Switching providers returns to that provider's root directory; files, directories, pagination behavior, and error states are independent between providers.
- Whether external storage can list, upload, download, or delete depends on the integration configuration and provider capabilities. A configuration error is shown directly as a file-list error.
- A storage provider is not just a filter label; do not assume same-named files in different providers are the same object.

## Uploads and Same-Named Files

- The current frontend prevents a single file larger than 150 MB from continuing to upload; the server, proxy, or external storage may impose stricter limits.
- Before upload, the target path is checked for a same-named file. When one is found, the user must confirm; upload must not be understood as always retaining two copies automatically.
- A successful upload means only that the file was written to the current storage; it does not mean a page, script, or content automatically references it.

## Downloads and Deletion

- Multiple files and folders can be selected for a packaged download. Generating the package can take time when there are many files or external storage responds slowly.
- Deleting a folder and deleting a file are separate operations and act on the current storage provider.
- Check resource references before deletion. Do not assume page links are automatically repaired afterward or that external storage provides a recycle bin.

## Difference from the Media Library

- The file page is suitable for documents, archives, script attachments, and other general files.
- The media library is better for site images and icons and provides media capabilities such as image preview, dimensions, and Alt.
- Uploading an image to the file page does not automatically provide all media-library management behavior or migrate it to the media library.
