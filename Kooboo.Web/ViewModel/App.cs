//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Web.ViewModel
{
    public class AppViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }

        public string ThumbNail { get; set; }

        public string Category { get; set; }

        public int Size { get; set; }

        public string Description { get; set; }

        public int Score { get; set; } = 100;

        public string Tags { get; set; }

        public DateTime LastModified { get; set; }

        public long DownloadCount { get; set; }

        public int PageCount { get; set; }

        public int ContentCount { get; set; }

        public int ImageCount { get; set; }

        public int LayoutCount { get; set; }

        public int MenuCount { get; set; }

        public int ViewCount { get; set; }

        public decimal Price { get; set; }

        public CurrencyViewModel Currency { get; set; }

        public bool CanTrial { get; set; }
    }

    public class AppDetailViewModel
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public List<string> Images { get; set; } = new List<string>();

        public string Name { get; set; }

        public string Link { get; set; }

        public string Category { get; set; }

        public int Size { get; set; }

        public string Description { get; set; }

        public int Score { get; set; } = 100;

        public string Tags { get; set; }

        public DateTime LastModified { get; set; }

        public string PreviewUrl { get; set; }

        public string DownloadUrl { get; set; }

        public string DownloadCode { get; set; }

        public long DownloadCount { get; set; }

        public bool IsPrivate { get; set; }

        public int PageCount { get; set; }

        public int ContentCount { get; set; }

        public int ImageCount { get; set; }

        public int LayoutCount { get; set; }

        public int MenuCount { get; set; }

        public int ViewCount { get; set; }


        public string ThumbNail { get; set; }

        public decimal Price { get; set; }

        public CurrencyViewModel Currency { get; set; }

        public bool CanTrial { get; set; }

    }
}
