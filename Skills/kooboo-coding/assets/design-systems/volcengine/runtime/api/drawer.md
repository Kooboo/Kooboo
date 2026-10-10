# YlDrawer

Import and register `{ YlDrawer }` from `/yuanli.js` after installation.

Props: modelValue=false, title required, busy=false, size=md (mini/sm/md/lg → 480/600/720/1080px, viewport bounded). Right-side native modal drawer. update:modelValue(false) requests closing; busy guards Escape and close controls. Default body and footer({close}) slots match YlDialog; native focus containment/return is preserved. Caller owns async work and errors.
