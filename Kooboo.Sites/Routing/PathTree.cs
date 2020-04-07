//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using Kooboo.Extensions;
using System;
using Kooboo.Sites.Models;
using System.Linq;

namespace Kooboo.Sites.Routing
{
    /// <summary>
    /// cached routing information inside a tree for fast access. 
    /// </summary>
    public class PathTree
    {
        private object _locker = new object();

        private string wildcard;
        /// <summary>
        /// The empty root. This is only the contains without any value. 
        /// </summary>
        public Path root;

        public PathTree()
        {
            wildcard = "{}";
            root = new Path();
        }

        public void AddOrUpdate(Route route)
        {
            AddOrUpdate(route.Name, route.Id, route.objectId, route.DestinationConstType, route.Parameters);
        }

        public void AddOrUpdate(string RelativeUrl, Guid RouteKey, Guid ObjectId, byte ConstType, Dictionary<string, string> Parameters)
        {
            if (string.IsNullOrEmpty(RelativeUrl))
            {
                RelativeUrl = "/";
            }

            RelativeUrl = RelativeUrl.Replace("\\", "/").ToLower().Trim();
            string[] segments = RelativeUrl.Split('/');

            List<string> seglist = new List<string>();

            foreach (var item in segments)
            {
                if (!string.IsNullOrEmpty(item))
                {

                    seglist.Add(item.RemoveRoutingCurlyBracket());
                }
            }

            AddList(seglist, RouteKey, ObjectId, ConstType, Parameters);
        }

        public void Del(string RelativeUrl)
        {
            if (string.IsNullOrEmpty(RelativeUrl))
            {
                RelativeUrl = "/";
            }

            RelativeUrl = RelativeUrl.Replace("\\", "/").ToLower().Trim();
            string[] segments = RelativeUrl.Split('/');

            List<int> seglist = new List<int>();

            Path currentslot = root;

            foreach (var item in segments)
            {
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }

                var subpath = item.RemoveRoutingCurlyBracket();

                Path child = null;

                if (currentslot.Children.ContainsKey(subpath))
                {
                    child = currentslot.Children[subpath];
                }

                if (child == null)
                {
                    return;
                }
                else
                {
                    currentslot = child;
                }
            }

            currentslot.RouteId = default(Guid);
            RemoveEmptySlot(currentslot);
        }

        /// <summary>
        /// Find the Route key based on relative url.
        /// </summary>
        /// <param name="RelativeUrl"></param>
        /// <returns></returns>
        public Guid FindRouteId(string RelativeUrl, bool EnsureObjectId)
        {
            lock (_locker)
            {
                Path path = FindPath(RelativeUrl, EnsureObjectId);

                if (EnsureObjectId && !this.HasObject(path))
                {
                    path = FindShortestWildCardPath(path);
                } 

                if (path == null)
                {
                    int QuestionMark = RelativeUrl.IndexOf("?");
                    if (QuestionMark > 0)
                    {
                        RelativeUrl = RelativeUrl.Substring(0, QuestionMark);
                        path = FindPath(RelativeUrl, EnsureObjectId);
                        if (path == null)
                        {
                            return default(Guid);
                        }
                    }
                    else
                    {
                        return default(Guid);
                    }
                }
                 

                if (path == null)
                {
                    return default(Guid);
                }

                else
                {
                    return path.RouteId;
                }
            }
        }
         

        private void RemoveEmptySlot(Path currentPath)
        {
            if (currentPath.RouteId == default(Guid) && currentPath.ParentPath != null && (currentPath.Children == null || currentPath.Children.Count == 0))
            {
                currentPath.ParentPath.Children.Remove(currentPath.segment);
                RemoveEmptySlot(currentPath.ParentPath);
            }
        }

        private void AddList(List<string> seglist, Guid routeid, Guid ObjectId, byte ConstType, Dictionary<string, string> Parameters)
        {
            lock (_locker)
            {
                Path currentslot = root;

                foreach (string item in seglist)
                {
                    if (item == null)
                    {
                        continue; 
                    } 
                    var key = item.ToLower();  
                    Path child = currentslot.Children.Values.ToList().Find(o => o.segment.ToLower() == key);
                    if (child == null)
                    {
                        child = addPath(currentslot, item, ObjectId);
                        currentslot = child;
                    }
                    else
                    {
                        currentslot = child;
                    }
                }

                if (currentslot.ObjectId != ObjectId)
                {
                    if (ObjectId != default(Guid))
                    {
                        currentslot.ObjectId = ObjectId;
                    }
                }

                currentslot.RouteId = routeid;
            }
        }

        private Path addPath(Path parent, string currentSegment, Guid ObjectId)
        {
            Path newslot = new Path();
            newslot.segment = currentSegment;
            newslot.ParentPath = parent;
            newslot.ObjectId = ObjectId;

            if (currentSegment.Contains("{") && currentSegment.Contains("}") && currentSegment != "{}")
            {
                newslot.PartialWildCard = true;
            }

            if (parent.Children.ContainsKey(currentSegment))
            {
                var currentValue = parent.Children[currentSegment];
                if (currentValue.ObjectId == default(Guid))
                {
                    currentValue.ObjectId = ObjectId;
                }
            }
            else
            {
                parent.Children.Add(currentSegment, newslot);
            }
            return newslot;

        }


        public Path FindPath(String RelativeUrl, bool EnsureObjectId)
        {
            if (string.IsNullOrEmpty(RelativeUrl))
            {
                return root;
            }

            RelativeUrl = RelativeUrl.Replace("\\", "/").ToLower();
            string[] segments = RelativeUrl.Split('/');

            Path currentpath = root;

            foreach (var item in segments)
            {
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }
                Path child = _findPath(currentpath, item.RemoveRoutingCurlyBracket(), EnsureObjectId);
                if (child == null)
                {
                    if (item.StartsWith("?"))
                    {
                        if (EnsureObjectId)
                        {
                            if (currentpath.ObjectId != default(Guid))
                            {
                                return currentpath;
                            }
                        }
                        else
                        {
                            return currentpath;
                        }
                    }
                    // TODO: verify situation of this task. It breaks now of importing sites with both
                    //  pages for   [/subpath]   and [/subpath/sub/sub.html]. 
                    // if currentpath has {} sub. 
                    if (currentpath.segment == "{}" && currentpath.ObjectId != default(Guid))
                    {
                        if (!currentpath.Children.Any())
                        {
                            return currentpath;
                        } 
                    }

                    //if (currentpath.Children == null || !currentpath.Children.Any())
                    //{
                    //    if (currentpath.ObjectId != default(Guid))
                    //    { return currentpath;  }
                    //} 
                    return null;
                }
                else
                {
                    currentpath = child;
                }
            }

            return currentpath;

        }
         
        private Path _findPath(Path parent, string currentsegment, bool EnsureObjectId = false)
        {
            if (parent.Children.ContainsKey(currentsegment))
            {
                var item = parent.Children[currentsegment];

                if (EnsureObjectId)
                {
                    if (item != null && (item.ObjectId != default(Guid) || item.Children.Any()))
                    {
                        return item;
                    }
                }
                else
                {
                    return item;
                }
            }

            if (parent.Children.ContainsKey(this.wildcard))
            {
                if (!currentsegment.StartsWith("?"))
                {
                    var item = parent.Children[this.wildcard];
                    if (EnsureObjectId)
                    {
                        if (item != null && (item.ObjectId != default(Guid) || item.Children.Any()))
                        {
                            return item;
                        }
                    }
                    else
                    {
                        return item;
                    }
                }
            }

            foreach (var item in parent.Children)
            {
                if (item.Value.PartialWildCard)
                {
                    if (currentsegment.ContainsAllParts(item.Value.PartialParts))
                    {
                        if (EnsureObjectId)
                        {
                            if (item.Value != null && (item.Value.ObjectId != default(Guid) || item.Value.Children.Any()))
                            {
                                return item.Value;
                            }
                        }
                        else
                        {
                            return item.Value;
                        }
                    }
                }
            }
            return null;

        }

        private Path FindShortestWildCardPath(Path parent)
        {
            if (parent == null || parent.Children == null)
            {
                return null;
            }

            Path child;

            if (parent.Children.TryGetValue(this.wildcard, out child))
            {
                if (this.HasObject(child))
                {
                    return child;
                }
                else
                {
                    return FindShortestWildCardPath(child);
                }
            }

            return null;
        }

        public bool HasObject(Path path)
        {
            if (path == null)
            {
                return false;
            }
            return path.RouteId != default(Guid);
        }
    }

}
