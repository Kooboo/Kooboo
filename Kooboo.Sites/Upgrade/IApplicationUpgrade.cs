//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.Sites.Upgrade
{
    public interface IApplicationUpgrade
    {
        System.Version UpVersion { get; }

        System.Version LowerVersion { get; }

        void Do();
    }
}