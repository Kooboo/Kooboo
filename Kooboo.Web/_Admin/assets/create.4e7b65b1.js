var V=(b,t,a)=>new Promise((d,m)=>{var r=o=>{try{i(a.next(o))}catch(n){m(n)}},p=o=>{try{i(a.throw(o))}catch(n){m(n)}},i=o=>o.done?d(o.value):Promise.resolve(o.value).then(r,p);i((a=a.apply(b,t)).next())});import{_ as E}from"./index.f237f871.js";import{_ as R}from"./tooltip.28a451cc.js";import{g as S}from"./i18n.48bd28ac.js";import{_ as k}from"./object-schema.cf697800.js";import{d as T,g as c,cd as w,c as U,o as q,a as I,b as e,u as s,w as l,f as g,aH as O,t as C}from"./url.2e6a77c4.js";import{u as $}from"./replace-all.7cf5f327.js";import{i as D,s as F}from"./user-options.2c08b61d.js";import{r as A,a as H,l as J,i as K}from"./validate.efc4a5c0.js";import{E as j}from"./index.fc90f5ad.js";import{E as z,a as G}from"./index.6855d037.js";import"./index.2bc50276.js";import"./windi.a5b0b048.js";import"./index.ff0264f3.js";import"./index.379e80ad.js";import"./focus-trap.23f44899.js";import"./isNil.98bb3b88.js";import"./event.53b2ad83.js";import"./icon-button.3313e42d.js";import"./index.8262f5ef.js";import"./index.fcc3ea42.js";import"./index.0d2684bf.js";import"./index.a439b087.js";import"./error.7e8331f1.js";import"./event.776e7e11.js";import"./scroll.f51de5d8.js";import"./isEqual.f1ae9fb3.js";import"./_baseIsEqual.c0d7e77a.js";import"./debounce.61c67278.js";import"./toNumber.574be4f1.js";import"./index.064c7de9.js";import"./validator.e2869aba.js";import"./main.ea42d807.js";import"./index.50c16ae5.js";import"./preload-helper.13a99eb0.js";import"./index.d75a71d9.js";import"./index.aba77680.js";import"./index.6d45a031.js";import"./index.31f26a0f.js";import"./index.60af85f4.js";import"./style.19d0c187.js";import"./_baseClone.adbc92f5.js";const L={class:"p-24 mb-64"},M={class:"rounded-normal bg-fff dark:bg-[#252526] mt-16 py-24 px-56px"},P={class:"w-504px"},Q={class:"flex items-center"},Ao=T({setup(b){const{t}=S(),a=c([]),d=w(),m=c(""),r=c(""),p=c(),i=U(()=>({name:m.value}));function o(){d.goBackOrTo($({name:"useroptions"}))}const n={name:[A(1,50),H(t("common.nameRequiredTips")),J(),K(D,t("common.nameExistsTips"))]};function y(){return V(this,null,function*(){var _;yield(_=p.value)==null?void 0:_.validate(),yield F({name:m.value,display:r.value,schema:JSON.stringify(a.value)}),o()})}return(_,u)=>{const x=j,f=z,N=R,h=G,B=E;return q(),I("div",L,[e(h,{ref_key:"form",ref:p,model:s(i),"label-position":"top",rules:s(n)},{default:l(()=>[g("div",M,[g("div",P,[e(f,{label:s(t)("common.name"),prop:"name"},{default:l(()=>[e(x,{modelValue:m.value,"onUpdate:modelValue":u[0]||(u[0]=v=>m.value=v)},null,8,["modelValue"])]),_:1},8,["label"]),e(f,{label:s(t)("common.displayName")},{default:l(()=>[e(x,{modelValue:r.value,"onUpdate:modelValue":u[1]||(u[1]=v=>r.value=v)},null,8,["modelValue"])]),_:1},8,["label"])]),e(f,null,{label:l(()=>[g("div",Q,[O(C(s(t)("common.customSettings"))+" ",1),e(N,{tip:s(t)("common.settingSchemaTips",{name:m.value||"{Name}"}),"custom-class":"ml-4"},null,8,["tip"])])]),default:l(()=>[e(k,{schema:a.value},null,8,["schema"])]),_:1})])]),_:1},8,["model","rules"]),e(B,{permission:{feature:"userOptions",action:"setting"},onCancel:o,onSave:y})])}}});export{Ao as default};