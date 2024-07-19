//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

namespace Kooboo.IndexedDB
{
    /// <summary>
    /// /range query.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Range<T>
    {
        public enum Kind
        {
            Range,
            None,
            All
        }

        public Kind Take = Kind.Range;

        /// <summary>
        /// This value is the lower-bound of the key range.
        /// </summary>
        public T lower;

        /// <summary>
        /// This value is the upper-bound of the key range.
        /// </summary>
        public T upper;

        /// <summary>
        /// Returns false if the lower-bound value is included in the key range. Returns true if the lower-bound value is excluded from the key range.
        /// </summary>
        public bool lowerOpen;

        /// <summary>
        /// Returns false if the upper-bound value is included in the key range. Returns true if the upper-bound value is excluded from the key range.
        /// </summary>
        public bool upperOpen;


        /// <summary>
        /// Creates and returns a new key range with both lower and upper set to value and both lowerOpen and upperOpen set to false. If the value parameter is not a valid key, the implementation must throw a DOMException of type DataError.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Range<T> only(T value)
        {
            return new Range<T> { lower = value, upper = value, lowerOpen = false, upperOpen = false };
        }


        /// <summary>
        /// Creates and returns a new key range with lower set to lower, lowerOpen set to open, upper set to undefined and and upperOpen set to true.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Range<T> lowerBound(T lower, bool open)
        {
            return new Range<T> { lower = lower, lowerOpen = open, upper = default(T), upperOpen = true };
        }


        /// <summary>
        /// Creates and returns a new key range with lower set to undefined, lowerOpen set to true, upper set to upper and and upperOpen set to open.
        /// </summary>
        /// <param name="upper"></param>
        /// <param name="open"></param>
        /// <returns></returns>
        public static Range<T> upperBound(T upper, bool open)
        {
            return new Range<T> { upper = upper, upperOpen = open };
        }


        /// <summary>
        /// Creates and returns a new key range with lower set to lower, lowerOpen set to lowerOpen, upper set to upper and upperOpen set to upperOpen.
        /// </summary>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        /// <param name="lowerOpen"></param>
        /// <param name="upperOpen"></param>
        /// <returns></returns>
        public static Range<T> bound(T lower, T upper, bool lowerOpen = false, bool upperOpen = false)
        {
            return new Range<T> { lower = lower, upper = upper, lowerOpen = lowerOpen, upperOpen = upperOpen };
        }

        public static Range<T> TakeAll()
        {
            return new Range<T> { Take = Kind.All };
        }

        public static Range<T> TakeNone()
        {
            return new Range<T> { Take = Kind.None };
        }
    }
}