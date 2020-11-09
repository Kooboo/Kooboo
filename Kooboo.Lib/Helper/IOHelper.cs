//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kooboo.Lib.Helper
{
    public static class IOHelper
    {
        /// <summary>
        /// MIMEs the type.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static string MimeType(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return "text/html";
            }

            int Qmark = fileName.IndexOf("?");
            if (Qmark > -1)
            {
                fileName = fileName.Substring(0, Qmark);
            }

            int HashMark = fileName.IndexOf("#");
            if (HashMark > -1)
            {
                fileName = fileName.Substring(0, HashMark);
            }

            string extension = null;

            try
            {
                extension = Path.GetExtension(fileName);
            }
            catch (Exception)
            {
            }

            if (extension == null)
            {
                return "text/html";
            }
            switch (extension.ToLower())
            {
                case ".323":
                    return "text/h323";
                case ".3g2":
                    return "video/3gpp2";
                case ".3gp2":
                    return "video/3gpp2";
                case ".3gp":
                    return "video/3gpp";
                case ".3gpp":
                    return "video/3gpp";
                case ".aac":
                    return "audio/aac";
                case ".aaf":
                    return "application/octet-stream";
                case ".aca":
                    return "application/octet-stream";
                case ".accdb":
                    return "application/msaccess";
                case ".accde":
                    return "application/msaccess";
                case ".accdt":
                    return "application/msaccess";
                case ".acx":
                    return "application/internet-property-stream";
                case ".adt":
                    return "audio/vnd.dlna.adts";
                case ".adts":
                    return "audio/vnd.dlna.adts";
                case ".afm":
                    return "application/octet-stream";
                case ".ai":
                    return "application/postscript";
                case ".aif":
                    return "audio/x-aiff";
                case ".aifc":
                    return "audio/aiff";
                case ".aiff":
                    return "audio/aiff";
                case ".application":
                    return "application/x-ms-application";
                case ".apk":
                    return "application/vnd.android.package-archive";
                case ".xapk":
                    return "application/vnd.android.package-archive";
                case ".art":
                    return "image/x-jg";
                case ".asd":
                    return "application/octet-stream";
                case ".asf":
                    return "video/x-ms-asf";
                case ".asi":
                    return "application/octet-stream";
                case ".asm":
                    return "text/plain";
                case ".asr":
                    return "video/x-ms-asf";
                case ".asx":
                    return "video/x-ms-asf";
                case ".atom":
                    return "application/atom+xml";
                case ".au":
                    return "audio/basic";
                case ".avi":
                    return "video/x-msvideo";
                case ".axs":
                    return "application/olescript";
                case ".bas":
                    return "text/plain";
                case ".bcpio":
                    return "application/x-bcpio";
                case ".bin":
                    return "application/octet-stream";
                case ".bmp":
                    return "image/bmp";
                case ".c":
                    return "text/plain";
                case ".cab":
                    return "application/vnd.ms-cab-compressed";
                case ".calx":
                    return "application/vnd.ms-office.calx";
                case ".cat":
                    return "application/vnd.ms-pki.seccat";
                case ".cdf":
                    return "application/x-cdf";
                case ".chm":
                    return "application/octet-stream";
                case ".class":
                    return "application/x-java-applet";
                case ".clp":
                    return "application/x-msclip";
                case ".cmx":
                    return "image/x-cmx";
                case ".cnf":
                    return "text/plain";
                case ".cod":
                    return "image/cis-cod";
                case ".cpio":
                    return "application/x-cpio";
                case ".cpp":
                    return "text/plain";
                case ".crd":
                    return "application/x-mscardfile";
                case ".crl":
                    return "application/pkix-crl";
                case ".crt":
                    return "application/x-x509-ca-cert";
                case ".csh":
                    return "application/x-csh";
                case ".css":
                    return "text/css";
                case ".scss":
                    return "text/css";
                case ".csv":
                    return "application/octet-stream";
                case ".cur":
                    return "application/octet-stream";
                case ".dcr":
                    return "application/x-director";
                case ".deploy":
                    return "application/octet-stream";
                case ".der":
                    return "application/x-x509-ca-cert";
                case ".dib":
                    return "image/bmp";
                case ".dir":
                    return "application/x-director";
                case ".disco":
                    return "text/xml";
                case ".dll":
                    return "application/x-msdownload";
                case ".dll.config":
                    return "text/xml";
                case ".dlm":
                    return "text/dlm";
                case ".doc":
                    return "application/msword";
                case ".docm":
                    return "application/vnd.ms-word.document.macroEnabled.12";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".dot":
                    return "application/msword";
                case ".dotm":
                    return "application/vnd.ms-word.template.macroEnabled.12";
                case ".dotx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.template";
                case ".dsp":
                    return "application/octet-stream";
                case ".dtd":
                    return "text/xml";
                case ".dvi":
                    return "application/x-dvi";
                case ".dvr-ms":
                    return "video/x-ms-dvr";
                case ".dwf":
                    return "drawing/x-dwf";
                case ".dwp":
                    return "application/octet-stream";
                case ".dxr":
                    return "application/x-director";
                case ".eml":
                    return "message/rfc822";
                case ".emz":
                    return "application/octet-stream";
                case ".eot":
                    return "application/vnd.ms-fontobject";
                case ".eps":
                    return "application/postscript";
                case ".etx":
                    return "text/x-setext";
                case ".evy":
                    return "application/envoy";
                case ".exe":
                    return "application/octet-stream";
                case ".exe.config":
                    return "text/xml";
                case ".fdf":
                    return "application/vnd.fdf";
                case ".fif":
                    return "application/fractals";
                case ".fla":
                    return "application/octet-stream";
                case ".flr":
                    return "x-world/x-vrml";
                case ".flv":
                    return "video/x-flv";
                case ".gif":
                    return "image/gif";
                case ".gtar":
                    return "application/x-gtar";
                case ".gz":
                    return "application/x-gzip";
                case ".h":
                    return "text/plain";
                case ".hdf":
                    return "application/x-hdf";
                case ".hdml":
                    return "text/x-hdml";
                case ".hhc":
                    return "application/x-oleobject";
                case ".hhk":
                    return "application/octet-stream";
                case ".hhp":
                    return "application/octet-stream";
                case ".hlp":
                    return "application/winhlp";
                case ".hqx":
                    return "application/mac-binhex40";
                case ".hta":
                    return "application/hta";
                case ".htc":
                    return "text/x-component";
                case ".htm":
                    return "text/html";
                case ".html":
                    return "text/html";
                case ".htt":
                    return "text/webviewhtml";
                case ".hxt":
                    return "text/html";
                case ".ical":
                    return "text/calendar";
                case ".icalendar":
                    return "text/calendar";
                case ".ico":
                    return "image/x-icon";
                case ".ics":
                    return "text/calendar";
                case ".ief":
                    return "image/ief";
                case ".ifb":
                    return "text/calendar";
                case ".iii":
                    return "application/x-iphone";
                case ".inf":
                    return "application/octet-stream";
                case ".ins":
                    return "application/x-internet-signup";
                case ".isp":
                    return "application/x-internet-signup";
                case ".IVF":
                    return "video/x-ivf";
                case ".jar":
                    return "application/java-archive";
                case ".java":
                    return "application/octet-stream";
                case ".jck":
                    return "application/liquidmotion";
                case ".jcz":
                    return "application/liquidmotion";
                case ".jfif":
                    return "image/pjpeg";
                case ".jpb":
                    return "application/octet-stream";
                case ".jpe":
                    return "image/jpeg";
                case ".jpeg":
                    return "image/jpeg";
                case ".jpg":
                    return "image/jpeg";
                case ".js":
                    return "application/javascript";
                case ".jsx":
                    return "text/jscript";
                case ".latex":
                    return "application/x-latex";
                case ".lit":
                    return "application/x-ms-reader";
                case ".lpk":
                    return "application/octet-stream";
                case ".lsf":
                    return "video/x-la-asf";
                case ".lsx":
                    return "video/x-la-asf";
                case ".lzh":
                    return "application/octet-stream";
                case ".m13":
                    return "application/x-msmediaview";
                case ".m14":
                    return "application/x-msmediaview";
                case ".m1v":
                    return "video/mpeg";
                case ".m2ts":
                    return "video/vnd.dlna.mpeg-tts";
                case ".m3u":
                    return "audio/x-mpegurl";
                case ".m4a":
                    return "audio/mp4";
                case ".m4v":
                    return "video/mp4";
                case ".man":
                    return "application/x-troff-man";
                case ".manifest":
                    return "application/x-ms-manifest";
                case ".map":
                    return "text/plain";
                case ".mdb":
                    return "application/x-msaccess";
                case ".mdp":
                    return "application/octet-stream";
                case ".me":
                    return "application/x-troff-me";
                case ".mht":
                    return "message/rfc822";
                case ".mhtml":
                    return "message/rfc822";
                case ".mid":
                    return "audio/mid";
                case ".midi":
                    return "audio/mid";
                case ".mix":
                    return "application/octet-stream";
                case ".mmf":
                    return "application/x-smaf";
                case ".mno":
                    return "text/xml";
                case ".mny":
                    return "application/x-msmoney";
                case ".mov":
                    return "video/quicktime";
                case ".movie":
                    return "video/x-sgi-movie";
                case ".mp2":
                    return "video/mpeg";
                case ".mp3":
                    return "audio/mpeg";
                case ".mp4":
                    return "video/mp4";
                case ".mp4v":
                    return "video/mp4";
                case ".mkv":
                    return "video/x-matroska";
                case ".mpa":
                    return "video/mpeg";
                case ".mpe":
                    return "video/mpeg";
                case ".mpeg":
                    return "video/mpeg";
                case ".mpg":
                    return "video/mpeg";
                case ".mpp":
                    return "application/vnd.ms-project";
                case ".mpv2":
                    return "video/mpeg";
                case ".ms":
                    return "application/x-troff-ms";
                case ".msi":
                    return "application/octet-stream";
                case ".mso":
                    return "application/octet-stream";
                case ".mvb":
                    return "application/x-msmediaview";
                case ".mvc":
                    return "application/x-miva-compiled";
                case ".nc":
                    return "application/x-netcdf";
                case ".nsc":
                    return "video/x-ms-asf";
                case ".nws":
                    return "message/rfc822";
                case ".ocx":
                    return "application/octet-stream";
                case ".oda":
                    return "application/oda";
                case ".odc":
                    return "text/x-ms-odc";
                case ".ods":
                    return "application/oleobject";
                case ".oga":
                    return "audio/ogg";
                case ".ogg":
                    return "video/ogg";
                case ".ogv":
                    return "video/ogg";
                case ".ogx":
                    return "application/ogg";
                case ".one":
                    return "application/onenote";
                case ".onea":
                    return "application/onenote";
                case ".onetoc":
                    return "application/onenote";
                case ".onetoc2":
                    return "application/onenote";
                case ".onetmp":
                    return "application/onenote";
                case ".onepkg":
                    return "application/onenote";
                case ".osdx":
                    return "application/opensearchdescription+xml";
                case ".otf":
                    return "font/otf";
                case ".p10":
                    return "application/pkcs10";
                case ".p12":
                    return "application/x-pkcs12";
                case ".p7b":
                    return "application/x-pkcs7-certificates";
                case ".p7c":
                    return "application/pkcs7-mime";
                case ".p7m":
                    return "application/pkcs7-mime";
                case ".p7r":
                    return "application/x-pkcs7-certreqresp";
                case ".p7s":
                    return "application/pkcs7-signature";
                case ".pbm":
                    return "image/x-portable-bitmap";
                case ".pcx":
                    return "application/octet-stream";
                case ".pcz":
                    return "application/octet-stream";
                case ".pdf":
                    return "application/pdf";
                case ".pfb":
                    return "application/octet-stream";
                case ".pfm":
                    return "application/octet-stream";
                case ".pfx":
                    return "application/x-pkcs12";
                case ".pgm":
                    return "image/x-portable-graymap";
                case ".pko":
                    return "application/vnd.ms-pki.pko";
                case ".pma":
                    return "application/x-perfmon";
                case ".pmc":
                    return "application/x-perfmon";
                case ".pml":
                    return "application/x-perfmon";
                case ".pmr":
                    return "application/x-perfmon";
                case ".pmw":
                    return "application/x-perfmon";
                case ".png":
                    return "image/png";
                case ".pnm":
                    return "image/x-portable-anymap";
                case ".pnz":
                    return "image/png";
                case ".pot":
                    return "application/vnd.ms-powerpoint";
                case ".potm":
                    return "application/vnd.ms-powerpoint.template.macroEnabled.12";
                case ".potx":
                    return "application/vnd.openxmlformats-officedocument.presentationml.template";
                case ".ppam":
                    return "application/vnd.ms-powerpoint.addin.macroEnabled.12";
                case ".ppm":
                    return "image/x-portable-pixmap";
                case ".pps":
                    return "application/vnd.ms-powerpoint";
                case ".ppsm":
                    return "application/vnd.ms-powerpoint.slideshow.macroEnabled.12";
                case ".ppsx":
                    return "application/vnd.openxmlformats-officedocument.presentationml.slideshow";
                case ".ppt":
                    return "application/vnd.ms-powerpoint";
                case ".pptm":
                    return "application/vnd.ms-powerpoint.presentation.macroEnabled.12";
                case ".pptx":
                    return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                case ".prf":
                    return "application/pics-rules";
                case ".prm":
                    return "application/octet-stream";
                case ".prx":
                    return "application/octet-stream";
                case ".ps":
                    return "application/postscript";
                case ".psd":
                    return "application/octet-stream";
                case ".psm":
                    return "application/octet-stream";
                case ".psp":
                    return "application/octet-stream";
                case ".pub":
                    return "application/x-mspublisher";
                case ".qt":
                    return "video/quicktime";
                case ".qtl":
                    return "application/x-quicktimeplayer";
                case ".qxd":
                    return "application/octet-stream";
                case ".ra":
                    return "audio/x-pn-realaudio";
                case ".ram":
                    return "audio/x-pn-realaudio";
                case ".rar":
                    return "application/octet-stream";
                case ".ras":
                    return "image/x-cmu-raster";
                case ".rf":
                    return "image/vnd.rn-realflash";
                case ".rgb":
                    return "image/x-rgb";
                case ".rm":
                    return "application/vnd.rn-realmedia";
                case ".rmi":
                    return "audio/mid";
                case ".roff":
                    return "application/x-troff";
                case ".rpm":
                    return "audio/x-pn-realaudio-plugin";
                case ".rtf":
                    return "application/rtf";
                case ".rtx":
                    return "text/richtext";
                case ".scd":
                    return "application/x-msschedule";
                case ".sct":
                    return "text/scriptlet";
                case ".sea":
                    return "application/octet-stream";
                case ".setpay":
                    return "application/set-payment-initiation";
                case ".setreg":
                    return "application/set-registration-initiation";
                case ".sgml":
                    return "text/sgml";
                case ".sh":
                    return "application/x-sh";
                case ".shar":
                    return "application/x-shar";
                case ".sit":
                    return "application/x-stuffit";
                case ".sldm":
                    return "application/vnd.ms-powerpoint.slide.macroEnabled.12";
                case ".sldx":
                    return "application/vnd.openxmlformats-officedocument.presentationml.slide";
                case ".smd":
                    return "audio/x-smd";
                case ".smi":
                    return "application/octet-stream";
                case ".smx":
                    return "audio/x-smd";
                case ".smz":
                    return "audio/x-smd";
                case ".snd":
                    return "audio/basic";
                case ".snp":
                    return "application/octet-stream";
                case ".spc":
                    return "application/x-pkcs7-certificates";
                case ".spl":
                    return "application/futuresplash";
                case ".spx":
                    return "audio/ogg";
                case ".src":
                    return "application/x-wais-source";
                case ".ssm":
                    return "application/streamingmedia";
                case ".sst":
                    return "application/vnd.ms-pki.certstore";
                case ".stl":
                    return "application/vnd.ms-pki.stl";
                case ".sv4cpio":
                    return "application/x-sv4cpio";
                case ".sv4crc":
                    return "application/x-sv4crc";
                case ".svg":
                    return "image/svg+xml";
                case ".svgz":
                    return "image/svg+xml";
                case ".swf":
                    return "application/x-shockwave-flash";
                case ".t":
                    return "application/x-troff";
                case ".tar":
                    return "application/x-tar";
                case ".tcl":
                    return "application/x-tcl";
                case ".tex":
                    return "application/x-tex";
                case ".texi":
                    return "application/x-texinfo";
                case ".texinfo":
                    return "application/x-texinfo";
                case ".tgz":
                    return "application/x-compressed";
                case ".thmx":
                    return "application/vnd.ms-officetheme";
                case ".thn":
                    return "application/octet-stream";
                case ".tif":
                    return "image/tiff";
                case ".tiff":
                    return "image/tiff";
                case ".toc":
                    return "application/octet-stream";
                case ".tr":
                    return "application/x-troff";
                case ".trm":
                    return "application/x-msterminal";
                case ".ts":
                    return "video/vnd.dlna.mpeg-tts";
                case ".tsv":
                    return "text/tab-separated-values";
                case ".ttf":
                    return "application/octet-stream";
                case ".tts":
                    return "video/vnd.dlna.mpeg-tts";
                case ".txt":
                    return "text/plain";
                case ".u32":
                    return "application/octet-stream";
                case ".uls":
                    return "text/iuls";
                case ".ustar":
                    return "application/x-ustar";
                case ".vbs":
                    return "text/vbscript";
                case ".vcf":
                    return "text/x-vcard";
                case ".vcs":
                    return "text/plain";
                case ".vdx":
                    return "application/vnd.ms-visio.viewer";
                case ".vml":
                    return "text/xml";
                case ".vsd":
                    return "application/vnd.visio";
                case ".vss":
                    return "application/vnd.visio";
                case ".vst":
                    return "application/vnd.visio";
                case ".vsto":
                    return "application/x-ms-vsto";
                case ".vsw":
                    return "application/vnd.visio";
                case ".vsx":
                    return "application/vnd.visio";
                case ".vtx":
                    return "application/vnd.visio";
                case ".wav":
                    return "audio/wav";
                case ".wax":
                    return "audio/x-ms-wax";
                case ".wbmp":
                    return "image/vnd.wap.wbmp";
                case ".wcm":
                    return "application/vnd.ms-works";
                case ".wdb":
                    return "application/vnd.ms-works";
                case ".webm":
                    return "video/webm";
                case ".wks":
                    return "application/vnd.ms-works";
                case ".wm":
                    return "video/x-ms-wm";
                case ".wma":
                    return "audio/x-ms-wma";
                case ".wmd":
                    return "application/x-ms-wmd";
                case ".wmf":
                    return "application/x-msmetafile";
                case ".wml":
                    return "text/vnd.wap.wml";
                case ".wmlc":
                    return "application/vnd.wap.wmlc";
                case ".wmls":
                    return "text/vnd.wap.wmlscript";
                case ".wmlsc":
                    return "application/vnd.wap.wmlscriptc";
                case ".wmp":
                    return "video/x-ms-wmp";
                case ".wmv":
                    return "video/x-ms-wmv";
                case ".wmx":
                    return "video/x-ms-wmx";
                case ".wmz":
                    return "application/x-ms-wmz";
                case ".woff":
                    return "font/x-woff";
                case ".woff2":
                    return "font/x-woff2";
                case ".wps":
                    return "application/vnd.ms-works";
                case ".wri":
                    return "application/x-mswrite";
                case ".wrl":
                    return "x-world/x-vrml";
                case ".wrz":
                    return "x-world/x-vrml";
                case ".wsdl":
                    return "text/xml";
                case ".wtv":
                    return "video/x-ms-wtv";
                case ".wvx":
                    return "video/x-ms-wvx";
                case ".x":
                    return "application/directx";
                case ".xaf":
                    return "x-world/x-vrml";
                case ".xaml":
                    return "application/xaml+xml";
                case ".xap":
                    return "application/x-silverlight-app";
                case ".xbap":
                    return "application/x-ms-xbap";
                case ".xbm":
                    return "image/x-xbitmap";
                case ".xdr":
                    return "text/plain";
                case ".xht":
                    return "application/xhtml+xml";
                case ".xhtml":
                    return "application/xhtml+xml";
                case ".xla":
                    return "application/vnd.ms-excel";
                case ".xlam":
                    return "application/vnd.ms-excel.addin.macroEnabled.12";
                case ".xlc":
                    return "application/vnd.ms-excel";
                case ".xlm":
                    return "application/vnd.ms-excel";
                case ".xls":
                    return "application/vnd.ms-excel";
                case ".xlsb":
                    return "application/vnd.ms-excel.sheet.binary.macroEnabled.12";
                case ".xlsm":
                    return "application/vnd.ms-excel.sheet.macroEnabled.12";
                case ".xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".xlt":
                    return "application/vnd.ms-excel";
                case ".xltm":
                    return "application/vnd.ms-excel.template.macroEnabled.12";
                case ".xltx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.template";
                case ".xlw":
                    return "application/vnd.ms-excel";
                case ".xml":
                    return "text/xml";
                case ".xof":
                    return "x-world/x-vrml";
                case ".xpm":
                    return "image/x-xpixmap";
                case ".xps":
                    return "application/vnd.ms-xpsdocument";
                case ".xsd":
                    return "text/xml";
                case ".xsf":
                    return "text/xml";
                case ".xsl":
                    return "text/xml";
                case ".xslt":
                    return "text/xml";
                case ".xsn":
                    return "application/octet-stream";
                case ".xtp":
                    return "application/octet-stream";
                case ".xwd":
                    return "image/x-xwindowdump";
                case ".z":
                    return "application/x-compress";
                case ".zip":
                    return "application/x-zip-compressed";
                default:
                    // return "application/octet-stream";
                    return "text/html";
            }
        }

        public static bool IsStringType(string mimeType)
        {
            mimeType = mimeType.ToLower();
            return mimeType.Contains("text") || mimeType.Contains("script") || mimeType.Contains("xml") || mimeType.Contains("json") || mimeType.Contains("style") || mimeType.Contains("html");
        }

        public static string CombinePath(string basefolder, string subFolder)
        {
            if (string.IsNullOrEmpty(subFolder))
            {
                return basefolder;
            }

            if (string.IsNullOrEmpty(basefolder))
            {
                return subFolder;
            }

            if (subFolder.StartsWith("/") || subFolder.StartsWith("\\"))
            {
                subFolder = subFolder.Substring(1);
            }

            return Kooboo.Lib.Compatible.CompatibleManager.Instance.System.CombinePath(basefolder, subFolder);
        }

        /// <summary>
        /// Ensures the directory exists.
        /// </summary>
        public static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void EnsureFileDirectoryExists(string filePath)
        {
            var dir = Path.GetDirectoryName(filePath);
            EnsureDirectoryExists(dir);
        }

        public static string FindCommonPath(List<string> paths)
        {
            Func<List<string>, bool> HasSameValue = (list) =>
            {
                string current = null;

                foreach (var item in list)
                {
                    if (current == null)
                    {
                        current = item;
                    }
                    else
                    {
                        if (!Kooboo.Lib.Helper.StringHelper.IsSameValue(current, item))
                        {
                            return false;
                        }
                    }
                }
                return true;
            };

            Func<string, List<string>> ToSegments = Kooboo.Lib.Compatible.CompatibleManager.Instance.System.GetSegments;

            List<List<string>> AllSegments = new List<List<string>>();

            foreach (var item in paths)
            {
                AllSegments.Add(ToSegments(item));
            }

            List<string> common = new List<string>();

            int i = 0;

            while (i < 999)
            {
                List<string> indexitem = new List<string>();
                foreach (var item in AllSegments)
                {
                    if (i > item.Count() - 1)
                    {
                        break;
                    }
                    else
                    {
                        indexitem.Add(item[i]);
                    }
                }
                if (indexitem.Count() > 0 && HasSameValue(indexitem))
                {
                    common.Add(indexitem[0]);
                    i += 1;
                }
                else
                {
                    break;
                }
            }

            if (common.Count() == 0)
            {
                return null;
            }
            else
            {
                return Kooboo.Lib.Compatible.CompatibleManager.Instance.System.JoinPath(common.ToArray());
            }
        }

        public static int CountFiles(string Folder, bool Recursive = true)
        {
            if (!System.IO.Directory.Exists(Folder))
            {
                return 0;
            }
            if (Recursive)
            {
                return Directory.GetFiles(Folder, "*", SearchOption.AllDirectories).Length;
            }
            else
            {
                return Directory.GetFiles(Folder, "*", SearchOption.TopDirectoryOnly).Length;
            }

        }

        public static bool IsEqualBytes(byte[] x, byte[] y)
        {
            if (x == null || y == null || x.Length != y.Length)
            {
                return false;
            }

            int len = x.Length;

            for (int i = 0; i < len; i++)
            {
                if (x[i] == y[i])
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }

            return true;

        }

        public static long GetDirectorySize(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                return 0;
            }
            DirectoryInfo di = new DirectoryInfo(folderPath);
            return di.EnumerateFiles("*", SearchOption.AllDirectories).Sum(fi => fi.Length);
        }

        public static string ReadAllText(string FilePath)
        {
            if (File.Exists(FilePath))
            {
                System.IO.FileStream stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                var reader = new System.IO.StreamReader(stream);

                var text = reader.ReadToEnd();
                reader.Dispose();
                stream.Dispose();
                return text;
            }
            return null;
        }

        public static byte[] ReadAllBytes(string FilePath)
        {
            if (File.Exists(FilePath))
            {
                FileStream stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                MemoryStream ms = new MemoryStream();
                stream.CopyTo(ms);
                stream.Close();
                stream.Dispose();
                return ms.ToArray();
            }
            return null;
        }


        public static void WriteAllBytes(string FilePath, byte[] bytes)
        {
            Lib.Helper.IOHelper.EnsureFileDirectoryExists(FilePath);
            System.IO.File.WriteAllBytes(FilePath, bytes);
        }

        public static void Copy(string oldPath, string newPath)
        {
            if (System.IO.File.Exists(oldPath))
            {
                Lib.Helper.IOHelper.EnsureFileDirectoryExists(newPath);
                System.IO.File.Copy(oldPath, newPath);
            }
        }

        public static bool IsDirectory(string path)
        {
            var parts = path.Split("\\".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (parts == null || !parts.Any())
            {
                return false;
            }

            parts = parts.Reverse().ToArray();

            if (parts[0].Contains("."))
            {
                return false;
            }
            return true;
        }


        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                return;
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

    }
}
