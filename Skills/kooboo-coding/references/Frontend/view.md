# Site Component Development Guide

## Overview

This guide is a concise reference for site-component development. It introduces common structures, usage scenarios, and example code to help build reusable components quickly.

## Usage scenario

Use it when creating or modifying site components (including Views and placeholders).

## Example

### Component example

```html
<div>
    <h1>Welcome</h1>
    <view id="other_view"></view>
</div>
```

## Features

- `k-data` and `k-xx` bindings do not require `env="server"` on the View's outer element; use `<script env="server">` only for actual server-side scripts.
- A View should contain one reusable page block only. It must not wrap `<layout>`, `<placeholder>`, a complete `<html>` / `<body>`, or the whole-site page shell again.
- Shared Views such as Header / Footer are referenced once by the Layout by default; page-level Views such as Hero, Features, and forms are orchestrated by the Page's `<placeholder>`. Do not reference the same View repeatedly in Layout and Page.
- If a View does not appear in a Page, first check Page / Layout assembly and run [Preflight](../Preflight/index.md): `PF-KOBOO-TEMPLATE-003/005/007/010/013/014` cover assembly issues, and `PF-KOBOO-RESOURCE-001` covers a missing object targeted by `<view id>`. When the user explicitly selects `/debug`, read the corresponding Debug reference from the structured `playbook` returned by the finding.
