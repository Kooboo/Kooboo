//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

namespace Kooboo.Lib.Helper
{
    public static class JsonHelper
    {
        private static JsonSerializerSettings camelsetting = new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() };

        public static string Serialize(object Model)
        {
            return JsonConvert.SerializeObject(Model, Formatting.Indented, camelsetting);
        }

        public static string SerializeCaseSensitive(object model)
        {
            return JsonConvert.SerializeObject(model, Formatting.Indented);
        }

        public static string SerializeCaseSensitive(object model, params JsonConverter[] jsonConverters)
        {
            return JsonConvert.SerializeObject(model, Formatting.Indented, jsonConverters);
        }

        public static T Deserialize<T>(string json)
        {
            if (json[0] == 65279)
            {
                json = json.Substring(1);
            }
            return JsonConvert.DeserializeObject<T>(json, camelsetting);
        }

        public static JObject DeserializeJObject(string json)
        {
            if (json[0] == 65279)
            {
                json = json.Substring(1);
            }
            return (JObject)JsonConvert.DeserializeObject(json);
        }

        public static object DeserialzeBaseObject(string json)
        {
            if (json[0] == 65279)
            {
                json = json.Substring(1);
            }
            return JsonConvert.DeserializeObject(json);
        }

        public static object DeserializeObject(string json)
        {
            if (json[0] == 65279)
            {
                json = json.Substring(1);
            }

            return JsonConvert.DeserializeObject(json);
        }

        public static object Deserialize(string json)
        {
            if (json[0] == 65279)
            {
                json = json.Substring(1);
            }

            return JsonConvert.DeserializeObject(json);
        }

        public static object Deserialize(string json, Type type)
        {
            if (json[0] == 65279)
            {
                json = json.Substring(1);
            }
            return JsonConvert.DeserializeObject(json, type);
        }

        public static string GetString(JObject jobject, string memberName)
        {
            var token = GetObject(jobject, memberName);
            if (token == null)
            {
                return null;
            }
            else
            { return token.ToString(); }
        }

        public static Object GetObject(JObject jobject, string memberName)
        {
            if (jobject == null)
            {
                return null;
            }
            var token = jobject[memberName];
            if (token == null)
            {
                string lower = memberName.ToLower();

                foreach (var item in jobject.Properties().ToList())
                {
                    if (item.Name.ToLower() == lower)
                    {
                        token = item.Value;
                    }
                }
            }
            return token;
        }

        public static bool IsJson(string input)
        {
            if (input == null)
            {
                return false;
            }

            return JsonValidatorTool.JsonValidator.IsJson(input);
        }
    }

    public class IntJsonConvert : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            //return typeof(int) == objectType || typeof(Int16) == objectType
            //    || typeof(Int32) == objectType || typeof(Int64) == objectType
            //    ||typeof(Double)==objectType;
            return typeof(Double) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteValue(0);
            }
            else
            {
                //ensure int not be converted to double
                if (int.TryParse(value.ToString(), out var intvalue))
                {
                    writer.WriteValue(intvalue);
                }
                else
                {
                    writer.WriteValue(value);
                }

            }

        }
    }

    public class FormatedDateTimeConverter : Newtonsoft.Json.Converters.IsoDateTimeConverter
    {
        public FormatedDateTimeConverter(string dateTimeFormat) : this(dateTimeFormat, DateTimeStyles.RoundtripKind)
        {
        }

        public FormatedDateTimeConverter(string dateTimeFormat, DateTimeStyles dateTimeStyles)
        {
            DateTimeFormat = dateTimeFormat;
            DateTimeStyles = dateTimeStyles;
        }
    }
}


namespace JsonValidatorTool
{

    internal sealed class JsonChecker
    {
        private int _state;
        private long _offset;
        private readonly int _depth;
        private readonly Stack<Mode> _stack;

        private const int __ = -1;     /* the universal error code */

        /*
            Characters are mapped into these 31 character classes. This allows for
            a significant reduction in the size of the state transition table.
        */

        private const int C_SPACE = 0;  /* space */
        private const int C_WHITE = 1;  /* other whitespace */
        private const int C_LCURB = 2;  /* {  */
        private const int C_RCURB = 3;  /* } */
        private const int C_LSQRB = 4;  /* [ */
        private const int C_RSQRB = 5;  /* ] */
        private const int C_COLON = 6;  /* : */
        private const int C_COMMA = 7;  /* ; */
        private const int C_QUOTE = 8;  /* " */
        private const int C_BACKS = 9;  /* \ */
        private const int C_SLASH = 10;  /* / */
        private const int C_PLUS = 11;   /* + */
        private const int C_MINUS = 12;  /* - */
        private const int C_POINT = 13;  /* . */
        private const int C_ZERO = 14;  /* 0 */
        private const int C_DIGIT = 15;  /* 123456789 */
        private const int C_LOW_A = 16;  /* a */
        private const int C_LOW_B = 17;  /* b */
        private const int C_LOW_C = 18;  /* c */
        private const int C_LOW_D = 19;  /* d */
        private const int C_LOW_E = 20;  /* e */
        private const int C_LOW_F = 21;  /* f */
        private const int C_LOW_L = 22;  /* l */
        private const int C_LOW_N = 23;  /* n */
        private const int C_LOW_R = 24;  /* r */
        private const int C_LOW_S = 25;  /* s */
        private const int C_LOW_T = 26;  /* t */
        private const int C_LOW_U = 27;  /* u */
        private const int C_ABCDF = 28;  /* ABCDF */
        private const int C_E = 29;      /* E */
        private const int C_ETC = 30;    /* everything else */
        private const int NR_CLASSES = 31;

        private static readonly int[] ascii_class = new int[128]
        {
        /*
            This array maps the 128 ASCII characters into character classes.
            The remaining Unicode characters should be mapped to C_ETC.
            Non-whitespace control characters are errors.
        */
            __,      __,      __,      __,      __,      __,      __,      __,
            __,      C_WHITE, C_WHITE, __,      __,      C_WHITE, __,      __,
            __,      __,      __,      __,      __,      __,      __,      __,
            __,      __,      __,      __,      __,      __,      __,      __,

            C_SPACE, C_ETC,   C_QUOTE, C_ETC,   C_ETC,   C_ETC,   C_ETC,   C_ETC,
            C_ETC,   C_ETC,   C_ETC,   C_PLUS,  C_COMMA, C_MINUS, C_POINT, C_SLASH,
            C_ZERO,  C_DIGIT, C_DIGIT, C_DIGIT, C_DIGIT, C_DIGIT, C_DIGIT, C_DIGIT,
            C_DIGIT, C_DIGIT, C_COLON, C_ETC,   C_ETC,   C_ETC,   C_ETC,   C_ETC,

            C_ETC,   C_ABCDF, C_ABCDF, C_ABCDF, C_ABCDF, C_E,     C_ABCDF, C_ETC,
            C_ETC,   C_ETC,   C_ETC,   C_ETC,   C_ETC,   C_ETC,   C_ETC,   C_ETC,
            C_ETC,   C_ETC,   C_ETC,   C_ETC,   C_ETC,   C_ETC,   C_ETC,   C_ETC,
            C_ETC,   C_ETC,   C_ETC,   C_LSQRB, C_BACKS, C_RSQRB, C_ETC,   C_ETC,

            C_ETC,   C_LOW_A, C_LOW_B, C_LOW_C, C_LOW_D, C_LOW_E, C_LOW_F, C_ETC,
            C_ETC,   C_ETC,   C_ETC,   C_ETC,   C_LOW_L, C_ETC,   C_LOW_N, C_ETC,
            C_ETC,   C_ETC,   C_LOW_R, C_LOW_S, C_LOW_T, C_LOW_U, C_ETC,   C_ETC,
            C_ETC,   C_ETC,   C_ETC,   C_LCURB, C_ETC,   C_RCURB, C_ETC,   C_ETC
        };

        /*
            The state codes.
        */

        private const int GO = 00;  /* start    */
        private const int OK = 01;  /* ok       */
        private const int OB = 02;  /* object   */
        private const int KE = 03;  /* key      */
        private const int CO = 04;  /* colon    */
        private const int VA = 05;  /* value    */
        private const int AR = 06;  /* array    */
        private const int ST = 07;  /* string   */
        private const int ES = 08;  /* escape   */
        private const int U1 = 09;  /* u1       */
        private const int U2 = 10;  /* u2       */
        private const int U3 = 11;  /* u3       */
        private const int U4 = 12;  /* u4       */
        private const int MI = 13;  /* minus    */
        private const int ZE = 14;  /* zero     */
        private const int IN = 15;  /* integer  */
        private const int FR = 16;  /* fraction */
        private const int E1 = 17;  /* e        */
        private const int E2 = 18;  /* ex       */
        private const int E3 = 19;  /* exp      */
        private const int T1 = 20;  /* tr       */
        private const int T2 = 21;  /* tru      */
        private const int T3 = 22;  /* true     */
        private const int F1 = 23;  /* fa       */
        private const int F2 = 24;  /* fal      */
        private const int F3 = 25;  /* fals     */
        private const int F4 = 26;  /* false    */
        private const int N1 = 27;  /* nu       */
        private const int N2 = 28;  /* nul      */
        private const int N3 = 29;  /* null     */
        private const int NR_STATES = 30;


        private static readonly int[,] state_transition_table = new int[NR_STATES, NR_CLASSES]
        {
        /*
        The state transition table takes the current state and the current symbol,
        and returns either a new state or an action. An action is represented as a
        negative number. A JSON text is accepted if at the end of the text the
        state is OK and if the mode is Done.

                             white                                      1-9                                   ABCDF  etc
                         space |  {  }  [  ]  :  ,  "  \  /  +  -  .  0  |  a  b  c  d  e  f  l  n  r  s  t  u  |  E  |*/
            /*start  GO*/ {GO,GO,-6,__,-5,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__},
            /*ok     OK*/ {OK,OK,__,-8,__,-7,__,-3,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__},
            /*object OB*/ {OB,OB,__,-9,__,__,__,__,ST,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__},
            /*key    KE*/ {KE,KE,__,__,__,__,__,__,ST,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__},
            /*colon  CO*/ {CO,CO,__,__,__,__,-2,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__},
            /*value  VA*/ {VA,VA,-6,__,-5,__,__,__,ST,__,__,__,MI,__,ZE,IN,__,__,__,__,__,F1,__,N1,__,__,T1,__,__,__,__},
            /*array  AR*/ {AR,AR,-6,__,-5,-7,__,__,ST,__,__,__,MI,__,ZE,IN,__,__,__,__,__,F1,__,N1,__,__,T1,__,__,__,__},
            /*string ST*/ {ST,__,ST,ST,ST,ST,ST,ST,-4,ES,ST,ST,ST,ST,ST,ST,ST,ST,ST,ST,ST,ST,ST,ST,ST,ST,ST,ST,ST,ST,ST},
            /*escape ES*/ {__,__,__,__,__,__,__,__,ST,ST,ST,__,__,__,__,__,__,ST,__,__,__,ST,__,ST,ST,__,ST,U1,__,__,__},
            /*u1     U1*/ {__,__,__,__,__,__,__,__,__,__,__,__,__,__,U2,U2,U2,U2,U2,U2,U2,U2,__,__,__,__,__,__,U2,U2,__},
            /*u2     U2*/ {__,__,__,__,__,__,__,__,__,__,__,__,__,__,U3,U3,U3,U3,U3,U3,U3,U3,__,__,__,__,__,__,U3,U3,__},
            /*u3     U3*/ {__,__,__,__,__,__,__,__,__,__,__,__,__,__,U4,U4,U4,U4,U4,U4,U4,U4,__,__,__,__,__,__,U4,U4,__},
            /*u4     U4*/ {__,__,__,__,__,__,__,__,__,__,__,__,__,__,ST,ST,ST,ST,ST,ST,ST,ST,__,__,__,__,__,__,ST,ST,__},
            /*minus  MI*/ {__,__,__,__,__,__,__,__,__,__,__,__,__,__,ZE,IN,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__},
            /*zero   ZE*/ {OK,OK,__,-8,__,-7,__,-3,__,__,__,__,__,FR,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__},
            /*int    IN*/ {OK,OK,__,-8,__,-7,__,-3,__,__,__,__,__,FR,IN,IN,__,__,__,__,E1,__,__,__,__,__,__,__,__,E1,__},
            /*frac   FR*/ {OK,OK,__,-8,__,-7,__,-3,__,__,__,__,__,__,FR,FR,__,__,__,__,E1,__,__,__,__,__,__,__,__,E1,__},
            /*e      E1*/ {__,__,__,__,__,__,__,__,__,__,__,E2,E2,__,E3,E3,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__},
            /*ex     E2*/ {__,__,__,__,__,__,__,__,__,__,__,__,__,__,E3,E3,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__},
            /*exp    E3*/ {OK,OK,__,-8,__,-7,__,-3,__,__,__,__,__,__,E3,E3,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__},
            /*tr     T1*/ {__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,T2,__,__,__,__,__,__},
            /*tru    T2*/ {__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,T3,__,__,__},
            /*true   T3*/ {__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,OK,__,__,__,__,__,__,__,__,__,__},
            /*fa     F1*/ {__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,F2,__,__,__,__,__,__,__,__,__,__,__,__,__,__},
            /*fal    F2*/ {__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,F3,__,__,__,__,__,__,__,__},
            /*fals   F3*/ {__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,F4,__,__,__,__,__},
            /*false  F4*/ {__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,OK,__,__,__,__,__,__,__,__,__,__},
            /*nu     N1*/ {__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,N2,__,__,__},
            /*nul    N2*/ {__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,N3,__,__,__,__,__,__,__,__},
            /*null   N3*/ {__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,__,OK,__,__,__,__,__,__,__,__},
        };

        /*
            These modes can be pushed on the stack.
        */

        [Serializable]
        private enum Mode
        {
            Array,
            Done,
            Key,
            Object,
        };

        public const int NoDepthLimit = 0;

        public JsonChecker() : this(NoDepthLimit) { }

        public JsonChecker(int depth)
        {
            if (depth < 0)
                throw new ArgumentOutOfRangeException("depth", depth, null);

            /*
                Starts the checking process by constructing a JsonChecker
                object. It takes a depth parameter that restricts the level of maximum
                nesting.

                To continue the process, call Check for each character in the
                JSON text, and then call FinalCheck to obtain the final result.
                These functions are fully reentrant.

                The JsonChecker object will be deleted by FinalCheck.
                Check will delete the JsonChecker object if it sees an error.
            */
            _state = GO;
            _depth = depth;
            _stack = new Stack<Mode>(depth);
            Push(Mode.Done);
        }

        public void Check(int ch)
        {
            /*
                After calling new_JSON_checker, call this function for each character (or
                partial character) in your JSON text. It can accept UTF-8, UTF-16, or
                UTF-32. It returns if things are looking ok so far. If it rejects the
                text, it throws an exception.
            */
            int nextClass, nextState;
            /*
                Determine the character's class.
            */
            if (ch < 0)
                OnError();
            if (ch >= 128)
                nextClass = C_ETC;
            else
            {
                nextClass = ascii_class[ch];
                if (nextClass <= __)
                    OnError();
            }
            /*
                Get the next state from the state transition table.
            */
            nextState = state_transition_table[_state, nextClass];
            if (nextState >= 0)
            {
                /*
                    Change the state.
                */
                _state = nextState;
            }
            else
            {
                /*
                    Or perform one of the actions.
                */
                switch (nextState)
                {
                    /* empty } */
                    case -9:
                        Pop(Mode.Key);
                        _state = OK;
                        break;

                    /* } */
                    case -8:
                        Pop(Mode.Object);
                        _state = OK;
                        break;

                    /* ] */
                    case -7:
                        Pop(Mode.Array);
                        _state = OK;
                        break;

                    /* { */
                    case -6:
                        Push(Mode.Key);
                        _state = OB;
                        break;

                    /* [ */
                    case -5:
                        Push(Mode.Array);
                        _state = AR;
                        break;

                    /* " */
                    case -4:
                        switch (_stack.Peek())
                        {
                            case Mode.Key:
                                _state = CO;
                                break;
                            case Mode.Array:
                            case Mode.Object:
                                _state = OK;
                                break;
                            default:
                                OnError();
                                break;
                        }
                        break;

                    /* , */
                    case -3:
                        switch (_stack.Peek())
                        {
                            case Mode.Object:
                                /*
                                    A comma causes a flip from object mode to key mode.
                                */
                                Pop(Mode.Object);
                                Push(Mode.Key);
                                _state = KE;
                                break;
                            case Mode.Array:
                                _state = VA;
                                break;
                            default:
                                OnError();
                                break;
                        }
                        break;

                    /* : */
                    case -2:
                        /*
                            A colon causes a flip from key mode to object mode.
                        */
                        Pop(Mode.Key);
                        Push(Mode.Object);
                        _state = VA;
                        break;
                    /*
                    Bad action.
                */
                    default:
                        OnError();
                        break;
                }
            }

            _offset++;
        }

        public void FinalCheck()
        {
            /*
                The FinalCheck function should be called after all of the characters
                have been processed, but only if every call to Check returned
                without throwing an exception. This method throws an exception if the
                JSON text was not accepted; in other words, the final check failed.
            */
            if (_state != OK)
                OnError();

            Pop(Mode.Done);
        }

        private void Push(Mode mode)
        {
            /*
                Push a mode onto the stack or throw if there is overflow.
            */
            if (_depth > 0 && _stack.Count >= _depth)
                OnError();

            _stack.Push(mode);
        }

        private void Pop(Mode mode)
        {
            /*
                Pop the stack, assuring that the current mode matches the expectation.
                Throws if there is underflow or if the modes mismatch.
            */
            if (_stack.Pop() != mode)
                OnError();
        }

        private void OnError()
        {
            throw new JsonNotValidException(string.Format("Invalid JSON text at character offset {0}.", _offset.ToString("N0")));
        }
    }

    /// <summary>
    /// Json not valid exception.
    /// </summary>
    public class JsonNotValidException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:JsonValidatorTool.JsonNotValidException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public JsonNotValidException(string message) : base(message)
        {

        }
    }

    /// <summary>
    /// The JsonValidator class contains all methods to validate json strings.
    /// </summary>
    public static class JsonValidator
    {
        /// <summary>
        /// Validates the specified json.
        /// </summary>
        /// <param name="json">Json.</param>
        /// <param name="depth">It restricts the level of maximum nesting.</param>
        /// <exception cref="JsonNotValidException"></exception>
        public static void Validate(string json, int? depth = null)
        {
            using (var reader = new StringReader(json))
            {
                ValidateJson(ReadChars(reader), depth);
            }
        }

        /// <summary>
        /// Validates the specified json.
        /// </summary>
        /// <param name="json">Json.</param>
        /// <param name="depth">It restricts the level of maximum nesting.</param>
        public static bool IsValid(string json, int? depth = null)
        {
            var isint = int.TryParse(json, out _);
            var isbool = bool.TryParse(json, out _);
            var isdecimal = decimal.TryParse(json, out _);
            var isfloat = float.TryParse(json, out _);
            var isdouble = double.TryParse(json, out _);
            if (json == "null" || isint || json.StartsWith("\"") && json.EndsWith("\"") || isbool || isdecimal || isfloat || isdouble)
                return true;
            try
            {
                Validate(json, depth);
                return true;
            }
            catch (JsonNotValidException)
            {
                return false;
            }

        }

        static void ValidateJson(IEnumerable<char> chars, int? depth = null)
        {
            var checker = depth.HasValue ? new JsonChecker(depth.Value) : new JsonChecker();
            foreach (char ch in chars)
            {
                checker.Check(ch);
            }
            checker.FinalCheck();
        }

        static IEnumerable<char> ReadChars(TextReader reader)
        {
            int ch = reader.Read();
            while (ch != -1)
            {
                yield return (char)ch;
                ch = reader.Read();
            }
        }

        public static bool IsJson(string input)
        {
            return IsValid(input);
        }
    }
}



