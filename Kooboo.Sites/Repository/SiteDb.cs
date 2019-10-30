//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data;
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Models;
using Kooboo.Sites.Relation;
using Kooboo.Sites.Routing;
using Kooboo.Sites.SiteTransfer;
using Kooboo.Sites.SiteTransfer.Model;
using Kooboo.Sites.ThreadPool;
using Kooboo.Sites.ViewModel;
using System;
using System.Collections.Generic;
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
                RepoSize reposize = new RepoSize {Name = item.StoreName};
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

        public IRepository GetRepository(Type modelType)
        {
            return GetSiteRepositoryByModelType(modelType);
        }

        public IRepository GetRepository(byte constType)
        {
            var modeltype = Service.ConstTypeService.GetModelType(constType);
            return GetRepository(modeltype);
        }

        public IRepository GetRepository(string storeName)
        {
            var repotype = SiteRepositoryContainer.GetRepoTypeInfo(storeName);
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
            get { return _layouts ?? (_layouts = GetSiteRepository<LayoutRepository, Layout>()); }
        }

        private ContinueConvertRepository _continueConvert;

        public ContinueConvertRepository ContinueConverter
        {
            get
            {
                return _continueConvert ??
                       (_continueConvert = GetSiteRepository<ContinueConvertRepository, ContinueConverter>());
            }
        }

        private DataMethodSettingRepository _datamethodSettings;

        public DataMethodSettingRepository DataMethodSettings
        {
            get
            {
                return _datamethodSettings ?? (_datamethodSettings =
                           GetSiteRepository<DataMethodSettingRepository, DataMethodSetting>());
            }
        }

        private SyncSettingRepository _syncsetting;

        public SyncSettingRepository SyncSettings
        {
            get { return _syncsetting ?? (_syncsetting = GetSiteRepository<SyncSettingRepository, SyncSetting>()); }
        }

        private StorePool<Image> _imagepool;

        public StorePool<Image> ImagePool
        {
            get { return _imagepool ?? (_imagepool = new StorePool<Image>(this.Images)); }
        }

        private SynchronizationRepository _sychronization;

        public SynchronizationRepository Synchronization
        {
            get
            {
                return _sychronization ??
                       (_sychronization = GetSiteRepository<SynchronizationRepository, Synchronization>());
            }
        }

        private CoreSettingRepository _coresetting;

        public CoreSettingRepository CoreSetting
        {
            get { return _coresetting ?? (_coresetting = GetSiteRepository<CoreSettingRepository, CoreSetting>()); }
        }

        private SiteClusterRepository _sitecluster;

        public SiteClusterRepository SiteCluster
        {
            get { return _sitecluster ?? (_sitecluster = GetSiteRepository<SiteClusterRepository, SiteCluster>()); }
        }

        private Kooboo.Sites.Sync.SiteClusterSync.SiteClusterManager _clusterManager;

        public Kooboo.Sites.Sync.SiteClusterSync.SiteClusterManager ClusterManager
        {
            get
            {
                if (_clusterManager == null)
                {
                    if (this.WebSite.EnableCluster)
                    {
                        _clusterManager = new Sync.SiteClusterSync.SiteClusterManager(this);
                    }
                }
                return _clusterManager;
            }
        }

        private MenuRepository _menus;

        public MenuRepository Menus
        {
            get { return _menus ?? (_menus = GetSiteRepository<MenuRepository, Menu>()); }
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
            List<Type> noActiveTypes = new List<Type>();
            foreach (var item in SiteRepositoryContainer.Repos)
            {
                if (!SiteRepos.ContainsKey(item.Key))
                {
                    if (this.DatabaseDb.HasObjectStore(item.Key))
                    {
                        noActiveTypes.Add(item.Value);
                    }
                }
            }

            foreach (var item in noActiveTypes)
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

        public IRepository GetSiteRepository(Type repositoryType)
        {
            if (repositoryType == null)
            {
                return null;
            }
            var modeltype = Lib.Reflection.TypeHelper.GetGenericType(repositoryType);
            return GetSiteRepository(repositoryType, modeltype);
        }

        public IRepository GetSiteRepository(Type repositoryType, Type siteModelType)
        {
            if (repositoryType == null || siteModelType == null)
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
                        if (!(Activator.CreateInstance(repositoryType) is IRepository instance))
                        {
                            return null;
                        }
                        else
                        {
                            if (instance is ISiteRepositoryBase sitebase)
                            {
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

        public IRepository GetSiteRepositoryByModelType(Type modelType)
        {
            var repotype = SiteRepositoryContainer.GetRepoTypeInfo(modelType);
            return GetSiteRepository(repotype, modelType);
        }

        public bool IsStoreExists(Type modelType)
        {
            return this.DatabaseDb.HasObjectStore(modelType.Name);
        }

        #endregion newSiteRepo

        private CmsFileRepository _files;

        public CmsFileRepository Files
        {
            get { return _files ?? (_files = GetSiteRepository<CmsFileRepository, CmsFile>()); }
        }

        private FolderRepository _folders;

        public FolderRepository Folders
        {
            get { return _folders ?? (_folders = GetSiteRepository<FolderRepository, Folder>()); }
        }

        private DomElementRepository _domelements;

        public DomElementRepository DomElements
        {
            get { return _domelements ?? (_domelements = GetSiteRepository<DomElementRepository, DomElement>()); }
        }

        private RouteRepository _routes;

        public RouteRepository Routes
        {
            get { return _routes ?? (_routes = GetSiteRepository<RouteRepository, Route>()); }
        }

        private FormRepository _forms;

        public FormRepository Forms
        {
            get { return _forms ?? (_forms = GetSiteRepository<FormRepository, Form>()); }
        }

        private FormSettingRepository _formsetting;

        public FormSettingRepository FormSetting
        {
            get { return _formsetting ?? (_formsetting = GetSiteRepository<FormSettingRepository, FormSetting>()); }
        }

        private FormValueRepository _formvalues;

        public FormValueRepository FormValues
        {
            get { return _formvalues ?? (_formvalues = GetSiteRepository<FormValueRepository, FormValue>()); }
        }

        private ImageRepository _images;

        public ImageRepository Images
        {
            get { return _images ?? (_images = GetSiteRepository<ImageRepository, Image>()); }
        }

        private PageRepository _pages;

        public PageRepository Pages
        {
            get { return _pages ?? (_pages = GetSiteRepository<PageRepository, Page>()); }
        }

        private ViewRepository _views;

        public ViewRepository Views
        {
            get { return _views ?? (_views = GetSiteRepository<ViewRepository, View>()); }
        }

        private ScriptRepository _scripts;

        public ScriptRepository Scripts
        {
            get { return _scripts ?? (_scripts = GetSiteRepository<ScriptRepository, Script>()); }
        }

        private CodeRepository _code;

        public CodeRepository Code
        {
            get { return _code ?? (_code = GetSiteRepository<CodeRepository, Code>()); }
        }

        private BusinessRuleRepository _rules;
        public BusinessRuleRepository Rules => GetSiteRepository<BusinessRuleRepository, BusinessRule>();

        private RelationRepository _relation;

        public RelationRepository Relations
        {
            get { return _relation ?? (_relation = GetSiteRepository<RelationRepository, ObjectRelation>()); }
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
                return _viewdatamethod ??
                       (_viewdatamethod = GetSiteRepository<ViewDataMethodRepository, ViewDataMethod>());
            }
        }

        public DownloadFailTrackRepository DownloadFailedLog => GetSiteRepository<DownloadFailTrackRepository, DownloadFailTrack>();

        private SiteUserRepository _siteuser;

        public SiteUserRepository SiteUser
        {
            get { return _siteuser ?? (_siteuser = GetSiteRepository<SiteUserRepository, SiteUser>()); }
        }

        public PathTree RouteTree(byte constType = 0)
        {
            return Cache.RouteTreeCache.RouteTree(this, constType);
        }

        #region "log"

        private string _logFolder;

        internal string LogFolder
        {
            get
            {
                if (_logFolder == null)
                {
                    var folder = AppSettings.GetOrganizationFolder(this.WebSite.OrganizationId);
                    folder = System.IO.Path.Combine(folder, this.WebSite.Name);
                    _logFolder = System.IO.Path.Combine(folder, "visitorlog");
                    Lib.Helper.IOHelper.EnsureDirectoryExists(_logFolder);
                }
                return _logFolder;
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
                    name = lastslash > -1 ? item.Substring(lastslash + 1) : item;

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
            string fullFileName = System.IO.Path.Combine(weekfolder, filename);
            return this.DatabaseDb.GetSequence<T>(fullFileName) as Sequence<T>;
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

        private Sequence<SiteErrorLog> _errorLog;
        private DateTime _errorlogtime;

        public Sequence<SiteErrorLog> ErrorLog
        {
            get
            {
                if (_errorLog == null)
                {
                    lock (_lock)
                    {
                        if (_errorLog == null)
                        {
                            _errorlogtime = DateTime.Now;
                            _errorLog = LogByWeek<SiteErrorLog>(_GetWeekName(_errorlogtime));
                        }
                    }
                }
                if (GetWeekOfYear(_errorlogtime) != GetWeekOfYear(DateTime.Now))
                {
                    _errorLog = null;
                    return ErrorLog;
                }
                return _errorLog;
            }
        }

        public EditLog Log
        {
            get
            {
                return this.DatabaseDb.Log;
            }
        }

        #endregion "log"

        private StyleRepository _styles;

        public StyleRepository Styles
        {
            get { return _styles ?? (_styles = GetSiteRepository<StyleRepository, Style>()); }
        }

        private ResourceGroupRepository _resourceGroups;

        public ResourceGroupRepository ResourceGroups
        {
            get
            {
                return _resourceGroups ??
                       (_resourceGroups = GetSiteRepository<ResourceGroupRepository, ResourceGroup>());
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

        private ExternalResourceRepository _externalResource;

        public ExternalResourceRepository ExternalResource
        {
            get
            {
                return _externalResource ??
                       (_externalResource = GetSiteRepository<ExternalResourceRepository, ExternalResource>());
            }
        }

        private ThumbnailRepository _thumbnails;

        public ThumbnailRepository Thumbnails
        {
            get { return _thumbnails ?? (_thumbnails = GetSiteRepository<ThumbnailRepository, Thumbnail>()); }
        }

        private LabelRepository _labels;

        public LabelRepository Labels
        {
            get { return _labels ?? (_labels = GetSiteRepository<LabelRepository, Label>()); }
        }

        private kConfigRepository _kconfig;

        public kConfigRepository KConfig
        {
            get { return _kconfig ?? (_kconfig = GetSiteRepository<kConfigRepository, KConfig>()); }
        }

        private HtmlBlockRepository _htmlblocks;

        public HtmlBlockRepository HtmlBlocks
        {
            get { return _htmlblocks ?? (_htmlblocks = GetSiteRepository<HtmlBlockRepository, HtmlBlock>()); }
        }

        private ContentFolderRepository _contentfolders;

        public ContentFolderRepository ContentFolders
        {
            get
            {
                return _contentfolders ??
                       (_contentfolders = GetSiteRepository<ContentFolderRepository, ContentFolder>());
            }
        }

        private ContentTypeRepository _contentTypes;

        public ContentTypeRepository ContentTypes
        {
            get { return _contentTypes ?? (_contentTypes = GetSiteRepository<ContentTypeRepository, ContentType>()); }
        }

        private ContentCategoryRepository _contentcategory;

        public ContentCategoryRepository ContentCategories
        {
            get
            {
                return _contentcategory ??
                       (_contentcategory = GetSiteRepository<ContentCategoryRepository, ContentCategory>());
            }
        }

        private TextContentRepository _textcontent;

        public TextContentRepository TextContent
        {
            get { return _textcontent ?? (_textcontent = GetSiteRepository<TextContentRepository, TextContent>()); }
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
                if (repo != null && repo is ISiteRepositoryBase siterepo)
                {
                    Monitor.Enter(_repolocker);

                    try
                    {
                        siterepo.Reuild();
                    }
                    catch (Exception)
                    {
                        // ignored
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