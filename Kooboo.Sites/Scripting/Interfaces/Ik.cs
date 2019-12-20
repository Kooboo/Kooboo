//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Diagnosis;
using Kooboo.Sites.FrontEvent;
using Kooboo.Sites.Scripting.Global;
using System.Collections.Generic;
using System.ComponentModel;

namespace Kooboo.Sites.Scripting
{
    public interface Ik
    {
        Dictionary<string, string> config { get; set; }

        [Description("Get or set cookie value")]
        Cookie Cookie { get; }

        [Description("The kScript database engine")]
        kDatabase Database { get; }

        [Description("the dataContext of kview engine, the html render engine of kooboo. You can explicitly set value into datacontext or just declare the value as JS global variable, it will be accesible from kview engine as well.")]
        kDataContext DataContext { get; }
        kDatabase DB { get; }
        KDiagnosis diagnosis { get; set; }
        KScriptExtension ex { get; }
        KScriptExtension Extension { get; }

        [Description("Provide read and write access to text or binary files under the site folder. Below is fully functioning example.")]
        FileIO File { get; }

        [Description("Access to current request information.")]
        k.InfoModel Info { get; }

        [Description("Kooboo KeyValue store per site. Both key and value are string only.")]
        kKeyValue KeyValue { get; }

        [Description("Send an email. You may need credit to send internet emails on online version")]
        Global.Mail mail { get; }
        RenderContext RenderContext { get; set; }

        [Description("kScript is a server side JavaScript engine that support full ES5 syntax, with an additional k object. Kooboo use kScript to write extensions like Job, Event Activity, Database operation, etc. You may embed KScript in your html file(view, layout, page) with a script engine=kscript tag, or create external kScript file")]
        Request Request { get; }

        [Description("The http response object that is used to set data into http resposne stream")]
        Response Response { get; }
        
        [Description("Private MD5 and SHA1 encryption and a simple two way encrypt and decrypt function")]
        Security Security { get; }

        [Description("a temporary storage for small interactive information. Session does not persist")]
        Session Session { get; }

        [Description("The Kooboo website database with version control")]
        kSiteDb Site { get; }
        kSiteDb SiteDb { get; }
        KTemplate Template { get; }

        [Description("Get content from or post data to remote url.")]
        Curl Url { get; }

        [Description("Kooboo login user available from version 1.1 and up")]
        UserInfoModel User { get; }
        Dictionary<string, string> ViewData { get; }

        void export(object obj);
        void Help();
        void Import(string codename);
        void ImportCode(string codename);
        void output(object obj);
        void ViewHelp();
    }
}