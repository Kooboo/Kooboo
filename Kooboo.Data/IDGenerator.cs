//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.Extensions;

namespace Kooboo.Data
{
    public static class IDGenerator
    {
        public static Guid GetOrGenerate(string NameOrValueOrId, byte ConstType)
        {
            if (string.IsNullOrEmpty(NameOrValueOrId))
            {
                return System.Guid.NewGuid();
            } 
            Guid id; 
            if (System.Guid.TryParse(NameOrValueOrId, out id))
            {
                return id;
            }
            return Generate(NameOrValueOrId, ConstType);
        }

        public static Guid Generate(string NameOrValue, byte ConstType)
        {
            if (string.IsNullOrEmpty(NameOrValue))
            {
                return System.Guid.NewGuid();
            }
            else
            {
                string name = NameOrValue;
                if (ConstType == ConstObjectType.Route)
                {
                    name = name.RemoveCurlyBracketContent();
                    if (!name.StartsWith("/"))
                    {
                        name = "/" + name;
                    }
                }
                name = ConstType.ToString() + name;
                return Kooboo.Data.IDGenerator.GetId(name);
            }
        }

        public static Guid GetRouteId(string NameOrValue)
        {
            if (string.IsNullOrEmpty(NameOrValue))
            {
                NameOrValue = "/";
            }
            NameOrValue = NameOrValue.RemoveCurlyBracketContent();
            if (!NameOrValue.StartsWith("/"))
            {
                NameOrValue = "/" + NameOrValue;
            }
            return GetId(ConstObjectType.Route.ToString() + NameOrValue);
        }

        public static Guid GetResourceGroupId(string Name, byte ResourceType)
        {
            string unique = Name + ResourceType.ToString() + ConstObjectType.ResourceGroup.ToString();

            return Data.IDGenerator.GetId(unique);
        }

        public static Guid GetFolderId(string Name, byte FolderObjectConstType)
        {
            string unique = Name + ConstObjectType.Folder.ToString() + FolderObjectConstType.ToString();

            return Data.IDGenerator.GetId(unique);
        }
         
  
        public static Guid GetId(string key)
        {
            return Lib.Security.Hash.ComputeGuidIgnoreCase(key);  
        }

        public static string ConvertGuidToString(Guid id)
        {
            return id.ToString();
        }
   
        public static Guid GetDomainId(string topLevelDomain)
        {
            return  topLevelDomain.ToHashGuid();
        }
     
        public static Guid GetNewGuid()
        {
            return System.Guid.NewGuid();
        }
        
        public static Guid GetImageThumbNailId(Guid ImageId, int Height, int Width)
        {
            string uniquestring = ImageId.ToString() + Height.ToString() + Width.ToString();

            return uniquestring.ToHashGuid();
        }
         
        public static Guid GetRelationId(Guid objectxId, Guid objectyId)
        {
            string uniquestring = ConvertGuidToString(objectxId) + ConvertGuidToString(objectyId);
            return uniquestring.ToHashGuid();
        }
          
        public static Guid GetCmsCssDeclarationId(Guid CmsRuleId, string propertyname)
        {
            string uniquetext = CmsRuleId.ToString() + propertyname; 
            return uniquetext.ToHashGuid();
        }

        public static Guid GetRankingRuleId(byte siteObjectType, string comparetype, object compareValue)
        {
            string uniquestring = siteObjectType.ToString() + comparetype + compareValue.ToString();
            return uniquestring.ToHashGuid();
        }

        public static Guid GetExternalResourceId(Guid OwnerObjectId, string FullUrl, byte DestinationObjectType)
        {
            string unique = ConstObjectType.ExternalResource.ToString() + OwnerObjectId.ToString() + FullUrl + DestinationObjectType.ToString();
            return unique.ToHashGuid(); 
        }

        public static Guid GetAssemblyId(string AssemblyFullName)
        {
            return AssemblyFullName.ToHashGuid();
        }

  

        public static Guid GetDataSourceMethodSettingsId(string DataSourceName, Guid MethodSignagtureHash, string NewMethodName)
        {
            string uniquestring = DataSourceName + MethodSignagtureHash.ToString() + NewMethodName;

            return uniquestring.ToHashGuid();
        }
         
        public static Guid GetFormId(Guid OwnerObjectId, string koobooid)
        {
            string uniquestring;
            if (!string.IsNullOrEmpty(koobooid))
            {
                uniquestring = OwnerObjectId.ToString() + koobooid;
                return uniquestring.ToHashGuid();
            }
            else
            {
                /// this should not happen. 
                return GetNewGuid();
            }
        }

        public static Guid GetPageElementId(Guid OwnerObjectId, byte OwnerConstType, string KoobooId)
        {
            string unique = OwnerObjectId.ToString() + OwnerConstType.ToString() + KoobooId;

            return unique.ToHashGuid(); 

        }

    }
}
