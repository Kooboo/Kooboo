using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Helper
{
    public static class DatabaseColumnHelper
    {
        public static string ToFrontEndDataType(Type clrType)
        {
            // string, number, datetime, bool, undefined 
            if (clrType == typeof(Int16) || clrType == typeof(Int32) || clrType == typeof(Int64) || clrType == typeof(byte))
            {
                return "number";
            }
            else if (clrType == typeof(decimal) || clrType == typeof(double) || clrType == typeof(float))
            {
                return "number";
            }
            else if (clrType == typeof(DateTime))
            {
                return "datetime";
            }
            else if (clrType == typeof(bool))
            {
                return "bool";
            }
            else if (clrType == typeof(string) || clrType == typeof(Guid))
            {
                return "string";
            }
            return "string";
        }

        public static Type ToClrType(string type)
        {
            string lower = type.ToLower();

            if (lower == "datetime")
            {
                return typeof(System.DateTime);
            }
            else if (lower == "number")
            {
                return typeof(System.Double);
            }
            else if (lower == "bool")
            {
                return typeof(bool);
            }
            else if (lower == "string")
            {
                return typeof(string);
            }
            return typeof(string);
        }

        public static string DefaultDataTypeForControlType(string ControlType)
        {
            if (string.IsNullOrWhiteSpace(ControlType))
            {
                return "String";
            } 
            string lower = ControlType.ToLower();

            if (lower.Contains("date") || lower.Contains("time"))
            {
                return "DateTime";
            }
            else if (lower.Contains("number") || lower.Contains("int") || lower.Contains("long"))
            {
                return "number";
            }
            else if (lower.Contains("bool") || lower.Contains("yes"))
            {
                return "bool";
            }

            return "string";
             
            // displayName: Kooboo.text.component.controlType.textBox,
            //  value: "TextBox",
            //  dataType: "String"
            //},
            //{
            //  displayName: Kooboo.text.component.controlType.textArea,
            //  value: "TextArea",
            //  dataType: "String"
            //},
            //{
            //  displayName: Kooboo.text.component.controlType.wysiwygEditor,
            //  value: "Tinymce",
            //  dataType: "String"
            //},
            //{
            //  displayName: Kooboo.text.component.controlType.selection,
            //  value: "Selection",
            //  dataType: "Undefined"
            //},
            //{
            //  displayName: Kooboo.text.component.controlType.checkBox,
            //  value: "CheckBox",
            //  dataType: "Undefined"
            //},
            //{
            //  displayName: Kooboo.text.component.controlType.radioBox,
            //  value: "RadioBox",
            //  dataType: "Undefined"
            //},
            //{
            //  displayName: Kooboo.text.component.controlType.boolean,
            //  value: "Boolean",
            //  dataType: "Bool"
            //},
            //{
            //  displayName: Kooboo.text.component.controlType.dateTime,
            //  value: "DateTime",
            //  dataType: "DateTime"
            //},
            //{
            //  displayName: Kooboo.text.component.controlType.number,
            //  value: "Number",
            //  dataType: "Number"
            //} 
        }
    }



}
