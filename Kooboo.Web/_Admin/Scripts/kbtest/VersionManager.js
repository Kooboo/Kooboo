function MANA_ShouldCheck() {
    sessionStorage.removeItem("LAST_CHECKED_TIME");
    var flag = Kooboo.VersionManager.shouldCheckVersion("/_admin/account/login");
    expect(flag).to.be(false);

    sessionStorage.removeItem("LAST_CHECKED_TIME");
    var flag2 = Kooboo.VersionManager.shouldCheckVersion("/_admin/sites");
    expect(flag2).to.be(true);

    sessionStorage.removeItem("LAST_CHECKED_TIME");
    var flag3 = Kooboo.VersionManager.shouldCheckVersion("/_admin/sites");
    var flag4 = Kooboo.VersionManager.shouldCheckVersion("/_admin/sites");
    expect(flag3).to.be(true);
    expect(flag4).to.be(false);

}

function MANA_CheckTimeOut() {
    sessionStorage.removeItem("LAST_CHECKED_TIME");
    var flag5 = Kooboo.VersionManager.shouldCheckVersion("/_admin/sites");
    var time = Number(sessionStorage.getItem("LAST_CHECKED_TIME")) - 4 * 60 * 1000;
    sessionStorage.setItem("LAST_CHECKED_TIME", time);
    var flag6 = Kooboo.VersionManager.shouldCheckVersion("/_admin/sites");
    expect(flag5).to.be(true);
    expect(flag6).to.be(true);
}

function MANA_GetSiteVersion() {
    var id1 = "e9c57acd-852c-38ad-f47f-74f847d7006e",
        id2 = "e9c57acd-852c-38ad-f47f-74f847d7006f";

    sessionStorage.setItem("SITE_VERSION_" + id1, "44");

    var ver1 = Kooboo.VersionManager.getSiteVersionById(id1);
    var ver2 = Kooboo.VersionManager.getSiteVersionById(id2);
    expect(ver1).to.be(44);
    expect(ver2).to.be(-1);
}

function MANA_SetSitesVersion() {
    sessionStorage.clear();
    var id = "e9c57acd-852c-38ad-f47f-74f847d7006e";
    sessionStorage.setItem("SITE_VERSION_" + id, "44");
    var mock = {
        "e9c57acd-852c-38ad-f47f-74f847d7006e": "46"
    }
    Kooboo.VersionManager.setSitesVersion(mock);
    var ver = Kooboo.VersionManager.getSiteVersionById(id);
    expect(ver).to.be(46);
}

function MANA_clearCheckedTime() {
    sessionStorage.setItem('LAST_CHECKED_TIME', Date.now())

    Kooboo.VersionManager.clearCheckedTime();

    var time = sessionStorage.getItem('LAST_CHECKED_TIME');
    expect(time).to.be(null);
}