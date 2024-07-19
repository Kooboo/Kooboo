//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using Kooboo.Dom.CSS.rawmodel;
using Kooboo.Dom.CSS.Tokens;

namespace Kooboo.Dom.CSS
{
    /// <summary>
    /// //The algorithms defined in this section produce high-level CSS objects from lower-level objects.
    /// this algorithms is based on: http://dev.w3.org/csswg/css-syntax/
    /// </summary>
    public class TokenParser
    {

        private Tokenizer tokenizer;

        /// <summary>
        /// 5.3. Parser Entry Points.  They assume that they are invoked on a token stream, 
        /// </summary>
        /// <param name="tokenizer"></param>
        public TokenParser(Tokenizer tokenizer)
        {
            this.tokenizer = tokenizer;
        }

        /// <summary>
        ///  5.3. Parser Entry Points . they may also be invoked on a string; if so, first perform input preprocessing to produce a code point stream
        /// </summary>
        /// <param name="cssText"></param>
        public TokenParser(string cssText)
        {
            this.tokenizer = new Tokenizer(cssText);
        }

        public TokenParser()
        {

        }


        private cssToken currentToken;
        private cssToken ConsumeNextToken()
        {
            currentToken = tokenizer.ConsumeNextToken();
            return currentToken;
        }

        private void ReconsumeToken()
        {
            tokenizer.ReconsumeToken();
        }

        /// <summary>
        /// 5.3.1. Parse a stylesheet. To parse a stylesheet from a stream of tokens:
        /// </summary>
        /// <returns></returns>
        public stylesheet ParseStyleSheet()
        {
            //Create a new stylesheet.
            stylesheet stylesheet = new stylesheet();
            //Consume a list of rules from the stream of tokens, with the top-level flag set.
            //Assign the returned value to the stylesheet’s value.
            stylesheet.items = ConsumeListOfRules(true);
            //Return the stylesheet.
            return stylesheet;
        }

        public stylesheet ParseStyleSheet(string cssText)
        {
            this.tokenizer = new Tokenizer(cssText);
            return ParseStyleSheet();
        }

        /// <summary>
        /// 5.4.1. Consume a list of rules
        /// </summary>
        /// <param name="tokenizer"></param>
        /// <param name="top_level_flag"></param>
        /// <returns></returns>
        private List<Rule> ConsumeListOfRules(bool top_level_flag)
        {
            //To consume a list of rules:

            //Create an initially empty list of rules.
            List<Rule> rulelist = new List<Rule>();

            cssToken token;

            while (true)
            {
                //Repeatedly consume the next input token:
                token = ConsumeNextToken();

                //<whitespace-token>
                if (token.Type == enumTokenType.whitespace)
                {
                    //Do nothing.
                }
                //<EOF-token>
                else if (token.Type == enumTokenType.EOF)
                {
                    //Return the list of rules.
                    return rulelist;
                }
                //<CDO-token>
                //<CDC-token>
                else if (token.Type == enumTokenType.CDC || token.Type == enumTokenType.CDO)
                {
                    //If the top-level flag is set, do nothing.
                    if (!top_level_flag)
                    {
                        //Otherwise, reconsume the current input token. Consume a qualified rule. If anything is returned, append it to the list of rules.
                        ReconsumeToken();
                        QualifiedRule rule = ConsumeQualifiedRule();
                        if (rule != null)
                        {
                            rulelist.Add(rule);
                        }
                    }

                }
                //<at-keyword-token>
                else if (token.Type == enumTokenType.at_keyword)
                {
                    //Reconsume the current input token. Consume an at-rule. If anything is returned, append it to the list of rules.
                    ReconsumeToken();
                    AtRule rule = ConsumeAtRule();

                    if (rule != null)
                    {
                        rulelist.Add(rule);
                    }


                }
                //anything else
                else
                {
                    //Reconsume the current input token. Consume a qualified rule. If anything is returned, append it to the list of rules.
                    ReconsumeToken();
                    QualifiedRule rule = ConsumeQualifiedRule();
                    if (rule != null)
                    {
                        rulelist.Add(rule);
                    }

                }

            }

        }

        /// <summary>
        ///  5.4.3. Consume a qualified rule
        /// </summary>
        /// <returns></returns>
        private QualifiedRule ConsumeQualifiedRule()
        {

            //To consume a qualified rule:
            //Create a new qualified rule with its prelude initially set to an empty list, and its value initially set to nothing.

            QualifiedRule rule = new QualifiedRule();
            // set operation is performed by CTOR. 

            int startindex = -1;

            cssToken token = null;
            while (true)
            {
                //Repeatedly consume the next input token:
                token = ConsumeNextToken();

                if (startindex == -1)
                {
                    startindex = token.startIndex;
                }

                //<EOF-token>
                if (token.Type == enumTokenType.EOF)
                {
                    //This is a parse error. Return nothing.
                    return null;
                }
                //<{-token>
                else if (token.Type == enumTokenType.curly_bracket_left)
                {
                    //Consume a simple block and assign it to the qualified rule’s block. Return the qualified rule.
                    SimpleBlock block = ConsumeSimpleBlock();

                    block.startindex = token.startIndex;

                    rule.block = block;

                    rule.startindex = startindex;
                    rule.endindex = block.endindex;

                    return rule;
                }

                //simple block with an associated token of <{-token>

                //???????TODO: this must be an mistake in the syntax paper.. 

                //Assign the block to the qualified rule’s block. Return the qualified rule.
                //anything else
                else
                {
                    //Reconsume the current input token. Consume a component value. Append the returned value to the qualified rule’s prelude.

                    ReconsumeToken();
                    ComponentValue value = ConsumeComponentValue();
                    rule.prelude.Add(value);

                }
            }
        }


        /// <summary>
        /// 5.4.7. Consume a simple block
        /// </summary>
        /// <returns></returns>
        private SimpleBlock ConsumeSimpleBlock()
        {

            //The ending token is the mirror variant of the current input token. (E.g. if it was called with <[-token>, the ending token is <]-token>.)

            enumTokenType endTokenType = enumTokenType.curly_bracket_right;

            if (currentToken.Type == enumTokenType.curly_bracket_left)
            {
                endTokenType = enumTokenType.curly_bracket_right;
            }
            else if (currentToken.Type == enumTokenType.round_bracket_left)
            {
                endTokenType = enumTokenType.round_bracket_right;
            }
            else if (currentToken.Type == enumTokenType.square_bracket_left)
            {
                endTokenType = enumTokenType.square_bracket_right;
            }

            //Create a simple block with its associated token set to the current input token and with a value with is initially an empty list.

            SimpleBlock simpleblock = new SimpleBlock();

            simpleblock.token = currentToken;

            cssToken token;
            while (true)
            {
                token = ConsumeNextToken();
                //Repeatedly consume the next input token and process it as follows:

                //<EOF-token>
                //ending token
                if (token.Type == enumTokenType.EOF || token.Type == endTokenType)
                {
                    //Return the block.
                    simpleblock.endindex = token.endIndex;
                    if (token.Type == enumTokenType.EOF)
                    {
                        simpleblock.endindex = simpleblock.endindex - 1;
                    }
                    return simpleblock;
                }
                //anything else
                else
                {
                    //Reconsume the current input token. Consume a component value and append it to the value of the block.
                    ReconsumeToken();
                    ComponentValue value = ConsumeComponentValue();
                    simpleblock.value.Add(value);
                }
            }
        }

        /// <summary>
        /// 5.4.6. Consume a component value
        /// </summary>
        /// <returns></returns>
        private ComponentValue ConsumeComponentValue()
        {

            //Consume the next input token.
            cssToken token = ConsumeNextToken();

            //If the current input token is a <{-token>, <[-token>, or <(-token>, consume a simple block and return it.
            if (token.Type == enumTokenType.curly_bracket_left || token.Type == enumTokenType.square_bracket_left || token.Type == enumTokenType.round_bracket_left)
            {
                SimpleBlock simpleblock = ConsumeSimpleBlock();
                simpleblock.startindex = token.startIndex;

                return simpleblock;
            }
            //Otherwise, if the current input token is a <function-token>, consume a function and return it.
            else if (token.Type == enumTokenType.function)
            {
                Function func = ConsumeFunction();
                func.startindex = token.startIndex;
                return func;
            }
            else
            {
                //Otherwise, return the current input token.
                PreservedToken preservedtoken = new PreservedToken();
                preservedtoken.token = token;
                preservedtoken.startindex = token.startIndex;
                preservedtoken.endindex = token.endIndex;
                return preservedtoken;
            }
        }


        /// <summary>
        /// 5.4.8. Consume a function
        /// </summary>
        /// <returns></returns>
        private Function ConsumeFunction()
        {

            //Create a function with a name equal to the value of the current input token, and with a value which is initially an empty list.

            /// before calling this method, the calling method must already check that current token is a function token. 
            function_token token = currentToken as function_token;

            Function func = new Function();
            func.name = token.Value;
            func.startindex = token.startIndex;

            cssToken nexttoken = null;
            while (true)
            {
                //Repeatedly consume the next input token and process it as follows:
                nexttoken = ConsumeNextToken();

                //<EOF-token>
                //<)-token>
                if (nexttoken.Type == enumTokenType.EOF || nexttoken.Type == enumTokenType.round_bracket_right)
                {
                    //Return the function.
                    func.endindex = nexttoken.endIndex;
                    if (nexttoken.Type == enumTokenType.EOF)
                    {
                        func.endindex = func.endindex - 1;
                    }
                    return func;
                }

                //anything else
                else
                {
                    //Reconsume the current input token. Consume a component value and append the returned value to the function’s value.
                    ReconsumeToken();
                    ComponentValue value = ConsumeComponentValue();
                    func.value.Add(value);

                }

            }

        }


        /// <summary>
        /// 5.4.2. Consume an at-rule
        /// </summary>
        /// <returns></returns>
        private AtRule ConsumeAtRule()
        {
            //To consume an at-rule:
            //Create a new at-rule with its name set to the value of the current input token, its prelude initially set to an empty list, and its value initially set to nothing.
            AtRule rule = new AtRule();

            int startindex = -1;

            cssToken token = null;
            while (true)
            {
                //Repeatedly consume the next input token:
                token = ConsumeNextToken();
                if (startindex == -1) { startindex = token.startIndex; }

                //<semicolon-token>
                //<EOF-token>
                if (token.Type == enumTokenType.semicolon || token.Type == enumTokenType.EOF)
                {
                    //Return the at-rule.
                    rule.startindex = startindex;
                    rule.endindex = token.endIndex;

                    if (token.Type == enumTokenType.EOF)
                    {
                        rule.endindex = rule.endindex - 1;
                    }
                    return rule;
                }
                //<{-token>
                else if (token.Type == enumTokenType.curly_bracket_left)
                {
                    //Consume a simple block and assign it to the at-rule’s block. Return the at-rule.
                    SimpleBlock simpleblock = ConsumeSimpleBlock();
                    simpleblock.startindex = token.startIndex;
                    rule.block = simpleblock;

                    rule.startindex = startindex;
                    rule.endindex = simpleblock.endindex;

                    return rule;
                }
                //simple block with an associated token of <{-token>
                //Assign the block to the at-rule’s block. Return the at-rule.

                ///TODO: ???? check what does this means??? 

                //anything else
                else
                {
                    //Reconsume the current input token. Consume a component value. Append the returned value to the at-rule’s prelude.
                    ReconsumeToken();
                    ComponentValue value = ConsumeComponentValue();
                    rule.prelude.Add(value);

                }
            }
        }

    }
}
