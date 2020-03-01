using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Alipay.lib
{
    public class TradeStatus
    {
        public const string WAIT_BUYER_PAY = "WAIT_BUYER_PAY";
        public const string TRADE_CLOSED = "TRADE_CLOSED";
        public const string TRADE_SUCCESS = "TRADE_SUCCESS";
        public const string TRADE_FINISHED = "TRADE_FINISHED";
    }
}
