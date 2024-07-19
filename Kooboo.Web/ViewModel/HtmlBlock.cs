//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Web.ViewModel
{
    //public  class HtmlBlockViewModel
    //  {
    //      public string Name { get; set; }

    //      public Dictionary<string, string> Values { get; set; }
    //  } 


    public class HtmlBlockItemViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        private Dictionary<string, object> _values;

        public Dictionary<string, object> Values
        {
            get
            {
                if (_values == null)
                {
                    _values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                }
                return _values;
            }
            set
            {
                _values = value;
            }
        }

        public DateTime LastModified
        {
            get; set;
        }
        public Guid KeyHash { get; set; }

        public int StoreNameHash { get; set; }

        public Dictionary<string, int> Relations { get; set; }
    }

}
