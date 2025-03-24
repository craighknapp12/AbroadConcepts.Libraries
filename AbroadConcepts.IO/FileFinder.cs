using System.Collections.Generic;
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
        foreach (var possibleFilePattern in GetBasePatterns(filePattern))
        {
            foreach (var f in ResolveFile(possibleFilePattern))
            {
                yield return f;
            }
        }
    }

    public async IAsyncEnumerable<string> GetFilesAsync(string filePattern, [EnumeratorCancellation] CancellationToken ct)
    {
        List<IAsyncEnumerable<string>> tasks = new List<IAsyncEnumerable<string>> ();

        foreach (var possibleFilePattern in GetBasePatterns(filePattern))
        {
            tasks.Add(ResolveFileAsync(possibleFilePattern, ct));
        }

        foreach (var t in tasks)
        { 
            await foreach (var f in t)
            {
                yield return f;
            }
        }
    }


    private static IEnumerable<string> GetBasePatterns(string filePattern)
    {
        filePattern = UpdatePattern(filePattern);
        if ((filePattern.StartsWith('*') || filePattern.StartsWith('?')) && filePattern.IndexOf(_driveSeparator) == 1)
        {
            foreach (var matchingPattern in GetDrivePatterns(filePattern.Substring(3)))
            {
                yield return matchingPattern;
            }
        }
        else if ((filePattern.StartsWith('*') || filePattern.StartsWith('?')) && filePattern.IndexOf(_singleSeparator) == 1)
        {
            foreach (var matchingPattern in GetDrivePatterns(filePattern.Substring(2)))
            {
                var pattern = UpdatePattern(matchingPattern);
                yield return pattern;
            }
        }
        else if (filePattern.IndexOf(_driveSeparator) == 1)
        {
            yield return filePattern;
        }


        if (filePattern.IndexOf(_driveSeparator) == -1)
        {
            yield return $"{Directory.GetCurrentDirectory()}{_singleSeparator}{filePattern}";
        }
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
        var dirs = Directory.GetDirectories(file, searchPattern);
        foreach (var dir in dirs)
        {
            foreach (var filename in GetFileByPattern(dir, right))
            {
                yield return filename;
            }
        }

        foreach (var checkFile in Directory.GetFiles(file, searchPattern))
        {
            foreach (var filename in ResolveFile(checkFile))
            {
                yield return filename;
            }
        }

    }
    private async IAsyncEnumerable<string> GetFilesByPatternAsync(string file, string searchPattern, [EnumeratorCancellation] CancellationToken ct, string right = "")
    {
        var dirs = Directory.GetDirectories(file, searchPattern);
        var files = Directory.GetFiles(file, searchPattern);
        List<IAsyncEnumerable<string>> tasks = new List<IAsyncEnumerable<string>>();

        foreach (var dir in dirs)
        {
            tasks.Add(GetFileByPatternAsync(dir, right, ct));
        }

        foreach (var checkFile in files)
        {
            tasks.Add(ResolveFileAsync(checkFile, ct));
        }

        foreach (var t in tasks)
        { 
            await foreach (var filename in t)
            {
                yield return filename;
            }
        }

    }


    private IEnumerable<string> GetFileByPattern(string checkFile, string right)
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

    private async IAsyncEnumerable<string> GetFileByPatternAsync(string checkFile, string right, [EnumeratorCancellation] CancellationToken ct)
    {
        var dirFile = checkFile;

        if (!string.IsNullOrEmpty(right))
        {
            dirFile += _singleSeparator + right;
        }

        await foreach (var directoryFile in ResolveFileAsync(dirFile, ct))
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


    private async IAsyncEnumerable<string> IncludeFileAsync(string file, [EnumeratorCancellation] CancellationToken ct)
    {
        if (file.IsDirectory())
        {
            yield return file;
            if (_includeDirectories)
            {
                await foreach (var subItem in GetFilesByPatternAsync(file, "*", ct))
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

    private IEnumerable<string> ResolveFile(string filePattern)
    {
        var searchIndex = filePattern.IndexOfAny(_searchPattern);
        if (searchIndex == -1)
        {
            foreach (var filename in IncludeFile(filePattern))
            {
                yield return filename;
            }
        }
        else
        {
            foreach (var filename in ResolveFilePattern(filePattern))
            {
                yield return filename;
            }
        }
    }

    private async IAsyncEnumerable<string> ResolveFileAsync(string filePattern,[EnumeratorCancellation] CancellationToken ct)
    {
        var searchIndex = filePattern.IndexOfAny(_searchPattern);
        if (searchIndex == -1)
        {
            await foreach (var filename in IncludeFileAsync(filePattern, ct))
            {
                yield return filename;
            }
        }
        else
        {
            await foreach (var filename in ResolveFilePatternAsync(filePattern, ct))
            {
                yield return filename;
            }
        }
    }

    private IEnumerable<string> ResolveFilePattern(string filePattern)
    {
        string left, pattern, right;
        ParseFilePattern(filePattern, out left, out pattern, out right);
        foreach (var filename in GetFilesByPattern(left, pattern, right))
        {
            yield return filename;
        }
    }

    private async IAsyncEnumerable<string> ResolveFilePatternAsync(string filePattern, [EnumeratorCancellation] CancellationToken ct)
    {
        string left, pattern, right;
        ParseFilePattern(filePattern, out left, out pattern, out right);
        await foreach (var filename in GetFilesByPatternAsync(left, pattern, ct, right))
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
