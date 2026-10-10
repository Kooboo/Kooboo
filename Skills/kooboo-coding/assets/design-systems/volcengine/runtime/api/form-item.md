# YlFormItem

Import and register `{ YlFormItem }` from `/yuanli.js` after installation.

Props: label and for required; required=false; error, success, hint optional. Precedence: error > success > hint. Default scoped slot receives id, aria-describedby, aria-invalid and required; bind these to the control. Also pass invalid to YlInput/YlSelect for visual validation. Error/help text is associated with the control; no automatic schema validator or events.
