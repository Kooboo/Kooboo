//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Authorization
{
  public static  class Hierarchy
    {

        public static uint GetInt(byte LevelOne, byte LevelTwo = 0, byte LevelThree = 0, byte LevelFour = 0 )
        {
            byte[] fourbytes = new byte[4];
            fourbytes[0] = LevelOne;
            fourbytes[1] = LevelTwo;
            fourbytes[2] = LevelThree;
            fourbytes[3] = LevelFour;

            return BitConverter.ToUInt32(fourbytes, 0);  
        }
       
        public static bool HasRights(uint actioncode, HashSet<uint> rights)
        {
            if (rights.Contains(actioncode))
            {
                return true; 
            } 
            // check each level up.  
            var bytes = BitConverter.GetBytes(actioncode);

            if(bytes[3] !=0)
            {
                bytes[3] = 0;
                uint upone = BitConverter.ToUInt32(bytes, 0);
                if (rights.Contains(upone))
                {
                    return true;
                }
            }

            if (bytes[2] !=0)
            {
                bytes[2] = 0;

                var uptwo = BitConverter.ToUInt32(bytes, 0);

                if (rights.Contains(uptwo))
                {
                    return true;
                }
            }
            
             
            if (bytes[1] !=0)
            {
                bytes[1] = 0; 
                var upthree = BitConverter.ToUInt32(bytes, 0);
                if (rights.Contains(upthree))
                {
                    return true;
                }
            }
             
            return false;
        }

    }
}
