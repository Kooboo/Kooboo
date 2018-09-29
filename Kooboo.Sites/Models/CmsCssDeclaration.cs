//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.Extensions;

namespace Kooboo.Sites.Models
{
  public  class CmsCssDeclaration : SiteObject
    {

      public CmsCssDeclaration()
      {
          this.ConstType = ConstObjectType.CssDeclaration;
      }

      private Guid _id;
      
      public override Guid Id
        {
            set { _id = value; }
            get
            {
                if (_id == default(Guid))
                {
                    _id = Kooboo.Data.IDGenerator.GetCmsCssDeclarationId(this.CmsCssRuleId, this.PropertyName);
                }
                return _id;
            }
        }

      public Guid CmsCssRuleId { get; set; }
           
      private string _propertyname;

      public string PropertyName
      {
          get
          {
              return _propertyname;
          }
          set
          {
              _propertyname = value;

              if (!string.IsNullOrEmpty(_propertyname))
              {
                  _propertyname = _propertyname.ToLower(); 
              } 
          }
      }

      public string Value { get; set; }

      public bool Important { get; set; }

      /// <summary>
      /// redundancy
      /// </summary>
      public Guid ParentStyleId { get; set; } 
  
      public override int GetHashCode()
      {
          string unique = this.PropertyName + this.Value + this.Important.ToString();

          return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
      }

    }
}
