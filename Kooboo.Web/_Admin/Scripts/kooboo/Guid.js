(function() {
    var Guid = (function() {
        function Guid() {}
        Guid.NewGuid = function() {
            return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
                var r = Math.random() * 16 | 0,
                    v = c == "x" ? r : (r & 0x3 | 0x8);
                return v.toString(16);
            });
        }

        Guid.isValid = function(value) {
            var regex = new RegExp("^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$", "i");
            return regex.test(value);
        }

        Guid.Empty = "00000000-0000-0000-0000-000000000000";
        return Guid;
    })();

    var guid = {
        Empty: Guid.Empty,
        NewGuid: Guid.NewGuid,
        isValid: Guid.isValid
    };

    Kooboo.Guid = guid;
})();