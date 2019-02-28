//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Threading;

namespace Kooboo.Sites.Diagnosis
{
  public  class Progress
    { 
        private int _totalAmount; 
        public int TotalAmount
        {
            get
            {
                return _totalAmount;
            }
        }

        private int _checkedAmount; 
        public int CheckedAmount
        {
            get
            {
                return _checkedAmount;
            }
        }

        private int _criticalAmount; 
        public int CriticalAmount
        {
            get
            {
                return _criticalAmount;
            }
        }

        private int _warningAmount; 
        public int WarningAmount
        {
            get
            {
                return _warningAmount;
            }
        }

        public void IncreaseTotalAmount(int increament)
        {
            Interlocked.Add(ref _totalAmount, increament);
        }

        public void IncreaseCheckedAmount(int increament)
        {
            Interlocked.Add(ref _checkedAmount, increament);
        }

        public void IncreaseCriticalAmount()
        {
            Interlocked.Increment(ref _criticalAmount);
        }

        public void IncreaseWarnningAmount()
        {
            Interlocked.Increment(ref _warningAmount);
        }

    }


 




}
