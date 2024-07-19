//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Web.ViewModel
{
    public class PageImageViewModel
    {
        public Guid? PageId { get; set; }


        public List<PageImageItemViewModel> ContentImages { get; set; }


        public List<PageImageItemViewModel> StyleImages { get; set; }
    }

    public class PageImageItemViewModel
    {

        public Guid Id { get; set; }


        public string Name { get; set; }


        public string Thumbnail { get; set; }


        public string Url { get; set; }


        public string[] Elements { get; set; }


        public int Width { get; set; }


        public int Height { get; set; }


        public string Size { get; set; }
    }
    public class FontFamilyViewModel : IEqualityComparer<FontFamilyViewModel>
    {
        public static FontFamilyViewModel Instance = new FontFamilyViewModel();


        public string Name { get; set; }

        /// <summary>
        /// 0:Not exists,
        /// 1:Normal,
        /// 2:Custom
        /// </summary>

        public int Status { get; set; }

        public bool Equals(FontFamilyViewModel x, FontFamilyViewModel y)
        {
            return x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(FontFamilyViewModel obj)
        {
            return obj.Name.GetHashCode();
        }
    }

}
