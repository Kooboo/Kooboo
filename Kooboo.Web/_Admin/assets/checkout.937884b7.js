var k=(x,g,e)=>new Promise((l,m)=>{var f=s=>{try{p(e.next(s))}catch(_){m(_)}},t=s=>{try{p(e.throw(s))}catch(_){m(_)}},p=s=>s.done?l(s.value):Promise.resolve(s.value).then(f,t);p((e=e.apply(x,g)).next())});import{_ as F}from"./index.5f76a3ad.js";import{d as h,g as y,h as R,c as S,i as U,o as d,a as b,u as i,b as a,w as n,F as w,b6 as D,aH as A,t as B,k as E,j as C,x as G,cd as O,f as $}from"./url.8f5ec20c.js";import{B as T}from"./breadcrumb.edaec300.js";import{g as N}from"./i18n.bcd18f8a.js";import{u as j}from"./replace-all.d441bf14.js";import{b as z}from"./cart.d4b0153a.js";import{c as H}from"./order.51491e78.js";import{_ as K,a as L}from"./products-editor.3bc63884.js";import{_ as M}from"./icon-button.315b6443.js";import{b as Q,e as q}from"./create-dialog.2b89602b.js";import{_ as J}from"./create-address-dialog.e967124c.js";import{b as P,a as W}from"./index.59b1471f.js";import{_ as X}from"./plugin-vue_export-helper.21dcd24c.js";import{E as Y,a as Z}from"./index.6d63eb53.js";import{E as oo}from"./index.5cbbc5d7.js";import"./index.aec72f69.js";import"./windi.19264205.js";import"./index.05e21f33.js";import"./main.582f9de6.js";import"./index.50c16ae5.js";import"./preload-helper.13a99eb0.js";/* empty css                                                               */import"./plugin-vue_export-helper.41ffa612.js";import"./index.79f78425.js";import"./error.7e8331f1.js";import"./date.2bd9894e.js";import"./dayjs.min.59f10137.js";import"./search-input.5b2e8d23.js";/* empty css                                                                 */import"./index.b83a1079.js";import"./index.ec6ad7db.js";import"./focus-trap.eafcfd1f.js";import"./isNil.98bb3b88.js";import"./event.53b2ad83.js";import"./debounce.730e1961.js";import"./toNumber.6efebd6a.js";import"./index.daafc4da.js";import"./event.776e7e11.js";import"./index.9a83ee01.js";import"./index.c80f5028.js";import"./index.d4a6b2d5.js";import"./index.a3d8335f.js";import"./scroll.4888a9e9.js";import"./refs.d2253dd4.js";import"./product-variant.08b1c2e8.js";import"./guid.c1a40312.js";import"./editable-tags.310754fc.js";import"./index.6d22f937.js";import"./isEqual.11d86bcc.js";import"./_baseIsEqual.547729d3.js";import"./validator.b73911a9.js";import"./index.4ecdcecf.js";import"./dropdown.f6378640.js";import"./_baseIndexOf.4d7985be.js";import"./_baseClone.eeff2792.js";import"./property-item.c5caa906.js";import"./product.9a7c7001.js";import"./useFields.4b33b4c8.js";import"./useLabels.a0b6497c.js";import"./string.75b81683.js";import"./commerce.733e9edf.js";import"./index.87ae2965.js";import"./search-provider.17b07ede.js";import"./last.e7aa49db.js";import"./style.9c8f6403.js";import"./_baseUniq.098d08d2.js";import"./use-operation-dialog.175b540a.js";import"./media-list.vue_vue_type_style_index_0_scoped_true_lang.c0b84b74.js";import"./file.734aeed6.js";import"./validate.238a9986.js";import"./index.2341329b.js";import"./index.bda83f28.js";import"./index.bff48780.js";import"./index.649f6c77.js";import"./index.a32fb6e5.js";import"./index.7a75fca3.js";import"./folder.8308bb9d.js";import"./use-date.a1321c18.js";import"./relations-tag.7a82bbd0.js";import"./page.e927203a.js";import"./index.10f642b2.js";import"./aria.75ec5909.js";import"./use-copy-text.c117d066.js";import"./index.ea2df5df.js";import"./browser.6efdef8a.js";import"./throttle.3f3ee457.js";import"./index.564bc658.js";import"./_createCompounder.2027fb34.js";import"./color-picker.a16347ae.js";import"./index.f78668f8.js";import"./position.f6e7bc86.js";import"./image-cover.2420cf5d.js";import"./media-dialog.31d6c41c.js";import"./media.71de4b30.js";import"./index.b8a66e09.js";/* empty css                                                          *//* empty css                                                          */import"./layout.29f65550.js";import"./index.4972d853.js";import"./utils.dd51a1eb.js";import"./confirm.eadb49f1.js";import"./logo-transparent.1566007e.js";import"./index.99c245f4.js";import"./use-file-upload.c73251ef.js";import"./index.ff00fbd6.js";import"./index.57a23e6d.js";import"./index.55babe62.js";import"./index.6c05d6c4.js";import"./file.eef7ed38.js";/* empty css                                                         */import"./image-editor.vue_vue_type_style_index_0_scoped_true_lang.53a9e945.js";import"./isEmpty.8640792d.js";import"./discount.d9c45514.js";import"./currency-amount.e5254595.js";import"./index.3a977dfb.js";import"./useTag.af222f48.js";const to={class:"space-y-4"},eo={key:0,class:"bg-card dark:bg-444 rounded-normal px-16 py-8"},ro=h({props:{customerId:null},emits:["selected"],setup(x,{emit:g}){const e=x,l=y(),m=y(),{t:f}=N(),t=y(!1);function p(){return k(this,null,function*(){var r;e.customerId&&(m.value=yield Q(e.customerId),l.value=(r=m.value.addresses.findIndex(o=>o.isDefault))!=null?r:0)})}R(p);const s=S(()=>{const r=[];if(m.value)for(const o of m.value.addresses)r.push(`${o.firstName} ${o.lastName} ${o.phone} ${o.province} ${o.city} ${o.address1} ${o.address2} ${o.zip}`);return r});function _(r){return k(this,null,function*(){m.value.addresses.forEach(o=>o.isDefault=!1),m.value.addresses.push(r),yield q(m.value),p()})}return U(()=>l.value,()=>{var r;if(l.value>-1){const o=(r=m.value)==null?void 0:r.addresses[l.value];o&&g("selected",o)}}),(r,o)=>{const v=P,I=W,V=M;return d(),b("div",to,[i(s).length?(d(),b("div",eo,[a(I,{modelValue:l.value,"onUpdate:modelValue":o[0]||(o[0]=u=>l.value=u),class:"block"},{default:n(()=>[(d(!0),b(w,null,D(i(s),(u,c)=>(d(),b("div",{key:c,class:"flex items-center"},[a(v,{label:c},{default:n(()=>[A(B(u),1)]),_:2},1032,["label"])]))),128))]),_:1},8,["modelValue"])])):E("",!0),a(V,{circle:"",class:"text-blue",icon:"icon-a-addto",tip:i(f)("common.add"),onClick:o[1]||(o[1]=u=>t.value=!0)},null,8,["tip"]),t.value?(d(),C(J,{key:1,modelValue:t.value,"onUpdate:modelValue":o[2]||(o[2]=u=>t.value=u),onCreate:_},null,8,["modelValue"])):E("",!0)])}}});const ao={class:"px-24 pt-0 pb-84px space-y-12"},mo={key:0,class:"bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal"},so={class:"w-full"},io={class:"bg-card dark:bg-444 dark:text-gray rounded-normal px-16 py-8 w-full"},lo={class:"bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal"},po=h({setup(x){const g=G("id"),{t:e}=N(),l=O(),m=y(),f=y(""),t=y();z(g).then(r=>k(this,null,function*(){t.value=r}));function p(){l.goBackOrTo(j({name:"carts"}))}function s(){return k(this,null,function*(){yield H({cartId:t.value.id,note:f.value,address:m.value}),p()})}function _(r){m.value=r}return(r,o)=>{const v=Y,I=Z,V=oo,u=F;return d(),b(w,null,[a(T,{class:"p-24","crumb-path":[{name:i(e)("common.carts"),route:{name:"carts"}},{name:i(e)("commerce.checkout")}]},null,8,["crumb-path"]),$("div",ao,[t.value?(d(),b("div",mo,[a(I,{"label-position":"top"},{default:n(()=>[a(v,{label:i(e)("common.products")},{default:n(()=>[$("div",so,[a(K,{lines:t.value.lines,"onUpdate:lines":o[0]||(o[0]=c=>t.value.lines=c),readonly:"","customer-id":t.value.customerId,"discount-codes":t.value.discountCodes},null,8,["lines","customer-id","discount-codes"])])]),_:1},8,["label"]),a(v,{label:i(e)("common.contact")},{default:n(()=>[a(L,{modelValue:t.value.customerId,"onUpdate:modelValue":o[1]||(o[1]=c=>t.value.customerId=c),readonly:""},null,8,["modelValue"])]),_:1},8,["label"]),a(v,{label:i(e)("commerce.cartNote")},{default:n(()=>[$("div",io,B(t.value.note?t.value.note:"-"),1)]),_:1},8,["label"])]),_:1})])):E("",!0),$("div",lo,[t.value?(d(),C(I,{key:0,"label-position":"top"},{default:n(()=>[a(v,{label:i(e)("commerce.shippingAddress")},{default:n(()=>[a(ro,{"customer-id":t.value.customerId,class:"w-full",onSelected:_},null,8,["customer-id"])]),_:1},8,["label"]),a(v,{label:i(e)("commerce.orderNote")},{default:n(()=>[a(V,{modelValue:f.value,"onUpdate:modelValue":o[2]||(o[2]=c=>f.value=c),rows:2,type:"textarea"},null,8,["modelValue"])]),_:1},8,["label"])]),_:1})):E("",!0)])]),a(u,{permission:{feature:"carts",action:"edit"},"confirm-label":i(e)("commerce.checkout"),onCancel:p,onSave:s},null,8,["confirm-label"])],64)}}});var ke=X(po,[["__scopeId","data-v-0d8fc6e0"]]);export{ke as default};