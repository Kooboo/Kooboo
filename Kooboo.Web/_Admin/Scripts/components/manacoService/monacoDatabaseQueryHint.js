function monacoDatabaseQueryHint(monaco) {
  var _dbs = ["database", "mysql", "sqlserver", "sqlite"];
  var _dbTables = {};
  var _table_cols = {};

  monaco.languages.registerCompletionItemProvider("javascript", {
    triggerCharacters: ['"', "'", " ", "(", ")", "."],
    provideCompletionItems: function (model, position) {
      var str = model.getValueInRange({
        startLineNumber: position.lineNumber,
        startColumn: 1,
        endLineNumber: position.lineNumber,
        endColumn: position.column,
      });


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
    },
  });

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
