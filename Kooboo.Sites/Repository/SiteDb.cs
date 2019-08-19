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
            var dbName = AppSettings.GetDbName(website.OrganizationId, website.Name);
            this.DatabaseDb = DB.GetDatabase(dbName);
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

        public LayoutRepository Layouts => GetSiteRepository<LayoutRepository, Layout>();

        public ContinueConvertRepository ContinueConverter => GetSiteRepository<ContinueConvertRepository, ContinueConverter>();

        public DataMethodSettingRepository DataMethodSettings => GetSiteRepository<DataMethodSettingRepository, DataMethodSetting>();

        public SyncSettingRepository SyncSettings => GetSiteRepository<SyncSettingRepository, SyncSetting>();

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

        public SynchronizationRepository Synchronization => GetSiteRepository<SynchronizationRepository, Synchronization>();

        public CoreSettingRepository CoreSetting => GetSiteRepository<CoreSettingRepository, CoreSetting>();


        public SiteClusterRepository SiteCluster => GetSiteRepository<SiteClusterRepository, SiteCluster>();


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


        public MenuRepository Menus => GetSiteRepository<MenuRepository, Menu>();

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


        public CmsFileRepository Files => GetSiteRepository<CmsFileRepository, CmsFile>();

        public FolderRepository Folders => GetSiteRepository<FolderRepository, Folder>();

        public DomElementRepository DomElements => GetSiteRepository<DomElementRepository, DomElement>();

        public RouteRepository Routes => GetSiteRepository<RouteRepository, Route>();

        public FormRepository Forms => GetSiteRepository<FormRepository, Form>();

        public FormSettingRepository FormSetting => GetSiteRepository<FormSettingRepository, FormSetting>();

        public FormValueRepository FormValues => GetSiteRepository<FormValueRepository, FormValue>();

        public ImageRepository Images => GetSiteRepository<ImageRepository, Image>();

        public PageRepository Pages => GetSiteRepository<PageRepository, Page>();

        public ViewRepository Views => GetSiteRepository<ViewRepository, View>();

        public ScriptRepository Scripts => GetSiteRepository<ScriptRepository, Script>();

        public CodeRepository Code => GetSiteRepository<CodeRepository, Code>();

        public BusinessRuleRepository Rules => GetSiteRepository<BusinessRuleRepository, BusinessRule>();

        public RelationRepository Relations => GetSiteRepository<RelationRepository, ObjectRelation>();

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

        public ViewDataMethodRepository ViewDataMethods => GetSiteRepository<ViewDataMethodRepository, ViewDataMethod>();

        public DownloadFailTrackRepository DownloadFailedLog => GetSiteRepository<DownloadFailTrackRepository, DownloadFailTrack>();


        public SiteUserRepository SiteUser => GetSiteRepository<SiteUserRepository, SiteUser>();

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

        public StyleRepository Styles => GetSiteRepository<StyleRepository, Style>();


        public ResourceGroupRepository ResourceGroups => GetSiteRepository<ResourceGroupRepository, ResourceGroup>();

        public CmsCssRuleRepository CssRules => GetSiteRepository<CmsCssRuleRepository, CmsCssRule>();


        public ExternalResourceRepository ExternalResource => GetSiteRepository<ExternalResourceRepository, ExternalResource>();

        public ThumbnailRepository Thumbnails => GetSiteRepository<ThumbnailRepository, Thumbnail>();

        public LabelRepository Labels => GetSiteRepository<LabelRepository, Label>();

        public kConfigRepository KConfig => GetSiteRepository<kConfigRepository, KConfig>();

        public HtmlBlockRepository HtmlBlocks => GetSiteRepository<HtmlBlockRepository, HtmlBlock>();

        public ContentFolderRepository ContentFolders => GetSiteRepository<ContentFolderRepository, ContentFolder>();


        public ContentTypeRepository ContentTypes => GetSiteRepository<ContentTypeRepository, ContentType>();

        public ContentCategoryRepository ContentCategories => GetSiteRepository<ContentCategoryRepository, ContentCategory>();

        public TextContentRepository TextContent => GetSiteRepository<TextContentRepository, TextContent>();

        #region Ecommerce

        //private CategoryRepository _category;

        //public CategoryRepository Category
        //{
        //    get
        //    {
        //        return EnsureRepository<CategoryRepository, Category>(ref _category);
        //    }
        //}

        //private ProductTypeRepository _productType;

        //public ProductTypeRepository ProductType
        //{
        //    get
        //    {
        //        return EnsureRepository<ProductTypeRepository, ProductType>(ref _productType);
        //    }
        //}

        //public ProductCategoryRepository _productcategory; 
        //public ProductCategoryRepository ProductCategory
        //{
        //    get
        //    {
        //        return EnsureRepository<ProductCategoryRepository, ProductCategory>(ref _productcategory); 
        //    }
        //}

        //public ProductRepository _product;
        //public ProductRepository Product
        //{
        //    get
        //    {
        //        return EnsureRepository<ProductRepository, Product>(ref _product);
        //    }
        //}

        //public ProductVariantsRepository _productvariants; 

        //public ProductVariantsRepository ProductVariants
        //{
        //    get
        //    {
        //        return EnsureRepository<ProductVariantsRepository, ProductVariants>(ref _productvariants);
        //    }
        //}



        #endregion


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
