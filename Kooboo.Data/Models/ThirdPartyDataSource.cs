//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo;
using Kooboo.Extensions;

namespace Kooboo.Data.Models
{
    /// <summary>
    ///  third party dlls that did not implement IDatasource, but can be used directly as datasource to provide data for views. 
    /// </summary>
    public class ThirdPartyDataSource
    {

        private Guid _id;

        public Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    _id = Kooboo.Data.IDGenerator.GetId(this.Name);
                }
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        /// <summary>
        /// The full name of this datasource class name. 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The CLR type with full assembly information. 
        /// </summary>
        public string ClrType { get; set; }

        /// <summary>
        /// DataSource 所在的程序集。
        /// </summary>
        public string Assembly { get; set; }

    }

}
