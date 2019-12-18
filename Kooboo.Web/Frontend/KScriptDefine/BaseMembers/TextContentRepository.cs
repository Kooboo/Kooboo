using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kooboo.Web.Frontend.KScriptDefine.BaseMembers
{
    public interface TextContentRepository
    {
        [Description("Add a text content into content repository. Folder is a required.")]
        void Add(object value);

        [Description("Return an array of all TextContentObjects")]
        TextContentObject[] All();

        [Description("Delete an item based on id or userkey")]
        void Delete(string id);

        [Description("Return the first TextContentObject based on search condition")]
        TextContentObject Find(string searchCondition);

        [Description("Return an array of all matched TextContentObjects")]
        TextContentObject[] FindAll(string searchCondition);

        [Description("Get a text content object based on Id or UserKey")]
        TextContentObject Get(string nameorid);

        [Description("Query the content repository with fluent API")]
        ContentQuery Query(string searchCondition);

        [Description("update a text content values")]
        TextContentObject Update(TextContentObject value);
    }
}
