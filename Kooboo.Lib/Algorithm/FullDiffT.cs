//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Lib.Algorithm
{
    /// <summary>
    /// Return the full edit path. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FullDiffT<T>
    {
        private IEqualityComparer<T> comparer;

        public FullDiffT(IEqualityComparer<T> comparer)
        {
            this.comparer = comparer;
        }

        public FullDiffT()
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
        /// get the edit path from string A to B. 
        /// This is the full edit path. The change script should take the Add/Del part and ignore the keep part.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public List<FullEditAction<T>> GetPath(List<T> A, List<T> B)
        {
            List<FullEditAction<T>> actionlist = new List<FullEditAction<T>>();

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
                                        AddAction(actionlist, FullEditAction<T>.ActionType.Del, A[ai], ai, -1);
                                    }
                                }

                                AddAction(actionlist, FullEditAction<T>.ActionType.Keep, oneA, i, j);

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
                                        AddAction(actionlist, FullEditAction<T>.ActionType.Del, A[ai], ai, -1);
                                    }
                                }

                                // j= jstartindex+1. 
                                AddAction(actionlist, FullEditAction<T>.ActionType.Add, B[Bstartindex], -1, Bstartindex);

                                // AddAction(actionlist, EditAction<T>.ActionType.Keep, B[Bstartindex + 1]);
                                //TODO: please check this part. does oneA == B[Bstartindex +1. 
                                AddAction(actionlist, FullEditAction<T>.ActionType.Keep, oneA, i, j);

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
                                                    AddAction(actionlist, FullEditAction<T>.ActionType.Del, A[zi], zi, -1);
                                                }
                                            }

                                            if (Bstartindex < xj)
                                            {
                                                for (int zi = Bstartindex; zi < xj; zi++)
                                                {
                                                    AddAction(actionlist, FullEditAction<T>.ActionType.Add, B[zi], -1, zi);
                                                }
                                            }

                                            AddAction(actionlist, FullEditAction<T>.ActionType.Keep, A[xi], xi, xj);

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
                                        AddAction(actionlist, FullEditAction<T>.ActionType.Del, A[zi], zi, -1);
                                    }
                                }

                                for (int zi = Bstartindex; zi < j; zi++)
                                {
                                    AddAction(actionlist, FullEditAction<T>.ActionType.Add, B[zi], -1, zi);
                                }


                                AddAction(actionlist, FullEditAction<T>.ActionType.Keep, A[i], i, j);

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

        public List<FullEditAction<T>> GetChangePath(List<T> A, List<T> B)
        {
            return GetPath(A, B).TakeWhile(o => o.EditType != FullEditAction<T>.ActionType.Keep).ToList();
        }

        /// <summary>
        /// The length of items that are kept. 
        /// </summary>
        /// <param name="edits"></param>
        /// <returns></returns>
        public int KeepLength(List<Lib.Algorithm.FullEditAction<T>> edits)
        {
            int count = 0;
            foreach (var item in edits)
            {
                if (item.EditType == Lib.Algorithm.FullEditAction<T>.ActionType.Keep)
                {
                    count += 1;
                }
            }

            return count;
        }

        /// <summary>
        /// Get the common sub sequence using edit path.
        /// </summary>
        /// <param name="edits"></param>
        /// <returns></returns>
        public List<T> getCommons(List<Lib.Algorithm.FullEditAction<T>> edits)
        {
            List<T> list = new List<T>();

            foreach (var item in edits)
            {
                if (item.EditType == Lib.Algorithm.FullEditAction<T>.ActionType.Keep)
                {
                    list.Add(item.Value);
                }
            }
            return list;
        }

        /// <summary>
        /// the number of location that has add/del. 
        /// </summary>
        /// <param name="edits"></param>
        /// <returns></returns>
        public int ChangeLocationCount(List<Lib.Algorithm.FullEditAction<T>> edits)
        {
            int changecount = 0;
            bool inChange = false;

            foreach (var item in edits)
            {
                if (item.EditType != FullEditAction<T>.ActionType.Keep)
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

        private void AddAction(List<FullEditAction<T>> currentSet, FullEditAction<T>.ActionType type, T value, int AIndex, int BIndex)
        {
            FullEditAction<T> newAction = new FullEditAction<T>();
            newAction.EditType = type;
            newAction.Value = value;

            newAction.AIndex = AIndex;
            newAction.BIndex = BIndex;

            currentSet.Add(newAction);

        }

        private bool CheckEnd(List<FullEditAction<T>> actionlist, int ALen, int BLen, int currentAIndex, int currentBIndex, List<T> A, List<T> B)
        {
            if (currentAIndex == ALen && currentBIndex <= BLen - 1)
            {
                // end of A. Add all the rest of B. 
                for (int i = currentBIndex; i < BLen; i++)
                {
                    AddAction(actionlist, FullEditAction<T>.ActionType.Add, B[i], -1, i);
                }

                return true;
            }

            if (currentBIndex == BLen && currentAIndex <= ALen - 1)
            {
                // end of B. Remove all rest of A. 

                for (int i = currentAIndex; i < ALen; i++)
                {
                    AddAction(actionlist, FullEditAction<T>.ActionType.Del, A[i], i, -1);
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
    public class FullEditAction<T>
    {
        public ActionType EditType { get; set; }

        public T Value { get; set; }

        /// <summary>
        /// the item index of x list
        /// </summary>
        public int AIndex { get; set; }

        /// <summary>
        /// the item index of y list.
        /// </summary>
        public int BIndex { get; set; }


        public enum ActionType
        {
            Add = 0,
            Del = 2,
            Keep = 3
        }

    }



}
