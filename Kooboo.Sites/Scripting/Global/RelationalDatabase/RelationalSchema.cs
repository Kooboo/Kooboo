using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.RelationalDatabase
{
    public abstract class RelationalSchema
    {
        public class Item
        {
            public string Name { get; set; }

            public string Type { get; set; }

            public bool IsPrimaryKey { get; set; }
        }

        readonly List<Item> _items;

        public IEnumerable<Item> Items => _items;

        public string PrimaryKey => Items.FirstOrDefault(f => f.IsPrimaryKey)?.Name;

        public bool Created => _items.Count() > 0;

        public RelationalSchema(IDictionary<string, object> keyValuePairs)
        {
            _items = keyValuePairs.Select(s => new Item { Name = s.Key, Type = ConventType(s.Value?.GetType()) }).ToList();
        }

        public RelationalSchema(IEnumerable<Item> items)
        {
            _items = items.ToList();
        }

        internal abstract string ConventType(Type type);

        public List<Item> Compatible(RelationalSchema schema)
        {
            var newItems = new List<Item>();

            foreach (var item in schema._items)
            {
                var findItem = _items.FirstOrDefault(f => f.Name == item.Name);

                if (findItem == null)
                {
                    newItems.Add(item);
                    continue;
                }
            }

            return newItems;
        }


        public void AddItems(IEnumerable<Item> items)
        {
            _items.AddRange(items);
        }
    }
}
