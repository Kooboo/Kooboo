declare const k: Kooboo.Web.Frontend.KScriptDefine.KScript;
declare namespace Kooboo.Web.Frontend.KScriptDefine {
   interface KScript  {
       /** Access to the http request data, query string, form or headers. Cookie is available from k.cookie. */
       request:Kooboo.Web.Frontend.KScriptDefine.BaseMembers.Request;
       /** The http response object that is used to set data into http resposne stream */
       response:Kooboo.Web.Frontend.KScriptDefine.BaseMembers.Response;
       /** a temporary storage for small interactive information. Session does not persist */
       session:Kooboo.Web.Frontend.KScriptDefine.BaseMembers.Session;
       /** Get or set cookie value */
       cookie:Kooboo.Web.Frontend.KScriptDefine.BaseMembers.Cookie;
       /** The Kooboo website database with version control */
       site:Kooboo.Web.Frontend.KScriptDefine.BaseMembers.Site;
   }

}
declare namespace Kooboo.Web.Frontend.KScriptDefine.BaseMembers {
   interface Request  {
       /** The query string collection */
       queryString:Dictionary;
       form:Dictionary;
       method:string;
       clientIp:string;
       headers:Dictionary;
       url:string;
       files:UploadFile[];
   }

   interface Response  {
       execute(url:string):void;
       json(value:any):void;
       redirect(url:string):void;
       setHeader(key:string,value:string):void;
       write(value:any):void;
   }

   interface Session  {
       keys:any[];
       values:any[];
       clear():void;
       contains(key:string):boolean;
       get(key:string):any;
       remove(key:string):void;
       set(key:string,value:any):void;
   }

   interface Cookie  {
       keys:any[];
       values:any[];
       length:number;
       item:string;
       clear():void;
       containsKey(key:string):boolean;
       get(name:string):string;
       remove(key:string):void;
       set(name:string,value:string,days:number):void;
       set(name:string,value:string):void;
       setByMinutes(name:string,value:string,mins:number):void;
   }

   interface Site  {
       /** The repository that contains Objects that have Url route and Text body. */
       pages:RoutableTextRepository;
       /** The repository that contains Text Objects such as Layout and View */
       views:TextRepository;
       /** The repository that contains Text Objects such as Layout and View */
       layouts:TextRepository;
       /** The text content repository */
       textContents:TextContentRepository;
       /** The repository that contains Objects that have Url route and Text body. */
       scripts:RoutableTextRepository;
       /** The repository that contains Objects that have Url route and Text body. */
       styles:RoutableTextRepository;
   }

   interface Dictionary  {
       keys:any;
       values:any;
       length:number;
       item:string;
       add(key:string,value:string):void;
       contains(key:any):boolean;
       get(key:string):string;
   }

   interface UploadFile  {
       contentType:string;
       fileName:string;
       bytes:number[];
       save(filename:string):string;
   }

   interface RoutableTextRepository extends Kooboo.Web.Frontend.KScriptDefine.BaseMembers.TextRepository  {
       getAbsUrl(id:any):Kooboo.Sites.Models.SiteObject;
       getByUrl(url:string):Kooboo.Sites.Models.SiteObject;
       getUrl(id:any):Kooboo.Sites.Models.SiteObject;
   }

   interface TextRepository  {
       add(obj:any):void;
       all():Kooboo.Sites.Models.SiteObject[];
       delete(nameOrId:any):void;
       get(nameOrId:any):Kooboo.Sites.Models.SiteObject;
       update(siteObject:Kooboo.Sites.Models.SiteObject):void;
       updateBody(nameOrId:any,newbody:string):void;
   }

   interface TextContentRepository  {
       add(value:any):void;
       all():TextContentObject[];
       delete(id:string):void;
       find(searchCondition:string):TextContentObject;
       findAll(searchCondition:string):TextContentObject[];
       get(nameorid:string):TextContentObject;
       query(searchCondition:string):ContentQuery;
       update(value:TextContentObject):TextContentObject;
   }

   interface TextContentObject  {
       setCulture(culture:string):TextContentObject;
   }

   interface ContentQuery  {
       /** the number of items that will be skipped */
       skipcount:number;
       /** Is ascending order */
       ascending:boolean;
       /** The field name to order by */
       orderByField:string;
       /** The search query */
       searchCondition:string;
       count():number;
       orderBy(fieldname:string):ContentQuery;
       orderByDescending(fieldname:string):ContentQuery;
       skip(skip:number):ContentQuery;
       take(count:number):TextContentObject[];
   }

}
declare namespace Kooboo.Sites.Models {
   interface SiteObject  {
       constType:number;
       creationDate:Date;
       lastModified:Date;
       lastModifyTick:number;
       id:any;
       name:string;
       clone():any;
   }

}
