var V=(E,I,r)=>new Promise((u,s)=>{var g=l=>{try{c(r.next(l))}catch(_){s(_)}},o=l=>{try{c(r.throw(l))}catch(_){s(_)}},c=l=>l.done?u(l.value):Promise.resolve(l.value).then(g,o);c((r=r.apply(E,I)).next())});import{_ as P}from"./index.f237f871.js";import{d as w,g as b,h as D,c as F,i as R,o as d,a as x,u as p,b as a,w as n,F as C,b6 as A,aH as G,t as S,k as y,j as $,x as O,cd as T,f as h}from"./url.2e6a77c4.js";import{B as j}from"./breadcrumb.80c136b8.js";import{g as U}from"./i18n.48bd28ac.js";import{u as z}from"./replace-all.7cf5f327.js";import{b as H}from"./cart.dd59d4a2.js";import{c as K}from"./order.1bca6ba5.js";import{_ as L,a as M,b as Q,c as q}from"./products-editor.7d606721.js";import{_ as J}from"./icon-button.3313e42d.js";import{b as W,c as X,e as Y}from"./create-dialog.c1bf5b63.js";import{b as Z,a as oo}from"./index.695d0284.js";import{_ as to}from"./plugin-vue_export-helper.21dcd24c.js";import{E as eo,a as ro}from"./index.6855d037.js";import{E as ao}from"./index.fc90f5ad.js";import"./index.2bc50276.js";import"./windi.a5b0b048.js";import"./index.ff0264f3.js";import"./main.ea42d807.js";import"./index.50c16ae5.js";import"./preload-helper.13a99eb0.js";/* empty css                                                               */import"./plugin-vue_export-helper.41ffa612.js";import"./index.a439b087.js";import"./error.7e8331f1.js";import"./date.99e830d5.js";import"./dayjs.min.0a66969b.js";import"./search-input.67dfc984.js";/* empty css                                                                 */import"./index.b494e80e.js";import"./index.379e80ad.js";import"./focus-trap.23f44899.js";import"./isNil.98bb3b88.js";import"./event.53b2ad83.js";import"./debounce.61c67278.js";import"./toNumber.574be4f1.js";import"./index.77edae39.js";import"./event.776e7e11.js";import"./index.064c7de9.js";import"./index.fcc3ea42.js";import"./index.7034978e.js";import"./index.0d2684bf.js";import"./scroll.f51de5d8.js";import"./isEqual.f1ae9fb3.js";import"./_baseIsEqual.c0d7e77a.js";import"./validator.e2869aba.js";import"./index.c4e9b529.js";import"./index.603c1365.js";import"./refs.4001ce17.js";import"./shipping.f7d1d018.js";import"./currency-amount.de4456dd.js";import"./commerce.f8d3336c.js";import"./guid.c1a40312.js";import"./digital-shipping.f03d4646.js";import"./product-variant.f9d76101.js";import"./editable-tags.e8873ae5.js";import"./index.968f79fd.js";import"./dropdown.0507a1c7.js";import"./_baseIndexOf.4d7985be.js";import"./_baseClone.adbc92f5.js";import"./property-item.c8a41109.js";import"./product.c1a83798.js";import"./useFields.2c414714.js";import"./useLabels.456dfc28.js";import"./string.955a924b.js";import"./index.4a51c2b7.js";import"./search-provider.68dc3c2c.js";import"./last.e7aa49db.js";import"./style.19d0c187.js";import"./_baseUniq.c69c2d82.js";import"./use-operation-dialog.f0f37a10.js";import"./media-list.vue_vue_type_style_index_0_scoped_true_lang.118b18f5.js";import"./file.b0d4cc6e.js";import"./validate.efc4a5c0.js";import"./index.d75a71d9.js";import"./index.aba77680.js";import"./index.6d45a031.js";import"./index.31f26a0f.js";import"./index.60af85f4.js";import"./index.a731e53b.js";import"./folder.8308bb9d.js";import"./use-date.01b82ce0.js";import"./relations-tag.a09361ff.js";import"./page.831d1a45.js";import"./index.99c4f65d.js";import"./aria.75ec5909.js";import"./use-copy-text.a346ed23.js";import"./index.500e98ec.js";import"./browser.6147bf52.js";import"./throttle.041b2553.js";import"./_createCompounder.591d8488.js";import"./color-picker.770bc088.js";import"./index.ae716322.js";import"./position.c0f844a3.js";import"./image-cover.5aa60066.js";import"./media-dialog.00ce8f4f.js";import"./media.56507018.js";import"./index.4db7adae.js";/* empty css                                                          *//* empty css                                                          */import"./layout.7d98e9c1.js";import"./index.d971d096.js";import"./utils.6985c8b2.js";import"./confirm.e2c924ff.js";import"./logo-transparent.1566007e.js";import"./index.0e262df3.js";import"./use-file-upload.82732353.js";import"./index.470f0e7e.js";import"./index.deca86b5.js";import"./index.a47bd154.js";import"./index.be352e91.js";import"./file.e38b2555.js";/* empty css                                                         */import"./image-editor.vue_vue_type_style_index_0_scoped_true_lang.f6d26366.js";import"./isEmpty.dc565ae5.js";import"./discount.21525f15.js";import"./index.8262f5ef.js";import"./useTag.2282521e.js";const io={class:"space-y-4"},so={key:0,class:"bg-card dark:bg-444 rounded-normal px-16 py-8"},lo=w({props:{customerId:null},emits:["selected"],setup(E,{emit:I}){const r=E,u=b(),s=b(),{t:g}=U(),o=b(!1);function c(){return V(this,null,function*(){var m;r.customerId&&(s.value=yield X(r.customerId),u.value=(m=s.value.addresses.findIndex(t=>t.isDefault))!=null?m:0)})}D(c);const l=F(()=>{const m=[];if(s.value)for(const t of s.value.addresses)m.push(`${t.firstName} ${t.lastName} ${t.phone} ${t.province} ${t.city} ${t.address1} ${t.address2} ${t.zip}`);return m});function _(m){return V(this,null,function*(){s.value.addresses.forEach(t=>t.isDefault=!1),s.value.addresses.push(m),yield Y(s.value),c()})}return R(()=>u.value,()=>{var m;if(u.value>-1){const t=(m=s.value)==null?void 0:m.addresses[u.value];t&&I("selected",t)}}),(m,t)=>{const k=Z,e=oo,f=J;return d(),x("div",io,[p(l).length?(d(),x("div",so,[a(e,{modelValue:u.value,"onUpdate:modelValue":t[0]||(t[0]=v=>u.value=v),class:"block"},{default:n(()=>[(d(!0),x(C,null,A(p(l),(v,B)=>(d(),x("div",{key:B,class:"flex items-center"},[a(k,{label:B},{default:n(()=>[G(S(v),1)]),_:2},1032,["label"])]))),128))]),_:1},8,["modelValue"])])):y("",!0),a(f,{circle:"",class:"text-blue",icon:"icon-a-addto",tip:p(g)("common.add"),onClick:t[1]||(t[1]=v=>o.value=!0)},null,8,["tip"]),o.value?(d(),$(W,{key:1,modelValue:o.value,"onUpdate:modelValue":t[2]||(t[2]=v=>o.value=v),onCreate:_},null,8,["modelValue"])):y("",!0)])}}});const mo={class:"px-24 pt-0 pb-84px space-y-12"},po={key:0,class:"bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal"},no={class:"w-full"},uo={class:"grid grid-cols-3 gap-8"},co={class:"bg-card dark:bg-444 dark:text-gray rounded-normal px-16 py-8 w-full"},fo={class:"bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal"},vo=w({setup(E){const I=O("id"),{t:r}=U(),u=T(),s=b(),g=b(""),o=b(),c=b(!1),l=b(!1);H(I).then(k=>V(this,null,function*(){o.value=k}));function _(){u.goBackOrTo(z({name:"carts"}))}function m(){return V(this,null,function*(){yield K({cartId:o.value.id,note:g.value,address:s.value}),_()})}function t(k){s.value=k}return(k,e)=>{const f=eo,v=ro,B=ao,N=P;return d(),x(C,null,[a(j,{class:"p-24","crumb-path":[{name:p(r)("common.carts"),route:{name:"carts"}},{name:p(r)("commerce.checkout")}]},null,8,["crumb-path"]),h("div",mo,[o.value?(d(),x("div",po,[a(v,{"label-position":"top"},{default:n(()=>[a(f,null,{default:n(()=>[h("div",no,[a(L,{lines:o.value.lines,"onUpdate:lines":e[0]||(e[0]=i=>o.value.lines=i),"redeem-points":o.value.redeemPoints,"onUpdate:redeem-points":e[1]||(e[1]=i=>o.value.redeemPoints=i),readonly:"","customer-id":o.value.customerId,"shipping-id":o.value.shippingId,"discount-codes":o.value.discountCodes,"extension-button":o.value.extensionButton,"onUpdate:hasDigitalProducts":e[2]||(e[2]=i=>l.value=i),"onUpdate:hasPhysicsProducts":e[3]||(e[3]=i=>c.value=i)},null,8,["lines","redeem-points","customer-id","shipping-id","discount-codes","extension-button"])])]),_:1}),h("div",uo,[a(f,{label:p(r)("common.contact")},{default:n(()=>[a(M,{modelValue:o.value.customerId,"onUpdate:modelValue":e[4]||(e[4]=i=>o.value.customerId=i),readonly:""},null,8,["modelValue"])]),_:1},8,["label"]),c.value?(d(),$(f,{key:0,label:p(r)("common.expressShipping")},{default:n(()=>[a(Q,{modelValue:o.value.shippingId,"onUpdate:modelValue":e[5]||(e[5]=i=>o.value.shippingId=i),readonly:""},null,8,["modelValue"])]),_:1},8,["label"])):y("",!0),l.value?(d(),$(f,{key:1,label:p(r)("common.digitalShipping")},{default:n(()=>[a(q,{modelValue:o.value.digitalShippingId,"onUpdate:modelValue":e[6]||(e[6]=i=>o.value.digitalShippingId=i),readonly:""},null,8,["modelValue"])]),_:1},8,["label"])):y("",!0)]),a(f,{label:p(r)("commerce.cartNote")},{default:n(()=>[h("div",co,S(o.value.note?o.value.note:"-"),1)]),_:1},8,["label"])]),_:1})])):y("",!0),h("div",fo,[o.value?(d(),$(v,{key:0,"label-position":"top"},{default:n(()=>[c.value?(d(),$(f,{key:0,label:p(r)("commerce.shippingAddress")},{default:n(()=>[a(lo,{"customer-id":o.value.customerId,class:"w-full",onSelected:t},null,8,["customer-id"])]),_:1},8,["label"])):y("",!0),a(f,{label:p(r)("commerce.orderNote")},{default:n(()=>[a(B,{modelValue:g.value,"onUpdate:modelValue":e[7]||(e[7]=i=>g.value=i),rows:2,type:"textarea"},null,8,["modelValue"])]),_:1},8,["label"])]),_:1})):y("",!0)])]),a(N,{permission:{feature:"carts",action:"edit"},"confirm-label":p(r)("commerce.checkout"),onCancel:_,onSave:m},null,8,["confirm-label"])],64)}}});var Ee=to(vo,[["__scopeId","data-v-227f052a"]]);export{Ee as default};