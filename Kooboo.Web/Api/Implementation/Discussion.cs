using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Api;
using Kooboo.Data.Models;
using Kooboo.Web.ViewModel;
using Kooboo.Data;
using Kooboo.Lib.Helper;
using Newtonsoft.Json;

namespace Kooboo.Web.Api.Implementation
{
    public class DiscussionApi:IApi
    {
        public string ModelName
        {
            get
            {
                return "Discussion";
            }
        }

        public bool RequireSite
        {
            get
            {
                return false;
            }
        }

        public bool RequireUser
        {
            get
            {
                return false;
            }
        }

        public PagedListViewModel<Discussion> List()
        {
            var url= UrlHelper.Combine(AppSettings.ThemeUrl, "/_api/DiscussionReceiver/list");
            return HttpHelper.Get<PagedListViewModel<Discussion>>(url);
        }

        public Discussion Get(string id)
        {
            var url = UrlHelper.Combine(AppSettings.ThemeUrl, "/_api/DiscussionReceiver/get");
            var dic = new Dictionary<string, string>();
            dic.Add("id", id);
            return HttpHelper.Get<Discussion>(url,dic);

        }
        public bool Add(Discussion discussion,ApiCall call)
        {
            discussion.UserId = call.Context.User.Id;
            discussion.UserName = call.Context.User.UserName;

            var url = UrlHelper.Combine(AppSettings.ThemeUrl, "/_api/DiscussionReceiver/addorUpdate");
            var json = JsonConvert.SerializeObject(discussion);
            return HttpHelper.Post<bool>(url, json);
        }

        public bool View(Guid commentId)
        {
            var url = UrlHelper.Combine(AppSettings.ThemeUrl, "/_api/DiscussionReceiver/View");
            var dic = new Dictionary<string, string>();
            dic.Add("discussionId", commentId.ToString());
            var json = JsonConvert.SerializeObject(dic);
            return HttpHelper.Post<bool>(url, json);
        }

        public PagedListViewModel<Comment> CommentList(Guid discussionId)
        {
            var url = UrlHelper.Combine(AppSettings.ThemeUrl, "/_api/CommentReceiver/List");
            var dic = new Dictionary<string, string>();
            dic.Add("discussionId", discussionId.ToString());

            return HttpHelper.Get<PagedListViewModel<Comment>>(url,dic);
        }

        public List<Comment> NestCommentList(Guid commentId)
        {
            var url = UrlHelper.Combine(AppSettings.ThemeUrl, "/_api/CommentReceiver/NestCommentList");
            var dic = new Dictionary<string, string>();
            dic.Add("commentId", commentId.ToString());

            return HttpHelper.Get<List<Comment>>(url, dic);
        }


        public bool Reply(Comment comment,ApiCall call)
        {
            comment.UserId = call.Context.User.Id;
            comment.UserName = call.Context.User.UserName;
           
            var url = UrlHelper.Combine(AppSettings.ThemeUrl, "/_api/CommentReceiver/Reply");
            var json = JsonConvert.SerializeObject(comment);
            return HttpHelper.Post<bool>(url, json);
        }

        
    }
}
