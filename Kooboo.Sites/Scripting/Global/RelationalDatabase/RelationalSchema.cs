using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.RelationalDatabase
{
    public abstract class RelationalSchema
    {
        public abstract string[] DataTypes { get; }

        public class Item
        {
            public string Name { get; set; }

            public string Type { get; set; }
        }

        readonly List<Item> _items;

        public IEnumerable<Item> Items => _items;

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

        public bool Compatible(RelationalSchema schema, out List<Item> newItems)
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
