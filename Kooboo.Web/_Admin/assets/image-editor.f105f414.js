var V=Object.defineProperty,W=Object.defineProperties;var _=Object.getOwnPropertyDescriptors;var M=Object.getOwnPropertySymbols;var z=Object.prototype.hasOwnProperty,H=Object.prototype.propertyIsEnumerable;var C=(o,i,r)=>i in o?V(o,i,{enumerable:!0,configurable:!0,writable:!0,value:r}):o[i]=r,k=(o,i)=>{for(var r in i||(i={}))z.call(i,r)&&C(o,r,i[r]);if(M)for(var r of M(i))H.call(i,r)&&C(o,r,i[r]);return o},x=(o,i)=>W(o,_(i));var g=(o,i,r)=>new Promise((t,d)=>{var f=l=>{try{n(r.next(l))}catch(p){d(p)}},e=l=>{try{n(r.throw(l))}catch(p){d(p)}},n=l=>l.done?t(l.value):Promise.resolve(l.value).then(f,e);n((r=r.apply(o,i)).next())});import{d as J,M as K,cd as L,g as v,x as $,E as P,c as j,h as X,o as b,a as q,F,j as S,u as m,k as N,c0 as Y}from"./url.8f5ec20c.js";import{u as Z,e as h}from"./main.582f9de6.js";/* empty css                                                               */import{j as ee,a as te}from"./media-list.vue_vue_type_style_index_0_scoped_true_lang.c0b84b74.js";/* empty css                                                          */import"./file.734aeed6.js";import"./validate.238a9986.js";import"./use-date.a1321c18.js";import"./i18n.bcd18f8a.js";import"./style.9c8f6403.js";import"./use-copy-text.c117d066.js";/* empty css                                                          *//* empty css                                                                 */import"./confirm.eadb49f1.js";import"./use-file-upload.c73251ef.js";/* empty css                                                         */import"./image-editor.vue_vue_type_style_index_0_scoped_true_lang.53a9e945.js";import{I as ae}from"./image-editor.f97cfa84.js";import{_ as re}from"./index.5f76a3ad.js";import{S as ie}from"./index.f7600a46.js";import{u as oe}from"./use-save-tip.8f44d6c0.js";import"./index.50c16ae5.js";import"./windi.19264205.js";import"./preload-helper.13a99eb0.js";import"./replace-all.d441bf14.js";import"./index.2341329b.js";import"./index.bda83f28.js";import"./index.bff48780.js";import"./index.649f6c77.js";import"./index.a32fb6e5.js";import"./index.6d63eb53.js";import"./error.7e8331f1.js";import"./index.05e21f33.js";import"./_baseClone.eeff2792.js";import"./_baseIsEqual.547729d3.js";import"./isEqual.11d86bcc.js";import"./dayjs.min.59f10137.js";import"./toNumber.6efebd6a.js";import"./logo-transparent.1566007e.js";import"./index.10f642b2.js";import"./index.aec72f69.js";import"./index.5cbbc5d7.js";import"./event.776e7e11.js";import"./isNil.98bb3b88.js";import"./index.a3d8335f.js";import"./scroll.4888a9e9.js";import"./aria.75ec5909.js";import"./event.53b2ad83.js";import"./focus-trap.eafcfd1f.js";import"./validator.b73911a9.js";import"./index.c80f5028.js";import"./plugin-vue_export-helper.21dcd24c.js";import"./dark.166fd971.js";const lt=J({setup(o){const i=K(),r=L(),t=v(),d=$("id"),f=$("provider"),e=P({id:d,alt:"",base64:"",provider:f,name:""}),n=v(""),l=v(""),p=j(()=>`${e.name}${l.value}`),c=v(!1),U=v(),R=Z(),y=v(),E=oe(void 0,{defaultActiveMenu:"editImage",modelGetter:()=>e});function T(){return g(this,null,function*(){var s,w,B;t.value=yield ee({id:d,provider:f});let u=(s=t.value.name)!=null?s:"";const a=u.lastIndexOf(".");if(a>0&&(l.value=u.substring(a),u=u.substring(0,a)),e.name=u,t.value.alt=t.value.alt||"",e.alt=t.value.alt,e.base64=(w=t.value.base64)!=null?w:"",n.value=(B=t.value.base64)!=null?B:"",e.provider=f,c.value=t.value.url.endsWith(".svg"),c.value)if(t.value.svg)U.value=t.value.svg;else{const O=Y(R.site.prUrl,t.value.siteUrl||t.value.url),Q=yield h.get(O);U.value=Q.data}E.init(e)})}X(()=>{T()});const A=j(()=>t.value?e.base64!==n.value||e.alt!==t.value.alt||p.value!==t.value.name||c.value:!1);function I(){r.push({name:"media",query:x(k({},i.query),{id:void 0})})}function D(){return g(this,null,function*(){var u;if(c.value){const a=y.value.getSvgString();e.base64=a?window.btoa(a):""}else e.base64===n.value&&(e.base64="");(u=y.value)==null||u.validate(a=>g(this,null,function*(){!a||(yield te(x(k({},e),{name:p.value})),E.init(e),I())}))})}function G(){return g(this,null,function*(){if(!t.value)return!1;I()})}return(u,a)=>(b(),q(F,null,[t.value?(b(),q(F,{key:0},[c.value?(b(),S(ie,{key:0,ref_key:"imageEditor",ref:y,alt:m(e).alt,"onUpdate:alt":a[0]||(a[0]=s=>m(e).alt=s),name:m(e).name,"onUpdate:name":a[1]||(a[1]=s=>m(e).name=s),svg:U.value,ext:l.value,url:t.value.url,"site-url":t.value.siteUrl},null,8,["alt","name","svg","ext","url","site-url"])):(b(),S(m(ae),{key:1,ref_key:"imageEditor",ref:y,alt:m(e).alt,"onUpdate:alt":a[2]||(a[2]=s=>m(e).alt=s),base64:m(e).base64,"onUpdate:base64":a[3]||(a[3]=s=>m(e).base64=s),name:m(e).name,"onUpdate:name":a[4]||(a[4]=s=>m(e).name=s),ext:l.value,url:t.value.url,"site-url":t.value.siteUrl},null,8,["alt","base64","name","ext","url","site-url"]))],64)):N("",!0),t.value?(b(),S(re,{key:1,disabled:!m(A),onSave:D,onCancel:G},null,8,["disabled"])):N("",!0)],64))}});export{lt as default};