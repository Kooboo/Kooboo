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
using Kooboo.Lib.Reflection;
using Kooboo.Sites.ViewModel;
using Kooboo.Lib.Helper;
using Kooboo.Sites.SiteTransfer.Model;
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
        }

        public DiskSize GetSize()
        {
            DiskSize disksize = new DiskSize();

            long totallength = 0;

            foreach (var item in this.AllRepositories)
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
            if (ModelType == null)
            {
                return null;
            }
            var repo = AllRepositories.Find(o => o.ModelType == ModelType);
            if (repo != null)
            {
                return repo;
            }
            else
            {
                var all = AllRepositories;
                foreach (var item in all)
                {
                    if (item.ModelType == ModelType)
                    {
                        return item;
                    }
                }
                return this.GetRepository(ModelType.Name);
            }

        }

        public IRepository GetRepository(byte ConstType)
        {
            var modeltype = Service.ConstTypeService.GetModelType(ConstType);
            return GetRepository(modeltype);
        }

        public IRepository GetRepository(string StoreName)
        {
            if (string.IsNullOrEmpty(StoreName))
            {
                return null;
            }
            lock (_lock)
            {
                var find = this.AllRepositories.Find(o => o.StoreName == StoreName);
                if (find != null)
                {
                    return find;
                }

                find = this.AllRepositories.Find(o => o.StoreName.ToLower() == StoreName.ToLower());
                if (find != null)
                {
                    return find;
                }
            }
            return null;
        }

        private List<IRepository> _AllRepositories;

        /// <summary>
        /// All repositories excl text content repositories... 
        /// </summary>
        /// <returns></returns>
        internal List<IRepository> AllRepositories
        {
            get
            {
                if (_AllRepositories == null)
                {
                    lock (_lock)
                    {
                        if (_AllRepositories == null)
                        {
                            _AllRepositories = new List<IRepository>();

                            var allproperties = this.GetType().GetProperties();
                            foreach (var item in allproperties)
                            {
                                if (TypeHelper.HasInterface(item.GetMethod.ReturnType, typeof(IRepository)))
                                {
                                    var generittype = TypeHelper.GetGenericType(item.GetMethod.ReturnType);

                                    if (generittype != null)
                                    {
                                        var repo = item.GetValue(this);
                                        if (repo != null && repo is IRepository)
                                        {
                                            _AllRepositories.Add(repo as IRepository);
                                        }
                                    }
                                }
                            }

                        }
                    }

                }

                return _AllRepositories;
            }
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
                return EnsureRepository<LayoutRepository, Layout>(ref _layouts);
            }
        }

        private ContinueConvertRepository _continueConverter;

        public ContinueConvertRepository ContinueConverter
        {
            get
            {
                return EnsureRepository<ContinueConvertRepository, ContinueConverter>(ref _continueConverter);
            }
        }

        private DataMethodSettingRepository _datamethodsettings;
        public DataMethodSettingRepository DataMethodSettings
        {
            get
            {
                return EnsureRepository<DataMethodSettingRepository, DataMethodSetting>(ref _datamethodsettings);
            }
        }

        private SiteRepositoryBase<SyncSetting> _SyncSetting;

        public SiteRepositoryBase<SyncSetting> SyncSettings
        {
            get
            {
                return EnsureRepository<SiteRepositoryBase<SyncSetting>, SyncSetting>(ref _SyncSetting);
            }
        }



        private SiteRepositoryBase<QueueObject> _taskqueue;

        public SiteRepositoryBase<QueueObject> TaskQueue
        {
            get
            {
                return EnsureRepository<SiteRepositoryBase<QueueObject>, QueueObject>(ref _taskqueue);
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

        private SynchronizationRepository _Synchronization;

        public SynchronizationRepository Synchronization
        {
            get
            {
                return EnsureRepository<SynchronizationRepository, Synchronization>(ref _Synchronization);
            }
        }

        private SiteClusterRepository _Cluster;

        public SiteClusterRepository SiteCluster
        {
            get
            {
                return EnsureRepository<SiteClusterRepository, SiteCluster>(ref _Cluster);
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
                return EnsureRepository<MenuRepository, Menu>(ref _menus);
            }
        }

        private TransferTaskRepository _TransferTask;

        public TransferTaskRepository TransferTasks
        {
            get
            {
                return EnsureRepository<TransferTaskRepository, TransferTask>(ref _TransferTask);
            }
        }

        private TransferPageRepository _transferpages;
        public TransferPageRepository TransferPages
        {
            get
            {
                return EnsureRepository<TransferPageRepository, TransferPage>(ref _transferpages);
            }
        }

        private TRepository EnsureRepository<TRepository, TSiteObject>(ref TRepository repository)
            where TRepository : SiteRepositoryBase<TSiteObject>
            where TSiteObject : class, ISiteObject
        {
            if (repository == null)
            {
                lock (_lock)
                {
                    if (repository == null)
                    {
                        repository = Activator.CreateInstance<TRepository>();
                        repository.SiteDb = this;
                        repository.init();
                    }
                }
            }
            return repository;
        }

        private CmsFileRepository _files;
        public CmsFileRepository Files
        {
            get
            {
                return EnsureRepository<CmsFileRepository, CmsFile>(ref _files);
            }
        }

        private FolderRepository _folders;

        public FolderRepository Folders
        {
            get
            {
                return EnsureRepository<FolderRepository, Folder>(ref _folders);
            }
        }
          

        private DomElementRepository _DomElements;

        public DomElementRepository DomElements
        {
            get
            {
                return EnsureRepository<DomElementRepository, DomElement>(ref _DomElements);
            }
        }

        private RouteRepository _routes;
        public RouteRepository Routes
        {
            get
            {
                return EnsureRepository<RouteRepository, Route>(ref _routes);
            }
        }

        private FormRepository _forms; 
        public FormRepository Forms
        {
            get
            {
                return EnsureRepository<FormRepository, Form>(ref _forms);
            } 
        }

        private FormSettingRepository _formsetting; 

        public FormSettingRepository FormSetting 
        {
            get
            {
                return EnsureRepository<FormSettingRepository, FormSetting>(ref _formsetting); 
            }
        }

        private FormValueRepository _formvalues;

        public FormValueRepository FormValues
        {
            get
            {
                return EnsureRepository<FormValueRepository, FormValue>(ref _formvalues);
            }
        }

        private ImageRepository _images;

        public ImageRepository Images
        {
            get
            {
                return EnsureRepository<ImageRepository, Image>(ref _images);
            }
        }

        private PageRepository _pages;

        public PageRepository Pages
        {
            get
            {
                return EnsureRepository<PageRepository, Page>(ref _pages);
            }
        }

        private ViewRepository _views;

        public ViewRepository Views
        {
            get
            {
                return EnsureRepository<ViewRepository, View>(ref _views);
            }
        }

        private ScriptRepository _scripts;

        public ScriptRepository Scripts
        {
            get
            {
                return EnsureRepository<ScriptRepository, Script>(ref _scripts);
            }
        }


        private CodeRepository _code;

        public CodeRepository Code
        {
            get
            {
                return EnsureRepository<CodeRepository, Code>(ref _code);
            }
        }

        private BusinessRuleRepository _rules; 

        public BusinessRuleRepository Rules
        {
            get
            {
                return EnsureRepository<BusinessRuleRepository, BusinessRule>(ref _rules);
            }
        }
 

        private RelationRepository _relations;
        public RelationRepository Relations
        {
            get
            {
                return EnsureRepository<RelationRepository, ObjectRelation>(ref _relations);
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

        private ViewDataMethodRepository _ViewDataMethod;

        public ViewDataMethodRepository ViewDataMethods
        {
            get
            {
                return EnsureRepository<ViewDataMethodRepository, ViewDataMethod>(ref _ViewDataMethod);
            }
        }

        private SiteRepositoryBase<DownloadFailTrack> _downloadfailelog;

        public SiteRepositoryBase<DownloadFailTrack> DownloadFailedLog
        {
            get
            {
                return EnsureRepository<SiteRepositoryBase<DownloadFailTrack>, DownloadFailTrack>(ref _downloadfailelog);
            }
        }


        private SiteRepositoryBase<SiteUser> _siteuser;

        public SiteRepositoryBase<SiteUser> SiteUser
        {
            get
            {
                return EnsureRepository<SiteRepositoryBase<SiteUser>, SiteUser>(ref _siteuser);
            }
        }


        /// <summary>
        /// The cache route tree for route searching.
        /// </summary>
        /// <param name="ConstType"></param>
        /// <returns></returns>
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

                    int lastslash =Kooboo.Lib.Helper.PathHelper.GetLastSlash(item);
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

        #endregion

        private StyleRepository _styles;

        public StyleRepository Styles
        {
            get
            {
                return EnsureRepository<StyleRepository, Style>(ref _styles);
            }
        }

        private ResourceGroupRepository _resourceGroup;
        public ResourceGroupRepository ResourceGroups
        {
            get
            {
                return EnsureRepository<ResourceGroupRepository, ResourceGroup>(ref _resourceGroup);
            }
        }

        private CmsCssRuleRepository _CssRules;

        public CmsCssRuleRepository CssRules
        {
            get
            {
                return EnsureRepository<CmsCssRuleRepository, CmsCssRule>(ref _CssRules);
            }
        }


        //private CssDeclarationRepository _CssDeclarations;

        //public CssDeclarationRepository CssDeclarations
        //{
        //    get
        //    {
        //        return EnsureRepository<CssDeclarationRepository, CmsCssDeclaration>(ref _CssDeclarations);
        //    }
        //}

        private ExternalResourceRepository _ExternalResources;

        public ExternalResourceRepository ExternalResource
        {
            get
            {
                return EnsureRepository<ExternalResourceRepository, ExternalResource>(ref _ExternalResources);
            }
        }

        private ThumbnailRepository _thumbnails;
        public ThumbnailRepository Thumbnails
        {
            get
            {
                return EnsureRepository<ThumbnailRepository, Thumbnail>(ref _thumbnails);
            }
        }

        private LabelRepository _label;

        public LabelRepository Labels
        {
            get
            {
                return EnsureRepository<LabelRepository, Label>(ref _label);
            }
        }

        private HtmlBlockRepository _htmlblocks;
        public HtmlBlockRepository HtmlBlocks
        {
            get
            {
                return EnsureRepository<HtmlBlockRepository, HtmlBlock>(ref _htmlblocks);
            }
        }

        private ContentFolderRepository _contentfolder;

        public ContentFolderRepository ContentFolders
        {
            get
            {
                return EnsureRepository<ContentFolderRepository, ContentFolder>(ref _contentfolder);
            }
        }

        private ContentTypeRepository _contenttypes;

        public ContentTypeRepository ContentTypes
        {
            get
            {
                return EnsureRepository<ContentTypeRepository, ContentType>(ref _contenttypes);
            }

        }

        private ContentCategoryRepository _contentCategory;

        public ContentCategoryRepository ContentCategories
        {
            get
            {
                return EnsureRepository<ContentCategoryRepository, ContentCategory>(ref _contentCategory);
            }
        }

        private TextContentRepository _textcontentreponew;

        public TextContentRepository TextContent
        {
            get
            {
                return EnsureRepository<TextContentRepository, TextContent>(ref _textcontentreponew);
            }
        }


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



        public EditLog Log
        {
            get
            {
                return this.DatabaseDb.Log;
            }
        }
    }

    public enum LogType
    {
        Visitor = 0,
        Image = 1,
        SiteError = 2
    }
}
