//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Linq;

namespace Kooboo.Sites.Models
{
    /// <summary>
    /// When creating a folder, must specify the full path starts with /. 
    /// When generating folder collection, it must take the consideration of Folder and Routing. 
    /// </summary> 
    public class Folder : CoreObject
    {
        /// <summary>
        /// This constructor is required for the Serializer tool 
        /// </summary>
        public Folder()
        {
            this.ConstType = ConstObjectType.Folder;
        }

        public Folder(byte ObjectConstType)
        {
            this.ObjectConstType = ObjectConstType;
            this.ConstType = ConstObjectType.Folder;
        }

        private Guid _id; 
        public override Guid Id
        {
            get
            {
                if (_id == default(Guid))
                { 
                    _id = Data.IDGenerator.GetFolderId(Name, this.ObjectConstType);
                }
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        public byte ObjectConstType { get; set; }

        private string _fullpath;

        /// <summary>
        /// The full folder path in the format of /folder/subfolder
        /// </summary>
        public string FullPath
        {
            get
            {
                return _fullpath; 
            }
            set
            {
               if (!string.IsNullOrEmpty(value))
                {
                    _fullpath = value.Replace("\\", "/");  
                    if (_fullpath.EndsWith("/"))
                    {
                        _fullpath = _fullpath.TrimEnd('/'); 
                    }
                    _segment = null; 
                }
            }
        }

        private string _segment;
        public string Segment
        {
            get
            {
                if (string.IsNullOrEmpty(_segment))
                {
                    if (!string.IsNullOrEmpty(FullPath))
                    {
                        string[] segments = FullPath.Split('/');
                        foreach (var item in segments.Reverse())
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                _segment = item;
                                break;
                            }
                        }

                    }
                }
                return _segment;
            }
            set
            {
                _segment = value;
            }
        }
         
        public override string Name
        {
            get
            {
                return this.FullPath;
            }
        }

        public override int GetHashCode()
        {
            string unique = this.ConstType.ToString() + this.FullPath;
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique); 
        }
    }
}
