# YlAvatar

Import and register `{ YlAvatar }` from `/yuanli.js` after installation.

Props: name required, src optional; size=default (mini/default/medium/large → 16/20/24/40px); color=primary (red/beige/green/teal/purple/primary/grey); bordered=false adds the source white icon border; iconOnly=false; capsule=false; kind=person (person/department); subtitle optional; editable=false; disabled=false. Image failure falls back to the first Unicode name character. editable renders a keyboard-accessible button and emits edit(); caller owns the editing UI. iconOnly retains an accessible name. No slots.
