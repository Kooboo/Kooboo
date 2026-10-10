# YlDescriptions

Import and register `{ YlDescriptions }` from `/yuanli.js` after installation.

Props: title optional, items=[] with {key,label,value}, layout=stacked (stacked/label-left/label-right/inline/inline-colon). Source stacked, aligned and inline layouts use two columns where applicable and one on narrow screens. item-{key} slots receive {item}. Plain values are escaped; use a slot for components. No events.
