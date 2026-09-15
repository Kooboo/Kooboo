Kooboo Under-1-ms Render Benchmark Client
==========================================

This package contains the command-line client used to measure Kooboo's
server-render timing separately from network and complete-response timing.

Windows x64
-----------

Extract the ZIP archive and run:

  kooboo-stress.exe --help

Linux x64
---------

Extract the tar.gz archive and run:

  chmod +x kooboo-stress
  ./kooboo-stress --help

Official render workloads
-------------------------

One worker:

  ./kooboo-stress --mode render --sites 5000 --requests 100000 --workers 1 --warmup-per-site 1 --url-template "https://site1.trykooboo.com/" --host-template "site{0}.trykooboo.com" --expect "Site {0}" --output ./results/render-official-w1

Two workers:

  ./kooboo-stress --mode render --sites 5000 --requests 100000 --workers 2 --warmup-per-site 1 --url-template "https://site1.trykooboo.com/" --host-template "site{0}.trykooboo.com" --expect "Site {0}" --output ./results/render-official-w2

The benchmark page dynamically queries and renders ten blog items. Page cache
and full-page output cache must be disabled. A reusable Header View may use
Kooboo's cache-by-purpose feature, as it did in the published workload.

The same dynamic 10-blog site package was used for the earlier 5,000-site
capacity benchmark and for this under-1-ms rendering benchmark.

Build information
-----------------

The Windows x64 executable is self-contained NativeAOT. The Linux x64
executable is self-contained, single-file, trimmed, and ReadyToRun. Neither
package requires a separately installed .NET runtime. The project targets
.NET 10 and declares AOT compatibility.

Kooboo Server is not included. Download the latest Kooboo release from:

  https://github.com/Kooboo/Kooboo/releases/latest

Safety
------

Only run this client against infrastructure that you own or have explicit
permission to test.
