using Kooboo.Api;
using Kooboo.Data.Config;
using Kooboo.Data.ViewModel;
using Kooboo.Sites.Service;

namespace Kooboo.Web.Api.Implementation
{
    public class DataCenterApi : IApi
    {
        public string ModelName => "DataCenter";
        public bool RequireSite => false;
        public bool RequireUser => true;

        public List<DataCenterViewModel> List(ApiCall call)
        {
            return DataCenterService.List(call.Context);
        }

        public bool Enable(string name, ApiCall call)
        {
            return DataCenterService.Enable(name, call.Context);
        }
        public bool Disable(string name, ApiCall call)
        {
            return DataCenterService.Disable(name, call.Context);
        }

        public bool MakeDefault(string name, ApiCall call)
        {
            return DataCenterService.MakeDefault(name, call.Context);
        }
        
        // account:   public DataCenterViewModel CurrentDC(ApiCall call)
        public string CurrentDC(ApiCall call)
        {
            return DataCenterService.CurrentDC(call.Context);
        }
    }
}
