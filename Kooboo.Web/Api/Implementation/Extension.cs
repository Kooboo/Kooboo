//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Data;
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.Lib.Reflection;
using Kooboo.Sites.DataSources;
using Kooboo.Web.Areas.Admin.ViewModels;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Kooboo.Web.Api.Implementation
{
    public class ExtensionApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "Extension";
            }
        }

        public bool RequireSite
        {
            get
            {
                return false;
            }
        }

        public bool RequireUser
        {
            get
            {
                return false;
            }
        }

        public List<Data.Models.Dll> List(ApiCall call)
        {
            return Kooboo.Data.GlobalDb.Dlls.All().OrderBy(o => o.AssemblyName).ToList();
        }
        
        public List<Dll> Post(ApiCall call)
        {
            var formresult = Kooboo.Lib.NETMultiplePart.FormReader.ReadForm(call.Context.Request.PostData);

            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            var dlls = new List<Dll>();
            foreach (var item in formresult.Files)
            {
                Dll newdll = new Dll();
                newdll.AssemblyName = Path.GetFileNameWithoutExtension(item.FileName);
                newdll.Content = item.Bytes;

                var path = Path.Combine(tempDir, newdll.AssemblyName + ".dll");
                File.WriteAllBytes(path, newdll.Content);
                newdll.AssemblyVersion = AssemblyName.GetAssemblyName(path).Version.ToString();

                File.Delete(path);

                dlls.Add(newdll);
            }
            Directory.Delete(tempDir);

            foreach (var dll in dlls)
            {
                Data.GlobalDb.Dlls.AddOrUpdate(dll);
            }

            return dlls;

        }
        
        public List<DataSourceViewModel> Datasource(ApiCall call)
        {
            List<DataSourceViewModel> dataSourceList = new List<DataSourceViewModel>();

            foreach (var item in GlobalDb.DataMethodSettings.All().GroupBy(o => o.DeclareType))
            {
                List<DataMethodViewModel> dataSourceMethods = new List<DataMethodViewModel>();
                var methodlist = item.ToList();

                foreach (var methoditem in methodlist)
                {
                    DataMethodViewModel onemethod = new DataMethodViewModel
                    {
                        Id = methoditem.Id,
                        IsGlobal = true,
                        IsConfigurable = methoditem.Parameters.Any(),
                        MethodName = methoditem.MethodName,
                        DisplayName = methoditem.DisplayName,
                        OriginalMethodName = methoditem.OriginalMethodName,
                        MethodSignatureHash = methoditem.MethodSignatureHash,
                        Parameters = methoditem.Parameters
                    };
                    dataSourceMethods.Add(onemethod);
                }
                DataSourceViewModel datasource = new DataSourceViewModel
                {
                    Type = methodlist[0].DeclareType,
                    IsThridPartyDataSource = methodlist[0].IsThirdPartyType,
                    TypeHash = methodlist[0].DeclareTypeHash
                };

                var existing = dataSourceList.Find(o => o.TypeHash == datasource.TypeHash);
                if (existing != null)
                {
                    existing.Methods.AddRange(dataSourceMethods);
                }
                else
                {
                    datasource.Methods = dataSourceMethods;
                    dataSourceList.Add(datasource);
                }
            }

            return dataSourceList;
        }

        [Kooboo.Attributes.RequireParameters("ids")]
        public void Deletes(ApiCall call)
        {
            string json = call.GetValue("ids");
            if (string.IsNullOrEmpty(json))
            {
                json = call.Context.Request.Body;
            }

            List<Guid> ids = new List<Guid>();

            try
            {
                ids = Lib.Helper.JsonHelper.Deserialize<List<Guid>>(json);
            }
            catch (Exception)
            {
                //throw;
            }

            if (ids != null && ids.Count() > 0)
            {
                foreach (var item in ids)
                {
                    GlobalDb.Dlls.Delete(item);
                }
            }

        }

        public TypeTree TypeTree(ApiCall call)
        {
            var dlls = GlobalDb.Dlls.All().OrderBy(x => x.AssemblyName).ToList();
            var tree = new TypeTree();
            foreach (var dll in dlls)
            {
                var asmNode = new TypeTree();
                asmNode.Id = "asm-" + dll.AssemblyName;
                asmNode.Name = dll.AssemblyName;
                asmNode.Text = dll.AssemblyName + " (" + dll.AssemblyVersion + ")";
                asmNode.Icon = "fa fa-folder";
                asmNode.NodeType = NodeType.Assembly;

                var types = Assembly.Load(dll.Content).GetExportedTypes();

                CreateSubTree(types.ToList(), asmNode);

                tree.AddChild(asmNode);
            }

            return tree;
        }
        
        public static TypeTree CreateSubTree(List<Type> types, TypeTree root = null)
        {
            if (root == null)
            {
                root = new TypeTree();
            }

            foreach (var group in types.GroupBy(x => x.Namespace).OrderBy(x => x.Key))
            {
                var NameSpaceNode = new TypeTree
                {
                    Name = group.Key,
                    Text = group.Key,
                    Icon = "fa fa-folder icon-state-warning",
                    NodeType = NodeType.NameSpace
                };

                foreach (var type in group.OrderBy(x => x.Name))
                {
                    // Ignore types do not have parameterless constructors
                    if (!type.GetConstructors().Any(x => x.GetParameters().Length == 0))
                    {
                        continue;
                    }

                    // Ignore special types
                    if (typeof(Attribute).IsAssignableFrom(type)
                        || typeof(Exception).IsAssignableFrom(type))
                    {
                        continue;
                    }

                    var typeNode = new TypeTree
                    {
                        Id = type.FullName,
                        Name = AssemblyQualifiedNameWithoutVersion(type),
                        Text = type.Name,
                        Icon = "fa fa-file icon-state-warning",
                        NodeType = NodeType.Type
                    };

                    foreach (var method in Lib.Reflection.TypeHelper.GetPublicMethods(type).OrderBy(x => x.Name))
                    {
                        var parameters = method.GetParameters();
                        if (parameters.Any(p => p.IsIn || p.IsOut))
                        {
                            continue;
                        }

                        if (method.ReturnType == typeof(void))
                        {
                            continue;
                        }

                        var methodType = new TypeTree
                        {
                            Id = TypeHelper.GetMethodSignatureHash(method).ToString(),
                            Name = method.Name,
                            Text = method.Name, // GetMethodDisplayName(method),
                            Icon = "fa fa-flash icon-state-warning",
                            NodeType = NodeType.Method,
                            TypeName = type.Name,
                            TypeFullName = type.FullName,
                            TypeAssemblyQualifiedName = AssemblyQualifiedNameWithoutVersion(type)
                        };

                        typeNode.AddChild(methodType);
                    }

                    if (typeNode.Children.Count > 0)
                    {
                        NameSpaceNode.AddChild(typeNode);
                    }
                }

                if (NameSpaceNode.Children.Count > 0)
                {
                    root.AddChild(NameSpaceNode);
                }
            }
            return root;
        }


        static Dictionary<Type, string> _specialTypeDisplayNames = new Dictionary<Type, string>
        {
            { typeof(void), "void" },
            { typeof(Int16), "short" },
            { typeof(Int32), "int" },
            { typeof(Int64), "long" },
            { typeof(Single), "float" },
            { typeof(Double), "double" },
            { typeof(Decimal), "decimal" },
            { typeof(Boolean), "bool" }
        };

        private static string GetFriendlyTypeName(Type type)
        {
            if (_specialTypeDisplayNames.ContainsKey(type))
            {
                return _specialTypeDisplayNames[type];
            }

            if (type.IsGenericType)
            {
                var baseName = type.Name.Substring(0, type.Name.IndexOf('`'));
                var args = type.GetGenericArguments()
                               .Select(x => GetFriendlyTypeName(x));

                return baseName + "<" + String.Join(", ", args) + ">";
            }

            return type.Name;
        }

        public List<string> Thirdparty(ApiCall call)
        {
            return GlobalDb.DataMethodSettings.Query.Where(o => o.IsThirdPartyType).SelectAll().Select(o => o.MethodSignatureHash.ToString()).ToList(); 
        }

        [Kooboo.Attributes.RequireModel(typeof(List<TypeModel>))]
        public void ThirdPartyUpdate(ApiCall call)
        {
            var model = call.Context.Request.Model as List<TypeModel>;

            foreach (var type in model)
            {
                Type ClrType = TypeCache.GetType(type.FullName);
                bool IsThirdParty = !TypeHelper.HasInterface(ClrType, typeof(IDataSource));

                List<MethodInfo> TypeMethods = Lib.Reflection.TypeHelper.GetPublicMethods(ClrType).ToList();

                if (ClrType != null)
                {
                    foreach (var item in type.SelectedMethods)
                    {
                        DataMethodSetting MethodSetting = new DataMethodSetting();
                        MethodSetting.IsThirdPartyType = IsThirdParty;
                        MethodSetting.MethodSignatureHash = item.MethodHash;
                        MethodSetting.IsGlobal = true;
                        MethodSetting.DeclareType = ClrType.FullName;
                        MethodSetting.MethodName = item.Name;
                        MethodSetting.OriginalMethodName = item.Name;

                        var methodinfo = TypeHelper.GetRightMethodInfo(TypeMethods, item.Name, item.MethodHash);
                        if (methodinfo == null)
                        {
                            continue;
                        }
                        MethodSetting.IsStatic = methodinfo.IsStatic;
                        MethodSetting.IsVoid = (methodinfo.ReturnType == typeof(void));
                        MethodSetting.Parameters = DataSourceHelper.GetMethodParametes(methodinfo);
                        MethodSetting.ReturnType = methodinfo.ReturnType.FullName;

                        if (MethodSetting.Parameters.Count > 0)
                        {
                            MethodSetting.ParameterBinding = DataSourceHelper.GetDefaultBinding(MethodSetting.Parameters);
                        }

                        GlobalDb.DataMethodSettings.AddOrUpdate(MethodSetting);
                    }
                }
            }

        }

        public DataMethodSetting GetSetting(ApiCall call)
        {
            DataMethodSetting settings = GlobalDb.DataMethodSettings.Get(call.ObjectId);
            return settings; 
        }


        [Kooboo.Attributes.RequireModel(typeof(Dictionary<string, ParameterBinding>))]
        public void UpdateSetting(ApiCall call)
        {
            var bindings = call.Context.Request.Model as Dictionary<string, ParameterBinding>;

            var model = GlobalDb.DataMethodSettings.Get(call.ObjectId);
            if (model == null)
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Setting not found", call.Context)); 
            }
            model.ParameterBinding = bindings; 
            GlobalDb.DataMethodSettings.AddOrUpdate(model); 
        }



        private static Regex RemoveAssemblyVersion = new Regex(@", (Version|Culture|PublicKeyToken)=[^,\]]*(?=,|$|\])", RegexOptions.IgnoreCase);

        private static string AssemblyQualifiedNameWithoutVersion(Type type)
        {
            if (type.AssemblyQualifiedName == null)
                return null;
            return RemoveAssemblyVersion.Replace(type.AssemblyQualifiedName, String.Empty);
        }
    }
}
