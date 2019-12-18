using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Frontend.KScriptDefine
{
    public interface KScript
    {
        [Discription(@"method or property description")]
        User User { get; set; }

        void Execute(string code, User user);
    }

    public class User : Person
    {
        public string Name { get; set; }
        public string Pwd { get; set; }
        public int Age { get; set; }
    }

    public class Person
    {
        public int Id { get; set; }
        public KType KType { get; set; }

    }
    public enum KType
    {
        aaa,
        bbb
    }
}
