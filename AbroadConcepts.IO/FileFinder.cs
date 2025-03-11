using System.IO.Enumeration;
using System.Runtime.CompilerServices;

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

    public async  IAsyncEnumerable<string> GetFilesAsync(string filePattern,[EnumeratorCancellation] CancellationToken ct)
    {
        foreach (var file in GetBasePatterns(filePattern))
        {
            await foreach (var f in ResolveFileAsync(file, ct))
            {
                yield return f;
            }
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

    private IEnumerable<string> IncludeFile(string file)
    {
        if (file.IsDirectory())
        {
            yield return file;
            if (_includeDirectories)
            {
                foreach (var checkFile in Directory.GetDirectories(file, "*"))
                {
                    foreach (var directoryFile in ResolveFile(checkFile))
                    {
                        yield return directoryFile;
                    }
                }
                foreach (var checkFile in Directory.GetFiles(file, "*"))
                {
                    foreach (var filename in ResolveFile(checkFile))
                    {
                        yield return filename;
                    }
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
                foreach (var checkFile in Directory.GetDirectories(file, "*"))
                {
                    await foreach (var directoryFile in ResolveFileAsync(checkFile, ct))
                    {
                        yield return directoryFile;
                    }
                }
                foreach (var checkFile in Directory.GetFiles(file, "*"))
                {
                    await foreach (var filename in ResolveFileAsync(checkFile,ct))
                    {
                        yield return filename;
                    }
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

    private async IAsyncEnumerable<string> ResolveFileAsync(string file,[EnumeratorCancellation] CancellationToken ct)
    {
        var searchIndex = file.IndexOfAny(_searchPattern);
        if (searchIndex == -1)
        {
            await foreach (var filename in IncludeFileAsync(file, ct))
            {
                yield return filename;
            }
        }
        else
        {
            await foreach (var filename in ResolvePatternAsync (file, ct))
            {
                yield return filename;
            }
        }
    }

    private IEnumerable<string> ResolvePattern(string file)
    {
        string left, pattern, right;
        ParseFilePattern(file, out left, out pattern, out right);
        foreach (var filename in ResolvePattern(left, pattern, right))
        {
            yield return filename;
        }
    }


    private IEnumerable<string> ResolvePattern(string left, string pattern, string right)
    {
        foreach (var file in Directory.GetDirectories(left, pattern))
        {
            var lookFile = file;

            if (!string.IsNullOrEmpty(right))
            {
                lookFile += _singleSeparator + right;
            }

            foreach (var filename in ResolveFile(lookFile))
            {
                yield return filename;
            }
        }

        foreach (var file in Directory.GetFiles(left, pattern))
        {
            foreach (var filename in ResolveFile(file))
            {
                yield return filename;
            }
        }

    }

    private async IAsyncEnumerable<string> ResolvePatternAsync(string file, [EnumeratorCancellation] CancellationToken ct)
    {
        string left, pattern, right;
        ParseFilePattern(file, out left, out pattern, out right);
        await foreach (var filename in ResolvePatternAsync(left, pattern, right, ct))
        {
            yield return filename;
        }
    }

    private async IAsyncEnumerable<string> ResolvePatternAsync(string left, string pattern, string right, [EnumeratorCancellation] CancellationToken ct)
    {
        foreach (var file in Directory.GetDirectories(left, pattern))
        {
            var lookFile = file;

            if (!string.IsNullOrEmpty(right))
            {
                lookFile += _singleSeparator + right;
            }

            await foreach (var filename in ResolveFileAsync (lookFile, ct))
            {
                yield return filename;
            }
        }

        foreach (var file in Directory.GetFiles(left, pattern))
        {
            await foreach (var filename in ResolveFileAsync(file, ct))
            {
                yield return filename;
            }
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
