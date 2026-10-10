export*from"/element-plus.vendor.js";import{default as oe}from"/element-plus.vendor.js";import{computed as P,defineComponent as S,h as a,onBeforeUnmount as C,onMounted as M,ref as g,watch as E}from"vue";import{ElMenu as N,ElMenuItem as F,ElMenuItemGroup as A}from"/element-plus.vendor.js";import{ElDrawer as B}from"/element-plus.vendor.js";import{ElButton as O}from"/element-plus.vendor.js";import{ElSkeleton as R,ElSkeletonItem as KbNavigationSkeletonItem}from"/element-plus.vendor.js";import{defineComponent as x,h as v,onBeforeUnmount as y,onMounted as L,ref as k,watch as T}from"vue";import{loadLucide as D}from"/lucide.js";var m=x({name:"KbLucideIcon",props:{name:{type:String,required:!0},size:{type:[Number,String],default:18}},setup(t){let n=k(null),c,d=!1;y(()=>{d=!0});let r=()=>{let i=t.name.replace(/(^|-)([a-z])/g,(l,s,p)=>p.toUpperCase());d||(n.value=c?.icons?.[i]||null)};L(async()=>{try{c=await D(),r()}catch{d||(n.value=null)}}),T(()=>t.name,r);let o=([i,l,s])=>v(i,l,s?.map(o));return()=>v("svg",{viewBox:"0 0 24 24",fill:"none",stroke:"currentColor","stroke-width":2,"stroke-linecap":"round","stroke-linejoin":"round",...n.value?.[1],width:t.size,height:t.size,"aria-hidden":"true",focusable:"false",class:"kb-ep-icon"},n.value?.[2]?.map(o))}});var I=S({name:"KbElementShell",props:{appName:{type:String,required:!0},navigation:{type:Array,default:()=>[]},navigationLoading:Boolean,activePath:{type:String,default:""},collapsed:{type:Boolean,default:!1},navigationLabel:{type:String,default:"Navigation"},menuLabel:{type:String,default:"Open navigation"}},emits:["navigate","update:collapsed"],setup(t,{emit:n,slots:c,expose:d}){let r=g(null),o=g(!1),i=g(!1),l,s=()=>{i.value=l.matches,l.matches||(o.value=!1)};M(()=>{l=window.matchMedia("(max-width: 767px)"),s(),l.addEventListener("change",s)}),C(()=>l?.removeEventListener("change",s)),E(()=>t.activePath,()=>{o.value=!1,r.value&&(r.value.scrollTop=0)});let p=()=>n("update:collapsed",!t.collapsed);d({toggle:p,openNavigation:()=>{o.value=!0}});let w=P(()=>{let e=new Map;for(let u of t.navigation){let h=u.group||"";e.has(h)||e.set(h,[]),e.get(h).push(u)}return e}),f=e=>a(F,{index:e.path,disabled:e.disabled,key:e.path},{default:()=>[e.icon?a(m,{name:e.icon}):null,a("span",e.label)]}),b=()=>t.navigationLoading?a("div",{class:"kb-ep-navigation-skeleton",role:"status","aria-label":"\u6B63\u5728\u51C6\u5907\u5BFC\u822A","aria-busy":"true"},[a(R,{animated:!0,"aria-hidden":"true"},{template:()=>[96,112,80,104,88,72].map((e,t)=>a("div",{class:"kb-ep-navigation-skeleton-row",key:t},[a(KbNavigationSkeletonItem,{variant:"rect",class:"kb-ep-navigation-skeleton-icon"}),a(KbNavigationSkeletonItem,{variant:"text",class:"kb-ep-navigation-skeleton-label",style:{width:e+"px"}})]))})]):a("nav",{class:"kb-ep-navigation","aria-label":t.navigationLabel},[a(N,{defaultActive:t.activePath,onSelect:e=>{o.value=!1,n("navigate",e)}},{default:()=>[...w.value].map(([e,u])=>e?a(A,{title:e,key:e},{default:()=>u.map(f)}):u.map(f))})]);return()=>a("section",{class:["kb-ep-shell",{"is-collapsed":t.collapsed}]},[a("header",{class:"kb-ep-shell-header"},[a("div",{class:"kb-ep-shell-identity","data-app-region":"identity"},[a("span",{class:"kb-ep-shell-logo","aria-hidden":"true"},Array.from(t.appName.trim())[0]||"A"),a("span",{class:"kb-ep-shell-name",title:t.appName},t.appName)]),a("div",{class:"kb-ep-shell-actions"},[a(O,{class:"kb-ep-mobile-toggle","aria-label":t.menuLabel,"aria-expanded":o.value,onClick:()=>{o.value=!o.value}},{default:()=>a(m,{name:"menu"})}),c.actions?.({collapsed:t.collapsed,toggle:p})])]),a("div",{class:"kb-ep-shell-workspace"},[t.collapsed?null:a("aside",{class:"kb-ep-shell-aside"},b()),a("main",{ref:r,class:"kb-ep-shell-main"},c.default?.())]),i.value?a(B,{modelValue:o.value,"onUpdate:modelValue":e=>{o.value=e},title:t.navigationLabel,direction:"ltr",size:"min(300px, 90vw)",appendToBody:!0},{default:b}):null])}});var K={name:"zh-cn",el:{breadcrumb:{label:"\u9762\u5305\u5C51"},colorpicker:{confirm:"\u786E\u5B9A",clear:"\u6E05\u7A7A",defaultLabel:"\u989C\u8272\u9009\u62E9\u5668",description:"\u5F53\u524D\u989C\u8272 {color}\uFF0C\u6309 Enter \u952E\u9009\u62E9\u65B0\u989C\u8272",alphaLabel:"\u9009\u62E9\u900F\u660E\u5EA6\u7684\u503C",alphaDescription:"\u900F\u660E\u5EA6 {alpha}, \u5F53\u524D\u989C\u8272 {color}",hueLabel:"\u9009\u62E9\u8272\u76F8\u503C",hueDescription:"\u8272\u76F8 {hue}, \u5F53\u524D\u989C\u8272 {color}",svLabel:"\u9009\u62E9\u9971\u548C\u5EA6\u4E0E\u660E\u5EA6\u7684\u503C",svDescription:"\u9971\u548C\u5EA6 {saturation}, \u660E\u5EA6 {brightness}, \u5F53\u524D\u989C\u8272 {color}",predefineDescription:"\u9009\u62E9 {value} \u4F5C\u4E3A\u989C\u8272"},datepicker:{now:"\u6B64\u523B",today:"\u4ECA\u5929",cancel:"\u53D6\u6D88",clear:"\u6E05\u7A7A",confirm:"\u786E\u5B9A",dateTablePrompt:"\u4F7F\u7528\u65B9\u5411\u952E\u4E0E Enter \u952E\u53EF\u9009\u62E9\u65E5\u671F",monthTablePrompt:"\u4F7F\u7528\u65B9\u5411\u952E\u4E0E Enter \u952E\u53EF\u9009\u62E9\u6708\u4EFD",quarterTablePrompt:"\u4F7F\u7528\u65B9\u5411\u952E\u4E0E Enter \u952E\u53EF\u9009\u62E9\u5B63\u5EA6",yearTablePrompt:"\u4F7F\u7528\u65B9\u5411\u952E\u4E0E Enter \u952E\u53EF\u9009\u62E9\u5E74\u4EFD",selectedDate:"\u5DF2\u9009\u65E5\u671F",selectDate:"\u9009\u62E9\u65E5\u671F",selectTime:"\u9009\u62E9\u65F6\u95F4",startDate:"\u5F00\u59CB\u65E5\u671F",startTime:"\u5F00\u59CB\u65F6\u95F4",endDate:"\u7ED3\u675F\u65E5\u671F",endTime:"\u7ED3\u675F\u65F6\u95F4",prevYear:"\u524D\u4E00\u5E74",nextYear:"\u540E\u4E00\u5E74",prevMonth:"\u4E0A\u4E2A\u6708",nextMonth:"\u4E0B\u4E2A\u6708",year:"\u5E74",month1:"1 \u6708",month2:"2 \u6708",month3:"3 \u6708",month4:"4 \u6708",month5:"5 \u6708",month6:"6 \u6708",month7:"7 \u6708",month8:"8 \u6708",month9:"9 \u6708",month10:"10 \u6708",month11:"11 \u6708",month12:"12 \u6708",weeks:{sun:"\u65E5",mon:"\u4E00",tue:"\u4E8C",wed:"\u4E09",thu:"\u56DB",fri:"\u4E94",sat:"\u516D"},weeksFull:{sun:"\u661F\u671F\u65E5",mon:"\u661F\u671F\u4E00",tue:"\u661F\u671F\u4E8C",wed:"\u661F\u671F\u4E09",thu:"\u661F\u671F\u56DB",fri:"\u661F\u671F\u4E94",sat:"\u661F\u671F\u516D"},months:{jan:"\u4E00\u6708",feb:"\u4E8C\u6708",mar:"\u4E09\u6708",apr:"\u56DB\u6708",may:"\u4E94\u6708",jun:"\u516D\u6708",jul:"\u4E03\u6708",aug:"\u516B\u6708",sep:"\u4E5D\u6708",oct:"\u5341\u6708",nov:"\u5341\u4E00\u6708",dec:"\u5341\u4E8C\u6708"}},input:{characters:"{count} / {max} \u4E2A\u5B57\u7B26"},inputNumber:{decrease:"\u51CF\u5C11\u6570\u503C",increase:"\u589E\u52A0\u6570\u503C"},select:{loading:"\u52A0\u8F7D\u4E2D",noMatch:"\u65E0\u5339\u914D\u6570\u636E",noData:"\u65E0\u6570\u636E",placeholder:"\u8BF7\u9009\u62E9"},mention:{loading:"\u52A0\u8F7D\u4E2D"},dropdown:{toggleDropdown:"\u5207\u6362\u4E0B\u62C9\u9009\u9879"},cascader:{noMatch:"\u65E0\u5339\u914D\u6570\u636E",loading:"\u52A0\u8F7D\u4E2D",placeholder:"\u8BF7\u9009\u62E9",noData:"\u6682\u65E0\u6570\u636E"},pagination:{goto:"\u524D\u5F80",pagesize:"\u6761/\u9875",total:"\u5171 {total} \u6761",pageClassifier:"\u9875",page:"\u9875",prev:"\u4E0A\u4E00\u9875",next:"\u4E0B\u4E00\u9875",currentPage:"\u7B2C {pager} \u9875",prevPages:"\u5411\u524D {pager} \u9875",nextPages:"\u5411\u540E {pager} \u9875",deprecationWarning:"\u4F60\u4F7F\u7528\u4E86\u4E00\u4E9B\u5DF2\u88AB\u5E9F\u5F03\u7684\u7528\u6CD5\uFF0C\u8BF7\u53C2\u8003 el-pagination \u7684\u5B98\u65B9\u6587\u6863"},dialog:{close:"\u5173\u95ED\u6B64\u5BF9\u8BDD\u6846"},drawer:{close:"\u5173\u95ED\u6B64\u5BF9\u8BDD\u6846"},messagebox:{title:"\u63D0\u793A",confirm:"\u786E\u5B9A",cancel:"\u53D6\u6D88",error:"\u8F93\u5165\u7684\u6570\u636E\u4E0D\u5408\u6CD5!",close:"\u5173\u95ED\u6B64\u5BF9\u8BDD\u6846"},upload:{deleteTip:"\u6309 Delete \u952E\u53EF\u5220\u9664",delete:"\u5220\u9664",preview:"\u67E5\u770B\u56FE\u7247",continue:"\u7EE7\u7EED\u4E0A\u4F20"},slider:{defaultLabel:"\u6ED1\u5757\u4ECB\u4E8E {min} \u81F3 {max}",defaultRangeStartLabel:"\u9009\u62E9\u8D77\u59CB\u503C",defaultRangeEndLabel:"\u9009\u62E9\u7ED3\u675F\u503C"},table:{emptyText:"\u6682\u65E0\u6570\u636E",confirmFilter:"\u7B5B\u9009",resetFilter:"\u91CD\u7F6E",clearFilter:"\u5168\u90E8",sumText:"\u5408\u8BA1",selectAllLabel:"\u9009\u62E9\u6240\u6709\u884C",selectRowLabel:"\u9009\u62E9\u5F53\u524D\u884C",expandRowLabel:"\u5C55\u5F00\u5F53\u524D\u884C",collapseRowLabel:"\u6536\u8D77\u5F53\u524D\u884C",sortLabel:"\u6309 {column} \u6392\u5E8F",filterLabel:"\u6309 {column} \u8FC7\u6EE4"},tag:{close:"\u5173\u95ED\u6B64\u6807\u7B7E"},tour:{next:"\u4E0B\u4E00\u6B65",previous:"\u4E0A\u4E00\u6B65",finish:"\u7ED3\u675F\u5BFC\u89C8",close:"\u5173\u95ED\u6B64\u5BF9\u8BDD\u6846"},tree:{emptyText:"\u6682\u65E0\u6570\u636E"},transfer:{noMatch:"\u65E0\u5339\u914D\u6570\u636E",noData:"\u65E0\u6570\u636E",titles:["\u5217\u8868 1","\u5217\u8868 2"],filterPlaceholder:"\u8BF7\u8F93\u5165\u641C\u7D22\u5185\u5BB9",noCheckedFormat:"\u5171 {total} \u9879",hasCheckedFormat:"\u5DF2\u9009 {checked}/{total} \u9879"},image:{error:"\u52A0\u8F7D\u5931\u8D25"},pageHeader:{title:"\u8FD4\u56DE"},popconfirm:{confirmButtonText:"\u786E\u5B9A",cancelButtonText:"\u53D6\u6D88"},carousel:{leftArrow:"\u4E0A\u4E00\u5F20\u5E7B\u706F\u7247",rightArrow:"\u4E0B\u4E00\u5F20\u5E7B\u706F\u7247",indicator:"\u5E7B\u706F\u7247\u5207\u6362\u81F3\u7D22\u5F15 {index}"},inputOTP:{groupLabel:"\u4E00\u6B21\u6027\u5BC6\u7801\u8F93\u5165\u6846",defaultLabel:"\u8BF7\u8F93\u5165\u7B2C {index} \u4F4D OTP \u5B57\u7B26"}}};var z={name:"en",el:{breadcrumb:{label:"Breadcrumb"},colorpicker:{confirm:"OK",clear:"Clear",defaultLabel:"color picker",description:"current color is {color}. press enter to select a new color.",alphaLabel:"pick alpha value",alphaDescription:"alpha {alpha}, current color is {color}",hueLabel:"pick hue value",hueDescription:"hue {hue}, current color is {color}",svLabel:"pick saturation and brightness value",svDescription:"saturation {saturation}, brightness {brightness}, current color is {color}",predefineDescription:"select {value} as the color"},datepicker:{now:"Now",today:"Today",cancel:"Cancel",clear:"Clear",confirm:"OK",dateTablePrompt:"Use the arrow keys and enter to select the day of the month",monthTablePrompt:"Use the arrow keys and enter to select the month",quarterTablePrompt:"Use the arrow keys and enter to select the quarter",yearTablePrompt:"Use the arrow keys and enter to select the year",selectedDate:"Selected date",selectDate:"Select date",selectTime:"Select time",startDate:"Start Date",startTime:"Start Time",endDate:"End Date",endTime:"End Time",prevYear:"Previous Year",nextYear:"Next Year",prevMonth:"Previous Month",nextMonth:"Next Month",year:"",month1:"January",month2:"February",month3:"March",month4:"April",month5:"May",month6:"June",month7:"July",month8:"August",month9:"September",month10:"October",month11:"November",month12:"December",weeks:{sun:"Sun",mon:"Mon",tue:"Tue",wed:"Wed",thu:"Thu",fri:"Fri",sat:"Sat"},weeksFull:{sun:"Sunday",mon:"Monday",tue:"Tuesday",wed:"Wednesday",thu:"Thursday",fri:"Friday",sat:"Saturday"},months:{jan:"Jan",feb:"Feb",mar:"Mar",apr:"Apr",may:"May",jun:"Jun",jul:"Jul",aug:"Aug",sep:"Sep",oct:"Oct",nov:"Nov",dec:"Dec"}},input:{characters:"{count} / {max} characters"},inputNumber:{decrease:"decrease number",increase:"increase number"},select:{loading:"Loading",noMatch:"No matching data",noData:"No data",placeholder:"Select"},mention:{loading:"Loading"},dropdown:{toggleDropdown:"Toggle Dropdown"},cascader:{noMatch:"No matching data",loading:"Loading",placeholder:"Select",noData:"No data"},pagination:{goto:"Go to",pagesize:"/page",total:"Total {total}",pageClassifier:"",page:"Page",prev:"Go to previous page",next:"Go to next page",currentPage:"page {pager}",prevPages:"Previous {pager} pages",nextPages:"Next {pager} pages",deprecationWarning:"Deprecated usages detected, please refer to the el-pagination documentation for more details"},dialog:{close:"Close this dialog"},drawer:{close:"Close this dialog"},messagebox:{title:"Message",confirm:"OK",cancel:"Cancel",error:"Illegal input",close:"Close this dialog"},upload:{deleteTip:"press delete to remove",delete:"Delete",preview:"Preview",continue:"Continue"},slider:{defaultLabel:"slider between {min} and {max}",defaultRangeStartLabel:"pick start value",defaultRangeEndLabel:"pick end value"},table:{emptyText:"No Data",confirmFilter:"Confirm",resetFilter:"Reset",clearFilter:"All",sumText:"Sum",selectAllLabel:"Select all rows",selectRowLabel:"Select this row",expandRowLabel:"Expand this row",collapseRowLabel:"Collapse this row",sortLabel:"Sort by {column}",filterLabel:"Filter by {column}"},tag:{close:"Close this tag"},tour:{next:"Next",previous:"Previous",finish:"Finish",close:"Close this dialog"},tree:{emptyText:"No Data"},transfer:{noMatch:"No matching data",noData:"No data",titles:["List 1","List 2"],filterPlaceholder:"Enter keyword",noCheckedFormat:"{total} items",hasCheckedFormat:"{checked}/{total} checked"},image:{error:"FAILED"},pageHeader:{title:"Back"},popconfirm:{confirmButtonText:"Yes",cancelButtonText:"No"},carousel:{leftArrow:"Carousel arrow left",rightArrow:"Carousel arrow right",indicator:"Carousel switch to index {index}"},inputOTP:{groupLabel:"OTP Input",defaultLabel:"Please enter OTP character {index}"}}};var X="0.3.0",$="2.14.5";export{I as KbElementShell,m as KbLucideIcon,oe as default,$ as elementPlusVersion,z as epEn,K as epZhCn,X as runtimeVersion};
/*! Bundled license information:

element-plus/dist/locale/zh-cn.mjs:
element-plus/dist/locale/en.mjs:
  (*! Element Plus v2.14.5 *)
*/

/* Third-party license texts for Element Plus and its dependencies.
The official vendor controls its transitive versions; licenseSourceVersion in
licenses/third-party.json identifies the source of each retained license text.
@ctrl/tinycolor
Copyright (c) Scott Cooper <scttcper@gmail.com>

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


@element-plus/icons-vue
The MIT License (MIT)

Copyright (c) 2020-PRESENT Element Plus (https://github.com/element-plus)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.


@floating-ui/core
MIT License

Copyright (c) 2021-present Floating UI contributors

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


@floating-ui/dom
MIT License

Copyright (c) 2021-present Floating UI contributors

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


@floating-ui/utils
MIT License

Copyright (c) 2021-present Floating UI contributors

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


@popperjs/core
The MIT License (MIT)

Copyright (c) 2019 Federico Zivolo

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


@vue/shared
The MIT License (MIT)

Copyright (c) 2018-present, Yuxi (Evan) You

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.


@vueuse/core
MIT License

Copyright (c) 2019-PRESENT Anthony Fu<https://github.com/antfu>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.


@vueuse/shared
MIT License

Copyright (c) 2019-PRESENT Anthony Fu<https://github.com/antfu>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.


async-validator
The MIT License (MIT)

Copyright (c) 2014-present yiminghe

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


dayjs
MIT License

Copyright (c) 2018-present, iamkun

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.


element-plus
MIT License

Copyright (c) 2020-PRESENT Element Plus

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.


lodash-es
Copyright OpenJS Foundation and other contributors <https://openjsf.org/>

Based on Underscore.js, copyright Jeremy Ashkenas,
DocumentCloud and Investigative Reporters & Editors <http://underscorejs.org/>

This software consists of voluntary contributions made by many
individuals. For exact contribution history, see the revision history
available at https://github.com/lodash/lodash

The following license applies to all parts of this software except as
documented below:

====

Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

====

Copyright and related rights for sample code are waived via CC0. Sample
code is defined as all source code displayed within the prose of the
documentation.

CC0: http://creativecommons.org/publicdomain/zero/1.0/

====

Files located in the node_modules and vendor directories are externally
maintained libraries used by this software which have their own
licenses; we recommend you read them, as their terms may differ from the
terms above.


memoize-one
MIT License

Copyright (c) 2019 Alexander Reardon

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

normalize-wheel-es
BSD License

For FixedDataTable software

Copyright (c) 2015, Facebook, Inc. All rights reserved.

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

 * Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.

 * Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

 * Neither the name Facebook nor the names of its contributors may be used to
   endorse or promote products derived from this software without specific
   prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

*/
export {};
