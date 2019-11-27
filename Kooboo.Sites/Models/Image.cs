//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.Extensions;
using System.Security.Cryptography;
using Kooboo.Data.Interface;

namespace Kooboo.Sites.Models
{
    [Kooboo.Attributes.Diskable(Kooboo.Attributes.DiskType.Binary)]
    [Kooboo.Attributes.Routable]
    public class Image : CoreObject, IBinaryFile, IExtensionable
    {
        public Image()
        {
            this.ConstType = ConstObjectType.Image;
        }
        private Guid _id;

        public override Guid Id
        {
            set { _id = value; }
            get
            {
                if (_id == default(Guid))
                {
                    _id = System.Guid.NewGuid();

                }
                return _id;
            }
        }

        public string Alt { get; set; }

        public string Extension { get; set; }

        public bool IsSvg
        {
            get
            {
                if (string.IsNullOrEmpty(Extension))
                {
                    return false;
                }

                if (Extension.ToLower().EndsWith(".svg") || Extension.ToLower() == "svg")
                {
                    return true;
                }
                return false;
            }

        }

        /// <summary>
        /// For some websites, they put the image as a base64 embeded into webpage. we may ignore those images. 
        /// </summary>
        /// public Guid PageId { get; set; }

        public int Height
        {
            get; set;
        }

        public int Width
        {
            get; set;
        }

        private int _size;

        /// <summary>
        /// size in  bytes. 
        /// </summary>
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

        /// <summary>
        /// the content bytes of this file. 
        /// </summary>
        [Kooboo.Attributes.SummaryIgnore]
        public byte[] ContentBytes
        {
            get; set;
        }

        [Kooboo.IndexedDB.CustomAttributes.KoobooIgnore]
        public byte[] Bytes
        {
            get
            {
                return ContentBytes;
            }
            set
            {
                ContentBytes = value;
            }
        }


        public void ResetSize()
        {
            if (this.ContentBytes != null)
            {
                var size = Lib.Utilities.CalculateUtility.GetImageSize(this.ContentBytes);
                this.Height = size.Height;
                this.Width = size.Width;
                this.Size = this.ContentBytes.Length;
            }
        }

        public override int GetHashCode()
        {
            string uniquestring = this.Extension + this.Name + this.Alt;

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
