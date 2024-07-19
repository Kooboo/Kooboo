//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;


namespace Kooboo.Lib.Compatible
{
    public interface ISystem
    {

        string GetUpgradeUrl(string convertApiUrl);

        string GetSlash();

        int GetLastSlash(string path);

        string CombinePath(string root, string relativePath);

        string CombineRelativePath(string relativePath, string path);

        string JoinPath(string[] segments);

        List<string> GetSegments(string input);

    }
}
