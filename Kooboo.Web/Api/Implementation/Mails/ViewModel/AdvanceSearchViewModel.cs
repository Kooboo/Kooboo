using Kooboo.Mail.ViewModel;

namespace Kooboo.Web.Api.Implementation.Mails.ViewModel
{
    public class AdvanceSearchViewModel
    {
        public int Count { get; set; }
        public List<MessageViewModel> Data { get; set; }
    }
}
