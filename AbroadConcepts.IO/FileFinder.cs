using System.IO;
using System.IO.Enumeration;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using Microsoft.VisualBasic;

namespace AbroadConcepts.IO;

public class FileFinder(bool includeDirectories = false, bool createFile = false)
{
    private readonly bool _includeDirectories = includeDirectories;
    private readonly bool _createFile = createFile;

    private static readonly string _driveSeparator = $":{Path.DirectorySeparatorChar}";
    private static readonly string _doubleSeparators = $"{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}";
    private static readonly char[] _searchPattern = ['*', '?'];
    private static readonly string _singleSeparator = Path.DirectorySeparatorChar.ToString();

    public IEnumerable<string> GetFiles(string filePattern)
    {
        foreach (var file in GetBasePatterns(filePattern))
        {
            foreach (var f in ResolveFile(file))
            {
                yield return f;
            }
        }
    }

    public async IAsyncEnumerable<string> GetFilesAsync(string filePattern, [EnumeratorCancellation] CancellationToken ct)
    {
        var lockItem = new object();
        var items = new List<string>();
        var threads = new List<Thread>();
        var filePatterns = GetBasePatterns(filePattern).ToList();
        for (var i = 1; i < filePatterns.Count; i++)
        {
            var filenamePattern = filePatterns[i];
            var t = new Thread(() =>
            {
                foreach (var filename in ResolveFile(filenamePattern))
                {
                    lock (lockItem)
                    {
                        items.Add(filename);
                    }
                }
            });
            threads.Add(t);
            t.Start();
        }

        foreach (var item in ResolveFile(filePatterns[0]))
        {
            yield return item;
        }
        foreach (var t in threads)
        {
            t.Join();
        }
        await Task.Delay(0);

        foreach (var item in items)
        {
            yield return item;
        }
    }


    private static IEnumerable<string> GetBasePatterns(string filePattern)
    {
        filePattern = UpdatePattern(filePattern);
        if (filePattern.IndexOf(_driveSeparator) == 1)
        {
            foreach (var matchingPattern in GetDrivePatterns(filePattern.Substring(1 + _driveSeparator.Length)))
            {
                yield return matchingPattern;
            }
        }
        else if ((filePattern.StartsWith('*') || filePattern.StartsWith('?')) && filePattern.IndexOf(_singleSeparator)  == 1)
        {
            foreach (var matchingPattern in GetDrivePatterns( filePattern.Substring(2)))
            {
                yield return matchingPattern;
            }
        }

        yield return $"{Directory.GetCurrentDirectory()}{_singleSeparator}{filePattern}";

    }

    private static IEnumerable<string> GetDrivePatterns(string filePattern)
    {
        var drives = DriveInfo.GetDrives();

        foreach (var drive in drives)
        {
            if (drive.IsReady)
            {
                yield return drive + filePattern;
            }
        }
    }

    private IEnumerable<string> GetFilesByPattern(string file, string searchPattern, string right = "")
    {
        var lockItem = new object();
        var items = new List<string>();
        var threads = new List<Thread>();
        var dirs = Directory.GetDirectories(file, searchPattern);

        for (var i = 1; i < dirs.Length; i++)
        {
            var dir = dirs[i];
            var t = new Thread(() =>
            {
                foreach (var filename in GetFileByPattern(right, dir))
                {
                    lock(lockItem)
                    {
                        items.Add(filename);
                    }
                }
            });
            threads.Add(t);
            t.Start();
        }

        if (dirs.Length > 0)
        {
            foreach (var filename in GetFileByPattern(right, dirs[0]))
            {
                yield return filename;
            }
        }

        foreach (var t in threads)
        {
            t.Join();
        }

        foreach(var item in items)
        {
            yield return item;
        }

        foreach (var checkFile in Directory.GetFiles(file, searchPattern))
        {
            foreach (var filename in ResolveFile(checkFile))
            {
                yield return filename;
            }
        }

    }

    private IEnumerable<string> GetFileByPattern(string right, string checkFile)
    {
        var dirFile = checkFile;

        if (!string.IsNullOrEmpty(right))
        {
            dirFile += _singleSeparator + right;
        }
        foreach (var directoryFile in ResolveFile(dirFile))
        {
            yield return directoryFile;
        }
    }

    private IEnumerable<string> IncludeFile(string file)
    {
        if (file.IsDirectory())
        {
            yield return file;
            if (_includeDirectories)
            {
                foreach (var subItem in GetFilesByPattern(file, "*"))
                {
                    yield return subItem;
                }
            }
        }
        else if (_createFile || File.Exists(file))
        {
            yield return file;
        }
    }

    private static void ParseFilePattern(string file, out string left, out string pattern, out string right)
    {
        left = pattern = right = string.Empty;
        var patternIndex = file.IndexOfAny(_searchPattern);

        if (patternIndex > -1)
        {
            var fixPartIndex = file.LastIndexOf(_singleSeparator, patternIndex);
            if (fixPartIndex > -1)
            {
                left = file.Substring(0, fixPartIndex);
            }

            var rightIndex = file.IndexOf(_singleSeparator, patternIndex);
            if (rightIndex > -1)
            {
                right = file.Substring(rightIndex + 1);
                pattern = file.Substring(fixPartIndex + 1, rightIndex - (fixPartIndex + 1));
            }
            else
            {
                pattern = file.Substring(fixPartIndex + 1);
            }
        }
        else
        {
            left = file;
        }
    }

    private IEnumerable<string> ResolveFile(string file)
    {
        var searchIndex = file.IndexOfAny(_searchPattern);
        if (searchIndex == -1)
        {
            foreach (var filename in IncludeFile(file))
            {
                yield return filename;
            }
        }
        else
        {
            foreach (var filename in ResolvePattern(file))
            {
                yield return filename;
            }
        }
    }

    private IEnumerable<string> ResolvePattern(string file)
    {
        string left, pattern, right;
        ParseFilePattern(file, out left, out pattern, out right);
        foreach (var filename in GetFilesByPattern(left, pattern, right))
        {
            yield return filename;
        }
    }

    private static string UpdatePattern(string filePattern)
    {
        filePattern = filePattern.Replace(":", _driveSeparator);

        while (filePattern.Contains(_doubleSeparators))
            filePattern = filePattern.Replace(_doubleSeparators, _singleSeparator);
        
        return filePattern;
    }
}
