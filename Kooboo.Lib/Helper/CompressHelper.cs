using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace Kooboo.Lib.Helper;

public static class CompressHelper
{
    public static void ZipFolder(string folder, string zipPath)
    {
        using var fs = File.Create(zipPath);
        using var zipStream = new ZipOutputStream(fs);
        zipStream.SetLevel(3); // 0-9 范围内，0 为不压缩，9 为最大压缩
        AddFolderToZip(zipStream, folder, folder);
    }

    private static void AddFolderToZip(ZipOutputStream zipStream, string folderPath, string baseFolderPath)
    {
        string[] files = Directory.GetFiles(folderPath);

        foreach (string filePath in files)
        {
            FileInfo fileInfo = new FileInfo(filePath);

            string entryName = filePath.Substring(baseFolderPath.Length + 1);
            ZipEntry entry = new ZipEntry(entryName)
            {
                DateTime = fileInfo.LastWriteTime,
                Size = fileInfo.Length
            };

            zipStream.PutNextEntry(entry);

            using (FileStream fs = File.OpenRead(filePath))
            {
                fs.CopyTo(zipStream);
            }

            zipStream.CloseEntry();
        }

        string[] folders = Directory.GetDirectories(folderPath);

        foreach (string folder in folders)
        {
            AddFolderToZip(zipStream, folder, baseFolderPath);
        }
    }
    public static void UnZip(string file, string dir)
    {
        IOHelper.EnsureDirectoryExists(dir);
        using var zipIn = new ZipInputStream(File.OpenRead(file));

        while (true)
        {
            var entry = zipIn.GetNextEntry();
            if (entry == default) break;
            if (entry.IsDirectory)
            {
                var path = ReplaceSlash(Path.GetFullPath(Path.Combine(dir, entry.Name)));
                IOHelper.EnsureDirectoryExists(path);
            }
            else
            {
                var filePath = ReplaceSlash(Path.GetFullPath(Path.Combine(dir, entry.Name)));
                IOHelper.EnsureFileDirectoryExists(filePath);
                using var streamWriter = File.Create(filePath);
                int size = 2048;
                byte[] buffer = new byte[size];

                while ((size = zipIn.Read(buffer, 0, buffer.Length)) > 0)
                {
                    streamWriter.Write(buffer, 0, size);
                }
            }
        }
    }

    private static string ReplaceSlash(string value)
    {
        return value.Replace(@"\", "/");
    }
}