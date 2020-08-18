//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Jint.Native;
using Kooboo;
using Kooboo.Data;
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Scripting;
using Kooboo.Sites.Scripting.Global;
using Kooboo.Sites.Scripting.Global.Mysql;
using Kooboo.Sites.Scripting.Global.SMS;
using Kooboo.Sites.Scripting.Global.Sqlite;
using KScript.KscriptConfig;
using KScript.Sites;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace KScript
{
    public partial class k
    {
        private object _locker = new object();

        [KIgnore]
        public RenderContext RenderContext { get; set; }

        public k(RenderContext context)
        {
            this.RenderContext = context;
        }

        public k GetBySite(string SiteName)
        {
            var orgid = this.RenderContext.WebSite.OrganizationId;
            var allsites = Kooboo.Data.GlobalDb.WebSites.ListByOrg(orgid);

            if (allsites == null || !allsites.Any())
            {
                return null;
            }

            var find = allsites.Find(o => o.Name == SiteName);
            if (find == null)
            {
                find = allsites.Find(o => o.DisplayName == SiteName);
            }

            if (find == null)
            {
                return null;
            }

            RenderContext newcontext = new RenderContext();
            newcontext.Request = this.RenderContext.Request;
            newcontext.User = this.RenderContext.User;
            newcontext.WebSite = find;
            newcontext.IsSiteBinding = true;
            return new k(newcontext);
        }

        private WebSite _findsite(Guid id)
        {
            Guid orgid = this.RenderContext.WebSite.OrganizationId;
            List<WebSite> allsites = Kooboo.Data.GlobalDb.WebSites.ListByOrg(orgid);

            if (allsites == null || !allsites.Any())
            {
                return null;
            }

            return allsites.Find(o => o.Id == id);
        }

        public k GetBySiteId(object SiteId)
        {  
            Guid id = Kooboo.Lib.Helper.IDHelper.ParseKey(SiteId);
            WebSite find = _findsite(id); 

            if (find == null)
            {
                return null;
            }

            RenderContext newcontext = new RenderContext();
            newcontext.Request = this.RenderContext.Request;
            newcontext.User = this.RenderContext.User;
            newcontext.WebSite = find;
            newcontext.IsSiteBinding = true;
            return new k(newcontext);
        }
         

        [KIgnore]
        public object this[string key] { get { return ExtensionContainer.Get(key, RenderContext); } set { ExtensionContainer.Set(value); } }

        [KExtension]
        static KeyValuePair<string, Type>[] _ = ExtensionContainer.List.ToArray();

        [KExtension]
        static KeyValuePair<string, Type[]>[] __ = KscriptConfigContainer.KscriptConfigTypes.ToArray();

        private kDataContext _data;

        [Description("the dataContext of kview engine, the html render engine of kooboo. You can explicitly set value into datacontext or just declare the value as JS global variable, it will be accesible from kview engine.")]
        public kDataContext DataContext
        {
            get
            {
                if (_data == null)
                {
                    lock (_locker)
                    {
                        if (_data == null)
                        {
                            _data = new kDataContext(this.RenderContext);
                        }
                    }
                }
                return _data;
            }
        }

        private Response _response;

        [Description("The HTTP response object that is used to set data into http resposne stream")]
        public Response Response
        {
            get
            {
                if (_response == null)
                {
                    lock (_locker)
                    {
                        if (_response == null)
                        {
                            _response = new Response(this.RenderContext);
                        }
                    }
                }
                return _response;
            }
        }

        private Request _request;


        [Description(@"Access to the http request data, query string, form or headers. Cookie is available from k.cookie.
var value = k.request.queryname;
var value = k.request.queryString.queryname;
var value = k.request.form.queryname;")]
        public Request Request
        {
            get
            {
                if (_request == null)
                {
                    lock (_locker)
                    {
                        if (_request == null)
                        {
                            _request = new Request(this.RenderContext);
                        }
                    }
                }
                return _request;
            }
        }

        private Session _session;

        [Description(@"a temporary storage for small interactive information. Session does not persist
   k.session.set(""key"", obj);
   var back = k.session.get(""key"");
k.session.newkey = ""value""; 
var value = k.session.key; ")]
        public Session Session
        {
            get
            {
                if (_session == null)
                {
                    lock (_locker)
                    {
                        if (_session == null)
                        {
                            _session = Manager.GetOrSetSession(this.RenderContext);
                        }
                    }
                }
                return _session;
            }
        }

        private DocumentObjectModel _dom;

        [Description("Document objecgt model")]
        public DocumentObjectModel Dom
        {
            get
            {
                if (_dom == null)
                {
                    _dom = new DocumentObjectModel();
                }
                return _dom;
            }
            set
            {
                _dom = value;
            }
        }

        [Description("Send SMS notification")]
        public SMS sms
        {
            get
            {
                return new SMS(this.RenderContext); 
            }
        }

        private KDictionary _viewdata;

        [Description("Shared current context storage")]
        public KScript.KDictionary ViewData
        {
            get
            {
                if (_viewdata == null)
                {
                    lock (_locker)
                    {
                        if (_viewdata == null)
                        {
                            _viewdata = new KDictionary();
                        }
                    }
                }
                return _viewdata;
            }
        }

        private InfoModel _siteinfo;

        [Description("Access to current request information")]
        public InfoModel Info
        {
            get
            {
                if (_siteinfo == null)
                {
                    lock (_locker)
                    {
                        if (_siteinfo == null)
                        {
                            _siteinfo = new InfoModel(this.RenderContext);
                        }
                    }

                }
                return _siteinfo;
            }
        }

        private UserInfoModel _user;
        public UserInfoModel User
        {
            get
            {
                if (_user == null)
                {
                    _user = new UserInfoModel(this.RenderContext);
                }
                return _user;
            }
        }
         
        private FileVersion _version;

        [Description("append a version nr to image url for caching. Only valid when system set to image version cache")]
        public FileVersion Version
        {
            get
            {
                if (_version == null)
                {
                    _version = new FileVersion(this.RenderContext);
                }
                return _version;
            }
        }
         
        private KTemplate _template;
        [KIgnore]
        public KTemplate Template
        {
            get
            {
                if (_template == null)
                {
                    _template = new KTemplate(this.RenderContext);
                }
                return _template;
            }
        }


        private kSiteDb _sitedb;

        [KIgnore]
        public kSiteDb SiteDb
        {
            get
            {
                if (_sitedb == null)
                {
                    lock (_locker)
                    {
                        if (_sitedb == null)
                        {
                            if (this.RenderContext.WebSite != null)
                            {
                                _sitedb = new kSiteDb(this.RenderContext);
                            }
                        }
                    }
                }
                return _sitedb;
            }
        }


        [Description("The Kooboo website database with version control")]
        public kSiteDb Site
        {
            get
            {
                return this.SiteDb;
            }
        }

        private Curl _url;
        public Curl Url
        {
            get
            {
                if (_url == null)
                {
                    lock (_locker)
                    {
                        if (_url == null)
                        {
                            _url = new Curl(RenderContext);
                        }
                    }
                }
                return _url;
            }
        }

        private FileIO _file;


        [Description("Provide read and write access to text or binary files under the site folder")]
        public FileIO File
        {
            get
            {
                if (_file == null)
                {
                    lock (_locker)
                    {
                        if (_file == null)
                        {
                            _file = new FileIO(this.RenderContext);
                        }
                    }
                }
                return _file;
            }
        }

        private Cookie _cookie;

        [Description("Get or set cookie value")]
        public Cookie Cookie
        {
            get
            {
                if (_cookie == null)
                {
                    lock (_locker)
                    {
                        if (_cookie == null)
                        {
                            _cookie = new Cookie(this.RenderContext);
                        }
                    }
                }
                return _cookie;
            }
        }

        [Description("Access to configuration of current event")]

        private KDictionary _config;
        public KDictionary config
        {
            get
            {
                if (_config == null)
                {
                    _config = new KDictionary();
                }
                return _config;
            }
            set { _config = value; }
        }

        [Kooboo.Attributes.SummaryIgnore]
        public Kooboo.Sites.FrontEvent.IFrontEvent @event { get; set; }

        public Kooboo.Sites.Diagnosis.KDiagnosis diagnosis { get; set; }

        private kKeyValue _sitestore;

        [Description("The database key value storage")]
        public kKeyValue KeyValue
        {
            get
            {
                if (_sitestore == null)
                {
                    lock (_locker)
                    {
                        if (_sitestore == null)
                        {
                            _sitestore = new kKeyValue(this.RenderContext);
                        }
                    }
                }
                return _sitestore;
            }
        }

        private IDatabase _database;

        public IDatabase Database
        {
            get
            {
                if (_database == null)
                {
                    lock (_locker)
                    {
                        if (_database == null)
                        {
                            _database = new kDatabase(this.RenderContext);
                        }
                    }
                }
                return _database;
            }
        }

        private SqliteDatabase _sqlite;
        static object _sqliteLocker = new object();

        public SqliteDatabase Sqlite
        {
            get
            {
                if (_sqlite == null)
                {
                    lock (_sqliteLocker)
                    {
                        if (_sqlite == null)
                        {
                            var path = Path.Combine(AppSettings.GetFileIORoot(RenderContext.WebSite), "sqlite.db");
                            _sqlite = new SqliteDatabase($"Data source='{path}';");
                        }
                    }
                }
                return _sqlite;
            }
        }

        private MysqlDatabase _mysql;
        static object _mysqlLocker = new object();

        public MysqlDatabase Mysql
        {
            get
            {
                if (_mysql == null)
                {
                    lock (_mysqlLocker)
                    {
                        if (_mysql == null)
                        {
                            var setting = RenderContext.WebSite.SiteDb().CoreSetting.GetSetting<MysqlSetting>();

                            if (setting == null || string.IsNullOrWhiteSpace(setting.ConnectionString))
                            {
                                throw new InitException("  ->Please add the mysql connection string to the system configuration of the site<-  ");
                            }

                            _mysql = new MysqlDatabase(setting.ConnectionString);
                        }
                    }
                }
                return _mysql;
            }
        }

        private SqlServerDatabase _sqlServer;
        static object _sqlServerLocker = new object();

        public SqlServerDatabase SqlServer
        {
            get
            {
                if (_sqlServer == null)
                {
                    lock (_sqlServerLocker)
                    {
                        if (_sqlServer == null)
                        {
                            var setting = RenderContext.WebSite.SiteDb().CoreSetting.GetSetting<SqlServerSetting>();

                            if (setting == null || string.IsNullOrWhiteSpace(setting.ConnectionString))
                            {
                                throw new InitException("  ->Please add the sqlserver connection string to the system configuration of the site<-  ");
                            }
                            _sqlServer = new SqlServerDatabase(setting.ConnectionString);
                        }
                    }
                }
                return _sqlServer;
            }
        }

        private MongoDatabase _mongo;
        static object _mongoLocker = new object();

        public MongoDatabase Mongo
        {
            get
            {
                if (_mongo == null)
                {
                    lock (_mongoLocker)
                    {
                        if (_mongo == null)
                        {
                            var setting = RenderContext.WebSite.SiteDb().CoreSetting.GetSetting<MongoSetting>();

                            if (setting == null || string.IsNullOrWhiteSpace(setting.ConnectionString))
                            {
                                throw new InitException("  ->Please add the mongodb connection string to the system configuration of the site<-  ");
                            }

                            var url = new MongoUrl(setting.ConnectionString);
                            var databaseName = url.DatabaseName ?? $"db_{RenderContext.WebSite.Name.ToString()}";
                            var client = new MongoClient(url);
                            _mongo = new MongoDatabase(client.GetDatabase(databaseName));
                        }
                    }
                }
                return _mongo;
            }
        }

        [KIgnore]
        public IDatabase DB
        {

            get { return this.Database; }
        }

        private KScriptExtension _ex;
        [KIgnore]
        public KScriptExtension Extension
        {
            get
            {
                if (_ex == null)
                {
                    lock (_locker)
                    {
                        if (_ex == null)
                        {
                            _ex = new KScriptExtension(this.RenderContext);
                        }
                    }
                }
                return _ex;
            }
        }

        public KScriptExtension ex
        {
            get
            {
                return this.Extension;
            }
        }

        private KScript.Mail _mail;
        public KScript.Mail mail
        {
            get
            {
                if (_mail == null)
                {
                    lock (_locker)
                    {
                        if (_mail == null)
                        {
                            _mail = new KScript.Mail(this.RenderContext);
                        }
                    }
                }
                return _mail;
            }
        }

        Security _security;

        [Description("One way and two way encryption")]
        public Security Security
        {
            get
            {
                if (_security == null)
                {
                    lock (_locker)
                    {
                        if (_security == null)
                        {
                            _security = new Security();
                        }
                    }
                }
                return _security;
            }
        }

        [Description("Import and execute another KScript")]
        public void Import(string codename)
        {
            var sitedb = this.RenderContext.WebSite.SiteDb();
            var code = sitedb.Code.Get(codename);
            if (code != null)
            {
                var result = Kooboo.Sites.Scripting.Manager.ExecuteCode(this.RenderContext, code.Body, code.Id);
                if (result != null)
                {
                    Response.write(result);
                }
            }
        }

        [KIgnore]
        public void ImportCode(string codename)
        {
            Import(codename);
        }

        #region APIHelper

        [KIgnore]
        public void Help()
        {
            //var html = new HelperRender().Render(RenderContext);
            var html = new Kooboo.Sites.Scripting.Helper.ScriptHelper.ScriptHelperRender().Render(RenderContext);
            Response.write(html);
            //Help("k");
        }

        [KIgnore]
        public void ViewHelp()
        {
            var html = new ViewHelpRender().Render(RenderContext);
            Response.write(html);
        }

        internal List<object> ReturnValues = new List<object>();


        [Description("return value to the caller")]
        public void output(object obj)
        {
            this.ReturnValues.Add(obj);
        }

        [Description("return value to the caller")]
        public void export(object obj)
        {
            output(obj);
        }

        private Type GetPropertyType(string fullname)
        {
            string spe = ".";
            fullname = fullname.ToLower().Replace("k.", "");
            var psname = fullname.Split(spe.ToCharArray()).ToList();

            //walkaround:cz
            var currenttype = this.GetType();
            foreach (var item in psname)
            {
                var allpreps = _GetProperties(currenttype);

                foreach (var p in allpreps)
                {
                    if (p.Key.ToLower() == item)
                    {
                        currenttype = p.Value;
                    }
                }
            }
            return currenttype;
        }

        private List<PropertyViewModel> GetProperty(Type type, string methodbase)
        {
            var result = new List<PropertyViewModel>();

            var allparas = _GetProperties(type);

            foreach (var item in allparas)
            {
                var model = new PropertyViewModel() { Name = methodbase + "." + item.Key, ReturnType = item.Value };

                result.Add(model);
            }
            return result;
        }

        private string print(List<PropertyViewModel> input, string baseurl)
        {
            if (input == null || input.Count() == 0)
            {
                return string.Empty;
            }

            string result = "<br/><br/><h3>----- Properties -----</h3>";

            foreach (var item in input)
            {
                if (item.ReturnType == null || item.ReturnType == typeof(string) || !item.ReturnType.IsClass)
                {
                    result += "<br/><br/><b>" + item.Name + "</b>";
                    bool iscol = Kooboo.Lib.Reflection.TypeHelper.IsGenericCollection(item.ReturnType);

                    if (iscol)
                    {
                        var coltype = Kooboo.Lib.Reflection.TypeHelper.GetEnumberableType(item.ReturnType);
                        result += "<br/>Return type:  Array";
                        result += "<br/>Data type: " + coltype.Name;
                    }
                    else
                    {
                        result += "<br/>Return type: " + item.ReturnType.Name;
                    }
                }
                else
                {
                    string name = item.Name;
                    var para = new Dictionary<string, string>();
                    para.Add("name", name);
                    string url = Kooboo.Lib.Helper.UrlHelper.AppendQueryString(baseurl, para);
                    result += "<br/><br/> <a href='" + url + "'><b>" + name + "</b></a>";
                }
            }

            return result;
        }

        private string print(List<MethodViewModel> input)
        {
            if (input == null || input.Count() == 0)
            {
                return string.Empty;
            }
            string result = "<br/><br/><h3>----- Methods -----</h3>";

            foreach (var item in input)
            {
                string name = item.Name;
                result += "<br/><br/><b> " + name;

                string para = string.Empty;

                if (item.Parameters != null && item.Parameters.Count > 0)
                {
                    foreach (var p in item.Parameters)
                    {
                        para += p.Value + " " + p.Key + ", ";
                    }
                    para = para.TrimEnd(' ');
                    para = para.TrimEnd(',');
                }
                result += "(" + para + ")</b>";

                if (item.ReturnType != typeof(void))
                {
                    bool iscol = Kooboo.Lib.Reflection.TypeHelper.IsGenericCollection(item.ReturnType);

                    if (iscol)
                    {
                        var coltype = Kooboo.Lib.Reflection.TypeHelper.GetEnumberableType(item.ReturnType);
                        result += "<br/>Return type: Array";
                        result += "<br/>Data type: " + coltype.Name;
                    }
                    else
                    {
                        result += "<br/>Return type: " + item.ReturnType.Name;
                    }
                }

                if (!string.IsNullOrEmpty(item.Sample))
                {
                    result += "<br/><br/>Sample:<br/> " + item.Sample;
                }
            }

            return result;
        }

        private List<MethodViewModel> GetMethod(Type type, string MethodPathBase)
        {
            if (type == typeof(KScriptExtension))
            {
                return new List<MethodViewModel>();
            }

            var methods = new List<MethodViewModel>();

            var allmethods = Kooboo.Lib.Reflection.TypeHelper.GetPublicMethods(type);

            foreach (var item in allmethods)
            {
                var requirepara = item.GetCustomAttribute(typeof(Kooboo.Attributes.SummaryIgnore));
                if (requirepara == null)
                {
                    var model = new MethodViewModel();
                    model.Name = MethodPathBase + "." + item.Name;
                    var paras = item.GetParameters();
                    foreach (var p in paras)
                    {
                        model.Parameters.Add(p.Name, p.ParameterType.Name);
                    }

                    model.ReturnType = item.ReturnType;

                    if (item.ReturnType != typeof(void))
                    {
                        if (item.ReturnType.IsClass && item.ReturnType != typeof(string))
                        {
                            var SampleResponseData = Kooboo.Lib.Development.FakeData.GetFakeValue(item.ReturnType);
                            model.Sample = Kooboo.Lib.Helper.JsonHelper.Serialize(SampleResponseData);
                        }
                    }
                    methods.Add(model);
                }
            }

            return methods;
        }

        private Dictionary<string, Type> _GetProperties(Type objectType)
        {
            if (objectType == typeof(KScriptExtension))
            {
                return ExtensionContainer.List;
            }

            var fieldlist = new Dictionary<string, Type>();

            FieldInfo[] fieldInfo = objectType.GetFields();

            foreach (var item in objectType.GetProperties())
            {
                if (item.CanRead && item.PropertyType.IsPublic)
                {
                    var requirepara = item.GetCustomAttribute(typeof(Kooboo.Attributes.SummaryIgnore));
                    if (requirepara == null)
                    {
                        fieldlist.Add(item.Name, item.PropertyType);
                    }
                }
            }
            return fieldlist;
        }

        private List<MethodViewModel> GetRepositoryMethod(Type type, string MethodPathBase)
        {
            var methods = new List<MethodViewModel>();

            var repo = Activator.CreateInstance(type) as IRepository;

            bool isRoutable = Kooboo.Attributes.AttributeHelper.IsRoutable(repo.ModelType);

            var allmethods = Kooboo.Lib.Reflection.TypeHelper.GetPublicMethods(type);

            var takeMethods = new List<string>();
            if (isRoutable)
            {
                takeMethods.Add("GetByUrl");
            }
            takeMethods.Add("Get");
            takeMethods.Add("All");

            foreach (var methodname in takeMethods)
            {
                var finds = allmethods.FindAll(o => o.Name == methodname);
                if (finds == null || finds.Count() == 0)
                {
                    continue;
                }
                var item = finds[0];

                int paracount = item.GetParameters().Count();

                foreach (var f in finds)
                {
                    var count = f.GetParameters().Count();
                    if (count < paracount)
                    {
                        item = f;
                        paracount = count;
                    }
                }

                var model = new MethodViewModel();
                model.Name = MethodPathBase + "." + item.Name;
                var paras = item.GetParameters();
                foreach (var p in paras)
                {
                    model.Parameters.Add(p.Name, p.ParameterType.Name);
                }

                model.ReturnType = item.ReturnType;

                if (item.ReturnType != typeof(void))
                {
                    if (item.ReturnType.IsClass && item.ReturnType != typeof(string))
                    {
                        var SampleResponseData = Kooboo.Lib.Development.FakeData.GetFakeValue(item.ReturnType);
                        model.Sample = Kooboo.Lib.Helper.JsonHelper.Serialize(SampleResponseData);
                    }
                }
                methods.Add(model);
            }

            return methods;
        }

        #endregion

    }

    public class MethodViewModel
    {
        public MethodViewModel()
        {
            this.Name = string.Empty;
            this.Sample = string.Empty;
            this.Parameters = new Dictionary<string, string>();
        }

        public string Name { get; set; }
        public Dictionary<string, string> Parameters { get; set; }

        public string Sample { get; set; }
        public Type ReturnType { get; set; }
    }

    public class PropertyViewModel
    {
        public PropertyViewModel()
        {
            this.Name = string.Empty;
        }
        public string Name { get; set; }

        public Type ReturnType { get; set; }
    }

    public class UserInfoModel
    {
        public UserInfoModel(RenderContext context)
        {
            this.context = context;
        }

        public RenderContext context { get; set; }

        public Guid Id
        {
            get
            {
                if (context.User != null)
                {
                    return context.User.Id;
                }
                return default(Guid);
            }
        }

        public Guid _Id
        {
            get
            {
                if (context.User != null)
                {
                    return context.User.Id;
                }
                return default(Guid);
            }
        }

        public bool IsLogin
        {
            get
            {
                return context.User != null;
            }
        }


        public bool IsOfOrganization(string OrganizationName)
        {
            if (context.User == null)
            {
                return false;
            }
            var OrgId = default(Guid);
            if (!Guid.TryParse(OrganizationName, out OrgId))
            {
                OrgId = Kooboo.Lib.Security.Hash.ComputeGuidIgnoreCase(OrganizationName);
            }

            if (context.User.CurrentOrgId == OrgId)
            {
                return true;
            }

            var orgs = Kooboo.Data.GlobalDb.Users.Organizations(context.User.Id);

            if (orgs.Any())
            {
                var find = orgs.Find(o => o.Id == OrgId);
                if (find != null)
                {
                    return true;
                }
            }
            return false;
        }

        public Kooboo.Data.Models.User Get(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return null;
            return Kooboo.Data.GlobalDb.Users.Get(userName);
        }

        public void EnsureLogin(string redirectUrl)
        {
            if (!this.IsLogin)
            {
                this.context.Response.Redirect(302, redirectUrl);
                this.context.Response.End = true;
            }
        }

        public string UserName
        {
            get
            {
                if (context.User != null)
                {
                    return context.User.UserName;
                }
                return null;
            }
        }

        public string FirstName
        {
            get
            {
                if (context.User != null)
                {
                    return context.User.FirstName;
                }
                return null;
            }
        }

        public string LastName
        {
            get
            {
                if (context.User != null)
                {
                    return context.User.LastName;
                }
                return null;
            }
        }

        public string Language
        {
            get
            {
                if (context.User != null)
                {
                    return context.User.LastName;
                }
                return null;
            }
        }

        public string Email
        {
            get
            {
                if (context.User != null)
                {
                    return context.User.EmailAddress;
                }
                return null;
            }
        }

        public Kooboo.Data.Models.User Login(string username, string password)
        {
            var user = Kooboo.Data.GlobalDb.Users.Validate(username, password);

            if (user != null)
            {
                context.Response.AppendCookie(DataConstants.UserApiSessionKey, user.Id.ToString(), 30);
                context.User = user;
                return user;
            }

            return null;
        }

        public bool Exists(string UserName)
        {
            return Kooboo.Data.GlobalDb.Users.Get(UserName) != null;
        }

        [Obsolete]
        public bool ExistEmail(string email)
        {
            return Kooboo.Data.GlobalDb.Users.GetByEmail(email) != null;
        }

        public bool EmailExists(string email)
        {
            return Kooboo.Data.GlobalDb.Users.GetByEmail(email) != null;
        }

        public void Logout()
        {
            // log user out. 
            context.Response.DeleteCookie(DataConstants.UserApiSessionKey);
        }
    }
}
