//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Ude.Core
{
    public class EUCJPDistributionAnalyser : SJISDistributionAnalyser
    {
        public override int GetOrder(byte[] buf, int offset)
        {
            if (buf[offset] >= 160)
            {
                return (int)(checked(94 * (buf[offset] - 161) + buf[offset + 1] - 161));
            }
            return -1;
        }
    }
}
