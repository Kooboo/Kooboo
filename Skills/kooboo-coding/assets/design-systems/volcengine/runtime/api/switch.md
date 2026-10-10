# YlSwitch

Import and register `{ YlSwitch }` from `/yuanli.js` after installation.

Props: modelValue=false, disabled=false, loading=false, label required (accessible name), size=md (md/sm). Emits update:modelValue(boolean) on native click/keyboard; disabled/loading block changes. Provide visible label text beside the switch. The caller confirms business-sensitive changes and updates state after the API succeeds. No slots.
