# Site Operations - Collaboration Sync

## Page Purpose

- Admin pages include sync connections, pending changes, and the remote-server list.
- A collaboration-sync connection stores the remote server address, remote site, access credential, sync progress, and push/pull ignore scopes. It is not a Git branch or automatic three-way merge system.
- When creating a connection, a new remote site can be created if none is specified; this requires remote reachability, valid credentials, and sufficient permissions.

## Local Changes, Push, and Pull

- Local Changes comes from pending pushes in the current site's operation log relative to sync progress; it is not a complete file-by-file or object-by-object scan of both sites.
- Push really changes the remote site, and Pull really changes the local site. Both loop item by item through changes until the remote returns completion; neither is one atomic transaction that all succeeds or all fails.
- If a midway operation fails, earlier completed items may already be written to the target site. Recheck progress and remaining changes before retrying; do not assume the entire batch rolled back automatically.
- Successful push or pull proves only that the sync protocol processed the corresponding changes; it does not mean the target site is published, external dependencies work, or frontend business results are correct.

## Conflicts and Ignores

- The conflict page shows local and remote users, times, and text or image content for choosing a handling mode.
- Force Push, Force Pull, and Ignore Current choose the local side, remote side, or mark the current conflict record as handled. They do not automatically generate a three-way merge result.
- Ignore logs skip synchronization by object or log and can later be unignored. Ignoring does not delete the local object or undo content already synchronized to the other side.
- Push-ignore types and pull-ignore types are independent directions. Configuring one direction does not automatically affect the other.

## Settings Sync and Progress

- Site-setting differences have separate entry points from content or object changes; Pull Settings and Push Settings must be run separately, and ordinary object sync does not prove settings match.
- Sync progress can be adjusted manually, but it is only a cursor, not a data backup. Moving forward can skip unprocessed historical changes; moving backward can make old changes appear again.
- A remote difference of 0 does not necessarily prove there are no remote changes; it may also appear when the remote is unreachable or the request fails. Check connection status and credentials as well.

## Credentials, Servers, and Deletion

- A collaboration Bearer is an access credential with a role and expiration. Importing credential JSON establishes a connection; it does not permanently add the other party to the current organization or mean the credential never expires.
- The Server list stores reusable remote-server configuration. Deleting a server entry does not shut down the remote service, delete the remote site, or revoke content written by existing sync connections.
- Deleting a sync connection removes only local connection settings; it does not delete the remote site, roll back synchronized objects, or restore data previously overwritten.
- Confirm direction, target site, ignore scope, and current progress before push or pull. After synchronizing a production site, independently verify publication status and key frontend paths.
