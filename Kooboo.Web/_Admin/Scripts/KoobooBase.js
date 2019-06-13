(function(wind) {
  function handleRequestError(quest) {
    window.info.fail(
      Kooboo.text.info.networkError + ", " + Kooboo.text.info.checkServer
    );
  }

  function extend(Child, Parent) {
    Child.prototype = inherit(Parent.prototype);
    Child.prototype.constructor = Child;
  }

  function inherit(proto) {
    function F() {}
    F.prototype = proto;
    return new F();
  }

  var loading = {
    requestCount: 0,
    start: function() {
      this.requestCount++;
      $(".page-loading").show();
      // $("body").addClass("modal-open");
    },
    stop: function() {
      this.requestCount--;
      if (this.requestCount === 0) {
        $(".page-loading").hide();
      }
      // $("body").removeClass("modal-open");
    },
    initial: function() {
      if (this.requestCount === 0) {
        $(".page-loading").hide();
      }
    }
  };

  function BaseModel(typeName) {
    this.name = typeName;
  }

  BaseModel.prototype = {
    executeGet: function(method, data, hideLoading, useSync) {
      var self = this;
      var hideLoading = !!hideLoading && true;
      hideLoading && $(".page-loading").hide();
      !hideLoading && loading.start();

      return DataCache.getData(this.name, method, data, useSync)
        .fail(function(fail) {
          handleRequestError(fail);
        })
        .always(function() {
          !hideLoading && loading.stop();
        })
        .done(function(res) {
          if (!res.success) {
            Kooboo.handleFailMessages(res.messages);
          }
        });
    },
    executePost: function(method, data, hideLoading, extendParams, useSync) {
      var self = this;
      var hideLoading = !!hideLoading && true;
      !hideLoading && loading.start();
      hideLoading && $(".page-loading").hide();

      if (data instanceof Object) {
        data = JSON.stringify(data);
      }
      data = encodeURIComponent(data);

      return DataCache.postData(this.name, method, data, extendParams, useSync)
        .fail(function(fail) {
          handleRequestError(fail);
        })
        .always(function() {
          !hideLoading && loading.stop();
        })
        .done(function(res) {
          // clean cache...
          if (!res.success) {
            Kooboo.handleFailMessages(res.messages);
          } else {
            // data cached
          }
        });
    },
    executeUpload: function(method, data, progressor) {
      var self = this;

      !progressor && loading.start();

      return DataCache.uploadData(this.name, method, data, progressor)
        .fail(function(fail) {
          handleRequestError(fail);
        })
        .always(function() {
          !progressor && loading.stop();
        })
        .done(function(res) {
          if (!res.success) {
            Kooboo.handleFailMessages(res.messages);
          }
        });
    },

    syncAjax: function(route, data) {
      var self = this;

      loading.start();
      return $.ajax({
        url: this._getUrl(route),
        type: "GET",
        data: data,
        async: false
      })
        .fail(function(fail) {
          handleRequestError(fail);
        })
        .always(function() {
          loading.stop();
        })
        .done(function(res) {
          if (!res.success) {
            Kooboo.handleFailMessages(res.message);
          }
        });
    },

    get: function(paras) {
      return this.executeGet("get", paras);
    },
    Get: function(paras) {
      return this.executeGet("Get", paras);
    },
    getEdit: function(para) {
      return this.executeGet("GetEdit", para);
    },
    getList: function(paras) {
      return this.executeGet("list", paras);
    },
    post: function(paras) {
      return this.executePost("post", paras);
    },
    put: function(paras) {
      return this.executePost("put", paras);
    },
    Delete: function(paras) {
      return this.executePost("Delete", paras);
    },
    Deletes: function(paras) {
      return this.executePost("Deletes", paras);
    },
    isUniqueName: function(paras) {
      return this._getUrl("isUniqueName", paras);
    },

    _getUrl: function(route) {
      var url = "/_api/" + this.name + "/" + route;
      if (Kooboo.getQueryString("SiteId")) {
        url += "?SiteId=" + Kooboo.getQueryString("SiteId");
      }
      return url;
    },

    _getUrl: function(method) {
      var url = "/_api/" + this.name + "/" + method;
      if (Kooboo.getQueryString("SiteId")) {
        url += "?SiteId=" + Kooboo.getQueryString("SiteId");
      }
      return url;
    }
  };

  //view
  function View() {
    this.name = "View";

    this.CompareType = function(para) {
      return this.executeGet("CompareType", para);
    };

    this.dataMethod = function(para) {
      return this.executeGet("DataMethod", para);
    };

    this.ViewMethods = function(para) {
      return this.executeGet("ViewMethods", para);
    };

    this.Copy = function(para) {
      return this.executePost("Copy", para);
    };
  }
  extend(View, BaseModel);

  //page
  function Page() {
    this.name = "Page";

    this.getAll = function(para) {
      return this.executeGet("all", para);
    };

    this.getAccessToken = function(para) {
      return this.executeGet("GetAccessToken", para);
    };

    this.getDefaultRoute = function(para) {
      return this.executeGet("DefaultRoute", para);
    };

    this.defaultRouteUpdate = function(para) {
      return this.executePost("DefaultRouteUpdate", para);
    };

    this.ConvertFile = function(para) {
      return this.executeUpload("ConvertFile", para);
    };

    this.Copy = function(para) {
      return this.executePost("Copy", para);
    };

    this.PostRichText = function(para) {
      return this.executePost("PostRichText", para);
    };
  }
  extend(Page, BaseModel);

  //user
  function User() {
    this.name = "User";

    this.login = function(para) {
      return this.executePost("login", para);
    };

    this.logout = function(para) {
      return this.executePost("logout", para);
    };

    this.register = function(para) {
      return this.executePost("register", para);
    };

    this.get = function(para) {
      return this.executeGet("getUser", para);
    };

    this.getCulture = function(para) {
      return this.executeGet("Culture", para);
    };

    this.getUser = function(para) {
      return this.executeGet("getUser", para);
    };

    this.changePassword = function(para) {
      return this.executePost("changePassword", para);
    };

    this.updateProfile = function(para) {
      return this.executePost("updateProfile", para);
    };

    this.checkUser = function(para) {
      return this.executeGet("checkUser", para);
    };

    this.ForgotPassword = function(para) {
      return this.executePost("ForgotPassword", para);
    };

    this.ResetPassword = function(para) {
      return this.executePost("ResetPassword", para);
    };
    this.getLanguage = function() {
      return this.executePost("getLanguage", {});
    };

    this.isUniqueEmail = function(paras) {
      return this._getUrl("isUniqueEmail", paras);
    };

    this.verifyEmail = function(para) {
      return this.executePost("VerifyEmail", para);
    };
  }
  extend(User, BaseModel);

  function Organization() {
    (this.name = "Organization"),
      (this.getOrganizations = function(para) {
        return this.executeGet("getOrganizations", para);
      });

    this.changeUserOrg = function(para) {
      return this.executePost("changeUserOrg", para);
    };

    this.useCoupon = function(para) {
      return this.executePost("useCoupon", para);
    };

    this.payRecharge = function(para) {
      return this.executePost("payRecharge", para);
    };

    this.getOrg = function(para) {
      return this.executeGet("getOrg", para);
    };

    this.getUsers = function(para) {
      return this.executeGet("getUsers", para);
    };

    this.addUser = function(para) {
      return this.executePost("addUser", para);
    };

    this.deleteUser = function(para) {
      return this.executePost("deleteUser", para);
    };

    this.getDataCenter = function(para) {
      return this.executeGet("GetDataCenter", para);
    };

    this.updateDataCenter = function(para) {
      return this.executePost("UpdateDataCenter", para);
    };
  }
  extend(Organization, BaseModel);

  //DataSource
  function DataSource() {
    this.name = "DataMethodSetting";

    this.update = function(para) {
      return this.executePost("Update", para);
    };
  }
  extend(DataSource, BaseModel);

  DataSource.prototype.getData = function(para) {
    return this.executeGet("centerList", para);
  };

  DataSource.prototype.getPublicData = function(para) {
    return this.executeGet("public", para);
  };

  DataSource.prototype.getPrivateData = function(para) {
    return this.executeGet("private", para);
  };

  // DataMethodSetting
  function DataMethodSetting() {
    this.name = "DataMethodSetting";

    this.byView = function(para) {
      return this.executeGet("byView", para);
    };
  }
  extend(DataMethodSetting, BaseModel);

  function Database() {
    this.name = "Database";

    this.getTables = function(para) {
      return this.executeGet("Tables", para);
    };

    this.getData = function(para) {
      return this.executeGet("Data", para);
    };

    this.updateData = function(para) {
      return this.executePost("UpdateData", para);
    };

    this.deleteData = function(para) {
      return this.executePost("DeleteData", para);
    };

    this.getColumns = function(para) {
      return this.executeGet("Columns", para);
    };

    this.updateColumn = function(para) {
      return this.executePost("UpdateColumn", para);
    };

    this.createTable = function(para) {
      return this.executePost("CreateTable", para);
    };

    this.isUniqueTableName = function(para) {
      return this._getUrl("IsUniqueTableName", para);
    };

    this.deleteTables = function(para) {
      return this.executePost("DeleteTables", para);
    };

    this.getAvailableControlTypes = function(para) {
      return this.executeGet("AvailableControlTypes", para);
    };
  }
  extend(Database, BaseModel);

  //Site
  function Site() {
    this.name = "Site";

    this.clusterList = function(para) {
      return this.executeGet("ClusterList", para);
    };

    this.export = function(para) {
      return this.executePost("export", para);
    };

    this.ExportStoreUrl = function(para) {
      return this._getUrl("ExportStore", para);
    };

    this.ExportUrl = function(para) {
      return this._getUrl("export", para);
    };

    this.getDiskSync = function(para) {
      return this.executeGet("DiskSyncGet", para);
    };

    this.updateDiskSync = function(para) {
      return this.executePost("DiskSyncUpdate", para);
    };

    this.Create = function(para) {
      return this.executePost("Create", para);
    };

    this.Import = function(para, progressor) {
      return this.executeUpload("ImportSite", para, progressor);
    };

    this.Langs = function(para) {
      return this.executeGet("Langs", para);
    };

    this.SwitchStatus = function(para) {
      return this.executePost("SwitchStatus", para);
    };

    this.getCultures = function(para) {
      return this.executeGet("Cultures", para);
    };

    this.CheckDomainBindingAvailable = function(para) {
      return this._getUrl("CheckDomainBindingAvailable", para);
    };

    this.getName = function(para) {
      return this.executeGet("GetName", para);
    };

    this.getExportStoreNames = function() {
      return this.executeGet("ExportStoreNames");
    };

    this.getTypes = function() {
      return this.executeGet("Types");
    };
  }
  extend(Site, BaseModel);

  //Domain
  function Domain() {
    this.name = "Domain";
    this.getAvailable = function(para) {
      return this.executeGet("Available", para);
    };
    this.creatDomain = function(para) {
      return this.executePost("Create", para);
    };
    this.searchDomain = function(para) {
      return this.executeGet("Search", para);
    };
    this.getPaymentStatus = function(para) {
      return this.executeGet("PaymentStatus", para, true);
    };
    this.payDomain = function(para) {
      return this.executePost("payDomain", para);
    };
    this.serverInfo = function(para) {
      return this.executeGet("ServerInfo", para);
    };
  }
  extend(Domain, BaseModel);

  //Component - Header
  function Bar() {
    this.name = "Bar";
    this.getHeader = function(para) {
      return this.executeGet("header", para);
    };
  }
  extend(Bar, BaseModel);

  //Component - Sidebar
  function Sidebar() {
    var self = this;
    this.name = "Bar";
    this.getSidebar = function(para) {
      return self.executeGet("sitemenu", para);
    };
    this.getDomainSidebar = function(para) {
      return self.executeGet("domainMenu", para);
    };
    this.getExtensionSidebar = function(para) {
      return self.executeGet("extensionMenu", para);
    };
    this.getMarketSidebar = function(para) {
      return self.executeGet("MarketSideBar", para);
    };
  }
  extend(Sidebar, BaseModel);

  // Style
  function Style() {
    this.name = "Style";

    this.Get = function(para) {
      return this.executeGet("Get", para);
    };

    this.GetEdit = function(para) {
      return this.executeGet("GetEdit", para);
    };

    this.Update = function(para) {
      return this.executePost("Update", para);
    };

    this.GetRules = function(para) {
      return this.executeGet("GetRules", para);
    };

    this.UpdateRules = function(para) {
      return this.executePost("UpdateRules", para);
    };

    this.getExternalList = function(para) {
      return this.executeGet("External", para);
    };

    this.getEmbeddedList = function(para) {
      return this.executeGet("Embedded", para);
    };

    this.getInlineList = function(para) {
      return this.executeGet("Inline", para);
    };

    this.getRuleRelation = function(para) {
      return this.executeGet("RuleRelation", para);
    };

    this.getExtensions = function(para) {
      return this.executeGet("getExtensions", para);
    };
  }
  extend(Style, BaseModel);

  // CSSRule
  function CSSRule() {
    this.name = "CSSRule";

    this.getInlineList = function(para) {
      return this.executeGet("InlineList", para);
    };

    this.getInline = function(para) {
      return this.executeGet("getInline", para);
    };

    this.getRelation = function(para) {
      return this.executeGet("Relation", para);
    };

    this.updateInline = function(para) {
      return this.executePost("UpdateInline", para);
    };
  }
  extend(CSSRule, Style);

  // Transfer
  function Transfer() {
    this.name = "Transfer";

    this.singlePage = function(para) {
      return this.executePost("Single", para);
    };

    this.getSubUrl = function(para) {
      return this.executeGet("GetSubUrl", para);
    };

    this.byLevel = function(para) {
      return this.executeGet("ByLevel", para);
    };

    this.byPage = function(para) {
      return this.executePost("ByPage", para);
    };

    this.getStatus = function(para) {
      return this.executeGet("GetStatus", para, true);
    };

    this.getTaskStatus = function(para) {
      return this.executeGet("GetTaskStatus", para, true);
    };
  }
  extend(Transfer, BaseModel);

  function ContentFolder() {
    this.name = "ContentFolder";
    this.getColumnsById = function(para) {
      return this.executeGet("Columns", para);
    };

    this.getFolderInfoById = function(para) {
      return this.executeGet("Get", para);
    };
  }
  extend(ContentFolder, BaseModel);

  // Menu
  function Menu() {
    this.name = "Menu";

    this.Create = function(para) {
      return this.executePost("Create", para);
    };

    this.Update = function(para) {
      return this.executePost("Update", para);
    };

    this.UpdateTemplate = function(para) {
      return this.executePost("UpdateTemplate", para);
    };

    this.UpdateUrl = function(para) {
      return this.executePost("UpdateUrl", para);
    };

    this.getFlat = function(para) {
      return this.executeGet("getFlat", para, true);
    };

    this.CreateSub = function(para) {
      return this.executePost("CreateSub", para);
    };

    this.getLang = function(para) {
      return this.executeGet("getLang", para);
    };

    this.updateLang = function(para) {
      return this.executePost("UpdateLang", para);
    };

    this.Swap = function(para) {
      return this.executePost("Swap", para);
    };
  }
  extend(Menu, BaseModel);

  // ResourceGroup
  function ResourceGroup() {
    this.name = "ResourceGroup";

    this.Style = function(para) {
      return this.executeGet("Style", para);
    };

    this.Script = function(para) {
      return this.executeGet("Script", para);
    };

    this.Get = function(para) {
      return this.executeGet("Get", para);
    };

    this.Create = function(para) {
      return this.executePost("Update", para);
    };

    this.Update = function(para) {
      return this.executePost("Update", para);
    };
  }
  extend(ResourceGroup, BaseModel);

  // Script
  function Script() {
    this.name = "Script";

    this.getExternalList = function(para) {
      return this.executeGet("External", para);
    };

    this.getEmbeddedList = function(para) {
      return this.executeGet("Embedded", para);
    };

    this.Get = function(para) {
      return this.executeGet("Get", para);
    };

    this.Update = function(para) {
      return this.executePost("Update", para);
    };

    this.getExtensions = function(para) {
      return this.executeGet("getExtensions", para);
    };
  }
  extend(Script, BaseModel);

  // KScript
  function KScript() {
    this.name = "KScript";

    this.getExternalList = function(para) {
      return this.executeGet("External", para);
    };

    this.getEmbeddedList = function(para) {
      return this.executeGet("Embedded", para);
    };

    this.Get = function(para) {
      return this.executeGet("Get", para);
    };

    this.Update = function(para) {
      return this.executePost("Update", para);
    };
  }
  extend(KScript, BaseModel);

  // Media
  function Media() {
    this.name = "Media";

    this.createFolder = function(para) {
      return this.executePost("CreateFolder", para);
    };

    this.getListBy = function(para) {
      return this.executeGet("ListBy", para);
    };

    this.deleteFolders = function(para) {
      return this.executePost("DeleteFolders", para);
    };

    this.deleteImages = function(para) {
      return this.executePost("DeleteImages", para);
    };

    this.imageUpdate = function(para) {
      return this.executePost("ImageUpdate", para);
    };

    this.ContentImage = function(para) {
      return this.executeUpload("ContentImage", para);
    };

    this.getDialogList = function(para) {
      return this.executeGet("List", para, true);
    };

    this.getPagedList = function(para) {
      return this.executeGet("PagedList", para);
    };

    this.getPagedListBy = function(para) {
      return this.executeGet("PagedListBy", para);
    };
  }
  extend(Media, BaseModel);

  function ContentType() {
    this.name = "ContentType";

    this.save = function(para) {
      return this.executePost("Post", para);
    };
  }
  extend(ContentType, BaseModel);

  function Component() {
    this.name = "Component";

    this.TagObjects = function(para) {
      return this.executeGet("TagObjects", para);
    };

    this.getPreviewHtml = function(para) {
      return this.executeGet("PreviewHtml", para);
    };

    this.getSource = function(para) {
      return this.executeGet("GetSource", para, false, true);
    };
  }
  extend(Component, BaseModel);

  function TextContent() {
    this.name = "textContent";

    this.getFields = function(para) {
      return this.executeGet("GetFields", para);
    };

    this.langupdate = function(para) {
      return this.executePost("langupdate", para, false, null, true);
    };

    this.update = function(para) {
      // 上面那个因为在使用的时候需要同步验证，但是这个用法是错误的。修改完后把上面的删了
      return this.executePost("langupdate", para);
    };

    this.getByFolder = function(para) {
      if (!para || (para && !para.folderId)) {
        var folderId = Kooboo.getQueryString("folder");
        para = para || {};
        para.folderId = folderId;
      }
      return this.executeGet("ByFolder", para);
    };

    this.getFolderId = function(para) {
      var id = "";
      this.syncAjax("GetFolderId", para).done(function(res) {
        id = res.model;
      });
      return id;
    };

    this.search = function(para) {
      return this.executeGet("Search", para);
    };
  }
  extend(TextContent, BaseModel);

  function SiteLog() {
    this.name = "SiteLog";

    this.Blame = function(para) {
      return this.executePost("Blame", para);
    };

    this.Restore = function(para) {
      return this.executePost("Restore", para);
    };

    this.CheckOut = function(para) {
      return this.executePost("CheckOut", para);
    };

    this.Versions = function(para) {
      return this.executeGet("Versions", para);
    };

    this.Revert = function(para) {
      return this.executePost("Revert", para);
    };

    this.Compare = function(para) {
      return this.executeGet("Compare", para);
    };

    this.ExportBatch = function(para) {
      return this.executeGet("ExportBatch", para);
    };

    this.ExportItem = function(para) {
      return this.executePost("ExportItem", para);
    };
    this.ExportItems = function(para) {
      return this.executePost("ExportItems", para);
    };

    this.DownloadPageUrl = function(para) {
      return this._getUrl("DownloadBatch", para);
    };
  }
  extend(SiteLog, BaseModel);

  function Label() {
    this.name = "Label";

    this.UpdateUrl = function(para) {
      return this._getUrl("Update", para);
    };

    this.Update = function(para) {
      return this.executePost("Update", para);
    };

    this.getKeys = function(para) {
      return this.executeGet("Keys", para);
    };
  }
  extend(Label, BaseModel);

  function Link() {
    this.name = "Link";

    this.All = function(para) {
      return this.executeGet("all", para);
    };
    this.SyncAll = function(para) {
      return this.executeGet("all", para, true, true);
    };
  }
  extend(Link, BaseModel);

  function Editor() {
    this.name = "InlineEditor";
    this.Update = function(para) {
      return this.executePost("Update", para);
    };
  }
  extend(Editor, BaseModel);

  //Binding
  function Binding() {
    this.name = "Binding";
    this.listBySite = function(para) {
      return this.executeGet("listbysite", para);
    };
    this.ListByDomain = function(para) {
      return this.executeGet("ListByDomain", para);
    };
    this.SiteBinding = function(para) {
      return this.executeGet("SiteBinding", para);
    };
  }
  extend(Binding, BaseModel);

  //Visitor logs
  function VisitorLog() {
    this.name = "VisitorLog";

    this.All = function(para) {
      return this.executeGet("list", para);
    };

    this.getWeekNames = function(para) {
      return this.executeGet("WeekNames", para);
    };

    this.TopPages = function(para) {
      return this.executeGet("TopPages", para);
    };

    this.TopReferer = function(para) {
      return this.executeGet("TopReferer", para);
    };

    this.TopImages = function(para) {
      return this.executeGet("TopImages", para);
    };

    this.ErrorList = function(para) {
      return this.executeGet("ErrorList", para);
    };

    this.ErrorDetail = function(para) {
      return this.executeGet("ErrorDetail", para);
    };

    this.GetRegionCount = function(para) {
      return this.executeGet("GetRegionCount", para);
    };

    this.Monthly = function(para) {
      return this.executeGet("Monthly", para);
    };
  }
  extend(VisitorLog, BaseModel);

  function Job() {
    this.name = "Job";

    this.getLogs = function(para) {
      return this.executeGet("Logs", para);
    };

    this.Run = function(para) {
      return this.executeGet("Run", para);
    };
  }
  extend(Job, BaseModel);

  function Upload() {
    this.name = "Upload";

    this.Style = function(para, progressor) {
      return this.executeUpload("Style", para, progressor);
    };

    this.Script = function(para, progressor) {
      return this.executeUpload("Script", para, progressor);
    };

    this.Images = function(para) {
      return this.executeUpload("Image", para);
    };

    this.File = function(para) {
      return this.executeUpload("File", para);
    };

    this.Package = function(para) {
      return this.executeUpload("Package", para);
    };
  }
  extend(Upload, BaseModel);

  function Template() {
    this.name = "Template";

    this.Share = function(para) {
      return this.executeUpload("Share", para);
    };

    this.batchShare = function(para) {
      return this.executeUpload("ShareBatch", para);
    };

    this.list = function(para) {
      return this.executeGet("List", para);
    };

    this.private = function(para) {
      return this.executeGet("Private", para);
    };

    this.personal = function(para) {
      return this.executeGet("Personal", para);
    };

    this.Use = function(para) {
      return this.executePost("Use", para);
    };

    this.Search = function(para) {
      return this.executeGet("Search", para);
    };

    this.Update = function(para) {
      return this.executeUpload("Update", para);
    };
  }
  extend(Template, BaseModel);

  function Diagnosis() {
    this.name = "Diagnosis";

    this.getItems = function(para) {
      return this.executeGet("items", para);
    };
    this.startScacn = function(para) {
      return this.executePost("scan", para);
    };

    this.getProgress = function(para) {
      return this.executePost("progress", para, true);
    };

    this.stopScan = function(para) {
      return this.executePost("cancel", para);
    };

    this.startSession = function(para) {
      return this.executeGet("StartSession", para);
    };

    this.getStatus = function(para) {
      return this.executeGet("Status", para, true);
    };

    this.cancel = function(para) {
      return this.executeGet("cancel", para, true);
    };
  }
  extend(Diagnosis, BaseModel);

  function Extensions() {
    this.name = "extension";
    this.typetree = function() {
      return this.executeGet("typetree");
    };
    this.post = function(para) {
      return this.executeUpload("post", para);
    };
    this.dataSource = function() {
      return this.executePost("datasource");
    };
    this.thirdparty = function() {
      return this.executePost("thirdparty");
    };
    this.thirdPartyUpdate = function(para) {
      return this.executePost("thirdPartyUpdate", para);
    };
    this.getSetting = function(para) {
      return this.executeGet("getSetting", para);
    };
    this.UpdateSetting = function(methodId, para) {
      return this.executePost("UpdateSetting?id=" + methodId, para);
    };
  }
  extend(Extensions, BaseModel);

  function EmailAddress() {
    this.name = "EmailAddress";

    this.Address = function(para) {
      return this.executeGet("Address", para);
    };

    this.Domains = function(para) {
      return this.executeGet("Domains", para);
    };

    this.getMemberList = function(para) {
      return this.executeGet("MemberList", para);
    };

    this.saveMember = function(para) {
      return this.executePost("MemberPost", para);
    };

    this.removeMember = function(para) {
      return this.executePost("MemberDelete", para);
    };

    this.updateForward = function(para) {
      return this.executePost("UpdateForward", para);
    };
  }
  extend(EmailAddress, BaseModel);

  function EmailDraft() {
    this.name = "EmailDraft";

    this.Compose = function(para) {
      return this.executeGet("Compose", para);
    };

    this.targetAddresses = function(para) {
      return this.executeGet("TargetAddresses", para);
    };

    this.Save = function(para) {
      return this.executePost("Post", para);
    };
  }
  extend(EmailDraft, BaseModel);

  function EmailAttachment() {
    this.name = "EmailAttachment";

    this.ImagePost = function(para) {
      return this.executeUpload("ImagePost", para);
    };

    this.AttachmentPost = function(para) {
      return this.executeUpload("AttachmentPost", para);
    };

    this.DeleteAttachment = function(para) {
      return this.executePost("DeleteAttachment", para);
    };

    this.downloadAttachment = function(para) {
      return this._getUrl("msgfile", para);
    };
  }
  extend(EmailAttachment, BaseModel);

  function EmailMessage() {
    this.name = "EmailMessage";

    this.Delete = function(para) {
      return this.executePost("Deletes", para);
    };

    this.Send = function(para) {
      return this.executeUpload("Send", para);
    };

    this.More = function(para) {
      return this.executeGet("More", para);
    };

    this.Moves = function(para) {
      return this.executePost("Moves", para);
    };

    this.getContent = function(para) {
      return this.executeGet("Content", para);
    };

    this.getForwardContent = function(para) {
      return this.executeGet("Forward", para);
    };

    this.getReplyContent = function(para) {
      return this.executeGet("Reply", para);
    };

    this.MarkReads = function(para) {
      return this.executePost("MarkReads", para);
    };
  }
  extend(EmailMessage, BaseModel);

  function Publish() {
    this.name = "publish";

    this.getRemoteSiteList = function(para) {
      return this.executePost("RemoteSiteList", para);
    };

    this.pushItems = function(para) {
      return this.executePost("pushItems", para);
    };

    this.push = function(para) {
      return this.executePost("push", para);
    };

    this.pull = function(para) {
      return this.executePost("pull", para, true);
    };

    this.pushLog = function(para) {
      return this.executeGet("OutItem", para);
    };

    this.pullLog = function(para) {
      return this.executeGet("InItem", para);
    };
  }
  extend(Publish, BaseModel);

  var UserPublish = function() {
    this.name = "UserPublish";

    this.getRemoteDomains = function(para) {
      return this.executeGet("RemoteDomains", para);
    };

    this.deleteServer = function(para) {
      return this.executePost("DeleteServer", para);
    };

    this.updateServer = function(para) {
      return this.executePost("UpdateServer", para);
    };

    this.getRemoteDomains = function(para) {
      return this.executeGet("RemoteDomains", para);
    };
  };

  extend(UserPublish, BaseModel);

  var Dashboard = function() {
    this.name = "Dashboard";

    this.All = function(para) {
      return this.executeGet("All", para);
    };

    this.getItems = function(para) {
      return this.executeGet("Items", para);
    };
  };
  extend(Dashboard, BaseModel);

  var Form = function() {
    this.name = "Form";

    this.GetEdit = function(para) {
      return this.executeGet("GetEdit", para);
    };

    this.updateBody = function(para) {
      return this.executePost("UpdateBody", para);
    };

    this.getExternalList = function(para) {
      return this.executeGet("External", para);
    };

    this.getEmbeddedList = function(para) {
      return this.executeGet("Embedded", para);
    };

    this.getSetting = function(para) {
      return this.executeGet("GetSetting", para);
    };

    this.updateSetting = function(para) {
      return this.executePost("UpdateSetting", para);
    };

    this.getSource = function(para) {
      return this.executeGet("FormSource", para);
    };

    this.UpdateForm = function(para) {
      return this.executePost("UpdateForm", para);
    };

    this.Values = function(para) {
      return this.executeGet("FormValues", para);
    };

    this.DeleteValues = function(para) {
      return this.executePost("DeleteFormValues", para);
    };

    this.getKoobooForm = function(para) {
      return this.executeGet("GetKoobooForm", para);
    };

    this.updateKoobooForm = function(para) {
      return this.executePost("UpdateKoobooForm", para);
    };
  };
  extend(Form, BaseModel);

  function File() {
    this.name = "File";

    this.createFolder = function(para) {
      return this.executePost("CreateFolder", para);
    };

    this.deleteFolders = function(para) {
      return this.executePost("DeleteFolders", para);
    };

    this.deleteFiles = function(para) {
      return this.executePost("DeleteFiles", para);
    };
  }
  extend(File, BaseModel);

  function Profile() {
    this.name = "Profile";

    this.getAccount = function(para) {
      return this.executeGet("Account", para);
    };
  }
  extend(Profile, BaseModel);

  function Disk() {
    this.name = "Disk";

    this.CleanRepository = function(para) {
      return this.executePost("CleanRepository", para);
    };

    this.CleanLog = function(para) {
      return this.executePost("CleanLog", para);
    };

    this.getSize = function(para) {
      return this.executeGet("GetSize", para);
    };
  }
  extend(Disk, BaseModel);

  function Relation() {
    this.name = "Relation";

    this.showBy = function(para) {
      return this.executeGet("ShowBy", para, true);
    };
  }
  extend(Relation, BaseModel);

  function Url() {
    this.name = "Url";

    this.getInternalList = function(para) {
      return this.executeGet("Internal", para);
    };

    this.getExternalList = function(para) {
      return this.executeGet("External", para);
    };

    this.updateUrl = function(para) {
      return this._getUrl("UpdateUrl", para);
    };
  }
  extend(Url, BaseModel);

  function Layout() {
    this.name = "Layout";

    this.Copy = function(para) {
      return this.executePost("Copy", para);
    };
  }
  extend(Layout, BaseModel);

  function System() {
    this.name = "System";

    this.Version = function(para) {
      return this.executeGet("Version", para, true);
    };

    this.loadFile = function(para) {
      var files;
      $.ajax({
        url: this._getUrl("loadFile"),
        type: "post",
        dataType: "json",
        data: JSON.stringify(para),
        async: false,
        success: function(res) {
          files = res.success ? res.model : {};
        }
      });
      return files;
    };
  }
  extend(System, BaseModel);

  function Search() {
    this.name = "Search";

    this.Enable = function(para) {
      return this.executePost("Enable", para);
    };

    this.Disable = function(para) {
      return this.executePost("Disable", para);
    };

    this.Rebuild = function(para) {
      return this.executePost("Rebuild", para);
    };

    this.Clean = function(para) {
      return this.executePost("Clean", para);
    };

    this.getIndexStat = function(para) {
      return this.executeGet("IndexStat", para);
    };

    this.getLastest = function(para) {
      return this.executeGet("Lastest", para);
    };

    this.SearchStat = function(para) {
      return this.executeGet("SearchStat", para);
    };

    this.getWeekNames = function(para) {
      return this.executeGet("WeekNames", para);
    };
  }

  extend(Search, BaseModel);

  function HtmlBlock() {
    this.name = "HtmlBlock";
    this.syncPost = function(para) {
      return this.executePost("Post", para, true, {}, true);
    };
  }
  extend(HtmlBlock, BaseModel);

  function Cluster() {
    this.name = "Cluster";

    this.get = function(para) {
      return this.executeGet("Get", para);
    };

    this.isValidateCustomServer = function(para) {
      return this.executeGet("ValidateCustomServer", para);
    };
  }
  extend(Cluster, BaseModel);

  function KeyValue() {
    this.name = "KeyValue";

    this.Update = function(para) {
      return this.executePost("Update", para);
    };
  }
  extend(KeyValue, BaseModel);

  function Code() {
    this.name = "Code";

    this.getCodeType = function(para) {
      return this.executeGet("CodeType", para);
    };

    this.getListByType = function(para) {
      return this.executeGet("ListByType", para);
    };
  }
  extend(Code, BaseModel);

  function BusinessRule() {
    this.name = "BusinessRule";

    this.getAvailableCodes = function(para) {
      return this.executeGet("GetAvailableCodes", para);
    };

    this.getListByEvent = function(para) {
      return this.executeGet("ListByEvent", para);
    };

    this.getConditionOption = function(para) {
      return this.executeGet("ConditionOption", para);
    };

    this.getSetting = function(para) {
      return this.executeGet("GetSetting", para);
    };

    this.getTemp = function(para) {
      return this.executeGet("Temp", para);
    };

    this.deleteRule = function(para) {
      return this.executePost("DeleteRule", para);
    };
  }
  extend(BusinessRule, BaseModel);

  function Debugger() {
    this.name = "Debugger";

    this.startSession = function(para) {
      return this.executeGet("StartSession", para);
    };

    this.stopSession = function(para) {
      return this.executeGet("StopSession", para, true, true);
    };

    this.setBreakpoints = function(para) {
      return this.executePost("SetBreakPoints", para, true);
    };

    this.step = function(para) {
      return this.executeGet("Step", para, true);
    };

    this.getInfo = function(para) {
      return this.executeGet("GetInfo", para, true);
    };

    this.execute = function(para) {
      return this.executeGet("Execute", para, true);
    };
  }
  extend(Debugger, BaseModel);

  function SiteUser() {
    this.name = "SiteUser";

    this.getCurrentUsers = function(para) {
      return this.executeGet("CurrentUsers", para);
    };

    this.getAvailableUsers = function(para) {
      return this.executeGet("AvailableUsers", para);
    };
    this.getRoles = function(para) {
      return this.executeGet("Roles", para);
    };

    this.addUser = function(para) {
      return this.executePost("AddUser", para);
    };
  }
  extend(SiteUser, BaseModel);

  function Certificate() {
    this.name = "Certificate";
  }
  extend(Certificate, BaseModel);

  function Commerce() {
    this.name = "Commerce";

    this.enableSites = function(para) {
      return this.executePost("EnableSites", para);
    };

    this.disableSites = function(para) {
      return this.executePost("DisableSites", para);
    };
  }
  extend(Commerce, BaseModel);

  function Product() {
    this.name = "Product";

    this.getEdit = function(para) {
      return this.executeGet("GetEdit", para);
    };

    this.getList = function(para) {
      return this.executeGet("ProductList", para);
    };
  }
  extend(Product, BaseModel);

  function ProductType() {
    this.name = "ProductType";
    this.getColumnsById = function(para) {
      return this.executeGet("Columns", para);
    };
  }
  extend(ProductType, BaseModel);

  function ProductCategory() {
    this.name = "Category";

    this.checkProuctCount = function(para) {
      return this.executeGet("CheckProuctCount", para);
    };
  }
  extend(ProductCategory, BaseModel);

  function KConfig() {
    this.name = "KConfig";

    this.update = function(para) {
      return this.executePost("Update", para);
    };
  }
  extend(KConfig, BaseModel);

  function APIGeneration() {
    this.name = "APIGeneration";

    this.getTypes = function(para) {
      return this.executeGet("Types", para);
    };

    this.getObjects = function(para) {
      return this.executeGet("Objects", para);
    };

    this.getActions = function(para) {
      return this.executeGet("Actions", para);
    };

    this.Generate = function(para) {
      return this.executePost("Generate", para);
    };
  }
  extend(APIGeneration, BaseModel);

  function Currency() {
    this.name = "Currency";

    this.change = function(para) {
      return this.executePost("Change", para);
    };
  }
  extend(Currency, BaseModel);

  function CoreSetting() {
    this.name = "CoreSetting";
    this.update = function(para) {
      return this.executePost("update", para);
    };
  }
  extend(CoreSetting, BaseModel);

  function TableRelation() {
    this.name = "TableRelation";
    this.getTablesAndFields = function(para) {
      return this.executeGet("getTablesAndFields", para);
    };
    this.getRelationTypes = function(para) {
      return this.executeGet("getRelationTypes", para);
    };
    this.addRelation = function(para) {
      return this.executePost("AddRelation", para);
    };
  }
  extend(TableRelation, BaseModel);

  function TransferTask() {
    this.name = "TransferTask";
  }
  extend(TransferTask, BaseModel);

  function Role() {
    this.name = "Role";
  }
  extend(Role, BaseModel);

  wind.Kooboo = {
    APIGeneration: new APIGeneration(),
    Bar: new Bar(),
    BaseModel: new BaseModel(),
    Binding: new Binding(),
    BusinessRule: new BusinessRule(),
    Certificate: new Certificate(),
    Cluster: new Cluster(),
    Code: new Code(),
    ContentFolder: new ContentFolder(),
    ContentType: new ContentType(),
    Commerce: new Commerce(),
    Component: new Component(),
    CoreSetting: new CoreSetting(),
    CSSRule: new CSSRule(),
    Currency: new Currency(),
    Database: new Database(),
    DataSource: new DataSource(),
    DataMethodSetting: new DataMethodSetting(),
    Dashboard: new Dashboard(),
    Debugger: new Debugger(),
    Diagnosis: new Diagnosis(),
    Disk: new Disk(),
    Domain: new Domain(),
    EmailAddress: new EmailAddress(),
    EmailDraft: new EmailDraft(),
    EmailAttachment: new EmailAttachment(),
    EmailMessage: new EmailMessage(),
    Extensions: new Extensions(),
    File: new File(),
    Form: new Form(),
    ResourceGroup: new ResourceGroup(),
    HtmlBlock: new HtmlBlock(),
    Editor: new Editor(),
    Job: new Job(),
    KConfig: new KConfig(),
    KScript: new KScript(),
    Label: new Label(),
    Layout: new Layout(),
    Link: new Link(),
    Media: new Media(),
    Menu: new Menu(),
    Organization: new Organization(),
    Relation: new Relation(),
    Page: new Page(),
    ProductType: new ProductType(),
    Product: new Product(),
    ProductCategory: new ProductCategory(),
    Profile: new Profile(),
    Publish: new Publish(),
    Role: new Role(),
    Search: new Search(),
    Script: new Script(),
    Site: new Site(),
    Sidebar: new Sidebar(),
    SiteLog: new SiteLog(),
    KeyValue: new KeyValue(),
    SiteUser: new SiteUser(),
    Style: new Style(),
    System: new System(),
    TableRelation: new TableRelation(),
    Template: new Template(),
    Transfer: new Transfer(),
    TransferTask: new TransferTask(),
    VisitorLog: new VisitorLog(),
    Upload: new Upload(),
    User: new User(),
    UserPublish: new UserPublish(),
    Url: new Url(),
    View: new View()
  };
  wind.Kooboo.TextContent = new TextContent();

  Kooboo.getQueryString = function(name, source) {
    if (!name) {
      return null;
    }
    source = source || window.location.search.substr(1);
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = source.match(reg);
    if (r != null) {
      return r[2];
    }
    return null;
  };
  Kooboo.getURLParams = function(search) {
    var params = {};

    if (search.indexOf("?") > -1) {
      search = search.split("?")[1];
    }

    var temp = search.split("&");
    temp.forEach(function(param) {
      var match = param.match(/(\w*)+=(\S*)/);
      match && (params[match[1].toLowerCase()] = match[2]);
    });

    return params;
  };
  Kooboo.isSameURLParams = function(p1, p2) {
    var key1 = Object.keys(p1),
      key2 = Object.keys(p2),
      same = true;

    if (key1.length !== key2.length) {
      same = false;
    } else {
      for (var i = 0; i < key1.length; i++) {
        if (key2.indexOf(key1[i]) == -1) {
          same = false;
        } else {
          if (p1[key1[i]] !== p2[key2[i]]) {
            same = false;
          }
        }
      }
    }

    return same;
  };
  Kooboo.objToArr = function(obj, keyName, valueName) {
    var arr = [];

    if (obj) {
      Object.keys(obj).forEach(function(key) {
        var _temp = {};
        _temp[keyName || "key"] = key;
        _temp[valueName || "value"] = obj[key];
        arr.push(_temp);
      });
    }

    return arr;
  };
  Kooboo.arrToObj = function(arr, keyName, valueName) {
    var obj = {};
    if (arr && arr.length) {
      for (var i = 0, len = arr.length; i < len; i++) {
        obj[arr[i][keyName || "key"]] = arr[i][valueName || "value"];
      }
    }

    return obj;
  };

  Kooboo.bytesToSize = function(filesize) {
    var gigabytes = 1024 * 1024 * 1024;
    var returnValue = filesize / gigabytes;
    if (returnValue > 1) {
      return Math.floor(returnValue * 100) / 100 + " GB";
    }
    var megabyte = 1024 * 1024;
    returnValue = filesize / megabyte;
    if (returnValue > 1) {
      return Math.floor(returnValue * 100) / 100 + " MB";
    }
    var kilobyte = 1024;
    returnValue = filesize / kilobyte;
    return Math.floor(returnValue * 100) / 100 + " KB";
  };

  Kooboo.getLabelClass = function(type) {
    var _class = "";
    switch (type.toLowerCase()) {
      case "htmlblock":
        _class = "label-primary";
        break;
      case "layout":
        _class = "blue";
        break;
      case "menu":
        _class = "label-info";
        break;
      case "page":
        _class = "label-success";
        break;
      case "script":
        _class = "green";
        break;
      case "style":
        _class = "orange";
        break;
      case "view":
        _class = "label-warning";
        break;
      case "image":
        _class = "yellow";
        break;
      case "textcontent":
        _class = "dark";
        break;
      case "cssDeclaration":
        _class = "red";
        break;
      case "file":
        _class = "purple";
        break;
      default:
        _class = "label-danger";
        break;
    }
    return _class;
  };

  Kooboo.getLabelColor = function(type) {
    var _color = "";
    switch (type.toLowerCase()) {
      case "htmlblock":
        _color = "#5ba52e";
        break;
      case "layout":
        _color = "#55a439";
        break;
      case "menu":
        _color = "#4da348";
        break;
      case "view":
        _color = "#229e97";
        break;
      case "page":
        _color = "#44a25a";
        break;
      case "script":
        _color = "#39a16e";
        break;
      case "style":
        _color = "#2e9f83";
        break;
      case "image":
        _color = "#179dab";
        break;
      case "textcontent":
        _color = "#0e9cbd";
        break;
      case "cssDeclaration":
        _color = "#069bcc";
        break;
      case "file":
        _color = "#009ad6";
        break;
      default:
        _color = "#005683";
        break;
    }
    return _color;
  };

  Kooboo.trim = function(str) {
    var whitespace =
      " \n\r\t\f\x0b\xa0\u2000\u2001\u2002\u2003\u2004\u2005\u2006\u2007\u2008\u2009\u200a\u200b\u2028\u2029\u3000";
    for (var i = 0; i < str.length; i++) {
      if (whitespace.indexOf(str.charAt(i)) === -1) {
        str = str.substring(i);
        break;
      }
    }
    for (i = str.length - 1; i >= 0; i--) {
      if (whitespace.indexOf(str.charAt(i)) === -1) {
        str = str.substring(0, i + 1);
        break;
      }
    }
    return whitespace.indexOf(str.charAt(0)) === -1 ? str : "";
  };

  Kooboo.handleFailMessages = function(messages) {
    if (messages) {
      for (var i = 0, len = messages.length; i < len; i++) {
        window.info.fail(messages[i]);
      }
    }
  };

  Kooboo.getRandomId = function() {
    return "kb_id_" + +new Date() + Math.ceil(Math.random() * Math.pow(2, 20));
  };

  Kooboo.getResourceTagId = function(type) {
    return (
      type +
      "-" +
      Math.round(Math.random() * Math.pow(2, 16)) +
      "-" +
      +new Date()
    );
  };

  Kooboo.event = {
    input: {
      positiveIntegerOnly: function(m, e) {
        if (e.keyCode >= 48 && e.keyCode <= 57 /*number*/) {
          return true;
        } else if (e.keyCode >= 96 && e.keyCode <= 105 /*number*/) {
          return true;
        } else if (e.keyCode == 8 /*BACKSPACE*/) {
          return true;
        } else if (e.keyCode == 9 /* Tab */) {
          return true;
        } else {
          return false;
        }
      },
      positiveNumberOnly: function(m, e) {
        if (e.keyCode >= 48 && e.keyCode <= 57 /*number*/) {
          return true;
        } else if (e.keyCode >= 96 && e.keyCode <= 105 /*number*/) {
          return true;
        } else if (
          e.keyCode == 8 /*BACKSPACE*/ ||
          e.keyCode == 190 /*.*/ ||
          e.keyCode == 110 /* . */
        ) {
          return true;
        } else if (e.keyCode == 9 /* Tab */) {
          return true;
        } else {
          return false;
        }
      },
      inputNumberOnly: function(m, e) {
        if (e.keyCode >= 48 && e.keyCode <= 57 /*number*/) {
          return true;
        } else if (e.keyCode >= 96 && e.keyCode <= 105 /*number*/) {
          return true;
        } else if (
          (e.keyCode == 189 /* - */ && e.keyCode == 190) /*.*/ ||
          e.keyCode == 69 /*e*/ ||
          e.keyCode == 8 /*BACKSPACE*/
        ) {
          return true;
        } else if (e.keyCode == 9 /* Tab */) {
          return true;
        } else {
          return false;
        }
      }
    }
  };

  Kooboo.isFileNameExist = function(uploadFiles, localFiles) {
    var exist = false;

    for (var i = 0, len = uploadFiles.length; i < len; i++) {
      for (var j = 0, _len = localFiles.length; j < _len; j++) {
        var localName =
          typeof localFiles[j].name == "function"
            ? localFiles[j].name()
            : localFiles[j].name;
        if (uploadFiles[i].name.toLowerCase() == localName.toLowerCase()) {
          exist = true;
          break;
        }
      }
    }

    return exist;
  };
  Kooboo.getTemplate = function(url) {
    if (localStorage.getItem(url)) {
      return localStorage.getItem(url);
    } else {
      var text = "";
      $.ajax({
        url: url,
        type: "get",
        cache: true,
        async: false,
        success: function(res) {
          text = res;
          localStorage.setItem(url, res);
        }
      });
      return text;
    }
  };
  Kooboo.loadJS = function(paths, fromLayout) {
    $("script[spa-script]", $("head")).each(function(idx, script) {
      $(script).remove();
    });

    var unCachedScripts = [];
    paths.forEach(function(path) {
      var hasCached = localStorage.getItem(path);
      if (hasCached) {
        loadJS(path, hasCached, fromLayout);
      } else {
        unCachedScripts.push(path);
      }
    });

    if (unCachedScripts.length) {
      var scripts = Kooboo.System.loadFile(unCachedScripts);
      var _paths = Object.keys(scripts);
      _paths.forEach(function(path) {
        if (scripts[path]) {
          loadJS(path, scripts[path], fromLayout);
          localStorage.setItem(path, scripts[path]);
        } else {
          console.error(
            "Load " +
              path +
              " failed. Please ensure your script path is correct."
          );
        }
      });
    }

    function loadJS(path, text, fromLayout) {
      var script = $(
        '<script data-src="' +
          path +
          '"' +
          (fromLayout ? "layout-script" : "spa-script") +
          ">" +
          text +
          "</script>"
      );
      $("head").append(script);
    }
  };

  Kooboo.loadCSS = function(paths) {
    $("style[data-href]", $("head")).each(function(idx, style) {
      $(style).remove();
    });

    var unCachedStyles = [];
    paths.forEach(function(path) {
      var hasCached = localStorage.getItem(path);
      hasCached ? loadCSS(path, hasCached) : unCachedStyles.push(path);
    });

    if (unCachedStyles.length) {
      var styles = Kooboo.System.loadFile(unCachedStyles);
      var _paths = Object.keys(styles);
      _paths.forEach(function(path) {
        localStorage.setItem(path, styles[path]);
        loadCSS(path, styles[path]);
      });
    }

    function loadCSS(path, text) {
      var style = '<style data-href="' + path + '">' + text + "</style>";
      $("head").append(style);
    }
  };
})(window);
