var u=(s,m,t)=>new Promise((i,r)=>{var p=o=>{try{e(t.next(o))}catch(a){r(a)}},c=o=>{try{e(t.throw(o))}catch(a){r(a)}},e=o=>o.done?i(o.value):Promise.resolve(o.value).then(p,c);e((t=t.apply(s,m)).next())});import{_ as f}from"./index.5f76a3ad.js";import{d,cd as _,g as B,o as g,a as v,b as n,u as l,F as b}from"./url.8f5ec20c.js";import{B as h}from"./breadcrumb.edaec300.js";import{g as k}from"./i18n.bcd18f8a.js";import{c as y}from"./discount.d9c45514.js";import{u as D}from"./replace-all.d441bf14.js";import{_ as x}from"./edit-form.dfb03b81.js";import"./index.aec72f69.js";import"./windi.19264205.js";import"./index.05e21f33.js";import"./main.582f9de6.js";import"./index.50c16ae5.js";import"./preload-helper.13a99eb0.js";/* empty css                                                               */import"./plugin-vue_export-helper.21dcd24c.js";import"./plugin-vue_export-helper.41ffa612.js";import"./index.79f78425.js";import"./error.7e8331f1.js";import"./condition.e2f7dc08.js";import"./icon-button.315b6443.js";import"./index.ec6ad7db.js";import"./focus-trap.eafcfd1f.js";import"./isNil.98bb3b88.js";import"./event.53b2ad83.js";import"./index.3a977dfb.js";import"./index.c80f5028.js";import"./index.59b1471f.js";import"./event.776e7e11.js";import"./index.6d63eb53.js";import"./_baseClone.eeff2792.js";import"./_baseIsEqual.547729d3.js";import"./isEqual.11d86bcc.js";import"./index.6d22f937.js";import"./index.5cbbc5d7.js";import"./scroll.4888a9e9.js";import"./debounce.730e1961.js";import"./toNumber.6efebd6a.js";import"./index.9a83ee01.js";import"./validator.b73911a9.js";import"./guid.c1a40312.js";import"./index.0a8d3d17.js";import"./index.e3f90979.js";import"./index.d6799c63.js";import"./dayjs.min.59f10137.js";import"./index.373c837a.js";const vo=d({setup(s){const{t:m}=k(),t=_(),i=B({title:"",code:"",condition:{isAny:!1,items:[]},endDate:"",startDate:"",isPercent:!1,method:"AutomaticDiscount",type:"ProductAmountOff",value:10,priority:0,isExclusion:!1});function r(){t.goBackOrTo(D({name:"product collections"}))}function p(){return u(this,null,function*(){yield y(i.value),r()})}return(c,e)=>{const o=f;return g(),v(b,null,[n(h,{class:"p-24","crumb-path":[{name:l(m)("common.discounts"),route:{name:"discounts"}},{name:l(m)("common.create")}]},null,8,["crumb-path"]),n(x,{model:i.value},null,8,["model"]),n(o,{permission:{feature:"productCategories",action:"edit"},onCancel:r,onSave:p})],64)}}});export{vo as default};