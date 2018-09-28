$(function() {

    var doubleSuffix = [".co.uk", ".org.uk", ".ltd.uk", ".plc.uk", ".me.uk", ".br.com", ".cn.com", ".eu.com", ".hu.com", ".no.com", ".qc.com", ".sa.com", ".se.com", ".se.net", ".us.com", ".uy.com", ".co.ac", ".gv.ac", ".or.ac", ".ac.ac", ".ac.at", ".co.at", ".gv.at", ".or.at", ".asn.au", ".com.au", ".edu.au", ".org.au", ".net.au", ".id.au", ".ac.be", ".adm.br", ".adv.br", ".am.br", ".arq.br", ".art.br", ".bio.br", ".cng.br", ".cnt.br", ".com.br", ".ecn.br", ".eng.br", ".esp.br", ".etc.br", ".eti.br", ".fm.br", ".fot.br", ".fst.br", ".g12.br", ".gov.br", ".ind.br", ".inf.br", ".jor.br", ".lel.br", ".med.br", ".mil.br", ".net.br", ".nom.br", ".ntr.br", ".odo.br", ".org.br", ".ppg.br", ".pro.br", ".psc.br", ".psi.br", ".rec.br", ".slg.br", ".tmp.br", ".tur.br", ".tv.br", ".vet.br", ".zlg.br", ".ab.ca", ".bc.ca", ".mb.ca", ".nb.ca", ".nf.ca", ".ns.ca", ".nt.ca", ".on.ca", ".pe.ca", ".qc.ca", ".sk.ca", ".yk.ca", ".ac.cn", ".com.cn", ".edu.cn", ".gov.cn", ".org.cn", ".bj.cn", ".sh.cn", ".tj.cn", ".cq.cn", ".he.cn", ".nm.cn", ".ln.cn", ".jl.cn", ".hl.cn", ".js.cn", ".zj.cn", ".ah.cn", ".gd.cn", ".gx.cn", ".hi.cn", ".sc.cn", ".gz.cn", ".yn.cn", ".xz.cn", ".sn.cn", ".gs.cn", ".qh.cn", ".nx.cn", ".xj.cn", ".tw.cn", ".hk.cn", ".mo.cn", ".com.ec", ".tm.fr", ".com.fr", ".asso.fr", ".presse.fr", ".co.il", ".net.il", ".ac.il", ".k12.il", ".gov.il", ".muni.il", ".ac.in", ".co.in", ".org.in", ".ernet.in", ".gov.in", ".net.in", ".res.in", ".ac.jp", ".co.jp", ".go.jp", ".or.jp", ".ne.jp", ".ac.kr", ".co.kr", ".go.kr", ".ne.kr", ".nm.kr", ".or.kr", ".asso.mc", ".tm.mc", ".com.mm", ".org.mm", ".net.mm", ".edu.mm", ".gov.mm", ".org.ro", ".store.ro", ".tm.ro", ".firm.ro", ".www.ro", ".arts.ro", ".rec.ro", ".info.ro", ".nom.ro", ".nt.ro", ".com.sg", ".org.sg", ".net.sg", ".gov.sg", ".ac.th", ".co.th", ".go.th", ".mi.th", ".net.th", ".or.th", ".com.tr", ".edu.tr", ".gov.tr", ".k12.tr", ".net.tr", ".org.tr", ".com.tw", ".org.tw", ".net.tw", ".ac.uk", ".uk.com", ".uk.net", ".gb.com", ".gb.net", ".com.hk", ".org.hk", ".net.hk", ".edu.hk", ".eu.lv", ".co.nz", ".org.nz", ".net.nz", ".maori.nz", ".iwi.nz", ".com.pt", ".edu.pt", ".com.ve", ".net.ve", ".org.ve", ".web.ve", ".info.ve", ".co.ve", ".net.ru", ".org.ru", ".com.hr", ".net.cn", ".com.ag", ".net.ag", ".org.ag", ".com.bz", ".net.bz", ".com.co", ".net.co", ".nom.co", ".com.es", ".nom.es", ".org.es", ".firm.in", ".gen.in", ".ind.in", ".com.mx", ".idv.tw"],
        CONVERTYPE = {
            AUTO: "0",
            SEMI_AUTO: "1",
            MANUAL: "2",
            ONLINE: "3"
        };

    var transferViewModel = function(data) {

        var self = this;

        this.showError = ko.observable(false);

        this.url = ko.validateField({
            required: Kooboo.text.validation.required,
            regex: {
                pattern: /[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)/,
                message: Kooboo.text.validation.urlInvalid
            }
        });

        this.urlFocused = ko.observable(true);

        this.url.subscribe(function(val) {

            if (val) {
                var tempName = "",
                    isDouble = false;
                try {
                    var url = new URL(val);
                    tempName = url.host;
                } catch (ex) {
                    if (Kooboo.BrowserInfo.getBrowser() == "ie") {
                        var parser = document.createElement("a");
                        parser.href = val;
                        tempName = parser.hostname;
                    }
                } finally {
                    doubleSuffix.forEach(function(suffix) {
                        if (tempName.indexOf(suffix, tempName.length - suffix.length) !== -1) {
                            isDouble = true;
                            tempName = tempName.substr(0, tempName.lastIndexOf(suffix));
                        }
                    })

                    var pureNameArray = tempName.split("."),
                        pureName = pureNameArray[pureNameArray.length - 2 + (isDouble ? 1 : 0)];

                    self.siteName(pureName);
                }
            }
        });

        this.siteName = ko.validateField({
            required: Kooboo.text.validation.required,
            remote: {
                url: Kooboo.Site.isUniqueName(),
                message: Kooboo.text.validation.taken,
                type: "get",
                data: {
                    name: function() {
                        return self.siteName()
                    }
                }
            },
            regex: {
                pattern: /^[A-Za-z][\w\-]*$/,
                message: Kooboo.text.validation.siteNameInvalid
            }
        });
        this.siteName.subscribe(function(val) {
            self.subDomain(val);
        });

        this.rootDomain = ko.observable();

        this.subDomain = ko.validateField({
            required: Kooboo.text.validation.required,
            stringlength: {
                min: 1,
                max: 63,
                message: Kooboo.text.validation.minLength + 1 + ", " + Kooboo.text.validation.maxLength + 63
            },
            remote: {
                url: Kooboo.Site.CheckDomainBindingAvailable(),
                message: Kooboo.text.validation.taken,
                type: "get",
                data: {
                    SubDomain: function() {
                        return self.subDomain && self.subDomain();
                    },
                    RootDomain: function() {
                        return self.rootDomain && self.rootDomain();
                    }
                }
            },
            regex: {
                pattern: /^[A-Za-z][\w\-]*$/,
                message: Kooboo.text.validation.siteNameInvalid
            }
        });

        this.domains = ko.observableArray(data);

        this.autoConvert = ko.observable(true);

        this.convertType = ko.observable($('input:radio').length == 1 ? CONVERTYPE.ONLINE : CONVERTYPE.AUTO);
        this.convertType.subscribe(function(type) {

            if (type !== CONVERTYPE.MANUAL) {
                self.scanDone(false);
            }

            self.autoConvert(type === CONVERTYPE.AUTO);
            self.enableScan(type === CONVERTYPE.MANUAL);
            self.startDisabled(type === CONVERTYPE.MANUAL);
        });

        this.totalPages = ko.observable(20);

        this.enableScan = ko.observable(false);

        this.depth = ko.observable(2);

        this.startDisabled = ko.observable();

        this.scanStart = function() {
            Kooboo.Transfer.getSubUrl({
                url: self.url(),
                pages: self.totalPages()
            }).then(function(res) {

                if (res.success) {
                    self.scanDone(true);
                    self.checkAllUrl(false);
                    self.checkedUrl([]);
                    self.urls([]);
                    _.forEach(res.model, function(u) {
                        self.urls.push(new urlModel(u));
                    });
                }
            })
        };

        this.scanDone = ko.observable(false);

        this.urls = ko.observableArray();

        this.checkedUrl = ko.observableArray();
        this.checkedUrl.subscribe(function(urls) {
            self.startDisabled(urls.length == 0);
        })

        this.getCheckedUrls = function() {
            var _urls = [];
            _.forEach(self.checkedUrl(), function(url) {
                _urls.push(url.value());
            });

            return JSON.stringify(_urls);
        }

        this.checkAllUrl = ko.pureComputed({
            read: function() {
                if (self.urls().length == 0) {
                    return false;
                }
                return self.urls().length == self.checkedUrl().length;
            },
            write: function(selected) {
                self.checkedUrl.removeAll();

                _.forEach(self.urls(), function(u) {
                    u.selected(selected);
                    selected && self.checkedUrl.push(u);
                })
            },
            owner: this
        });

        this.checkUrl = function(m) {
            m.selected(!m.selected());

            m.selected() ? self.checkedUrl.push(m) : self.checkedUrl.remove(m);
        }

        this.isValid = function() {
            return self.url.isValid() &&
                self.siteName.isValid() &&
                self.subDomain.isValid();
        }

        this.handleEnter = function(m, e) {
            if (e.keyCode !== 13) {
                return true;
            } else {
                if (self.convertType() == CONVERTYPE.AUTO || self.convertType() == CONVERTYPE.SEMI_AUTO) {
                    self.urlFocused(false);
                    self.startConvert();
                } else if (self.convertType() == CONVERTYPE.MANUAL) {
                    self.urlFocused(false);
                    self.scanStart();
                }
            }
        }

        this.startConvert = function() {
            if (self.isValid()) {
                self.showError(false);

                switch (self.convertType()) {
                    case CONVERTYPE.AUTO:
                    case CONVERTYPE.ONLINE:
                        Kooboo.Transfer.byLevel({
                            RootDomain: self.rootDomain(),
                            SubDomain: self.subDomain(),
                            SiteName: self.siteName(),
                            url: self.url()
                        }).then(function(res) {

                            if (res.success) {
                                location.href = Kooboo.Route.Get(Kooboo.Route.Site.Transferring, {
                                    TaskId: res.taskId,
                                    SiteId: res.siteId
                                })
                            }
                        })
                        break;
                    case CONVERTYPE.SEMI_AUTO:
                        Kooboo.Transfer.byLevel({
                            RootDomain: self.rootDomain(),
                            SubDomain: self.subDomain(),
                            SiteName: self.siteName(),
                            url: self.url(),
                            TotalPages: self.totalPages(),
                            Depth: self.depth()
                        }).then(function(res) {

                            if (res.success) {
                                location.href = Kooboo.Route.Get(Kooboo.Route.Site.Transferring, {
                                    TaskId: res.taskId,
                                    SiteId: res.siteId
                                })
                            }
                        })
                        break;
                    case CONVERTYPE.MANUAL:
                        Kooboo.Transfer.byPage({
                            SubDomain: self.subDomain(),
                            RootDomain: self.rootDomain(),
                            SiteName: self.siteName(),
                            Urls: self.getCheckedUrls(),
                        }).then(function(res) {

                            if (res.success) {
                                location.href = Kooboo.Route.Get(Kooboo.Route.Site.Transferring, {
                                    TaskId: res.taskId,
                                    SiteId: res.siteId
                                })
                            }
                        });
                        break;
                }
            } else {
                $("body").animate({
                    scrollTop: 0
                }, 100, function() {
                    self.showError(true);
                })
            }
        }

        this.SPAClick = function(m, e) {
            e.preventDefault();
            self.showError(false);
            Kooboo.SPA.getView(Kooboo.Route.Site.ListPage, {
                container: '[layout="default"]'
            })
        }
    }

    var urlModel = function(url) {
        var self = this;

        this.selected = ko.observable(false);

        this.value = ko.observable(url);
    }


    $.when(Kooboo.Domain.getAvailable()).then(function(availableRes) {
        if (availableRes.success) {
            var transVm = new transferViewModel(availableRes.model);
            $("#component_container_header").css({
                position: "absolute",
                zIndex: "200000"
            });
            ko.applyBindings(transVm, document.getElementById("main"));
        }
    });

});