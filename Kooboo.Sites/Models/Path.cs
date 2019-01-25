//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Models
{
    /// <summary>
    /// used for folder or routes path. 
    /// </summary>
    public class Path
    {
        public Path()
        {

        }


        public Path ParentPath { get; set; }

        public string segment { get; set; }

        private Dictionary<string, Path> _children;

        public Dictionary<string, Path> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new Dictionary<string, Path>();
                }
                return _children;
            }
            set
            {
                _children = value;
            }      
        }

        public Guid RouteId { get; set; }

        public Guid ObjectId { get; set; }

        public bool PartialWildCard { get; set; }

        private List<string> _PartialParts;
        public List<string> PartialParts
        {
            get
            {
                if (this.PartialWildCard && _PartialParts == null)
                {
                    _PartialParts = new List<string>();
                    if (!string.IsNullOrEmpty(this.segment))
                    {
                        string[] seperators = new string[1];
                        seperators[0] = "{}";
                        _PartialParts = this.segment.Split(seperators, StringSplitOptions.RemoveEmptyEntries).ToList();
                    }
                }
                return _PartialParts;
            }
        }

    }

    [DataContract]
    public class CrumbPath
    {
        /// <summary>
        /// The segment name.
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// The full Path from root to current. 
        /// </summary>
        [DataMember(Name = "fullPath")]
        public string FullPath { get; set; }
    }

}

