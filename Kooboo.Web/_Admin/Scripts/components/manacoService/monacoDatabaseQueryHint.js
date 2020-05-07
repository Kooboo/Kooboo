function monacoDatabaseQueryHint(monaco) {
  var _dbs = ["database", "mysql", "sqlserver", "sqlite", "mongo"];

  var _hintMethods = [
    ".find('",
    ".findall('",
    ".query('",
    ".orderby('",
    ".orderbydescending('",
    ".where('",
  ];

  var _comparers = [
    ">",
    ">=",
    "<",
    "<=",
    "=",
    "==",
    "!=",
    "contains",
    "startwith",
  ];

  var _noHintMehods = [".skip(", ".take(", ".count("];

  var _dbTables = {};
  var _table_cols = {};

  monaco.languages.registerCompletionItemProvider("javascript", {
    triggerCharacters: ['"', "'", " ", "(", "."],
    provideCompletionItems: provideCompletionItems,
  });

  monaco.languages.registerCompletionItemProvider("html", {
    triggerCharacters: ['"', "'", " ", "(", "."],
    provideCompletionItems: provideCompletionItems,
  });

  function provideCompletionItems(model, position) {
    var str = getStr(model, position);

    str = str.replace(/\s+/g, "").toLowerCase();

    var dbName = _dbs.find(function (f) {
      return str.indexOf("k." + f) > -1;
    });

    if (!dbName) return;

    return new Promise(function (rs, rj) {
      new Promise(function (tableRs) {
        if (_dbTables[dbName]) tableRs();
        else {
          new Kooboo.HttpClientModel("kscript")
            .executeGet("gettables", { database: dbName }, true, false, true)
            .then(function (rsp) {
              _dbTables[dbName] = rsp.model ? rsp.model : [];
              tableRs();
            });
        }
      }).then(function () {
        if (str.endsWith("k." + dbName + ".")) {
          rs(getResult(_dbTables[dbName]));
        }

        str = str.replace(/\"+/g, "'");

        var table = _dbTables[dbName].find(function (f) {
          return (
            str.indexOf("k." + dbName + "." + f) > -1 ||
            str.indexOf("k." + dbName + ".gettable('" + f + "')") > -1
          );
        });

        if (!table) return rs();
        var tableKey = dbName + "_" + table;

        new Promise(function (colRs) {
          if (_table_cols[tableKey]) colRs();
          else {
            new Kooboo.HttpClientModel("kscript")
              .executeGet(
                "getcolumns",
                {
                  database: dbName,
                  table: table,
                },
                true,
                false,
                true
              )
              .then(function (rsp) {
                _table_cols[tableKey] = rsp.model;
                colRs();
              });
          }
        }).then(function () {
          if (canHintCol(str)) {
            var endsWithCol = _table_cols[tableKey].find(function (f) {
              return str.endsWith(f.toLowerCase());
            });

            if (endsWithCol) rs(getResult(_comparers));
            else rs(getResult(_table_cols[tableKey]));
          } else rj();
        });
      });
    });
  }

  function canHintCol(str) {
    return (
      _hintMethods.find(function (f) {
        return str.indexOf(f) > -1;
      }) &&
      !_noHintMehods.find(function (f) {
        return str.indexOf(f) > -1;
      })
    );
  }

  function getStr(model, position) {
    var startLine = position.lineNumber;
    while (startLine > 0) {
      var firstChar = model.getValueInRange({
        startLineNumber: startLine,
        startColumn: model.getLineFirstNonWhitespaceColumn(startLine),
        endLineNumber: startLine,
        endColumn: model.getLineFirstNonWhitespaceColumn(startLine) + 1,
      });

      var preLineLastChat = model.getValueInRange({
        startLineNumber: startLine - 1,
        startColumn: model.getLineLastNonWhitespaceColumn(startLine - 1),
        endLineNumber: startLine - 1,
        endColumn: model.getLineLastNonWhitespaceColumn(startLine - 1) + 1,
      });

      if (firstChar == "." || preLineLastChat == ".") {
        startLine--;
      } else {
        break;
      }
    }

    return model.getValueInRange({
      startLineNumber: startLine,
      startColumn: 1,
      endLineNumber: position.lineNumber,
      endColumn: position.column,
    });
  }

  function getResult(arr) {
    return {
      suggestions: arr.map(function (item) {
        return {
          label: item,
          kind: monaco.languages.CompletionItemKind.Property,
          documentation: item,
          insertText: item,
        };
      }),
    };
  }
}
