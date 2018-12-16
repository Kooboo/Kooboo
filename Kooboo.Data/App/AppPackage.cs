//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.App
{
    public class AppPackage:IGolbalObject
    {
        private Guid _id;
        public Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    _id = System.Guid.NewGuid();
                }
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public Int64 EditVersion { get; set; }
        public Guid SiteId { get; set; }

        public string SiteName { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public Guid OrganizationId { get; set; }

        public string ThumbNail { get; set; }

        private List<string> _images;
        public List<string> Images
        {
            get
            {
                if (_images == null)
                {
                    _images = new List<string>();
                }
                return _images;
            }
            set { _images = value; }
        }

        public string Name { get; set; }

        public string Link { get; set; }

        [Kooboo.IndexedDB.CustomAttributes.KoobooIgnore]
        public string FilePath
        {
            get
            {
                string file = this.Id.ToString().Replace("-", "") + ".zip";
                string fullfilename = System.IO.Path.Combine(Data.AppSettings.AppFolder, "package", file);
                Lib.Helper.IOHelper.EnsureFileDirectoryExists(fullfilename);
                return fullfilename;
            }
        }

        public int Size { get; set; }

        public string Description { get; set; }

        public int Score { get; set; } = 1;

        public string Tags { get; set; }

        public DateTime LastModified { get; set; }

        public int PageCount { get; set; }

        public int ContentCount { get; set; }

        public int ImageCount { get; set; }

        public int LayoutCount { get; set; }

        public int MenuCount { get; set; }

        public int ViewCount { get; set; }

        public bool IsApproved { get; set; }

        public decimal Price { get; set; } = 0;

        public string Currency { get; set; } = "CNY";

        public override int GetHashCode()
        {
            string unique = this.Description + this.Link + this.Name + this.SiteName + this.Tags + this.ThumbNail + this.UserName;
            unique += this.ContentCount.ToString() + this.ImageCount.ToString() + this.LayoutCount.ToString() + this.ViewCount.ToString() + this.PageCount.ToString() + this.MenuCount.ToString();

            unique += this.EditVersion.ToString() + this.UserId.ToString() + this.OrganizationId.ToString() + this.SiteId.ToString() + this.Score.ToString();

            foreach (var item in this.Images)
            {
                unique += item;
            }
            unique += this.IsApproved.ToString();
            unique += this.CanTrial.ToString();
            unique += this.Price.ToString() + this.Currency.ToString();

            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        }

        public Guid ZipHash { get; set; }

        public Guid BinaryHash { get; set; }

        public bool CanTrial { get; set; }
    }
}
