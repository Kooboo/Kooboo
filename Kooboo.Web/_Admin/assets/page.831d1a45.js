import{cg as u,g as m,U as i,ci as y}from"./url.2e6a77c4.js";import{d as T}from"./i18n.48bd28ac.js";import{a as w}from"./validate.efc4a5c0.js";import{E as A}from"./index.99c4f65d.js";import{a as E,E as x}from"./index.6855d037.js";import{E as I}from"./index.fc90f5ad.js";import{e as C}from"./guid.c1a40312.js";import{c as d,e as P}from"./style.19d0c187.js";import{u as N}from"./replace-all.7cf5f327.js";function D(e){const t=window.decodeURIComponent(e).matchAll(/\{([^\/\}]+)\}/g),o=[];for(const n of t)n[1]&&o.push(n[1]);return o}function L(e,t){const o=Object.keys(e.value),n=m({});o.forEach(r=>{n.value[r]=[w(t("common.valueRequiredTips"))]});const s=o.map(r=>i(x,{prop:r,label:r},{default:()=>i(I,{modelValue:e.value[r],"onUpdate:modelValue"(a){e.value[r]=a}})}));return i(E,{model:e,rules:n.value,labelPosition:"top",class:"el-form--label-normal"},{default:()=>s})}function G(){const e=T.global.t;function t(o,n){var a;const s=D(o);if(!s.length){u(o);return}const r=m({});s.forEach(c=>{r.value[c]=`{${c}}`}),A({title:(a=n==null?void 0:n.title)!=null?a:e("common.preview"),message:L(r,e),showCancelButton:!0,confirmButtonText:e("common.goTo"),cancelButtonText:e("common.cancel"),roundButton:!0,customClass:"el-message-box--preview font-family",beforeClose:(c,g,l)=>{if(c==="confirm"){const{component:b}=g.message;b.exposed.validate(v=>{!v||(u(y(window.decodeURIComponent(o),r.value)),l())});return}l()}}).catch(()=>{})}return{onPreview:t}}const M=4,$=7;function f(e){var r;const t=document.createElement("div");for(const a in e.attributes){const c=(r=e.attributes[a])!=null?r:"";t.setAttribute(a,c)}const o=t.outerHTML,n=o.substring(M,o.length-$);let s=`<${e.name}${n}>`;if(!e.children)s+=`</${e.name}>`;else{if(typeof e.children=="string")s+=e.children;else for(const a of e.children)s+=`${f(a)}`;s+=`</${e.name}>`}return s}const B=e=>{const t=e.getAttribute("k-placeholder");return t?{name:t,innerHtml:e.innerHTML,addons:[]}:void 0};function p(e){const t={id:"",attributes:{},content:"",type:e.tagName.toLowerCase()};for(const o of e.getAttributeNames()){const n=o.toLowerCase();t.attributes[n]=e.getAttribute(n)}if(t.id=t.attributes.id,t.type==="layout"){const o=[];for(const n of Array.from(e.children)){if(n.tagName.toLowerCase()!=="placeholder")continue;const s={name:n.getAttribute("id"),addons:[],innerHtml:""};for(const r of Array.from(n.children))s.addons.push(p(r));o.push(s)}t.content=o}else t.content=e.innerHTML;return t}function O(e,t){const o=d(t),n=[];for(const s of P(o)){const r=B(s);r&&n.push(r)}return{id:e,type:"layout",attributes:{id:e},content:n}}function h(e){const t={name:e.type,attributes:e.attributes,children:[]};if(e.content&&typeof e.content!="string")for(const o of e.content){const n={name:"placeholder",attributes:{id:o.name},children:[]};for(const s of o.addons)n.children.push(h(s));t.children.push(n)}else t.children=e.content;return t}function S(e){return f(h(e))}function z(e){const o=d(e).body.children[0];return p(o)}function J(e,t){const o=t&&t!==C?t:void 0;return N({name:o?"layout-page-design":"page-design",query:{id:e,layoutId:o}})}export{S as a,p as e,J as g,O as l,z as p,G as u};