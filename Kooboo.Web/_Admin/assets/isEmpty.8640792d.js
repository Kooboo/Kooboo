import{c4 as i,bq as o,bX as e,b_ as n,c1 as f,bT as p,c9 as a}from"./url.8f5ec20c.js";import{d as y}from"./_baseIsEqual.547729d3.js";var c="[object Map]",b="[object Set]",g=Object.prototype,m=g.hasOwnProperty;function j(r){if(r==null)return!0;if(i(r)&&(o(r)||typeof r=="string"||typeof r.splice=="function"||e(r)||n(r)||f(r)))return!r.length;var t=y(r);if(t==c||t==b)return!r.size;if(p(r))return!a(r).length;for(var s in r)if(m.call(r,s))return!1;return!0}export{j as i};