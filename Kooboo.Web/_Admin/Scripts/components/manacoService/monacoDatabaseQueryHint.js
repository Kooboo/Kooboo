function monacoDatabaseQueryHint(monaco) {
  var _dbs = ["database", "mysql", "sqlserver", "sqlite"];
  var _hintMethods = [
    ".find('",
    ".findAll('",
    ".query('",
    ".orderBy('",
    ".orderByDescending('",
    ".where('",
  ];

  var _noHintMehods = [".skip(", ".take(", ".count("];

  var _dbTables = {};
  var _table_cols = {};

  monaco.languages.registerCompletionItemProvider("javascript", {
    triggerCharacters: ['"', "'", " ", "(", "."],
    provideCompletionItems: function (model, position) {
      var str = getStr(model, position);

      str = str.replace(/\s+/g, "").toLowerCase();

      var dbName = _dbs.find(function (f) {
        return str.indexOf("k." + f) > -1;
      });

      if (!dbName) return;

      if (!_dbTables[dbName]) {
        var tables = new Kooboo.HttpClientModel(dbName).executeGet(
          "tables",
          null,
          true,
          true,
          true
        ).responseJSON.model;

        _dbTables[dbName] = tables ? tables : [];
      }

      if (str.endsWith("k." + dbName + ".")) {
        return getResult(_dbTables[dbName]);
      }

      str = str.replace(/\"+/g, "'");

      var table = _dbTables[dbName].find(function (f) {
        return (
          str.indexOf("k." + dbName + "." + f) > -1 ||
          str.indexOf("k." + dbName + ".gettable('" + f + "')") > -1
        );
      });

      if (!table) return;
      var tableKey = dbName + "_" + table;

      if (!_table_cols[tableKey]) {
        var cols = new Kooboo.HttpClientModel(dbName).executeGet(
          "columns",
          {
            table: table,
          },
          true,
          true,
          true
        ).responseJSON.model;

        _table_cols[tableKey] = cols
          ? cols.map(function (f) {
              return f.name;
            })
          : [];
      }

      if (
        _hintMethods.find(function (f) {
          return str.indexOf(f) > -1;
        }) &&
        !_noHintMehods.find(function (f) {
          return str.indexOf(f) > -1;
        })
      ) {
        var endsWithCol = _table_cols[tableKey].find(function (f) {
          return str.endsWith(f.toLowerCase());
        });

        if (endsWithCol) {
          return getResult([
            ">",
            ">=",
            "<",
            "<=",
            "=",
            "==",
            "!=",
            "contains",
            "startwith",
          ]);
        }

        return getResult(_table_cols[tableKey]);
      }
    },
  });

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
        startColumn: model.getLineLastNonWhitespaceColumn(startLine),
        endLineNumber: startLine - 1,
        endColumn: model.getLineLastNonWhitespaceColumn(startLine) + 1,
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
