//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Text;

namespace Kooboo.Dom
{

    //    Interface CharacterData
    //interface CharacterData : Node {
    //  [TreatNullAs=EmptyString] attribute DOMString data;
    //  readonly attribute unsigned long length;
    //  DOMString substringData(unsigned long offset, unsigned long count);
    //  void appendData(DOMString data);
    //  void insertData(unsigned long offset, DOMString data);
    //  void deleteData(unsigned long offset, unsigned long count);
    //  void replaceData(unsigned long offset, unsigned long count, DOMString data);
    //};
    [Serializable]
    public abstract class CharacterData : Node
    {
        public CharacterData()
        {
            sb = new StringBuilder();
        }

        private StringBuilder sb;

        public string data
        {
            get
            {
                return sb.ToString();
            }

            set { appendData(value); }

        }




        //public int length
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(data))
        //        {
        //            return 0;
        //        }
        //        else
        //        {
        //            return data.Length;
        //        }
        //    }
        //}

        public string substringData(int offset, int count)
        {
            throw new NotImplementedException();
        }



        public void appendData(string data)
        {
            sb.Append(data);
        }

        public void appendData(char chr)
        {
            sb.Append(data);
        }

        public void insertData(int offset, string data)
        { throw new NotImplementedException(); }

        public void deleteData(int offset, int count)
        { throw new NotImplementedException(); }

        public void replaceData(int offset, int count, string data)
        { throw new NotImplementedException(); }

    }
}
