# Site Layout Development Guide

## Overview

This guide is a concise reference for site-layout development. It introduces common placeholders, usage scenarios, and example code to help build page skeletons quickly.

## Usage scenario

Use it when creating or modifying site layouts (page skeletons and placeholders).

## Example

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Document</title>
</head>
<body>
    <div k-placeholder="Main"></div>
    <div k-placeholder="Sider"></div>
</body>
</html>
```

## Required structure

- `k-placeholder="[Custom Name]"`: A Layout must contain at least one page placeholder to identify the page-content injection area, such as `Main` or `Sider`. A Page's `<placeholder id="[Custom Name]">` must match this attribute value character-for-character; matching is case-sensitive, so `Main` and `main` differ. See the `page` reference for detailed usage examples.
- Site-wide chrome (header/footer): by default, reference it once in the Layout through `<view id="site_header">` and `<view id="site_footer">`; **do not** reference a same-named View again inside a Page `<placeholder>`.
- Ordinary Layout elements must not declare `env="server"`; `k-placeholder`, `view`, and template bindings do not depend on an outer server environment.

## Default site-building skeleton (with chrome)

This is the **single source for the Layout skeleton** in Site Building S4 step ①. Other references (including the SOP) only refer to this section and do not repeat the code.

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link rel="stylesheet" href="/kooboo-var.css" />
</head>
<body>
    <view id="site_header"></view>
    <div k-placeholder="Main"></div>
    <view id="site_footer"></view>
</body>
</html>
```
