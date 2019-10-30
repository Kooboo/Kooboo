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

        public override bool AddOrUpdate(T value, Guid userId = default(Guid))
        {
            return AddOrUpdate(value, false, false, userId);
        }

        public bool AddOrUpdate(T value, bool updateSource = false, bool updateSameEmbedded = false, Guid userId = default(Guid))
        {
            lock (_locker)
            {
                var old = Get(value.Id);
                if (!value.IsEmbedded)
                {
                    this.Store.CurrentUserId = userId;
                }

                if (old == null)
                {
                    if (value.IsEmbedded && updateSource)
                    {
                        this.UpdateEmbedded(value, value.Body);
                    }
                    else
                    {
                        RaiseBeforeEvent(value, ChangeType.Add);
                        this.Store.add(value.Id, value, !value.IsEmbedded);
                        RaiseEvent(value, ChangeType.Add);
                    }
                    return true;
                }
                else
                {
                    if (!IsEqual(old, value))
                    {
                        RaiseBeforeEvent(value, ChangeType.Update, old);

                        if (!(value.IsEmbedded && updateSource))
                        {
                            value.LastModified = DateTime.UtcNow;
                            Store.update(value.Id, value, !value.IsEmbedded);
                        }

                        if (value.IsEmbedded)
                        {
                            if (updateSource)
                            {
                                this.UpdateEmbedded(value, value.Body);
                            }

                            if (updateSameEmbedded)
                            {
                                foreach (var item in GetSameEmbedded(old.BodyHash))
                                {
                                    if (item.Id != value.Id)
                                    {
                                        item.Body = value.Body;
                                        AddOrUpdate(item, updateSource, false);
                                    }
                                }
                            }
                        }

                        RaiseEvent(value, ChangeType.Update, old);

                        return true;
                    }
                }
                return false;
            }
        }

        public void Delete(Guid id, bool updateSource = true, bool updateSameEmbedded = false, Guid userId = default(Guid))
        {
            var value = Get(id);
            Delete(value, updateSource, updateSameEmbedded, userId);
        }

        public override void Delete(Guid id, Guid userId = default(Guid))
        {
            Delete(id, true, false, userId);
        }

        private void Delete(T value, bool updateSource = true, bool updateSameEmbedded = false, Guid userId = default(Guid))
        {
            if (value == null)
            { return; }

            if (updateSource)
            {
                Clean(value);
            }
            RaiseBeforeEvent(value, ChangeType.Delete, default(T));
            this.Store.delete(value.Id, !value.IsEmbedded);

            if (updateSameEmbedded)
            {
                foreach (var item in GetSameEmbedded(value.BodyHash))
                {
                    if (item.Id != value.Id)
                    {
                        item.Body = value.Body;
                        Delete(item.Id, updateSource, false);
                    }
                }
            }
            RaiseEvent(value, ChangeType.Delete, default(T));
        }

        public override T GetByNameOrId(string nameOrGuid)
        {
            bool parseok = Guid.TryParse(nameOrGuid, out var key);

            return parseok ? Get(key) : this.Query.Where(o => o.Name == nameOrGuid).FirstOrDefault();
        }

        public void RemoveEmbedded(T value)
        {
            if (value != null && value.IsEmbedded)
            {
                var modeltype = Service.ConstTypeService.GetModelType(value.OwnerConstType);

                var repo = this.SiteDb.GetRepository(modeltype);

                var parentobject = repo?.Get(value.OwnerObjectId);

                if (parentobject != null && parentobject is IDomObject domobject)
                {
                    var element = Relation.DomRelation.GetEmbeddedByItemIndex(domobject.Dom, value.ItemIndex, value.DomTagName);

                    if (element != null)
                    {
                        var newhtml = domobject.Dom.HtmlSource.Substring(0, element.location.openTokenStartIndex);
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
                string objecturl = route?.Name;

                if (objecturl == null)
                {
                    return; // not url, should not be possible.
                }

                var relations = this.SiteDb.Relations.GetReferredBy(value as SiteObject);

                foreach (var item in relations)
                {
                    var repo = this.SiteDb.GetRepository(item.ConstTypeX);

                    var parentobject = repo?.Get(item.objectXId);

                    if (parentobject != null && parentobject is IDomObject domobject)
                    {
                        if (domobject is Page page)
                        {
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

                                    if (Lib.Helper.StringHelper.IsSameValue(itemurl, objecturl))
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

        public void UpdateEmbedded(T value, string body)
        {
            if (value == null)
            {
                return;
            }
            if (value.IsEmbedded)
            {
                var modeltype = Service.ConstTypeService.GetModelType(value.OwnerConstType);

                var repo = this.SiteDb.GetRepository(modeltype);

                var parentobject = repo?.Get(value.OwnerObjectId);

                if (parentobject != null && parentobject is IDomObject domobject)
                {
                    var element = Relation.DomRelation.GetEmbeddedByItemIndex(domobject.Dom, value.ItemIndex, value.DomTagName);

                    if (element != null)
                    {
                        var newhtml = domobject.Dom.HtmlSource.Substring(0, element.location.openTokenEndIndex + 1);
                        newhtml += body;
                        newhtml += domobject.Dom.HtmlSource.Substring(element.location.endTokenStartIndex);

                        domobject.Body = newhtml;
                    }
                    else
                    {
                        if (value.ConstType == ConstObjectType.Style)
                        {
                            body = "<style>" + body + "</style>";
                        }
                        else if (value.ConstType == ConstObjectType.Script)
                        {
                            body = "<script>" + body + "</script>";
                        }
                        else if (value.ConstType == ConstObjectType.Form)
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
                            domobject.Body += body;
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

        public List<T> GetEmbeddeds(bool distinct = true)
        {
            var list = Query.Where(o => o.OwnerObjectId != default(Guid)).SelectAll();

            if (distinct)
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

        public int CountSameEmbedded(int bodyHash)
        {
            return Query.Where(o => o.BodyHash == bodyHash && o.OwnerObjectId != default(Guid)).Count();
        }

        public List<T> GetSameEmbedded(int bodyHash)
        {
            return Query.Where(o => o.BodyHash == bodyHash && o.OwnerObjectId != default(Guid)).SelectAll();
        }

        public List<T> GetByOwnerId(Guid ownerId, byte ownerConstType)
        {
            return Query.Where(o => o.OwnerObjectId == ownerId && o.OwnerConstType == ownerConstType).SelectAll();
        }

        public T Upload(byte[] contentBytes, string fullName, Guid userId)
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
                this.AddOrUpdate(old, userId);
                return old;
            }
            else
            {
                T newvalue = (T)Activator.CreateInstance(typeof(T));
                newvalue.Name = UrlHelper.FileName(fullName);
                newvalue.Body = body;
                SiteDb.Routes.AddOrUpdate(relativeUrl, newvalue as SiteObject, userId);
                this.AddOrUpdate(newvalue, userId);
                return newvalue;
            }
        }

        List<object> IEmbeddableRepository.GetSameEmbedded(int bodyHash)
        {
            var result = this.GetSameEmbedded(bodyHash);
            if (result != null && result.Any())
            {
                return result.ToList<object>();
            }
            return new List<object>();
        }

        public override List<UsedByRelation> GetUsedBy(Guid objectId)
        {
            var value = Get(objectId);

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
                    UsedByRelation relation = new UsedByRelation
                    {
                        ObjectId = item.OwnerObjectId, ConstType = item.OwnerConstType
                    };
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
        List<Object> GetSameEmbedded(int bodyHash);
    }
}