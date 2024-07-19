//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Web.ViewModel.Ecommerce
{
    public class CategoryViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Values { get; set; }

        private List<CategoryViewModel> _subcats;

        public List<CategoryViewModel> SubCats
        {
            get
            {
                if (_subcats == null)
                {
                    _subcats = new List<CategoryViewModel>();
                }
                return _subcats;
            }
            set
            {
                _subcats = value;
            }

        }

    }


}
