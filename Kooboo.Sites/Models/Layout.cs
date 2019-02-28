//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Extensions;
using Kooboo.Data;
using Kooboo.Data.Interface;
using Kooboo.Attributes; 

namespace Kooboo.Sites.Models
{
    [NameAsID]
    [Diskable(DiskType.Text)]
    public class Layout : DomObject, IExtensionable
    {

        private Guid _id;
        [Kooboo.Attributes.SummaryIgnore]
        public override Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    if (!string.IsNullOrEmpty(this.Name))
                    {
                        _id = Data.IDGenerator.Generate(this.Name, this.ConstType);
                    } 
                }
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        public override string Name
        {
            get
            {
                return base.Name;
            }

            set
            {
                base.Name = value;
                _id = default(Guid);
            }
        }

        public Layout()
        {
            this.ConstType = ConstObjectType.Layout; 
        }


        /// <summary>
        /// Layout extension,default:TAL
        /// </summary>
        [Kooboo.Attributes.SummaryIgnore]
        public string Extension { get; set; } = "html";

        public override int GetHashCode()
        {
            return Lib.Security.Hash.ComputeIntCaseSensitive(Body); 
        }
         
    }
}