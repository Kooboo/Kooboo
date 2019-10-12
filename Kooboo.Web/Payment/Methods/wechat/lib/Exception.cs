using System;

namespace WxPayAPI
{
    public class WxPayException : Exception
    {
        public WxPayException(string msg) : base(msg)
        {
        }
    }
}