//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Extensions;
using System;
using System.Security.Cryptography;

namespace Kooboo.Sites.Models
{
   [Kooboo.Attributes.Diskable(Kooboo.Attributes.DiskType.Binary)]
   [Kooboo.Attributes.Routable]
    public class CmsFile : CoreObject, IBinaryFile,  IExtensionable
    {
        public CmsFile()
        {
            this.ConstType = ConstObjectType.CmsFile;
        }

        private Guid _id;
        public override Guid Id
        {
            set { _id = value; }
            get
            {
                if (_id == default(Guid))
                {
                    _id = Kooboo.Data.IDGenerator.GetNewGuid();
                }
                return _id;
            }
        }

        [Kooboo.Attributes.SummaryIgnore]
        public string Extension { get; set; }

        /// <summary>
        /// the content bytes of this file. 
        /// </summary>
        [Kooboo.Attributes.SummaryIgnore]
        private byte[] _contentbytes; 
        public byte[] ContentBytes {
            get {
                return _contentbytes; 
            }
            set
            {
                _contentbytes = value; 
                if (_contentbytes !=null)
                {
                    this.Size = _contentbytes.Length; 
                }
                else
                {
                    this.Size = 0; 
                } 
            }
        }
         
        /// <summary>
        ///  this is for some file like text file, etc... 
        /// </summary>
        [Obsolete]
        public string ContentString { get; set; }

        /// <summary>
        /// The content type of this file. like. application/flash. 
        /// This is often used to save original content type saved from other location. 
        /// </summary>
        public string ContentType { get; set; }

        private int _size; 
        public int Size
        {
            get
            {
                if (_size == default(int))
                { 
                    if (ContentBytes != null)
                    {
                        _size = ContentBytes.Length;
                    } 
                }
                return _size;
            }
            set
            {
                _size = value;
            }
        }

        public override int GetHashCode()
        {
            string uniquestring = this.Extension + this.Name;

            if (this.ContentBytes != null)
            {
                MD5 md5Hasher = MD5.Create();
                byte[] data = md5Hasher.ComputeHash(ContentBytes);
                uniquestring += System.Text.Encoding.ASCII.GetString(data);
            }

            return Lib.Security.Hash.ComputeIntCaseSensitive(uniquestring); 
        }
    }
}
