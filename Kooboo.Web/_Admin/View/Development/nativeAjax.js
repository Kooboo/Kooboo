function ajax(options) {
    options = options || {};

    options.type = (options.type || "GET").toUpperCase();

    options.dataType = options.dataType || "json";

    var params = getParams(option.data);

    var xhr = null;
    if (window.XMLHttpRequest) {
        xhr = new XMLHttpRequest();
    } else {
        xhr = new ActiveXObject("Microsoft.XMLHTTP");
    }

    xhr.onreadystatechange = function() {
        if (xhr.readyState == 4) {
            var status = xhr.status;

            if (status >= 200 && status < 300) {
                options.success && options.success(xhr.responseText, xhr.responseXML);
            } else {
                options.fail && options.fail(status);
            }
        }
    }

    if (options.type == "GET") {
        xhr.open("GET", options.url + "?" + params, true);
        xhr.send(null);
    } else if (options.type == "POST") {
        xhr.open("POST", options.url, true);
        xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        xhr.send(params);
    }
}

function getParams(data) {

    if (typeof data == "string") {

    } else {
        var arr = [],
            keys = Object.keys(data);

        for (var i = 0, len = keys.length; i < len; i++) {
            arr.push(encodeURIComponent(key) + "=" + encodeURIComponent(data[key]));
        }

        arr.push(("v=" + Math.random() * 10).replace(".", ""))

        return arr.join("&");
    }
}