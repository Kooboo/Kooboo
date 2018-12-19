(function(KB) {
    KB.Route = {
        User: {
            LoginPage: GetRoute("Account/Login", true),
            LoginSuccess: GetRoute("Sites", true),
            ForgotPassword: GetRoute("Account/ForgotPassword", true),
            ResetPassword: GetRoute("Account/ResetPassword", true),
            RegisterPage: GetRoute("Account/Register", true),
            RegisterSuccess: GetRoute("Sites", true),
        },
        Site: {
            DetailPage: GetRoute("Site"),
            ListPage: GetRoute("Sites", true),
            ImportPage: GetRoute("Sites/Import", true),
            CreatePage: GetRoute("Sites/Create", true),
            TransferPage: GetRoute("Sites/Transfer", true),
            TemplatePage: GetRoute("Sites/Template", true),
            Transferring: GetRoute("Sites/Transferring", true),
            Share: GetRoute("Sites/Share", true)
        },
        Email: {
            Compose: GetRoute("Emails/Compose", true),
            InboxPage: GetRoute("Emails/Inbox", true),
            SentPage: GetRoute("Emails/Sent", true),
            DraftPage: GetRoute("Emails/Draft", true),
            TrashPage: GetRoute("Emails/Trash", true),
            SpamPage: GetRoute("Emails/Spam", true),
            AddressesPage: GetRoute("Emails/Addresses", true),
            PrintPage: GetRoute("Emails/Print", true)
        },
        Page: {
            ListPage: GetRoute("Pages"),
            Create: GetRoute("Page/Edit"),
            Settings: GetRoute("Page/Edit"),
            Design: GetRoute("Page/Design"),
            EditPage: GetRoute("Page/EditPage"),
            EditRichText: GetRoute("Page/EditRichText"),
            EditLayout: GetRoute("Page/EditLayout"),
            EditRedirector: GetRoute("Page/EditRedirector")
        },
        View: {
            Create: GetRoute("Development/View"),
            DetailPage: GetRoute("Development/View"),
            ListPage: GetRoute("Development/Views"),
            Versions: GetRoute("System/Versions")
        },
        Layout: {
            Create: GetRoute("Development/Layout"),
            DetailPage: GetRoute("Development/Layout"),
            ListPage: GetRoute("Development/Layouts"),
            Versions: GetRoute("System/Versions")
        },
        Form: {
            DetailPage: GetRoute("Development/Form"),
            ListPage: GetRoute("Development/Forms"),
            ValuePage: GetRoute("Development/Form/Values"),
            KooFormPage: GetRoute("Development/KooForm"),
            Redirector: GetRoute("Development/FormRedirector")
        },
        Label: {

        },
        Style: {
            Create: GetRoute("Development/Style"),
            DetailPage: GetRoute("Development/Style"),
            ListPage: GetRoute("Development/Styles")
        },
        Script: {
            Create: GetRoute("Development/Script"),
            DetailPage: GetRoute("Development/Script"),
            ListPage: GetRoute("Development/Scripts")
        },
        KScript: {
            Create: GetRoute("Development/KScript"),
            DetailPage: GetRoute("Development/KScript"),
            ListPage: GetRoute("Development/KScripts")
        },
        Code: {
            ListPage: GetRoute("Development/Code"),
            EditPage: GetRoute("Development/EditCode"),
            DebugPage: GetRoute("Development/Kscript/Debugger")
        },
        Menu: {
            DetailPage: GetRoute("Development/Menu"),
            ListPage: GetRoute("Development/Menus"),
            DialogPage: GetRoute("Development/MenuDialog")
        },
        HtmlBlock: {
            Create: GetRoute("Contents/HtmlBlock"),
            DetailPage: GetRoute("Contents/HtmlBlock"),
            ListPage: GetRoute("Contents/HtmlBlocks"),
            MultiLangListPage: GetRoute("Multilingual/HtmlBlocks"),
            MultiLangDetailPage: GetRoute("Multilingual/HtmlBlock"),
            DialogPage: GetRoute("Contents/HtmlBlockDialog")
        },
        Image: {
            ListPage: GetRoute("Contents/Images"),
            Edit: GetRoute("Contents/EditImage")
        },
        SiteLog: {
            ListPage: GetRoute("System/SiteLogs"),
            LogVersions: GetRoute("System/SiteLog/LogVersions"),
            DownloadPage: GetRoute("/_api/sitelog/downloadbatch"),
            VersionsCompare: GetRoute("System/SiteLog/VersionsCompare")
        },
        VisitorLog: {
            ListPage: GetRoute("System/VisitorLogs")
        },
        DataSource: {
            ListPage: GetRoute("Development/DataSources"),
            DataMethodSetting: GetRoute("Development/DataMethodSetting"),
            DataMethodSettingDialog: GetRoute("Development/DataMethodSettingDialog")
        },
        Database: {
            TablePage: GetRoute("Storage/Database"),
            DataPage: GetRoute("Storage/Database/Data"),
            EditDataPage: GetRoute("Storage/Database/EditData"),
            ColumnsPage: GetRoute("Storage/Database/Columns")
        },
        TextContent: {
            ListPage: GetRoute("Contents/TextContents"),
            DetailPage: GetRoute("Contents/Content"),
            DialogPage: GetRoute("Contents/ContentDialog"),
            ByFolder: GetRoute("Contents/TextContentsByFolder"),
            ByLangFolder: GetRoute("Multilingual/TextContentsByFolder")
        },
        ContentType: {
            ListPage: GetRoute("Contents/ContentTypes"),
            DetailPage: GetRoute("Contents/ContentType"),
            textContentEdit: GetRoute("Contents/TextContent"),
            multiTextContentEdit: GetRoute("Multilingual/TextContent"),
            TextContentDialog: GetRoute("Contents/TextContentDialog")
        },
        Job: {
            JobsPage: GetRoute("System/Jobs"),
            JobLogsPage: GetRoute("System/JobLogs")
        },
        Publish: {
            ListPage: GetRoute("Sync"),
            DetailList: GetRoute("Sync/List")
        },
        Domain: {
            IndexPage: GetRoute("Domains", true),
            Register: GetRoute("Domains/Register"),
            DomainBinding: GetRoute("Domains/DomainBinding", true),
            SiteBindingSettings: GetRoute("Domains/SiteBindingSettings", true)
        },
        Extension: {
            DataSourceMethodSettings: GetRoute("Extensions/DataSourceMethodSettings"),
            DataSource: GetRoute("Extensions/DataSource")
        },
        Event: {
            ListPage: GetRoute("Events"),
            DetailPage: GetRoute("Events/Event")
        },
        Product: {
            ListPage: GetRoute("ECommerce/Products"),
            DetailPage: GetRoute("ECommerce/Product"),
            Type: {
                ListPage: GetRoute("ECommerce/Product/Types"),
                DetailPage: GetRoute("ECommerce/Product/Type")
            },
            CategoriesPage: GetRoute("ECommerce/Product/Categories")
        },
        Market: {
            IndexPage: GetRoute("Market/Index", true)
        },
        Discussion: {
            ListPage: GetRoute("Market/Discussion/Index"),
            DetailPage: GetRoute("Market/Discussion/Detail"),
            MyPage: GetRoute("Market/Discussion/MyThreads")
        },
        Demand: {
            ListPage: GetRoute("Market/Demand/Index"),
            DetailPage: GetRoute("Market/Demand/Detail"),
            MyDemandPage: GetRoute("Market/Demand/MyDemands"),
            MyProposalPage: GetRoute("Market/Demand/Proposals")
        },
        Supplier: {
            ListPage: GetRoute("Market/Supplier/Index"),
            DetailPage: GetRoute("Market/Supplier/Detail"),
            ServicePage: GetRoute("Market/Supplier/Service"),
            MyOrdersPage: GetRoute("Market/Supplier/MyOrders"),
            MyOffersPage: GetRoute("Market/Supplier/MyOffers"),
            OrderPage: GetRoute("Market/Supplier/Order")
        },
        Template: {
            ListPage: GetRoute("Market/Template/Index")
        },
        Hardware: {
            ListPage: GetRoute("Market/Hardware/Index")
        },
        App: {
            ListPage: GetRoute("Market/App/Index")
        },
        Get: function(PageRoute, params) {

            var paramStr = "";

            if (params) {
                if (PageRoute.indexOf("?") > -1) {
                    paramStr += "&";
                } else {
                    paramStr += "?";
                }

                if (typeof params === "object") {
                    var keys = Object.keys(params);
                    for (var i = 0, len = keys.length; i < len; i++) {
                        paramStr += keys[i] + "=" + params[keys[i]];

                        if (i !== len - 1) {
                            paramStr += "&";
                        }
                    }
                }
                return PageRoute + paramStr;
            } else {
                return PageRoute;
            }
        }
    }

    function GetRoute(route, witoutSiteId) {
        var url = "/_Admin/" + route;

        if (!witoutSiteId) {
            var siteid = Kooboo.getQueryString("SiteId");
            if (siteid) {
                url += "?SiteId=" + siteid;
            }
        }
        return url;
    }

})(Kooboo);