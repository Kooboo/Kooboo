//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Extensions;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
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

        public void AddOrUpdate(string relativeUrl, Guid routeKey, Guid objectId, byte constType, Dictionary<string, string> parameters)
        {
            if (string.IsNullOrEmpty(relativeUrl))
            {
                relativeUrl = "/";
            }

            relativeUrl = relativeUrl.Replace("\\", "/").ToLower().Trim();
            string[] segments = relativeUrl.Split('/');

            List<string> seglist = new List<string>();

            foreach (var item in segments)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    seglist.Add(item.RemoveRoutingCurlyBracket());
                }
            }

            AddList(seglist, routeKey, objectId, constType, parameters);
        }

        public void Del(string relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl))
            {
                relativeUrl = "/";
            }

            relativeUrl = relativeUrl.Replace("\\", "/").ToLower().Trim();
            string[] segments = relativeUrl.Split('/');

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
        /// <param name="relativeUrl"></param>
        /// <param name="ensureObjectId"></param>
        /// <returns></returns>
        public Guid FindRouteId(string relativeUrl, bool ensureObjectId)
        {
            lock (_locker)
            {
                Path path = FindPath(relativeUrl, ensureObjectId);

                if (path == null)
                {
                    int questionMark = relativeUrl.IndexOf("?");
                    if (questionMark > 0)
                    {
                        relativeUrl = relativeUrl.Substring(0, questionMark);
                        path = FindPath(relativeUrl, ensureObjectId);
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

                if (!this.HasObject(path))
                {
                    path = FindShortestWildCardPath(path);
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

        public Path FindPath(String relativeUrl, bool ensureObjectId)
        {
            if (string.IsNullOrEmpty(relativeUrl))
            {
                return root;
            }

            relativeUrl = relativeUrl.Replace("\\", "/").ToLower();
            string[] segments = relativeUrl.Split('/');

            Path currentpath = root;

            foreach (var item in segments)
            {
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }

                Path child = _findPath(currentpath, item.RemoveRoutingCurlyBracket(), ensureObjectId);
                if (child == null)
                {
                    // TODO: verify situation of this task. It breaks now of importing sites with both
                    //  pages for   [/subpath]   and [/subpath/sub/sub.html].
                    // if currentpath has {} sub.
                    //if (currentpath.Children == null || !currentpath.Children.Any())
                    //{
                    //    if (currentpath.ObjectId != default(Guid))
                    //    {
                    //        return currentpath;
                    //    }
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

        private void RemoveEmptySlot(Path currentPath)
        {
            if (currentPath.RouteId == default(Guid) && currentPath.ParentPath != null && (currentPath.Children == null || currentPath.Children.Count == 0))
            {
                currentPath.ParentPath.Children.Remove(currentPath.segment);
                RemoveEmptySlot(currentPath.ParentPath);
            }
        }

        private void AddList(List<string> seglist, Guid routeid, Guid objectId, byte constType, Dictionary<string, string> parameters)
        {
            lock (_locker)
            {
                Path currentslot = root;

                foreach (string item in seglist)
                {
                    Path child = _findPath(currentslot, item);
                    if (child == null)
                    {
                        child = addPath(currentslot, item, objectId);
                        currentslot = child;
                    }
                    else
                    {
                        currentslot = child;
                    }
                }

                if (currentslot.ObjectId != objectId)
                {
                    if (objectId != default(Guid))
                    {
                        currentslot.ObjectId = objectId;
                    }
                }

                currentslot.RouteId = routeid;
            }
        }

        private Path addPath(Path parent, string currentSegment, Guid objectId)
        {
            Path newslot = new Path {segment = currentSegment, ParentPath = parent, ObjectId = objectId};

            if (currentSegment.Contains("{") && currentSegment.Contains("}") && currentSegment != "{}")
            {
                newslot.PartialWildCard = true;
            }

            if (parent.Children.ContainsKey(currentSegment))
            {
                var currentValue = parent.Children[currentSegment];
                if (currentValue.ObjectId == default(Guid))
                {
                    currentValue.ObjectId = objectId;
                }
            }
            else
            {
                parent.Children.Add(currentSegment, newslot);
            }
            return newslot;
        }

        private Path _findPath(Path parent, string currentsegment, bool ensureObjectId = false)
        {
            if (parent.Children.ContainsKey(currentsegment))
            {
                var item = parent.Children[currentsegment];

                if (ensureObjectId)
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
                var item = parent.Children[this.wildcard];

                if (ensureObjectId)
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

            foreach (var item in parent.Children)
            {
                if (item.Value.PartialWildCard)
                {
                    if (currentsegment.ContainsAllParts(item.Value.PartialParts))
                    {
                        if (ensureObjectId)
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