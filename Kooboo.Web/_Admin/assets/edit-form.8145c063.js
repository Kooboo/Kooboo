var C=Object.defineProperty;var V=Object.getOwnPropertySymbols;var U=Object.prototype.hasOwnProperty,S=Object.prototype.propertyIsEnumerable;var w=(n,s,a)=>s in n?C(n,s,{enumerable:!0,configurable:!0,writable:!0,value:a}):n[s]=a,x=(n,s)=>{for(var a in s||(s={}))U.call(s,a)&&w(n,a,s[a]);if(V)for(var a of V(s))S.call(s,a)&&w(n,a,s[a]);return n};var y=(n,s,a)=>new Promise((d,m)=>{var O=l=>{try{e(a.next(l))}catch(t){m(t)}},f=l=>{try{e(a.throw(l))}catch(t){m(t)}},e=l=>l.done?d(l.value):Promise.resolve(l.value).then(O,f);e((a=a.apply(n,s)).next())});import{g as $}from"./i18n.48bd28ac.js";import{b as A,e as B}from"./product.c1a83798.js";import{u as P}from"./commerce.f8d3336c.js";import{_ as z,a as M,V as j}from"./option-group-editor.fe5eab36.js";import{_ as q}from"./key-value-editor.b270c5cd.js";import{u as G}from"./product-variant.f9d76101.js";import{_ as H}from"./index.67130863.js";import{z as J}from"./main.ea42d807.js";import{u as K}from"./useFields.2c414714.js";import{E as L,a as Q}from"./index.6855d037.js";import{d as R,g as _,h as T,o as W,a as X,b as u,w as b,u as o,f as Y,k as Z}from"./url.2e6a77c4.js";const tt={key:0,class:"px-24 pt-0 pb-84px space-y-12"},at={class:"bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal"},vt=R({props:{id:null},setup(n,{expose:s}){const a=n,d=_([]),{t:m}=$(),O=P(),f=_();O.loadCategories();let e=G();const l=_(),t=_(),{fields:E,customFields:D}=K();T(()=>y(this,null,function*(){t.value=yield A(a.id),t.value.tags||(t.value.tags=[]),d.value=t.value.attributes.map(i=>({key:i.key,value:i.value,options:[]}));const c=t.value.variants.sort((i,p)=>i.order>p.order?1:-1);for(const i of c)e.addVariant(i)}));function F(){return y(this,null,function*(){var i,p;try{yield f.value.form.validate(),yield(p=(i=l.value)==null?void 0:i.form)==null?void 0:p.validate()}catch(r){throw J(),r}const c=x({},t.value);c.attributes=d.value.map(r=>({key:r.key,value:r.value})),e.list.value.forEach((r,v)=>r.order=v),c.variants=e.list.value,yield B(c)})}return s({onSave:F}),(c,i)=>{const p=L,r=Q;return t.value?(W(),X("div",tt,[u(z,{ref_key:"basicInfo",ref:f,model:t.value,fields:o(E)},{default:b(()=>[u(p,{label:o(m)("common.attributes")},{default:b(()=>[u(q,{modelValue:d.value,"onUpdate:modelValue":i[0]||(i[0]=v=>d.value=v),"key-input-attributes":{placeholder:o(m)("common.attributeSamples")},"value-input-attributes":{placeholder:o(m)("common.value")},class:"max-w-600px space-y-8 w-full"},null,8,["modelValue","key-input-attributes","value-input-attributes"])]),_:1},8,["label"])]),_:1},8,["model","fields"]),u(H,{ref_key:"customData",ref:l,data:t.value.customData,"custom-fields":o(D)},null,8,["data","custom-fields"]),Y("div",at,[u(r,{"label-position":"top"},{default:b(()=>{var v,g;return[u(p,{label:o(m)("commerce.variantOptions")},{default:b(()=>[u(M,{options:o(e).options.value,variants:o(e).list.value,"variant-options":t.value.variantOptions,class:"max-w-600px",onUpdateOptionItem:o(e).updateOptionItem,onUpdateOptionName:o(e).updateOptionName,onAddOptionItem:i[1]||(i[1]=(h,N)=>{var I,k;return o(e).addOptionItem(h,N,(k=(I=t.value)==null?void 0:I.featuredImage)!=null?k:"")}),onDeleteOptionItem:o(e).deleteOptionItem,onDeleteOption:o(e).deleteOption,onAddOption:o(e).addOption},null,8,["options","variants","variant-options","onUpdateOptionItem","onUpdateOptionName","onDeleteOptionItem","onDeleteOption","onAddOption"])]),_:1},8,["label"]),u(j,{"variant-options":t.value.variantOptions,variants:o(e).list.value,options:o(e).options.value,"default-image":(g=(v=t.value)==null?void 0:v.featuredImage)!=null?g:"","is-digital":t.value.isDigital},null,8,["variant-options","variants","options","default-image","is-digital"])]}),_:1})])])):Z("",!0)}}});export{vt as _};