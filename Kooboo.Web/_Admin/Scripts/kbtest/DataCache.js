function searchLocalStorage(key) {
    var result = {};

    for (var i = 0; i < localStorage.length; i++) {
        var localkey = localStorage.key(i);
        if (localkey.indexOf(key) != -1) {

            result[localkey] = localStorage[localkey];
        }
    }
    return result;
}

function GetPropertyHash(paradata) {
    if (!paradata) {
        return 0;
    }
    var value = "";
    if (typeof(paradata) === "string") {
        value = paradata;
    } else {
        for (var key in paradata) {
            value += paradata[key];
        }
    }
    return value.hashCode();
}

function GetPropertyHashTest(params) {

    var data = {
        a: "b"
    };
    var hash = GetPropertyHash(data);

    var diff = {
        b: "a"
    };
    var same = {
        a: "b"
    };
    var diffhash = GetPropertyHash(diff);
    var samehash = GetPropertyHash(same);

    expect(hash).to.be(samehash);
    expect(hash).to.not.be(diffhash);
}

function localStorage_search() {
    var item1 = {
        dd: "value"
    };
    var item2 = {
        dd: "value2"
    };
    localStorage.setItem("myuniquekey1", item1);
    localStorage.setItem("myuniquekey2", item2);

    var search = searchLocalStorage("myuniquekey");

    var count = 0;
    for (var key in search) {
        count += 1;
    }
    expect(count).to.be(2);

    search = searchLocalStorage("myuniquekey2");

    count = 0;
    for (var key in search) {
        count += 1;
    }
    expect(count).to.be(1);

    localStorage.removeItem("myuniquekey1");
    localStorage.removeItem("myuniquekey2");
}

function DataCache_GetKey(params) {

    var keyone = DataCache.getKey("page", "get");
    var keytwo = DataCache.getKey("page", "get", {
        id: "aaa"
    });

    var keythree = DataCache.getKey("page", "get");

    expect(keyone).to.not.be(keytwo);
    expect(keyone).to.be(keythree);
}

function DataCache_get_item_from_cache() {
    var key = DataCache.getKey("page", "get", {
        id: "xxx"
    });
    var data = {
        id: "aa",
        value: "string value"
    };
    localStorage.setItem(key, JSON.stringify(data));

    var valueback = DataCache.getData("page", "get", {
        id: "xxx"
    }).done(function(res) {
        expect(data.id).to.be(res.model.id);
    });

}

function DataCache_Get_Data_From_Remote() {
    var backup = DataCache.requestData;
    DataCache.requestData = function() {
        var result = {};
        result.model = { a: "a" };
        result.success = true;
        return $.Deferred().resolve(result);
    }

    var key = DataCache.getKey("page", "get", {
        id: "xyz"
    });
    localStorage.removeItem(key);
    DataCache.getData("page", "get", {
        id: "xyz"
    }).done(function(res) {
        expect(res.model.a).to.be("a");
    });

    DataCache.requestData = backup;
}

function DataCache_Remove_Item(params) {

    var backup = DataCache.getSiteId();
    DataCache.getSiteId = function() {
        return "xxxxsiteid";
    }

    var key = DataCache.getKey("anytype", "anymethod");
    localStorage.setItem(key, "test value");

    DataCache.removeData("anytype");
    var value = localStorage.getItem(key);
    expect(value).to.be(null);
    DataCache.getSiteId = backup;
}