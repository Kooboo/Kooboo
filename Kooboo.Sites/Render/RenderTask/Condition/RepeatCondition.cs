//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Render
{
    public class RepeatCondition
    {
        public Stack<RepeaterCounter> stack = new Stack<RepeaterCounter>();

        public void Push(int total)
        {
            RepeaterCounter counter = new RepeaterCounter {Total = total, Current = 0};

            this.stack.Push(counter);
        }

        public void Pop()
        {
            this.stack.Pop();
        }

        public RepeaterCounter CurrentCounter
        {
            get
            {
                return this.stack.First();
            }
        }

        public bool Check(string condition)
        {
            if (string.IsNullOrEmpty(condition))
            {
                return false;
            }

            string lower = condition.ToLower().Trim();

            if (lower == "odd")
            {
                return IsOdd(this.CurrentCounter.Current);
            }
            else if (lower == "even")
            {
                return !IsOdd(this.CurrentCounter.Current);
            }
            else if (lower == "first")
            {
                return this.CurrentCounter.Current == 1;
            }
            else if (lower == "last")
            {
                return this.CurrentCounter.Current == this.CurrentCounter.Total;
            }
            else
            {
                if (int.TryParse(lower, out var counter))
                {
                    return counter == this.CurrentCounter.Current;
                }
                return false;
            }
        }

        private bool IsOdd(int value)
        {
            return value % 2 != 0;
        }

        public class RepeaterCounter
        {
            public int Total { get; set; }
            public int Current { get; set; }
        }
    }
}