using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AbroadConcepts.IO;
public static class FileExtension
{

    public static string EnsureExtension (this string filePrefix, string extension)
    {
        if (!Path.HasExtension(filePrefix))
        {
            return filePrefix + extension;
        }

        return filePrefix;
    }

    public static IEnumerable<string> GetFiles(this string filePattern, bool includeDirectories = false, bool createFile = false)
    {
        var fileFinder = new FileFinder(includeDirectories, createFile);
        return fileFinder.GetFiles(filePattern);
    }

    public static bool IsDirectory(this string path)
    {
        if (Directory.Exists(path) || File.Exists(path))
        { 
            FileAttributes fa = File.GetAttributes(path);
            return (fa & FileAttributes.Directory) != 0;
        }

        return false;
    }

}
