using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Scripting.Sqlite
{
    public class SqliteSchema
    {
        public class Item
        {
            public string Name { get; set; }

            public SqliteType Type { get; set; }
        }

        public enum SqliteType
        {
            NULL,
            INTEGER,
            REAL,
            TEXT,
            BLOB
        }

        readonly List<Item> _items;

        public IEnumerable<Item> Items => _items;

        public bool Created => _items.Count() > 0;

        public SqliteSchema(IDictionary<string, object> keyValuePairs)
        {
            _items = keyValuePairs.Select(s => new Item { Name = s.Key, Type = ConventType(s.Value?.GetType()) }).ToList();
        }

        public SqliteSchema(IEnumerable<SqliteSchema.Item> items)
        {
            _items = items.ToList();
        }

        static SqliteType ConventType(Type type)
        {
            if (type == typeof(string)) return SqliteType.TEXT;
            if (type == typeof(double)) return SqliteType.REAL;
            if (type == null) return SqliteType.NULL;
            if (type == typeof(bool)) return SqliteType.INTEGER;
            throw new NotSupportedException();
        }

        public bool Compatible(SqliteSchema schema, out List<Item> newItems)
        {
            newItems = new List<Item>();

            foreach (var item in schema._items)
            {
                var findItem = _items.FirstOrDefault(f => f.Name == item.Name);
                if (findItem == default) newItems.Add(item);
                else if (findItem.Type != item.Type) return false;
            }

            return true;
        }

        public void AddItems(IEnumerable<Item> items)
        {
            _items.AddRange(items);
        }
    }
}
