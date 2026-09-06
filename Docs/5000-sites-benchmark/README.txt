Kooboo 5,000-site benchmark release
====================================

Files in the GitHub release:

- kooboo-5000-dynamic-sites.zip
  Import this package into a Kooboo instance. It is the dynamic blog website
  used as the source package when the 5,000 separate sites were provisioned.

- kooboo-stress-win-x64.zip
  Self-contained Windows x64 command-line test client. Extract it and run:
  kooboo-stress.exe

- kooboo-stress-linux-x64.tar.gz
  Self-contained Linux x64 command-line test client. Extract it and run:
  chmod +x kooboo-stress
  ./kooboo-stress

- kooboo-stress-source.zip
  C# source code and publishing scripts for the test client. The project targets
  .NET 10, declares AOT compatibility, and can also be published as NativeAOT
  on the target operating system.

- final150-target-results.tar.gz
  Target-server information and monitoring records.

- final150-loadgen-results.tar.gz
  Load-generator information and console output.

- final150-detailed-results.tar.gz
  Request-level CSV and exact test summary.

- SHA256SUMS.txt
  SHA-256 checksum for every individual release artifact.

Safety
------

Only run the stress-test client against infrastructure that you own or have
explicit permission to test. The public client limits tests to 200 request
starts per second and 20 minutes, but these client-side limits do not replace
authorization or server-side protection.

The Windows and Linux packages are self-contained; a separate .NET runtime is
not required on the test machine.
