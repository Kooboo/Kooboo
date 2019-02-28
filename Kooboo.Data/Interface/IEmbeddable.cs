//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Interface
{
    public interface IEmbeddable : ITextObject
    {
        Guid OwnerObjectId { get; set; }

        byte OwnerConstType { get; set; }

        bool IsEmbedded { get; set; }
        
        int BodyHash { get; set; }

        string KoobooOpenTag { get; set; } 

        string Engine { get; set; }

        /// <summary>
        /// The item index of same embedded type object on the parent...
        /// </summary>
        int ItemIndex { get; set; }

        /// <summary>
        /// The dom html tag name of this object, it is script or style now. 
        /// </summary>
        string DomTagName { get;   }
         

    }
}
