using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;
using Kooboo.IndexedDB.Query;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Caching.Memory;

namespace Kooboo.Mail
{

    public class SqlLiteHelper
    {
        private static object _synclocker = new object();

        public SqlLiteHelper(string DbPath)
        {
            var fullPath = System.IO.Path.GetFullPath(DbPath);
            Kooboo.Lib.Helper.IOHelper.EnsureFileDirectoryExists(DbPath);
            // this.ConnectionString = "Data Source='" + fullPath + "';PRAGMA Pooling=true;PRAGMA journal_mode = WAL;PRAGMA synchronous = NORMAL";
            this.ConnectionString = "Data Source='" + fullPath + "';Pooling=true;";
            ///PRAGMA journal_mode = WAL
            //PRAGMA synchronous = NORMAL
        }

        private bool _keepalive;
        public bool KeepConnectionAlive
        {
            get
            {
                return _keepalive;
            }
            set
            {
                _keepalive = value;
                if (_keepalive == false)
                {
                    if (_aliveConn != null)
                    {
                        _aliveConn.Dispose();
                    }
                }
            }

        }

        private string ConnectionString { get; set; }

        private SqliteConnection NewConnection
        {
            get
            {
                var _connnection = new SqliteConnection(this.ConnectionString);
                _connnection.Open();
                SetConnectionWALMode(_connnection);
                return _connnection;
            }
        }

        private SqliteConnection _aliveConn;
        private SqliteConnection AliveConnection
        {
            get
            {
                if (_aliveConn == null)
                {
                    _aliveConn = new SqliteConnection(this.ConnectionString);
                    _aliveConn.Open();
                    SetConnectionWALMode(_aliveConn);
                }
                if (_aliveConn.State != System.Data.ConnectionState.Open)
                {
                    _aliveConn.Open();
                    SetConnectionWALMode(_aliveConn);
                }
                return _aliveConn;
            }
        }

        private void SetConnectionWALMode(SqliteConnection aliveConn)
        {
            lock (_synclocker)
            {
                try
                {
                    using (var cmd = aliveConn.CreateCommand())
                    {
                        cmd.CommandText =
                        @"
                        PRAGMA journal_mode = 'wal'
                    ";
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (System.Exception)
                {
                }
            }
        }

        public SqliteConnection GetConnection()
        {
            if (this.KeepConnectionAlive)
            {
                return this.AliveConnection;
            }
            else
            {
                return this.NewConnection;
            }
        }

        public IEnumerable<T> Query<T>(string sql)
        {
            lock (_synclocker)
            {
                var conn = GetConnection();
                try
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        var reader = cmd.ExecuteReader();
                        var list = MapToList<T>(reader);
                        reader.Close();
                        return list;
                    }
                }
                catch (Exception)
                {

                }
                finally
                {
                    if (!this.KeepConnectionAlive)
                    {
                        conn.Dispose();
                    }
                }
                return null;
            }

        }


        public async Task<IEnumerable<T>> QueryAsync<T>(string sql)
        {
            var conn = GetConnection();
            try
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    var reader = await cmd.ExecuteReaderAsync();
                    var list = MapToList<T>(reader);
                    reader.Close();
                    return list;
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                if (!this.KeepConnectionAlive)
                {
                    conn.Dispose();
                }
            }
            return null;
        }

        public T QueryFirstOrDefault<T>(string Sql)
        {
            lock (_synclocker)
            {
                var conn = GetConnection();
                try
                {
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = Sql;
                        var reader = command.ExecuteReader();
                        var obj = MapToObject<T>(reader);
                        reader.Close();
                        return obj;
                    }
                }
                catch (Exception)
                {

                }
                finally
                {
                    if (!this.KeepConnectionAlive)
                    {
                        conn.Dispose();
                    }
                }
                return default(T);
            }
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string Sql)
        {
            var conn = GetConnection();
            try
            {
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = Sql;
                    var reader = await command.ExecuteReaderAsync();
                    var obj = MapToObject<T>(reader);
                    reader.Close();
                    return obj;
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                if (!this.KeepConnectionAlive)
                {
                    conn.Dispose();
                }
            }
            return default(T);
        }


        public void Execute(string Sql)
        {
            lock (_synclocker)
            {
                var conn = GetConnection();
                try
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Sql;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {

                }
                finally
                {
                    if (!this.KeepConnectionAlive)
                    {
                        conn.Dispose();
                    }
                }
            }
        }

        public async Task ExecuteAsync(string Sql)
        {
            var conn = GetConnection();
            try
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = Sql;
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                if (!this.KeepConnectionAlive)
                {
                    conn.Dispose();
                }
            }
        }

        public T ExecuteScalar<T>(string Sql)
        {
            lock (_synclocker)
            {
                var conn = GetConnection();
                try
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Sql;
                        var obj = cmd.ExecuteScalar();

                        if (obj == null || obj is System.DBNull)
                        {
                            return default(T);
                        }
                        else
                        {
                            var TObj = ConvertType(typeof(T), obj);
                            return (T)TObj;
                        }
                    }
                }
                catch (Exception)
                {

                }
                finally
                {
                    if (!this.KeepConnectionAlive)
                    {
                        conn.Dispose();
                    }
                }
                return default(T);
            }
        }

        public async Task<T> ExecuteScalarAsync<T>(string Sql)
        {
            var conn = GetConnection();
            try
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = Sql;

                    var obj = await cmd.ExecuteScalarAsync();

                    if (obj == null || obj is System.DBNull)
                    {
                        return default(T);
                    }
                    else
                    {
                        var TObj = ConvertType(typeof(T), obj);
                        return (T)TObj;
                    }
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                if (!this.KeepConnectionAlive)
                {
                    conn.Dispose();
                }
            }
            return default(T);
        }

        // can only be used with keep alive
        public void BeginTransaction()
        {
            if (!KeepConnectionAlive)
            {
                throw new Exception("transaction can only used with Keep Alive Connection");
            }
            this.AliveConnection.BeginTransaction();
        }

        public void Commit()
        {
            this.Execute("Commit");
        }

        public void CloseActiveConnection()
        {
            this.AliveConnection.Close();
        }

        private object ConvertType(Type destType, object Value)
        {

            if (destType.IsEnum)
            {
                if (int.TryParse(Value.ToString(), out int result))
                {
                    return result;
                }
                else
                {
                    var value = Value.ToString();
                    var obj = Lib.Helper.EnumHelper.GetEnum(destType, value);

                    if (obj == null)
                    {
                        if (int.TryParse(value, out int enumvalue))
                        {
                            return enumvalue;
                        }
                    }
                    else
                    {
                        return obj;
                    }
                }
            }

            else if (destType == typeof(DateTime) || destType == typeof(DateOnly) || destType == typeof(DateTimeOffset))
            {
                if (int.TryParse(Value.ToString(), out int result))
                {
                    var seconds = Convert.ToDouble(result);

                    return UTCStartdate.AddSeconds(seconds);

                }
                else
                {
                    // must be text.
                    if (DateTime.TryParse(Value.ToString(), out var date))
                    {
                        return date;
                    }
                }
            }

            else
            {

                return ChangeType(Value, destType);


            }

            return null;

        }

        internal MemoryCache colCache = new MemoryCache(new MemoryCacheOptions());


        public List<SqliteColumn> GetColumns(string tableName)
        {


            List<SqliteColumn> cols = null;

            if (colCache.TryGetValue<List<SqliteColumn>>(tableName, out List<SqliteColumn> result))
            {
                return result;
            }

            var sql = "PRAGMA table_info(" + tableName + ");";
            cols = this.Query<SqliteColumn>(sql).ToList();

            // Find PK. 
            var find = cols.ToList().Find(o => o.pk == true);
            if (find != null && find.type == "INTEGER")
            {
                // check if this is incremental. 
                var tableSql = @"SELECT type, name, sql FROM sqlite_master
WHERE type = 'table'   AND name = '" + tableName + "' AND sql LIKE '%AUTOINCREMENT%'";

                var master = this.Get<SqliteTable>(tableSql);

                if (master != null && master.sql.Contains("AUTOINCREMENT"))
                {
                    find.incremental = true;
                }
            }

            if (cols != null && cols.Any())
            {
                colCache.Set<List<SqliteColumn>>(tableName, cols, TimeSpan.FromHours(24));
                return cols;
            }

            return new List<SqliteColumn>();
        }

        public IEnumerable<string> GetTables()
        {
            string sql = "SELECT name FROM sqlite_master WHERE type='table';";
            return this.Query<string>(sql);
        }

        public void Update(string TableName, string KeyName, object Id, Dictionary<string, object> Values)
        {
            string Sql = "UPDATE " + TableName + " SET ";
            List<string> updatevalues = UpdateItemSQL(TableName, Values, KeyName);
            if (updatevalues.Any())
            {
                Sql += string.Join(",", updatevalues);
                Sql += " WHERE " + KeyName + "=" + ObjToString(Id);
                this.Execute(Sql);
            }
        }

        public void Update<T>(T value, string tableName, string keyname)
        {
            var id = Lib.Reflection.Dynamic.GetObjectMember(value, keyname);
            if (id != null)
            {
                var values = Lib.Reflection.Dynamic.GetMemberValues(value);
                Update(tableName, keyname, id, values);

            }
        }


        public void Insert(string TableName, Dictionary<string, object> Values)
        {
            var sql = "INSERT INTO " + TableName;
            sql += InsertSQL(TableName, Values);
            this.Execute(sql);
        }

        private List<string> UpdateItemSQL(string TableName, Dictionary<string, object> Values, string KeyName)
        {
            if (Values == null)
            {
                return new List<string>();
            }

            // Values["LastModified"] = DateTime.Now;

            var cols = GetColumns(TableName);
            List<string> updatevalues = new List<string>();
            foreach (var item in Values)
            {
                if (item.Key != KeyName)   // 
                {
                    var find = cols.Find(o => string.Compare(o.name, item.Key, true) == 0);

                    if (find != null && find.incremental == false)
                    {
                        var itemvalue = ObjToString(item.Value);
                        string ItemUpdate = EscapeReservedkeywords(item.Key) + "=" + itemvalue;
                        updatevalues.Add(ItemUpdate);
                    }
                }
            }
            return updatevalues;
        }

        private string InsertSQL(string TableName, Dictionary<string, object> Values)
        {
            if (Values == null)
            {
                return null;
            }

            // Values["LastModified"] = DateTime.Now;

            var cols = GetColumns(TableName);

            //INSERT INTO tableName ( CustomerName,...) VALUES (@CustomerName,...) 
            List<string> names = new List<string>();
            List<string> values = new List<string>();
            foreach (var item in Values)
            {
                var find = cols.Find(o => string.Compare(o.name, item.Key, true) == 0);

                if (find != null && find.incremental == false)
                {
                    var itemvalue = ObjToString(item.Value);
                    names.Add(EscapeReservedkeywords(item.Key));
                    values.Add(itemvalue);
                }
            }

            var strname = string.Join(",", names);
            var strvalue = string.Join(",", values);

            return "(" + strname + ") VALUES (" + strvalue + ")";
        }


        private HashSet<string> _reservedWords;
        private HashSet<string> ReservedWords
        {
            get
            {
                if (_reservedWords == null)
                {
                    var words = "ABORT\r\nACTION\r\nADD\r\nAFTER\r\nALL\r\nALTER\r\nALWAYS\r\nANALYZE\r\nAND\r\nAS\r\nASC\r\nATTACH\r\nAUTOINCREMENT\r\nBEFORE\r\nBEGIN\r\nBETWEEN\r\nBY\r\nCASCADE\r\nCASE\r\nCAST\r\nCHECK\r\nCOLLATE\r\nCOLUMN\r\nCOMMIT\r\nCONFLICT\r\nCONSTRAINT\r\nCREATE\r\nCROSS\r\nCURRENT\r\nCURRENT_DATE\r\nCURRENT_TIME\r\nCURRENT_TIMESTAMP\r\nDATABASE\r\nDEFAULT\r\nDEFERRABLE\r\nDEFERRED\r\nDELETE\r\nDESC\r\nDETACH\r\nDISTINCT\r\nDO\r\nDROP\r\nEACH\r\nELSE\r\nEND\r\nESCAPE\r\nEXCEPT\r\nEXCLUDE\r\nEXCLUSIVE\r\nEXISTS\r\nEXPLAIN\r\nFAIL\r\nFILTER\r\nFIRST\r\nFOLLOWING\r\nFOR\r\nFOREIGN\r\nFROM\r\nFULL\r\nGENERATED\r\nGLOB\r\nGROUP\r\nGROUPS\r\nHAVING\r\nIF\r\nIGNORE\r\nIMMEDIATE\r\nIN\r\nINDEX\r\nINDEXED\r\nINITIALLY\r\nINNER\r\nINSERT\r\nINSTEAD\r\nINTERSECT\r\nINTO\r\nIS\r\nISNULL\r\nJOIN\r\nKEY\r\nLAST\r\nLEFT\r\nLIKE\r\nLIMIT\r\nMATCH\r\nMATERIALIZED\r\nNATURAL\r\nNO\r\nNOT\r\nNOTHING\r\nNOTNULL\r\nNULL\r\nNULLS\r\nOF\r\nOFFSET\r\nON\r\nOR\r\nORDER\r\nOTHERS\r\nOUTER\r\nOVER\r\nPARTITION\r\nPLAN\r\nPRAGMA\r\nPRECEDING\r\nPRIMARY\r\nQUERY\r\nRAISE\r\nRANGE\r\nRECURSIVE\r\nREFERENCES\r\nREGEXP\r\nREINDEX\r\nRELEASE\r\nRENAME\r\nREPLACE\r\nRESTRICT\r\nRETURNING\r\nRIGHT\r\nROLLBACK\r\nROW\r\nROWS\r\nSAVEPOINT\r\nSELECT\r\nSET\r\nTABLE\r\nTEMP\r\nTEMPORARY\r\nTHEN\r\nTIES\r\nTO\r\nTRANSACTION\r\nTRIGGER\r\nUNBOUNDED\r\nUNION\r\nUNIQUE\r\nUPDATE\r\nUSING\r\nVACUUM\r\nVALUES\r\nVIEW\r\nVIRTUAL\r\nWHEN\r\nWHERE\r\nWINDOW\r\nWITH\r\nWITHOUT";

                    var parts = words.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

                    _reservedWords = new HashSet<string>(parts);
                }
                return _reservedWords;
            }
        }

        private string EscapeReservedkeywords(string column)
        {
            var uppper = column.ToUpper();
            if (ReservedWords.Contains(uppper))
            {
                return "[" + column + "]";
            }
            return column;
        }


        public string ObjToString(object obj)
        {
            if (obj == null)
            {
                return "";
            }

            var type = obj.GetType();
            if (type == typeof(Guid))
            {
                Guid id = (Guid)obj;
                var str = id.ToString();
                return "'" + str + "'";
            }
            else if (type == typeof(DateTime))
            {
                var date = (DateTime)obj;

                if (date == default(DateTime))
                {
                    //  date = DateTime.Now;
                }

                return "'" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            }
            else if (type == typeof(bool))
            {
                var yesno = (bool)obj;
                if (yesno)
                {
                    return "1";
                }
                else
                {
                    return "0";
                }
            }
            else if (type == typeof(int) || type == typeof(Int16) || type == typeof(byte) || type == typeof(Int64))
            {
                return obj.ToString();
            }
            //else if (type.IsEnum)
            //{
            //}
            else if (type == typeof(string))
            {
                var str = obj.ToString();
                if (str == null)
                {
                    return null;
                }
                else
                {
                    return "'" + str.Replace("'", "''") + "'";
                }
            }

            else if (!type.IsValueType && !type.IsEnum)
            {
                // non value type, using json. 
                string json = System.Text.Json.JsonSerializer.Serialize(obj, new JsonSerializerOptions()
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });

                return "'" + json.Replace("'", "''") + "'";
            }


            return "'" + obj.ToString() + "'";
        }

        public void Add<T>(T Value, string tableName)
        {
            var values = Lib.Reflection.Dynamic.GetMemberValues(Value);
            Insert(tableName, values);
        }

        public long AddGetId<T>(T Value, string tableName, string IDColName)
        {
            var values = Lib.Reflection.Dynamic.GetMemberValues(Value);

            var sql = "INSERT INTO " + tableName;
            sql += InsertSQL(tableName, values);

            sql += " RETURNING " + IDColName;

            var id = this.ExecuteScalar<long>(sql); // this.Execute(sql);

            return id;

        }


        public bool AddOrUpdate<T>(T value, string TableName, string keyName)
        {
            var id = Lib.Reflection.Dynamic.GetObjectMember(value, keyName);
            if (id != null)
            {
                var values = Lib.Reflection.Dynamic.GetMemberValues(value);
                return AddOrUpdate(id, TableName, keyName, values);
            }

            return false;
        }

        public bool AddOrUpdate(object Id, string TableName, string KeyName, Dictionary<string, object> values)
        {
            // if key is an autocremental value.
            var col = this.GetColumns(TableName);
            var find = col.Find(o => o.name == KeyName);
            if (find != null && find.incremental)
            {
                if (values.TryGetValue(KeyName, out object value))
                {
                    //has key, if zero. 
                    if (long.TryParse(value.ToString(), out long idvalue))
                    {
                        if (idvalue == 0)
                        {
                            Insert(TableName, values);
                            return true;
                        }
                    }
                }
                else
                {
                    // not key. insert. 
                    Insert(TableName, values);
                    return true;
                }
            }


            var currentitem = Get<object>(TableName, KeyName, Id);
            if (currentitem != null)
            {
                Update(TableName, KeyName, Id, values);
            }
            else
            {
                Insert(TableName, values);
            }
            return true;
        }


        public void Delete(string TableName, string KeyName, object id)
        {
            string sql = "DELETE FROM " + TableName + " WHERE " + EscapeReservedkeywords(KeyName) + "=" + ObjToString(id);
            this.Execute(sql);
        }
        public T Get<T>(string TableName, string KeyName, object Id)
        {
            var sql = "SELECT * FROM " + TableName + " WHERE " + EscapeReservedkeywords(KeyName) + "=" + ObjToString(Id);
            return this.QueryFirstOrDefault<T>(sql);
        }

        public T Get<T>(string TableName, Dictionary<string, object> criteria)
        {
            var criteriaList = new List<string>();
            foreach (var kvp in criteria)
            {
                criteriaList.Add($"{EscapeReservedkeywords(kvp.Key)} = {ObjToString(kvp.Value)}");
            }

            var sql = $"SELECT * FROM [{TableName}] WHERE {string.Join(" AND ", criteriaList.ToArray())}";
            return QueryFirstOrDefault<T>(sql);
        }

        public async Task<T> GetAsync<T>(string TableName, string KeyName, object Id)
        {
            var sql = "SELECT * FROM " + TableName + " WHERE " + EscapeReservedkeywords(KeyName) + "=" + ObjToString(Id);
            return await this.QueryFirstOrDefaultAsync<T>(sql);
        }


        public IEnumerable<T> FindAll<T>(string tableName, string FieldName, object CompareValue)
        {
            var sql = "SELECT * FROM " + tableName + " WHERE " + FieldName + "=" + ObjToString(CompareValue);
            return this.Query<T>(sql);
        }

        public T Find<T>(string tableName, string FieldName, object CompareValue)
        {
            var sql = "SELECT * FROM " + tableName + " WHERE " + EscapeReservedkeywords(FieldName) + "=" + ObjToString(CompareValue);
            return this.QueryFirstOrDefault<T>(sql);
        }


        public List<T> All<T>(string tableName)
        {
            var sql = "SELECT * FROM " + tableName;
            return this.Query<T>(sql).ToList();
        }


        public T Get<T>(string sql)
        {
            return this.QueryFirstOrDefault<T>(sql);
        }

        public async Task<T> GetAsync<T>(string sql)
        {
            return await this.QueryFirstOrDefaultAsync<T>(sql);
        }

        public SqlWhere<T> Where<T>(string TableName)
        {
            return new SqlWhere<T>(this, TableName);
        }


        public string GetInsertSql<T>(T value, string TableName, string keyName)
        {
            var id = Lib.Reflection.Dynamic.GetObjectMember(value, keyName);
            if (id != null)
            {
                var values = Lib.Reflection.Dynamic.GetMemberValues(value);
                var sql = "INSERT INTO " + TableName;
                sql += InsertSQL(TableName, values);
                return sql;
            }
            return null;
        }

        public string GetUpdateSql<T>(T value, string TableName, string keyName)
        {
            var id = Lib.Reflection.Dynamic.GetObjectMember(value, keyName);
            if (id != null)
            {
                var values = Lib.Reflection.Dynamic.GetMemberValues(value);

                string Sql = "UPDATE " + TableName + " SET ";
                List<string> updatevalues = UpdateItemSQL(TableName, values, keyName);
                if (updatevalues.Any())
                {
                    Sql += string.Join(",", updatevalues);
                    Sql += " WHERE " + keyName + "=" + ObjToString(id);
                    this.Execute(Sql);
                }

                return Sql;
            }
            return null;
        }

        #region Converter 

        private static ConcurrentDictionary<string, object> CacheConverter { get; set; } = new ConcurrentDictionary<string, object>();

        private SimpleSerializer<T> GetConverter<T>()
        {
            var name = typeof(T).Name;
            if (CacheConverter.TryGetValue(name, out var converter))
            {
                return converter as SimpleSerializer<T>;
            }

            var obj = new SimpleSerializer<T>();

            CacheConverter.TryAdd(name, obj);

            return obj;
        }

        private T MapToObject<T>(DbDataReader reader)
        {
            if (reader.Read())
            {
                HashSet<SqlFieldInfo> fields = new HashSet<SqlFieldInfo>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    SqlFieldInfo field = new SqlFieldInfo() { Name = reader.GetName(i), Value = reader.GetValue(i), Type = reader.GetFieldType(i) };
                    if (field.Value != null)
                    {
                        fields.Add(field);
                    }
                }

                var converter = GetConverter<T>();
                return converter.FromFieldValues(fields);
            }
            return default(T);
        }

        private List<T> MapToList<T>(DbDataReader reader)
        {
            var result = new List<T>();

            var converter = GetConverter<T>();

            while (reader.Read())
            {
                HashSet<SqlFieldInfo> fields = new HashSet<SqlFieldInfo>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    SqlFieldInfo field = new SqlFieldInfo() { Name = reader.GetName(i), Value = reader.GetValue(i), Type = reader.GetFieldType(i) };
                    if (field.Value != null)
                    {
                        fields.Add(field);
                    }
                }

                var item = converter.FromFieldValues(fields);
                result.Add(item);
            }

            return result;
        }

        #endregion

        public static DateTime UTCStartdate
        {
            get
            {
                return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            }
        }

        public static object ChangeType(object value, Type ConversionType)
        {
            if (value == null)
            {
                if (!ConversionType.IsValueType)
                {
                    return Activator.CreateInstance(ConversionType);
                }
                else
                {
                    return null;
                }
            }

            object result;

            if (ConversionType == typeof(String))
            {
                return value.ToString();
            }
            else if (ConversionType == typeof(Guid))
            {
                Guid id;
                if (Guid.TryParse(value?.ToString(), out id))
                {
                    return id;
                }
                return default(Guid);
            }
            else if (ConversionType == typeof(bool))
            {
                bool ok;

                string strValue = value.ToString().ToLower();

                if (bool.TryParse(strValue, out ok))
                {
                    return ok;
                }
                else if (strValue == "1" || strValue == "true" || strValue == "yes")
                {
                    return true;
                }
                return false;
            }
            else if (ConversionType == typeof(DateOnly))
            {
                if (value is DateTime)
                {
                    var datevalue = (DateTime)value;
                    return DateOnly.FromDateTime(datevalue);
                }
                else
                {
                    if (DateTime.TryParse(value?.ToString(), out var dateValue))
                    {
                        return DateOnly.FromDateTime(dateValue);
                    }
                    return DateOnly.MinValue;
                }
            }
            else
            {
                result = Convert.ChangeType(value, ConversionType);
            }

            return result;
        }

    }

    public class SqliteColumn
    {
        public string name
        { get; set; }

        public string type { get; set; }

        public bool pk { get; set; }

        public bool incremental { get; set; }   // auto incremental. 
    }


    public class SqliteTable
    {
        public string type { get; set; }
        public string name { get; set; }

        public string sql { get; set; }

        //        SELECT type, name, sql
        //FROM sqlite_master
        //WHERE type = 'table'
        //  AND name = 'DmarcReport'
        //  AND sql LIKE '%AUTOINCREMENT%'

    }


    public class ColumnCache
    {
        public List<SqliteColumn> Columns { get; set; }
        public DateTime LastCheck { get; set; }
    }

    public class SimpleSerializer<T>
    {
        public Dictionary<string, SetterInfo<T>> FieldSetter { get; set; } = new Dictionary<string, SetterInfo<T>>(StringComparer.OrdinalIgnoreCase);

        public bool IsValueType { get; set; }

        private Type TType { get; set; }

        public SimpleSerializer()
        {
            var type = typeof(T);
            this.TType = type;

            if (type.IsValueType || type == typeof(string))
            {
                this.IsValueType = true;
            }
            else
            {

                var allfields = GetPublicPropertyOrFields(type);

                foreach (var item in allfields)
                {
                    var action = GetSetObjectValue<T>(item.Key, item.Value);
                    SetterInfo<T> setter = new SetterInfo<T>();
                    setter.Name = item.Key;
                    setter.Action = action;
                    setter.FieldType = item.Value;

                    FieldSetter.Add(item.Key, setter);
                }
            }
        }

        public T FromFieldValues(HashSet<SqlFieldInfo> Values)
        {

            if (this.IsValueType)
            {
                var first = Values.First();

                var obj = ConvertValueType(this.TType, first.Value, first.Type);

                return obj != null ? (T)obj : default(T);
            }
            else
            {

                T result = Activator.CreateInstance<T>();

                foreach (var item in Values)
                {
                    if (item.Value == null || item.Value == DBNull.Value)
                    {
                        continue;
                    }

                    if (FieldSetter.TryGetValue(item.Name, out var setter))
                    {
                        if (!setter.FieldType.IsValueType && setter.FieldType != typeof(string))
                        {
                            var obj = System.Text.Json.JsonSerializer.Deserialize(item.Value.ToString(), setter.FieldType);
                            setter.Action(result, obj);
                        }

                        else
                        {
                            var obj = ConvertValueType(setter.FieldType, item.Value, item.Type);
                            if (obj != null)
                            {
                                setter.Action(result, obj);
                            }
                        }
                    }
                }

                return result;
            }

        }

        public object ConvertValueType(Type destType, object Value, Type valueType)
        {

            if (destType == typeof(bool))
            {
                var value = Value.ToString().ToLower();
                if (value == "0" || value == "false")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            else if (destType.IsEnum)
            {
                if (valueType == typeof(Int32) || valueType == typeof(byte) || valueType == typeof(Int64) || valueType == typeof(Int16))
                {
                    return Convert.ToInt32(Value);
                }
                else
                {
                    var value = Value.ToString();
                    var obj = Lib.Helper.EnumHelper.GetEnum(destType, value);

                    if (obj == null)
                    {
                        if (int.TryParse(value, out int enumvalue))
                        {
                            return enumvalue;
                        }
                    }
                    else
                    {
                        return obj;
                    }
                }
            }

            else if (destType == typeof(DateTime) || destType == typeof(DateOnly) || destType == typeof(DateTimeOffset))
            {
                if (valueType == typeof(Int16) || valueType == typeof(Int32) || valueType == typeof(Int64))
                {
                    var seconds = Convert.ToDouble(Value);

                    return SqlLiteHelper.UTCStartdate.AddSeconds(seconds);

                }
                else
                {
                    // must be text.
                    if (DateTime.TryParse(Value.ToString(), out var date))
                    {
                        return date;
                    }
                }
            }

            //else if (!setter.FieldType.IsValueType && setter.FieldType != typeof(string))
            //{
            //    var obj = System.Text.Json.JsonSerializer.Deserialize(item.Value.ToString(), setter.FieldType);
            //    setter.Action(result, obj);
            //}


            else
            {
                if (destType != valueType)
                {
                    return SqlLiteHelper.ChangeType(Value, destType);
                }
                else
                {
                    return Value;
                }

            }

            return null;

        }

        public Dictionary<string, Type> GetPublicPropertyOrFields(Type ClassType)
        {
            Dictionary<string, Type> result = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in ClassType.GetProperties())
            {
                if (item.CanRead && item.CanWrite)
                {
                    result.Add(item.Name, item.PropertyType);
                }
            }

            foreach (var item in ClassType.GetFields())
            {
                if (item.IsPublic && !item.IsStatic)
                {
                    result.Add(item.Name, item.FieldType);
                }
            }

            return result;
        }

        public Action<TValue, object> GetSetObjectValue<TValue>(string FieldName, Type fieldtype)
        {
            ParameterExpression arg = Expression.Parameter(typeof(TValue));

            Expression expr = Expression.PropertyOrField(arg, FieldName);

            var objectpara = Expression.Parameter(typeof(object));

            var righttype = Expression.Convert(objectpara, fieldtype);

            var valueExp = Expression.Parameter(fieldtype);

            return Expression.Lambda<Action<TValue, object>>(Expression.Assign(expr, righttype), arg, objectpara).Compile();
        }


    }

    public class SetterInfo<T>
    {
        public Type FieldType;
        public string Name;
        public Action<T, object> Action;
    }

    public record SqlFieldInfo
    {
        public string Name;
        public Type Type;
        public object Value;
    }

    public class EuqalField : IEqualityComparer<SqlFieldInfo>
    {
        public bool Equals(SqlFieldInfo? x, SqlFieldInfo? y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode([DisallowNull] SqlFieldInfo obj)
        {
            return ComputeInt(obj.Name);
        }

        public int ComputeInt(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                s = " ";
            }
            s = s.ToLower();
            var chars = s.ToCharArray();
            var lastCharInd = chars.Length - 1;
            var num1 = 0x15051505;
            var num2 = num1;
            var ind = 0;
            while (ind <= lastCharInd)
            {
                var ch = chars[ind];
                var nextCh = ++ind > lastCharInd ? '\0' : chars[ind];
                num1 = (((num1 << 5) + num1) + (num1 >> 0x1b)) ^ (nextCh << 16 | ch);
                if (++ind > lastCharInd) break;
                ch = chars[ind];
                nextCh = ++ind > lastCharInd ? '\0' : chars[ind++];
                num2 = (((num2 << 5) + num2) + (num2 >> 0x1b)) ^ (nextCh << 16 | ch);
            }
            return num1 + num2 * 0x5d588b65;
        }

    }


    public class SqlWhere<TValue>
    {
        private string OrderByFieldName { get; set; }

        private bool OrderByASC { get; set; }

        private int skip;

        //public Filter<TValue> filter;

        public List<WhereTerm> WhereConditions { get; set; } = new List<WhereTerm>();

        private SqlLiteHelper sqlitehelper { get; set; }

        private string TableName { get; set; }

        public SqlWhere(SqlLiteHelper SqlHelper, string tableName)
        {
            this.sqlitehelper = SqlHelper;
            this.TableName = tableName;
        }

        public SqlWhere<TValue> Where(Expression<Predicate<TValue>> predicate)
        {
            EnsureOperator();
            LambdaExpression lambda = predicate as LambdaExpression;
            ParseLambda(lambda);
            return this;
        }

        public SqlWhere<TValue> WhereIn<T>(string FieldOrPropertyName, IEnumerable<T> Values)
        {
            if (!Values.Any())
            {
                RemoveTailOperator();
                return this;
            }

            EnsureOperator();

            var ranges = Values
                .Where(it => it != null)
                .Select(item => sqlitehelper.ObjToString(item));

            WhereConditions.Add(new WhereTerm
            {
                Sql = $"[{FieldOrPropertyName}] IN ({string.Join(", ", ranges)})",
                Type = WhereTerm.TermType.ConditionText
            });

            return this;
        }

        public SqlWhere<TValue> WhereNotIn<T>(string FieldOrPropertyName, IEnumerable<T> Values)
        {
            if (!Values.Any())
            {
                RemoveTailOperator();
                return this;
            }
            EnsureOperator();

            var ranges = Values
                .Where(it => it != null)
                .Select(item => sqlitehelper.ObjToString(item));

            WhereConditions.Add(new WhereTerm
            {
                Sql = $"[{FieldOrPropertyName}] NOT IN ({string.Join(", ", ranges)})",
                Type = WhereTerm.TermType.ConditionText
            });

            return this;
        }

        private void EnsureOperator()
        {
            if (!this.WhereConditions.Any())
            {
                return;
            }

            var last = this.WhereConditions.Last();
            if (last.Type == WhereTerm.TermType.AND || last.Type == WhereTerm.TermType.OR || last.Type == WhereTerm.TermType.NOT)
            {
                return;
            }

            this.WhereConditions.Add(new WhereTerm() { Type = WhereTerm.TermType.AND });
        }

        private void RemoveTailOperator()
        {
            var last = WhereConditions.LastOrDefault();
            if (last == null)
            {
                return;
            }

            if (last.Type == WhereTerm.TermType.AND || last.Type == WhereTerm.TermType.OR || last.Type == WhereTerm.TermType.NOT)
            {
                WhereConditions.Remove(last);
            }
        }


        public SqlWhere<TValue> AddOperator(OperatorType type)
        {
            if (type == OperatorType.AND)
            {
                this.WhereConditions.Add(new WhereTerm() { Type = WhereTerm.TermType.AND });
            }
            else if (type == OperatorType.OR)
            {
                this.WhereConditions.Add(new WhereTerm() { Type = WhereTerm.TermType.OR });
            }
            else if (type == OperatorType.NOT)
            {
                this.WhereConditions.Add(new WhereTerm() { Type = WhereTerm.TermType.NOT });
            }
            return this;
        }

        public enum OperatorType
        {
            AND = 0,
            OR = 1,
            NOT = 2
        }


        public SqlWhere<TValue> OrderByAscending(string FieldOrPropertyName)
        {
            this.OrderByFieldName = FieldOrPropertyName;
            this.OrderByASC = true;
            return this;
        }

        public SqlWhere<TValue> OrderByAscending(Expression<Func<TValue, object>> expression)
        {
            string fieldname = Kooboo.IndexedDB.Helper.ExpressionHelper.GetFieldName<TValue>(expression);
            if (!string.IsNullOrEmpty(fieldname))
            {
                return OrderByAscending(fieldname);
            }
            return this;
        }

        /// <summary>
        /// Order by descending on a field or property. This field should have an index on it. 
        /// </summary>
        public SqlWhere<TValue> OrderByDescending(string FieldOrPropertyName)
        {
            this.OrderByFieldName = FieldOrPropertyName;
            this.OrderByASC = false;
            return this;
        }

        public SqlWhere<TValue> OrderByDescending(Expression<Func<TValue, object>> expression)
        {
            string fieldname = Kooboo.IndexedDB.Helper.ExpressionHelper.GetFieldName<TValue>(expression);

            if (!string.IsNullOrEmpty(fieldname))
            {
                return OrderByDescending(fieldname);
            }

            return this;
        }

        public string GenerateQuerySql()
        {
            var sql = $"SELECT * FROM [{TableName}]{GenerateWhere()}";

            if (!string.IsNullOrWhiteSpace(this.OrderByFieldName))
            {
                sql += " Order By " + this.OrderByFieldName;
                if (!this.OrderByASC)
                {
                    sql += " DESC";
                }
            }

            return sql;
        }

        private string GenerateCountSql()
        {
            var sql = "SELECT COUNT(*) FROM " + this.TableName;
            sql += GenerateWhere();
            return sql;
        }

        private string GenerateWhere()
        {
            if (this.WhereConditions.Any())
            {
                string sql = " WHERE ";

                foreach (var item in this.WhereConditions)
                {
                    if (item.Type == WhereTerm.TermType.StartBracket)
                    {
                        sql += "(";
                    }
                    else if (item.Type == WhereTerm.TermType.EndBracket)
                    {
                        sql += ")";
                    }
                    else if (item.Type == WhereTerm.TermType.AND)
                    {
                        sql += " AND ";
                    }
                    else if (item.Type == WhereTerm.TermType.OR)
                    {
                        sql += " OR ";
                    }
                    else if (item.Type == WhereTerm.TermType.NOT)
                    {
                        sql += " NOT ";
                    }
                    else if (item.Type == WhereTerm.TermType.ConditionText)
                    {
                        sql += item.Sql;
                    }
                }
                return sql;
            }

            return null;
        }


        public SqlWhere<TValue> Skip(int count)
        {
            this.skip = count;
            return this;
        }

        public TValue FirstOrDefault()
        {
            var items = Take(1);
            if (items != null && items.Any())
            {
                return items.First();
            }
            return default(TValue);
        }


        public List<TValue> SelectAll()
        {
            return Take(99999).ToList();
        }


        public int Count()
        {
            var sql = this.GenerateCountSql();

            return this.sqlitehelper.ExecuteScalar<int>(sql);
        }

        public IEnumerable<TValue> Take(int count)
        {
            var sql = GenerateQuerySql();
            sql += " Limit " + this.skip.ToString() + ", " + count.ToString();
            return this.sqlitehelper.Query<TValue>(sql);
        }


        public bool Exists()
        {
            var item = Take(1);
            return item != null && item.Any();
        }

        private void ParseLambda(LambdaExpression lambda)
        {
            ParseExpression(lambda.Body);
        }

        private void ParseAndAlso(System.Linq.Expressions.Expression expression)
        {
            BinaryExpression binary = expression as BinaryExpression;

            ParseExpression(binary.Left);

            this.WhereConditions.Add(new WhereTerm() { Type = WhereTerm.TermType.AND });

            ParseExpression(binary.Right);
        }

        private void ParseOrElse(System.Linq.Expressions.Expression expression)
        {
            BinaryExpression binary = expression as BinaryExpression;

            this.WhereConditions.Add(new WhereTerm() { Type = WhereTerm.TermType.StartBracket });

            ParseExpression(binary.Left);

            this.WhereConditions.Add(new WhereTerm() { Type = WhereTerm.TermType.EndBracket });

            this.WhereConditions.Add(new WhereTerm() { Type = WhereTerm.TermType.OR });


            this.WhereConditions.Add(new WhereTerm() { Type = WhereTerm.TermType.StartBracket });
            ParseExpression(binary.Right);
            this.WhereConditions.Add(new WhereTerm() { Type = WhereTerm.TermType.EndBracket });
        }

        private void ParseExpression(System.Linq.Expressions.Expression expression)
        {
            if (expression.NodeType == ExpressionType.Equal)
            {
                ParseComparer(expression, Comparer.EqualTo);
            }
            else if (expression.NodeType == ExpressionType.GreaterThan)
            {
                ParseComparer(expression, Comparer.GreaterThan);
            }
            else if (expression.NodeType == ExpressionType.NotEqual)
            {
                ParseComparer(expression, Comparer.NotEqualTo);
            }
            else if (expression.NodeType == ExpressionType.LessThan)
            {
                ParseComparer(expression, Comparer.LessThan);
            }
            else if (expression.NodeType == ExpressionType.LessThanOrEqual)
            {
                ParseComparer(expression, Comparer.LessThanOrEqual);
            }
            else if (expression.NodeType == ExpressionType.GreaterThanOrEqual)
            {
                ParseComparer(expression, Comparer.GreaterThanOrEqual);
            }
            else if (expression.NodeType == ExpressionType.AndAlso || expression.NodeType == ExpressionType.And)
            {
                ParseAndAlso(expression);
            }
            else if (expression.NodeType == ExpressionType.OrElse || expression.NodeType == ExpressionType.Or)
            {
                ParseOrElse(expression);
            }
            else if (expression.NodeType == ExpressionType.MemberAccess)
            {
                ParseMemberAccess(expression);
            }
            else if (expression.NodeType == ExpressionType.Not)
            {
                ParseNotExpression(expression);
            }
            else if (expression.NodeType == ExpressionType.Call)
            {
                ParseCallExpresion(expression);
            }
            else
            {
                var msg = "expression not supported " + expression.NodeType.ToString();
                throw new Exception(msg);
            }
        }

        private void ParseMemberAccess(System.Linq.Expressions.Expression expression)
        {
            MemberExpression member = expression as MemberExpression;
            string name = member.Member.Name;

            var sql = name + "=" + this.sqlitehelper.ObjToString(true);
            this.WhereConditions.Add(new WhereTerm() { Sql = sql, Type = WhereTerm.TermType.ConditionText });
        }

        private void ParseCallExpresion(System.Linq.Expressions.Expression expression)
        {
            //throw new NotImplementedException();
            MethodCallExpression call = expression as MethodCallExpression;
            var methodName = call.Method.Name;

            if (methodName == "Contains" || methodName == "StartsWith" || methodName == "EndsWith")
            {
                var caller = call.Object;
                string callname = null;

                if (caller.NodeType == ExpressionType.MemberAccess)
                {
                    MemberExpression member = caller as MemberExpression;
                    callname = member.Member.Name;
                }

                var arg = call.Arguments.First();
                var exvalue = GetExpressionValue(arg);

                string sql = null;

                if (exvalue != null && callname != null)
                {
                    var strValue = exvalue.ToString().Replace("'", "''");

                    if (methodName == "Contains")
                    {
                        sql = callname + " like '%" + strValue + "%'";
                    }
                    else if (methodName == "StartsWith")
                    {
                        sql = callname + " like '" + strValue + "%'";
                    }
                    else if (methodName == "EndsWith")
                    {
                        sql = callname + "  like '%" + strValue + "'";
                    }

                }

                if (sql != null)
                {
                    this.WhereConditions.Add(new WhereTerm() { Sql = sql, Type = WhereTerm.TermType.ConditionText });
                }

            }
            else
            {
                throw new Exception("only Contains, StartsWith and EndsWith are supported");
            }

            ///this.filter.MethodCall(call);
        }

        private void ParseNotExpression(System.Linq.Expressions.Expression expression)
        {
            UnaryExpression unary = expression as UnaryExpression;
            if (unary.Operand.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression member = unary.Operand as MemberExpression;
                string name = member.Member.Name;

                var sql = name + "!=" + this.sqlitehelper.ObjToString(true);

                this.WhereConditions.Add(new WhereTerm() { Sql = sql, Type = WhereTerm.TermType.ConditionText });

            }
            else
            {
                throw new Exception("more complicate condition is not supported yet");
            }
        }

        private void ParseComparer(System.Linq.Expressions.Expression expression, Comparer compare)
        {
            BinaryExpression binary = expression as BinaryExpression;
            string name = string.Empty;
            if (binary.Left.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression member = binary.Left as MemberExpression;
                name = member.Member.Name;
            }
            else if (binary.Left.NodeType == ExpressionType.Convert)
            {
                UnaryExpression unary = binary.Left as UnaryExpression;
                MemberExpression member = unary.Operand as MemberExpression;
                name = member.Member.Name;
            }

            object constatvalue;
            if (binary.Right.NodeType == ExpressionType.Constant)
            {
                ConstantExpression value = binary.Right as ConstantExpression;
                constatvalue = value.Value;
            }
            else if (binary.Right.NodeType == ExpressionType.MemberAccess)
            {
                constatvalue = System.Linq.Expressions.Expression
                    .Lambda<Func<object>>(System.Linq.Expressions.Expression.Convert(binary.Right, typeof(object)))
                    .Compile().Invoke();
            }
            else if (binary.Right.NodeType == ExpressionType.Convert)
            {
                UnaryExpression unary = binary.Right as UnaryExpression;
                MemberExpression member = unary.Operand as MemberExpression;

                constatvalue = System.Linq.Expressions.Expression
                    .Lambda<Func<object>>(System.Linq.Expressions.Expression.Convert(member, typeof(object))).Compile()
                    .Invoke();
            }
            else
            {
                throw new Exception("operation not supported yet, please report " + binary.Right.NodeType.ToString());
            }

            string sql = string.Empty;

            if (compare == Comparer.EqualTo)
            {
                sql = name + " = " + this.sqlitehelper.ObjToString(constatvalue);
            }
            else if (compare == Comparer.GreaterThan)
            {
                sql = name + " > " + this.sqlitehelper.ObjToString(constatvalue);
            }
            else if (compare == Comparer.GreaterThanOrEqual)
            {
                sql = name + " >= " + this.sqlitehelper.ObjToString(constatvalue);
            }
            else if (compare == Comparer.LessThan)
            {
                sql = name + " < " + this.sqlitehelper.ObjToString(constatvalue);
            }
            else if (compare == Comparer.LessThanOrEqual)
            {
                sql = name + " <= " + this.sqlitehelper.ObjToString(constatvalue);
            }
            else if (compare == Comparer.NotEqualTo)
            {
                sql = name + " != " + this.sqlitehelper.ObjToString(constatvalue);
            }
            else if (compare == Comparer.Contains)
            {
                sql = name + " like '%" + constatvalue.ToString() + "%'";
            }
            else if (compare == Comparer.StartWith)
            {
                sql = name + " like  '" + constatvalue.ToString() + "%'";
            }

            this.WhereConditions.Add(new WhereTerm() { Sql = sql, Type = WhereTerm.TermType.ConditionText });
        }

        private object GetExpressionValue(System.Linq.Expressions.Expression member)
        {
            var objectMember = System.Linq.Expressions.Expression.Convert(member, typeof(object));
            var getterLambda = System.Linq.Expressions.Expression.Lambda<Func<object>>(objectMember);
            var getter = getterLambda.Compile();
            return getter();
        }

        private object GetValue(MemberExpression member)
        {
            var objectMember = System.Linq.Expressions.Expression.Convert(member, typeof(object));

            var getterLambda = System.Linq.Expressions.Expression.Lambda<Func<object>>(objectMember);

            var getter = getterLambda.Compile();

            return getter();
        }

        public record WhereTerm
        {
            public string Sql { get; set; }

            public TermType Type { get; set; }

            public enum TermType
            {
                AND = 0,
                OR = 1,
                NOT = 2,
                ConditionText = 3,  // sql condition text. 
                StartBracket = 4,
                EndBracket = 5,

            }
        }
    }
}
