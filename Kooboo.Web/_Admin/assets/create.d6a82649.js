var N=Object.defineProperty;var k=Object.getOwnPropertySymbols;var S=Object.prototype.hasOwnProperty,C=Object.prototype.propertyIsEnumerable;var w=(a,r,i)=>r in a?N(a,r,{enumerable:!0,configurable:!0,writable:!0,value:i}):a[r]=i,h=(a,r)=>{for(var i in r||(r={}))S.call(r,i)&&w(a,i,r[i]);if(k)for(var i of k(r))C.call(r,i)&&w(a,i,r[i]);return a};var b=(a,r,i)=>new Promise((f,g)=>{var s=p=>{try{c(i.next(p))}catch(t){g(t)}},u=p=>{try{c(i.throw(p))}catch(t){g(t)}},c=p=>p.done?f(p.value):Promise.resolve(p.value).then(s,u);c((i=i.apply(a,r)).next())});import{_ as U}from"./index.5f76a3ad.js";import{d as $,cd as A,x as M,g as _,h as P,i as T,o as R,a as z,b as n,u as m,f as I,w as O,F as G}from"./url.8f5ec20c.js";import{B as K}from"./breadcrumb.edaec300.js";import{g as Q}from"./i18n.bcd18f8a.js";import{c as j}from"./product.9a7c7001.js";import{u as q,g as H}from"./commerce.733e9edf.js";import{u as J}from"./replace-all.d441bf14.js";import{_ as L,V as W,a as X}from"./option-group-editor.ce3e28ca.js";import{_ as Y}from"./key-value-editor.958730ca.js";import{e as Z}from"./guid.c1a40312.js";import{u as tt,c as ot}from"./product-variant.08b1c2e8.js";import{_ as rt}from"./index.84fe3ce9.js";import{z as it}from"./main.582f9de6.js";import{u as mt}from"./useFields.4b33b4c8.js";import{_ as et}from"./plugin-vue_export-helper.21dcd24c.js";import{E as at,a as pt}from"./index.6d63eb53.js";import"./index.aec72f69.js";import"./windi.19264205.js";import"./index.05e21f33.js";/* empty css                                                               */import"./plugin-vue_export-helper.41ffa612.js";import"./index.79f78425.js";import"./error.7e8331f1.js";import"./image-cover.2420cf5d.js";import"./media-dialog.31d6c41c.js";import"./index.7a75fca3.js";import"./media.71de4b30.js";import"./media-list.vue_vue_type_style_index_0_scoped_true_lang.c0b84b74.js";import"./index.b8a66e09.js";/* empty css                                                          */import"./index.59b1471f.js";import"./event.776e7e11.js";import"./index.ec6ad7db.js";import"./focus-trap.eafcfd1f.js";import"./isNil.98bb3b88.js";import"./event.53b2ad83.js";import"./search-provider.17b07ede.js";import"./toNumber.6efebd6a.js";import"./last.e7aa49db.js";import"./_baseIndexOf.4d7985be.js";import"./style.9c8f6403.js";import"./_baseIsEqual.547729d3.js";import"./_baseUniq.098d08d2.js";import"./use-operation-dialog.175b540a.js";import"./file.734aeed6.js";import"./validate.238a9986.js";import"./index.2341329b.js";import"./index.bda83f28.js";import"./index.bff48780.js";import"./index.649f6c77.js";import"./index.a32fb6e5.js";import"./index.5cbbc5d7.js";import"./index.d4a6b2d5.js";import"./index.a3d8335f.js";import"./scroll.4888a9e9.js";import"./index.c80f5028.js";import"./refs.d2253dd4.js";import"./folder.8308bb9d.js";import"./use-date.a1321c18.js";import"./dayjs.min.59f10137.js";import"./relations-tag.7a82bbd0.js";import"./icon-button.315b6443.js";import"./index.3a977dfb.js";import"./page.e927203a.js";import"./index.10f642b2.js";import"./aria.75ec5909.js";import"./validator.b73911a9.js";import"./index.b83a1079.js";import"./debounce.730e1961.js";import"./index.daafc4da.js";import"./index.9a83ee01.js";import"./index.6d22f937.js";import"./isEqual.11d86bcc.js";import"./use-copy-text.c117d066.js";import"./index.ea2df5df.js";import"./browser.6efdef8a.js";import"./throttle.3f3ee457.js";import"./index.564bc658.js";import"./search-input.5b2e8d23.js";/* empty css                                                                 *//* empty css                                                          */import"./layout.29f65550.js";import"./index.4972d853.js";import"./position.f6e7bc86.js";import"./utils.dd51a1eb.js";import"./confirm.eadb49f1.js";import"./logo-transparent.1566007e.js";import"./index.99c245f4.js";import"./use-file-upload.c73251ef.js";import"./index.ff00fbd6.js";import"./index.57a23e6d.js";import"./index.55babe62.js";import"./index.6c05d6c4.js";import"./file.eef7ed38.js";/* empty css                                                         */import"./image-editor.vue_vue_type_style_index_0_scoped_true_lang.53a9e945.js";import"./editable-tags.310754fc.js";import"./index.4ecdcecf.js";import"./dropdown.f6378640.js";import"./_baseClone.eeff2792.js";import"./useTag.af222f48.js";import"./index.373c837a.js";import"./index.20b12391.js";import"./dark.166fd971.js";import"./dev-mode.1f379952.js";import"./index.72ccc088.js";import"./classCompletion.a22e38a6.js";import"./userWorker.b3a6730b.js";import"./editor.main.d2800f63.js";import"./preload-helper.13a99eb0.js";import"./vuedraggable.umd.7929a3b6.js";import"./cloneDeep.060340c1.js";import"./config.812575d1.js";import"./index.8bc4b1d6.js";import"./index.73c941f5.js";import"./toggleComment.5b29ca87.js";import"./use-save-tip.8f44d6c0.js";import"./index.4dc50d47.js";import"./index.3be79fd5.js";import"./index.79dc05e0.js";import"./alert.8fe5500a.js";import"./index.d84e2378.js";import"./image-editor.f97cfa84.js";import"./main.esm.6441082b.js";import"./index.7633560d.js";import"./pick.80c375ab.js";import"./_basePickBy.0604ed3c.js";import"./index.87ae2965.js";import"./_createCompounder.2027fb34.js";import"./color-picker.a16347ae.js";import"./index.f78668f8.js";import"./string.75b81683.js";import"./useLabels.a0b6497c.js";import"./index.ccf403d8.js";import"./k-table.7d05e82e.js";import"./sortable.esm.a99254e8.js";import"./content-effect.92be69e3.js";import"./textContent.85a87e1d.js";import"./omitBy.da11f041.js";import"./pickBy.705899be.js";import"./isEmpty.8640792d.js";import"./index.d6799c63.js";import"./index.e3f90979.js";import"./index.0a8d3d17.js";import"./custom-field.b807d01a.js";import"./index.50c16ae5.js";const nt={class:"px-24 pt-0 pb-84px space-y-12"},st={class:"bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal"},lt={class:"bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal"},ut=$({setup(a){const{t:r}=Q(),i=A(),f=M("typeId");q().loadCategories();const s=_(),u=_([]),c=_(),p=_();let t=tt();t.addVariant(ot([],""));const{fields:V,customFields:B}=mt(),l=_({categories:[],attributes:[],title:"",seoName:"",description:"",featuredImage:"",images:[],active:!0,variants:[],tags:[],customData:{},variantOptions:[]});P(()=>b(this,null,function*(){if(f&&f!=Z&&(s.value=yield H(f)),s.value){for(const o of s.value.attributes)u.value.push({key:o.name,value:"",options:o.type=="Selection"?o.options:[]});for(const o of s.value.options)t.options.value.push(o.name);for(const o of s.value.options)for(const e of o.options)t.addOptionItem(o.name,e,"")}}));function x(){i.goBackOrTo(J({name:"product management"}))}function D(){return b(this,null,function*(){var e,v;try{yield c.value.form.validate(),yield(v=(e=p.value)==null?void 0:e.form)==null?void 0:v.validate()}catch(d){throw it(),d}const o=h({},l.value);o.attributes=u.value.map(d=>({key:d.key,value:d.value})),o.variants=t.list.value,yield j(o),x()})}return T(()=>{var o;return(o=l.value)==null?void 0:o.featuredImage},o=>{if(!!o)for(const e of t.list.value)e.image||(e.image=o)}),(o,e)=>{const v=at,d=pt,F=U;return R(),z(G,null,[n(K,{class:"p-24","crumb-path":[{name:m(r)("common.productManagement"),route:{name:"product management"}},{name:m(r)("common.create")}]},null,8,["crumb-path"]),I("div",nt,[n(L,{ref_key:"basicInfo",ref:c,model:l.value,fields:m(V)},null,8,["model","fields"]),n(rt,{ref_key:"customData",ref:p,data:l.value.customData,"custom-fields":m(B)},null,8,["data","custom-fields"]),I("div",st,[n(d,{"label-position":"top"},{default:O(()=>[n(v,{label:m(r)("common.attributes")},{default:O(()=>[n(Y,{modelValue:u.value,"onUpdate:modelValue":e[0]||(e[0]=y=>u.value=y),"key-input-attributes":{placeholder:m(r)("common.name")},lass:"max-w-600px space-y-8 w-full"},null,8,["modelValue","key-input-attributes"])]),_:1},8,["label"]),n(v,{label:m(r)("commerce.variantOptions")},{default:O(()=>[n(X,{options:m(t).options.value,variants:m(t).list.value,"variant-options":l.value.variantOptions,class:"max-w-600px",onUpdateOptionItem:m(t).updateOptionItem,onUpdateOptionName:m(t).updateOptionName,onAddOptionItem:e[1]||(e[1]=(y,E)=>m(t).addOptionItem(y,E,l.value.featuredImage)),onDeleteOptionItem:m(t).deleteOptionItem,onDeleteOption:m(t).deleteOption,onAddOption:m(t).addOption},null,8,["options","variants","variant-options","onUpdateOptionItem","onUpdateOptionName","onDeleteOptionItem","onDeleteOption","onAddOption"])]),_:1},8,["label"])]),_:1})]),I("div",lt,[n(W,{variants:m(t).list.value,options:m(t).options.value,"default-image":l.value.featuredImage},null,8,["variants","options","default-image"])])]),n(F,{permission:{feature:"productManagement",action:"edit"},onCancel:x,onSave:D})],64)}}});var Yr=et(ut,[["__scopeId","data-v-5f921f8c"]]);export{Yr as default};