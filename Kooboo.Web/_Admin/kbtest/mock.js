(function (global) {
    var MockCache = function () {
        var self = this;
        self.index = 0;
        self.cacheIndex = [];
        self.cacheMock = {};

        self.add = function (instance, mock) {
            self.index += 1;
            var cacheIndex = {
                instance: instance,
                index: self.index
            };
            self.cacheIndex.push(cacheIndex);

            self.cacheMock[self.index] = mock;

        };
        self.get = function (instance) {
            var mockManager;
            for (var i = 0; i < self.cacheIndex.length; i++) {
                var cacheData = self.cacheIndex[i];
                if (cacheData && cacheData.instance == instance) {
                    var index = cacheData.index;
                    if (self.cacheMock[index]) {
                        mockManager = self.cacheMock[index];
                        break;
                    }
                }
            }
            return mockManager;
        }
    }

    window.KMock = function (instance) {
        var mock = KMock.cache.get(instance);
        if (!mock) {
            mock = new MockManager(instance);
            KMock.cache.add(instance, mock);
        }
        return mock;
    };
    
    KMock.cache = new MockCache();
 
    var MockManager=function(mockedObject) {
        var self = this;
        var mockKeyName = "_kmock";
        var mockedObject = mockedObject;
        var tempObject={}
        function buildMember(path, content) {
            var parts = path.split('.');
            var index = 0;
            var currentObject = mockedObject;
            while (index < parts.length - 1) {
                var member = currentObject[parts[index]];
                if (!member) {
                    currentObject = currentObject[parts[index]] = {};
                } else {
                    currentObject = currentObject[parts[index]];
                }
                index++;
            }
            //save original value
            currentObject[mockKeyName + parts[parts.length - 1]] = currentObject[parts[parts.length - 1]];
            currentObject[parts[parts.length - 1]] = content;
            
        }
        self.callFake=function(name,returns) {
            buildMember(name, returns);
            return this;
        }

        self.flush = function () {
            var keys = Object.keys(mockedObject);
            var mockKeys = [];
            for (var i = 0; i < keys.length; i++) {
                var key = keys[i];
                if (key.indexOf(mockKeyName) == 0) {
                    mockKeys.push(key);
                }
            }

            for (var i = 0; i < mockKeys.length; i++) {
                var mockKey = mockKeys[i];
                key = mockKey.replace(mockKeyName, "");
                mockedObject[key] = mockedObject[mockKey];
                delete mockedObject[mockKey];
            }

        }

    }
})(window);
