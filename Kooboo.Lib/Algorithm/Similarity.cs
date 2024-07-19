//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Lib.Algorithm
{
    public static class Similarity
    {

        /// <summary>
        /// Get the similarity score, score between 0 to 100. 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int GetPercent(string x, string y)
        {
            var editpath = Kooboo.Lib.Algorithm.Diff.GetPath(x, y);

            // similiarity is the length of keep / (x.len = y.len)/2. 
            int keepcount = 0;
            foreach (var item in editpath)
            {
                if (item.EditType == Lib.Algorithm.EditAction.ActionType.Keep)
                {
                    keepcount += item.Value.Length;
                }
            }

            return (int)(100 * keepcount / ((x.Length + y.Length) / 2));
        }

    }
}
