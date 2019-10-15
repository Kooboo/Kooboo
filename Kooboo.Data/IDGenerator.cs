//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.Extensions;

namespace Kooboo.Data
{
    public static class IDGenerator
    {
        public static Guid GetOrGenerate(string nameOrValueOrId, byte constType)
        {
            if (string.IsNullOrEmpty(nameOrValueOrId))
            {
                return System.Guid.NewGuid();
            }

            if (System.Guid.TryParse(nameOrValueOrId, out var id))
            {
                return id;
            }
            return Generate(nameOrValueOrId, constType);
        }

        public static Guid Generate(string nameOrValue, byte constType)
        {
            if (string.IsNullOrEmpty(nameOrValue))
            {
                return System.Guid.NewGuid();
            }

            string name = nameOrValue;
            if (constType == ConstObjectType.Route)
            {
                name = name.RemoveCurlyBracketContent();
                if (!name.StartsWith("/"))
                {
                    name = "/" + name;
                }
            }
            name = constType.ToString() + name;
            return Kooboo.Data.IDGenerator.GetId(name);
        }

        public static Guid GetRouteId(string nameOrValue)
        {
            if (string.IsNullOrEmpty(nameOrValue))
            {
                nameOrValue = "/";
            }
            nameOrValue = nameOrValue.RemoveCurlyBracketContent();
            if (!nameOrValue.StartsWith("/"))
            {
                nameOrValue = "/" + nameOrValue;
            }
            return GetId(ConstObjectType.Route.ToString() + nameOrValue);
        }

        public static Guid GetResourceGroupId(string name, byte resourceType)
        {
            string unique = name + resourceType.ToString() + ConstObjectType.ResourceGroup.ToString();

            return Data.IDGenerator.GetId(unique);
        }

        public static Guid GetFolderId(string name, byte folderObjectConstType)
        {
            string unique = name + ConstObjectType.Folder.ToString() + folderObjectConstType.ToString();

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
        
        public static Guid GetImageThumbNailId(Guid imageId, int height, int width)
        {
            string uniquestring = imageId.ToString() + height.ToString() + width.ToString();

            return uniquestring.ToHashGuid();
        }
         
        public static Guid GetRelationId(Guid objectxId, Guid objectyId)
        {
            string uniquestring = ConvertGuidToString(objectxId) + ConvertGuidToString(objectyId);
            return uniquestring.ToHashGuid();
        }
          
        public static Guid GetCmsCssDeclarationId(Guid cmsRuleId, string propertyname)
        {
            string uniquetext = cmsRuleId.ToString() + propertyname; 
            return uniquetext.ToHashGuid();
        }

        public static Guid GetRankingRuleId(byte siteObjectType, string comparetype, object compareValue)
        {
            string uniquestring = siteObjectType.ToString() + comparetype + compareValue.ToString();
            return uniquestring.ToHashGuid();
        }

        public static Guid GetExternalResourceId(Guid ownerObjectId, string fullUrl, byte destinationObjectType)
        {
            string unique = ConstObjectType.ExternalResource.ToString() + ownerObjectId.ToString() + fullUrl + destinationObjectType.ToString();
            return unique.ToHashGuid(); 
        }

        public static Guid GetAssemblyId(string assemblyFullName)
        {
            return assemblyFullName.ToHashGuid();
        }

  

        public static Guid GetDataSourceMethodSettingsId(string dataSourceName, Guid methodSignagtureHash, string newMethodName)
        {
            string uniquestring = dataSourceName + methodSignagtureHash.ToString() + newMethodName;

            return uniquestring.ToHashGuid();
        }
         
        public static Guid GetFormId(Guid ownerObjectId, string koobooid)
        {
            if (!string.IsNullOrEmpty(koobooid))
            {
                var uniquestring = ownerObjectId.ToString() + koobooid;
                return uniquestring.ToHashGuid();
            }

            /// this should not happen. 
            return GetNewGuid();
        }

        public static Guid GetPageElementId(Guid ownerObjectId, byte ownerConstType, string koobooId)
        {
            string unique = ownerObjectId.ToString() + ownerConstType.ToString() + koobooId;

            return unique.ToHashGuid(); 

        }

    }
}
