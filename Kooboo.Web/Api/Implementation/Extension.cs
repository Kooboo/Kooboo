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
                Dll newdll = new Dll
                {
                    AssemblyName = Path.GetFileNameWithoutExtension(item.FileName), Content = item.Bytes
                };

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

            if (ids != null && ids.Any())
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
                var asmNode = new TypeTree
                {
                    Id = "asm-" + dll.AssemblyName,
                    Name = dll.AssemblyName,
                    Text = dll.AssemblyName + " (" + dll.AssemblyVersion + ")",
                    Icon = "fa fa-folder",
                    NodeType = NodeType.Assembly
                };

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
                var nameSpaceNode = new TypeTree
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
                        nameSpaceNode.AddChild(typeNode);
                    }
                }

                if (nameSpaceNode.Children.Count > 0)
                {
                    root.AddChild(nameSpaceNode);
                }
            }
            return root;
        }

        private static Dictionary<Type, string> _specialTypeDisplayNames = new Dictionary<Type, string>
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
                Type clrType = TypeCache.GetType(type.FullName);
                bool isThirdParty = !TypeHelper.HasInterface(clrType, typeof(IDataSource));

                List<MethodInfo> typeMethods = Lib.Reflection.TypeHelper.GetPublicMethods(clrType).ToList();

                if (clrType != null)
                {
                    foreach (var item in type.SelectedMethods)
                    {
                        DataMethodSetting methodSetting = new DataMethodSetting
                        {
                            IsThirdPartyType = isThirdParty,
                            MethodSignatureHash = item.MethodHash,
                            IsGlobal = true,
                            DeclareType = clrType.FullName,
                            MethodName = item.Name,
                            OriginalMethodName = item.Name
                        };

                        var methodinfo = TypeHelper.GetRightMethodInfo(typeMethods, item.Name, item.MethodHash);
                        if (methodinfo == null)
                        {
                            continue;
                        }
                        methodSetting.IsStatic = methodinfo.IsStatic;
                        methodSetting.IsVoid = (methodinfo.ReturnType == typeof(void));
                        methodSetting.Parameters = DataSourceHelper.GetMethodParametes(methodinfo);
                        methodSetting.ReturnType = methodinfo.ReturnType.FullName;

                        if (methodSetting.Parameters.Count > 0)
                        {
                            methodSetting.ParameterBinding = DataSourceHelper.GetDefaultBinding(methodSetting.Parameters);
                        }

                        GlobalDb.DataMethodSettings.AddOrUpdate(methodSetting);
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
            return type.AssemblyQualifiedName == null ? null : RemoveAssemblyVersion.Replace(type.AssemblyQualifiedName, String.Empty);
        }
    }
}