//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.IO;

namespace Kooboo.Render.ObjectSource
{
    public class DirectoryRender
    {
        public static string Resolve(string localPath, string localRootPath)
        {
            if (localRootPath.StartsWith(Kooboo.Data.AppSettings.RootPath))
            {
                return null;
            }

            var fileList = string.Empty;
            DirectoryInfo directory = new DirectoryInfo(localPath);
            if (!directory.Exists)
            {
                return null;
            }
            if (directory.FullName.ToLower() != localRootPath.ToLower())
            {
                string header = TemplateString.Tr.Replace("{className}", "arrow-left");
                string url = null;
                if (directory.Parent != null)
                {
                    string fullname = directory.Parent.FullName;
                    if (fullname.Length == localRootPath.Length)
                    {
                        url = "/";
                    }
                    else
                    {
                        if (fullname.Length >= localRootPath.Length)
                        {
                            url = fullname.Substring(localRootPath.Length);
                            url = url.Replace("\\", "/");
                            if (!url.StartsWith("/") && !url.StartsWith("\\"))
                            {
                                url = "/" + url;
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(url))
                {
                    url = "/";
                }

                header = header.Replace("{url}", url);
                header = header.Replace("{name}", "Parent Directory");
                header = header.Replace("{time}", "");

                fileList += header;

            }
            foreach (var info in directory.GetDirectories())
            {
                string Folder = TemplateString.Tr.Replace("{className}", "folder");
                Folder = Folder.Replace("{url}", info.FullName.Replace(localRootPath, string.Empty));
                Folder = Folder.Replace("{name}", info.Name);
                Folder = Folder.Replace("{time}", info.LastWriteTime.ToShortDateString());

                fileList += Folder;
            }
            foreach (var info in directory.GetFiles())
            {
                var className = String.Empty;
                switch (info.Name.Substring(info.Name.LastIndexOf(".") + 1).ToLower())
                {
                    case "jpeg":
                    case "jpg":
                    case "gif":
                    case "bmp":
                    case "tiff":
                    case "png":
                    case "ico":
                        className = "file-image-o";
                        break;
                    case "html":
                    case "cshtml":
                    case "shtml":
                    case "xml":
                    case "css":
                    case "less":
                    case "scss":
                    case "sass":
                    case "js":
                    case "jsx":
                    case "json":
                        className = "file-code-o";
                        break;
                    case "wav":
                    case "mp3":
                        className = "file-audio-o";
                        break;
                    case "ogg":
                    case "mp4":
                        className = "file-video-o";
                        break;
                    case "pdf":
                        className = "file-pdf-o";
                        break;
                    case "rar":
                    case "zip":
                    case "7z":
                        className = "file-archive-o";
                        break;
                    default:
                        className = "file-text-o";
                        break;
                };

                string file = TemplateString.Tr.Replace("{className}", className);
                file = file.Replace("{url}", info.FullName.Replace(localRootPath, string.Empty));
                file = file.Replace("{name}", info.Name);
                file = file.Replace("{time}", info.LastWriteTime.ToShortDateString());

                fileList += file;
            }

            return TemplateString.Main.Replace("{fileList}", fileList);
        }
    }
    public class TemplateString
    {
        public static string Main = @"<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <title>Directory </title>
    <meta name='viewport' content='width=device-width, initial-scale=1, user-scalable=no'>
    <style>
        * {
            margin: 0;
            padding: 0;
            font: 16px/1.2 Helvetica, Arial, 'Microsoft YaHei', sans-serif;
        }

#header {
    width: 100%;
            background: #0087c2;
            line-height: 20px;
            color: #ffffff;
            text-align: center;
            font-size: 20px;
            margin-bottom: 20px;
        }

#header div {
padding: 10px 0;
            }

        .file-container {
            width: 600px;
            max-width: 92%;
            margin: 0 auto;
        }

        #explorer tr td {
            font-size: 16px;
            line-height: 1.5;
            padding: 2px 4px;
            vertical-align: top;
        }

        #explorer th {
            text-align: left;
            font-size: 20px;
            font-weight: 700;
            line-height: 1.5;
            white-space: nowrap;
        }

        #explorer a {
            text-decoration: none;
            color: #252525;
            word-break: break-all;
        }

        #explorer a:hover {
            color: #0087c2;
        }

        #explorer .icon-link{
            float: right;
            margin: 0;
        }

        [class|=icon] {
            width: 16px;
            height: 16px;
            margin: 5px 0;
        }

        .icon-folder {
            background: url('data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIxMDAlIiBoZWlnaHQ9IjEwMCUiIHZpZXdCb3g9IjAgMCAxNjY0IDE3OTIiPjxwYXRoIGZpbGw9IiNkZTczMDAiIGQ9Ik0xNjY0IDYwOHY3MDRxMCA5Mi02NiAxNTh0LTE1OCA2NmgtMTIxNnEtOTIgMC0xNTgtNjZ0LTY2LTE1OHYtOTYwcTAtOTIgNjYtMTU4dDE1OC02NmgzMjBxOTIgMCAxNTggNjZ0NjYgMTU4djMyaDY3MnE5MiAwIDE1OCA2NnQ2NiAxNTh6IiAvPjwvc3ZnPg==') no-repeat;
        }

        .icon-arrow-left {
            background: url('data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIxMDAlIiBoZWlnaHQ9IjEwMCUiIHZpZXdCb3g9IjAgMCAxNTM2IDE3OTIiPjxwYXRoIGZpbGw9IiMwMDAwMDAiIGQ9Ik0xNTM2IDg5NnYxMjhxMCA1My0zMi41IDkwLjV0LTg0LjUgMzcuNWgtNzA0bDI5MyAyOTRxMzggMzYgMzggOTB0LTM4IDkwbC03NSA3NnEtMzcgMzctOTAgMzctNTIgMC05MS0zN2wtNjUxLTY1MnEtMzctMzctMzctOTAgMC01MiAzNy05MWw2NTEtNjUwcTM4LTM4IDkxLTM4IDUyIDAgOTAgMzhsNzUgNzRxMzggMzggMzggOTF0LTM4IDkxbC0yOTMgMjkzaDcwNHE1MiAwIDg0LjUgMzcuNXQzMi41IDkwLjV6IiAvPjwvc3ZnPg==') no-repeat;
        }

        .icon-file-text-o {
            background: url('data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIxMDAlIiBoZWlnaHQ9IjEwMCUiIHZpZXdCb3g9IjAgMCAxNTM2IDE3OTIiPjxwYXRoIGZpbGw9IiMwMDAwMDAiIGQ9Ik0xNDY4IDM4MHEyOCAyOCA0OCA3NnQyMCA4OHYxMTUycTAgNDAtMjggNjh0LTY4IDI4aC0xMzQ0cS00MCAwLTY4LTI4dC0yOC02OHYtMTYwMHEwLTQwIDI4LTY4dDY4LTI4aDg5NnE0MCAwIDg4IDIwdDc2IDQ4ek0xMDI0IDEzNnYzNzZoMzc2cS0xMC0yOS0yMi00MWwtMzEzLTMxM3EtMTItMTItNDEtMjJ6TTE0MDggMTY2NHYtMTAyNGgtNDE2cS00MCAwLTY4LTI4dC0yOC02OHYtNDE2aC03Njh2MTUzNmgxMjgwek0zODQgODAwcTAtMTQgOS0yM3QyMy05aDcwNHExNCAwIDIzIDl0OSAyM3Y2NHEwIDE0LTkgMjN0LTIzIDloLTcwNHEtMTQgMC0yMy05dC05LTIzdi02NHpNMTEyMCAxMDI0cTE0IDAgMjMgOXQ5IDIzdjY0cTAgMTQtOSAyM3QtMjMgOWgtNzA0cS0xNCAwLTIzLTl0LTktMjN2LTY0cTAtMTQgOS0yM3QyMy05aDcwNHpNMTEyMCAxMjgwcTE0IDAgMjMgOXQ5IDIzdjY0cTAgMTQtOSAyM3QtMjMgOWgtNzA0cS0xNCAwLTIzLTl0LTktMjN2LTY0cTAtMTQgOS0yM3QyMy05aDcwNHoiIC8+PC9zdmc+') no-repeat;
        }

        .icon-file-image-o {
            background: url('data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIxMDAlIiBoZWlnaHQ9IjEwMCUiIHZpZXdCb3g9IjAgMCAxNTM2IDE3OTIiPjxwYXRoIGZpbGw9IiMwMDAwMDAiIGQ9Ik0xNDY4IDM4MHEyOCAyOCA0OCA3NnQyMCA4OHYxMTUycTAgNDAtMjggNjh0LTY4IDI4aC0xMzQ0cS00MCAwLTY4LTI4dC0yOC02OHYtMTYwMHEwLTQwIDI4LTY4dDY4LTI4aDg5NnE0MCAwIDg4IDIwdDc2IDQ4ek0xMDI0IDEzNnYzNzZoMzc2cS0xMC0yOS0yMi00MWwtMzEzLTMxM3EtMTItMTItNDEtMjJ6TTE0MDggMTY2NHYtMTAyNGgtNDE2cS00MCAwLTY4LTI4dC0yOC02OHYtNDE2aC03Njh2MTUzNmgxMjgwek0xMjgwIDEyMTZ2MzIwaC0xMDI0di0xOTJsMTkyLTE5MiAxMjggMTI4IDM4NC0zODR6TTQ0OCAxMDI0cS04MCAwLTEzNi01NnQtNTYtMTM2IDU2LTEzNiAxMzYtNTYgMTM2IDU2IDU2IDEzNi01NiAxMzYtMTM2IDU2eiIgLz48L3N2Zz4=') no-repeat;
        }
        
        .icon-file-code-o {
            background: url('data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIxMDAlIiBoZWlnaHQ9IjEwMCUiIHZpZXdCb3g9IjAgMCAxNTM2IDE3OTIiPjxwYXRoIGZpbGw9IiMwMDAwMDAiIGQ9Ik0xNDY4IDM4MHEyOCAyOCA0OCA3NnQyMCA4OHYxMTUycTAgNDAtMjggNjh0LTY4IDI4aC0xMzQ0cS00MCAwLTY4LTI4dC0yOC02OHYtMTYwMHEwLTQwIDI4LTY4dDY4LTI4aDg5NnE0MCAwIDg4IDIwdDc2IDQ4ek0xMDI0IDEzNnYzNzZoMzc2cS0xMC0yOS0yMi00MWwtMzEzLTMxM3EtMTItMTItNDEtMjJ6TTE0MDggMTY2NHYtMTAyNGgtNDE2cS00MCAwLTY4LTI4dC0yOC02OHYtNDE2aC03Njh2MTUzNmgxMjgwek00ODAgNzY4cTgtMTEgMjEtMTIuNXQyNCA2LjVsNTEgMzhxMTEgOCAxMi41IDIxdC02LjUgMjRsLTE4MiAyNDMgMTgyIDI0M3E4IDExIDYuNSAyNHQtMTIuNSAyMWwtNTEgMzhxLTExIDgtMjQgNi41dC0yMS0xMi41bC0yMjYtMzAxcS0xNC0xOSAwLTM4ek0xMjgyIDEwNjlxMTQgMTkgMCAzOGwtMjI2IDMwMXEtOCAxMS0yMSAxMi41dC0yNC02LjVsLTUxLTM4cS0xMS04LTEyLjUtMjF0Ni41LTI0bDE4Mi0yNDMtMTgyLTI0M3EtOC0xMS02LjUtMjR0MTIuNS0yMWw1MS0zOHExMS04IDI0LTYuNXQyMSAxMi41ek02NjIgMTUzMHEtMTMtMi0yMC41LTEzdC01LjUtMjRsMTM4LTgzMXEyLTEzIDEzLTIwLjV0MjQtNS41bDYzIDEwcTEzIDIgMjAuNSAxM3Q1LjUgMjRsLTEzOCA4MzFxLTIgMTMtMTMgMjAuNXQtMjQgNS41eiIgLz48L3N2Zz4=') no-repeat;
        }

        .icon-file-audio-o {
            background: url('data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIxMDAlIiBoZWlnaHQ9IjEwMCUiIHZpZXdCb3g9IjAgMCAxNTM2IDE3OTIiPjxwYXRoIGZpbGw9IiMwMDAwMDAiIGQ9Ik0xNDY4IDM4MHEyOCAyOCA0OCA3NnQyMCA4OHYxMTUycTAgNDAtMjggNjh0LTY4IDI4aC0xMzQ0cS00MCAwLTY4LTI4dC0yOC02OHYtMTYwMHEwLTQwIDI4LTY4dDY4LTI4aDg5NnE0MCAwIDg4IDIwdDc2IDQ4ek0xMDI0IDEzNnYzNzZoMzc2cS0xMC0yOS0yMi00MWwtMzEzLTMxM3EtMTItMTItNDEtMjJ6TTE0MDggMTY2NHYtMTAyNGgtNDE2cS00MCAwLTY4LTI4dC0yOC02OHYtNDE2aC03Njh2MTUzNmgxMjgwek02MjAgODUwcTIwIDggMjAgMzB2NTQ0cTAgMjItMjAgMzAtOCAyLTEyIDItMTIgMC0yMy05bC0xNjYtMTY3aC0xMzFxLTE0IDAtMjMtOXQtOS0yM3YtMTkycTAtMTQgOS0yM3QyMy05aDEzMWwxNjYtMTY3cTE2LTE1IDM1LTd6TTEwMzcgMTUzOXEzMSAwIDUwLTI0IDEyOS0xNTkgMTI5LTM2M3QtMTI5LTM2M3EtMTYtMjEtNDMtMjR0LTQ3IDE0cS0yMSAxNy0yMy41IDQzLjV0MTQuNSA0Ny41cTEwMCAxMjMgMTAwIDI4MnQtMTAwIDI4MnEtMTcgMjEtMTQuNSA0Ny41dDIzLjUgNDIuNXExOCAxNSA0MCAxNXpNODI2IDEzOTFxMjcgMCA0Ny0yMCA4Ny05MyA4Ny0yMTl0LTg3LTIxOXEtMTgtMTktNDUtMjB0LTQ2IDE3LTIwIDQ0LjUgMTggNDYuNXE1MiA1NyA1MiAxMzF0LTUyIDEzMXEtMTkgMjAtMTggNDYuNXQyMCA0NC41cTIwIDE3IDQ0IDE3eiIgLz48L3N2Zz4=') no-repeat;
        }

        .icon-file-video-o {
            background: url('data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIxMDAlIiBoZWlnaHQ9IjEwMCUiIHZpZXdCb3g9IjAgMCAxNTM2IDE3OTIiPjxwYXRoIGZpbGw9IiMwMDAwMDAiIGQ9Ik0xNDY4IDM4MHEyOCAyOCA0OCA3NnQyMCA4OHYxMTUycTAgNDAtMjggNjh0LTY4IDI4aC0xMzQ0cS00MCAwLTY4LTI4dC0yOC02OHYtMTYwMHEwLTQwIDI4LTY4dDY4LTI4aDg5NnE0MCAwIDg4IDIwdDc2IDQ4ek0xMDI0IDEzNnYzNzZoMzc2cS0xMC0yOS0yMi00MWwtMzEzLTMxM3EtMTItMTItNDEtMjJ6TTE0MDggMTY2NHYtMTAyNGgtNDE2cS00MCAwLTY4LTI4dC0yOC02OHYtNDE2aC03Njh2MTUzNmgxMjgwek03NjggNzY4cTUyIDAgOTAgMzh0MzggOTB2Mzg0cTAgNTItMzggOTB0LTkwIDM4aC0zODRxLTUyIDAtOTAtMzh0LTM4LTkwdi0zODRxMC01MiAzOC05MHQ5MC0zOGgzODR6TTEyNjAgNzcwcTIwIDggMjAgMzB2NTc2cTAgMjItMjAgMzAtOCAyLTEyIDItMTQgMC0yMy05bC0yNjUtMjY2di05MGwyNjUtMjY2cTktOSAyMy05IDQgMCAxMiAyeiIgLz48L3N2Zz4=') no-repeat;
        }
        
        .icon-file-pdf-o {
            background: url('data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIxMDAlIiBoZWlnaHQ9IjEwMCUiIHZpZXdCb3g9IjAgMCAxNTM2IDE3OTIiPjxwYXRoIGZpbGw9IiMwMDAwMDAiIGQ9Ik0xNDY4IDM4MHEyOCAyOCA0OCA3NnQyMCA4OHYxMTUycTAgNDAtMjggNjh0LTY4IDI4aC0xMzQ0cS00MCAwLTY4LTI4dC0yOC02OHYtMTYwMHEwLTQwIDI4LTY4dDY4LTI4aDg5NnE0MCAwIDg4IDIwdDc2IDQ4ek0xMDI0IDEzNnYzNzZoMzc2cS0xMC0yOS0yMi00MWwtMzEzLTMxM3EtMTItMTItNDEtMjJ6TTE0MDggMTY2NHYtMTAyNGgtNDE2cS00MCAwLTY4LTI4dC0yOC02OHYtNDE2aC03Njh2MTUzNmgxMjgwek04OTQgMTA3MXEzMyAyNiA4NCA1NiA1OS03IDExNy03IDE0NyAwIDE3NyA0OSAxNiAyMiAyIDUyIDAgMS0xIDJsLTIgMnYxcS02IDM4LTcxIDM4LTQ4IDAtMTE1LTIwdC0xMzAtNTNxLTIyMSAyNC0zOTIgODMtMTUzIDI2Mi0yNDIgMjYyLTE1IDAtMjgtN2wtMjQtMTJxLTEtMS02LTUtMTAtMTAtNi0zNiA5LTQwIDU2LTkxLjV0MTMyLTk2LjVxMTQtOSAyMyA2IDIgMiAyIDQgNTItODUgMTA3LTE5NyA2OC0xMzYgMTA0LTI2Mi0yNC04Mi0zMC41LTE1OS41dDYuNS0xMjcuNXExMS00MCA0Mi00MGgyMSAxcTIzIDAgMzUgMTUgMTggMjEgOSA2OC0yIDYtNCA4IDEgMyAxIDh2MzBxLTIgMTIzLTE0IDE5MiA1NSAxNjQgMTQ2IDIzOHpNMzE4IDE0ODJxNTItMjQgMTM3LTE1OC01MSA0MC04Ny41IDg0dC00OS41IDc0ek03MTYgNTYycS0xNSA0Mi0yIDEzMiAxLTcgNy00NCAwLTMgNy00MyAxLTQgNC04LTEtMS0xLTJ0LTAuNS0xLjUtMC41LTEuNXEtMS0yMi0xMy0zNiAwIDEtMSAydjJ6TTU5MiAxMjIzcTEzNS01NCAyODQtODEtMi0xLTEzLTkuNXQtMTYtMTMuNXEtNzYtNjctMTI3LTE3Ni0yNyA4Ni04MyAxOTctMzAgNTYtNDUgODN6TTEyMzggMTIwN3EtMjQtMjQtMTQwLTI0IDc2IDI4IDEyNCAyOCAxNCAwIDE4LTEgMC0xLTItM3oiIC8+PC9zdmc+') no-repeat;
        }

        .icon-file-archive-o {
            background:  url(data:image/svg+xml; base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIxMDAlIiBoZWlnaHQ9IjEwMCUiIHZpZXdCb3g9IjAgMCAxNTM2IDE3OTIiPjxwYXRoIGZpbGw9IiMwMDAwMDAiIGQ9Ik02NDAgMzg0di0xMjhoLTEyOHYxMjhoMTI4ek03NjggNTEydi0xMjhoLTEyOHYxMjhoMTI4ek02NDAgNjQwdi0xMjhoLTEyOHYxMjhoMTI4ek03NjggNzY4di0xMjhoLTEyOHYxMjhoMTI4ek0xNDY4IDM4MHEyOCAyOCA0OCA3NnQyMCA4OHYxMTUycTAgNDAtMjggNjh0LTY4IDI4aC0xMzQ0cS00MCAwLTY4LTI4dC0yOC02OHYtMTYwMHEwLTQwIDI4LTY4dDY4LTI4aDg5NnE0MCAwIDg4IDIwdDc2IDQ4ek0xMDI0IDEzNnYzNzZoMzc2cS0xMC0yOS0yMi00MWwtMzEzLTMxM3EtMTItMTItNDEtMjJ6TTE0MDggMTY2NHYtMTAyNGgtNDE2cS00MCAwLTY4LTI4dC0yOC02OHYtNDE2aC0xMjh2MTI4aC0xMjh2LTEyOGgtNTEydjE1MzZoMTI4MHpNNzgxIDk0M2wxMDcgMzQ5cTggMjcgOCA1MiAwIDgzLTcyLjUgMTM3LjV0LTE4My41IDU0LjUtMTgzLjUtNTQuNS03Mi41LTEzNy41cTAtMjUgOC01MiAyMS02MyAxMjAtMzk2di0xMjhoMTI4djEyOGg3OXEyMiAwIDM5IDEzdDIzIDM0ek02NDAgMTQwOHE1MyAwIDkwLjUtMTl0MzcuNS00NS0zNy41LTQ1LTkwLjUtMTktOTAuNSAxOS0zNy41IDQ1IDM3LjUgNDUgOTAuNSAxOXoiIC8+PC9zdmc+) no-repeat;
        }

        @media screen and (max-width: 767px){
            .file-container{
                border: 1px solid #ddd;
                border-radius: 2px;
                overflow-x: scroll;
            }
        }
    </style>
</head>
<body>
    <div id='header'>
        <div>File Explorer</div>
    </div>
    <div class='file-container'>
        <table id='explorer' cellpadding='0' cellspacing='0' style='width: 100%;'>
            <tr>
                <th>&nbsp;</th>
                <th>Name</th>
                <th>Last modified</th>
            </tr>
            {fileList}
        </table>
    </div>
</body>
</html>";

        public static string Tr = @"<tr>
    <td>
        <a class='icon-link' href='{url}'><div class='icon-{className}'></div></a>
    </td>
    <td>
     <a href='{url}'>{name}</a>
    </td>
    <td>{time}</td>
</tr>";


    }

}
