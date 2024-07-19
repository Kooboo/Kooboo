using System;
using System.Collections.Generic;

namespace Kooboo.Lib.Domain
{

    public class TreePath
    {
        public Dictionary<char, TreePath> Children;
        public bool HasSuffix;

        // for init adding. this is only run one time. Not need to be thread safe. 
        public TreePath GetOrSetChild(char current)
        {
            if (Children == null)
            {
                Children = new Dictionary<char, TreePath>();
            }

            if (Children.TryGetValue(current, out var path))
            {
                return path;
            }

            var treePath = new TreePath();
            Children[current] = treePath;

            return treePath;
        }

        // for search. 
        public TreePath GetChild(char current)
        {
            if (Children == null)
            {
                return null;
            }
            if (Children.TryGetValue(current, out var path))
            {
                return path;
            }
            return null;
        }
    }

    public class StringDomainTree
    {
        public static StringDomainTree Instance { get; set; } = new StringDomainTree(true);

        public StringDomainTree(bool LoadPublicData)
        {
            this.Root = new StringTreePath();
            if (LoadPublicData)
            {
                PublicSuffix suffix = new PublicSuffix();
                var lines = suffix.ReadAllLines();

                foreach (var item in lines)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        var sf = item.Trim();

                        if (sf.StartsWith("*."))
                        {
                            sf = sf.Substring(2);
                        }

                        this.AddToTree(sf);
                    }
                }
            }
        }

        public StringTreePath Root;

        public string GetMatchSuffic(string FullDomain)
        {
            return GetMatchSufficIndex(FullDomain);
        }

        public string GetMatchSufficIndex(string FullDomain)
        {
            // Stack<int> found = new();

            int found = -1;

            var path = this.Root;

            var parts = FullDomain.Split('.', StringSplitOptions.RemoveEmptyEntries);

            for (int i = parts.Length - 1; i >= 0; i--)
            {
                path = path.GetChild(parts[i]);

                if (path == null)
                {
                    break;
                }

                else if (path.HasSuffix)
                {
                    found = i;
                }
            }

            if (found > -1)
            {
                string value = null;
                for (int i = found; i < parts.Length; i++)
                {
                    value += parts[i] + ".";
                }
                return value.TrimEnd('.');
            }

            return null;
        }

        public void AddToTree(string domainSuffix)
        {
            if (string.IsNullOrWhiteSpace(domainSuffix))
            {
                return;
            }
            if (domainSuffix.StartsWith("*."))
            {
                domainSuffix = domainSuffix.Substring(2);
            }
            var currentPath = this.Root;

            var parts = domainSuffix.Split('.', StringSplitOptions.RemoveEmptyEntries);

            int len = parts.Length;

            for (int i = len - 1; i >= 0; i--)
            {
                var currentChar = parts[i];
                currentPath = currentPath.GetOrSetChild(currentChar);
            }
            currentPath.HasSuffix = true;
        }
    }

    public class StringTreePath
    {
        public Dictionary<string, StringTreePath> Children;

        public bool HasSuffix;

        // for init adding. this is only run one time. Not need to be thread safe. 
        public StringTreePath GetOrSetChild(string current)
        {
            if (Children == null)
            {
                Children = new Dictionary<string, StringTreePath>();
            }

            if (Children.TryGetValue(current, out var path))
            {
                return path;
            }

            var treePath = new StringTreePath();
            Children[current] = treePath;

            return treePath;
        }

        // for search. 
        public StringTreePath GetChild(string current)
        {
            if (Children == null)
            {
                return null;
            }
            if (Children.TryGetValue(current, out var path))
            {
                return path;
            }
            return null;
        }
    }

}
