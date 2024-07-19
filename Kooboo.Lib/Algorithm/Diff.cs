//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Linq;


namespace Kooboo.Lib.Algorithm
{
    public class Diff
    {

        /// <summary>
        /// The famouse longest common sub sequence problem.
        /// This implmentation follows: http://en.wikipedia.org/wiki/Longest_common_subsequence_problem
        /// This implementation may have very poor performance. 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static string LCS(string x, string y)
        {
            int xlength = x.Length;
            int ylength = y.Length;

            if (xlength == 0 || ylength == 0)
            {
                return string.Empty;
            }

            string xSub = x.Substring(0, x.Length - 1);
            string ySub = y.Substring(0, y.Length - 1);

            if (x[xlength - 1] == y[ylength - 1])

                return LCS(xSub, ySub) + x[x.Length - 1];

            else
            {
                string a = LCS(x, ySub);
                string b = LCS(xSub, y);
                return (a.Length > b.Length) ? a : b;
            }
        }


        /// <summary>
        /// get the edit path from string A to B. 
        /// This is the full edit path. The change script should take the Add/Del part and ignore the keep part.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static List<EditAction> GetPath(string A, string B)
        {
            List<EditAction> actionlist = new List<EditAction>();

            int ALen = A.Length;
            int BLen = B.Length;
            bool foundshorter = false;

            int jstartindex = 0; // the current compare position of string B. 
            int istartindex = 0; // current/last index positionn of string A. 

            for (int i = 0; i < ALen; i++)
            {
                char Achar = A[i];
                for (int j = jstartindex; j < BLen; j++)
                {
                    char Bchar = B[j];

                    if (Achar == Bchar)
                    {
                        if (j == jstartindex || j == jstartindex + 1)
                        {
                            if (j == jstartindex)
                            {
                                if (istartindex < i)
                                {
                                    AddAction(actionlist, EditAction.ActionType.Del, A.Substring(istartindex, i - istartindex));
                                }
                                AddAction(actionlist, EditAction.ActionType.Keep, Achar.ToString());

                                jstartindex = j + 1;
                                istartindex = i + 1;

                                if (CheckEnd(actionlist, ALen, BLen, istartindex, jstartindex, A, B))
                                {
                                    return actionlist;
                                }
                            }
                            else
                            {
                                if (istartindex < i)
                                {
                                    AddAction(actionlist, EditAction.ActionType.Del, A.Substring(istartindex, i - istartindex));
                                }

                                AddAction(actionlist, EditAction.ActionType.Add, B[jstartindex].ToString());
                                AddAction(actionlist, EditAction.ActionType.Keep, B[jstartindex + 1].ToString());

                                jstartindex = jstartindex + 2;

                                istartindex = i + 1;

                                if (CheckEnd(actionlist, ALen, BLen, istartindex, jstartindex, A, B))
                                {
                                    return actionlist;
                                }
                            }
                        }
                        else
                        {
                            // need to look at the shortest, no necessary go down now.
                            int distince = j - jstartindex;  // the current distance. 
                            int maxLookAheadIndex = i + distince;

                            foundshorter = false;

                            if (maxLookAheadIndex > ALen)
                            {
                                maxLookAheadIndex = ALen;
                            }

                            for (int xi = i + 1; xi < maxLookAheadIndex; xi++)
                            {
                                char XAChar = A[xi];
                                for (int xj = jstartindex; xj < j; xj++)
                                {
                                    char XBChar = B[xj];
                                    if (XAChar == XBChar)
                                    {
                                        // get the distince and compare.
                                        int Xdistince = xj - jstartindex + xi - i;

                                        if (Xdistince < distince)
                                        {
                                            // shorter path found. use it.
                                            foundshorter = true;

                                            // remove x, add y and keep current. 
                                            if (i < xi)
                                            {
                                                AddAction(actionlist, EditAction.ActionType.Del, A.Substring(i, xi - i));
                                            }
                                            if (jstartindex < xj)
                                            {
                                                AddAction(actionlist, EditAction.ActionType.Add, B.Substring(jstartindex, xj - jstartindex));
                                            }
                                            AddAction(actionlist, EditAction.ActionType.Keep, A[xi].ToString());

                                            i = xi;
                                            istartindex = i + 1;
                                            jstartindex = xj + 1;

                                            if (CheckEnd(actionlist, ALen, BLen, istartindex, jstartindex, A, B))
                                            {
                                                return actionlist;
                                            }
                                            break;
                                        }
                                    }
                                }
                                if (foundshorter)
                                {
                                    break;
                                }
                            }
                            /// if not found, the j is the next shortest path. go down now. 
                            // add the y, and keep current. 
                            if (!foundshorter)
                            {
                                if (istartindex < i)
                                {
                                    AddAction(actionlist, EditAction.ActionType.Del, A.Substring(istartindex, i - istartindex));
                                }
                                AddAction(actionlist, EditAction.ActionType.Add, B.Substring(jstartindex, j - jstartindex));
                                AddAction(actionlist, EditAction.ActionType.Keep, A[i].ToString());

                                jstartindex = j + 1;
                                istartindex = i + 1;

                                if (CheckEnd(actionlist, ALen, BLen, istartindex, jstartindex, A, B))
                                {
                                    return actionlist;
                                }
                            }
                        }
                        break;
                    }
                }
            }
            return actionlist;
        }

        public static List<EditAction> GetChangePath(string A, string B)
        {
            return GetPath(A, B).TakeWhile(o => o.EditType != EditAction.ActionType.Keep).ToList();
        }

        private static void AddAction(List<EditAction> currentSet, EditAction.ActionType type, string value)
        {
            EditAction newAction = new EditAction() { EditType = type, Value = value };

            int length = currentSet.Count;

            if (length == 0)
            {
                currentSet.Add(newAction);
            }
            else
            {
                EditAction lastaction = currentSet[length - 1];

                if (lastaction.EditType == newAction.EditType)
                {
                    lastaction.Value += newAction.Value;
                }
                else
                {
                    currentSet.Add(newAction);
                }
            }
        }

        private static bool CheckEnd(List<EditAction> actionlist, int ALen, int BLen, int currentAIndex, int currentBIndex, string A, string B)
        {
            if (currentAIndex == ALen && currentBIndex <= BLen - 1)
            {
                // end of A. Add all the rest of B. 
                AddAction(actionlist, EditAction.ActionType.Add, B.Substring(currentBIndex));
                return true;
            }

            if (currentBIndex == BLen && currentAIndex <= ALen - 1)
            {
                // end of B. Remove all rest of A. 
                AddAction(actionlist, EditAction.ActionType.Del, A.Substring(currentAIndex));
                return true;
            }

            if (currentAIndex == ALen && currentBIndex == BLen)
            {
                return true;
            }

            return false;

        }

    }

    /// <summary>
    /// The edit path from object A to object B. Now we use string only. Can be extended to others.
    /// </summary>
    public class EditAction
    {

        public ActionType EditType { get; set; }

        public string Value { get; set; }

        public enum ActionType
        {
            Add = 0,
            Del = 2,
            Keep = 3
        }

    }



}
