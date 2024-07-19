//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Lib.Algorithm
{
    public class Diff<T>
    {
        private IEqualityComparer<T> comparer;

        public Diff(IEqualityComparer<T> comparer)
        {
            this.comparer = comparer;
        }


        public Diff()
        {

        }


        public bool CompareT(T x, T y)
        {
            if (this.comparer != null)
            {
                return this.comparer.Equals(x, y);
            }
            else
            {
                return x.GetHashCode() == y.GetHashCode();
            }
        }

        /// <summary>
        /// The famouse longest common sub sequence problem.
        /// This implmentation follows: http://en.wikipedia.org/wiki/Longest_common_subsequence_problem
        /// This implementation may have very poor performance. 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public List<T> LCS(List<T> x, List<T> y)
        {
            int xlength = x.Count;
            int ylength = y.Count;

            if (xlength == 0 || ylength == 0)
            {
                // return empty? should make it null?
                return new List<T>();
            }

            List<T> xSub = x.Take(xlength - 1).ToList();
            List<T> ySub = y.Take(ylength - 1).ToList();

            if (this.CompareT(x[xlength - 1], y[ylength - 1]))
            {
                var next = LCS(xSub, ySub);
                next.Add(x[xlength - 1]);
                return next;
            }
            else
            {
                List<T> a = LCS(x, ySub);
                List<T> b = LCS(xSub, y);

                if (a.Count > b.Count)
                {
                    return a;
                }
                else
                {
                    return b;
                }

            }
        }

        /// <summary>
        /// get the edit path from string A to B. 
        /// This is the full edit path. The change script should take the Add/Del part and ignore the keep part.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public List<EditAction<T>> GetPath(List<T> A, List<T> B)
        {
            List<EditAction<T>> actionlist = new List<EditAction<T>>();

            int ALen = A.Count;
            int BLen = B.Count;
            bool foundshorter = false;

            int Bstartindex = 0; // the current compare position of string B. 

            int Astartindex = 0; // current/last index positionn of string A. 

            for (int i = 0; i < ALen; i++)
            {
                T oneA = A[i];

                for (int j = Bstartindex; j < BLen; j++)
                {
                    T oneB = B[j];

                    if (this.CompareT(oneA, oneB))
                    {

                        if (j == Bstartindex || j == Bstartindex + 1)
                        {
                            if (j == Bstartindex)
                            {

                                if (Astartindex < i)
                                {
                                    for (int ai = Astartindex; ai < i; ai++)
                                    {
                                        AddAction(actionlist, EditAction<T>.ActionType.Del, A[ai]);
                                    }

                                }

                                AddAction(actionlist, EditAction<T>.ActionType.Keep, oneA);

                                Bstartindex = j + 1;
                                Astartindex = i + 1;

                                if (CheckEnd(actionlist, ALen, BLen, Astartindex, Bstartindex, A, B))
                                {
                                    return actionlist;
                                }


                            }
                            else
                            {

                                if (Astartindex < i)
                                {
                                    for (int ai = Astartindex; ai < i; ai++)
                                    {
                                        AddAction(actionlist, EditAction<T>.ActionType.Del, A[ai]);
                                    }

                                }

                                // j= jstartindex+1. 
                                AddAction(actionlist, EditAction<T>.ActionType.Add, B[Bstartindex]);

                                // AddAction(actionlist, EditAction<T>.ActionType.Keep, B[Bstartindex + 1]);
                                //TODO: please check this part. does oneA == B[Bstartindex +1. 
                                AddAction(actionlist, EditAction<T>.ActionType.Keep, oneA);

                                Bstartindex = Bstartindex + 2;

                                Astartindex = i + 1;

                                if (CheckEnd(actionlist, ALen, BLen, Astartindex, Bstartindex, A, B))
                                {
                                    return actionlist;
                                }

                            }

                        }


                        else
                        {
                            // need to look at the shortest, no necessary go down now.
                            int distince = j - Bstartindex;  // the current distance. 
                            int maxLookAheadIndex = i + distince;

                            foundshorter = false;

                            if (maxLookAheadIndex > ALen)
                            {
                                maxLookAheadIndex = ALen;
                            }

                            for (int xi = i + 1; xi < maxLookAheadIndex; xi++)
                            {

                                T XA = A[xi];

                                for (int xj = Bstartindex; xj < j; xj++)
                                {
                                    T XB = B[xj];

                                    if (this.CompareT(XA, XB))
                                    {
                                        // get the distince and compare.
                                        int Xdistince = xj - Bstartindex + xi - i;

                                        if (Xdistince < distince)
                                        {
                                            // shorter path found. use it. 

                                            foundshorter = true;

                                            // remove x, add y and keep current. 
                                            if (Astartindex < xi)
                                            {
                                                for (int zi = i; zi < xi; zi++)
                                                {
                                                    AddAction(actionlist, EditAction<T>.ActionType.Del, A[zi]);
                                                }
                                            }

                                            if (Bstartindex < xj)
                                            {
                                                for (int zi = Bstartindex; zi < xj; zi++)
                                                {
                                                    AddAction(actionlist, EditAction<T>.ActionType.Add, B[zi]);
                                                }
                                            }

                                            AddAction(actionlist, EditAction<T>.ActionType.Keep, A[xi]);

                                            i = xi;
                                            Astartindex = i + 1;
                                            Bstartindex = xj + 1;


                                            if (CheckEnd(actionlist, ALen, BLen, Astartindex, Bstartindex, A, B))
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

                                if (Astartindex < i)
                                {
                                    for (int zi = Astartindex; zi < i; zi++)
                                    {
                                        AddAction(actionlist, EditAction<T>.ActionType.Del, A[zi]);
                                    }
                                }

                                for (int zi = Bstartindex; zi < j; zi++)
                                {
                                    AddAction(actionlist, EditAction<T>.ActionType.Add, B[zi]);
                                }


                                AddAction(actionlist, EditAction<T>.ActionType.Keep, A[i]);

                                Bstartindex = j + 1;
                                Astartindex = i + 1;

                                if (CheckEnd(actionlist, ALen, BLen, Astartindex, Bstartindex, A, B))
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

        public List<EditAction<T>> GetChangePath(List<T> A, List<T> B)
        {
            return GetPath(A, B).TakeWhile(o => o.EditType != EditAction<T>.ActionType.Keep).ToList();
        }

        /// <summary>
        /// The length of items that are kept. 
        /// </summary>
        /// <param name="edits"></param>
        /// <returns></returns>
        public int KeepLength(List<Lib.Algorithm.EditAction<T>> edits)
        {
            int count = 0;
            foreach (var item in edits)
            {
                if (item.EditType == Lib.Algorithm.EditAction<T>.ActionType.Keep)
                {
                    count += item.Value.Count;
                }
            }

            return count;
        }

        /// <summary>
        /// Get the common sub sequence using edit path.
        /// </summary>
        /// <param name="edits"></param>
        /// <returns></returns>
        public List<T> getCommons(List<Lib.Algorithm.EditAction<T>> edits)
        {
            List<T> list = new List<T>();

            foreach (var item in edits)
            {
                if (item.EditType == Lib.Algorithm.EditAction<T>.ActionType.Keep)
                {
                    list.AddRange(item.Value);
                }
            }
            return list;
        }

        /// <summary>
        /// the number of location that has add/del. 
        /// </summary>
        /// <param name="edits"></param>
        /// <returns></returns>
        public int ChangeLocationCount(List<Lib.Algorithm.EditAction<T>> edits)
        {
            int changecount = 0;
            bool inChange = false;

            foreach (var item in edits)
            {
                if (item.EditType != EditAction<T>.ActionType.Keep)
                {
                    if (!inChange)
                    {
                        inChange = true;
                        changecount += 1;
                    }
                }
                else
                {
                    inChange = false;
                }
            }

            return changecount;
        }

        private void AddAction(List<EditAction<T>> currentSet, EditAction<T>.ActionType type, T value)
        {
            EditAction<T> newAction = new EditAction<T>();
            newAction.EditType = type;
            newAction.Value.Add(value);

            int length = currentSet.Count;

            if (length == 0)
            {
                currentSet.Add(newAction);
            }
            else
            {
                EditAction<T> lastaction = currentSet[length - 1];

                if (lastaction.EditType == newAction.EditType)
                {
                    lastaction.Value.Add(value);
                }
                else
                {
                    currentSet.Add(newAction);
                }
            }
        }

        private bool CheckEnd(List<EditAction<T>> actionlist, int ALen, int BLen, int currentAIndex, int currentBIndex, List<T> A, List<T> B)
        {
            if (currentAIndex == ALen && currentBIndex <= BLen - 1)
            {
                // end of A. Add all the rest of B. 
                for (int i = currentBIndex; i < BLen; i++)
                {
                    AddAction(actionlist, EditAction<T>.ActionType.Add, B[i]);
                }

                return true;
            }

            if (currentBIndex == BLen && currentAIndex <= ALen - 1)
            {
                // end of B. Remove all rest of A. 

                for (int i = currentAIndex; i < ALen; i++)
                {
                    AddAction(actionlist, EditAction<T>.ActionType.Del, A[i]);
                }

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
    public class EditAction<T>
    {
        public EditAction()
        {
            this.Value = new List<T>();
        }

        public ActionType EditType { get; set; }

        public List<T> Value { get; set; }

        /// <summary>
        /// the item index of x list
        /// </summary>
        public int XItemIndex { get; set; }

        /// <summary>
        /// the item index of y list.
        /// </summary>
        public int YItemIndex { get; set; }


        public enum ActionType
        {
            Add = 0,
            Del = 2,
            Keep = 3
        }

    }



}
