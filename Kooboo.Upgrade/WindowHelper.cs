//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Kooboo.Upgrade
{
    public class WindowHelper
    {
        private delegate bool WNDENUMPROC(IntPtr hWnd, int lParam);
        
        //enum windows
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, int lParam);

        //get window text
        [DllImport("user32.dll")]
        private static extern int GetWindowTextW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);

        //get window class name
        [DllImport("user32.dll")]
        private static extern int GetClassNameW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);

        public static List<IntPtr> GetAllKoobooWindows()
        {
            //string windowName = "Kooboo";
            List<IntPtr> wndList = new List<IntPtr>();

            //enum all desktop windows 
            EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                StringBuilder sb = new StringBuilder(256);

                GetWindowTextW(hWnd, sb, sb.Capacity);
                var windowName = sb.ToString();

                GetClassNameW(hWnd, sb, sb.Capacity);
                var className = sb.ToString();

                if (windowName == "Kooboo" && className.IndexOf("Kooboo.exe")>-1)
                {
                    wndList.Add(hWnd);
                }
                
                return true;
            }, 0);

            return wndList;
        }

        /// <summary>
        /// get window by processId
        /// </summary>
        /// <param name="windows"></param>
        /// <param name="processId"></param>
        /// <returns></returns>
        public static IntPtr GetKoobooWindow(List<IntPtr> windows,int processId)
        {

            foreach(var window in windows)
            {
                int id;
                GetWindowThreadProcessId(window, out id);
                if (processId == id)
                {
                    return window;
                }
            }
            return IntPtr.Zero;
        }
    }
}
