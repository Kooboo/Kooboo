using Kooboo.Data.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using ExtensionAttribute = System.Runtime.CompilerServices.ExtensionAttribute;

namespace Kooboo.Web.Frontend.KScriptDefine
{
    public class KScriptToTsDefineConventer
    {
        class Define
        {
            public string Namespace { get; set; }
            public string Name { get; set; }
            public string Discription { get; set; }
            public List<Type> Extends { get; set; }
            public List<Property> Properties { get; set; }
            public List<Method> Methods { get; set; }
            public Dictionary<string, string> Enums { get; set; }
        }

        class Property
        {
            public string Type { get; set; }
            public string Name { get; set; }
            public string Discription { get; set; }
        }

        class Method
        {
            public string Name { get; set; }
            public string ReturnType { get; set; }
            public List<Param> Params { get; set; }
            public string Discription { get; set; }
        }

        class Param
        {
            public string Type { get; set; }
            public string Name { get; set; }
        }

        class Indentation
        {
        }

        readonly Dictionary<Type, Define> _defines = new Dictionary<Type, Define>();
        readonly Queue<Type> _queue = new Queue<Type>();

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

        static readonly string[] _skipMthods = new string[] { "GetType", "ToString", "Equals", "GetHashCode", "GetEnumerator" };
        static readonly string[] _skipNamespaces = new string[] { "System", "Jint" };

        readonly IEnumerable<MethodInfo> _extensionMethodInfos = Lib.Reflection.AssemblyLoader.AllAssemblies
                .SelectMany(s => s.GetTypes())
                .SelectMany(s => s.GetMethods())
                .Where(w => w.GetCustomAttributes(false).Any(a => a.GetType() == typeof(ExtensionAttribute)))
                .ToArray();

        public string Convent(Type type)
        {
            void Recursion(Type t)
            {
                if (IsSystemType(t)) return;

                var define = TypeToDefine(t);

                var extensionProperties = ExtensionToProperties(t);
                define.Properties?.AddRange(extensionProperties);

                _defines.Add(t, define);

                while (_queue.Any())
                {
                    var nextType = _queue.Dequeue();
                    if (_defines.ContainsKey(nextType)) continue;
                    Recursion(nextType);
                }
            }

            Recursion(type);



            return DefinesToString(type);
        }

        IEnumerable<Property> ExtensionToProperties(Type parentType)
        {
            var fields = parentType.GetRuntimeFields().Where(w => w.GetCustomAttribute(typeof(Data.Attributes.ExtensionAttribute)) != null && w.IsStatic);
            var extensions = new List<KeyValuePair<string, Type>>();

            foreach (var prop in fields)
            {
                extensions.AddRange(prop.GetValue(null) as KeyValuePair<string, Type>[]);
            }

            return extensions.Select(s => new Property
            {
                Name = CamelCaseName(s.Key),
                Type = TypeString(parentType, s.Value),
                Discription = null
            }).GroupBy(g => g.Name).Select(s => s.First()).ToList();
        }

        string DefinesToString(Type type)
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
                        var extendList = define.Extends.Where(w => _defines.ContainsKey(w)).Select(s => $"{GetNamespace(s)}{_defines[s].Name}");
                        var extends = extendList.Any() ? $"extends {string.Join(",", extendList)} " : string.Empty;
                        builder.AppendLine($"   {declare}interface {define.Name} {extends}{{");

                        if (define.Properties != null)
                        {
                            foreach (var item in define.Properties)
                            {
                                if (item.Discription != null)
                                {
                                    builder.AppendLine($"       /** {item.Discription} */");
                                }

                                builder.AppendLine($"       {item.Name}:{item.Type};");
                            }
                        }

                        if (define.Methods != null)
                        {
                            foreach (var item in define.Methods)
                            {
                                var @params = item.Params.Select(s => $"{s.Name}:{s.Type}");
                                builder.AppendLine($"       {item.Name}({string.Join(",", @params)}):{item.ReturnType};");
                            }
                        }

                    }
                    else
                    {
                        builder.AppendLine($"   {declare}enum {define.Name} {{");

                        foreach (var item in define.Enums)
                        {
                            builder.AppendLine($"       {item.Key}={item.Value},");
                        }
                    }

                    builder.AppendLine($"   }}");
                    builder.AppendLine();
                }

                if (!string.IsNullOrWhiteSpace(defines.Key))
                {
                    builder.AppendLine($"}}");
                }
            }

            return builder.ToString();
        }

        Define TypeToDefine(Type type)
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

        string GetTypeName(Type type)
        {
            var name = type.Name;

            if (type.IsAnsiClass && name.EndsWith("&"))
            {
                name = name.Substring(0, name.Length - 1);
            }

            return name;
        }

        Define ConvertClassOrInterface(Type type)
        {

            var properties = type.GetProperties()
                                .Where(p => p.GetMethod.IsPublic && p.GetCustomAttribute(typeof(IgnoreAttribute)) == null)
                                .Select(s => new Property
                                {
                                    Name = CamelCaseName(s.Name),
                                    Type = TypeString(type, s.PropertyType),
                                    Discription = GetDiscription(s)
                                }).GroupBy(g => g.Name).Select(s => s.First()).ToList();

            var methods = type.GetMethods()
                                .Where(p => p.IsPublic && !p.IsSpecialName && !_skipMthods.Contains(p.Name) && p.GetCustomAttribute(typeof(IgnoreAttribute)) == null)
                                .Union(_extensionMethodInfos.Where(w => w.GetParameters().FirstOrDefault()?.ParameterType == type))
                                .Select(s =>
                                {
                                    var defineType = s.GetCustomAttribute(typeof(DefineTypeAttribute)) as DefineTypeAttribute;
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

            var extends = new List<Type>();
            extends.AddRange(type.GetInterfaces());
            if (type.BaseType != null) extends.Add(type.BaseType);

            return new Define
            {
                Methods = methods,
                Properties = properties,
                Extends = extends,
                Discription = GetDiscription(type)
            };
        }

        Define ConvertEnum(Type type)
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

        static string CamelCaseName(string pascalCaseName)
        {
            return pascalCaseName[0].ToString().ToLower() + pascalCaseName.Substring(1);
        }

        string TypeString(Type parentType, Type type)
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

        string GetNamespace(Type type, bool suffix = true)
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

        static Type GetArrayOrEnumerableType(Type type)
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

        static Type GetNullableType(Type type)
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

        string ConvertType(Type typeToUse)
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

        bool IsSystemType(Type type)
        {
            if (type.FullName == null) return true;
            return _skipNamespaces.Any(a => type.FullName.StartsWith(a));
        }

        string GetDiscription(MemberInfo memberInfo)
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
