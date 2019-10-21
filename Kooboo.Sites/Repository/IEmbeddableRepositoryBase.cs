//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.Dom;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Models;
using Kooboo.Sites.Service;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Kooboo.Sites.Repository
{
    public class IEmbeddableRepositoryBase<T> : SiteRepositoryBase<T>, IEmbeddableRepository where T : class, IEmbeddable
    {
        private object _locker = new object();

        public override bool AddOrUpdate(T Value, Guid UserId = default(Guid))
        {
            return AddOrUpdate(Value, false, false, UserId);
        }

        public bool AddOrUpdate(T Value, bool updateSource = false, bool UpdateSameEmbedded = false, Guid UserId = default(Guid))
        {
            lock (_locker)
            {
                var old = Get(Value.Id);
                if (!Value.IsEmbedded)
                {
                    this.Store.CurrentUserId = UserId;
                }

                if (old == null)
                {
                    if (Value.IsEmbedded && updateSource)
                    {
                        this.UpdateEmbedded(Value, Value.Body);
                    }
                    else
                    {
                        RaiseBeforeEvent(Value, ChangeType.Add);
                        this.Store.add(Value.Id, Value, !Value.IsEmbedded);
                        RaiseEvent(Value, ChangeType.Add);
                    }
                    return true;
                }
                else
                {
                    if (!IsEqual(old, Value))
                    {
                        RaiseBeforeEvent(Value, ChangeType.Update, old);

                        if (!(Value.IsEmbedded && updateSource))
                        {
                            Value.LastModified = DateTime.UtcNow;
                            Store.update(Value.Id, Value, !Value.IsEmbedded);
                        }

                        if (Value.IsEmbedded)
                        {
                            if (updateSource)
                            {
                                this.UpdateEmbedded(Value, Value.Body);
                            }

                            if (UpdateSameEmbedded)
                            {
                                foreach (var item in GetSameEmbedded(old.BodyHash))
                                {
                                    if (item.Id != Value.Id)
                                    {
                                        item.Body = Value.Body;
                                        AddOrUpdate(item, updateSource, false);
                                    }
                                }
                            }
                        }

                        RaiseEvent(Value, ChangeType.Update, old);

                        return true;
                    }
                }
                return false;
            }

        }

        public void Delete(Guid id, bool UpdateSource = true, bool UpdateSameEmbedded = false, Guid UserId = default(Guid))
        {
            var Value = Get(id);
            Delete(Value, UpdateSource, UpdateSameEmbedded, UserId);
        }

        public override void Delete(Guid id, Guid UserId = default(Guid))
        {
            Delete(id, true, false, UserId);
        }

        private void Delete(T Value, bool UpdateSource = true, bool UpdateSameEmbedded = false, Guid UserId = default(Guid))
        {
            if (Value == null)
            { return; }

            if (UpdateSource)
            {
                Clean(Value);  
            }
            RaiseBeforeEvent(Value, ChangeType.Delete, default(T));
            this.Store.delete(Value.Id, !Value.IsEmbedded);

            if (UpdateSameEmbedded)
            {
                foreach (var item in GetSameEmbedded(Value.BodyHash))
                {
                    if (item.Id != Value.Id)
                    {
                        item.Body = Value.Body;
                        Delete(item.Id, UpdateSource, false);
                    }
                }
            }
            RaiseEvent(Value, ChangeType.Delete, default(T));
        }

        public override T GetByNameOrId(string NameOrGuid)
        {
            Guid key;
            bool parseok = Guid.TryParse(NameOrGuid, out key);

            if (parseok)
            {
                return Get(key);
            }
            else
            {
                return this.Query.Where(o => o.Name == NameOrGuid).FirstOrDefault();
            }
        }

        public void RemoveEmbedded(T value)
        {
            if (value != null && value.IsEmbedded)
            {
                var modeltype = Service.ConstTypeService.GetModelType(value.OwnerConstType);

                var repo = this.SiteDb.GetRepository(modeltype);

                var parentobject = repo?.Get(value.OwnerObjectId);

                if (parentobject != null && parentobject is IDomObject)
                {
                    var domobject = parentobject as IDomObject;

                    string newhtml = string.Empty;

                    var element = Relation.DomRelation.GetEmbeddedByItemIndex(domobject.Dom, value.ItemIndex, value.DomTagName);

                    if (element != null)
                    {
                        newhtml = domobject.Dom.HtmlSource.Substring(0, element.location.openTokenStartIndex);
                        newhtml += domobject.Dom.HtmlSource.Substring(element.location.endTokenEndIndex + 1);

                        domobject.Body = newhtml;
                        repo.AddOrUpdate(domobject);
                    }

                }
            }
        }
        
        public void RemoveExternalStyleScript(T value)
        {
            if (value != null && !value.IsEmbedded)
            { 
                var route = this.SiteDb.Routes.GetByObjectId(value.Id);
                string objecturl = route != null ? route.Name : null;

                if (objecturl == null)
                {
                    return; // not url, should not be possible. 
                }

                var relations = this.SiteDb.Relations.GetReferredBy(value as SiteObject);

                foreach (var item in relations)
                {
                    var repo = this.SiteDb.GetRepository(item.ConstTypeX);

                    var parentobject = repo?.Get(item.objectXId);

                    if (parentobject != null && parentobject is IDomObject)
                    {
                        var domobject = parentobject as IDomObject;

                        if (domobject is Page)
                        {
                            var page = domobject as Page;
                            page.Headers.Styles.Remove(objecturl);
                            page.Headers.Scripts.Remove(objecturl);
                        }

                        List<SourceUpdate> updates = new List<SourceUpdate>();

                        if (value.ConstType == ConstObjectType.Style)
                        { 
                            HTMLCollection styletags = domobject.Dom.getElementsByTagName("link");

                            foreach (var cssitem in styletags.item)
                            {
                                if (cssitem.hasAttribute("rel") && cssitem.getAttribute("rel").ToLower().Contains("stylesheet"))
                                {
                                    string itemurl = DomUrlService.GetLinkOrSrc(cssitem);

                                    if (Lib.Helper.StringHelper.IsSameValue(itemurl,objecturl))
                                    {
                                        updates.Add(new SourceUpdate() { StartIndex = cssitem.location.openTokenStartIndex, EndIndex = cssitem.location.endTokenEndIndex }); 
                                    } 
                                }
                            }  
                        }
                        else if (value.ConstType == ConstObjectType.Script)
                        {

                            HTMLCollection scripttags = domobject.Dom.getElementsByTagName("script");

                            foreach (var jsitem in scripttags.item)
                            {
                                if (jsitem.hasAttribute("src"))
                                {
                                    string itemurl = jsitem.getAttribute("src");

                                    if (Lib.Helper.StringHelper.IsSameValue(itemurl, objecturl))
                                    {
                                        updates.Add(new SourceUpdate() { StartIndex = jsitem.location.openTokenStartIndex, EndIndex = jsitem.location.endTokenEndIndex });
                                    } 
                                }
                            }  
                        }

                        // Form is handlbed by component... 

                        domobject.Body = Kooboo.Sites.Service.DomService.UpdateSource(domobject.Body, updates);

                        repo.AddOrUpdate(domobject); 
                    } 

                }


            }
        }

        public void Clean(T value)
        {
            if (value.IsEmbedded)
            {
                RemoveEmbedded(value); 
            }
           else
            {
                RemoveExternalStyleScript(value); 
            }
        } 

        public void UpdateEmbedded(T Value, string body)
        {
            if (Value == null)
            {
                return;
            }
            if (Value.IsEmbedded)
            {
                var modeltype = Service.ConstTypeService.GetModelType(Value.OwnerConstType);

                var repo = this.SiteDb.GetRepository(modeltype);

                var parentobject = repo?.Get(Value.OwnerObjectId);

                if (parentobject != null && parentobject is IDomObject)
                {
                    var domobject = parentobject as IDomObject;

                    string newhtml = string.Empty;

                    var element = Relation.DomRelation.GetEmbeddedByItemIndex(domobject.Dom, Value.ItemIndex, Value.DomTagName);

                    if (element != null)
                    {
                        newhtml = domobject.Dom.HtmlSource.Substring(0, element.location.openTokenEndIndex + 1);
                        newhtml += body;
                        newhtml += domobject.Dom.HtmlSource.Substring(element.location.endTokenStartIndex);

                        domobject.Body = newhtml;

                    }
                    else
                    {
                        if (Value.ConstType == ConstObjectType.Style)
                        {
                            body = "<style>" + body + "</style>";
                        }
                        else if (Value.ConstType == ConstObjectType.Script)
                        {
                            body = "<script>" + body + "</script>";
                        }
                        else if (Value.ConstType == ConstObjectType.Form)
                        {
                            body = "<form>" + body + "</form>";
                        }

                        int bodylocation = domobject.Dom.body.location.endTokenStartIndex;
                        if (bodylocation < 0)
                        {
                            bodylocation = domobject.Dom.documentElement.location.endTokenStartIndex;
                        }
                        if (bodylocation > 0)
                        {

                            domobject.Body = domobject.Body.Substring(0, bodylocation) + body + domobject.Body.Substring(bodylocation);
                        }
                        else
                        {
                            domobject.Body = domobject.Body + body;
                        }
                    }

                    repo.AddOrUpdate(domobject);
                }
            }

        }

        public List<T> GetExternals()
        {
            return Query.Where(it => it.OwnerObjectId == default(Guid)).SelectAll();
        }

        public List<T> GetEmbeddeds(bool Distinct = true)
        {
            var list = Query.Where(o => o.OwnerObjectId != default(Guid)).SelectAll();

            if (Distinct)
            {
                List<T> result = new List<T>();

                foreach (var item in list)
                {
                    if (result.Find(o => o.BodyHash == item.BodyHash) == null)
                    {
                        result.Add(item);
                    }
                }
                return result;
            }
            else
            {
                return list;
            }


        }

        public int CountSameEmbedded(int BodyHash)
        {
            return Query.Where(o => o.BodyHash == BodyHash && o.OwnerObjectId != default(Guid)).Count();
        }

        public List<T> GetSameEmbedded(int BodyHash)
        {
            return Query.Where(o => o.BodyHash == BodyHash && o.OwnerObjectId != default(Guid)).SelectAll();
        }

        public List<T> GetByOwnerId(Guid OwnerId, byte OwnerConstType)
        {
            return Query.Where(o => o.OwnerObjectId == OwnerId && o.OwnerConstType == OwnerConstType).SelectAll();
        }
         
        public T Upload(byte[] contentBytes, string fullName, Guid UserId)
        {
            string relativeUrl = UrlHelper.RelativePath(fullName);

            if (contentBytes == null || contentBytes.Length == 0)
            { throw new Exception(Data.Language.Hardcoded.GetValue("Upload contain no data")); }

            var old = this.GetByUrl(relativeUrl);

            var encoding = Kooboo.Lib.Helper.EncodingDetector.GetEncoding(ref contentBytes);

            string body = encoding.GetString(contentBytes);

            if (string.IsNullOrEmpty(body))
            {
                return default(T);
            }

            if (old != null)
            {
                old.Body = body;
                this.AddOrUpdate(old, UserId);
                return old;
            }
            else
            {
                T newvalue = (T)Activator.CreateInstance(typeof(T));
                newvalue.Name = UrlHelper.FileName(fullName);
                newvalue.Body = body;
                SiteDb.Routes.AddOrUpdate(relativeUrl, newvalue as SiteObject, UserId);
                this.AddOrUpdate(newvalue, UserId);
                return newvalue;
            }
        }

        List<object> IEmbeddableRepository.GetSameEmbedded(int BodyHash)
        {
            var result = this.GetSameEmbedded(BodyHash);
            if (result != null && result.Count() > 0)
            {
                return result.ToList<object>();
            }
            return new List<object>();
        }

        public override List<UsedByRelation> GetUsedBy(Guid ObjectId)
        {
            var value = Get(ObjectId);

            return _GetUsedBy(value);
        }

        private List<UsedByRelation> _GetUsedBy(T value)
        {
            List<UsedByRelation> result = new List<UsedByRelation>();
            if (value == null)
            { return result; }

            if (value.IsEmbedded)
            {
                var samestyles = this.GetSameEmbedded(value.BodyHash);

                foreach (var item in samestyles)
                {
                    UsedByRelation relation = new UsedByRelation();
                    relation.ObjectId = item.OwnerObjectId;
                    relation.ConstType = item.OwnerConstType;
                    Helper.RelationHelper.SetNameUrl(SiteDb, relation);
                    relation.Remark = item.KoobooOpenTag;
                    result.Add(relation);
                }

                return result;
            }
            else
            {
                return base.GetUsedBy(value.Id);
            }
        }

    }


    public interface IEmbeddableRepository
    {
        List<Object> GetSameEmbedded(int BodyHash);
    }
}
