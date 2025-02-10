using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbroadConcepts.IO;
public static class FileExtension
{

    public static List<string> GetFiles(this string filePattern, bool includeDirectory = false, bool createFile = false)
    {
        string left, pattern, right;
        ParseFilePattern(filePattern, out left, out pattern, out right);
        List<string> list = new List<string>();

        if (string.IsNullOrEmpty(pattern))
        { 
            if (Directory.Exists(left))
            {
                if (includeDirectory)
                {
                    list.Add(left);
                }
                foreach (var dir in Directory.GetDirectories(left))
                {
                    list.AddRange(dir.GetFiles(includeDirectory, createFile));
                }

                foreach (var file in Directory.GetFiles(left))
                {
                    list.Add(file);
                }
            }

            if (File.Exists(left) || createFile)
            {
                list.Add(left);
            }
        }
        else
        {
            foreach (var dir in Directory.GetDirectories(left, pattern))
            {
                var fullPath = dir + Path.DirectorySeparatorChar + right;
                list.AddRange(fullPath.GetFiles(includeDirectory, createFile) );
            }
            foreach(var file in Directory.GetFiles(left, pattern))
            {
                list.AddRange(file.GetFiles(includeDirectory, createFile));
            }
        }

        return list;
    }

    public static string EnsureExtension(this string file, string extension)
    {
        if (!Path.HasExtension(file))
        {
            file += extension;
        }

        return file;
    }

    public static bool IsDirectory(this string path)
    {
        FileAttributes fa = File.GetAttributes(path);
        return (fa & FileAttributes.Directory) != 0;
    }

    private static void ParseFilePattern(string filePattern, out string left, out string pattern, out string right)
    {
        left = pattern = right = string.Empty;
        var patternIndex = filePattern.IndexOfAny(['?', '*']);
        if (patternIndex > -1)
        {
            var fixPartIndex = filePattern.LastIndexOf(Path.DirectorySeparatorChar, patternIndex);
            if (fixPartIndex > -1)
            {
                left = filePattern.Substring(0, fixPartIndex);
            }

            var rightIndex = filePattern.IndexOf(Path.DirectorySeparatorChar, patternIndex);
            if (rightIndex > -1)
            {
                right = filePattern.Substring(rightIndex + 1);
                pattern = filePattern.Substring(fixPartIndex + 1, rightIndex - (fixPartIndex + 1));
            }
            else
            {
                pattern = filePattern.Substring(fixPartIndex + 1);
            }
        }
        else
        {
            left = filePattern;
        }

        if (string.IsNullOrEmpty(left))
        {
            left = Directory.GetCurrentDirectory();
        }
    }

}
