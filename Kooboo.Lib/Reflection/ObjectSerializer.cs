using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Lib.Reflection
{

    public class ObjectSerializer<T>
    {
        public Dictionary<string, SetterInfo<T>> FieldSetter { get; set; } = new Dictionary<string, SetterInfo<T>>(StringComparer.OrdinalIgnoreCase);

        public ObjectSerializer()
        {
            var type = typeof(T);
            var allfields = GetPublicPropertyOrFields(type);
            foreach (var item in allfields)
            {
                var action = GetSetObjectValue<T>(item.Key, item.Value);
                SetterInfo<T> setter = new SetterInfo<T>
                {
                    Name = item.Key,
                    Action = action,
                    FieldType = item.Value
                };
                FieldSetter.Add(item.Key, setter);
            }
        }

        public T FromFieldValues(IDictionary<string, object> values)
        {
            var TypeValues = values.Select(o => new FieldValue() { Name = o.Key, Value = o.Value, Type = GetValueType(o.Value) }).ToList();
            return FromFieldValues(TypeValues);

            Type GetValueType(object value)
            {
                return value == null ? null : value.GetType();
            }
        }

        public T FromFieldValues(List<FieldValue> Values)
        {

            T result = Activator.CreateInstance<T>();

            foreach (var item in Values)
            {
                if (item.Value == null || item.Value == DBNull.Value)
                {
                    continue;
                }

                if (FieldSetter.TryGetValue(item.Name, out var setter))
                {
                    if (setter.FieldType == typeof(bool))
                    {
                        var value = item.Value.ToString();
                        if (value == "0" || value == "false")
                        {
                            setter.Action(result, false);
                        }
                        else
                        {
                            setter.Action(result, true);
                        }
                    }

                    else if (setter.FieldType.IsEnum)
                    {
                        if (item.Type == typeof(Int32) || item.Type == typeof(Int64) || item.Type == typeof(Int16))
                        {
                            setter.Action(result, item.Value);
                        }
                        else
                        {
                            var value = item.Value.ToString();
                            var obj = Lib.Helper.EnumHelper.GetEnum(setter.FieldType, value);

                            if (obj == null)
                            {
                                if (int.TryParse(value, out int enumvalue))
                                {
                                    setter.Action(result, enumvalue);
                                }
                            }
                            else
                            {
                                setter.Action(result, obj);
                            }

                        }

                    }
                    else
                    {
                        if (setter.FieldType != item.Type)
                        {
                            var convertValue = TypeHelper.ChangeType(item.Value, setter.FieldType);
                            setter.Action(result, convertValue);
                        }
                        else
                        {
                            setter.Action(result, item.Value);
                        }


                    }
                }
            }
            return result;

        }

        public Dictionary<string, Type> GetPublicPropertyOrFields(Type ClassType)
        {
            return TypeHelper.GetPublicFieldOrProperties(ClassType);
        }

        public Action<TValue, object> GetSetObjectValue<TValue>(string FieldName, Type fieldtype)
        {
            return TypeHelper.GetSetObjectValue<TValue>(FieldName, fieldtype);
        }
    }

    public class ObjectSerializer
    {
        public Type DestType { get; set; }

        public Dictionary<string, SetterInfo> FieldSetter { get; set; } = new Dictionary<string, SetterInfo>(StringComparer.OrdinalIgnoreCase);

        public ObjectSerializer(Type destType)
        {
            this.DestType = destType;

            var allfields = GetPublicPropertyOrFields(destType);

            foreach (var item in allfields)
            {

                var action = GetSetObjectValue(item.Key, DestType, item.Value);

                SetterInfo setter = new()
                {
                    Name = item.Key,
                    Action = action,
                    FieldType = item.Value
                };
                FieldSetter.Add(item.Key, setter);
            }
        }

        public object FromFieldValues(IDictionary<string, object> values)
        {
            var TypeValues = values.Select(o => new FieldValue() { Name = o.Key, Value = o.Value, Type = GetValueType(o.Value) }).ToList();
            return FromFieldValues(TypeValues);

            Type GetValueType(object value)
            {
                return value == null ? null : value.GetType();
            }
        }

        public object FromFieldValues(List<FieldValue> Values)
        {

            var result = Activator.CreateInstance(this.DestType);

            foreach (var item in Values)
            {
                if (item.Value == null || item.Value == DBNull.Value)
                {
                    continue;
                }

                if (FieldSetter.TryGetValue(item.Name, out var setter))
                {
                    if (setter.RealType == typeof(bool))
                    {
                        var value = item.Value.ToString();
                        if (value == "0" || value == "false")
                        {
                            setter.Action(result, false);
                        }
                        else
                        {
                            setter.Action(result, true);
                        }
                    }

                    else if (setter.RealType.IsEnum)
                    {
                        if (item.Type == typeof(Int32) || item.Type == typeof(Int64) || item.Type == typeof(Int16))
                        {
                            setter.Action(result, item.Value);
                        }
                        else
                        {
                            var value = item.Value.ToString();
                            var obj = Lib.Helper.EnumHelper.GetEnum(setter.RealType, value);

                            if (obj == null)
                            {
                                if (int.TryParse(value, out int enumvalue))
                                {
                                    setter.Action(result, enumvalue);
                                }
                            }
                            else
                            {
                                setter.Action(result, obj);
                            }

                        }

                    }
                    else
                    {
                        if (setter.FieldType != item.Type)
                        {
                            var convertValue = TypeHelper.ChangeType(item.Value, setter.FieldType);
                            setter.Action(result, convertValue);
                        }
                        else
                        {
                            setter.Action(result, item.Value);
                        }
                    }
                }
            }
            return result;

        }

        public Dictionary<string, Type> GetPublicPropertyOrFields(Type ClassType)
        {

            return TypeHelper.GetPublicFieldOrProperties(ClassType);


        }

        public Action<object, object> GetSetObjectValue(string FieldName, Type objType, Type fieldtype)
        {
            return Lib.Reflection.TypeHelper.GetSetObjectValue(FieldName, objType, fieldtype);

        }
    }


    public class SetterInfo<T>
    {
        public Type FieldType;
        public string Name;
        public Action<T, object> Action;

        private Type _realType;
        public Type RealType
        {
            get
            {
                if (_realType == null)
                {
                    if (FieldType != null)
                    {
                        if (FieldType.IsGenericType && FieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            _realType = Nullable.GetUnderlyingType(FieldType)!;
                        }
                        else
                        {
                            _realType = FieldType;
                        }
                    }

                }
                return _realType;
            }

        }
    }

    public class SetterInfo
    {
        public Type FieldType;

        private Type _realType;
        public Type RealType
        {
            get
            {
                if (_realType == null)
                {
                    if (FieldType != null)
                    {
                        if (FieldType.IsGenericType && FieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            _realType = Nullable.GetUnderlyingType(FieldType)!;
                        }
                        else
                        {
                            _realType = FieldType;
                        }
                    }

                }
                return _realType;
            }

        }

        public string Name;
        public Action<object, object> Action;
    }

    public record FieldValue
    {
        public string Name;
        public Type Type;
        public object Value;
    }
}
