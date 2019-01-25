//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kooboo.Data.Context
{

    public class HeaderValueQuery
    {
        public string KeyOrExpression;
        /// <summary>
        /// check whether it is an expression or not. 
        /// </summary>
        public bool IsExpression { get; set; }

        public bool IsString { get; set; }

        public List<ItemQuery> ItemValues { get; set; }

        public bool RequireRender { get; set; }

        public HeaderValueQuery(string KeyOrExpression)
        {
            if (string.IsNullOrEmpty(KeyOrExpression))
            {
                return;
            }

            KeyOrExpression = KeyOrExpression.Trim();

            if (KeyOrExpression.IndexOf("{") > -1 && KeyOrExpression.IndexOf("}") > -1)
            {
                this.KeyOrExpression = KeyOrExpression;
                this.IsExpression = true;
                string regexpattern = @"\{(?<Name>.*?)\}";
                var matches = Regex.Matches(KeyOrExpression, regexpattern);

                int counter = 0;

                this.ItemValues = new List<ItemQuery>();
                foreach (Match item in matches)
                {
                    string value = item.Groups["Name"].Value;

                    ItemQuery itemquery = new ItemQuery();
                    itemquery.Query = new GetValueQuery(value);
                    this.ItemValues.Add(itemquery);

                    this.KeyOrExpression = this.KeyOrExpression.Replace("{" + value + "}", "{" + counter.ToString() + "}");
                    counter += 1;
                }
                this.RequireRender = true;

            }
            else
            {

                if (Lib.Helper.StringHelper.IsString(KeyOrExpression))
                {
                    this.IsString = true;
                    if (KeyOrExpression.StartsWith("'"))
                    {
                        this.KeyOrExpression = KeyOrExpression.Trim('\'');

                    }
                    else if (KeyOrExpression.StartsWith("\""))
                    {
                        this.KeyOrExpression = KeyOrExpression.Trim('"');
                    }
                    else
                    {
                        this.KeyOrExpression = KeyOrExpression;
                    }
                }
                else
                {
                    this.IsString = true;
                    this.KeyOrExpression = KeyOrExpression;
                }

            }
        }

        // the final output... 
        public string Render(RenderContext context)
        {
            if (this.IsString)
            {
                return this.KeyOrExpression;
            }
            else if (this.RequireRender)
            {
                foreach (var item in ItemValues)
                {
                    if (item.Value == null)
                    {
                        var objvalue = context.DataContext.GetValueByQuery(item.Query);
                        if (objvalue != null)
                        {
                            item.Value = objvalue.ToString();
                        }
                    }
                }
            }

            if (ItemValues != null && ItemValues.Any())
            {
                var valuePara = ItemValues.Select(o => o.Value).ToArray();
                return string.Format(this.KeyOrExpression, valuePara);
            }
            else
            {
                return this.KeyOrExpression;
            }
        }


        public void TryAssignValue(IDictionary data, RenderContext context)
        {

            if (this.RequireRender)
            {
                bool allhasvalue = true;

                foreach (var item in this.ItemValues)
                {
                    if (item.Value == null)
                    {
                        var objvalue = context.DataContext.GetValueFromStackItem(data, item.Query);
                        if (objvalue != null)
                        {
                            item.Value = objvalue.ToString();
                        }
                        else
                        {
                            allhasvalue = false;
                        }
                    }
                }

                if (allhasvalue)
                {
                    this.RequireRender = false;
                }

            }

        }

        public void InitValue(RenderContext context)
        {

            if (this.RequireRender)
            {

                bool allhasvalue = true;

                foreach (var item in this.ItemValues)
                {
                    if (item.Value == null)
                    {
                        var objvalue = context.DataContext.GetValueByQuery(item.Query);
                        if (objvalue != null)
                        {
                            item.Value = objvalue.ToString();
                        }
                        else
                        {
                            allhasvalue = false;
                        }
                    }
                }

                if (allhasvalue)
                {
                    this.RequireRender = false;
                }


            }



        }           

    }


    public class ItemQuery
    {
        public string Value { get; set; }

        public GetValueQuery Query { get; set; }
    }


}
