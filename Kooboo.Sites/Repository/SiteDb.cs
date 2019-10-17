//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using Kooboo.Data.Models;
using Kooboo.Sites.Routing;
using Kooboo.Sites.SiteTransfer;
using Kooboo.Data;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Relation;
using Kooboo.Data.Interface;
using Kooboo.Sites.ThreadPool;
using Kooboo.Sites.ViewModel;
using Kooboo.Lib.Helper;
using Kooboo.Sites.SiteTransfer.Model;
using System.Linq;
using System.Threading;
//using Kooboo.Sites.Ecommerce.Repository;
//using Kooboo.Sites.Ecommerce.Models;

namespace Kooboo.Sites.Repository
{
    public class SiteDb
    {
        public Guid Id
        {
            get { return this.WebSite.Id; }
        }

        public SiteDb(WebSite website)
        {
            this.WebSite = website; 
            this.DatabaseDb = DB.GetDatabase(website);
            this.SiteRepos = new Dictionary<string, IRepository>(StringComparer.OrdinalIgnoreCase);
        }

        public DiskSize GetSize()
        {
            DiskSize disksize = new DiskSize();

            long totallength = 0;

            foreach (var item in this.ActiveRepositories())
            {
                RepoSize reposize = new RepoSize();
                reposize.Name = item.StoreName;
                string folder = item.Store.ObjectFolder;
                reposize.Length = IOHelper.GetDirectorySize(folder);
                reposize.ItemCount = item.Store.Count();
                disksize.RepositorySize.Add(reposize);
                totallength += reposize.Length;
            }

            disksize.RepositoryEditCount = this.Log.Store.Count();

            //disksize.ImageLogWeeks = this.GetLogWeekNames(LogType.Image);
            disksize.VisitorLogWeeks = this.LogWeekNames();
            //disksize.ImageLog = IOHelper.GetDirectorySize(this.LogFolder); 
            disksize.VisitorLog = IOHelper.GetDirectorySize(this.LogFolder);

            long editlogsize = IOHelper.GetDirectorySize(this.Log.Store.ObjectFolder);

            totallength += disksize.VisitorLog;
            totallength += editlogsize;

            disksize.Total = totallength;
            return disksize;
        }

        public IRepository GetRepository(Type ModelType)
        {
            return GetSiteRepositoryByModelType(ModelType);
        }

        public IRepository GetRepository(byte ConstType)
        {
            var modeltype = Service.ConstTypeService.GetModelType(ConstType);
            return GetRepository(modeltype);
        }

        public IRepository GetRepository(string StoreName)
        {
            var repotype = SiteRepositoryContainer.GetRepoTypeInfo(StoreName);
            return GetSiteRepository(repotype);
        }

        private object _lock = new object();

        private string _name;
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    _name = this.DatabaseDb.Name;
                }
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public Database DatabaseDb { get; set; }

        public WebSite WebSite { get; set; }



        private LayoutRepository _layouts;
        public LayoutRepository Layouts
        {
            get
            {
                if (_layouts == null)
                {
                    _layouts = GetSiteRepository<LayoutRepository, Layout>();
                }
                return _layouts;
            }
        }

        private ContinueConvertRepository _continueConvert;
        public ContinueConvertRepository ContinueConverter
        {
            get
            {
                if (_continueConvert == null)
                {
                    _continueConvert = GetSiteRepository<ContinueConvertRepository, ContinueConverter>();
                }
                return _continueConvert;
            }
        }

        private DataMethodSettingRepository _datamethodSettings;
        public DataMethodSettingRepository DataMethodSettings
        {
            get
            {
                if (_datamethodSettings == null)
                {
                    _datamethodSettings = GetSiteRepository<DataMethodSettingRepository, DataMethodSetting>();
                }
                return _datamethodSettings;
            }
        }

        private SyncSettingRepository _syncsetting;

        public SyncSettingRepository SyncSettings
        {
            get
            {
                if (_syncsetting == null)
                {
                    _syncsetting = GetSiteRepository<SyncSettingRepository, SyncSetting>();
                }
                return _syncsetting;
            }
        }

        private StorePool<Image> _imagepool;
        public StorePool<Image> ImagePool
        {
            get
            {
                if (_imagepool == null)
                {
                    _imagepool = new StorePool<Image>(this.Images);
                }
                return _imagepool;
            }
        }


        private SynchronizationRepository _sychronization;
        public SynchronizationRepository Synchronization
        {
            get
            {
                if (_sychronization == null)
                {
                    _sychronization = GetSiteRepository<SynchronizationRepository, Synchronization>();
                }
                return _sychronization;
            }
        }


        private CoreSettingRepository _coresetting;

        public CoreSettingRepository CoreSetting
        {
            get
            {
                if (_coresetting == null)
                {
                    _coresetting = GetSiteRepository<CoreSettingRepository, CoreSetting>();
                }
                return _coresetting;
            }
        }

        private SiteClusterRepository _sitecluster;

        public SiteClusterRepository SiteCluster
        {
            get
            {
                if (_sitecluster == null)
                {
                    _sitecluster = GetSiteRepository<SiteClusterRepository, SiteCluster>();
                }
                return _sitecluster;
            }
        }


        private Kooboo.Sites.Sync.SiteClusterSync.SiteClusterManager _ClusterManager;

        public Kooboo.Sites.Sync.SiteClusterSync.SiteClusterManager ClusterManager
        {
            get
            {
                if (_ClusterManager == null)
                {
                    if (this.WebSite.EnableCluster)
                    {
                        _ClusterManager = new Sync.SiteClusterSync.SiteClusterManager(this);
                    }
                }
                return _ClusterManager;
            }
        }

        private MenuRepository _menus;
        public MenuRepository Menus
        {
            get
            {
                if (_menus == null)
                {
                    _menus = GetSiteRepository<MenuRepository, Menu>();
                }
                return _menus;
            }
        }


        public TransferTaskRepository TransferTasks => GetSiteRepository<TransferTaskRepository, TransferTask>();

        public TransferPageRepository TransferPages => GetSiteRepository<TransferPageRepository, TransferPage>();


        #region newSiteRepo

        private object _repolocker = new object();

        internal bool AllLoaded = false;   // where it should check to load all existing stores or not. 

        public Dictionary<string, IRepository> SiteRepos { get; set; }

        // this may have slight better performance. 
        public T GetSiteRepository<T, TModel>()
            where T : IRepository
            where TModel : class, ISiteObject
        {
            var instance = GetSiteRepository(typeof(T), typeof(TModel));
            if (instance != null)
            {
                return (T)instance;
            }
            return default(T);
        }

        public List<IRepository> ActiveRepositories()
        {
            if (!AllLoaded)
            {
                lock (_repolocker)
                {
                    if (!AllLoaded)
                    {
                        LoadExistingStore();
                    }
                    AllLoaded = true;
                }
            }

            return SiteRepos.Values.ToList();
        }


        internal void LoadExistingStore()
        {
            List<Type> NoActiveTypes = new List<Type>();
            foreach (var item in SiteRepositoryContainer.Repos)
            {
                if (!SiteRepos.ContainsKey(item.Key))
                {
                    if (this.DatabaseDb.HasObjectStore(item.Key))
                    {
                        NoActiveTypes.Add(item.Value);
                    }
                }
            }

            foreach (var item in NoActiveTypes)
            {
                var repo = GetSiteRepository(item);
                if (repo != null)
                {
                    SiteRepos[repo.StoreName] = repo;
                }
            }
        }

        public T GetSiteRepository<T>() where T : IRepository
        {
            var instance = GetSiteRepository(typeof(T));

            if (instance == null)
            {
                return default(T);
            }

            return (T)instance;
        }

        public IRepository GetSiteRepository(Type RepositoryType)
        {
            if (RepositoryType == null)
            {
                return null;
            }
            var modeltype = Lib.Reflection.TypeHelper.GetGenericType(RepositoryType);
            return GetSiteRepository(RepositoryType, modeltype);
        }


        public IRepository GetSiteRepository(Type RepositoryType, Type siteModelType)
        {
            if (RepositoryType == null || siteModelType == null)
            {
                return null;
            }
            string name = siteModelType.Name;
            if (!SiteRepos.ContainsKey(name))
            {
                lock (_repolocker)
                {
                    if (!SiteRepos.ContainsKey(name))
                    {
                        var instance = Activator.CreateInstance(RepositoryType) as IRepository;

                        if (instance == null)
                        {
                            return null;
                        }
                        else
                        {
                            if (instance is ISiteRepositoryBase)
                            {
                                var sitebase = instance as ISiteRepositoryBase;
                                sitebase.SiteDb = this;
                                sitebase.init();
                            }
                            SiteRepos[name] = instance;
                        }
                    }

                }
            }

            return SiteRepos[name];
        }

        public IRepository GetSiteRepositoryByModelType(Type ModelType)
        {
            var repotype = SiteRepositoryContainer.GetRepoTypeInfo(ModelType);
            return GetSiteRepository(repotype, ModelType);
        }

        public bool IsStoreExists(Type ModelType)
        {
            return this.DatabaseDb.HasObjectStore(ModelType.Name);
        }

        #endregion

        private CmsFileRepository _files;
        public CmsFileRepository Files
        {
            get
            {
                if (_files == null)
                {
                    _files = GetSiteRepository<CmsFileRepository, CmsFile>();
                }
                return _files;
            }
        }


        private FolderRepository _folders;
        public FolderRepository Folders
        {
            get
            {
                if (_folders == null)
                {
                    _folders = GetSiteRepository<FolderRepository, Folder>();
                }
                return _folders;
            }
        }

        private DomElementRepository _domelements;

        public DomElementRepository DomElements
        {
            get
            {
                if (_domelements == null)
                {
                    _domelements = GetSiteRepository<DomElementRepository, DomElement>();
                }
                return _domelements;
            }
        }


        private RouteRepository _routes;
        public RouteRepository Routes
        {
            get
            {
                if (_routes == null)
                {
                    _routes = GetSiteRepository<RouteRepository, Route>();
                }
                return _routes;
            }
        }


        private FormRepository _forms;
        public FormRepository Forms
        {
            get
            {
                if (_forms == null)
                {
                    _forms = GetSiteRepository<FormRepository, Form>();
                }
                return _forms;
            }
        }



        private FormSettingRepository _formsetting;
        public FormSettingRepository FormSetting
        {
            get
            {
                if (_formsetting == null)
                {
                    _formsetting = GetSiteRepository<FormSettingRepository, FormSetting>();
                }
                return _formsetting;
            }
        }

        private FormValueRepository _formvalues;

        public FormValueRepository FormValues
        {
            get
            {
                if (_formvalues == null)
                {
                    _formvalues = GetSiteRepository<FormValueRepository, FormValue>();
                }
                return _formvalues;
            }
        }

        private ImageRepository _images;
        public ImageRepository Images
        {
            get
            {
                if (_images == null)
                {
                    _images = GetSiteRepository<ImageRepository, Image>();
                }
                return _images;
            }
        }

        private PageRepository _pages;
        public PageRepository Pages
        {
            get
            {
                if (_pages == null)
                {
                    _pages = GetSiteRepository<PageRepository, Page>();
                }
                return _pages;
            }
        }

        private ViewRepository _views;
        public ViewRepository Views
        {
            get
            {
                if (_views == null)
                {
                    _views = GetSiteRepository<ViewRepository, View>();
                }
                return _views;
            }
        }

        private ScriptRepository _scripts;
        public ScriptRepository Scripts
        {
            get
            {
                if (_scripts == null)
                {
                    _scripts = GetSiteRepository<ScriptRepository, Script>();
                }
                return _scripts;
            }
        }


        private CodeRepository _code;
        public CodeRepository Code
        {
            get
            {
                if (_code == null)
                {
                    _code = GetSiteRepository<CodeRepository, Code>();
                }
                return _code;
            }
        }

        private BusinessRuleRepository _rules;
        public BusinessRuleRepository Rules => GetSiteRepository<BusinessRuleRepository, BusinessRule>();

        private RelationRepository _relation;
        public RelationRepository Relations
        {
            get
            {
                if (_relation == null)
                {
                    _relation = GetSiteRepository<RelationRepository, ObjectRelation>();
                }
                return _relation;
            }
        }


        private SearchIndexRepository _searchindex;
        public SearchIndexRepository SearchIndex
        {
            get
            {
                if (_searchindex == null)
                {
                    lock (_lock)
                    {
                        if (_searchindex == null)
                        {
                            _searchindex = new SearchIndexRepository(this);
                        }
                    }
                }
                return _searchindex;
            }
        }


        private ViewDataMethodRepository _viewdatamethod;

        public ViewDataMethodRepository ViewDataMethods
        {
            get
            {
                if (_viewdatamethod == null)
                {
                    _viewdatamethod = GetSiteRepository<ViewDataMethodRepository, ViewDataMethod>();
                }
                return _viewdatamethod;
            }
        }


        public DownloadFailTrackRepository DownloadFailedLog => GetSiteRepository<DownloadFailTrackRepository, DownloadFailTrack>();

        private SiteUserRepository _Siteuser;

        public SiteUserRepository SiteUser
        {
            get
            {
                if (_Siteuser == null)
                {
                    _Siteuser = GetSiteRepository<SiteUserRepository, SiteUser>();
                }
                return _Siteuser;
            }
        }

        public PathTree RouteTree(byte ConstType = 0)
        {
            return Cache.RouteTreeCache.RouteTree(this, ConstType);
        }

        #region "log"

        private string _LogFolder;
        internal string LogFolder
        {
            get
            {
                if (_LogFolder == null)
                {
                    var folder = AppSettings.GetOrganizationFolder(this.WebSite.OrganizationId);
                    folder = System.IO.Path.Combine(folder, this.WebSite.Name);
                    _LogFolder = System.IO.Path.Combine(folder, "visitorlog");
                    Lib.Helper.IOHelper.EnsureDirectoryExists(_LogFolder);
                }
                return _LogFolder;
            }
        }

        public List<string> LogWeekNames()
        {
            List<string> result = new List<string>();
            string folder = LogFolder;
            var subs = System.IO.Directory.GetDirectories(folder);

            if (subs != null)
            {
                foreach (var item in subs)
                {
                    string name = null;

                    int lastslash = Kooboo.Lib.Compatible.CompatibleManager.Instance.System.GetLastSlash(item);
                    if (lastslash > -1)
                    {
                        name = item.Substring(lastslash + 1);
                    }
                    else
                    {
                        name = item;
                    }

                    result.Add(name);
                }
            }
            return result;

        }

        public Sequence<T> LogByWeek<T>(string weekname = null)
        {
            if (string.IsNullOrEmpty(weekname))
            {
                weekname = _GetWeekName(DateTime.Now);
            }
            string weekfolder = System.IO.Path.Combine(LogFolder, weekname);
            Lib.Helper.IOHelper.EnsureDirectoryExists(weekfolder);

            var filename = typeof(T).Name + ".log";
            string FullFileName = System.IO.Path.Combine(weekfolder, filename);
            return this.DatabaseDb.GetSequence<T>(FullFileName) as Sequence<T>;
        }

        private int GetWeekOfYear(DateTime datetime)
        {
            return System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(datetime, System.Globalization.CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
        }

        internal string _GetWeekName(DateTime datetime)
        {
            var weekname = GetWeekOfYear(datetime);
            string strweekname = weekname > 9 ? weekname.ToString() : "0" + weekname.ToString();
            return datetime.Year.ToString() + "-" + strweekname;
        }

        private Sequence<VisitorLog> _visitorLog;
        private DateTime _visitorlogtime;
        public Sequence<VisitorLog> VisitorLog
        {
            get
            {
                if (_visitorLog == null)
                {
                    lock (_lock)
                    {
                        if (_visitorLog == null)
                        {
                            _visitorlogtime = DateTime.Now;
                            _visitorLog = LogByWeek<VisitorLog>(_GetWeekName(_visitorlogtime));
                        }
                    }
                }

                if (GetWeekOfYear(_visitorlogtime) != GetWeekOfYear(DateTime.Now))
                {
                    _visitorLog = null;
                    return VisitorLog;
                }
                return _visitorLog;
            }
        }

        private Sequence<ImageLog> _imageLog;
        private DateTime _imagelogtime;

        public Sequence<ImageLog> ImageLog
        {
            get
            {
                if (_imageLog == null)
                {
                    lock (_lock)
                    {
                        if (_imageLog == null)
                        {
                            _imagelogtime = DateTime.Now;
                            _imageLog = LogByWeek<ImageLog>(_GetWeekName(_imagelogtime));
                        }
                    }
                }
                if (GetWeekOfYear(_imagelogtime) != GetWeekOfYear(DateTime.Now))
                {
                    _imageLog = null;
                    return ImageLog;
                }
                return _imageLog;
            }
        }

        private Sequence<SiteErrorLog> _ErrorLog;
        private DateTime _errorlogtime;

        public Sequence<SiteErrorLog> ErrorLog
        {
            get
            {
                if (_ErrorLog == null)
                {
                    lock (_lock)
                    {
                        if (_ErrorLog == null)
                        {
                            _errorlogtime = DateTime.Now;
                            _ErrorLog = LogByWeek<SiteErrorLog>(_GetWeekName(_errorlogtime));
                        }
                    }
                }
                if (GetWeekOfYear(_errorlogtime) != GetWeekOfYear(DateTime.Now))
                {
                    _ErrorLog = null;
                    return ErrorLog;
                }
                return _ErrorLog;
            }
        }

        public EditLog Log
        {
            get
            {
                return this.DatabaseDb.Log;
            }
        }


        #endregion


        private StyleRepository _styles;

        public StyleRepository Styles
        {
            get
            {
                if (_styles == null)
                {
                    _styles = GetSiteRepository<StyleRepository, Style>();
                }
                return _styles;
            }
        }

        private ResourceGroupRepository _ResourceGroups;

        public ResourceGroupRepository ResourceGroups
        {
            get
            {
                if (_ResourceGroups == null)
                {
                    _ResourceGroups = GetSiteRepository<ResourceGroupRepository, ResourceGroup>();
                }
                return _ResourceGroups;
            }
        }


        private CmsCssRuleRepository _cssrules;
        public CmsCssRuleRepository CssRules
        {
            get
            {
                if (_cssrules == null)
                {
                    _cssrules = GetSiteRepository<CmsCssRuleRepository, CmsCssRule>();
                }
                return _cssrules;
            }
        }

        private ExternalResourceRepository _ExternalResource;

        public ExternalResourceRepository ExternalResource
        {
            get
            {
                if (_ExternalResource == null)
                {
                    _ExternalResource = GetSiteRepository<ExternalResourceRepository, ExternalResource>();
                }
                return _ExternalResource;
            }
        }

        private ThumbnailRepository _Thumbnails;
        public ThumbnailRepository Thumbnails
        {
            get
            {
                if (_Thumbnails == null)
                {
                    _Thumbnails = GetSiteRepository<ThumbnailRepository, Thumbnail>();
                }
                return _Thumbnails;
            }
        }

        private LabelRepository _labels;
        public LabelRepository Labels
        {
            get
            {
                if (_labels == null)
                {
                    _labels = GetSiteRepository<LabelRepository, Label>();
                }
                return _labels;
            }
        }


        private kConfigRepository _kconfig;

        public kConfigRepository KConfig
        {
            get
            {
                if (_kconfig == null)
                {
                    _kconfig = GetSiteRepository<kConfigRepository, KConfig>();
                }
                return _kconfig;
            }
        }

        private HtmlBlockRepository _htmlblocks;
        public HtmlBlockRepository HtmlBlocks
        {
            get
            {
                if (_htmlblocks == null)
                {
                    _htmlblocks = GetSiteRepository<HtmlBlockRepository, HtmlBlock>();
                }
                return _htmlblocks;
            }
        }


        private ContentFolderRepository _contentfolders;
        public ContentFolderRepository ContentFolders
        {
            get
            {
                if (_contentfolders == null)
                {
                    _contentfolders = GetSiteRepository<ContentFolderRepository, ContentFolder>();
                }
                return _contentfolders;
            }
        }


        private ContentTypeRepository _ContentTypes;
        public ContentTypeRepository ContentTypes
        {
            get
            {
                if (_ContentTypes == null)
                {
                    _ContentTypes = GetSiteRepository<ContentTypeRepository, ContentType>();
                }
                return _ContentTypes;
            }
        }

        private ContentCategoryRepository _contentcategory;

        public ContentCategoryRepository ContentCategories
        {
            get
            {
                if (_contentcategory == null)
                {
                    _contentcategory = GetSiteRepository<ContentCategoryRepository, ContentCategory>();
                }
                return _contentcategory;
            }
        }

        private TextContentRepository _textcontent;
        public TextContentRepository TextContent
        {
            get
            {
                if (_textcontent == null)
                {
                    _textcontent = GetSiteRepository<TextContentRepository, TextContent>();
                }
                return _textcontent;
            }
        } 

        // rebuild the index... 
        public void ClearLog(string[] storenames)
        {
            bool published = this.WebSite.Published;
            bool disksync = this.WebSite.EnableDiskSync;
            bool continuedownload = this.WebSite.ContinueDownload;

            this.WebSite.Published = false;
            this.WebSite.EnableDiskSync = false;
            this.WebSite.ContinueDownload = false;

            ImagePool.ClearAll();
            this.DatabaseDb.Close();

            foreach (var item in storenames)
            {
                if (Lib.Helper.StringHelper.IsSameValue(item, Images.StoreName))
                {
                    ImagePool.ClearAll();
                    Images.Store.Close();
                }

                var repo = this.GetRepository(item);
                if (repo != null && repo is ISiteRepositoryBase)
                {
                    var siterepo = repo as ISiteRepositoryBase;


                    Monitor.Enter(_repolocker);

                    try
                    {
                        siterepo.Reuild();
                    }
                    catch (Exception)
                    {

                    }
                    finally
                    {
                        Monitor.Exit(_repolocker);
                    }

                }

            }

            this.WebSite.Published = published;
            this.WebSite.EnableDiskSync = disksync;
            this.WebSite.ContinueDownload = continuedownload;
        }

    }

}
