(function(KB, $) {
    KB.VersionManager = {
        ADMIN_KEY: "SYSTEM_ADMIN_VERSION",
        SITE_VERSION_PREFIX_KEY: "SITE_VERSION_",
        TIME_KEY: "LAST_CHECKED_TIME",
        KB_USER_API_KEY: "_kooboo_api_user",
        init: function() {
            var self = this;
            if (self.shouldCheckVersion()) {
                KB.System.Version().then(function(res) {
                    if (res.success) {
                        self.setAdminVersion(new Version(res.model.admin));
                        self.setSitesVersion(res.model.siteVersions);
                    }
                })
            }
        },
        shouldCheckVersion: function(input) {
            var self = this;
            if (!input) {
                input = location.pathname;
            }
            if (input.toLowerCase().indexOf("/_admin/account") != -1 || input === "/") {
                return false;
            }

            var isUserExist = localStorage.getItem(this.KB_USER_API_KEY),
                currentUser = getCurrentUser();
            if (isUserExist) {
                if (currentUser !== isUserExist) {
                    localStorage.clear();
                    localStorage.setItem(this.KB_USER_API_KEY, currentUser);
                    return true;
                }
            } else {
                currentUser && localStorage.setItem(this.KB_USER_API_KEY, currentUser);
            }

            var lastCheckTime = getLastCheckTime();
            if (lastCheckTime == -1 ||
                (lastCheckTime > 0 && (Date.now() - lastCheckTime >= 3 * 60 * 1000))) {
                setCheckedTime();
                return true;
            }

            return false;

            function setCheckedTime() {
                sessionStorage.setItem(self.TIME_KEY, Date.now());
            }

            function getLastCheckTime() {
                var time = sessionStorage.getItem(self.TIME_KEY);
                return time ? Number(time) : -1;
            }

            function getCurrentUser() {
                var cookiestring = RegExp("" + self.KB_USER_API_KEY + "[^;]+").exec(document.cookie);
                return decodeURIComponent(!!cookiestring ? cookiestring.toString().replace(/^[^=]+./, "") : "");
            }
        },
        setAdminVersion: function(onlineVer) {
            var localVer = new Version(sessionStorage.getItem(this.ADMIN_KEY)); 
            if (!localVer.isValid()) {
                sessionStorage.setItem(this.ADMIN_KEY, onlineVer.toString());
            } else if (onlineVer.isDiffFrom(localVer)) {
                sessionStorage.setItem(this.ADMIN_KEY, onlineVer.toString());
                this.onAdminVersionChange(localVer, onlineVer);
            }
        },
        onAdminVersionChange: function(oldVersion, newVersion) {
            localStorage.clear();
            location.reload();
        },
        setSitesVersion: function(sites) {
            var self = this,
                list = KB.objToArr(sites);
            list.forEach(function(site) {
                var id = site.key,
                    _oldVer = self.getSiteVersionById(id),
                    version = Number(site.value);
                if (_oldVer > -1) {
                    if (_oldVer < version) {
                        sessionStorage.setItem(self.SITE_VERSION_PREFIX_KEY + id, version);
                        self.onSiteVersionChange(id, version);
                    }
                } else {
                    sessionStorage.setItem(self.SITE_VERSION_PREFIX_KEY + id, version);
                }
            });
        },
        getSiteVersionById: function(id) {
            if (id) {
                var version = sessionStorage.getItem(this.SITE_VERSION_PREFIX_KEY + id);
                return version ? Number(version) : -1;
            }
        },
        onSiteVersionChange: function(siteId, newVersion) {
            var removed = [];
            for (var i = 0, len = localStorage.length; i < len; i++) {
                var cachedKey = localStorage.key(i);
                if (cachedKey.indexOf(siteId) > -1) {
                    removed.push(cachedKey);
                }
            }

            removed.forEach(function(rm) {
                localStorage.removeItem(rm);
            })
        },
        clearCheckedTime: function() {
            sessionStorage.removeItem(this.TIME_KEY);
        }
    };

    var Version = function(ver) {
        this.name = '';
        this._major = -1;
        this._minor = -1;
        this._patch = -1;
        this._revision = -1;

        if (ver) {
            if (typeof ver == 'number') {
                ver = ver + '.0.0'
            };
            this.name = ver;
            var temp = ver.split('.');
            this._major = Number(temp[0]);
            this._minor = Number(temp[1] || 0);
            this._patch = Number(temp[2] || 0);
            this._revision = Number(temp[3] || 0);
        }

        this.toString = function() {
            return this.name;
        }

        this.isDiffFrom = function (comp) {
            if (this._major !== comp._major) {
                return true;
            }
            if (this._minor !== comp._minor) {
                return true;
            }
            if (this._patch !== comp._patch) {
                return true;
            }
            if (this._revision !== comp._revision) {
                return true;
            }
            return false;
        }

        this.isNewerThan = function(comp) {
            if (this._major > comp._major) {
                return true;
            }
            if (this._minor > comp._minor) {
                return true;
            }
            if (this._patch > comp._patch) {
                return true;
            }
            if (this._revision > comp._revision) {
                return true;
            }
            return false;
        }

        this.isValid = function() {
            if (this._major < 0 || this._minor < 0 || this._patch < 0 || this._revision < 0) {
                return false;
            }
            if (this._major == 0 && this._minor == 0 && this._patch == 0 && this._revision == 0) {
                return false;
            }
            return true;
        }
    }
    KB.Version = Version;

})(Kooboo, jQuery)