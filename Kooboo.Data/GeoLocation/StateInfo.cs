//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Data.GeoLocation
{ 
	public class StateInfo
	{

        private int _id; 

		public int Id {
            get
            {
                if (_id == default(int))
                {
                    string unique = this.StateName + this.Country;

                    _id = Lib.Security.Hash.ComputeIntCaseSensitive(unique); 
                }
                return _id; 
            }
            set
            {
                _id = value; 
            }

        }

 		public string StateName { get; set; }

 		public short Country { get; set; }
	}
}
