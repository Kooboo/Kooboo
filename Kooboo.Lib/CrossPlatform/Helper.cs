//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Lib.CrossPlatform
{
    public static class Helper
    {
        private static object _lock = new object();

        private static IImage _image;

        public static IImage Image
        {
            get
            {
                if (_image == null)
                {
                    lock(_lock)
                    {
                        if (_image == null)
                        {
                            _image = CreateInstance<IImage>();
                        }
                    } 
                }
                return _image;  
            }
        }

        public static T CreateInstance<T>()
            where T : ICrossPlatform
        {
            var type = typeof(T);

            var allImplementions = Lib.Reflection.AssemblyLoader.LoadTypeByInterface(type);

            var target = default(T);

            foreach (var item in allImplementions)
            {
                var instance = (T)Activator.CreateInstance(item);

                if (target == null)
                {
                    target = instance; 
                }
                else if (instance.Priority > target.Priority)
                {
                    target = instance;
                } 
            }
            return target;
        }

    }
}
