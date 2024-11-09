using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Lib.Storage
{

    public class JsonListStore<T> where T : IJsonListObject
    {
        private string _fullFileName;

        public JsonListStore(string fullFileName)
        {
            _fullFileName = fullFileName;
            Helper.IOHelper.EnsureDirectoryExists(_fullFileName);
        }

        public string Extension { get; set; } = "json";


        public List<T> All()
        {
            if (System.IO.File.Exists(this._fullFileName))
            {
                string text = System.IO.File.ReadAllText(this._fullFileName);

                if (!string.IsNullOrEmpty(text))
                {
                    var result = System.Text.Json.JsonSerializer.Deserialize<List<T>>(text);

                    return result != null ? result : new List<T>();
                }
            }
            return new List<T>();
        }


        public T Find(string key)
        {
            return this.Find(o => o.Id == key);
        }

        public T Find(Predicate<T> query)
        {
            var all = this.All();
            return all.Find(query);
        }

        public List<T> FindAll(Predicate<T> query)
        {
            var all = this.All();
            return all.FindAll(query);
        }

        public void AddOrUpdate(T Value)
        {
            var all = this.All();

            var count = all.Count();

            bool found = false; 

            for (int i = 0; i < count; i++)
            {
                if (all[i].Id == Value.Id)
                {
                    all[i] = Value;
                    found = true;
                    break; 
                }
            }

            if (!found)
            {
                all.Add(Value); 
            }

            Save(all); 
        }


        public void Delete(string Id)
        {
            var all = this.All();
            all.RemoveAll(o => o.Id == Id); 
            this.Save(all); 
        }

        public void Delete(Predicate<T> match)
        {
            var all = this.All();
            all.RemoveAll(match);
            this.Save(all); 
        }

        private void Save(List<T> Values)
        {
            string Json = System.Text.Json.JsonSerializer.Serialize(Values);

            System.IO.File.WriteAllText(this._fullFileName, Json); 
        }
 
    }



    public interface IJsonListObject
    {
        string Id { get; set; }
    }


}
