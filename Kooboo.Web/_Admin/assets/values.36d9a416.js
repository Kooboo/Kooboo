var u=(d,a,i)=>new Promise((o,s)=>{var p=t=>{try{r(i.next(t))}catch(m){s(m)}},n=t=>{try{r(i.throw(t))}catch(m){s(m)}},r=t=>t.done?o(t.value):Promise.resolve(t.value).then(p,n);r((i=i.apply(d,a)).next())});import{_ as h}from"./k-table.d4486972.js";import{B as b}from"./breadcrumb.80c136b8.js";import{d as w,g as y,c as C,o as c,a as f,f as g,b as _,u as l,w as S,F as v,x as k,b6 as B,j as N}from"./url.2e6a77c4.js";import{g as x}from"./i18n.48bd28ac.js";import{W as E}from"./dev-mode.c4133b5a.js";import{b as F}from"./confirm.e2c924ff.js";import{E as V}from"./index.b494e80e.js";import"./sortable.esm.a99254e8.js";import"./icon-button.3313e42d.js";import"./windi.a5b0b048.js";import"./index.2bc50276.js";import"./index.ff0264f3.js";import"./index.379e80ad.js";import"./focus-trap.23f44899.js";import"./isNil.98bb3b88.js";import"./event.53b2ad83.js";import"./index.8262f5ef.js";import"./index.fcc3ea42.js";import"./main.ea42d807.js";import"./index.50c16ae5.js";import"./preload-helper.13a99eb0.js";import"./replace-all.7cf5f327.js";import"./index.7034978e.js";import"./index.0d2684bf.js";import"./index.fc90f5ad.js";import"./event.776e7e11.js";import"./error.7e8331f1.js";import"./index.a439b087.js";import"./scroll.f51de5d8.js";import"./isEqual.f1ae9fb3.js";import"./_baseIsEqual.c0d7e77a.js";import"./debounce.61c67278.js";import"./toNumber.574be4f1.js";import"./index.064c7de9.js";import"./validator.e2869aba.js";import"./index.77edae39.js";import"./index.695d0284.js";/* empty css                                                               */import"./plugin-vue_export-helper.21dcd24c.js";import"./plugin-vue_export-helper.41ffa612.js";import"./guid.c1a40312.js";import"./use-shortcuts.21ce2d8e.js";import"./userWorker.b3a6730b.js";import"./editor.main.d2800f63.js";import"./config.9fb52765.js";import"./dark.ddf8665a.js";import"./page.831d1a45.js";import"./validate.efc4a5c0.js";import"./index.d75a71d9.js";import"./index.aba77680.js";import"./index.6d45a031.js";import"./index.31f26a0f.js";import"./index.60af85f4.js";import"./style.19d0c187.js";import"./index.6855d037.js";import"./_baseClone.adbc92f5.js";import"./index.99c4f65d.js";import"./index.603c1365.js";import"./aria.75ec5909.js";import"./index.f5a869bb.js";import"./dropdown.0507a1c7.js";import"./refs.4001ce17.js";import"./use-copy-text.a346ed23.js";import"./index.c6df1b45.js";import"./index.0f940e7f.js";import"./classCompletion.a22e38a6.js";import"./vuedraggable.umd.5840ebc7.js";import"./cloneDeep.ff43c1f8.js";import"./toggleComment.5b29ca87.js";import"./use-save-tip.04d9878f.js";import"./index.a731e53b.js";import"./index.cbddf8eb.js";import"./index.c4e9b529.js";import"./index.66d5d547.js";import"./index.232e741a.js";import"./index.af90dc36.js";import"./index.470f0e7e.js";import"./index.deca86b5.js";import"./alert.583ccfe6.js";import"./index.fafb14b3.js";import"./media-list.vue_vue_type_style_index_0_scoped_true_lang.118b18f5.js";/* empty css                                                          */import"./file.b0d4cc6e.js";import"./use-date.01b82ce0.js";import"./dayjs.min.0a66969b.js";/* empty css                                                          *//* empty css                                                                 */import"./use-file-upload.82732353.js";/* empty css                                                         */import"./image-editor.vue_vue_type_style_index_0_scoped_true_lang.f6d26366.js";import"./image-editor.846c8dc6.js";import"./main.esm.5190fb65.js";import"./index.f01d4ffd.js";import"./logo-transparent.1566007e.js";const z={class:"p-24 flex items-center"},D={class:"p-24 pt-0"},yo=w({setup(d){const{t:a}=x(),i=E(),o=y({list:[],pageNr:1,pageSize:30,totalCount:0,totalPages:0}),s=C(()=>{let r=new Set;return o.value.list.forEach(t=>{var m;(m=Object.keys(t.values))==null||m.forEach(e=>r.add(e))}),Array.from(r)}),p=r=>u(this,null,function*(){o.value=yield i.getFormValues(k("id"),r),o.value.list||(o.value.list=[]),o.value.list.sort((t,m)=>t.lastModified>m.lastModified?-1:1)}),n=r=>u(this,null,function*(){yield F(r.length),i.deleteFormValues(r.map(t=>t.id)),p(o.value.pageNr)});return p(o.value.pageNr),(r,t)=>{const m=V;return c(),f(v,null,[g("div",z,[_(b,{"crumb-path":[{name:l(a)("common.forms"),route:{name:"forms"}},{name:l(a)("common.data")}]},null,8,["crumb-path"])]),g("div",D,[_(l(h),{data:o.value.list,"show-check":"","table-layout":"auto",pagination:{pageSize:o.value.pageSize,pageCount:o.value.totalPages,currentPage:o.value.pageNr},onChange:p,onDelete:n},{default:S(()=>[(c(!0),f(v,null,B(l(s),e=>(c(),N(m,{key:e,label:e,prop:`values.${e}`},null,8,["label","prop"]))),128))]),_:1},8,["data","pagination"])])],64)}}});export{yo as default};