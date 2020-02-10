using Kooboo.Data.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using ExtensionAttribute = System.Runtime.CompilerServices.ExtensionAttribute;

[assembly: InternalsVisibleTo("Kooboo.Web.Test")]
namespace Kooboo.Web.Frontend.KScriptDefine
{
    public class KScriptToTsDefineConventer
    {
        internal class Define
        {
            public string Namespace { get; set; }
            public string Name { get; set; }
            public string Discription { get; set; }
            public List<string> Extends { get; set; }
            public List<Property> Properties { get; set; }
            public List<Method> Methods { get; set; }
            public Dictionary<string, string> Enums { get; set; }
            public string ValueType { get; set; }
        }

        internal class Property
        {
            public string Type { get; set; }
            public string Name { get; set; }
            public string Discription { get; set; }
        }

        internal class Method
        {
            public string Name { get; set; }
            public string ReturnType { get; set; }
            public List<Param> Params { get; set; }
            public string Discription { get; set; }
        }

        internal class Param
        {
            public string Type { get; set; }
            public string Name { get; set; }
        }

        readonly string _indentation = "  ";

        static readonly string[] _skipMthods = new string[] { "GetType", "ToString", "Equals", "GetHashCode", "GetEnumerator" };
        static readonly string[] _skipNamespaces = new string[] { "System", "Jint", "Newtonsoft" };

        static readonly IDictionary<Type, string> _convertedTypes = new Dictionary<Type, string>()
        {
            [typeof(string)] = "string",
            [typeof(char)] = "string",
            [typeof(byte)] = "number",
            [typeof(sbyte)] = "number",
            [typeof(short)] = "number",
            [typeof(ushort)] = "number",
            [typeof(int)] = "number",
            [typeof(uint)] = "number",
            [typeof(long)] = "number",
            [typeof(ulong)] = "number",
            [typeof(float)] = "number",
            [typeof(double)] = "number",
            [typeof(decimal)] = "number",
            [typeof(bool)] = "boolean",
            [typeof(object)] = "any",
            [typeof(void)] = "void",
            [typeof(DateTime)] = "Date",
        };

        readonly Dictionary<string, Define> _defines = new Dictionary<string, Define>();
        readonly Queue<Type> _queue = new Queue<Type>();

        readonly IEnumerable<MethodInfo> _extensionMethodInfos = Lib.Reflection.AssemblyLoader.AllAssemblies
                .SelectMany(s => s.GetTypes())
                .SelectMany(s => s.GetMethods())
                .Where(w => IsExtension(w))
                .ToArray();

        public string Convent(Type type)
        {
            void Recursion(Type t)
            {
                if (IsSystemType(t)) return;

                var define = TypeToDefine(t);

                var extensionProperties = GetExtensionProperties(t);
                define.Properties?.AddRange(extensionProperties);

                _defines.Add(t.FullName, define);

                while (_queue.Any())
                {
                    var nextType = _queue.Dequeue();
                    if (_defines.ContainsKey(nextType.FullName)) continue;
                    Recursion(nextType);
                }
            }

            Recursion(type);

            return DefinesToString(type);
        }

        internal IEnumerable<Property> GetExtensionProperties(Type type)
        {
            var fields = type.GetRuntimeFields().Where(w => IsKExtension(w) && w.IsStatic);
            var properties = new List<Property>();
            var extensions = new List<KeyValuePair<string, Type>>();

            foreach (var prop in fields)
            {
                var value = prop.GetValue(null);
                if (value is KeyValuePair<string, Type>[] items)
                {
                    properties.AddRange(items.Select(s => new Property
                    {
                        Name = CamelCaseName(s.Key),
                        Type = TypeString(type, s.Value),
                        Discription = null
                    }));
                }
                else if (value is KeyValuePair<string, Type[]>[] dynamicItems)
                {
                    foreach (var item in dynamicItems)
                    {
                        properties.Add(new Property
                        {
                            Name = item.Key,
                            Discription = null,
                            Type = item.Key
                        });

                        _defines.Add(item.Key, new Define
                        {
                            Name = item.Key,
                            Properties = item.Value.Select(s => new Property
                            {
                                Name = CamelCaseName(s.Name),
                                Type = TypeString(type, s)
                            }).ToList(),
                            Extends = new List<string>(),
                            Namespace = GetNamespace(type, false)
                        });
                    }
                }
            }

            return properties.GroupBy(g => g.Name).Select(s => s.Last()).ToList();
        }

        internal string DefinesToString(Type type)
        {
            var builder = new StringBuilder();

            builder.AppendLine($"declare const k: {GetNamespace(type)}{type.Name};");
            foreach (var defines in _defines.GroupBy(global => global.Value.Namespace))
            {
                if (!string.IsNullOrWhiteSpace(defines.Key))
                {
                    builder.AppendLine($"declare namespace {defines.Key} {{");
                }

                foreach (var group in defines)
                {
                    var define = group.Value;
                    var declare = string.IsNullOrWhiteSpace(define.Namespace) ? "declare " : string.Empty;

                    if (define.Enums == null)
                    {
                        var extends = define.Extends.Any() ? $"extends {string.Join(",", define.Extends)} " : string.Empty;
                        builder.AppendLine($"{_indentation}{declare}interface {define.Name} {extends}{{");

                        if (define.ValueType != null)
                        {
                            builder.AppendLine($"{_indentation}{_indentation}[key:string]:{define.ValueType};");
                        }

                        if (define.Properties != null)
                        {
                            foreach (var item in define.Properties)
                            {
                                if (item.Discription != null)
                                {
                                    builder.AppendLine($"{_indentation}{_indentation}/** {item.Discription} */");
                                }

                                builder.AppendLine($"{_indentation}{_indentation}{item.Name}:{item.Type};");
                            }
                        }

                        if (define.Methods != null)
                        {
                            foreach (var item in define.Methods)
                            {

                                if (item.Discription != null)
                                {
                                    builder.AppendLine($"{_indentation}{_indentation}/** {item.Discription} */");
                                }
                                var @params = item.Params.Select(s => $"{s.Name}:{s.Type}");
                                builder.AppendLine($"{_indentation}{_indentation}{item.Name}({string.Join(",", @params)}):{item.ReturnType};");
                            }
                        }

                    }
                    else
                    {
                        builder.AppendLine($"{_indentation}{declare}enum {define.Name} {{");

                        foreach (var item in define.Enums)
                        {
                            builder.AppendLine($"{_indentation}{_indentation}{item.Key}={item.Value},");
                        }
                    }

                    builder.AppendLine($"{_indentation}}}");
                    builder.AppendLine();
                }

                if (!string.IsNullOrWhiteSpace(defines.Key))
                {
                    builder.AppendLine($"}}");
                }
            }

            return builder.ToString();
        }

        internal Define TypeToDefine(Type type)
        {
            Define define;

            if (type.IsClass || type.IsInterface || type.IsValueType)
            {
                define = ConvertClassOrInterface(type);
            }
            else if (type.IsEnum)
            {
                define = ConvertEnum(type);
            }
            else
            {
                throw new InvalidOperationException();
            }

            define.Name = GetTypeName(type);
            define.Namespace = GetNamespace(type, false);
            return define;
        }

        internal string GetTypeName(Type type)
        {
            var name = type.Name;

            if (type.IsAnsiClass && name.EndsWith("&"))
            {
                name = name.Substring(0, name.Length - 1);
            }

            return name;
        }

        internal List<Property> GetProperties(Type type)
        {
            return type.GetProperties()
                       .Where(p => p.GetMethod.IsPublic && p.GetCustomAttribute(typeof(KIgnoreAttribute)) == null)
                       .Select(s => new Property
                       {
                           Name = CamelCaseName(s.Name),
                           Type = TypeString(type, s.PropertyType),
                           Discription = GetDiscription(s)
                       }).GroupBy(g => g.Name).Select(s => s.First()).ToList();

        }

        internal List<Method> GetMethods(Type type)
        {
            return type.GetMethods()
                       .Where(p => p.IsPublic && !p.IsSpecialName && !_skipMthods.Contains(p.Name) && p.GetCustomAttribute(typeof(KIgnoreAttribute)) == null)
                       .Union(_extensionMethodInfos.Where(w => w.GetParameters().FirstOrDefault()?.ParameterType == type))
                       .Select(s =>
                       {
                           var defineType = s.GetCustomAttribute(typeof(KDefineTypeAttribute)) as KDefineTypeAttribute;
                           var returnType = defineType?.Return ?? s.ReturnType;
                           var @params = new List<Param>();
                           var defineParams = defineType?.Params?.GetEnumerator();
                           var isExtension = s.GetCustomAttributes(false).Any(a => a.GetType() == typeof(ExtensionAttribute));
                           var rawParams = isExtension ? s.GetParameters().Skip(1) : s.GetParameters();

                           foreach (var item in rawParams)
                           {
                               if (!defineParams?.MoveNext() ?? false) defineParams = null;
                               var itemType = (Type)defineParams?.Current ?? item.ParameterType;
                               var param = new Param { Name = item.Name, Type = TypeString(type, itemType) };
                               @params.Add(param);
                           }

                           return new Method
                           {
                               Name = CamelCaseName(s.Name),
                               Params = @params,
                               ReturnType = TypeString(type, returnType),
                               Discription = GetDiscription(s)
                           };
                       }).ToList();
        }

        internal List<string> GetExtends(Type type)
        {
            var extends = new List<Type>();
            extends.AddRange(type.GetInterfaces());
            if (type.BaseType != null) extends.Add(type.BaseType);
            return extends.Select(s => TypeString(type, s)).ToList();
        }

        internal Define ConvertClassOrInterface(Type type)
        {
            var valueType = type.GetCustomAttributesData().FirstOrDefault(f => f.AttributeType == typeof(KValueTypeAttribute))?.ConstructorArguments?[0].Value as Type;

            return new Define
            {
                Methods = GetMethods(type),
                Properties = GetProperties(type),
                Extends = GetExtends(type),
                Discription = GetDiscription(type),
                ValueType = valueType == null ? null : TypeString(type, valueType)
            };
        }

        internal Define ConvertEnum(Type type)
        {
            var enumValues = type.GetEnumValues().Cast<int>().ToArray();
            var enumNames = type.GetEnumNames();
            var enums = new Dictionary<string, string>();

            for (var i = 0; i < enumValues.Length; i++)
            {
                enums.Add(enumNames[i], enumValues[i].ToString());
            }

            return new Define
            {
                Name = type.Name,
                Enums = enums
            };
        }

        internal static string CamelCaseName(string pascalCaseName)
        {
            return pascalCaseName[0].ToString().ToLower() + pascalCaseName.Substring(1);
        }

        internal string TypeString(Type parentType, Type type)
        {
            var arrayType = GetArrayOrEnumerableType(type);
            var nullableType = GetNullableType(type);
            var typeToUse = nullableType ?? arrayType ?? type;
            var @namespace = GetNamespace(typeToUse);
            var parentNamespace = GetNamespace(parentType);
            if (parentNamespace == @namespace) @namespace = string.Empty;
            var suffix = "";
            suffix = arrayType != null ? "[]" : suffix;
            suffix = nullableType != null ? "|null" : suffix;
            return $"{@namespace}{ConvertType(typeToUse)}{suffix}";
        }

        internal static string GetNamespace(Type type, bool suffix = true)
        {
            if (type.FullName == null || type.IsGenericType || IsSystemType(type)) return string.Empty;
            var arr = type.FullName.Split('.');
            var @namespace = string.Join(".", arr.Take(arr.Length - 1));

            if (suffix)
            {
                @namespace = string.IsNullOrWhiteSpace(@namespace) ? string.Empty : $"{@namespace}.";
            }

            return @namespace;
        }

        internal static Type GetArrayOrEnumerableType(Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType();
            }

            else if (type.IsConstructedGenericType)
            {
                var typeArgument = type.GenericTypeArguments.First();

                if (typeof(IEnumerable<>).MakeGenericType(typeArgument).IsAssignableFrom(type))
                {
                    return typeArgument;
                }
            }

            return null;
        }

        internal static Type GetNullableType(Type type)
        {
            if (type.IsConstructedGenericType)
            {
                var typeArgument = type.GenericTypeArguments.First();

                if (typeArgument.IsValueType && typeof(Nullable<>).MakeGenericType(typeArgument).IsAssignableFrom(type))
                {
                    return typeArgument;
                }
            }

            return null;
        }

        internal string ConvertType(Type typeToUse)
        {
            if (_convertedTypes.ContainsKey(typeToUse))
            {
                return _convertedTypes[typeToUse];
            }

            if (typeToUse.IsGenericType || IsSystemType(typeToUse))
            {
                return "any";
            }

            if (typeToUse.IsConstructedGenericType && typeToUse.GetGenericTypeDefinition() == typeof(IDictionary<,>))
            {
                var keyType = typeToUse.GenericTypeArguments[0];
                var valueType = typeToUse.GenericTypeArguments[1];
                return $"{{ [key: {ConvertType(keyType)}]: {ConvertType(valueType)} }}";
            }

            _queue.Enqueue(typeToUse);

            return GetTypeName(typeToUse);
        }

        internal static bool IsSystemType(Type type)
        {
            if (type.FullName == null) return true;
            return _skipNamespaces.Any(a => type.FullName.StartsWith(a));
        }

        internal static bool IsKExtension(MemberInfo type)
        {
            return type.CustomAttributes.Any(a => a.AttributeType == typeof(KExtensionAttribute));
        }

        internal static bool IsExtension(MemberInfo type)
        {
            return type.CustomAttributes.Any(a => a.AttributeType == typeof(ExtensionAttribute));
        }

        internal static string GetDiscription(MemberInfo memberInfo)
        {
            var atr = memberInfo.GetCustomAttribute(typeof(DescriptionAttribute));
            if (atr is DescriptionAttribute discription)
            {
                return discription.Description;
            }
            return null;
        }
    }
}