# YlForm

Import and register `{ YlForm }` from `/yuanli.js` after installation.

Props: busy=false, title optional, columns=1 (1/2). Native form emits submit(event) after preventing native navigation, suppressed while busy. Default slot contains fields; actions slot sits below the grid. Two columns stack on narrow screens. Native constraints apply to native inputs; caller validates custom controls and owns requests. Use YlFormGroup for labeled groups.
